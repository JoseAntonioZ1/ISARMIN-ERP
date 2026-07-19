# DECISIONS.md

# Registro de Decisiones Arquitectónicas (ADR)

Este documento registra las principales decisiones técnicas tomadas durante el desarrollo del proyecto.

---

## ADR-001

### Decisión

Utilizar PostgreSQL como motor de base de datos.

### Justificación

- Open Source.
- Excelente rendimiento.
- Escalable.
- Compatible con Windows y Linux.
- Gran comunidad.

Estado: ✅ Aceptada

---

## ADR-002

### Decisión

Desarrollar una aplicación Web.

### Justificación

- No requiere instalación en los equipos cliente.
- Fácil mantenimiento.
- Acceso desde cualquier navegador.

Estado: ✅ Aceptada

---

## ADR-003

### Decisión

Implementar una arquitectura basada en Clean Architecture.

### Justificación

- Bajo acoplamiento.
- Alta mantenibilidad.
- Escalabilidad.

Estado: ✅ Aceptada

---

## ADR-004

### Decisión

Implementar autenticación mediante JWT.

### Justificación

- Estándar ampliamente utilizado.
- Compatible con APIs REST.
- Escalable.

Estado: ✅ Aceptada

---

## ADR-005

### Decisión

Desarrollar el sistema utilizando React + ASP.NET Core.

### Justificación

- Alto rendimiento.
- Ecosistema moderno.
- Amplia documentación.

Estado: ✅ Aceptada

---

## ADR-006

### Decisión

Organizar `Application` (y, en espejo, el frontend) por módulo de negocio (feature folders: Usuarios, Clientes, Inventario, Compras, Ventas, Caja, Taller, Servicios de Campo, Gestión Documental, Reportes, Configuración), no por tipo técnico.

### Justificación

- Refleja directamente los límites de dominio de `Architecture-Overview.md` (sección 5).
- Evita una carpeta `Services/` monolítica con decenas de archivos sin relación aparente entre sí.
- Facilita ubicar todo el código de un módulo en un solo lugar al mantenerlo.

Estado: ✅ Aceptada

---

## ADR-007

### Decisión

Las entidades transversales que se originan en más de un módulo (`SaldoPendiente`, `MovimientoInventario`, `MovimientoCaja`, `DocumentoAdjunto`) se modelan con **claves foráneas opcionales** hacia cada posible origen (garantizando exactamente una no nula mediante restricción `CHECK`), en lugar de herencia (TPH/TPT) o una referencia polimórfica genérica sin integridad referencial.

### Justificación

- Directriz explícita del propietario (2026-07-18): priorizar una solución simple y mantenible, compatible con PostgreSQL/Entity Framework Core.
- Mantiene claves foráneas reales (integridad referencial de PostgreSQL), a diferencia de una columna genérica "tipo + id".
- `Auditoria` es la única excepción intencional (ver `Architecture-Overview.md`, sección 7.3): al ser un log transversal a *cualquier* entidad, usa referencia genérica sin integridad declarativa, que es el patrón estándar para bitácoras.

Estado: ✅ Aceptada

---

## ADR-008

### Decisión

La autorización se basa en **permisos lógicos configurables** (ej. `"Inventario.Ajustar"`) resueltos dinámicamente desde las tablas `Rol`/`Permiso`, no en atributos `[Authorize(Roles = "...")]` con nombres de rol fijos en el código.

### Justificación

- Consecuencia directa de RN-028 (roles y permisos configurables por el Administrador, sin cambios de código para crear un rol nuevo). Usar roles fijos en atributos de autorización violaría esa regla en la práctica, aunque el catálogo de roles fuera configurable en la base de datos.

Estado: ✅ Aceptada

---

## ADR-009

### Decisión

La auditoría transversal (RF-078/RF-079) se implementa mediante un interceptor de `SaveChanges`/`SaveChangesAsync` en el `DbContext` de Entity Framework Core, no mediante llamadas manuales desde cada módulo de Application.

### Justificación

- Evita código duplicado (`AI_INSTRUCTIONS.md`): ningún módulo necesita invocar explícitamente "registrar auditoría" en cada operación crítica.
- Reduce el riesgo de que un desarrollador olvide auditar una acción crítica en un módulo nuevo.

Estado: ✅ Aceptada

---

## ADR-010

### Decisión

El almacenamiento de archivos (Gestión Documental, RF-084 a RF-087) se abstrae detrás de una interfaz `IAlmacenamientoArchivos` en `Application`, con una única implementación en V1 (sistema de archivos local, en Infrastructure).

### Justificación

- Consistente con RNF-007/RNF-025: la base de datos y el almacenamiento principal deben ser locales, sin depender de servicios cloud.
- Deja un punto de extensión explícito para que un servicio cloud (ej. Firebase) se agregue en el futuro como almacenamiento secundario, sin modificar `Application` ni `Domain` — solo agregando una nueva implementación en `Infrastructure`.

Estado: ✅ Aceptada

---

## ADR-011

### Decisión

La capa `Application` se organiza mediante **CQRS ligero**: cada caso de uso de escritura es un `Command` con su `Handler`, cada caso de uso de lectura es una `Query` con su `Handler`. Sin librería de mediación (no se adopta MediatR); los controladores de `API` resuelven los handlers directamente por inyección de dependencias.

### Justificación

- Recomendación explícita del propietario (2026-07-19).
- Separa claramente operaciones que cambian estado (pasan por el modelo de dominio rico) de operaciones de solo lectura (pueden proyectar directamente a DTOs, sin cargar entidades completas), sin la complejidad operativa de un CQRS completo (bases separadas, *event sourcing*) que este proyecto no necesita (RNF-005: 5–20 usuarios concurrentes).
- Evita agregar una dependencia nueva (MediatR) no evaluada en `TECH_STACK.md`; puede incorporarse después sin romper la estructura, si el crecimiento del proyecto lo justifica.

Estado: ✅ Aceptada

---

## ADR-012

### Decisión

Se separan dos correcciones al diseño de datos transversales registrado en ADR-007:

1. **Cobranzas** (`SaldoPendiente`) se modela como módulo de `Application` independiente de **Caja** (`Caja`, `MovimientoCaja`) — Cobranzas registra la obligación de pago; Caja registra el movimiento físico del dinero cuando esa obligación se cobra.
2. **MovimientoInventario** (Kardex) **no** sigue el patrón de claves foráneas opcionales de ADR-007. Solo `ProductoId` es una FK fuerte; su origen (Compra, Venta, OrdenTrabajo, ServicioCampo, Ajuste) se registra como dato informativo (`origen_tipo` + `origen_id`) sin FK declarativa — el mismo patrón que `Auditoria`.

### Justificación

- Corrección explícita del propietario (2026-07-19): evitar que Caja y Cobranzas se mezclen en un solo módulo, y evitar demasiadas columnas de clave foránea opcional en `MovimientoInventario` (hasta 6 motivos posibles en CAT-013, frente a los 3 orígenes de `SaldoPendiente`).
- El criterio general para decidir entre "FK opcionales" y "referencia genérica sin FK" queda documentado en `Architecture-Overview.md` (sección 7): depende de si la entidad es una relación de dominio activa con pocos orígenes (FK fuerte) o un registro histórico/log con muchos orígenes posibles (referencia informativa).

Estado: ✅ Aceptada

---

## ADR-013

### Decisión

Convenciones físicas del esquema PostgreSQL (`04-Database/Physical-Data-Model.md`):
- Nombres de tablas/columnas en `snake_case`, traducidos automáticamente desde las clases `PascalCase` de C# mediante el paquete `EFCore.NamingConventions`.
- Claves primarias `uuid` (`gen_random_uuid()`), no `serial`/`bigserial`.
- Catálogos configurables (roles, medios de pago, categorías) como tablas de referencia; catálogos fijos de una máquina de estados (estado de OT, de Venta, de Servicio de Campo, motivo de movimiento de inventario) como `varchar` + `CHECK`, no `ENUM` nativo de PostgreSQL.

### Justificación

- `snake_case`: evita tener que citar identificadores en SQL manual; convención estándar de PostgreSQL.
- `uuid`: permite generar IDs en el cliente sin depender del servidor, lo cual es relevante para el punto de extensión ya documentado de una futura app móvil/offline para Servicios de Campo (RF-071) — no es una complejidad añadida sin motivo, responde a un requerimiento futuro ya identificado.
- Catálogos configurables vs. fijos: coherente con RN-028 (roles configurables) y con la distinción ya usada en ADR-007/ADR-012 entre relaciones activas y registros controlados por la máquina de estados de `Application`.
- `CHECK` en vez de `ENUM` nativo: más simple de modificar en una migración futura, consistente con el criterio de simplicidad ya aplicado en ADR-007/ADR-012.

Estado: ✅ Aceptada

---

## ADR-014

### Decisión

Principio general para reglas ligadas a normativa peruana (SUNAT/RENIEC): las **reglas legales duras** (ej. una Factura exige RUC) se implementan en `Domain`, porque son ley, no una preferencia de ISARMIN. Los **valores que la norma puede cambiar periódicamente** (montos umbral, series/correlativos) se implementan como **datos de configuración** administrables por el Administrador, nunca como constantes en el código.

### Justificación

- Evita que un cambio normativo (ej. SUNAT actualiza el monto que exige documento de identidad en una Boleta) obligue a una recompilación del sistema.
- Establece un criterio consistente y reutilizable para cualquier regla futura de este tipo (ej. al implementar la integración SUNAT diferida, BQ-050), en vez de decidir caso por caso sin un principio declarado.
- Aplicado por primera vez en RN-009 (regla dura) y RN-035 (configuración) de `Business-Rules.md`, análisis del 2026-07-19.

Estado: ✅ Aceptada
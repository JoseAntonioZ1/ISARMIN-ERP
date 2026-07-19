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
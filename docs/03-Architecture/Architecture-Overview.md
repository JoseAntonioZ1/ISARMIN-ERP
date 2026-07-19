# Architecture-Overview.md — Arquitectura del Sistema (Fase 3: Diseño)

## 1. Propósito

Define la arquitectura de ISARMIN ERP: capas, estructura de solución, límites de módulos (bounded contexts) y patrones transversales. Es la traducción directa de [Conceptual-Data-Model.md](../01-Requirements/Conceptual-Data-Model.md) y [Use-Cases.md](../01-Requirements/Use-Cases.md) a una estructura de software concreta, respetando las decisiones ya tomadas en [TECH_STACK.md](../00-Project/TECH_STACK.md) y [DECISIONS.md](../00-Project/DECISIONS.md) (ADR-001 a ADR-005).

Ninguna decisión de este documento contradice una regla de negocio. Donde una decisión de arquitectura requeriría interpretar una regla no documentada, se marca explícitamente **[PENDIENTE]** en vez de asumirse.

## 2. Principios rectores

1. **Regla de dependencia de Clean Architecture:** las capas internas (Domain) no conocen a las externas (Infrastructure, API). Toda dependencia apunta hacia adentro.
2. **SOLID y Clean Code**, conforme a `CODING_STANDARDS.md`.
3. **Roles y catálogos configurables, no fijos en código** (RN-028): la arquitectura no debe usar enums de rol hardcodeados para autorización (ver sección 8).
4. **Extensibilidad sin romper la arquitectura principal**: los puntos de extensión previstos (app móvil/offline para Campo, lectura de código de barras, multi-sucursal, nube como almacenamiento secundario) se habilitan mediante abstracciones (interfaces), no se construyen ahora.
5. **Nada de lo excluido explícitamente de V1 se incluye en esta arquitectura** (sección 12).

## 3. Vista de capas

```mermaid
flowchart TB
    API["ISARMIN.API — Presentation<br/>Controllers, Middleware, Auth JWT"]
    APP["ISARMIN.Application<br/>Casos de uso, DTOs, Validadores (FluentValidation),<br/>Interfaces de repositorio y servicios externos"]
    DOM["ISARMIN.Domain<br/>Entidades, Value Objects, Reglas de negocio invariantes,<br/>Interfaces de dominio — SIN dependencias externas"]
    INFRA["ISARMIN.Infrastructure<br/>EF Core + Npgsql, Repositorios concretos,<br/>Almacenamiento de archivos, PDF (QuestPDF), Logging (Serilog)"]

    API --> APP
    APP --> DOM
    INFRA --> APP
    INFRA --> DOM
    API -.inyecta implementaciones de.-> INFRA
```

| Capa | Responsabilidad | No debe contener |
|---|---|---|
| **Domain** | Entidades (ver `Conceptual-Data-Model.md`), invariantes de negocio que no dependen de infraestructura (ej. "una OT debe tener un cliente" — RN-004). | Referencias a EF Core, ASP.NET, PostgreSQL. |
| **Application** | Casos de uso (uno por cada `UC-XX` de `Use-Cases.md`), orquestación, DTOs, validación de entrada, interfaces (`IProductoRepository`, `IAlmacenamientoArchivos`, etc.). | Implementación concreta de acceso a datos. |
| **Infrastructure** | Implementación de las interfaces de Application/Domain: EF Core `DbContext`, repositorios, servicio de archivos, generación de PDF, envío a impresora. | Lógica de negocio. |
| **API** | Controladores REST, autenticación JWT, autorización, mapeo HTTP ↔ Application (DTOs), Swagger. | Lógica de negocio y acceso a datos directo. |

## 4. Estructura de la solución .NET

```
ISARMIN.sln
├── src/
│   ├── ISARMIN.Domain/
│   │   ├── Entities/            (una carpeta por módulo, ver sección 5)
│   │   ├── Enums/                (catálogos confirmados: TipoMovimientoInventario, MedioPago, etc.)
│   │   └── Common/                (clase base Entity, interfaces de dominio compartidas)
│   ├── ISARMIN.Application/
│   │   ├── Modulos/
│   │   │   ├── Usuarios/
│   │   │   ├── Clientes/
│   │   │   ├── Inventario/
│   │   │   ├── Compras/
│   │   │   ├── Ventas/
│   │   │   │   ├── Commands/      (ej. RegistrarVentaCommand + Handler)
│   │   │   │   └── Queries/        (ej. ConsultarVentaQuery + Handler, DTOs de solo lectura)
│   │   │   ├── Cobranzas/
│   │   │   ├── Caja/
│   │   │   ├── Taller/
│   │   │   ├── ServiciosCampo/
│   │   │   ├── GestionDocumental/
│   │   │   ├── Reportes/            (solo Queries — no tiene Commands propios)
│   │   │   └── Configuracion/
│   │   └── Common/                (comportamiento transversal: interfaces ICommandHandler/IQueryHandler)
│   ├── ISARMIN.Infrastructure/
│   │   ├── Persistence/           (DbContext, configuraciones EF Core, migraciones)
│   │   ├── Repositories/
│   │   ├── Storage/                (IAlmacenamientoArchivos → implementación local)
│   │   ├── Reporting/               (QuestPDF)
│   │   └── Auditing/                 (interceptor de auditoría)
│   └── ISARMIN.API/
│       ├── Controllers/            (uno por módulo)
│       ├── Middleware/
│       └── Program.cs
└── tests/                             (fuera de alcance de este documento — Fase 4/07-Development)
```

Esta estructura sigue el patrón de **"Application organizada por módulo/feature"** en lugar de por tipo técnico (no hay una carpeta `Services/` genérica con 40 archivos) — es más mantenible a largo plazo y refleja directamente los límites de dominio de la sección 5.

### 4.1 Patrón de Application: CQRS ligero (corrección 2026-07-19)

Dentro de cada módulo de `Application`, toda operación se modela como un **Command** (escritura) o una **Query** (lectura), nunca como un "Service" genérico con métodos mixtos:

- **Command:** representa una intención de cambio (`RegistrarVentaCommand`, `EntregarEquipoCommand`, `AjustarInventarioCommand` — uno por cada Caso de Uso de escritura en `Use-Cases.md`). Su `Handler` carga las entidades de `Domain` necesarias, aplica las invariantes de negocio (ej. RN-003, RN-008) y persiste vía Repository/Unit of Work.
- **Query:** representa una intención de lectura (`ConsultarStockQuery`, `ConsultarHistorialEquipoQuery`). Su `Handler` puede consultar directamente contra `DbContext` (proyecciones `AsNoTracking` a DTOs) **sin pasar por el modelo de dominio rico** — una lectura no necesita reconstruir invariantes de escritura, solo devolver datos.

**Por qué "ligero" y no CQRS completo:** una sola base de datos (PostgreSQL), sin réplicas de lectura separadas, sin *event sourcing*. La separación es a nivel de código (Application), no de infraestructura — apropiada para el tamaño real de la operación (RNF-005: 5–20 usuarios concurrentes), sin la complejidad operativa de un CQRS completo que este proyecto no necesita.

**Despacho sin librería de mediación:** los controladores de `API` resuelven `ICommandHandler<TCommand, TResultado>` / `IQueryHandler<TQuery, TResultado>` directamente por inyección de dependencias (constructor). No se agrega MediatR ni otra librería de *pipeline* — mantiene el patrón "ligero" sin una dependencia nueva no evaluada en `TECH_STACK.md`. Si el proyecto crece y se justifica un pipeline con comportamientos transversales (ej. logging, validación automática por *pipeline*), MediatR podría incorporarse después sin romper esta estructura, ya que los Commands/Queries ya están modelados como clases independientes.

## 5. Límites de módulos (Bounded Contexts)

Cada módulo de negocio (`PROJECT_SCOPE.md`) se traduce en una carpeta/namespace propio dentro de `Domain` y `Application`. Un módulo **no** referencia directamente el `DbContext` de otro — solo sus interfaces expuestas.

| Módulo | Entidades que posee | Depende de |
|---|---|---|
| **Identidad y Acceso** | Usuario, Rol, Permiso, UsuarioRol | — (módulo base) |
| **Terceros** | Cliente, Proveedor | Identidad (auditoría) |
| **Catálogo e Inventario** *(shared kernel)* | Producto, Categoria, MovimientoInventario | Identidad |
| **Compras** | Compra, CompraDetalle | Terceros, Catálogo e Inventario |
| **Ventas** | Venta, VentaDetalle, PagoVenta | Terceros, Catálogo e Inventario, Cobranzas |
| **Taller** | OrdenTrabajo, Diagnostico, CotizacionReparacion, ConsumoRepuesto, Garantia | Terceros, Catálogo e Inventario, Cobranzas |
| **Servicios de Campo** | ServicioCampo, ServicioCampoDetalle | Terceros, Catálogo e Inventario, Cobranzas |
| **Cobranzas** *(módulo propio — separado de Caja, ver nota abajo)* | SaldoPendiente | Identidad (quién autoriza) |
| **Caja** | Caja, MovimientoCaja | Identidad, Cobranzas (al registrar el cobro de un saldo pendiente) |
| **Gestión Documental** *(shared kernel)* | DocumentoAdjunto | — (referenciado por Compras, Ventas, Taller, Campo) |
| **Reportes** | (sin entidades propias; consulta de otros módulos) | Todos (solo lectura) |
| **Auditoría** *(transversal, no es un módulo con lógica de negocio)* | Auditoria | Se implementa como interceptor en Infrastructure, no como módulo de Application (ver sección 9) |
| **Configuración** | (parámetros generales, series de comprobantes) | — |
| **ParticipacionTemporal** | Vive dentro de Taller y Servicios de Campo (no es módulo propio) | Taller, Servicios de Campo |

**Nota — Cobranzas separado de Caja (corrección 2026-07-19):** aunque ambos manejan dinero, son conceptos distintos: **Cobranzas** modela una obligación de pago pendiente (quién debe, cuánto, quién lo autorizó — RN-001/RN-031), mientras que **Caja** modela el movimiento físico de efectivo/medios de pago del negocio (RN-025). Ventas, Taller y Servicios de Campo dependen de Cobranzas para dejar un saldo pendiente; cuando ese saldo se cobra (UC-21), Cobranzas genera un `MovimientoCaja` — por eso Caja depende de Cobranzas, y no al revés. Antes se habían bundleado como un solo módulo ("Caja incluye Cobranzas"), lo cual mezclaba dos responsabilidades distintas dentro de la misma capa de Application.

**Nota sobre "Catálogo e Inventario" como shared kernel:** dado que RN-006 exige un inventario único compartido entre Tienda, Taller y Campo, este módulo es intencionalmente compartido — Ventas, Taller y Servicios de Campo dependen de él para consumir stock, pero **nunca se duplica** la lógica de descuento de inventario en cada uno (violaría "no generes código duplicado" de `AI_INSTRUCTIONS.md`). El descuento de stock se expone como un servicio único de Application (`IServicioInventario.RegistrarMovimiento(...)`), invocado desde Ventas/Taller/Campo.

## 6. Diagrama de dependencias entre módulos

```mermaid
flowchart LR
    Identidad --> Terceros
    Identidad --> Inventario["Catálogo e Inventario"]
    Terceros --> Compras
    Inventario --> Compras
    Terceros --> Ventas
    Inventario --> Ventas
    Cobranzas --> Ventas
    Terceros --> Taller
    Inventario --> Taller
    Cobranzas --> Taller
    Terceros --> Campo["Servicios de Campo"]
    Inventario --> Campo
    Cobranzas --> Campo
    Cobranzas -.genera movimiento al cobrar.-> Caja
    Ventas -.adjunta.-> GestionDocumental
    Compras -.adjunta.-> GestionDocumental
    Taller -.adjunta.-> GestionDocumental
    Campo -.adjunta.-> GestionDocumental
```

## 7. Patrón de datos transversales — resolución concreta

No todas las entidades transversales se resuelven igual. El criterio para elegir el patrón es: **¿es una relación de dominio activa que necesita integridad referencial fuerte (pocos orígenes posibles, consultada por su origen), o es fundamentalmente un registro histórico/log (muchos orígenes posibles, consultado casi siempre por su sujeto principal, no por su origen)?**

### 7.1 SaldoPendiente (módulo Cobranzas) — FK opcionales
Tabla única con **tres claves foráneas opcionales**: `VentaId`, `OrdenTrabajoId`, `ServicioCampoId`. Se garantiza que **exactamente una** esté presente mediante:
- Una restricción `CHECK` a nivel de PostgreSQL (vía migración de EF Core con SQL crudo o `HasCheckConstraint`, disponible desde EF Core 7).
- Validación adicional en Application (FluentValidation) antes de persistir, como capa de defensa adicional (no reemplaza el CHECK de base de datos).

Se justifica el FK fuerte aquí porque es una **obligación financiera activa** (se consulta, se actualiza, se cierra — RN-001/RN-031) con solo 3 orígenes posibles. Evita `TPH`/`TPT` (herencia de tablas) y evita una referencia polimórfica sin integridad real.

### 7.2 MovimientoCaja y DocumentoAdjunto — mismo patrón de FK opcionales
Igual razonamiento que SaldoPendiente, con un número de orígenes igualmente acotado (`VentaId`, `CompraId`, `OrdenTrabajoId`, `ServicioCampoId`, o ninguno para un gasto operativo directo). Se reconcilian (RN-015) y se consultan por su origen con frecuencia — justifica mantener la integridad referencial declarativa.

### 7.3 MovimientoInventario (Kardex) — referencia genérica, no FK por cada origen (corrección 2026-07-19)
**Se descarta el patrón de FK opcionales para esta entidad.** El Kardex tiene hasta 6 motivos posibles (Compra, Venta, ConsumoTaller, ConsumoCampo, Ajuste, Devolución — CAT-013), lo que habría requerido demasiadas columnas nulas para un beneficio marginal. En su lugar:
- **Única FK obligatoria y fuerte:** `ProductoId` — es la relación que realmente importa para el negocio (RN-002: todo movimiento debe quedar registrado *contra un producto*).
- **Origen informativo, sin integridad declarativa:** `origen_tipo` (catálogo CAT-013) + `origen_id` (identificador simple, sin `FOREIGN KEY` en el esquema) — el mismo patrón que `Auditoria` (sección 7.4), porque el Kardex es, en esencia, **un log histórico del producto**, no una relación de dominio que se actualice o cierre como `SaldoPendiente`.
- Si en el futuro se necesita trazar "todos los movimientos que generó la Venta X", se resuelve por consulta (`WHERE origen_tipo = 'Venta' AND origen_id = X`), igual que ya se hace con `Auditoria` — no requiere una FK declarativa para ser útil.

### 7.4 Auditoria — referencia genérica (sin cambios)
Por su volumen y por registrar **cualquier** entidad del sistema, usa una referencia genérica (`EntidadTipo` + `EntidadId`), **sin integridad referencial declarativa** — es un log, no una relación de negocio.

### 7.5 Definición de códigos de producto (corrección 2026-07-19)

Para evitar ambigüedad de aquí en adelante (ver también [Glossary.md](../01-Requirements/Glossary.md)):

| Campo | Definición | Origen |
|---|---|---|
| **Código Interno** (`codigo_interno`) | Identificador propio que ISARMIN asigna a cada producto al registrarlo. **Obligatorio.** Se ingresa manualmente por el Administrador como parte del registro manual de productos (RF-023) — el sistema no lo autogenera en V1 (no se documentó esa necesidad; si se requiere autogeneración, es una mejora futura a validar). | RF-023 |
| **Código de Barras Comercial** (`codigo_barras`) | Código de fábrica del producto (ej. EAN-13, UPC), tal como viene impreso en el empaque. **Opcional.** Se captura si el producto lo trae; no se genera desde el sistema. Preparado para una futura lectura por escáner (fuera de alcance de V1). | RF-023, BQ-056 (resuelta) |

Ambos son atributos de `Producto` (`Catálogo e Inventario`), no catálogos ni entidades separadas.

## 8. Autenticación y Autorización

- **Autenticación:** JWT (ADR-004), emitido tras validar credenciales contra `Usuario` (hash seguro, RNF-011).
- **Autorización — decisión clave derivada de RN-028:** dado que los roles son configurables (no fijos en código), **no se usará** `[Authorize(Roles = "Administrador")]` de ASP.NET Core de forma literal (eso asumiría roles fijos). En su lugar:
  - Cada endpoint se protege con un **permiso lógico** (ej. `"Inventario.Ajustar"`, `"Taller.Entregar"`), no con un nombre de rol.
  - Al autenticarse, el token JWT incluye los permisos efectivos del usuario (resueltos desde `Rol` → `Permiso` en ese momento).
  - Un `AuthorizationHandler` personalizado de ASP.NET Core valida el permiso lógico contra los claims del token.
  - Esto permite que el Administrador cree un rol nuevo mañana (ej. "Caja") y le asigne permisos, **sin recompilar el backend** — cumple RN-028 literalmente, no solo en apariencia.

## 9. Auditoría transversal (sin duplicar código por módulo)

Se implementa mediante un **interceptor de `SaveChanges`/`SaveChangesAsync`** en el `DbContext` de EF Core (Infrastructure), que:
1. Antes de guardar, inspecciona las entidades rastreadas con estado `Added`, `Modified` o `Deleted` (esta última se traduce a baja lógica, RN-023).
2. Genera automáticamente un registro en `Auditoria` con usuario (del contexto de autenticación), fecha, hora, acción y entidad afectada (RF-078).
3. Ningún módulo de Application necesita invocar manualmente "registrar auditoría" — se cumple RF-078/079 de forma transversal y sin código repetido.

## 10. Gestión Documental y almacenamiento de archivos

- Interfaz `IAlmacenamientoArchivos` en Application (`Guardar`, `Obtener`, `Eliminar`).
- Implementación V1 en Infrastructure: **sistema de archivos local** (carpeta en el servidor), consistente con RNF-007 (base de datos principal local, sin depender de Firebase/cloud).
- Punto de extensión explícito para el futuro: una segunda implementación de `IAlmacenamientoArchivos` (ej. Firebase Storage) podría añadirse **sin tocar Application ni Domain**, solo registrando una nueva implementación en Infrastructure — consistente con RNF-025 (cloud solo como almacenamiento secundario, nunca obligatorio).

## 11. Frontend (React 19 + TypeScript)

Estructura por módulo, espejando el backend (facilita que un mismo desarrollador entienda ambos lados):

```
src/
├── modules/
│   ├── usuarios/
│   ├── clientes/
│   ├── inventario/
│   ├── compras/
│   ├── ventas/
│   ├── caja/
│   ├── taller/
│   ├── servicios-campo/
│   └── configuracion/
├── shared/
│   ├── components/        (UI compartida)
│   ├── hooks/
│   └── api/                  (cliente HTTP, TanStack Query)
└── App.tsx
```

- **TanStack Query** para estado del servidor (listas, detalle) — evita duplicar lógica de caché manual.
- **Zustand** para estado de UI global (sesión actual, permisos efectivos del usuario para mostrar/ocultar acciones en pantalla — reflejo en el frontend de la autorización configurable del backend, sección 8).
- **React Hook Form + Zod** para formularios y validación en cliente, espejando (no reemplazando) la validación de FluentValidation en el backend (RNF: nunca confiar solo en el frontend, `CODING_STANDARDS.md`).

## 12. Explícitamente fuera de esta arquitectura (V1)

Confirmado por el propietario, no se diseña ni se deja como trabajo parcial:
- Aplicación móvil dedicada.
- Sincronización offline.
- Firma digital / evidencias avanzadas desde dispositivos móviles.
- Flujos complejos de aprobación no requeridos (ej. Orden de Compra formal con aprobación — RF-037, futuro).
- Dependencia obligatoria de servicios cloud.

Estos puntos **sí** tienen un lugar reservado para conectarse después sin rediseño: la API REST ya es consumible por cualquier cliente futuro (incluida una app móvil), y `IAlmacenamientoArchivos` ya está abstraído (sección 10).

## 13. Puntos que la Arquitectura decide ahora (no requieren volver a preguntar al negocio)

Estas son decisiones de **implementación técnica**, no de negocio — quedan resueltas en este documento sin necesitar validación adicional del propietario:
- Patrón de FKs opcionales para SaldoPendiente, MovimientoCaja y DocumentoAdjunto; referencia genérica (sin FK declarativa) para MovimientoInventario y Auditoria (sección 7).
- Cobranzas como módulo independiente de Caja (sección 5).
- CQRS ligero como patrón de Application, sin librería de mediación (sección 4.1).
- Autorización basada en permisos, no en roles fijos (sección 8).
- Auditoría vía interceptor, no manual (sección 9).
- Abstracción de almacenamiento de archivos (sección 10).

## 14. Siguiente paso

Con esta arquitectura definida, el siguiente entregable natural de la Fase 3 (`PROJECT_ROADMAP.md`) es el **Modelo Entidad-Relación físico** (`04-Database/`), traduciendo `Data-Dictionary.md` + sección 7 de este documento a tablas, tipos de PostgreSQL y migraciones de EF Core.

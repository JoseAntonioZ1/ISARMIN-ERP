# Functional-Requirements.md — Requerimientos Funcionales

## 1. Propósito

Este documento especifica los requerimientos funcionales (RF) de ISARMIN ERP, siguiendo la estructura recomendada por **IEEE 830-1998** (Software Requirements Specification) y complementada con atributos de priorización propios de **BABOK v3** (Requirements Life Cycle Management).

Cada requerimiento describe **qué debe hacer el sistema**, no cómo debe construirse (eso corresponde a la fase de arquitectura, fuera del alcance de este documento).

## 2. Convención

| Marca | Significado |
|---|---|
| **[C] Confirmado** | El requerimiento está explícitamente respaldado por la documentación fuente del proyecto. |
| **[I] Inferido** | El módulo/funcionalidad está declarado en el alcance del proyecto, pero el detalle específico del requerimiento es una deducción razonable del analista. |
| **[PV] Pendiente de Validación** | Hipótesis de trabajo sin base documental directa; debe confirmarse con el cliente antes de tomarse como definitiva. |

**Prioridad:** Alta (necesario para el alcance inicial v1 declarado en `PROJECT_CONTEXT.md`) / Media (mejora relevante, no bloqueante para v1) / Baja (deseable, evaluable a futuro).

Cada fila indica el **Actor principal** (ver [Actors.md](Actors.md)) y, cuando corresponde, una referencia cruzada a una regla de negocio (`RN-XXX`, ver [Business-Rules.md](Business-Rules.md)) o a una pregunta pendiente (`BQ-XXX`, ver [Business-Questions.md](../02-Business/Business-Questions.md)).

## 3. Nota de trazabilidad con la versión anterior

La versión previa de este documento contenía únicamente seis requerimientos (RF-001 a RF-006), correspondientes a Clientes y al inicio de Taller. Se han **reorganizado por módulo y renumerado** para integrarlos en una especificación completa, preservando su redacción original:

| ID anterior | Descripción | ID actual |
|---|---|---|
| RF-001 | Registrar clientes | **RF-012** |
| RF-002 | Editar clientes | **RF-013** |
| RF-003 | Eliminar clientes | **RF-014** |
| RF-004 | Buscar clientes | **RF-015** |
| RF-005 | Registrar equipos para reparación | **RF-051** |
| RF-006 | Generar una Orden de Trabajo | **RF-052** |

## 4. Requerimientos funcionales por módulo

### 4.1 Usuarios y Autenticación

| ID | Descripción | Prioridad | Estado | Actor principal | Referencia |
|---|---|---|---|---|---|
| RF-001 | El sistema permitirá registrar usuarios con su información básica y rol asignado. | Alta | [I] | Administrador | BQ-041 |
| RF-002 | El sistema permitirá editar los datos de un usuario existente. | Alta | [I] | Administrador | — |
| RF-003 | El sistema permitirá desactivar (no eliminar físicamente) a un usuario, preservando su historial para auditoría. | Alta | [I] | Administrador | RF-078 |
| RF-004 | El sistema permitirá a un usuario autenticarse mediante credenciales propias. | Alta | [I] | Todos | — |
| RF-005 | El sistema permitirá cerrar la sesión activa de un usuario. | Media | [I] | Todos | — |
| RF-006 | El sistema deberá bloquear el acceso tras un número determinado de intentos fallidos de autenticación. | Media | [PV] | — | BQ-047 |
| RF-007 | El sistema permitirá restablecer la contraseña de un usuario. | Media | [PV] | Administrador | BQ-048 |

### 4.2 Roles y Permisos

| ID | Descripción | Prioridad | Estado | Actor principal | Referencia |
|---|---|---|---|---|---|
| RF-008 | El sistema permitirá crear y administrar roles como conjuntos de permisos. | Alta | [I] | Administrador | — |
| RF-009 | El sistema permitirá asignar uno o más roles a un usuario. | Alta | [PV] | Administrador | BQ-042 |
| RF-010 | El sistema permitirá definir permisos granulares por módulo y acción (crear, editar, eliminar, consultar, anular). | Alta | [PV] | Administrador | BQ-063 |
| RF-011 | El sistema deberá restringir el acceso a cada módulo según el rol del usuario autenticado. | Alta | [I] | — | — |

### 4.3 Clientes

| ID | Descripción | Prioridad | Estado | Actor principal | Referencia |
|---|---|---|---|---|---|
| RF-012 | El sistema permitirá registrar clientes. | Alta | [C] | Recepcionista, Vendedor | — |
| RF-013 | El sistema permitirá editar clientes. | Alta | [C] | Recepcionista, Vendedor | — |
| RF-014 | El sistema permitirá eliminar clientes. **Nota del analista:** dado que un cliente puede tener historial de ventas u OT asociado (RN-005), se recomienda evaluar baja lógica en vez de eliminación física, para no romper trazabilidad. | Media | [C]/[PV] | Administrador | BQ-064 |
| RF-015 | El sistema permitirá buscar clientes. | Alta | [C] | Recepcionista, Vendedor | — |
| RF-016 | El sistema permitirá diferenciar entre cliente persona natural y persona jurídica, para determinar el tipo de comprobante emitible. | Alta | [PV] | Vendedor | BQ-011, CAT-006 |
| RF-017 | El sistema permitirá consultar el historial de compras, órdenes de trabajo y servicios de campo asociados a un cliente. | Alta | [I] | Todos (consulta) | RN-005 |

### 4.4 Proveedores

| ID | Descripción | Prioridad | Estado | Actor principal | Referencia |
|---|---|---|---|---|---|
| RF-018 | El sistema permitirá registrar proveedores. | Alta | [I] | Almacenero | — |
| RF-019 | El sistema permitirá editar proveedores. | Alta | [I] | Almacenero | — |
| RF-020 | El sistema permitirá desactivar proveedores. | Media | [I] | Administrador | — |
| RF-021 | El sistema permitirá buscar proveedores. | Alta | [I] | Almacenero | — |
| RF-022 | El sistema permitirá consultar el historial de compras asociado a un proveedor. | Media | [I] | Almacenero, Contador | — |

### 4.5 Inventario / Almacén

| ID | Descripción | Prioridad | Estado | Actor principal | Referencia |
|---|---|---|---|---|---|
| RF-023 | El sistema permitirá registrar productos con su información básica (nombre, categoría, unidad de medida, precio, stock). | Alta | [I] | Almacenero | — |
| RF-024 | El sistema permitirá editar productos. | Alta | [I] | Almacenero | — |
| RF-025 | El sistema permitirá clasificar productos por categoría (herramientas eléctricas, manuales, materiales eléctricos/sanitarios, repuestos, accesorios, tecnología, etc.). | Alta | [C] | Almacenero | CAT-007 |
| RF-026 | El sistema permitirá consultar el stock disponible de un producto en tiempo real. | Alta | [I] | Vendedor, Almacenero, Técnico | — |
| RF-027 | El sistema deberá registrar automáticamente todo movimiento de inventario (ingreso, salida, ajuste) en el Kardex del producto. | Alta | [C] | — | RN-002 |
| RF-028 | El sistema deberá impedir la venta o consumo de un producto sin stock disponible. | Alta | [C] | — | RN-003, BQ-001 |
| RF-029 | El sistema permitirá diferenciar el motivo de cada movimiento de inventario (venta, consumo en taller, consumo en campo, compra, ajuste, devolución). | Alta | [I] | Almacenero | CAT-015 |
| RF-030 | El sistema permitirá realizar ajustes manuales de inventario (conteos físicos), registrando usuario y motivo. | Media | [PV] | Almacenero | BQ-004 |
| RF-031 | El sistema permitirá definir un nivel mínimo de stock por producto y alertar cuando se alcance. | Media | [PV] | Almacenero | BQ-005 |
| RF-032 | El sistema deberá soportar un inventario único compartido entre Tienda, Taller y Servicios de Campo. | **Alta — crítico** | [C] | — | BQ-001, BQ-003 |

### 4.6 Compras

| ID | Descripción | Prioridad | Estado | Actor principal | Referencia |
|---|---|---|---|---|---|
| RF-033 | El sistema permitirá registrar órdenes de compra a proveedores. | Alta | [PV] | Almacenero | BQ-020 |
| RF-034 | El sistema permitirá registrar la recepción de mercadería asociada a una orden de compra, actualizando el stock. | Alta | [I] | Almacenero | RF-027 |
| RF-035 | El sistema permitirá registrar el costo de compra de cada producto, actualizando su costo de referencia. | Alta | [PV] | Almacenero | BQ-021 |
| RF-036 | El sistema permitirá consultar el historial de compras por proveedor o por producto. | Media | [I] | Almacenero, Contador | — |
| RF-037 | El sistema permitirá registrar compras directas sin orden de compra previa. | Media | [PV] | Almacenero | BQ-022 |

### 4.7 Ventas

| ID | Descripción | Prioridad | Estado | Actor principal | Referencia |
|---|---|---|---|---|---|
| RF-038 | El sistema permitirá registrar una venta seleccionando productos del inventario, calculando el total automáticamente. | Alta | [I] | Vendedor | — |
| RF-039 | El sistema permitirá emitir distintos tipos de comprobante de venta: cotización, boleta, factura, nota de venta y ticket. | Alta | [C] | Vendedor | CAT-004 |
| RF-040 | El sistema permitirá convertir una cotización aprobada en una venta formal, sin duplicar el registro. | Alta | [I] | Vendedor | — |
| RF-041 | El sistema deberá validar el stock disponible antes de confirmar una venta. | Alta | [C] | — | RN-003 |
| RF-042 | El sistema permitirá registrar el o los medios de pago utilizados en una venta. | Alta | [PV] | Vendedor | BQ-012, CAT-005 |
| RF-043 | El sistema permitirá anular una venta, registrando auditoría y revirtiendo el movimiento de inventario asociado. | Alta | [PV] | Vendedor, Administrador | BQ-013 |
| RF-044 | El sistema deberá emitir comprobantes electrónicos (boleta/factura) conforme a la normativa SUNAT vigente, si se determina obligatorio. | Alta (condicionado) | [PV] | — | BQ-050, BQ-051 |
| RF-045 | El sistema permitirá aplicar descuentos a una venta, dentro de límites autorizados por rol. | Media | [PV] | Vendedor | BQ-014 |

### 4.8 Caja

| ID | Descripción | Prioridad | Estado | Actor principal | Referencia |
|---|---|---|---|---|---|
| RF-046 | El sistema permitirá registrar la apertura de caja con un monto inicial. | Alta | [PV] | Cajero/Vendedor | BQ-030 |
| RF-047 | El sistema permitirá registrar ingresos y egresos de caja asociados a ventas, cobros de OT y otros conceptos. | Alta | [PV] | Cajero/Vendedor | — |
| RF-048 | El sistema permitirá realizar el cierre/arqueo de caja, comparando el monto teórico contra el físico declarado. | Alta | [PV] | Cajero/Vendedor, Contador | BQ-031 |
| RF-049 | El sistema permitirá emitir un reporte de movimientos de caja por turno/jornada. | Media | [PV] | Contador | — |
| RF-050 | El sistema deberá impedir la entrega de un equipo reparado sin el registro previo del pago correspondiente. | Alta | [C] | Recepcionista | RN-001 |

### 4.9 Taller (Recepción, Diagnóstico, Cotización, Reparación, Garantía, Entrega)

| ID | Descripción | Prioridad | Estado | Actor principal | Referencia |
|---|---|---|---|---|---|
| RF-051 | El sistema permitirá registrar equipos para reparación, asociados a un cliente. | Alta | [C] | Recepcionista | — |
| RF-052 | El sistema permitirá generar una Orden de Trabajo (OT) para un equipo recibido. | Alta | [C] | Recepcionista | RN-004, RN-005 |
| RF-053 | El sistema permitirá emitir un comprobante de recepción al momento de registrar el equipo. | Alta | [C] | Recepcionista | — |
| RF-054 | El sistema permitirá registrar el diagnóstico técnico de un equipo dentro de su OT. | Alta | [I] | Técnico | — |
| RF-055 | El sistema permitirá generar una cotización de reparación a partir del diagnóstico registrado. | Alta | [I] | Técnico, Supervisor | — |
| RF-056 | El sistema permitirá registrar la aprobación o el rechazo del cliente sobre una cotización de reparación. | Alta | [I] | Recepcionista | BQ-034 |
| RF-057 | El sistema permitirá registrar los repuestos consumidos durante una reparación, descontándolos automáticamente del inventario. | Alta | [C] | Técnico | RN-002, RN-003 |
| RF-058 | El sistema permitirá registrar el resultado de las pruebas realizadas antes de la entrega del equipo. | Media | [I] | Técnico | — |
| RF-059 | El sistema permitirá registrar la entrega del equipo al cliente, validando el pago previo. | Alta | [C] | Recepcionista | RN-001 |
| RF-060 | El sistema permitirá consultar el historial único de cada equipo a través de sus distintos ingresos. | Alta | [C] | Todos (consulta) | RN-005 |
| RF-061 | El sistema permitirá registrar una garantía asociada a una reparación, con su período de cobertura. | Media | [PV] | Técnico, Supervisor | BQ-035, BQ-036 |
| RF-062 | El sistema permitirá vincular una nueva OT a una garantía vigente, identificando si la falla está cubierta. | Media | [PV] | Recepcionista, Supervisor | BQ-036 |
| RF-063 | El sistema permitirá registrar y consultar el estado actual de una OT en todo momento. | Alta | [PV] | Todos (consulta) | Business-States.md, BQ-032 |

### 4.10 Servicios de Campo

| ID | Descripción | Prioridad | Estado | Actor principal | Referencia |
|---|---|---|---|---|---|
| RF-064 | El sistema permitirá registrar una solicitud de servicio de campo asociada a un cliente. | Alta | [I] | Recepcionista | — |
| RF-065 | El sistema permitirá agendar y asignar un técnico a un servicio de campo. | Alta | [PV] | Supervisor | BQ-037 |
| RF-066 | El sistema permitirá registrar el diagnóstico y/o trabajo realizado en un servicio de campo. | Alta | [I] | Técnico | — |
| RF-067 | El sistema permitirá registrar los materiales/repuestos consumidos en un servicio de campo, descontándolos del inventario compartido. | Alta | [C] | Técnico | RN-002, RF-032 |
| RF-068 | El sistema permitirá generar una cotización para un servicio de campo, de forma análoga al Taller. | Media | [PV] | Técnico, Supervisor | BQ-038 |
| RF-069 | El sistema permitirá registrar la conformidad del cliente al finalizar un servicio de campo. | Media | [PV] | Técnico | BQ-039 |
| RF-070 | El sistema permitirá registrar el cobro de un servicio de campo. | Alta | [PV] | Técnico, Cajero | BQ-040 |
| RF-071 | El sistema deberá permitir el registro de información de un servicio de campo en condiciones de conectividad limitada, sincronizando al recuperar conexión. | Alta | [PV] | Técnico | BQ-052, BQ-053 |

### 4.11 Reportes

| ID | Descripción | Prioridad | Estado | Actor principal | Referencia |
|---|---|---|---|---|---|
| RF-072 | El sistema permitirá generar un reporte de ventas por período. | Alta | [I] | Gerente, Contador | — |
| RF-073 | El sistema permitirá generar un reporte de estado de inventario (Kardex, stock actual, quiebres). | Alta | [I] | Gerente, Almacenero | — |
| RF-074 | El sistema permitirá generar un reporte de Órdenes de Trabajo por estado, técnico y período. | Alta | [I] | Supervisor, Gerente | — |
| RF-075 | El sistema permitirá generar un reporte de servicios de campo por período y técnico. | Media | [I] | Supervisor, Gerente | — |
| RF-076 | El sistema permitirá generar un reporte de caja por período. | Alta | [I] | Contador, Gerente | — |
| RF-077 | El sistema permitirá generar reportes adicionales según necesidad gerencial (rentabilidad, rotación de inventario, indicadores de taller). | Media | [PV] | Gerente | BQ-054 |

### 4.12 Auditoría

| ID | Descripción | Prioridad | Estado | Actor principal | Referencia |
|---|---|---|---|---|---|
| RF-078 | El sistema deberá registrar de forma inmutable las acciones relevantes de cada usuario (creación, edición, eliminación, anulación). | Alta | [C] | — | RNF-005 |
| RF-079 | El sistema permitirá consultar el historial de auditoría filtrando por usuario, módulo y fecha. | Media | [I] | Administrador, Gerente | BQ-049 |

### 4.13 Configuración

| ID | Descripción | Prioridad | Estado | Actor principal | Referencia |
|---|---|---|---|---|---|
| RF-080 | El sistema permitirá configurar los datos generales de la empresa (razón social, RUC, logo, direcciones). | Alta | [I] | Administrador | — |
| RF-081 | El sistema permitirá configurar las series y correlativos de los distintos comprobantes. | Alta | [PV] | Administrador | BQ-050 |
| RF-082 | El sistema permitirá administrar los catálogos del sistema (ver Business-Catalogs.md) sin requerir cambios de código, en línea con la visión de solución configurable declarada para el proyecto. | Media | [PV] | Administrador | — |

## 5. Requerimientos explícitamente fuera de alcance (v1)

No se documentan por ausencia total de información de negocio, no por decisión del analista:
- Portal de autoservicio para clientes (depende de **BQ-006**).
- Integración con comercio electrónico / venta online.
- Aplicación móvil dedicada para técnicos de campo (depende de **BQ-052**, **BQ-053**).

## 6. Siguiente paso

Cada requerimiento marcado **[PV]** debe resolverse contra [Business-Questions.md](../02-Business/Business-Questions.md) antes de derivar casos de uso detallados o el modelo de dominio.

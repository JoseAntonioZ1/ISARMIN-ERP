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

## 3.1 Actualización de actores (validación con el propietario, 2026-07-18)

La columna **Actor principal** de las tablas siguientes usa los nombres de rol especulados en la versión original de `Actors.md` (Recepcionista, Vendedor, Almacenero, Supervisor, Cajero, Contador). La validación directa con el propietario confirmó que la operación real tiene solo **3 roles**: **Administrador/Propietario**, **Ventas** y **Técnico** (ver `Actors.md`, sección 5). En lugar de reescribir las 82 filas de este documento, se deja esta tabla de equivalencia como referencia de lectura:

| Actor citado en este documento | Rol real equivalente |
|---|---|
| Recepcionista, Cajero, Vendedor | **Ventas** (con la ambigüedad pendiente de BQ-089 sobre si Ventas cubre también la recepción de equipos de Taller) |
| Almacenero, Supervisor, Gerente | **Administrador/Propietario** |
| Contador | Sin rol interno confirmado en el MVP (ver `Actors.md`, ACT-008) |
| Técnico | **Técnico** (sin cambios — único rol para Taller y Campo) |

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

> **Confirmado por el propietario (2026-07-18):** el sistema **no debe asumir departamentos separados ni roles fijos en el código**. Los roles deben ser configurables por el Administrador, para permitir crecimiento futuro. Roles iniciales de referencia (semilla, no exhaustivos ni fijos): Administrador/Gerente, Ventas, Técnico, Caja — pero debe ser posible crear nuevos roles posteriormente sin cambios de arquitectura.

| ID | Descripción | Prioridad | Estado | Actor principal | Referencia |
|---|---|---|---|---|---|
| RF-008 | El sistema permitirá crear, editar y administrar roles como conjuntos de permisos configurables, sin roles fijos en el código. | **Alta** | [C] | Administrador | — |
| RF-009 | El sistema permitirá asignar uno o más roles a un usuario. | Alta | [PV] | Administrador | BQ-042 |
| RF-010 | El sistema permitirá definir permisos granulares por módulo y acción (crear, editar, eliminar, consultar, anular) para cada rol, incluyendo la posibilidad de que más de un rol comparta acceso a un mismo módulo (ej. la recepción de equipos, ver RF-051). | **Alta** | [C] | Administrador | BQ-063 (resuelta), BQ-089 (resuelta) |
| RF-011 | El sistema deberá restringir el acceso a cada módulo según el rol del usuario autenticado. | Alta | [I] | — | — |

### 4.3 Clientes

| ID | Descripción | Prioridad | Estado | Actor principal | Referencia |
|---|---|---|---|---|---|
| RF-012 | El sistema permitirá registrar clientes. | Alta | [C] | Recepcionista, Vendedor | — |
| RF-013 | El sistema permitirá editar clientes. | Alta | [C] | Recepcionista, Vendedor | — |
| RF-014 | El sistema permitirá desactivar clientes manteniendo todo su historial asociado (ventas, reparaciones, servicios, pagos, historial de equipos). No existe eliminación física. | Alta | [C] | Administrador | BQ-064 (resuelta) |
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
| RF-023 | El sistema permitirá registrar productos **manualmente** (sin importación masiva en V1) con al menos los siguientes atributos: **código interno (obligatorio)**, nombre, categoría, marca, unidad de medida, costo de adquisición, precio de venta, margen, stock, y **código de barras comercial (opcional)**. | Alta | [C] | Administrador | BQ-056 (resuelta) |
| RF-024 | El sistema permitirá editar productos. | Alta | [I] | Almacenero | — |
| RF-025 | El sistema permitirá clasificar productos por categoría (herramientas eléctricas, manuales, materiales eléctricos/sanitarios, repuestos, accesorios, tecnología, etc.). | Alta | [C] | Almacenero | CAT-007 |
| RF-026 | El sistema permitirá consultar el stock disponible de un producto en tiempo real. | Alta | [I] | Vendedor, Almacenero, Técnico | — |
| RF-027 | El sistema deberá registrar automáticamente todo movimiento de inventario (ingreso, salida, ajuste) en el Kardex del producto. | Alta | [C] | — | RN-002 |
| RF-028 | El sistema deberá impedir la venta o consumo de un producto sin stock disponible, validando la disponibilidad al momento de confirmar la operación (sin stock negativo en operaciones normales). | Alta | [C] | — | RN-003, RN-007, BQ-001 (resuelta) |
| RF-029 | El sistema permitirá diferenciar el motivo de cada movimiento de inventario (venta, consumo en taller, consumo en campo, compra, ajuste, devolución). | Alta | [I] | Almacenero | CAT-015 |
| RF-030 | El sistema permitirá realizar ajustes manuales de inventario (conteos físicos, incluyendo la única excepción que permite dejar stock en un valor distinto al calculado por el Kardex), registrando usuario y motivo obligatorio. **Solo el Administrador/Propietario puede realizar este ajuste** (RN-008). | **Alta** | [C] | Administrador | BQ-004, BQ-001 (resuelta) |
| RF-031 | El sistema permitirá definir un nivel mínimo de stock por producto y alertar cuando se alcance. | Media | [PV] | Almacenero | BQ-005 |
| RF-032 | El sistema deberá soportar un inventario único compartido entre Tienda, Taller y Servicios de Campo. | **Alta — crítico** | [C] | — | BQ-001, BQ-003 |

### 4.6 Compras

> **Flujo real confirmado por el propietario (2026-07-18):** Proveedor → Compra realizada → Documento de compra → Registro en sistema → Actualización de inventario. No existe un proceso formal de Orden de Compra con aprobación previa; **queda explícitamente para una versión futura**.

| ID | Descripción | Prioridad | Estado | Actor principal | Referencia |
|---|---|---|---|---|---|
| RF-033 | El sistema permitirá registrar una compra ya realizada, indicando proveedor, fecha, productos adquiridos y cantidad, sin requerir una orden de compra ni aprobación previa. El registro deberá actualizar automáticamente el inventario (stock y Kardex) de los productos adquiridos. | Alta | [C] | Administrador | BQ-020, RN-024, RF-027 |
| RF-034 | El sistema permitirá registrar el documento de compra (boleta/factura del proveedor u otro comprobante) asociado a cada compra registrada. | Alta | [C] | Administrador | — |
| RF-035 | El sistema permitirá registrar el costo de cada producto adquirido, actualizando su costo de referencia mediante costo promedio ponderado (RN-013, decisión tentativa). | Alta | [PV] | Administrador | BQ-021 |
| RF-036 | El sistema permitirá consultar el historial de compras por proveedor o por producto. | Media | [I] | Administrador | — |
| RF-037 | *(Fuera de alcance v1 — versión futura)* El sistema permitirá formalizar un flujo de Orden de Compra con aprobación previa, para cuando el negocio requiera ese nivel de control. | Baja | [PV] | — | BQ-020, BQ-022 |

### 4.7 Ventas

| ID | Descripción | Prioridad | Estado | Actor principal | Referencia |
|---|---|---|---|---|---|
| RF-038 | El sistema permitirá registrar una venta seleccionando productos del inventario, calculando el total automáticamente. | Alta | [I] | Vendedor | — |
| RF-039 | El sistema permitirá emitir distintos tipos de comprobante de venta: cotización, boleta, factura, nota de venta y ticket. | Alta | [C] | Vendedor | CAT-004 |
| RF-040 | El sistema permitirá convertir una cotización aprobada en una venta formal, sin duplicar el registro. | Alta | [I] | Vendedor | — |
| RF-041 | El sistema deberá validar el stock disponible antes de confirmar una venta. | Alta | [C] | — | RN-003 |
| RF-042 | El sistema permitirá registrar el o los medios de pago utilizados en una venta, seleccionando entre un catálogo **configurable** de medios de pago. Valores iniciales confirmados: Efectivo, Yape, Plin, Transferencia bancaria (ampliable a Tarjeta u otros sin cambios de arquitectura). | Alta | [C] | Ventas | BQ-012 (resuelta), Business-Catalogs.md (CAT-008) |
| RF-043 | El sistema permitirá anular una venta, registrando auditoría y revirtiendo el movimiento de inventario asociado. | Alta | [PV] | Vendedor, Administrador | BQ-013 |
| RF-044 | El sistema deberá emitir comprobantes electrónicos (boleta/factura) conforme a la normativa SUNAT vigente, si se determina obligatorio. | Alta (condicionado) | [PV] | — | BQ-050, BQ-051 |
| RF-045 | El sistema permitirá aplicar descuentos a una venta, dentro de límites autorizados por rol. | Media | [PV] | Vendedor | BQ-014 |
| RF-089 | El sistema permitirá registrar una venta con saldo pendiente autorizado por el Administrador/Propietario (RN-031), registrando usuario autorizante, monto pendiente y referencia de pago pendiente. | Alta | [C] | Administrador, Ventas | RN-031, BQ-019 (resuelta) |
| RF-091 | El sistema permitirá registrar la devolución de un producto vendido mediante un movimiento de inventario de tipo "Devolución" (CAT-013), con trazabilidad al comprobante de venta original. | Alta | [C] | Administrador, Ventas | RN-032, BQ-087 (resuelta) |

### 4.8 Caja

| ID | Descripción | Prioridad | Estado | Actor principal | Referencia |
|---|---|---|---|---|---|
| RF-046 | El sistema permitirá registrar la apertura de caja con un monto inicial. | Alta | [PV] | Cajero/Vendedor | BQ-030 |
| RF-047 | El sistema permitirá registrar ingresos y egresos de caja asociados a ventas, cobros de OT y otros conceptos. | Alta | [PV] | Cajero/Vendedor | — |
| RF-048 | El sistema permitirá realizar el cierre/arqueo de caja, comparando el monto teórico contra el físico declarado. | Alta | [PV] | Cajero/Vendedor, Contador | BQ-031 |
| RF-049 | El sistema permitirá emitir un reporte de movimientos de caja por turno/jornada. | Media | [PV] | Contador | — |
| RF-050 | El sistema permitirá registrar cobros de Taller con estado de pago completo, adelanto, o saldo pendiente autorizado — el pago **no** bloquea la entrega del equipo (RN-001, corregida el 2026-07-18). **Dejar un saldo pendiente requiere autorización explícita del Administrador/Propietario**, registrando el usuario que autorizó. | Alta | [C] | Administrador, Ventas, Técnico | RN-001, BQ-093 (resuelta) |

### 4.9 Taller (Recepción, Diagnóstico, Cotización, Reparación, Garantía, Entrega)

| ID | Descripción | Prioridad | Estado | Actor principal | Referencia |
|---|---|---|---|---|---|
| RF-051 | El sistema permitirá registrar equipos para reparación, asociados a un cliente. **Confirmado (2026-07-18):** la recepción no es exclusiva de un rol — hoy no existe un recepcionista dedicado, por lo que el sistema debe permitir que Administrador, Técnico o Ventas registren la recepción, según quién atienda al cliente en ese momento (permiso configurable por rol, RF-010). | Alta | [C] | Administrador, Técnico o Ventas (configurable) | BQ-089 (resuelta) |
| RF-052 | El sistema permitirá generar una Orden de Trabajo (OT) para un equipo recibido, dejando registro del usuario que realizó el registro. | Alta | [C] | Administrador, Técnico o Ventas (configurable) | RN-004, RN-005 |
| RF-053 | El sistema permitirá emitir un comprobante de recepción al momento de registrar el equipo. | Alta | [C] | Administrador, Técnico o Ventas (configurable) | — |
| RF-054 | El sistema permitirá registrar el diagnóstico técnico de un equipo dentro de su OT. | Alta | [I] | Técnico | — |
| RF-055 | El sistema permitirá generar una cotización de reparación a partir del diagnóstico registrado. | Alta | [I] | Técnico, Supervisor | — |
| RF-056 | El sistema permitirá registrar la aprobación o el rechazo del cliente sobre una cotización de reparación. En caso de rechazo, el sistema permitirá opcionalmente registrar un cobro por el diagnóstico ya realizado, según decisión del usuario caso por caso (no se asume gratuidad ni costo fijo). | Alta | [C] | Administrador, Ventas o Técnico | RN-030, BQ-034 (resuelta) |
| RF-057 | El sistema permitirá registrar los repuestos consumidos durante una reparación, descontándolos automáticamente del inventario. | Alta | [C] | Técnico | RN-002, RN-003 |
| RF-058 | El sistema permitirá registrar el resultado de las pruebas realizadas antes de la entrega del equipo. | Media | [I] | Técnico | — |
| RF-059 | El sistema permitirá registrar la entrega del equipo al cliente, capturando: fecha y hora, usuario que realiza la entrega, estado del pago (completo antes de la entrega, completo al momento, adelanto, o saldo pendiente autorizado), monto pagado y saldo pendiente si corresponde. **El pago no es un prerrequisito bloqueante para la entrega** (RN-001, corregida el 2026-07-18). Si se deja saldo pendiente, el sistema exigirá registrar el **usuario Administrador/Propietario que autorizó**, además del monto pendiente y una fecha o referencia de pago pendiente. | Alta | [C] | Administrador, Ventas, Técnico | RN-001, BQ-093 (resuelta) |
| RF-088 | El sistema permitirá registrar el cobro posterior de un saldo pendiente asociado a una OT ya entregada. | Alta | [C] | Administrador, Ventas | RN-031, BQ-093 (resuelta) |
| RF-060 | El sistema permitirá consultar el historial completo de cada equipo a través de sus distintos ingresos, incluyendo por cada uno: fecha de ingreso, diagnóstico, reparación realizada, repuestos utilizados, técnico responsable y garantía asociada. | Alta | [C] | Todos (consulta) | RN-005 |
| RF-061 | El sistema permitirá registrar, para una reparación, si tiene garantía asociada, su período (fecha de inicio y fecha de finalización). **Alcance V1 confirmado y reducido (2026-07-18):** no incluye tipos de garantía, condiciones de cobertura ni gestión avanzada — eso queda para una versión futura. | Alta | [C] | Técnico, Administrador | BQ-035 (resuelta con alcance reducido) |
| RF-062 | El sistema permitirá asociar una nueva Orden de Trabajo a una garantía existente vigente. | Alta | [C] | Administrador | BQ-036 (resuelta con alcance reducido) |
| RF-063 | El sistema permitirá registrar y consultar el estado actual de una OT en todo momento. | Alta | [PV] | Todos (consulta) | Business-States.md, BQ-032 |

### 4.10 Servicios de Campo

| ID | Descripción | Prioridad | Estado | Actor principal | Referencia |
|---|---|---|---|---|---|
| RF-064 | El sistema permitirá registrar una solicitud de servicio de campo asociada a un cliente. | Alta | [I] | Recepcionista | — |
| RF-065 | El sistema permitirá agendar y asignar un técnico a un servicio de campo. | Alta | [PV] | Supervisor | BQ-037 |
| RF-066 | El sistema permitirá registrar el diagnóstico y/o trabajo realizado en un servicio de campo. | Alta | [I] | Técnico | — |
| RF-067 | El sistema permitirá registrar los materiales/repuestos consumidos en un servicio de campo, descontándolos del inventario compartido. | Alta | [C] | Técnico | RN-002, RF-032 |
| RF-068 | El sistema permitirá generar una cotización para un servicio de campo, de forma análoga al Taller. | Media | [PV] | Técnico, Supervisor | BQ-038 |
| RF-069 | El sistema permitirá registrar el cierre de un servicio de campo capturando: estado final del servicio, observaciones, y el usuario responsable del cierre. No incluye firma digital ni evidencias fotográficas en V1. | Alta | [C] | Técnico | RN-019, BQ-039 (resuelta) |
| RF-070 | El sistema permitirá registrar el cobro de un servicio de campo, con los mismos medios de pago configurables de RF-042, y admitiendo saldo pendiente autorizado por el Administrador/Propietario (RN-031). | Alta | [C] | Técnico, Ventas | RN-031, BQ-040 |
| RF-090 | El sistema permitirá registrar la participación de trabajadores temporales (ej. ayudantes de instalaciones) en una Orden de Trabajo o Servicio de Campo como dato de referencia (nombre, rol de apoyo), sin necesidad de crear una cuenta de usuario. | Media | [C] | Administrador, Técnico | RN-027 |
| RF-071 | El registro de un servicio de campo (diagnóstico, materiales, conformidad, cobro) se realizará **desde el sistema web**, típicamente al volver a la red local. **Confirmado y acotado el 2026-07-18:** para V1 se retiran explícitamente la aplicación móvil dedicada y la sincronización offline; el sistema debe diseñarse con un punto de extensión para incorporarlas en una versión futura, sin requerir rediseño. | Alta | [C] | Técnico | BQ-052 (resuelta), BQ-053 (resuelta) |

### 4.11 Reportes

| ID | Descripción | Prioridad | Estado | Actor principal | Referencia |
|---|---|---|---|---|---|
| RF-072 | El sistema permitirá generar un reporte de ventas por período. | Alta | [I] | Gerente, Contador | — |
| RF-073 | El sistema permitirá generar un reporte de estado de inventario (Kardex, stock actual, quiebres). | Alta | [I] | Gerente, Almacenero | — |
| RF-074 | El sistema permitirá generar un reporte de Órdenes de Trabajo por estado, técnico y período. | Alta | [I] | Supervisor, Gerente | — |
| RF-075 | El sistema permitirá generar un reporte de servicios de campo por período y técnico. | Media | [I] | Supervisor, Gerente | — |
| RF-076 | El sistema permitirá generar un reporte de caja por período. | Alta | [I] | Contador, Gerente | — |
| RF-077 | El sistema permitirá generar reportes adicionales según necesidad gerencial (rentabilidad, rotación de inventario, indicadores de taller). | Media | [PV] | Gerente | BQ-054 |

### 4.12 Auditoría (capacidad transversal, no módulo independiente)

> **Confirmado por el propietario (2026-07-18):** la auditoría se mantiene en V1 como funcionalidad transversal (no como módulo complejo independiente ni con interfaz propia extensa).

| ID | Descripción | Prioridad | Estado | Actor principal | Referencia |
|---|---|---|---|---|---|
| RF-078 | El sistema deberá registrar de forma inmutable las siguientes acciones críticas: creación de registros, modificaciones, eliminaciones lógicas, anulaciones, movimientos de inventario, movimientos de caja y cambios importantes en Órdenes de Trabajo. | Alta | [C] | — | RNF-005 |
| RF-079 | El sistema permitirá consultar el historial de auditoría, mostrando para cada evento: usuario, fecha, hora, acción realizada y registro afectado. | Alta | [C] | Administrador | BQ-049 (resuelta) |

### 4.13 Configuración

| ID | Descripción | Prioridad | Estado | Actor principal | Referencia |
|---|---|---|---|---|---|
| RF-080 | El sistema permitirá configurar los datos generales de la empresa (razón social, RUC, logo, direcciones). | Alta | [I] | Administrador | — |
| RF-081 | El sistema permitirá configurar las series y correlativos de los distintos comprobantes. | Alta | [PV] | Administrador | BQ-050 |
| RF-082 | El sistema permitirá administrar los catálogos del sistema (ver Business-Catalogs.md) sin requerir cambios de código, en línea con la visión de solución configurable declarada para el proyecto. | Media | [PV] | Administrador | — |

### 4.14 Relación entre Ventas y Servicios Técnicos

> **Nuevo requerimiento incorporado el 2026-07-18**, a partir de la validación del propietario: una venta puede combinar materiales vendidos con la mano de obra de una reparación o servicio de campo (ej. una instalación eléctrica factura materiales + mano de obra en un mismo comprobante).

| ID | Descripción | Prioridad | Estado | Actor principal | Referencia |
|---|---|---|---|---|---|
| RF-083 | El sistema permitirá que un comprobante de venta esté asociado a una reparación (OT), a un servicio de campo, o a una cotización aprobada de cualquiera de los dos, combinando en un mismo comprobante los materiales/repuestos utilizados y el costo de mano de obra. | Alta | [C] | Administrador, Ventas | RF-040, RF-057, RF-067 |

### 4.15 Gestión Documental

> **Nuevo módulo incorporado el 2026-07-18**, a partir de la validación del propietario: el sistema debe quedar preparado para almacenar archivos asociados a distintas entidades del negocio.

| ID | Descripción | Prioridad | Estado | Actor principal | Referencia |
|---|---|---|---|---|---|
| RF-084 | El sistema permitirá adjuntar fotografías de equipos a una Orden de Trabajo o a un Servicio de Campo. | Alta | [C] | Técnico, Administrador | RF-052, RF-064 |
| RF-085 | El sistema permitirá adjuntar el documento de compra (boleta/factura del proveedor) a una compra registrada. | Alta | [C] | Administrador | RF-034 |
| RF-086 | El sistema permitirá adjuntar cotizaciones, comprobantes e informes técnicos a la entidad correspondiente (venta, OT o servicio de campo). | Alta | [C] | Administrador, Ventas, Técnico | RF-039, RF-055, RF-068 |
| RF-087 | El sistema deberá estar preparado para almacenar y recuperar los archivos adjuntos de forma confiable, incluyendo su respaldo (ver Non-Functional-Requirements.md, sección de Respaldos). | Alta | [C] | — | RNF (respaldos) |

## 5. Requerimientos explícitamente fuera de alcance (v1)

Confirmados como fuera de alcance por el propietario / `PROJECT_SCOPE.md` (2026-07-18):
- Portal de autoservicio para clientes (**BQ-006**, resuelta).
- Aplicación móvil dedicada y sincronización offline para Servicios de Campo — la V1 es web; queda como punto de extensión futuro (**BQ-052**, **BQ-053**, resueltas).
- Flujo formal de Orden de Compra con aprobación previa (RF-037, **BQ-020**, **BQ-022**).
- Gestión avanzada de garantías (tipos de garantía, condiciones de cobertura detalladas) — V1 solo registra si tiene garantía, período y asociación a nueva OT (RF-061, RF-062).
- Integración con comercio electrónico / venta online.
- Lectura de código de barras mediante escáner (RF-023, confirmado 2026-07-18): el producto debe permitir almacenar un código de barras comercial opcional, pero la funcionalidad de escaneo no es obligatoria en V1. La arquitectura no debe impedir incorporarla después.

## 6. Siguiente paso

Cada requerimiento marcado **[PV]** debe resolverse contra [Business-Questions.md](../02-Business/Business-Questions.md) antes de derivar casos de uso detallados o el modelo de dominio.

# Business-Questions.md — Preguntas de Negocio Pendientes de Validación

## 1. Propósito

Este es el documento más importante de la fase de análisis: consolida **todas** las preguntas que deben resolverse con el cliente (ISARMIN PERÚ S.A.C.) antes de aprobar el modelo de dominio y comenzar el diseño de arquitectura. Sigue la técnica de **"Elicitation Results Register"** de BABOK v3 — un registro vivo que se actualiza a medida que se obtienen respuestas.

Cada pregunta referenciada como `[PV]` en `01-Requirements/` y `02-Business/` tiene aquí su origen formal. **Ninguna pregunta de este documento fue respondida por el analista**: todas están abiertas salvo las que se indican explícitamente como resueltas en la sección 3, cuya respuesta proviene de documentación oficial del cliente (`PROJECT_SCOPE.md`), no de una suposición del analista.

## 2. Convención

| Columna | Significado |
|---|---|
| **ID** | Identificador único, `BQ-XXX`. |
| **Prioridad** | **Alta**: bloquea decisiones de arquitectura/modelo de datos si no se responde. **Media**: afecta el detalle funcional pero no bloquea el diseño general. |
| **Relacionado con** | Documento(s) y sección donde esta pregunta ya fue citada. |

Las preguntas ya resueltas se marcan con **✅ RESUELTA** o **🟡 PARCIALMENTE RESUELTA** al inicio de su celda "Pregunta", seguidas de una nota con la fuente de la respuesta.

## 3. Estado del levantamiento

| Total de preguntas | Resueltas | Parcialmente resueltas | Pendientes |
|---|---|---|---|
| 84 | 2 | 1 | 81 |

**Última actualización:** 2026-07-18 — se incorporó `PROJECT_SCOPE.md` (previamente vacío), que respondió BQ-006 y BQ-002 en su totalidad, y BQ-050 parcialmente. Ver el detalle en cada sección correspondiente (4, 5 y 13).

---

## 4. Clientes y Proveedores

| ID | Pregunta | Prioridad | Relacionado con |
|---|---|---|---|
| BQ-006 | ✅ **RESUELTA.** ¿El actor "Cliente" accederá alguna vez directamente al sistema (portal web o app) para aprobar cotizaciones o consultar el estado de su equipo/servicio? ¿O es exclusivamente una entidad administrada por el personal interno? | **Alta** | Actors.md (ACT-009), Functional-Requirements.md |
| BQ-011 | ¿Qué tipos de cliente maneja el negocio (persona natural / persona jurídica)? ¿Qué documento de identidad corresponde a cada uno (DNI, RUC, Carné de Extranjería, Pasaporte)? ¿Existe alguna segmentación comercial adicional (mayorista, minorista, cliente frecuente)? | Alta | RF-016, Business-Catalogs.md (CAT-009, CAT-020) |
| BQ-024 | ¿Los proveedores requerirán algún tipo de acceso directo al sistema (ej. portal para confirmar una orden de compra), o toda la interacción se gestiona a través del Almacenero? | Media | Actors.md (ACT-010) |
| BQ-064 | Al "eliminar" un cliente (RF-014), ¿corresponde una baja lógica (recomendada por el analista para preservar trazabilidad) o debe permitirse eliminación física bajo ciertas condiciones? | Media | Business-Rules.md (RN-023) |
| BQ-074 | ¿Qué datos son obligatorios para registrar un cliente (además de nombre y documento de identidad)? ¿Dirección, teléfono, correo son obligatorios u opcionales? | Media | RF-012 |
| BQ-075 | ¿Un mismo producto puede comprarse a más de un proveedor, o existe una relación de exclusividad producto-proveedor? | Baja | RF-018, RF-033 |

> **Respuesta a BQ-006 (fuente: `PROJECT_SCOPE.md`, sección "Funcionalidades Fuera del Alcance Inicial"):** "Portal para clientes" y "Portal para técnicos" están explícitamente fuera del alcance de la primera versión. Se confirma que el actor Cliente **no** tendrá acceso directo al sistema en el MVP; es exclusivamente una entidad administrada por el personal interno (Recepcionista/Vendedor). Impacto: `Actors.md` (ACT-009) puede actualizarse de [PV] a [C]; el modelo de autenticación no necesita contemplar usuarios externos en esta fase.

## 5. Inventario

| ID | Pregunta | Prioridad | Relacionado con |
|---|---|---|---|
| BQ-001 | ¿Debe permitirse que el stock de un producto quede en negativo bajo alguna circunstancia (ej. venta con reposición en camino), o es una restricción absoluta? | **Alta** | RN-003, RN-008, RF-028 |
| BQ-002 | ✅ **RESUELTA.** ¿La empresa opera desde un único almacén/local, o existen (o se planean) múltiples almacenes o sucursales? | **Alta** | Actors.md (ACT-006), Business-Catalogs.md (CAT-016) |
| BQ-003 | Ante una posible concurrencia entre una venta en tienda y un consumo de repuesto en taller sobre el mismo producto, ¿debe existir un mecanismo de reserva de stock, o basta con validar disponibilidad al confirmar cada operación? | **Alta** | RN-007, Business-Processes.md (BP-001) |
| BQ-004 | ¿Cómo se realizan hoy los ajustes/conteos físicos de inventario? ¿Quién los autoriza y con qué frecuencia? | Media | RF-030 |
| BQ-005 | ¿Se requiere definir un stock mínimo por producto con alertas automáticas de reposición? | Media | RF-031, Business-States.md (ST-004) |
| BQ-007 | La lista de categorías de producto declarada (herramientas, materiales, repuestos, etc.) ¿es exhaustiva o solo ilustrativa? ¿Se requiere una jerarquía de categorías y subcategorías? | Media | Business-Catalogs.md (CAT-002) |
| BQ-008 | ¿Qué unidades de medida maneja el negocio (unidad, metro, kilogramo, rollo, etc.)? ¿Algún producto requiere conversión entre unidades (ej. cable por metro vendido también por rollo)? | Media | Business-Catalogs.md (CAT-014) |

> **Respuesta a BQ-002 (fuente: `PROJECT_SCOPE.md`, sección "Funcionalidades Fuera del Alcance Inicial"):** "Integración con múltiples sucursales" está explícitamente fuera del alcance de la primera versión. Se confirma **almacén/local único** para el MVP. Impacto: `Business-Catalogs.md` (CAT-016) puede fijarse en "Almacén Único" como valor confirmado [C]; no es necesario modelar multi-almacén en esta fase, aunque el diseño debería dejar el punto de extensión abierto dado que `PROJECT_CONTEXT.md` prevé "múltiples sucursales" como posible ampliación futura.

## 6. Ventas

| ID | Pregunta | Prioridad | Relacionado con |
|---|---|---|---|
| BQ-009 | ¿La empresa opera en una única moneda (Soles), o en algún caso maneja precios/compras en moneda extranjera? | Media | Business-Catalogs.md (CAT-019) |
| BQ-010 | ¿Bajo qué criterio se decide emitir boleta, factura, nota de venta o ticket para una misma venta? | Alta | Business-Processes.md (BP-001) |
| BQ-012 | ¿Qué medios de pago acepta la empresa en Ventas (efectivo, tarjeta, transferencia, billeteras digitales)? ¿Se aceptan pagos combinados en una misma venta? | Alta | RF-042, Business-Catalogs.md (CAT-008) |
| BQ-013 | ¿Bajo qué reglas puede anularse una venta ya emitida? ¿Existe un plazo límite? ¿Qué rol lo autoriza? | Alta | RF-043, Business-States.md (ST-007) |
| BQ-014 | ¿Existen límites de descuento por rol? ¿Quién autoriza descuentos mayores a ese límite? | Media | RF-045 |
| BQ-015 | ¿Las cotizaciones de venta tienen un plazo de vigencia? ¿Cuál? | Media | Business-States.md (ST-002) |
| BQ-016 | ¿Una cotización de venta vencida puede reactivarse, o debe generarse una nueva? | Baja | Business-States.md (ST-002) |
| BQ-017 | ¿Se permiten pagos parciales o anticipos en una venta de tienda? | Media | Business-States.md (ST-003) |
| BQ-018 | Al anular una venta con comprobante electrónico ya emitido, ¿se requiere una nota de crédito ante SUNAT, o basta una marca interna de anulación? | **Alta** | Business-States.md (ST-007) |
| BQ-019 | ¿La empresa vende al crédito? De ser así, ¿existen límites de crédito por cliente y condiciones ante mora? | Media | — |

## 7. Compras

| ID | Pregunta | Prioridad | Relacionado con |
|---|---|---|---|
| BQ-020 | ¿Las órdenes de compra requieren aprobación formal? ¿A partir de qué monto y por parte de qué rol? | Media | RF-033, Business-States.md (ST-006) |
| BQ-021 | ¿Qué método de valorización de inventario utiliza (o debería utilizar) la empresa: PEPS, promedio ponderado, costo estándar u otro? | **Alta** | RN-013, RF-035 |
| BQ-022 | ¿Se realizan compras directas sin orden de compra previa (compras menores o urgentes)? ¿Bajo qué condiciones? | Media | RF-037 |
| BQ-023 | ¿Se acepta la recepción parcial de una orden de compra, o toda la mercadería debe recibirse en un solo evento? | Media | Business-States.md (ST-006) |
| BQ-025 | ¿Existen plazos de pago pactados con proveedores (crédito, contado)? | Baja | — |
| BQ-026 | ¿Se evalúa o califica a los proveedores de alguna forma (precio, tiempo de entrega, calidad)? | Baja | — |

## 8. Caja

| ID | Pregunta | Prioridad | Relacionado con |
|---|---|---|---|
| BQ-027 | ¿Cuántas cajas o puntos de cobro operan (u operarán) simultáneamente? | Media | Business-Processes.md (BP-005) |
| BQ-028 | ¿Los cobros de Taller y Servicios de Campo pasan por la misma caja que las ventas de tienda, o se manejan de forma separada? | Media | RF-050, RF-070 |
| BQ-029 | ¿El arqueo de caja debe cuadrar por cada medio de pago por separado (efectivo, tarjeta, transferencia), o solo el total general? | Media | RF-048 |
| BQ-030 | ¿Existe el concepto formal de "caja" con apertura y cierre de turno, o los cobros se registran sin ese control? | **Alta** | RF-046, Business-Rules.md (RN-014) |
| BQ-031 | En caso de existir arqueo de caja, ¿qué ocurre operativamente cuando el monto físico no coincide con el monto teórico del sistema? | Media | RF-048, Business-Rules.md (RN-015) |

## 9. Taller

| ID | Pregunta | Prioridad | Relacionado con |
|---|---|---|---|
| BQ-032 | ¿Cuáles son los estados reales por los que pasa una Orden de Trabajo en la operación actual? (la propuesta en Business-States.md ST-001 es una hipótesis a confirmar o corregir). | **Alta** | RF-063, Business-States.md (ST-001) |
| BQ-033 | ¿El pago debe registrarse estrictamente antes de la entrega física del equipo, o pueden ocurrir en el mismo acto (ej. el cliente paga al recibir)? | Alta | RN-001, Business-States.md (ST-001) |
| BQ-034 | Si el cliente rechaza la cotización de reparación, ¿qué ocurre con el equipo y con el diagnóstico ya realizado? ¿Se cobra algún concepto (ej. revisión técnica) aunque no se repare? ¿Cómo se evidencia formalmente la aprobación del cliente (firma física, digital, verbal)? | **Alta** | RF-056, Business-Rules.md (RN-016) |
| BQ-035 | ¿La empresa ofrece garantía sobre las reparaciones? ¿Cuál es su duración estándar? ¿Cubre solo el repuesto reemplazado, la mano de obra, o ambos? | Media | RF-061, Business-Catalogs.md (CAT-017) |
| BQ-036 | Si un equipo reingresa por la misma falla dentro del período de garantía, ¿cómo se vincula la nueva OT a la reparación original? ¿Se cobra o es gratuito? | Media | RF-062 |
| BQ-077 | ¿Existen tiempos de atención esperados o comprometidos (SLA) para el diagnóstico y la reparación? | Baja | — |
| BQ-078 | ¿Qué ocurre si el cliente no recoge su equipo tras ser notificado de que está listo? ¿Existe un plazo o política de almacenamiento/abandono? | Media | — |

## 10. Servicios de Campo

| ID | Pregunta | Prioridad | Relacionado con |
|---|---|---|---|
| BQ-037 | ¿Cómo se agenda hoy un servicio de campo? ¿Existe un calendario o ruta de técnicos? ¿Cómo se decide qué técnico asignar? | Alta | RF-065, Business-States.md (ST-005) |
| BQ-038 | ¿Un servicio de campo se cotiza de forma similar a una reparación de Taller, o tiene un modelo comercial distinto (ej. tarifa fija por visita)? | Media | RF-068 |
| BQ-039 | ¿Cómo se registra la conformidad del cliente al finalizar un servicio de campo (firma, foto, checklist)? ¿Qué ocurre si el cliente no está conforme? | Alta | RF-069, Business-States.md (ST-005) |
| BQ-040 | ¿Cómo se cobra un servicio de campo? ¿En el sitio (requiere medio de pago móvil) o posteriormente en el local? | Alta | RF-070 |
| BQ-052 | ¿Los técnicos de campo necesitarán registrar información del servicio (diagnóstico, materiales, conformidad) mientras están fuera de la red local de la empresa? | **Alta** | RF-071, Non-Functional-Requirements.md (RNF-014) |
| BQ-053 | Si se requiere conectividad remota para Servicios de Campo, ¿qué mecanismo es aceptable dado que la infraestructura declarada es "PC de escritorio + red local" (app con sincronización diferida, acceso remoto seguro, dispositivo con datos móviles)? | **Alta** | RF-071, Non-Functional-Requirements.md (RNF-014) |

## 11. Usuarios y Roles

| ID | Pregunta | Prioridad | Relacionado con |
|---|---|---|---|
| BQ-041 | ¿"Técnico" es un único rol para Taller y Campo, o deben existir roles distintos con permisos y necesidades de acceso diferentes (ej. el técnico de campo necesita acceso remoto)? | **Alta** | Actors.md (ACT-007), Business-Catalogs.md (CAT-005) |
| BQ-042 | ¿Un usuario puede tener asignado más de un rol simultáneamente (ej. Vendedor que también es Cajero)? | Alta | RF-009 |
| BQ-043 | ¿Puede un usuario mantener sesión activa en más de un dispositivo simultáneamente? | Baja | RF-004 |
| BQ-044 | ¿Debe existir un tiempo de inactividad tras el cual la sesión se cierre automáticamente? | Media | RF-005 |
| BQ-045 | ¿Los permisos deben poder definirse por sucursal/almacén además de por rol, considerando la posibilidad de múltiples sedes a futuro (ver BQ-002)? | Baja | RF-010 |
| BQ-046 | ¿Existirá algún mecanismo de autoregistro de usuarios, o todo usuario es creado exclusivamente por el Administrador? | Media | RF-001 |
| BQ-047 | ¿Qué política de seguridad de contraseñas se requiere (complejidad, expiración, bloqueo tras intentos fallidos)? | Media | RF-006, Non-Functional-Requirements.md (RNF-013) |
| BQ-048 | ¿El restablecimiento de contraseña debe ser autoservicio (ej. correo de recuperación) o exclusivamente gestionado por el Administrador? | Media | RF-007 |
| BQ-049 | ¿Qué nivel de detalle se espera en la auditoría (solo cambios críticos, o todo acceso y consulta)? ¿Cuánto tiempo debe conservarse ese historial? | Media | RF-079 |
| BQ-057 | El rol "Administrador", ¿es un perfil técnico (TI) o administrativo (Gerencia)? ¿Existirá más de un Administrador? | Media | Actors.md (ACT-001) |
| BQ-058 | El rol "Gerente", ¿tiene funciones de aprobación sobre operaciones (descuentos, compras) o es exclusivamente consultivo/de reportes? | Media | Actors.md (ACT-002) |
| BQ-059 | El rol "Supervisor", ¿supervisa Taller, Servicios de Campo, o ambos? ¿Sus funciones se solapan con las del Gerente? | Media | Actors.md (ACT-003) |
| BQ-060 | El rol "Recepcionista", ¿también gestiona el agendamiento de Servicios de Campo, o esa función corresponde a otro actor? | Media | Actors.md (ACT-004) |
| BQ-061 | El rol "Vendedor", ¿registra el cobro directamente o siempre deriva a un cajero independiente? | Media | Actors.md (ACT-005) |
| BQ-062 | El rol "Contador", ¿operará directamente en el ERP, o solo recibirá reportes/exportaciones hacia un sistema contable externo? | Media | Actors.md (ACT-008) |
| BQ-063 | ¿Cuál es la matriz real de permisos por rol y módulo? (la matriz propuesta en Actors.md, sección 7, es una hipótesis de trabajo). | **Alta** | Actors.md (sección 7), RF-010 |

## 12. Reportes

| ID | Pregunta | Prioridad | Relacionado con |
|---|---|---|---|
| BQ-054 | Más allá de los reportes operativos básicos (ventas, inventario, OT, caja), ¿qué reportes específicos necesita la Gerencia para la toma de decisiones (ej. rentabilidad por línea de negocio, rotación de inventario, indicadores de taller)? | Media | RF-077 |
| BQ-080 | ¿En qué formato deben poder exportarse los reportes (PDF, Excel, ambos)? | Baja | — |
| BQ-081 | ¿Los reportes se generan bajo demanda, o también deben programarse/enviarse automáticamente (ej. reporte diario de caja por correo)? | Baja | — |

## 13. SUNAT y Facturación Electrónica

| ID | Pregunta | Prioridad | Relacionado con |
|---|---|---|---|
| BQ-050 | 🟡 **PARCIALMENTE RESUELTA.** ¿La facturación electrónica (boleta/factura) es obligatoria desde el lanzamiento del sistema, o puede diferirse a una fase posterior? ¿Cómo se definen las series y correlativos? | **Alta** | RF-044, RF-081 |
| BQ-051 | En caso de requerirse facturación electrónica, ¿ya existe un proveedor OSE/PSE contratado, o debe evaluarse como parte del proyecto? | **Alta** | Actors.md (ACT-011) |
| BQ-072 | ¿Durante cuánto tiempo deben conservarse los comprobantes y registros contables según la normativa aplicable a ISARMIN? | Media | Non-Functional-Requirements.md (RNF-022) |
| BQ-082 | ¿La empresa ya cuenta con RUC y régimen tributario activo para operar con comprobantes electrónicos, o es un trámite pendiente? | Alta | — |

> **Respuesta parcial a BQ-050 (fuente: `PROJECT_SCOPE.md`, sección "Restricciones"):** "La integración con SUNAT será considerada en el diseño, aunque podrá implementarse en una fase posterior." Esto confirma que **no es obligatoria desde el lanzamiento**, pero **no responde** cómo se definen series/correlativos ni qué ocurre mientras tanto (¿se emite boleta/factura físico o manual en el intervalo?). BQ-050 permanece **parcialmente abierta** por ese motivo, y BQ-051, BQ-072 y BQ-082 siguen totalmente pendientes.

## 14. Infraestructura

| ID | Pregunta | Prioridad | Relacionado con |
|---|---|---|---|
| BQ-055 | ¿Qué modelo(s) de impresora se usará para comprobantes/tickets? ¿Formato térmico (58/80mm) o A4? ¿Ya existen equipos comprados? | Media | Actors.md (ACT-012) |
| BQ-056 | ¿Los productos actuales ya tienen codificación de barras, o debe generarse desde el sistema? | Media | Actors.md (ACT-013) |
| BQ-065 | ¿Bajo qué carga concurrente debe cumplirse el tiempo de respuesta objetivo (< 3 segundos, RNF-001)? | Media | Non-Functional-Requirements.md |
| BQ-066 | ¿Qué versiones mínimas de navegador deben soportarse? ¿Se requiere soporte para dispositivos móviles/tablets? | Media | Non-Functional-Requirements.md (RNF-003) |
| BQ-067 | ¿Se requiere que el sistema sea utilizable desde tablets o celulares (relevante especialmente para Técnicos de Campo)? | Alta | Non-Functional-Requirements.md (RNF-004) |
| BQ-068 | ¿Cuántos usuarios concurrentes se esperan hoy? ¿Y en un horizonte de 3 años? | **Alta** | Non-Functional-Requirements.md (RNF-005) |
| BQ-069 | ¿Cuál es el volumen estimado de productos en catálogo, ventas por día y equipos de taller por mes? | Alta | Non-Functional-Requirements.md (RNF-006) |
| BQ-070 | ¿Qué frecuencia y retención de respaldos se espera? ¿Dónde deben almacenarse (USB externo, nube gratuita, NAS)? ¿Cuál es el tiempo máximo aceptable para restaurar el sistema ante una falla? | **Alta** | Non-Functional-Requirements.md (RNF-007, RNF-008) |
| BQ-071 | Dado que el servidor inicial es una PC de escritorio sin redundancia declarada, ¿qué nivel de tolerancia a fallas (corte eléctrico, falla de disco) es aceptable para el negocio? | Alta | Non-Functional-Requirements.md (RNF-009) |
| BQ-073 | ¿Cuál es el nivel de familiaridad con sistemas informáticos del personal que usará el sistema (vendedores, recepcionistas, técnicos)? | Media | Non-Functional-Requirements.md (RNF-023) |

## 15. Gestión del Proyecto

| ID | Pregunta | Prioridad | Relacionado con |
|---|---|---|---|
| BQ-083 | ¿El stack tecnológico propuesto en el README (React + TypeScript, ASP.NET Core, PostgreSQL, Tailwind) es una decisión definitiva, o sigue en evaluación? | **Alta** | `TECH_STACK.md` (vacío), `DECISIONS.md` (vacío) |
| BQ-084 | ¿Existe un cronograma o fecha objetivo de lanzamiento para la primera versión del sistema? | Media | `PROJECT_ROADMAP.md` (vacío) |
| BQ-085 | ¿Existen restricciones de presupuesto o de tiempo adicionales no mencionadas en el contexto del proyecto? | Media | — |
| BQ-086 | ¿Existen datos actuales (Excel, cuadernos, formatos físicos) de clientes, productos o historial de taller que deban migrarse al arranque del sistema? | Alta | — |

## 16. Mantenimiento de este documento

Este registro debe actualizarse cada vez que:
1. Se obtenga una respuesta del cliente (agregar columna "Respuesta" y fecha, y actualizar el estado en la sección 3).
2. Surja una nueva pregunta durante el levantamiento (asignar el siguiente ID disponible, nunca reutilizar un ID retirado).
3. Una pregunta quede obsoleta (marcarla como retirada con justificación, no eliminarla del documento, para conservar la trazabilidad de la decisión).

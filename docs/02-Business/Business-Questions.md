# Business-Questions.md — Preguntas de Negocio Pendientes de Validación

## 1. Propósito

Este es el documento más importante de la fase de análisis: consolida **todas** las preguntas que deben resolverse con el cliente (ISARMIN PERÚ S.A.C.) antes de aprobar el modelo de dominio y comenzar el diseño de arquitectura. Sigue la técnica de **"Elicitation Results Register"** de BABOK v3 — un registro vivo que se actualiza a medida que se obtienen respuestas.

Cada pregunta referenciada como `[PV]` en `01-Requirements/` y `02-Business/` tiene aquí su origen formal. **Ninguna pregunta de este documento fue respondida por el analista**: todas están abiertas salvo las que se indican explícitamente como resueltas o parcialmente resueltas, cuya respuesta proviene de fuentes oficiales del cliente (`PROJECT_SCOPE.md` y la validación directa del propietario de ISARMIN PERÚ S.A.C. del 2026-07-18), no de una suposición del analista.

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
| 89 | 20 | 13 | 56 |

**Última actualización:** 2026-07-18 — el propietario de ISARMIN PERÚ S.A.C. validó directamente el funcionamiento real del negocio (contexto de la empresa, usuarios reales, inventario compartido, operación de Taller y Servicios de Campo, conectividad de campo, roles y permisos, caja, compras/costos, migración inicial e infraestructura), resolviendo total o parcialmente 33 preguntas y añadiendo 5 nuevas (BQ-087 a BQ-091) detectadas durante la validación. Ver el detalle en cada sección correspondiente. Se confirmó además, por fuera de este registro de preguntas, que **Garantías** sí forma parte del MVP (integrada a Taller/OT, no como módulo independiente) y que **Auditoría** es una capacidad transversal obligatoria (bitácoras básicas), no un módulo funcional propio — ver `PROJECT_SCOPE.md`.

**Hallazgo que requiere seguimiento inmediato:** la resolución de BQ-052/BQ-053 (conectividad de Servicios de Campo) es la más relevante de esta ronda — reduce el riesgo crítico RSK-004 identificado en la auditoría técnica. El hallazgo de BQ-033 (posible tensión entre el orden "recojo → cobro" narrado por el cliente y la regla ya confirmada RN-001) y la nueva pregunta BQ-089 (¿quién registra la recepción de equipos si el rol Ventas no tiene acceso a Taller?) deben aclararse puntualmente con el propietario antes de fijar la máquina de estados de la Orden de Trabajo y la matriz de permisos definitiva.

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

> **Contexto adicional confirmado por el propietario (no resuelve BQ-001/BQ-003 formalmente, pero los contextualiza):** se validó con ejemplos reales que el inventario único compartido funciona exactamente como se había inferido — ej. los carbones para herramientas se venden como producto, se usan como repuesto en reparaciones y se compran en cantidad porque también se revenden a otros técnicos; el cable eléctrico se vende en tienda y también se usa en instalaciones de campo. Esto confirma RN-006 con evidencia concreta. Dado que la operación tiene solo 2 usuarios permanentes (ver sección 11), el riesgo de concurrencia real sobre el mismo producto es bajo en la práctica, pero **la regla de negocio explícita (BQ-001: ¿stock negativo permitido?, BQ-003: ¿mecanismo de reserva?) sigue sin respuesta directa** y debe confirmarse igual, aunque con prioridad de riesgo reducida.

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
| BQ-087 | *(Nueva)* ¿Existen devoluciones de productos vendidos? ¿Bajo qué condiciones y plazo se aceptan? | **Alta** | Business-Rules.md, RF-038 |

> **Nota:** BQ-019 y BQ-087 fueron explícitamente reconfirmadas como pendientes por el propio propietario ("Si existen ventas al crédito", "Si existen devoluciones" — puntos 5 y 6 de su lista de información pendiente). No se recibió información nueva sobre medios de pago (BQ-012): sigue siendo una omisión notable, ya que el levantamiento no mencionó efectivo, tarjeta, transferencia ni billeteras digitales en ningún momento.

## 7. Compras

| ID | Pregunta | Prioridad | Relacionado con |
|---|---|---|---|
| BQ-020 | ✅ **RESUELTA.** ¿Las órdenes de compra requieren aprobación formal? ¿A partir de qué monto y por parte de qué rol? | Media | RF-033, Business-States.md (ST-006) |
| BQ-021 | 🟡 **PARCIALMENTE RESUELTA.** ¿Qué método de valorización de inventario utiliza (o debería utilizar) la empresa: PEPS, promedio ponderado, costo estándar u otro? | **Alta** | RN-013, RF-035 |
| BQ-022 | ✅ **RESUELTA.** ¿Se realizan compras directas sin orden de compra previa (compras menores o urgentes)? ¿Bajo qué condiciones? | Media | RF-037 |
| BQ-023 | 🟡 **CONTEXTUALIZADA.** ¿Se acepta la recepción parcial de una orden de compra, o toda la mercadería debe recibirse en un solo evento? | Media | Business-States.md (ST-006) |
| BQ-025 | ¿Existen plazos de pago pactados con proveedores (crédito, contado)? | Baja | — |
| BQ-026 | ¿Se evalúa o califica a los proveedores de alguna forma (precio, tiempo de entrega, calidad)? | Baja | — |

> **Respuesta a BQ-020 y BQ-022 (fuente: validación con el propietario):** "Actualmente las compras se realizan directamente a proveedores u otras tiendas." No se describió ningún proceso de aprobación formal ni de orden de compra (OC) previa — el propietario es quien decide y ejecuta la compra directamente, siendo el único que las realiza. Se confirma que **no existe (ni se requiere para el MVP) un flujo formal de aprobación de OC**; el modelo de compra directa es el modo normal de operar. **Impacto:** el estado "Aprobada" de `Business-States.md` (ST-006) puede simplificarse u omitirse en el MVP; `Functional-Requirements.md` (RF-033) debe re-priorizarse como registro simple de compra, no como flujo de aprobación. BQ-023 queda **contextualizada, no eliminada**: como no hay OC formal, la recepción parcial no aplica hoy, pero podría necesitarse si el negocio crece y formaliza compras a proveedores mayores.
>
> **Respuesta parcial a BQ-021 (fuente: validación con el propietario):** el propietario no confirmó un método exacto, pero se adoptó como **propuesta técnica tentativa: costo promedio ponderado**, sujeta a confirmación final antes de fijarse en el modelo de datos. `Business-Rules.md` (RN-013) debe reflejar esto como decisión tentativa, no definitiva.

## 8. Caja

| ID | Pregunta | Prioridad | Relacionado con |
|---|---|---|---|
| BQ-027 | ✅ **RESUELTA.** ¿Cuántas cajas o puntos de cobro operan (u operarán) simultáneamente? | Media | Business-Processes.md (BP-005) |
| BQ-028 | ✅ **RESUELTA.** ¿Los cobros de Taller y Servicios de Campo pasan por la misma caja que las ventas de tienda, o se manejan de forma separada? | Media | RF-050, RF-070 |
| BQ-029 | ¿El arqueo de caja debe cuadrar por cada medio de pago por separado (efectivo, tarjeta, transferencia), o solo el total general? | Media | RF-048 |
| BQ-030 | 🟡 **PARCIALMENTE RESUELTA.** ¿Existe el concepto formal de "caja" con apertura y cierre de turno, o los cobros se registran sin ese control? | **Alta** | RF-046, Business-Rules.md (RN-014) |
| BQ-031 | En caso de existir arqueo de caja, ¿qué ocurre operativamente cuando el monto físico no coincide con el monto teórico del sistema? | Media | RF-048, Business-Rules.md (RN-015) |
| BQ-088 | *(Nueva)* ¿Se requiere separar explícitamente el dinero personal del propietario del dinero del negocio dentro del control de caja del sistema? | **Alta** | RF-046, Business-Rules.md (RN-014) |

> **Respuesta a BQ-027 y BQ-028 (fuente: validación con el propietario):** existe una única caja física (2 usuarios, un solo local); los ingresos de venta de productos, reparaciones y servicios técnicos se manejan todos como una sola caja, sin separación por línea de negocio.
>
> **Respuesta parcial a BQ-030 (fuente: validación con el propietario):** se confirma que el ERP **debe implementar** registro de ingresos, registro de egresos, control diario y cierre de caja con reportes — es decir, sí se requiere el concepto formal de caja. Lo que **queda explícitamente pendiente** (reconocido así por el propio propietario): el monto de apertura inicial, la frecuencia exacta de los cierres, y la pregunta recién incorporada **BQ-088** sobre la separación entre dinero personal y del negocio — un riesgo real y frecuente en negocios familiares pequeños que debe resolverse antes de diseñar el módulo de Caja, ya que afecta directamente su modelo de datos (¿existe un solo "fondo" o dos fondos lógicos separados?).

## 9. Taller

| ID | Pregunta | Prioridad | Relacionado con |
|---|---|---|---|
| BQ-032 | ✅ **RESUELTA (flujo general).** ¿Cuáles son los estados reales por los que pasa una Orden de Trabajo en la operación actual? (la propuesta en Business-States.md ST-001 es una hipótesis a confirmar o corregir). | **Alta** | RF-063, Business-States.md (ST-001) |
| BQ-033 | 🟡 **PARCIALMENTE RESUELTA — REQUIERE ACLARACIÓN PUNTUAL.** ¿El pago debe registrarse estrictamente antes de la entrega física del equipo, o pueden ocurrir en el mismo acto (ej. el cliente paga al recibir)? | Alta | RN-001, Business-States.md (ST-001) |
| BQ-034 | Si el cliente rechaza la cotización de reparación, ¿qué ocurre con el equipo y con el diagnóstico ya realizado? ¿Se cobra algún concepto (ej. revisión técnica) aunque no se repare? ¿Cómo se evidencia formalmente la aprobación del cliente (firma física, digital, verbal)? | **Alta** | RF-056, Business-Rules.md (RN-016) |
| BQ-035 | 🟡 **PARCIALMENTE RESUELTA (alcance confirmado).** ¿La empresa ofrece garantía sobre las reparaciones? ¿Cuál es su duración estándar? ¿Cubre solo el repuesto reemplazado, la mano de obra, o ambos? | Media | RF-061, Business-Catalogs.md (CAT-017) |
| BQ-036 | Si un equipo reingresa por la misma falla dentro del período de garantía, ¿cómo se vincula la nueva OT a la reparación original? ¿Se cobra o es gratuito? | Media | RF-062 |
| BQ-077 | ¿Existen tiempos de atención esperados o comprometidos (SLA) para el diagnóstico y la reparación? | Baja | — |
| BQ-078 | ¿Qué ocurre si el cliente no recoge su equipo tras ser notificado de que está listo? ¿Existe un plazo o política de almacenamiento/abandono? | Media | — |
| BQ-091 | *(Nueva)* ¿Qué datos exactos contienen hoy los recibos/notas de recepción manuales de equipos (nombre, DNI, equipo, falla reportada, fecha, firma, etc.)? | Media | RF-053 |

> **Respuesta a BQ-032 (fuente: validación con el propietario):** el propietario confirmó el flujo real: *entrega del equipo → registro manual de datos → comprobante de recepción → diagnóstico → informe de costo/reparación estimada → aprobación del cliente → reparación (con repuestos del inventario) → pruebas → recojo del equipo → cobro*. Esto coincide, en general, con la hipótesis de `Business-States.md` (ST-001), y permite fijar los nombres de estado definitivos en la próxima actualización de ese documento.
>
> ⚠️ **Hallazgo importante sobre BQ-033 — posible tensión con RN-001, no resuelta por asunción:** la secuencia entregada por el propietario lista textualmente *"Cliente recoge equipo ↓ Se realiza cobro"*, es decir, el recojo aparece **antes** del cobro en el orden narrado. Esto podría leerse en tensión con la regla ya confirmada RN-001 ("No puede entregarse un equipo sin registrar el pago"). La interpretación más probable (no asumida como definitiva) es que "cliente recoge equipo" describe el **inicio de la visita** de recojo (el cliente llega al local), y el cobro ocurre **dentro de esa misma visita, antes de la entrega física real** — no que el equipo salga del local sin pago. **Se requiere una confirmación puntual y explícita del propietario sobre este matiz exacto antes de fijar la máquina de estados de la OT**, en lugar de asumir cualquiera de las dos lecturas.
>
> **Respuesta parcial a BQ-035 (fuente: validación con el propietario, y validación de alcance):** se confirma que **Garantías formará parte del MVP**, integrada como funcionalidad de Taller/Órdenes de Trabajo (no como módulo independiente complejo). Queda pendiente si existe hoy algún control informal de garantías (pregunta reconocida como pendiente por el propio propietario) y las reglas exactas de duración/cobertura (BQ-036 sigue totalmente abierta).

## 10. Servicios de Campo

| ID | Pregunta | Prioridad | Relacionado con |
|---|---|---|---|
| BQ-037 | 🟡 **PARCIALMENTE RESUELTA.** ¿Cómo se agenda hoy un servicio de campo? ¿Existe un calendario o ruta de técnicos? ¿Cómo se decide qué técnico asignar? | Alta | RF-065, Business-States.md (ST-005) |
| BQ-038 | ✅ **RESUELTA.** ¿Un servicio de campo se cotiza de forma similar a una reparación de Taller, o tiene un modelo comercial distinto (ej. tarifa fija por visita)? | Media | RF-068 |
| BQ-039 | ¿Cómo se registra la conformidad del cliente al finalizar un servicio de campo (firma, foto, checklist)? ¿Qué ocurre si el cliente no está conforme? | Alta | RF-069, Business-States.md (ST-005) |
| BQ-040 | 🟡 **PARCIALMENTE RESUELTA.** ¿Cómo se cobra un servicio de campo? ¿En el sitio (requiere medio de pago móvil) o posteriormente en el local? | Alta | RF-070 |
| BQ-052 | ✅ **RESUELTA PARA EL MVP.** ¿Los técnicos de campo necesitarán registrar información del servicio (diagnóstico, materiales, conformidad) mientras están fuera de la red local de la empresa? | **Alta** | RF-071, Non-Functional-Requirements.md (RNF-014) |
| BQ-053 | ✅ **RESUELTA PARA EL MVP.** Si se requiere conectividad remota para Servicios de Campo, ¿qué mecanismo es aceptable dado que la infraestructura declarada es "PC de escritorio + red local" (app con sincronización diferida, acceso remoto seguro, dispositivo con datos móviles)? | **Alta** | RF-071, Non-Functional-Requirements.md (RNF-014) |

> **Respuesta a BQ-052 y BQ-053 — la resolución más importante de esta ronda de validación (fuente: validación con el propietario):** "Los trabajos de campo son realizados principalmente por el propietario. Actualmente no existe registro digital desde campo. Para la primera versión: se mantiene como prioridad una aplicación web administrativa. No se considera obligatoria una aplicación móvil inicial. Sin embargo, la arquitectura debe permitir una futura ampliación para: registro desde celular, fotografías, ubicación, evidencia del trabajo, firma del cliente." **Esto significa que, para el MVP, el registro de un servicio de campo se hace al volver al local (no en tiempo real desde el sitio del cliente)** — no se requiere resolver conectividad offline ni una app móvil en esta fase, pero **la arquitectura debe diseñarse con un punto de extensión explícito** para esa ampliación futura (ej. API ya preparada para ser consumida por un cliente móvil más adelante). **Impacto directo:** el riesgo RSK-004 de `Business-Risks.md` se reclasifica de Crítico a Medio (ver esa actualización); ADR-002 de `DECISIONS.md` ("Aplicación Web") deja de estar en tensión con el alcance de Servicios de Campo.
>
> **Respuesta parcial a BQ-038 y BQ-037 (fuente: validación con el propietario):** el proceso de campo confirmado es *cliente solicita → se evalúa el trabajo → se realiza una cotización → se determinan materiales → los materiales salen del inventario de la tienda → se realiza el trabajo → se entrega comprobante/cotización final*, análogo al de Taller (BQ-038 resuelta). Sobre BQ-037: hoy el propietario realiza estos trabajos casi en exclusiva, por lo que no existe un proceso formal de asignación entre varios técnicos — la pregunta pierde urgencia para el MVP pero reaparecerá si se contratan técnicos adicionales.
>
> **Respuesta parcial a BQ-040:** el proceso confirmado termina con "se entrega comprobante/cotización final" tras ejecutar el trabajo, lo que sugiere cobro en el mismo sitio/visita; el medio de pago exacto (BQ-012) sigue sin confirmar.

## 11. Usuarios y Roles

| ID | Pregunta | Prioridad | Relacionado con |
|---|---|---|---|
| BQ-041 | ✅ **RESUELTA.** ¿"Técnico" es un único rol para Taller y Campo, o deben existir roles distintos con permisos y necesidades de acceso diferentes (ej. el técnico de campo necesita acceso remoto)? | **Alta** | Actors.md (ACT-007), Business-Catalogs.md (CAT-005) |
| BQ-042 | 🟡 **PARCIALMENTE RESUELTA.** ¿Un usuario puede tener asignado más de un rol simultáneamente (ej. Vendedor que también es Cajero)? | Alta | RF-009 |
| BQ-043 | ¿Puede un usuario mantener sesión activa en más de un dispositivo simultáneamente? | Baja | RF-004 |
| BQ-044 | ¿Debe existir un tiempo de inactividad tras el cual la sesión se cierre automáticamente? | Media | RF-005 |
| BQ-045 | 🟡 **BAJA URGENCIA CONFIRMADA.** ¿Los permisos deben poder definirse por sucursal/almacén además de por rol, considerando la posibilidad de múltiples sedes a futuro (ver BQ-002)? | Baja | RF-010 |
| BQ-046 | ✅ **RESUELTA.** ¿Existirá algún mecanismo de autoregistro de usuarios, o todo usuario es creado exclusivamente por el Administrador? | Media | RF-001 |
| BQ-047 | ¿Qué política de seguridad de contraseñas se requiere (complejidad, expiración, bloqueo tras intentos fallidos)? | Media | RF-006, Non-Functional-Requirements.md (RNF-013) |
| BQ-048 | ¿El restablecimiento de contraseña debe ser autoservicio (ej. correo de recuperación) o exclusivamente gestionado por el Administrador? | Media | RF-007 |
| BQ-049 | 🟡 **PARCIALMENTE RESUELTA (alcance confirmado).** ¿Qué nivel de detalle se espera en la auditoría (solo cambios críticos, o todo acceso y consulta)? ¿Cuánto tiempo debe conservarse ese historial? | Media | RF-079 |
| BQ-057 | ✅ **RESUELTA.** El rol "Administrador", ¿es un perfil técnico (TI) o administrativo (Gerencia)? ¿Existirá más de un Administrador? | Media | Actors.md (ACT-001) |
| BQ-058 | ✅ **RESUELTA (rol descartado).** El rol "Gerente", ¿tiene funciones de aprobación sobre operaciones (descuentos, compras) o es exclusivamente consultivo/de reportes? | Media | Actors.md (ACT-002) |
| BQ-059 | ✅ **RESUELTA (rol descartado).** El rol "Supervisor", ¿supervisa Taller, Servicios de Campo, o ambos? ¿Sus funciones se solapan con las del Gerente? | Media | Actors.md (ACT-003) |
| BQ-060 | 🟡 **PARCIALMENTE RESUELTA — NUEVA AMBIGÜEDAD DETECTADA.** El rol "Recepcionista", ¿también gestiona el agendamiento de Servicios de Campo, o esa función corresponde a otro actor? | Media | Actors.md (ACT-004) |
| BQ-061 | ✅ **RESUELTA.** El rol "Vendedor", ¿registra el cobro directamente o siempre deriva a un cajero independiente? | Media | Actors.md (ACT-005) |
| BQ-062 | ✅ **RESUELTA (rol no existe en el MVP).** El rol "Contador", ¿operará directamente en el ERP, o solo recibirá reportes/exportaciones hacia un sistema contable externo? | Media | Actors.md (ACT-008) |
| BQ-063 | ✅ **RESUELTA.** ¿Cuál es la matriz real de permisos por rol y módulo? (la matriz propuesta en Actors.md, sección 7, es una hipótesis de trabajo). | **Alta** | Actors.md (sección 7), RF-010 |
| BQ-089 | *(Nueva — Alta)* El rol "Ventas" entregado por el propietario no incluye explícitamente acceso a Taller/Órdenes de Trabajo, pero el "Área 1: Tienda/Recepción" es donde físicamente se reciben y entregan los equipos. ¿Quién registra la recepción y entrega de equipos en el sistema: Ventas, Técnico, o ambos? | **Alta** | Actors.md, RF-051, RF-059 |

> **Respuesta a la mayoría de este bloque — simplificación real de roles (fuente: validación con el propietario, sección "Usuarios actuales" y "Usuarios y permisos iniciales"):** en la operación real de ISARMIN existen solo **dos usuarios permanentes**: el **Propietario** (Administrador con acceso completo: configuración, inventario, compras, ventas, taller, servicios, reportes — y además es el técnico principal que hace reparaciones, diagnósticos y servicios de campo) y la **Encargada de Ventas** (atención al cliente, ventas de tienda, registro de ventas, caja, consulta de inventario, apoyo administrativo). Existen además **trabajadores temporales** (ayudantes de instalaciones) que **no administran información y no tendrán usuario propio** en esta fase.
>
> **Esto confirma que los roles especulativos "Gerente", "Supervisor", "Recepcionista" y "Contador" de `Actors.md` no existen como roles distintos en la operación real** — sus funciones, en la medida en que existen, las concentra el Propietario (Administrador) o no aplican en el MVP. `Actors.md` debe actualizarse para reflejar el catálogo real de 3 roles internos (Administrador, Ventas, Técnico) + Cliente y Proveedor como actores externos sin acceso al sistema, dejando los roles descartados documentados como "no utilizados en el MVP" en lugar de eliminarlos silenciosamente (para no perder la trazabilidad de por qué se descartaron).
>
> ⚠️ **Nueva ambigüedad detectada (BQ-089), no resuelta por asunción:** la matriz de permisos entregada para el rol "Ventas" no menciona acceso a Taller/Órdenes de Trabajo, pero la descripción del "Área 1: Tienda/Recepción" sí incluye "Recepción de equipos para reparación" y "Entrega de equipos reparados" como actividades de esa misma área. No se debe asumir si la Encargada de Ventas registra la recepción/entrega en el sistema o si esa función queda reservada al Técnico (Propietario) — debe confirmarse explícitamente.

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
| BQ-068 | 🟡 **PARCIALMENTE RESUELTA.** ¿Cuántos usuarios concurrentes se esperan hoy? ¿Y en un horizonte de 3 años? | **Alta** | Non-Functional-Requirements.md (RNF-005) |
| BQ-069 | ¿Cuál es el volumen estimado de productos en catálogo, ventas por día y equipos de taller por mes? | Alta | Non-Functional-Requirements.md (RNF-006) |
| BQ-070 | ¿Qué frecuencia y retención de respaldos se espera? ¿Dónde deben almacenarse (USB externo, nube gratuita, NAS)? ¿Cuál es el tiempo máximo aceptable para restaurar el sistema ante una falla? | **Alta** | Non-Functional-Requirements.md (RNF-007, RNF-008) |
| BQ-071 | Dado que el servidor inicial es una PC de escritorio sin redundancia declarada, ¿qué nivel de tolerancia a fallas (corte eléctrico, falla de disco) es aceptable para el negocio? | Alta | Non-Functional-Requirements.md (RNF-009) |
| BQ-073 | ¿Cuál es el nivel de familiaridad con sistemas informáticos del personal que usará el sistema (vendedores, recepcionistas, técnicos)? | Media | Non-Functional-Requirements.md (RNF-023) |
| BQ-090 | *(Nueva)* ¿Cuáles son las características del equipo que se usará como servidor (procesador, RAM, almacenamiento, sistema operativo)? | Media | Non-Functional-Requirements.md (RNF-009) |

> **Respuesta parcial a BQ-068 (fuente: validación con el propietario):** se confirma un número de usuarios muy bajo en la operación real: **2 usuarios permanentes** (Propietario y Encargada de Ventas), más eventuales trabajadores temporales sin acceso al sistema. Esto reduce sustancialmente el riesgo de concurrencia (ver actualización de RSK-001 en `Business-Risks.md`), aunque el horizonte a 3 años no fue confirmado. **BQ-069, BQ-071 (specs del servidor → BQ-090) y BQ-073 siguen explícitamente pendientes** — de hecho, el propio propietario reconoció "cantidad aproximada de productos", "reparaciones mensuales", "servicios de campo mensuales" y "características del equipo servidor" como información todavía no entregada.

## 15. Gestión del Proyecto

| ID | Pregunta | Prioridad | Relacionado con |
|---|---|---|---|
| BQ-083 | ✅ **RESUELTA.** ¿El stack tecnológico propuesto en el README (React + TypeScript, ASP.NET Core, PostgreSQL, Tailwind) es una decisión definitiva, o sigue en evaluación? | **Alta** | `TECH_STACK.md`, `DECISIONS.md` |
| BQ-084 | ¿Existe un cronograma o fecha objetivo de lanzamiento para la primera versión del sistema? | Media | `PROJECT_ROADMAP.md` |
| BQ-085 | ¿Existen restricciones de presupuesto o de tiempo adicionales no mencionadas en el contexto del proyecto? | Media | — |
| BQ-086 | ✅ **RESUELTA.** ¿Existen datos actuales (Excel, cuadernos, formatos físicos) de clientes, productos o historial de taller que deban migrarse al arranque del sistema? | Alta | — |

> **Respuesta a BQ-083 (fuente: `DECISIONS.md` ADR-001 a ADR-005, y reconfirmado por el propietario: "La decisión de utilizar una aplicación web se mantiene. La solución debe estar preparada para crecimiento futuro, más usuarios, nuevas sucursales, nuevos módulos"):** el stack y el patrón arquitectónico son definitivos, no una propuesta.
>
> **Respuesta a BQ-086 (fuente: validación con el propietario):** la empresa ya está operando, no se parte de un sistema vacío. **Habrá migración inicial, pero limitada al catálogo de inventario**: productos, categorías, stock disponible, precio actual y costo aproximado (si existe la información). **No existe ningún historial digital de ventas, reparaciones o compras anteriores** — la trazabilidad transaccional (Kardex, historial de OT, historial de ventas) comienza recién con la puesta en marcha del ERP. Esto simplifica considerablemente el esfuerzo de migración de datos frente al riesgo originalmente identificado en RSK-011.

## 16. Mantenimiento de este documento

Este registro debe actualizarse cada vez que:
1. Se obtenga una respuesta del cliente (agregar columna "Respuesta" y fecha, y actualizar el estado en la sección 3).
2. Surja una nueva pregunta durante el levantamiento (asignar el siguiente ID disponible, nunca reutilizar un ID retirado).
3. Una pregunta quede obsoleta (marcarla como retirada con justificación, no eliminarla del documento, para conservar la trazabilidad de la decisión).

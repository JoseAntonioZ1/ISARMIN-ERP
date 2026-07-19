# Business-Rules.md — Reglas de Negocio

## 1. Propósito

Documenta las reglas de negocio (RN) que restringen o condicionan el comportamiento del sistema, siguiendo el enfoque de **Business Rules Analysis de BABOK v3** (una regla de negocio es una directriz que restringe una acción, es atómica, declarativa y no negociable a nivel de implementación — a diferencia de un requerimiento funcional, que describe una capacidad).

Toda regla debe tener una **justificación** que explique su origen: o bien está declarada explícitamente en la documentación fuente, o se infiere lógicamente de un proceso ya descrito, o es una hipótesis del analista pendiente de confirmación.

## 2. Convención

Igual que en los demás documentos: **[C]** Confirmado, **[I]** Inferido, **[PV]** Pendiente de Validación.

## 3. Reglas confirmadas originalmente

| ID | Regla | Estado | Módulo | Justificación |
|---|---|---|---|---|
| RN-001 | Un equipo puede pasar a estado **"Listo para Entrega" con el pago pendiente** — el pago **no** es un prerrequisito bloqueante para la entrega. Al momento de entregar el equipo se debe registrar: fecha y hora, usuario que realiza la entrega, estado del pago, monto pagado y saldo pendiente (si corresponde). El sistema debe permitir cuatro variantes de pago: pago completo antes de la entrega, pago completo al momento de la entrega, adelanto, o saldo pendiente autorizado. **La entrega con saldo pendiente requiere autorización explícita del Administrador/Propietario** (no de cualquier usuario), y el sistema debe registrar: usuario que autorizó, monto pendiente, y fecha o referencia de pago pendiente cuando corresponda. En V1 no se manejan políticas de crédito complejas ni límites automáticos (ver RN-030). | [C] — **corregida el 2026-07-18** (ver nota de corrección), autorización resuelta (**BQ-093**) | Taller / Caja | Corrección explícita del propietario al resolver **BQ-033** y **BQ-093**, reemplazando la regla original. |
| RN-002 | Todo movimiento de inventario debe quedar registrado. | [C] | Inventario | Declarada explícitamente; fundamento de la trazabilidad exigida al proyecto. |
| RN-003 | No puede venderse un producto sin stock. | [C] | Ventas / Inventario | Declarada explícitamente. |
| RN-004 | Una Orden de Trabajo debe pertenecer a un cliente. | [C] | Taller | Declarada explícitamente. |
| RN-005 | Cada equipo tendrá un historial único. | [C] | Taller | Declarada explícitamente. |

> ⚠️ **Historial de corrección de RN-001 (importante para trazabilidad de decisiones, no se oculta el cambio):**
> - **Texto original** (declarado en la documentación fuente del proyecto): "No puede entregarse un equipo sin registrar el pago."
> - **Primera validación (2026-07-18):** el propietario describió el proceso como *"Cliente recoge equipo → Se realiza cobro"*, lo que generó la duda BQ-033 sobre si el pago era estrictamente previo a la entrega o podía ocurrir en el mismo acto. Se dejó la regla original sin cambios a la espera de aclaración puntual.
> - **Segunda validación (2026-07-18, respuesta directa a BQ-033):** el propietario corrigió explícitamente la regla — el flujo real es: *equipo listo → cliente retorna a recogerlo → en ese momento se cobra y se entrega*, y el sistema **debe permitir explícitamente** que el equipo salga con saldo pendiente autorizado (no solo pago completo). Esto **reemplaza** la regla original, no es una simple aclaración de orden.
> - **Nueva pregunta derivada (no resuelta por asunción):** ¿quién puede autorizar que un equipo salga con saldo pendiente — cualquier usuario que realiza la entrega, o requiere aprobación específica del Administrador/Propietario? → **BQ-093**.

## 4. Reglas de Inventario (compartido entre líneas de negocio)

| ID | Regla | Estado | Justificación | Referencia |
|---|---|---|---|---|
| RN-006 | El inventario es único y compartido entre Tienda Comercial, Taller y Servicios de Campo; ningún movimiento puede registrarse fuera de este inventario centralizado. | [C] | Confirmado directamente por el propietario (2026-07-18) con ejemplos reales: los carbones para herramientas se venden, se usan como repuesto y se compran en volumen para reventa a otros técnicos; el cable eléctrico se vende en tienda y se usa en instalaciones de campo. No existen almacenes separados. | RF-032 |
| RN-007 | Ante una posible concurrencia entre operaciones que disputan el mismo stock (ej. una venta en tienda y un consumo de repuesto en taller sobre el mismo producto), el sistema valida disponibilidad al momento de confirmar cada operación, y rechaza la operación si no hay stock suficiente (sin stock negativo, ver RN-008). | [C] | El propietario confirmó (2026-07-18, tercera ronda) que el inventario no debe permitir stock negativo en operaciones normales, lo cual resuelve implícitamente el mecanismo: validación en el momento de confirmación, sin necesidad de un sistema de reserva previa. Riesgo de concurrencia real además reducido por operar con solo 2 usuarios permanentes. | BQ-001, BQ-003 |
| RN-008 | El stock de un producto no puede quedar en valor negativo en operaciones normales (ventas, consumo en Taller o Campo). La única excepción es un **ajuste manual de inventario**, que solo puede realizar el **Administrador/Propietario**, indicando el motivo. | [C] — resuelta (2026-07-18, tercera ronda) | Confirmado explícitamente por el propietario, cerrando la pregunta sobre stock negativo. | BQ-001 (resuelta) |

## 5. Reglas de Ventas

| ID | Regla | Estado | Justificación | Referencia |
|---|---|---|---|---|
| RN-009 | Una factura solo puede emitirse a clientes que cuenten con RUC registrado. | [I] | Regla estándar de la normativa tributaria peruana para este tipo de comprobante; se asume aplicable salvo indicación contraria. | RF-016, BQ-011 |
| RN-010 | Una venta solo puede anularse dentro de un plazo determinado y por un rol autorizado. | [PV] | No existe regla documentada sobre anulaciones; se plantea como necesaria para control interno. | BQ-013 |
| RN-011 | Los descuentos aplicados a una venta no pueden superar un porcentaje máximo sin autorización de un rol superior (ej. Supervisor o Gerente). | [PV] | Práctica común en retail/ERP; no confirmado para ISARMIN. | BQ-014 |
| RN-031 | El sistema permite registrar saldos pendientes autorizados en ventas, reparaciones (OT) y servicios de campo, siempre con autorización del Administrador/Propietario (extensión de RN-001 a todos los módulos de cobro). **No existe módulo de crédito formal**, límites de crédito automáticos, ni gestión de cobranzas en V1. | [C] — resuelta (2026-07-18, tercera ronda) | El propietario confirmó explícitamente que no se implementará un módulo formal de créditos/cuentas por cobrar/cobranzas; el único mecanismo de venta "a crédito" es el saldo pendiente autorizado ya definido para Taller, generalizado a Ventas y Campo. | RN-001, BQ-019 (resuelta) |
| RN-032 | Las devoluciones de productos vendidos se registran mediante un movimiento de inventario de tipo "Devolución" (CAT-013), con trazabilidad al comprobante de venta original. No se requiere un módulo o flujo complejo de gestión de devoluciones en V1. | [C] — resuelta (2026-07-18, tercera ronda) | Confirmado explícitamente por el propietario. | RN-002, CAT-013, BQ-087 (resuelta) |

## 6. Reglas de Compras

| ID | Regla | Estado | Justificación | Referencia |
|---|---|---|---|---|
| RN-012 | Toda compra registrada debe estar asociada a un proveedor previamente registrado en el sistema. | [I] | Consistencia referencial estándar; se infiere de la existencia del módulo Proveedores. | RF-018, RF-033 |
| RN-013 | El costo de compra de un producto debe actualizar su costo de referencia utilizando **costo promedio ponderado** como método de valorización. | [PV] — decisión tentativa | El propietario no confirmó un método exacto; se adopta el costo promedio ponderado como propuesta técnica tentativa del analista, sujeta a confirmación final antes de fijarse en el modelo de datos. | BQ-021 |
| RN-024 | Las compras se registran de forma directa (proveedor, fecha, productos, cantidad, costo) junto con su documento de compra (boleta/factura del proveedor), sin un flujo formal de orden de compra con aprobación previa. | [C] | Confirmado por el propietario: "Actualmente las compras se realizan directamente a proveedores u otras tiendas"; es el único que las realiza, sin necesidad de aprobación de un tercero. El flujo real confirmado es: Proveedor → Compra realizada → Documento de compra → Registro en sistema → Actualización de inventario. | RF-033, RF-034, BQ-020, BQ-022 |

## 7. Reglas de Caja

| ID | Regla | Estado | Justificación | Referencia |
|---|---|---|---|---|
| RN-014 | No puede registrarse el cobro de una venta o de una OT sin una caja abierta en el turno correspondiente. | [PV] — concepto de caja confirmado | El propietario confirmó que el ERP debe implementar registro de ingresos/egresos, control diario y cierre de caja; sí existe (o se requiere) el concepto formal de caja. El mecanismo exacto de apertura y la obligatoriedad estricta de "caja abierta" antes de cobrar siguen sin confirmarse en detalle. | BQ-030 |
| RN-015 | El cierre de caja requiere la conciliación entre el monto teórico (calculado por el sistema) y el monto físico declarado por el responsable. | [PV] | Confirmado que debe existir cierre de caja; qué ocurre exactamente si no cuadra sigue sin definirse. | BQ-031 |
| RN-025 | Existe una única caja para todo el negocio: los ingresos por venta de productos, reparaciones de Taller y Servicios de Campo se registran en la misma caja, sin separación por línea de negocio. | [C] | Confirmado por el propietario: solo 2 usuarios permanentes, un único local, y los tres tipos de ingreso se describen como parte del mismo control de caja. | RF-046, BQ-027, BQ-028 |
| RN-026 | El sistema debe permitir separar explícitamente el dinero personal del propietario del dinero del negocio dentro del control de caja. | [PV] | Riesgo real y frecuente en negocios familiares pequeños, señalado explícitamente como pendiente de validar por el propio propietario; no se define aún el mecanismo (¿un solo fondo con categorización de retiros personales, o dos fondos lógicos separados?). | BQ-088 |

## 8. Reglas de Taller

| ID | Regla | Estado | Justificación | Referencia |
|---|---|---|---|---|
| RN-016 | Una reparación no puede iniciarse sin que el cliente haya aprobado previamente la cotización correspondiente. | [I] | Se infiere directamente del proceso descrito: "Cliente aprueba → Se realiza reparación". | RF-056, BQ-034 |
| RN-030 | Si el cliente rechaza la cotización de reparación, el sistema debe permitir registrar el rechazo y dejar abierta la posibilidad de cobrar o no un concepto por el diagnóstico ya realizado, según decisión configurable del negocio caso por caso — no se asume que siempre es gratuito ni que siempre tiene costo fijo. | [C] — resuelta (2026-07-18, tercera ronda) | El propietario confirmó explícitamente que esta decisión no debe asumirse fija en ningún sentido; el sistema debe permitir ambas variantes. | RF-056, BQ-034 (resuelta) |
| RN-017 | Para V1, una garantía es simplemente un período (fecha de inicio y fecha de finalización) asociado a una reparación, que permite vincular una nueva Orden de Trabajo a una garantía vigente. No incluye tipos de garantía, condiciones de cobertura (repuesto vs. mano de obra) ni gestión avanzada — eso queda para una versión futura. | [C] — alcance V1 acotado y confirmado (2026-07-18) | El propietario redujo explícitamente el alcance de Garantías para V1 a este registro simple, descartando la complejidad de tipos/condiciones de cobertura planteada originalmente por el analista. | RF-061, RF-062, BQ-035, BQ-036 |
| RN-018 | Los repuestos consumidos en una reparación se descuentan del mismo inventario compartido definido en RN-006, sin excepción. | [I] | Corolario directo de RN-002/RN-006 aplicado al contexto de Taller. | RF-057 |

## 9. Reglas de Servicios de Campo

| ID | Regla | Estado | Justificación | Referencia |
|---|---|---|---|---|
| RN-019 | Un servicio de campo se cierra registrando: estado final del servicio, observaciones, y el usuario responsable del cierre. No se requiere firma digital, evidencias fotográficas avanzadas ni confirmación desde dispositivos móviles en V1 (consistente con RF-071: V1 es web). | [C] — resuelta (2026-07-18, tercera ronda) | El propietario confirmó estos tres campos como el mecanismo de conformidad para V1, descartando explícitamente mecanismos avanzados. | BQ-039 (resuelta) |
| RN-020 | Los materiales consumidos en un servicio de campo se descuentan del mismo inventario compartido definido en RN-006. | [I] | Corolario de RN-006 aplicado a Servicios de Campo. | RF-067 |

## 10. Reglas de Usuarios, Roles y Auditoría

| ID | Regla | Estado | Justificación | Referencia |
|---|---|---|---|---|
| RN-021 | Un usuario desactivado no puede autenticarse en el sistema, pero su historial de acciones se conserva íntegramente para auditoría. | [I] | Consecuencia de RF-003 y del requisito de auditoría (RNF-005/RNF-010). | RF-003, RF-078 |
| RN-022 | Toda acción crítica (anulación de venta, ajuste de inventario, eliminación lógica) debe quedar asociada al usuario que la ejecutó, sin excepción. | [I] | Consecuencia directa del requisito de auditoría declarado explícitamente (RNF-005). La auditoría se confirmó como capacidad transversal obligatoria (bitácoras básicas de acciones críticas), no como módulo funcional independiente. | RF-078 |
| RN-027 | Los trabajadores temporales (ayudantes de instalaciones, personal contratado para proyectos específicos) no tienen usuario propio ni acceso al sistema en V1. El sistema debe quedar preparado para registrar su participación en una Orden de Trabajo o Servicio de Campo (ej. "ayudante asignado") como dato de referencia, sin necesidad de crear una cuenta de usuario. | [C] — reconfirmado (2026-07-18, tercera ronda) | Confirmado por el propietario en dos rondas: estos trabajadores no administran información, pero su participación debe poder quedar registrada como referencia. | Actors.md, RF-090 |
| RN-028 | Los roles y sus permisos por módulo/acción deben ser configurables por el Administrador (no fijos en el código), para permitir crear nuevos roles a futuro sin cambios de arquitectura. | [C] | Confirmado explícitamente por el propietario (segunda ronda, 2026-07-18): "no asumir que existen departamentos separados... la arquitectura debe permitir crecimiento mediante roles configurables". | RF-008, RF-010 |
| RN-029 | El registro de la recepción de un equipo no es exclusivo de un rol: puede realizarlo el Administrador, el Técnico o Ventas, según quién atienda al cliente en ese momento; toda recepción debe generar una OT asociada y mantener trazabilidad del usuario que la registró. | [C] | Confirmado por el propietario (segunda ronda, 2026-07-18), resolviendo BQ-089. | RF-051, RF-052, RN-022 |

## 11. Regla transversal de integridad de datos

| ID | Regla | Estado | Justificación | Referencia |
|---|---|---|---|---|
| RN-023 | Ninguna entidad con historial transaccional asociado (cliente, producto, proveedor, usuario) debe eliminarse físicamente de la base de datos; debe aplicarse baja lógica para preservar la trazabilidad exigida al proyecto. Para **Clientes**, esto está confirmado en firme: "El sistema permitirá desactivar clientes manteniendo todo su historial asociado" (ventas, reparaciones, servicios, pagos, historial de equipos), sin eliminación física. | [C] para Clientes (confirmado 2026-07-18, resuelve BQ-064) / [I] para productos, proveedores y usuarios (aún por confirmar entidad por entidad) | Para Clientes: confirmado explícitamente por el propietario. Para el resto: consecuencia directa del objetivo de "trazabilidad completa de cada movimiento" declarado en el contexto de negocio, y de `AI_INSTRUCTIONS.md` ("nunca elimines funcionalidades/datos sin justificación"). | RF-014, RF-020 |

## 12. Reglas pendientes de levantamiento (sin hipótesis suficiente para inferir)

Las siguientes áreas no cuentan con información suficiente ni siquiera para plantear una hipótesis razonable, y deben levantarse directamente con el cliente sin proponer una regla provisional:

- Reglas de crédito a clientes (límites, plazos, mora).
- Reglas de asignación de técnicos (carga de trabajo, especialidad).
- Reglas de priorización cuando existen múltiples OT o servicios de campo simultáneos.
- Reglas de devolución de productos vendidos.

Ver [Business-Questions.md](../02-Business/Business-Questions.md) para el detalle de las preguntas asociadas.

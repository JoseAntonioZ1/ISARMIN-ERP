# Business-Rules.md — Reglas de Negocio

## 1. Propósito

Documenta las reglas de negocio (RN) que restringen o condicionan el comportamiento del sistema, siguiendo el enfoque de **Business Rules Analysis de BABOK v3** (una regla de negocio es una directriz que restringe una acción, es atómica, declarativa y no negociable a nivel de implementación — a diferencia de un requerimiento funcional, que describe una capacidad).

Toda regla debe tener una **justificación** que explique su origen: o bien está declarada explícitamente en la documentación fuente, o se infiere lógicamente de un proceso ya descrito, o es una hipótesis del analista pendiente de confirmación.

## 2. Convención

Igual que en los demás documentos: **[C]** Confirmado, **[I]** Inferido, **[PV]** Pendiente de Validación.

## 3. Reglas confirmadas originalmente (preservadas sin cambios)

| ID | Regla | Estado | Módulo | Justificación |
|---|---|---|---|---|
| RN-001 | No puede entregarse un equipo sin registrar el pago. | [C] | Taller / Caja | Declarada explícitamente en la documentación fuente. |
| RN-002 | Todo movimiento de inventario debe quedar registrado. | [C] | Inventario | Declarada explícitamente; fundamento de la trazabilidad exigida al proyecto. |
| RN-003 | No puede venderse un producto sin stock. | [C] | Ventas / Inventario | Declarada explícitamente. |
| RN-004 | Una Orden de Trabajo debe pertenecer a un cliente. | [C] | Taller | Declarada explícitamente. |
| RN-005 | Cada equipo tendrá un historial único. | [C] | Taller | Declarada explícitamente. |

> ⚠️ **Nota de validación (2026-07-18) sobre RN-001:** el propietario describió el proceso real de entrega como *"Cliente recoge equipo → Se realiza cobro"*, es decir, narrando el recojo antes del cobro. Esto no se interpreta como una derogación de RN-001 (que sigue vigente y confirmada), sino que probablemente describe el inicio de la visita de recojo, con el cobro ocurriendo antes de la entrega física real dentro de esa misma visita. **Se mantiene RN-001 sin cambios, pero el orden exacto de los pasos debe confirmarse puntualmente (ver BQ-033) antes de fijar la máquina de estados de la Orden de Trabajo.**

## 4. Reglas de Inventario (compartido entre líneas de negocio)

| ID | Regla | Estado | Justificación | Referencia |
|---|---|---|---|---|
| RN-006 | El inventario es único y compartido entre Tienda Comercial, Taller y Servicios de Campo; ningún movimiento puede registrarse fuera de este inventario centralizado. | [C] | Confirmado directamente por el propietario (2026-07-18) con ejemplos reales: los carbones para herramientas se venden, se usan como repuesto y se compran en volumen para reventa a otros técnicos; el cable eléctrico se vende en tienda y se usa en instalaciones de campo. No existen almacenes separados. | RF-032 |
| RN-007 | Ante una posible concurrencia entre operaciones que disputan el mismo stock (ej. una venta en tienda y un consumo de repuesto en taller sobre el mismo producto), debe existir un mecanismo de resolución (reserva, bloqueo, o descuento en el momento de confirmación). | [PV] — riesgo reducido | Sigue sin regla documentada, pero el propietario confirmó que solo existen 2 usuarios permanentes operando en un único local, lo que reduce sustancialmente la probabilidad real de concurrencia simultánea sobre un mismo producto. | BQ-001, BQ-003 |
| RN-008 | El stock de un producto no puede quedar en valor negativo, salvo autorización expresa de un rol habilitado (a confirmar si esta excepción existe). | [PV] | Complementa RN-003; el caso de excepción (venta con stock pendiente de reposición) no está documentado. | BQ-001 |

## 5. Reglas de Ventas

| ID | Regla | Estado | Justificación | Referencia |
|---|---|---|---|---|
| RN-009 | Una factura solo puede emitirse a clientes que cuenten con RUC registrado. | [I] | Regla estándar de la normativa tributaria peruana para este tipo de comprobante; se asume aplicable salvo indicación contraria. | RF-016, BQ-011 |
| RN-010 | Una venta solo puede anularse dentro de un plazo determinado y por un rol autorizado. | [PV] | No existe regla documentada sobre anulaciones; se plantea como necesaria para control interno. | BQ-013 |
| RN-011 | Los descuentos aplicados a una venta no pueden superar un porcentaje máximo sin autorización de un rol superior (ej. Supervisor o Gerente). | [PV] | Práctica común en retail/ERP; no confirmado para ISARMIN. | BQ-014 |

## 6. Reglas de Compras

| ID | Regla | Estado | Justificación | Referencia |
|---|---|---|---|---|
| RN-012 | Toda compra registrada debe estar asociada a un proveedor previamente registrado en el sistema. | [I] | Consistencia referencial estándar; se infiere de la existencia del módulo Proveedores. | RF-018, RF-033 |
| RN-013 | El costo de compra de un producto debe actualizar su costo de referencia utilizando **costo promedio ponderado** como método de valorización. | [PV] — decisión tentativa | El propietario no confirmó un método exacto; se adopta el costo promedio ponderado como propuesta técnica tentativa del analista, sujeta a confirmación final antes de fijarse en el modelo de datos. | BQ-021 |
| RN-024 | Las compras se registran de forma directa (proveedor, fecha, productos, cantidad, costo), sin un flujo formal de orden de compra con aprobación previa. | [C] | Confirmado por el propietario: "Actualmente las compras se realizan directamente a proveedores u otras tiendas"; es el único que las realiza, sin necesidad de aprobación de un tercero. | RF-033, BQ-020, BQ-022 |

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
| RN-017 | Una garantía cubre exclusivamente la falla original diagnosticada, dentro de un período determinado, y no cubre fallas nuevas o no relacionadas. | [PV] — alcance confirmado, regla pendiente | Se confirmó que Garantías **sí forma parte del MVP**, integrada a Taller/Órdenes de Trabajo (no como módulo independiente); las reglas exactas de duración y cobertura siguen sin definirse. | BQ-035, BQ-036 |
| RN-018 | Los repuestos consumidos en una reparación se descuentan del mismo inventario compartido definido en RN-006, sin excepción. | [I] | Corolario directo de RN-002/RN-006 aplicado al contexto de Taller. | RF-057 |

## 9. Reglas de Servicios de Campo

| ID | Regla | Estado | Justificación | Referencia |
|---|---|---|---|---|
| RN-019 | Un servicio de campo no puede cerrarse ni facturarse sin la conformidad registrada del cliente. | [PV] | Práctica estándar para servicios fuera de local, análoga a la entrega en Taller (RN-001); no confirmada. | BQ-039 |
| RN-020 | Los materiales consumidos en un servicio de campo se descuentan del mismo inventario compartido definido en RN-006. | [I] | Corolario de RN-006 aplicado a Servicios de Campo. | RF-067 |

## 10. Reglas de Usuarios, Roles y Auditoría

| ID | Regla | Estado | Justificación | Referencia |
|---|---|---|---|---|
| RN-021 | Un usuario desactivado no puede autenticarse en el sistema, pero su historial de acciones se conserva íntegramente para auditoría. | [I] | Consecuencia de RF-003 y del requisito de auditoría (RNF-005/RNF-010). | RF-003, RF-078 |
| RN-022 | Toda acción crítica (anulación de venta, ajuste de inventario, eliminación lógica) debe quedar asociada al usuario que la ejecutó, sin excepción. | [I] | Consecuencia directa del requisito de auditoría declarado explícitamente (RNF-005). La auditoría se confirmó como capacidad transversal obligatoria (bitácoras básicas de acciones críticas), no como módulo funcional independiente. | RF-078 |
| RN-027 | Los trabajadores temporales (ayudantes de instalaciones, personal contratado para proyectos específicos) no tienen usuario propio ni acceso al sistema. | [C] | Confirmado por el propietario: estos trabajadores no administran información. Si se requiere referenciarlos (ej. "ayudante asignado" en un Servicio de Campo), debe modelarse como un dato de referencia, no como un actor del sistema. | Actors.md |

## 11. Regla transversal de integridad de datos

| ID | Regla | Estado | Justificación | Referencia |
|---|---|---|---|---|
| RN-023 | Ninguna entidad con historial transaccional asociado (cliente, producto, proveedor, usuario) debe eliminarse físicamente de la base de datos; debe aplicarse baja lógica para preservar la trazabilidad exigida al proyecto. | [I] | Consecuencia directa del objetivo de "trazabilidad completa de cada movimiento" declarado en el contexto de negocio, y de `AI_INSTRUCTIONS.md` ("nunca elimines funcionalidades/datos sin justificación"). | RF-014, RF-020 |

## 12. Reglas pendientes de levantamiento (sin hipótesis suficiente para inferir)

Las siguientes áreas no cuentan con información suficiente ni siquiera para plantear una hipótesis razonable, y deben levantarse directamente con el cliente sin proponer una regla provisional:

- Reglas de crédito a clientes (límites, plazos, mora).
- Reglas de asignación de técnicos (carga de trabajo, especialidad).
- Reglas de priorización cuando existen múltiples OT o servicios de campo simultáneos.
- Reglas de devolución de productos vendidos.

Ver [Business-Questions.md](../02-Business/Business-Questions.md) para el detalle de las preguntas asociadas.

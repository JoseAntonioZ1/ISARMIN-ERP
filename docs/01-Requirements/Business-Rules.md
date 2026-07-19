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

## 4. Reglas de Inventario (compartido entre líneas de negocio)

| ID | Regla | Estado | Justificación | Referencia |
|---|---|---|---|---|
| RN-006 | El inventario es único y compartido entre Tienda Comercial, Taller y Servicios de Campo; ningún movimiento puede registrarse fuera de este inventario centralizado. | [I] | Declarado explícitamente que "existe un único inventario compartido"; esta regla formaliza esa afirmación como restricción de diseño. | RF-032 |
| RN-007 | Ante una posible concurrencia entre operaciones que disputan el mismo stock (ej. una venta en tienda y un consumo de repuesto en taller sobre el mismo producto), debe existir un mecanismo de resolución (reserva, bloqueo, o descuento en el momento de confirmación). | [PV] | No hay regla documentada que resuelva este conflicto; es el vacío de mayor riesgo detectado en el análisis. | BQ-001, BQ-003 |
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
| RN-013 | El costo de compra de un producto debe actualizar su costo de referencia según un método de valorización definido (PEPS, promedio ponderado u otro). | [PV] | El método de valorización no está documentado; es indispensable definirlo antes de modelar Inventario/Compras. | BQ-021 |

## 7. Reglas de Caja

| ID | Regla | Estado | Justificación | Referencia |
|---|---|---|---|---|
| RN-014 | No puede registrarse el cobro de una venta o de una OT sin una caja abierta en el turno correspondiente. | [PV] | Práctica estándar de control de caja; no confirmada para ISARMIN (¿existe siquiera el concepto de "caja" con apertura/cierre formal?). | BQ-030 |
| RN-015 | El cierre de caja requiere la conciliación entre el monto teórico (calculado por el sistema) y el monto físico declarado por el responsable. | [PV] | Idem. | BQ-031 |

## 8. Reglas de Taller

| ID | Regla | Estado | Justificación | Referencia |
|---|---|---|---|---|
| RN-016 | Una reparación no puede iniciarse sin que el cliente haya aprobado previamente la cotización correspondiente. | [I] | Se infiere directamente del proceso descrito: "Cliente aprueba → Se realiza reparación". | RF-056, BQ-034 |
| RN-017 | Una garantía cubre exclusivamente la falla original diagnosticada, dentro de un período determinado, y no cubre fallas nuevas o no relacionadas. | [PV] | El módulo "Garantías" está declarado en el alcance, pero sin reglas asociadas. | BQ-035, BQ-036 |
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
| RN-022 | Toda acción crítica (anulación de venta, ajuste de inventario, eliminación lógica) debe quedar asociada al usuario que la ejecutó, sin excepción. | [I] | Consecuencia directa del requisito de auditoría declarado explícitamente (RNF-005). | RF-078 |

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

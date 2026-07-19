# Business-Workflows.md — Flujos Operativos de Trabajo

## 1. Propósito

Mientras [Business-Processes.md](Business-Processes.md) describe el proceso de negocio a nivel macro (BPMN, de punta a punta), este documento detalla el **flujo operativo interno** de cada proceso: la secuencia exacta de pasos, el responsable de cada uno, las entradas/salidas de información y, sobre todo, **la transición de estado del sistema que produce cada paso** — el insumo directo para diseñar la máquina de estados en [Business-States.md](Business-States.md) y los eventos en [Business-Events.md](Business-Events.md).

La diferencia de nivel es intencional y sigue la práctica de **UML Activity Diagrams** (nivel operativo) anidados dentro de un proceso BPMN (nivel macro).

## 2. Convención

**[C]** Confirmado · **[I]** Inferido · **[PV]** Pendiente de Validación.

---

## WF-001 — Flujo operativo de la Orden de Trabajo (Taller)

Corresponde a BP-002. Es el flujo mejor documentado del proyecto.

| Paso | Actividad | Responsable | Entrada | Salida | Estado resultante de la OT | Evento generado |
|---|---|---|---|---|---|---|
| 1 | Registrar cliente (si es nuevo) y equipo | Recepcionista | Datos del cliente y equipo | Registro de equipo | **Recibido** | EVT-005 |
| 2 | Generar Orden de Trabajo | Recepcionista | Registro de equipo | OT creada (RN-004) | **Recibido** | EVT-006 |
| 3 | Emitir comprobante de recepción | Recepcionista | OT creada | Comprobante impreso/digital | **Recibido** | EVT-007 |
| 4 | Diagnosticar equipo | Técnico | Equipo físico | Diagnóstico registrado | **En diagnóstico → Diagnosticado** | EVT-008 |
| 5 | Generar cotización de reparación | Técnico / Supervisor [PV] | Diagnóstico | Cotización asociada a la OT | **Cotizado** | EVT-009 |
| 6 | Presentar cotización al cliente | Recepcionista | Cotización | Decisión del cliente | **Pendiente de aprobación** | — |
| 7a | Registrar aprobación | Recepcionista | Decisión positiva | Autorización de reparar | **Aprobado** | EVT-010 |
| 7b | Registrar rechazo **[PV — BQ-034]** | Recepcionista | Decisión negativa | Cierre sin reparar | **Rechazado / Cerrado** | EVT-011 |
| 8 | Reparar equipo, consumir repuestos | Técnico | Autorización | Consumo de inventario (RN-002, RN-018) | **En reparación** | EVT-012, EVT-013 |
| 9 | Realizar pruebas | Técnico | Equipo reparado | Resultado de pruebas | **En pruebas → Listo para entrega** | EVT-014 |
| 10 | Registrar pago **(RN-001, orden exacto respecto al paso 11 a confirmar — BQ-033)** | Recepcionista/Cajero | Monto de la cotización | Pago registrado | **Pagado** | EVT-015 |
| 11 | Entregar equipo | Recepcionista | Pago registrado | Equipo entregado | **Entregado / Cerrado** | EVT-016 |
| 12 (condicional) | Registrar garantía **[PV — BQ-035]** | Técnico/Supervisor | OT cerrada | Garantía asociada | (no aplica a la OT; nueva entidad) | EVT-017 |

**Puntos de decisión (gateways) documentados:**
- ¿El cliente aprueba la cotización? → Sí: paso 8. No: cierre sin reparación **[PV: ¿se cobra el diagnóstico en este caso? BQ-034]**.
- ¿El pago se registra antes o después de la entrega física? → Documentado como "no puede entregarse sin pago" (RN-001), lo cual sugiere que el pago es un prerrequisito estricto, pero no se aclara si puede ser simultáneo al acto de entrega. **BQ-033**.

---

## WF-002 — Flujo operativo de Cotización y Venta en Tienda

Corresponde a BP-001.

| Paso | Actividad | Responsable | Entrada | Salida | Estado resultante | Evento generado |
|---|---|---|---|---|---|---|
| 1 | Consultar stock | Vendedor | Solicitud del cliente | Disponibilidad confirmada | — | — |
| 2 | Generar cotización (opcional) | Vendedor | Productos seleccionados | Cotización | **Emitida** | EVT-018 |
| 3a | Cliente acepta cotización dentro del plazo **[PV — vigencia no definida, BQ-015]** | Vendedor | Cotización | Conversión a venta | **Convertida** | EVT-019 |
| 3b | Cotización vence sin uso **[PV]** | Sistema | Plazo cumplido | Cotización vencida | **Vencida** | EVT-020 |
| 4 | Registrar venta directa (sin cotización previa) | Vendedor | Productos seleccionados | Venta registrada | **Registrada** | EVT-021 |
| 5 | Validar stock disponible | Sistema | Venta registrada | Confirmación o rechazo | — | — (RN-003) |
| 6 | Determinar tipo de comprobante | Vendedor | Tipo de cliente **[PV — BQ-011]** | Selección de comprobante | — | — |
| 7 | Emitir comprobante (boleta/factura/nota de venta/ticket) | Sistema | Venta confirmada | Comprobante emitido | **Emitido** | EVT-022 |
| 8 | Registrar cobro | Vendedor/Cajero | Comprobante | Pago asociado | **Pagado** | EVT-023 |
| 9 | Actualizar inventario | Sistema | Venta pagada | Kardex actualizado | — | EVT-024 |
| 10 (condicional) | Anular venta **[PV — BQ-013]** | Rol autorizado | Venta emitida | Reversión de inventario | **Anulada** | EVT-025 |

---

## WF-003 — Flujo operativo de Servicio de Campo

Corresponde a BP-003. **Confirmado por el propietario el 2026-07-18** (solicitud → evaluación → cotización → materiales → ejecución → comprobante final), con la excepción del paso de conformidad del cliente (BQ-039, sin confirmar) y la asignación formal de técnico (BQ-037, sin urgencia hoy porque el propietario realiza estos trabajos casi en exclusiva).

| Paso | Actividad | Responsable | Entrada | Salida | Estado resultante | Evento generado |
|---|---|---|---|---|---|---|
| 1 | Registrar solicitud de servicio | Recepcionista | Solicitud del cliente | Registro creado | **Solicitado** | EVT-026 |
| 2 | Asignar técnico y fecha **[PV — BQ-037]** | Supervisor | Disponibilidad de técnicos | Asignación | **Agendado** | EVT-027 |
| 3 | Trasladarse y ejecutar diagnóstico/trabajo | Técnico de campo | Equipo/instalación del cliente | Trabajo ejecutado | **En ejecución** | EVT-028 |
| 4 | Registrar materiales consumidos | Técnico de campo | Repuestos usados | Consumo de inventario | — | EVT-029 |
| 5 | Registrar conformidad del cliente **[PV — BQ-039]** | Técnico de campo | Trabajo finalizado | Conformidad | **Conforme / Observado** | EVT-030 |
| 6 | Registrar cobro **[PV — BQ-040]** | Técnico/Cajero | Conformidad | Pago registrado | **Cerrado** | EVT-031 |

---

## WF-004 — Flujo operativo de Compra a Proveedor

Corresponde a BP-004. **Simplificado y confirmado por el propietario el 2026-07-18**: no existe flujo de aprobación de OC (pasos 3 en la tabla original no aplican); las compras son directas.

| Paso | Actividad | Responsable | Entrada | Salida | Estado resultante | Evento generado |
|---|---|---|---|---|---|---|
| 1 | Detectar necesidad de reabastecimiento | Almacenero / Sistema (alerta) | Nivel de stock mínimo (RF-031) | Necesidad identificada | — | EVT-032 |
| 2 | Generar orden de compra | Almacenero | Producto y proveedor | OC creada | **Generada** | EVT-033 |
| 3 | Aprobar orden de compra **[PV — BQ-020]** | Gerente (hipótesis) | OC creada | OC aprobada | **Aprobada** | EVT-034 |
| 4 | Enviar orden al proveedor | Almacenero | OC aprobada | Orden enviada | **Enviada** | — |
| 5 | Recibir mercadería | Almacenero | OC enviada | Mercadería física | **Recibida parcial/total** | EVT-035 |
| 6 | Actualizar inventario y costo | Sistema | Recepción confirmada | Kardex actualizado, costo actualizado | **Cerrada** | EVT-036 |

## 3. Observación general

Los flujos WF-002, WF-003 y WF-004 combinan pasos confirmados (los que reflejan un dato explícito de `PROJECT_CONTEXT.md`) con pasos inferidos por analogía al único proceso completamente documentado (WF-001, Taller). **Ningún paso marcado [PV] debe implementarse como regla rígida sin antes confirmarlo con el cliente** — el riesgo de construir un flujo que no refleje la operación real de Ventas, Campo o Compras es alto precisamente porque ISARMIN nunca describió esos procesos en detalle.

# Glossary.md — Glosario de Términos del Negocio

## 1. Propósito

Diccionario de datos conceptual y glosario de términos del dominio, tal como recomienda **IEEE 830** (sección 2.3, "Definitions, acronyms, and abbreviations") y **BABOK v3** (técnica "Glossary"). Su objetivo es evitar ambigüedad terminológica entre el negocio, el equipo de análisis y el futuro equipo de desarrollo. Todo término usado en `Functional-Requirements.md`, `Business-Rules.md` y los documentos de `02-Business/` debe estar definido aquí.

## 2. Convención

| Marca | Significado |
|---|---|
| **[C]** | Término y definición confirmados en la documentación fuente. |
| **[I]** | Término mencionado en la documentación fuente; definición ampliada por el analista a partir del contexto. |
| **[PV]** | Término que el analista considera necesario para el dominio, pero cuya existencia/uso exacto en ISARMIN debe confirmarse. |

## 3. Glosario general

| Término | Definición | Estado |
|---|---|---|
| **OT (Orden de Trabajo)** | Documento/registro que ampara el ingreso de un equipo al Taller para diagnóstico y reparación, y que sirve de hilo conductor de todo el proceso hasta la entrega. | [C] |
| **Kardex** | Historial cronológico de todos los movimientos de un ítem de inventario (ingresos, salidas, ajustes), usado para trazabilidad y valorización. | [C] |
| **Servicio de Campo** | Trabajo técnico realizado fuera de las instalaciones de ISARMIN, en el domicilio o local del cliente. | [C] |
| **Almacén** | Ubicación física donde se resguarda el inventario. Actualmente se asume un almacén único compartido por las tres líneas de negocio. | [I] |
| **Stock** | Cantidad disponible de un producto en un momento dado, resultado neto de sus movimientos de Kardex. | [I] |
| **Repuesto** | Producto del inventario usado para reparar un equipo en Taller o en un Servicio de Campo, en lugar de ser vendido directamente al público. | [I] |
| **Diagnóstico** | Evaluación técnica que determina la falla de un equipo recibido y los repuestos/trabajo necesarios para repararlo; antecede a la Cotización dentro de una OT. | [I] |
| **Cotización** | Documento comercial que informa al cliente el costo estimado de una venta o reparación, antes de que este se comprometa a pagar. Puede emitirse tanto en Ventas (tienda) como en Taller. | [C] |
| **Boleta de Venta** | Comprobante de pago emitido a consumidores finales sin necesidad de RUC, conforme a normativa SUNAT. | [C] |
| **Factura** | Comprobante de pago emitido a compradores que requieren sustentar crédito fiscal (cuentan con RUC). | [C] |
| **Nota de Venta** | Documento interno de venta sin validez tributaria como comprobante de pago (no reemplaza boleta/factura ante SUNAT). | [C] |
| **Ticket** | Comprobante simplificado de venta, usualmente impreso en formato térmico. | [C] |
| **Garantía** | Compromiso de responder, sin costo adicional o con condiciones preferenciales, ante una falla del mismo tipo en un equipo reparado o producto vendido, dentro de un período determinado. Módulo mencionado en el alcance; sin reglas definidas aún. | [PV] |
| **Comprobante de Recepción** | Documento entregado al cliente al momento de dejar su equipo en el Taller, como constancia de la entrega física (previo al diagnóstico y la cotización). | [C] |
| **Orden de Compra (OC)** | Documento mediante el cual ISARMIN formaliza un pedido de mercadería a un proveedor. | [PV] |
| **Arqueo de Caja** | Proceso de conteo y conciliación del efectivo/movimientos de una caja al cierre de un turno o jornada, para verificar que lo físico coincida con lo registrado en el sistema. | [PV] |
| **Sucursal** | Local o punto de atención físico de la empresa. No hay evidencia de que ISARMIN opere más de una sede; se incluye el término por ser relevante para el diseño del inventario. | [PV] |
| **SUNAT** | Superintendencia Nacional de Aduanas y de Administración Tributaria del Perú; entidad ante la cual se deben sustentar los comprobantes de pago electrónicos. | [C] |
| **OSE / PSE** | Operador de Servicios Electrónicos / Proveedor de Servicios Electrónicos: intermediarios autorizados por SUNAT para la emisión y validación de comprobantes electrónicos. | [PV] |
| **RUC** | Registro Único de Contribuyentes; identificador tributario peruano requerido para emitir/recibir facturas. | [C] |
| **Serie y Correlativo** | Numeración secuencial y agrupada por serie que identifica de forma única a cada comprobante emitido, conforme a normativa SUNAT. | [I] |
| **Trazabilidad** | Capacidad del sistema de reconstruir el historial completo de un objeto de negocio (producto, equipo, comprobante) a partir de sus eventos registrados. Mencionada explícitamente como objetivo del sistema. | [C] |
| **Técnico de Taller** | Actor que ejecuta diagnóstico y reparación dentro de las instalaciones de ISARMIN. | [I] |
| **Técnico de Campo** | Actor que ejecuta trabajos técnicos en las instalaciones del cliente. Pendiente de validar si es un rol distinto al Técnico de Taller. | [PV] |
| **Auditoría** | Registro histórico e inmutable de las acciones relevantes realizadas por los usuarios en el sistema (quién, qué, cuándo), mencionado como requisito no funcional (RNF-005). | [C] |
| **Rol** | Conjunto de permisos que se asigna a un usuario y que determina qué acciones y módulos puede utilizar. | [I] |
| **Permiso** | Autorización específica y atómica para ejecutar una acción sobre un módulo (ej. "crear cliente", "anular venta"). | [PV] |
| **Catálogo** | Tabla maestra de valores predefinidos y reutilizables por el sistema (ej. formas de pago, estados de OT). Ver [Business-Catalogs.md](../02-Business/Business-Catalogs.md). | [I] |
| **Backup / Respaldo** | Copia de la información del sistema realizada de forma periódica para permitir su recuperación ante pérdida de datos (RNF-004). | [C] |

## 4. Acrónimos

| Acrónimo | Significado |
|---|---|
| **ERP** | Enterprise Resource Planning (Sistema de Planificación de Recursos Empresariales) |
| **OT** | Orden de Trabajo |
| **OC** | Orden de Compra |
| **RF** | Requerimiento Funcional |
| **RNF** | Requerimiento No Funcional |
| **RN** | Regla de Negocio |
| **BQ** | Business Question (Pregunta de Negocio) |
| **BP** | Business Process (Proceso de Negocio) |
| **EVT** | Business Event (Evento de Negocio) |
| **CAT** | Catálogo de Negocio |
| **RSK** | Risk (Riesgo) |
| **SUNAT** | Superintendencia Nacional de Aduanas y de Administración Tributaria |
| **RUC** | Registro Único de Contribuyentes |
| **OSE/PSE** | Operador/Proveedor de Servicios Electrónicos |

## 5. Notas

Este glosario se considera **vivo**: debe actualizarse cada vez que surja un término nuevo durante el levantamiento de información con el cliente (ver proceso de mantenimiento en [Business-Questions.md](../02-Business/Business-Questions.md)).

# Data-Dictionary.md — Diccionario de Datos Preliminar

## 1. Propósito

Detalla, campo por campo, cada entidad definida en [Conceptual-Data-Model.md](Conceptual-Data-Model.md). Es **preliminar**: usa tipos de dato conceptuales (Texto, Número, Fecha, Booleano, Decimal, Catálogo/Enum), no tipos físicos de PostgreSQL — esa traducción corresponde a la fase de Arquitectura/`04-Database`.

## 2. Convención

**Obligatorio:** Sí / No / Condicional (depende de otro campo). **Estado:** [C] Confirmado, [I] Inferido, [PV] Pendiente de Validación — heredado del requerimiento de origen.

---

## 3. Usuarios y Roles

### Usuario
| Campo | Tipo | Obligatorio | Descripción | Estado | Origen |
|---|---|---|---|---|---|
| id | Identificador | Sí | Identificador único. | [C] | RF-001 |
| nombre | Texto | Sí | Nombre completo del usuario. | [C] | RF-001 |
| credencial_acceso | Texto (hash) | Sí | Contraseña almacenada con hash seguro, nunca texto plano. | [C] | RF-004, RNF-011 |
| estado | Catálogo (Activo/Inactivo) | Sí | Baja lógica; nunca eliminación física (RN-021). | [C] | RF-003 |
| fecha_creacion | Fecha | Sí | — | [I] | RF-001 |

### Rol
| Campo | Tipo | Obligatorio | Descripción | Estado | Origen |
|---|---|---|---|---|---|
| id | Identificador | Sí | — | [C] | RF-008 |
| nombre | Texto | Sí | Ej. "Administrador", "Ventas", "Técnico" — valores semilla, no fijos (RN-028). | [C] | RF-008 |
| descripcion | Texto | No | — | [I] | RF-008 |

### Permiso
| Campo | Tipo | Obligatorio | Descripción | Estado | Origen |
|---|---|---|---|---|---|
| id | Identificador | Sí | — | [C] | RF-010 |
| rol_id | Referencia a Rol | Sí | — | [C] | RF-010 |
| modulo | Catálogo | Sí | Módulo del sistema (Clientes, Inventario, Taller, etc.). | [C] | RF-010 |
| accion | Catálogo (Crear/Editar/Eliminar/Consultar/Anular) | Sí | — | [C] | RF-010 |

### UsuarioRol
| Campo | Tipo | Obligatorio | Descripción | Estado | Origen |
|---|---|---|---|---|---|
| usuario_id | Referencia a Usuario | Sí | — | [PV] | BQ-042 |
| rol_id | Referencia a Rol | Sí | — | [PV] | BQ-042 |

---

## 4. Clientes y Proveedores

### Cliente
| Campo | Tipo | Obligatorio | Descripción | Estado | Origen |
|---|---|---|---|---|---|
| id | Identificador | Sí | — | [C] | RF-012 |
| nombre_razon_social | Texto | Sí | — | [C] | RF-012 |
| tipo_documento | Catálogo (DNI/RUC/CE/Pasaporte) | Sí | **Valores exactos y obligatoriedad pendientes** | [PV] | BQ-011 |
| numero_documento | Texto | Sí | — | [PV] | BQ-011 |
| tipo_cliente | Catálogo (Natural/Jurídica) | No | — | [PV] | BQ-011, CAT-009 |
| telefono | Texto | Condicional | **¿Obligatorio u opcional?** | [PV] | BQ-074 |
| direccion | Texto | Condicional | **¿Obligatorio u opcional?** | [PV] | BQ-074 |
| estado | Catálogo (Activo/Inactivo) | Sí | Baja lógica confirmada; nunca eliminación física. | [C] | RF-014, RN-023 |

### Proveedor
| Campo | Tipo | Obligatorio | Descripción | Estado | Origen |
|---|---|---|---|---|---|
| id | Identificador | Sí | — | [I] | RF-018 |
| nombre_razon_social | Texto | Sí | — | [I] | RF-018 |
| documento | Texto | No | — | [PV] | RF-018 |
| telefono / direccion | Texto | No | — | [I] | RF-018 |
| estado | Catálogo (Activo/Inactivo) | Sí | Baja lógica (RN-023, extensible a proveedores). | [I] | RF-020 |

---

## 5. Inventario

### Categoria
| Campo | Tipo | Obligatorio | Descripción | Estado | Origen |
|---|---|---|---|---|---|
| id | Identificador | Sí | — | [C] | RF-025 |
| nombre | Texto | Sí | Ej. Herramientas Eléctricas, Repuestos, Accesorios (CAT-002). | [C] | RF-025 |
| categoria_padre_id | Referencia a Categoria | No | **¿Se requiere jerarquía de subcategorías?** | [PV] | BQ-007 |

### Producto
| Campo | Tipo | Obligatorio | Descripción | Estado | Origen |
|---|---|---|---|---|---|
| id | Identificador | Sí | — | [C] | RF-023 |
| codigo | Texto | Sí | — | [C] | RF-023 |
| nombre | Texto | Sí | — | [C] | RF-023 |
| categoria_id | Referencia a Categoria | Sí | — | [C] | RF-023, RF-025 |
| marca | Texto | No | — | [C] | RF-023 |
| unidad_medida | Catálogo | Sí | **Valores exactos (unidad, metro, kg...) pendientes.** | [PV] | BQ-008, CAT-014 |
| costo_referencia | Decimal | Sí | Actualizado por costo promedio ponderado (tentativo). | [PV] | RN-013 |
| precio_venta | Decimal | Sí | — | [C] | RF-023 |
| margen | Decimal (calculado) | No | Derivado de costo y precio. | [C] | RF-023 |
| stock_actual | Número (calculado del Kardex) | Sí | Nunca negativo salvo ajuste autorizado (RN-008). | [C] | RF-026, RN-008 |
| stock_minimo | Número | No | **¿Se requiere? Aún sin confirmar.** | [PV] | BQ-005 |

### MovimientoInventario (Kardex)
| Campo | Tipo | Obligatorio | Descripción | Estado | Origen |
|---|---|---|---|---|---|
| id | Identificador | Sí | — | [C] | RN-002 |
| producto_id | Referencia a Producto | Sí | — | [C] | RF-027 |
| tipo_movimiento | Catálogo (Compra/Venta/ConsumoTaller/ConsumoCampo/Ajuste/Devolución) | Sí | CAT-013, confirmado. | [C] | RF-029 |
| cantidad | Número | Sí | Positivo (entrada) o negativo (salida). | [C] | RN-002 |
| fecha | Fecha/Hora | Sí | — | [C] | RN-002 |
| usuario_id | Referencia a Usuario | Sí | Trazabilidad (RN-022). | [C] | RN-022 |
| origen_tipo | Catálogo (Venta/Compra/OrdenTrabajo/ServicioCampo/Ajuste manual) | Sí | Ver decisión de modelado #2 en el modelo conceptual. | [I] | — |
| origen_id | Identificador | Condicional | Nulo solo si origen_tipo = Ajuste manual sin origen transaccional. | [I] | — |
| motivo_ajuste | Texto | Condicional | Obligatorio solo si tipo_movimiento = Ajuste (RN-008). | [C] | RF-030 |

---

## 6. Compras

### Compra
| Campo | Tipo | Obligatorio | Descripción | Estado | Origen |
|---|---|---|---|---|---|
| id | Identificador | Sí | — | [C] | RF-033 |
| proveedor_id | Referencia a Proveedor | Sí | — | [C] | RN-012 |
| fecha | Fecha | Sí | — | [C] | RF-033 |
| documento_compra_tipo | Texto | Sí | Boleta/Factura del proveedor u otro comprobante. | [C] | RF-034 |
| documento_compra_numero | Texto | Sí | — | [C] | RF-034 |
| usuario_id | Referencia a Usuario | Sí | — | [C] | RN-022 |
| total | Decimal (calculado) | Sí | Suma de CompraDetalle. | [C] | RF-033 |

### CompraDetalle
| Campo | Tipo | Obligatorio | Descripción | Estado | Origen |
|---|---|---|---|---|---|
| id | Identificador | Sí | — | [C] | RF-033 |
| compra_id | Referencia a Compra | Sí | — | [C] | RF-033 |
| producto_id | Referencia a Producto | Sí | — | [C] | RF-033 |
| cantidad | Número | Sí | — | [C] | RF-033 |
| costo_unitario | Decimal | Sí | Actualiza costo_referencia del Producto. | [C] | RF-035 |

---

## 7. Ventas

### Venta
| Campo | Tipo | Obligatorio | Descripción | Estado | Origen |
|---|---|---|---|---|---|
| id | Identificador | Sí | — | [C] | RF-038 |
| cliente_id | Referencia a Cliente | No | Puede ser cliente genérico en venta simple. | [I] | RF-038 |
| tipo_comprobante | Catálogo (Cotización/Boleta/Factura/Nota de Venta/Ticket) | Sí | CAT-004, confirmado. | [C] | RF-039 |
| serie / correlativo | Texto / Número | Condicional | Obligatorio si tipo_comprobante requiere numeración SUNAT. | [PV] | BQ-050 |
| fecha | Fecha/Hora | Sí | — | [C] | RF-038 |
| usuario_id | Referencia a Usuario (rol Ventas) | Sí | — | [C] | RN-022 |
| total | Decimal (calculado) | Sí | Suma de VentaDetalle. | [C] | RF-038 |
| estado | Catálogo (Registrada/Emitida/Pagada/Anulada) | Sí | Ver ST-007 en `Business-States.md`. | [PV] | BQ-013 |
| origen | Catálogo (Directa/OrdenTrabajo/ServicioCampo) | Sí | Soporta RF-083 (venta ligada a reparación/servicio). | [C] | RF-083 |

### VentaDetalle
| Campo | Tipo | Obligatorio | Descripción | Estado | Origen |
|---|---|---|---|---|---|
| id | Identificador | Sí | — | [I] | RF-038 |
| venta_id | Referencia a Venta | Sí | — | [I] | RF-038 |
| producto_id | Referencia a Producto | Sí | — | [I] | RF-038 |
| cantidad | Número | Sí | — | [I] | RF-038 |
| precio_unitario | Decimal | Sí | — | [I] | RF-038 |

### PagoVenta
| Campo | Tipo | Obligatorio | Descripción | Estado | Origen |
|---|---|---|---|---|---|
| id | Identificador | Sí | — | [PV] | BQ-012 |
| venta_id | Referencia a Venta | Sí | — | [PV] | BQ-012 |
| medio_pago | Catálogo (Efectivo/Yape/Plin/Transferencia — CAT-008) | Sí | Confirmado y configurable. | [C] | RF-042 |
| monto | Decimal | Sí | Permite dividir el pago entre medios (combinación exacta: sin confirmar). | [PV] | BQ-012 |

### SaldoPendiente (transversal — Venta, OrdenTrabajo o ServicioCampo)
| Campo | Tipo | Obligatorio | Descripción | Estado | Origen |
|---|---|---|---|---|---|
| id | Identificador | Sí | — | [C] | RN-001, RN-031 |
| origen_tipo | Catálogo (Venta/OrdenTrabajo/ServicioCampo) | Sí | — | [C] | RN-031 |
| origen_id | Identificador | Sí | — | [C] | RN-031 |
| monto_pendiente | Decimal | Sí | — | [C] | BQ-093 |
| usuario_autorizo_id | Referencia a Usuario (rol Administrador) | Sí | Solo el Administrador/Propietario autoriza. | [C] | BQ-093 |
| fecha_referencia_pago | Fecha | No | Fecha o referencia de pago pendiente, cuando corresponda. | [C] | BQ-093 |
| estado | Catálogo (Pendiente/Cobrado) | Sí | Se cierra vía UC-21 (Cobrar Saldo Pendiente). | [C] | RF-088 |

---

## 8. Caja

### Caja
| Campo | Tipo | Obligatorio | Descripción | Estado | Origen |
|---|---|---|---|---|---|
| id | Identificador | Sí | Única para todo el negocio (RN-025). | [C] | RF-046 |
| fecha_apertura | Fecha/Hora | Sí | — | [PV] | BQ-030 |
| monto_apertura | Decimal | Sí | **Monto inicial exacto: sin confirmar.** | [PV] | BQ-030 |
| fecha_cierre | Fecha/Hora | Condicional | — | [PV] | BQ-030 |
| monto_teorico_cierre | Decimal (calculado) | Condicional | Calculado por el sistema (RN-015). | [PV] | BQ-031 |
| monto_fisico_declarado | Decimal | Condicional | Declarado por el responsable al cerrar. | [PV] | BQ-031 |
| usuario_id | Referencia a Usuario | Sí | — | [C] | RN-022 |
| estado | Catálogo (Abierta/Cerrada) | Sí | — | [PV] | BQ-030 |

### MovimientoCaja
| Campo | Tipo | Obligatorio | Descripción | Estado | Origen |
|---|---|---|---|---|---|
| id | Identificador | Sí | — | [I] | RF-047 |
| caja_id | Referencia a Caja | Sí | — | [I] | RF-047 |
| tipo | Catálogo (Ingreso/Egreso) | Sí | — | [I] | RF-047 |
| monto | Decimal | Sí | — | [I] | RF-047 |
| origen_tipo | Catálogo (Venta/OrdenTrabajo/ServicioCampo/Compra/Gasto) | Sí | — | [I] | RF-047 |
| origen_id | Identificador | Condicional | Nulo si es un gasto operativo sin origen transaccional. | [I] | RF-047 |
| usuario_id | Referencia a Usuario | Sí | — | [C] | RN-022 |
| fecha | Fecha/Hora | Sí | — | [I] | RF-047 |

---

## 9. Taller

### OrdenTrabajo (OT)
| Campo | Tipo | Obligatorio | Descripción | Estado | Origen |
|---|---|---|---|---|---|
| id | Identificador | Sí | — | [C] | RF-052 |
| cliente_id | Referencia a Cliente | Sí | Obligatorio (RN-004). | [C] | RN-004 |
| equipo_descripcion | Texto | Sí | — | [I] | RF-051 |
| falla_reportada | Texto | Sí | **Campos exactos del recibo original: sin confirmar.** | [PV] | BQ-091 |
| fecha_recepcion | Fecha/Hora | Sí | — | [C] | RF-051 |
| usuario_recepcion_id | Referencia a Usuario (Administrador, Ventas o Técnico) | Sí | Configurable (RN-029). | [C] | RN-029 |
| estado | Catálogo (Recibido/Diagnosticado/Cotizado/Aprobado/Rechazado/EnReparación/EnPruebas/ListoParaEntrega/Entregado) | Sí | Ver ST-001. | [C] | RF-063 |
| fecha_entrega | Fecha/Hora | Condicional | — | [C] | RF-059 |
| usuario_entrega_id | Referencia a Usuario | Condicional | — | [C] | RF-059 |
| estado_pago | Catálogo (Completo antes/Completo al momento/Adelanto/Saldo pendiente) | Condicional | Obligatorio al entregar (RN-001). | [C] | RN-001 |

### Diagnostico
| Campo | Tipo | Obligatorio | Descripción | Estado | Origen |
|---|---|---|---|---|---|
| id | Identificador | Sí | — | [I] | RF-054 |
| orden_trabajo_id | Referencia a OrdenTrabajo | Sí | Relación 1:1. | [I] | RF-054 |
| descripcion | Texto | Sí | — | [I] | RF-054 |
| fecha | Fecha | Sí | — | [I] | RF-054 |
| usuario_id | Referencia a Usuario (Técnico) | Sí | — | [I] | RF-054 |

### CotizacionReparacion
| Campo | Tipo | Obligatorio | Descripción | Estado | Origen |
|---|---|---|---|---|---|
| id | Identificador | Sí | — | [I] | RF-055 |
| orden_trabajo_id | Referencia a OrdenTrabajo | Sí | — | [I] | RF-055 |
| monto_estimado | Decimal | Sí | — | [I] | RF-055 |
| fecha | Fecha | Sí | — | [I] | RF-055 |
| decision_cliente | Catálogo (Aprobada/Rechazada) | Condicional | — | [I] | RF-056 |
| cobro_diagnostico_rechazo | Decimal | No | Solo si se rechaza y se decide cobrar (RN-030). | [C] | RN-030 |
| evidencia_aprobacion | Texto | No | **Forma exacta (firma física/digital/verbal): sin confirmar.** | [PV] | BQ-034 (parcial) |

### ConsumoRepuesto
| Campo | Tipo | Obligatorio | Descripción | Estado | Origen |
|---|---|---|---|---|---|
| id | Identificador | Sí | — | [C] | RF-057 |
| orden_trabajo_id | Referencia a OrdenTrabajo | Sí | — | [C] | RF-057 |
| producto_id | Referencia a Producto | Sí | — | [C] | RF-057 |
| cantidad | Número | Sí | Genera MovimientoInventario tipo ConsumoTaller. | [C] | RN-018 |

### Garantia
| Campo | Tipo | Obligatorio | Descripción | Estado | Origen |
|---|---|---|---|---|---|
| id | Identificador | Sí | — | [C] | RF-061 |
| orden_trabajo_id | Referencia a OrdenTrabajo | Sí | OT que generó la garantía. | [C] | RF-061 |
| fecha_inicio | Fecha | Sí | — | [C] | RF-061 |
| fecha_fin | Fecha | Sí | — | [C] | RF-061 |
| orden_trabajo_reingreso_id | Referencia a OrdenTrabajo | No | Si el equipo reingresa por la misma falla (RF-062). | [C] | RF-062 |

---

## 10. Servicios de Campo

### ServicioCampo
| Campo | Tipo | Obligatorio | Descripción | Estado | Origen |
|---|---|---|---|---|---|
| id | Identificador | Sí | — | [I] | RF-064 |
| cliente_id | Referencia a Cliente | Sí | — | [I] | RF-064 |
| descripcion_trabajo | Texto | Sí | — | [I] | RF-064 |
| fecha_solicitud | Fecha | Sí | — | [I] | RF-064 |
| tecnico_asignado_id | Referencia a Usuario (Técnico) | Sí | Asignación formal de baja urgencia hoy (BQ-037). | [PV] | BQ-037 |
| fecha_ejecucion | Fecha | Condicional | — | [I] | RF-066 |
| estado_final | Texto/Catálogo | Sí | Confirmado como campo simple (no estados separados). | [C] | RN-019 |
| observaciones | Texto | No | — | [C] | RN-019 |
| usuario_cierre_id | Referencia a Usuario | Sí | — | [C] | RN-019 |
| estado | Catálogo (Solicitado/Agendado/EnEjecución/Cerrado) | Sí | Ver ST-005. | [C] | RF-064 |

### ServicioCampoDetalle
| Campo | Tipo | Obligatorio | Descripción | Estado | Origen |
|---|---|---|---|---|---|
| id | Identificador | Sí | — | [C] | RF-067 |
| servicio_campo_id | Referencia a ServicioCampo | Sí | — | [C] | RF-067 |
| producto_id | Referencia a Producto | Sí | — | [C] | RF-067 |
| cantidad | Número | Sí | Genera MovimientoInventario tipo ConsumoCampo. | [C] | RN-020 |

### ParticipacionTemporal
| Campo | Tipo | Obligatorio | Descripción | Estado | Origen |
|---|---|---|---|---|---|
| id | Identificador | Sí | — | [C] | RF-090 |
| orden_trabajo_id | Referencia a OrdenTrabajo | Condicional | Uno de los dos (OT o Servicio) debe estar presente. | [C] | RF-090 |
| servicio_campo_id | Referencia a ServicioCampo | Condicional | — | [C] | RF-090 |
| nombre_trabajador | Texto | Sí | Solo dato de referencia, sin cuenta de usuario (RN-027). | [C] | RN-027 |
| rol_apoyo | Texto | No | Ej. "Ayudante de instalación". | [I] | RN-027 |

---

## 11. Transversales

### DocumentoAdjunto
| Campo | Tipo | Obligatorio | Descripción | Estado | Origen |
|---|---|---|---|---|---|
| id | Identificador | Sí | — | [C] | RF-084 |
| entidad_tipo | Catálogo (Venta/Compra/OrdenTrabajo/ServicioCampo) | Sí | — | [C] | RF-084 a RF-087 |
| entidad_id | Identificador | Sí | — | [C] | RF-084 a RF-087 |
| tipo_documento | Catálogo (Foto/DocumentoCompra/Cotización/Comprobante/InformeTécnico) | Sí | — | [C] | RF-084 a RF-087 |
| archivo | Archivo/URL | Sí | Preparado para respaldo (RNF-007, RNF-025). | [C] | RF-087 |
| fecha | Fecha | Sí | — | [C] | RF-084 |
| usuario_id | Referencia a Usuario | Sí | — | [C] | RN-022 |
| tamano_maximo / retencion | — | — | **Límites de tamaño/cantidad y tiempo de conservación: sin confirmar.** | [PV] | BQ-092 |

### Auditoria
| Campo | Tipo | Obligatorio | Descripción | Estado | Origen |
|---|---|---|---|---|---|
| id | Identificador | Sí | — | [C] | RF-078 |
| usuario_id | Referencia a Usuario | Sí | — | [C] | RF-079 |
| fecha | Fecha | Sí | — | [C] | RF-079 |
| hora | Hora | Sí | — | [C] | RF-079 |
| accion | Catálogo (Creación/Modificación/Eliminación lógica/Anulación/Movimiento de inventario/Movimiento de caja/Cambio en OT) | Sí | Lista cerrada y confirmada (RF-078). | [C] | RF-078 |
| entidad_tipo | Texto | Sí | — | [C] | RF-079 |
| entidad_id | Identificador | Sí | — | [C] | RF-079 |
| detalle | Texto | No | **Nivel de detalle exacto (solo qué acción, o también valores antes/después): sin confirmar.** | [PV] | BQ-049 (mayormente resuelta; detalle fino de "detalle" abierto) |

## 12. Resumen de campos pendientes de validación (bloquean el modelo físico, no el conceptual)

| Campo/entidad | Pregunta pendiente |
|---|---|
| Cliente.tipo_documento, tipo_cliente | BQ-011 |
| Cliente.telefono, direccion (obligatoriedad) | BQ-074 |
| Categoria.categoria_padre_id | BQ-007 |
| Producto.unidad_medida (valores) | BQ-008 |
| Producto.stock_minimo | BQ-005 |
| Venta.serie/correlativo | BQ-050 |
| Venta.estado (reglas de anulación) | BQ-013 |
| PagoVenta (combinación de medios) | BQ-012 (parcial) |
| Caja.monto_apertura, mecánica de cierre | BQ-030, BQ-031 |
| OrdenTrabajo.falla_reportada (campos exactos) | BQ-091 |
| CotizacionReparacion.evidencia_aprobacion | BQ-034 (parcial) |
| ServicioCampo.tecnico_asignado_id (criterio de asignación) | BQ-037 |
| DocumentoAdjunto (límites y retención) | BQ-092 |
| Auditoria.detalle (nivel exacto) | BQ-049 (parcial) |

Ninguno de estos pendientes bloquea el Modelo Conceptual (las entidades y relaciones ya están confirmadas); todos deben resolverse antes de fijar el Modelo Entidad-Relación físico en `04-Database/`.

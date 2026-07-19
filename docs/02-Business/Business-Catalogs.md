# Business-Catalogs.md — Catálogos de Negocio

## 1. Propósito

Identifica las **tablas maestras / catálogos** (listas de valores controlados y reutilizables) que el sistema deberá administrar. En términos de UML, cada catálogo aquí listado es candidato a convertirse en una clase de tipo *enumeración* o *tabla de referencia* en el modelo de dominio.

**Principio aplicado (explícito por instrucción del usuario):** no se inventan valores de catálogo que no puedan deducirse razonablemente del negocio. Cuando la documentación fuente no permite deducir los valores, el catálogo se declara **[PV] sin valores confirmados**, y se propone únicamente como referencia de la industria, dejando explícito que debe validarse o descartarse.

## 2. Convención

**[C]** Los valores están explícitamente confirmados en la documentación fuente · **[I]** El catálogo existe con certeza (se infiere del negocio) pero algunos valores son deducidos, no confirmados literalmente · **[PV]** Ni el catálogo ni sus valores están confirmados; se trata de una propuesta de referencia sujeta a validación total.

---

## CAT-001 — Tipos de Documento de Venta

- **Estado:** [C]
- **Valores confirmados:** Cotización, Boleta, Factura, Nota de Venta, Ticket.
- **Uso:** Ventas (RF-039).

## CAT-002 — Categorías de Producto

- **Estado:** [C] (valores) / [PV] (estructura — ¿jerarquía categoría/subcategoría?)
- **Valores confirmados (ejemplos declarados en el contexto de negocio, no necesariamente exhaustivos):** Herramientas eléctricas, Herramientas manuales, Materiales eléctricos, Materiales sanitarios, Repuestos, Accesorios, Productos tecnológicos, Parlantes, Audífonos, Relojes, Focos, Interruptores, Tubos, Cables, Llaves, Codos, Carbones, Cuchillas, Rodamientos, Consumibles, Otros.
- **Pendiente de validar:** ¿Esta lista es exhaustiva o solo ejemplifica? ¿Se requiere una jerarquía de categorías y subcategorías? → **BQ-007**
- **Uso:** Inventario (RF-025).

## CAT-003 — Tipos de Equipo para Reparación (Taller)

- **Estado:** [C]
- **Valores confirmados:** Amoladoras, Taladros, Bombas de agua, Soldadoras, Hidrolavadoras, Lavadoras, Refrigeradoras, Licuadoras, Herramientas eléctricas, Equipos industriales, Equipos electromecánicos, Otros.
- **Uso:** Taller (RF-051).

## CAT-004 — Tipos de Servicio de Campo

- **Estado:** [C]
- **Valores confirmados:** Instalaciones eléctricas, Instalaciones industriales, Mantenimiento preventivo, Mantenimiento correctivo, Cambio de bombas, Cambio de motores, Instalación de tableros, Instalaciones domiciliarias, Instalaciones comerciales, Mantenimiento de plantas de agua, Otros proyectos técnicos.
- **Uso:** Servicios de Campo (RF-064).

## CAT-005 — Roles de Usuario

- **Estado:** [C] — actualizado tras validación directa con el propietario (2026-07-18).
- **Valores confirmados (operación real):** Administrador/Propietario, Ventas, Técnico.
- **Valores descartados (especulados originalmente, no existen en la operación real):** Gerente, Recepcionista, Almacenero, Supervisor, Contador — ver `Actors.md` sección 5 para el detalle de por qué se descartó cada uno.
- **Cliente** no es un rol de usuario del sistema (sin acceso, ver ACT-009).
- **Resuelto:** "Técnico" es un único rol para Taller y Campo, no se subdivide. Resuelve **BQ-041**.
- **Uso:** Usuarios y Roles (RF-008).

## CAT-006 — Estados de Orden de Trabajo

- **Estado:** [PV] — sin valores confirmados por el cliente; propuesta preliminar derivada del proceso descrito en `PROJECT_CONTEXT.md`.
- **Valores propuestos (a validar):** Recibido, Diagnosticado, Cotizado, Pendiente de Aprobación, Aprobado, Rechazado, En Reparación, En Pruebas, Listo para Entrega, Pagado, Entregado, Cerrado.
- **Ver:** [Business-States.md](Business-States.md) para el diagrama de transición completo.
- **Referencia:** BQ-032.

## CAT-007 — Estados de Cotización

- **Estado:** [PV]
- **Valores propuestos (a validar):** Emitida, Aprobada, Rechazada, Vencida, Convertida en Venta/OT.
- **Referencia:** BQ-015, BQ-034.

## CAT-008 — Formas / Medios de Pago

- **Estado:** [PV] — no hay mención alguna en la documentación fuente sobre qué medios de pago acepta la empresa.
- **Valores propuestos como referencia de mercado peruano (NO confirmados):** Efectivo, Tarjeta de Crédito, Tarjeta de Débito, Transferencia Bancaria, Billetera Digital (Yape/Plin), Depósito en Cuenta.
- **Referencia:** BQ-012.

## CAT-009 — Tipos de Cliente

- **Estado:** [PV]
- **Valores propuestos (a validar):** Persona Natural, Persona Jurídica.
- **Nota:** No se confirma si además existe una segmentación comercial (ej. mayorista/minorista, cliente frecuente) que afecte precios o crédito.
- **Referencia:** BQ-011.

## CAT-010 — Estados de Servicio de Campo

- **Estado:** [PV]
- **Valores propuestos (a validar):** Solicitado, Agendado, En Ejecución, Conforme, Observado, Cerrado.
- **Referencia:** BQ-037, BQ-039.

## CAT-011 — Estados de Orden de Compra

- **Estado:** [PV] — simplificación sugerida tras validación.
- **Valores propuestos originalmente:** Generada, Aprobada, Enviada, Recibida Parcial, Recibida Total, Cerrada, Anulada.
- **Nota de simplificación (2026-07-18):** el propietario confirmó que las compras se realizan **directamente** a proveedores, sin un flujo formal de orden de compra con aprobación previa (RN-024). Esto sugiere que, para el MVP, este catálogo de estados puede reducirse a algo como *Registrada → Recibida* (un simple registro de compra), sin los estados "Aprobada"/"Enviada" propuestos originalmente. Se mantiene la propuesta completa documentada por si el negocio crece y formaliza el proceso de compras a futuro.
- **Referencia:** BQ-020, BQ-022.

## CAT-012 — Estados de Comprobante de Venta

- **Estado:** [PV]
- **Valores propuestos (a validar):** Emitido, Pagado, Anulado.
- **Referencia:** BQ-013.

## CAT-013 — Motivos de Movimiento de Inventario (Kardex)

- **Estado:** [C] — confirmado por el propietario (2026-07-18): "Tipos de movimientos esperados: Compras, Ventas, Consumo en taller, Consumo en servicios de campo, Ajustes de inventario" — coincide exactamente con la propuesta inferida por el analista.
- **Valores confirmados:** Compra, Venta, Consumo en Taller, Consumo en Servicio de Campo, Ajuste de Inventario. "Devolución" se mantiene como valor propuesto adicional, pendiente de **BQ-087** (¿existen devoluciones?).
- **Referencia:** RF-029.

## CAT-014 — Unidades de Medida

- **Estado:** [PV] — no se documenta si el sistema requiere manejar más de una unidad de medida por producto.
- **Valores propuestos (a validar):** Unidad, Metro, Kilogramo, Litro, Rollo, Par, Juego.
- **Referencia:** BQ-008.

## CAT-015 — Prioridades (OT / Servicio de Campo)

- **Estado:** [PV] — no hay evidencia de que el negocio priorice órdenes; se propone como práctica común de taller.
- **Valores propuestos (a validar):** Alta, Media, Baja.
- **Referencia:** BQ-033 (relacionada, sobre gestión de tiempos en Taller).

## CAT-016 — Almacenes / Sucursales

- **Estado:** [C] — confirmado por el propietario (2026-07-18): un único local propio dividido en dos áreas físicas (Tienda/Recepción y Taller/Almacén), no dos almacenes ni dos sedes.
- **Valor confirmado:** Almacén Único. Resuelve **BQ-002**.
- **Nota:** aunque no se requiere modelar multi-almacén en el MVP, el diseño debería dejar el punto de extensión abierto, dado que `PROJECT_CONTEXT.md` prevé "nuevas sucursales" como posible ampliación futura (reafirmado por el propietario en la sección de Stack Tecnológico: "la solución debe estar preparada para... nuevas sucursales").

## CAT-017 — Tipos de Garantía

- **Estado:** [PV] — alcance confirmado, valores aún sin definir.
- **Confirmado (2026-07-18):** Garantías **sí forma parte del MVP**, integrada a Taller/Órdenes de Trabajo (no como módulo independiente complejo).
- **Valores propuestos (a validar):** Garantía de Repuesto, Garantía de Mano de Obra.
- **Referencia:** BQ-035, BQ-036.

## CAT-018 — Motivos de Rechazo / Anulación

- **Estado:** [PV] — no existe catálogo alguno documentado.
- **Ejemplos de motivo a validar:** Rechazo de cotización de reparación, Anulación de venta por error de registro, Anulación por solicitud del cliente.
- **Referencia:** BQ-013, BQ-034.

## CAT-019 — Monedas

- **Estado:** [PV] — se asume, sin confirmación, que la única moneda operativa es el Sol Peruano (PEN), dado que la empresa opera en Perú y factura ante SUNAT.
- **Referencia:** BQ-009.

## CAT-020 — Categorías de Documento de Identidad (Cliente/Proveedor)

- **Estado:** [PV] — necesario para el módulo Clientes/Proveedores dado el contexto peruano, pero no confirmado explícitamente.
- **Valores propuestos (a validar):** DNI, RUC, Carné de Extranjería, Pasaporte.
- **Referencia:** BQ-011.

## 3. Resumen de trazabilidad

| Catálogo | Estado | Módulo principal |
|---|---|---|
| CAT-001 Tipos de Documento de Venta | [C] | Ventas |
| CAT-002 Categorías de Producto | [C]/[PV] | Inventario |
| CAT-003 Tipos de Equipo | [C] | Taller |
| CAT-004 Tipos de Servicio de Campo | [C] | Servicios de Campo |
| CAT-005 Roles de Usuario | [C] | Usuarios |
| CAT-006 Estados de OT | [PV] | Taller |
| CAT-007 Estados de Cotización | [PV] | Ventas/Taller |
| CAT-008 Formas de Pago | [PV] | Ventas/Caja |
| CAT-009 Tipos de Cliente | [PV] | Clientes |
| CAT-010 Estados de Servicio de Campo | [PV] | Servicios de Campo |
| CAT-011 Estados de Orden de Compra | [PV] | Compras |
| CAT-012 Estados de Comprobante | [PV] | Ventas |
| CAT-013 Motivos de Movimiento de Inventario | [C] | Inventario |
| CAT-014 Unidades de Medida | [PV] | Inventario |
| CAT-015 Prioridades | [PV] | Taller/Campo |
| CAT-016 Almacenes/Sucursales | [C] | Inventario |
| CAT-017 Tipos de Garantía | [PV] | Taller |
| CAT-018 Motivos de Rechazo/Anulación | [PV] | Ventas/Taller |
| CAT-019 Monedas | [PV] | Ventas/Caja |
| CAT-020 Documentos de Identidad | [PV] | Clientes/Proveedores |

**Conclusión (actualizada 2026-07-18):** de los 20 catálogos identificados, **7 tienen valores totalmente confirmados** por el propietario (CAT-001, CAT-003, CAT-004, CAT-005, CAT-013, CAT-016, y CAT-002 con matiz de estructura pendiente). Los 13 restantes siguen marcados [PV] y ningún catálogo en ese estado debe cargarse como dato "semilla" del sistema sin antes validarlo — hacerlo equivaldría a inventar reglas de negocio.

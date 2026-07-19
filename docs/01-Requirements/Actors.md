# Actors.md — Actores del Sistema

## 1. Propósito

Este documento identifica y describe a todos los actores (humanos y sistemas externos) que interactúan con ISARMIN ERP, siguiendo el enfoque de modelado de actores de **UML (Use Case Diagrams)** y el análisis de stakeholders propuesto por **BABOK v3** (sección "Stakeholder Analysis").

Un actor es cualquier persona, rol, sistema o dispositivo externo que **envía o recibe información del sistema**, sin que ello implique necesariamente una cuenta de usuario individual (un actor es un rol, no una persona física — una misma persona puede ejercer más de un actor, y un actor puede ser ejercido por más de una persona).

Este documento es la base para:
- El diseño de la matriz de roles y permisos (módulo Usuarios/Roles).
- La identificación de casos de uso (un caso de uso siempre involucra al menos un actor primario).
- El diseño de las interfaces de integración con sistemas externos.

## 2. Convención utilizada en este documento

| Marca | Significado |
|---|---|
| **[C] Confirmado** | Declarado explícitamente en la documentación fuente del proyecto (`PROJECT_CONTEXT.md`, versión previa de `Actors.md`). |
| **[I] Inferido** | Deducido razonablemente del contexto de negocio ya declarado; el rol existe, pero su alcance exacto requiere validación. |
| **[PV] Pendiente de Validación** | Hipótesis de trabajo del analista, sin base documental directa. Debe confirmarse, ajustarse o descartarse con el cliente. |

Toda fila marcada `[I]` o `[PV]` referencia el identificador de pregunta correspondiente en [Business-Questions.md](../02-Business/Business-Questions.md).

## 3. Clasificación de actores

Siguiendo UML, se distingue entre:
- **Actor primario**: inicia una interacción para lograr un objetivo de negocio (ej. el Vendedor inicia una venta).
- **Actor secundario**: es requerido por el sistema para completar un caso de uso, pero no lo inicia (ej. el sistema de SUNAT valida un comprobante).
- **Actor humano interno**: personal de ISARMIN PERÚ S.A.C.
- **Actor humano externo**: no pertenece a la empresa (clientes, proveedores).
- **Actor sistema**: software o hardware externo con el que el ERP se integra.

## 4. Actores humanos internos

### ACT-001 — Administrador del Sistema
- **Tipo:** Primario / Interno
- **Estado:** [C]
- **Descripción:** Responsable de la configuración general del sistema, gestión de usuarios, roles y permisos, y parametrización de catálogos.
- **Objetivos frente al sistema:** Crear/editar/desactivar usuarios; asignar roles; configurar parámetros generales (series de comprobantes, catálogos, datos de la empresa).
- **Módulos con los que interactúa (inferido):** Usuarios, Roles y Permisos, Configuración, Auditoría.
- **Pendiente de validar:** ¿Es un rol técnico (TI) o administrativo (Gerencia)? ¿Existe más de un Administrador? → **BQ-057**

### ACT-002 — Gerente
- **Tipo:** Primario / Interno
- **Estado:** [C]
- **Descripción:** Responsable de la toma de decisiones estratégicas y supervisión general del negocio.
- **Objetivos frente al sistema (inferido):** Consultar reportes gerenciales (ventas, rentabilidad, inventario, indicadores de taller); posiblemente aprobar operaciones de alto impacto (descuentos especiales, compras grandes).
- **Módulos con los que interactúa:** Reportes, Auditoría, y consulta transversal a todos los módulos.
- **Pendiente de validar:** ¿Tiene funciones de aprobación (ej. aprobar descuentos, compras) o es únicamente consultivo? → **BQ-058**

### ACT-003 — Supervisor
- **Tipo:** Primario / Interno
- **Estado:** [C]
- **Descripción:** Rol de control intermedio, presumiblemente sobre las operaciones de Taller y/o Servicios de Campo.
- **Objetivos frente al sistema (inferido):** Supervisar avance de Órdenes de Trabajo y Servicios de Campo; reasignar técnicos; validar diagnósticos o cotizaciones antes de enviarlas al cliente.
- **Módulos con los que interactúa:** Taller, Servicios de Campo, Reportes operativos.
- **Pendiente de validar:** ¿Supervisa Taller, Campo, o ambos? ¿Su rol se solapa con el de Gerente? → **BQ-059**

### ACT-004 — Recepcionista
- **Tipo:** Primario / Interno
- **Estado:** [C]
- **Descripción:** Punto de contacto inicial con el cliente en el Taller; registra la recepción de equipos.
- **Objetivos frente al sistema (inferido):** Registrar cliente (si es nuevo); registrar equipo recibido; generar comprobante de recepción; consultar estado de una OT para informar al cliente.
- **Módulos con los que interactúa:** Clientes, Taller (recepción de equipos, consulta de estado de OT).
- **Pendiente de validar:** ¿También atiende recepción de Servicios de Campo (agendamiento)? → **BQ-060**

### ACT-005 — Vendedor
- **Tipo:** Primario / Interno
- **Estado:** [C]
- **Descripción:** Encargado de la atención y venta en la Tienda Comercial.
- **Objetivos frente al sistema (inferido):** Consultar stock; generar cotizaciones de venta; emitir comprobantes (boleta, factura, nota de venta, ticket); registrar cobro o derivar a Caja.
- **Módulos con los que interactúa:** Ventas, Inventario (consulta), Clientes, Caja (si emite y cobra en el mismo puesto).
- **Pendiente de validar:** ¿El vendedor cobra directamente o siempre deriva a un cajero independiente? → **BQ-061**

### ACT-006 — Almacenero
- **Tipo:** Primario / Interno
- **Estado:** [C]
- **Descripción:** Responsable de la custodia física y control del inventario compartido.
- **Objetivos frente al sistema (inferido):** Registrar ingreso de mercadería (compras); registrar salida de repuestos hacia Taller/Servicios de Campo; realizar ajustes/conteos de inventario; alertar sobre quiebres de stock.
- **Módulos con los que interactúa:** Inventario, Compras (recepción).
- **Pendiente de validar:** ¿Existe un almacenero por línea de negocio o uno solo para el almacén único? → **BQ-002** (ver Business-Questions)

### ACT-007 — Técnico
- **Tipo:** Primario / Interno
- **Estado:** [C]
- **Descripción:** Ejecuta el diagnóstico y reparación de equipos en Taller, y/o trabajos en campo.
- **Objetivos frente al sistema (inferido):** Registrar diagnóstico; registrar repuestos consumidos; actualizar estado de una OT o de un Servicio de Campo; registrar evidencia de trabajo realizado.
- **Módulos con los que interactúa:** Taller, Servicios de Campo, Inventario (consumo).
- **Pendiente de validar:** ¿"Técnico" es un único rol para Taller y Campo, o son roles distintos con permisos distintos (p. ej. el técnico de campo necesita acceso remoto/móvil)? → **BQ-041**

### ACT-008 — Contador
- **Tipo:** Primario / Interno
- **Estado:** [C]
- **Descripción:** Responsable del control contable y tributario de la empresa.
- **Objetivos frente al sistema (inferido):** Consultar/exportar comprobantes emitidos; conciliar caja; verificar cumplimiento de series/correlativos SUNAT; generar reportes contables.
- **Módulos con los que interactúa:** Caja, Ventas, Compras, Reportes, integración SUNAT.
- **Pendiente de validar:** ¿Usa el ERP directamente o solo recibe reportes/exportaciones para un sistema contable externo? → **BQ-062**

## 5. Actores humanos externos

### ACT-009 — Cliente
- **Tipo:** Primario / Externo
- **Estado:** [C]
- **Descripción:** Persona natural o jurídica que adquiere productos, solicita reparaciones o contrata servicios de campo.
- **Objetivos frente al sistema:** Hoy, es tratado como **sujeto de datos** gestionado por el personal interno (no hay evidencia de un canal de autoatención).
- **Pendiente de validar (crítico — afecta el modelo de seguridad):** ¿El Cliente accederá alguna vez directamente al sistema (portal web, app) para aprobar cotizaciones, ver el estado de su equipo, o descargar su comprobante? → **BQ-006** (ver Business-Questions). Mientras no se confirme, se asume que el Cliente **no es un usuario del sistema**, solo una entidad administrada por otros actores.

### ACT-010 — Proveedor
- **Tipo:** Secundario / Externo
- **Estado:** [I]
- **Descripción:** Empresa o persona que suministra productos/mercadería a ISARMIN. Mencionado como módulo de datos ("Proveedores") en el alcance del sistema, no como actor que interactúa directamente con el sistema.
- **Pendiente de validar:** ¿Existirá algún tipo de portal o integración donde el proveedor interactúe directamente (p. ej. confirmar una orden de compra)? Se asume que no, salvo indicación contraria. → **BQ-024**

## 6. Actores sistema (externos, no humanos)

### ACT-011 — SUNAT / Operador de Servicios Electrónicos (OSE/PSE)
- **Tipo:** Secundario / Sistema externo
- **Estado:** [I]
- **Descripción:** Entidad reguladora peruana y su infraestructura de facturación electrónica. El sistema deberá comunicarse con este actor para validar y transmitir comprobantes electrónicos (boleta/factura) si así lo exige la normativa vigente para el tipo de contribuyente de ISARMIN.
- **Pendiente de validar:** ¿La facturación electrónica es obligatoria desde el lanzamiento del sistema? ¿A través de qué proveedor OSE/PSE? → **BQ-050**, **BQ-051**

### ACT-012 — Impresora de Comprobantes / Ticketera
- **Tipo:** Secundario / Sistema externo (hardware)
- **Estado:** [I]
- **Descripción:** Dispositivo de impresión de boletas, facturas, tickets o comprobantes de recepción de equipos. Mencionado indirectamente en `PROJECT_CONTEXT.md` como parte del equipamiento físico aceptado.
- **Pendiente de validar:** Modelo(s) de impresora, formato (térmico 80mm/58mm, A4), si ya existen equipos comprados. → **BQ-055**

### ACT-013 — Lector de Código de Barras
- **Tipo:** Secundario / Sistema externo (hardware)
- **Estado:** [I]
- **Descripción:** Dispositivo para identificación rápida de productos en Ventas e Inventario. Mencionado en `PROJECT_CONTEXT.md` como equipamiento físico aceptado.
- **Pendiente de validar:** ¿Ya existe codificación de barras en los productos actuales, o debe generarse desde el sistema? → **BQ-056**

## 7. Matriz preliminar Actor–Módulo (hipótesis — [PV])

> Esta matriz es una **hipótesis de trabajo** para orientar el diseño de permisos. Ninguna celda debe considerarse definitiva hasta validar la matriz real de permisos con el cliente (**BQ-063**).

| Módulo | Admin | Gerente | Supervisor | Recepcionista | Vendedor | Almacenero | Técnico | Contador |
|---|---|---|---|---|---|---|---|---|
| Usuarios/Roles | ✔ | — | — | — | — | — | — | — |
| Clientes | ✔ | Consulta | Consulta | ✔ | ✔ | — | — | Consulta |
| Proveedores | ✔ | Consulta | — | — | — | ✔ | — | Consulta |
| Inventario | ✔ | Consulta | Consulta | — | Consulta | ✔ | Consulta | — |
| Compras | ✔ | Aprueba (PV) | — | — | — | ✔ | — | Consulta |
| Ventas | ✔ | Consulta | Consulta | — | ✔ | Consulta | — | Consulta |
| Caja | ✔ | Consulta | — | — | Parcial (PV) | — | — | ✔ |
| Taller (OT) | ✔ | Consulta | ✔ | ✔ | — | Consulta | ✔ | — |
| Servicios de Campo | ✔ | Consulta | ✔ | Parcial (PV) | — | Consulta | ✔ | — |
| Reportes | ✔ | ✔ | Parcial (PV) | — | — | — | — | ✔ |
| Auditoría | ✔ | Consulta | — | — | — | — | — | — |
| Configuración | ✔ | — | — | — | — | — | — | — |

## 8. Preguntas abiertas relacionadas

Ver [Business-Questions.md](../02-Business/Business-Questions.md), sección **Usuarios y Roles** (BQ-041 a BQ-049) y **Infraestructura** (BQ-055 a BQ-057), para el listado completo de preguntas derivadas de este documento.

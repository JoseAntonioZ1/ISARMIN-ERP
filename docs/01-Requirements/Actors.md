# Actors.md — Actores del Sistema

## 1. Propósito

Este documento identifica y describe a todos los actores (humanos y sistemas externos) que interactúan con ISARMIN ERP, siguiendo el enfoque de modelado de actores de **UML (Use Case Diagrams)** y el análisis de stakeholders propuesto por **BABOK v3** (sección "Stakeholder Analysis").

Un actor es cualquier persona, rol, sistema o dispositivo externo que **envía o recibe información del sistema**, sin que ello implique necesariamente una cuenta de usuario individual (un actor es un rol, no una persona física — una misma persona puede ejercer más de un actor, y un actor puede ser ejercido por más de una persona).

Este documento es la base para:
- El diseño de la matriz de roles y permisos (módulo Usuarios/Roles).
- La identificación de casos de uso (un caso de uso siempre involucra al menos un actor primario).
- El diseño de las interfaces de integración con sistemas externos.

**Nota de mantenimiento de IDs:** los identificadores `ACT-XXX` se mantienen estables respecto a la versión anterior de este documento, aunque el rol descrito en algunos de ellos haya sido descartado tras la validación con el propietario (2026-07-18). Esto evita romper las referencias cruzadas ya existentes en `Business-Questions.md`, `Business-Rules.md`, `Business-Catalogs.md` y `Business-Risks.md`.

## 2. Convención utilizada en este documento

| Marca | Significado |
|---|---|
| **[C] Confirmado** | Declarado o validado explícitamente por el propietario de ISARMIN PERÚ S.A.C. |
| **[I] Inferido** | Deducido razonablemente del contexto de negocio ya declarado; el rol existe, pero su alcance exacto requiere validación. |
| **[PV] Pendiente de Validación** | Hipótesis de trabajo del analista, sin base documental directa. Debe confirmarse, ajustarse o descartarse con el cliente. |
| **[DESCARTADO]** | Rol especulado en la versión original de este documento que la validación directa con el propietario (2026-07-18) confirmó que **no existe** en la operación real. Se conserva documentado, no se elimina, para preservar la trazabilidad de la decisión (conforme a `AI_INSTRUCTIONS.md`: "nunca elimines funcionalidades sin autorización"). |

Toda fila marcada `[I]` o `[PV]` referencia el identificador de pregunta correspondiente en [Business-Questions.md](../02-Business/Business-Questions.md).

## 3. Clasificación de actores

Siguiendo UML, se distingue entre:
- **Actor primario**: inicia una interacción para lograr un objetivo de negocio.
- **Actor secundario**: es requerido por el sistema para completar un caso de uso, pero no lo inicia (ej. el sistema de SUNAT valida un comprobante).
- **Actor humano interno**: personal de ISARMIN PERÚ S.A.C.
- **Actor humano externo**: no pertenece a la empresa (clientes, proveedores).
- **Actor sistema**: software o hardware externo con el que el ERP se integra.

## 4. Contexto real de la operación (validado con el propietario, 2026-07-18)

ISARMIN PERÚ S.A.C. es una empresa pequeña que opera en **un único local propio**, dividido en dos áreas físicas (no dos sedes ni dos almacenes): **Área 1 — Tienda/Recepción** (venta, atención al cliente, recepción y entrega de equipos, cotizaciones) y **Área 2 — Taller/Almacén** (reparaciones, diagnósticos, mantenimiento, almacenamiento). Actualmente existen **solo dos usuarios permanentes** en el negocio, más trabajadores temporales sin acceso al sistema. Esta validación **reemplaza la hipótesis original de 9 roles** por un catálogo real mucho más simple: la mayoría de los roles especulados en la versión anterior de este documento no existen como distintos entre sí, y sus funciones (cuando existen) las concentra el mismo Propietario.

## 5. Actores humanos internos

### ACT-001 — Administrador / Propietario
- **Tipo:** Primario / Interno
- **Estado:** [C] — confirmado directamente por el propietario.
- **Descripción:** Es la misma persona: dueño del negocio y técnico principal. Concentra en un solo rol las funciones que se habían especulado como roles separados (Gerente — ACT-002, Supervisor — ACT-003, y buena parte de Almacenero — ACT-006).
- **Responsabilidades reales declaradas:** Administración general, reparaciones, diagnósticos, servicios de campo, compras, gestión del negocio.
- **Acceso confirmado (fuente: propietario):** Completo — Configuración, Inventario, Compras, Ventas, Taller, Servicios, Reportes.
- **Cantidad:** Solo 1 (no existe más de un Administrador). Resuelve **BQ-057**.

### ACT-002 — ~~Gerente~~ [DESCARTADO]
- **Estado:** [DESCARTADO] — resuelve **BQ-058**.
- **Motivo:** no existe un rol "Gerente" distinto del Propietario. Sus funciones especuladas (consulta de reportes gerenciales, aprobación de operaciones de alto impacto) las concentra **ACT-001 — Administrador/Propietario**.

### ACT-003 — ~~Supervisor~~ [DESCARTADO]
- **Estado:** [DESCARTADO] — resuelve **BQ-059**.
- **Motivo:** no existe un rol "Supervisor" de Taller/Campo distinto del Propietario; la supervisión (si aplica) la ejerce ACT-001, que ya tiene acceso completo.

### ACT-004 — ~~Recepcionista~~ [DESCARTADO / PARCIALMENTE ABSORBIDO]
- **Estado:** [DESCARTADO], con una ambigüedad abierta — resuelve parcialmente **BQ-060**, y origina la nueva pregunta **BQ-089**.
- **Motivo:** no existe un rol "Recepcionista" independiente. Sus funciones especuladas (registrar cliente nuevo, registrar equipo recibido, emitir comprobante de recepción) ocurren en la práctica en la misma Área 1 (Tienda/Recepción) donde trabaja **ACT-005 — Ventas**, pero el acceso confirmado para Ventas (ver ACT-005) **no incluye explícitamente el módulo de Taller**. No se asume que Ventas absorbe esta función hasta que se confirme con el propietario.

### ACT-005 — Ventas (antes "Vendedor")
- **Tipo:** Primario / Interno
- **Estado:** [C] — confirmado directamente por el propietario.
- **Descripción:** Persona responsable de la atención y venta en el Área 1 (Tienda/Recepción). En la operación real se llama "Encargada de Ventas".
- **Responsabilidades reales declaradas:** Atención al cliente, ventas de tienda, registro de ventas, apoyo administrativo.
- **Acceso confirmado (fuente: propietario):** Clientes, Productos, Ventas, Caja, Consulta de inventario.
- **Resuelve:** **BQ-061** (sí cobra directamente — el rol incluye Caja, no existe un cajero independiente).
- **⚠️ Ambigüedad detectada, no asumida (BQ-089):** el acceso confirmado **no incluye explícitamente Taller/Órdenes de Trabajo**, pese a que la recepción y entrega de equipos ocurre físicamente en la misma área donde esta persona trabaja. No se debe asumir si registra la recepción/entrega de equipos en el sistema — debe confirmarse explícitamente.

### ACT-006 — ~~Almacenero~~ [DESCARTADO]
- **Estado:** [DESCARTADO] — contribuye a resolver **BQ-002**.
- **Motivo:** no existe un rol "Almacenero" independiente. El control del inventario (recepción de compras, ajustes) lo realiza el Propietario (ACT-001), consistente con que también es quien hace las compras, y con que se confirmó un almacén único (no requiere un responsable de almacén separado por línea de negocio).

### ACT-007 — Técnico
- **Tipo:** Primario / Interno
- **Estado:** [C] — confirmado directamente por el propietario. Resuelve **BQ-041**: es un **único rol**, sin distinción entre Taller y Campo (consistente con que hoy es el mismo Propietario quien ejecuta ambos).
- **Descripción:** Ejecuta diagnóstico, reparación y mantenimiento de equipos en Taller, y trabajos técnicos en Servicios de Campo. Hoy lo ejerce el propio Propietario (bajo su acceso de Administrador), pero el catálogo de roles del sistema debe contemplar "Técnico" como rol independiente para cuando se contrate personal técnico adicional.
- **Acceso confirmado (fuente: propietario):** Taller, Diagnósticos, Órdenes de Trabajo, Consumo de materiales.
- **Nota:** dado que hoy el Técnico y el Administrador son la misma persona, no fue necesario para el propietario resolver formalmente si un usuario puede tener más de un rol simultáneo — **BQ-042 queda parcialmente resuelta** (no es una necesidad urgente hoy, pero el sistema debería permitirlo para cuando se contrate personal).

### ACT-008 — ~~Contador~~ [DESCARTADO PARA EL MVP]
- **Estado:** [DESCARTADO PARA EL MVP] — resuelve **BQ-062** con este matiz.
- **Motivo:** no fue mencionado como usuario del sistema en la validación del propietario. Se asume, sin confirmación explícita adicional, que no es un actor interno del sistema en el MVP; si existe un contador externo, probablemente reciba reportes/exportaciones fuera del sistema.

### Personal sin acceso al sistema (no son actores del sistema)
- **Trabajadores temporales** (ej. ayudantes para instalaciones, personal contratado para proyectos específicos de Servicios de Campo): confirmado por el propietario que **no administran información y no tendrán usuario propio** en esta fase. No requieren modelarse como actor del sistema, solo — eventualmente — como un dato de referencia dentro de una Orden de Trabajo o Servicio de Campo (ej. "ayudante asignado"), si el negocio lo solicita más adelante.

## 6. Actores humanos externos

### ACT-009 — Cliente
- **Tipo:** Primario / Externo
- **Estado:** [C] — confirmado.
- **Descripción:** Persona natural o jurídica que adquiere productos, solicita reparaciones o contrata servicios de campo.
- **Acceso al sistema:** **Ninguno.** Resuelto por `PROJECT_SCOPE.md` ("Portal para clientes" está fuera del alcance inicial) y reafirmado por la ausencia total de mención a un canal de autoatención en la validación del propietario. Resuelve **BQ-006**. Es exclusivamente una entidad administrada por ACT-001 y ACT-005.

### ACT-010 — Proveedor
- **Tipo:** Secundario / Externo
- **Estado:** [I]
- **Descripción:** Empresa o persona que suministra productos/mercadería a ISARMIN. El propietario confirmó que las compras se hacen "directamente a proveedores u otras tiendas", sin describir ningún tipo de acceso o portal para el proveedor.
- **Pendiente de validar:** ¿Existirá algún tipo de portal o integración donde el proveedor interactúe directamente? Dado el tamaño de la operación, es muy improbable, pero no se asume sin confirmación explícita. → **BQ-024**

## 7. Actores sistema (externos, no humanos)

### ACT-011 — SUNAT / Operador de Servicios Electrónicos (OSE/PSE)
- **Tipo:** Secundario / Sistema externo
- **Estado:** [I]
- **Descripción:** Entidad reguladora peruana y su infraestructura de facturación electrónica.
- **Estado de la integración:** Parcialmente resuelto — `PROJECT_SCOPE.md` confirma que "podrá implementarse en una fase posterior", no es obligatoria desde el lanzamiento. Pendiente: proveedor OSE/PSE, RUC/régimen tributario activo. → **BQ-050, BQ-051, BQ-082**

### ACT-012 — Impresora de Comprobantes / Ticketera
- **Tipo:** Secundario / Sistema externo (hardware)
- **Estado:** [I]
- **Pendiente de validar:** Modelo(s), formato, si ya existen equipos comprados. → **BQ-055**

### ACT-013 — Lector de Código de Barras
- **Tipo:** Secundario / Sistema externo (hardware)
- **Estado:** [I]
- **Pendiente de validar:** ¿Ya existe codificación de barras en los productos actuales? No mencionado en la validación del propietario (que describe la operación actual basada en recibos manuales, lo que sugiere que probablemente no exista aún, pero no se asume). → **BQ-056**

## 8. Matriz real de permisos (confirmada por el propietario — reemplaza la hipótesis anterior)

> Esta matriz refleja la información entregada directamente por el propietario el 2026-07-18. Resuelve **BQ-063**. La única celda no confirmada explícitamente es la de Ventas sobre Taller, señalada abajo y en **BQ-089**.

| Módulo | Administrador/Propietario (ACT-001) | Ventas (ACT-005) | Técnico (ACT-007) |
|---|---|---|---|
| Configuración | ✔ | — | — |
| Usuarios/Roles | ✔ | — | — |
| Clientes | ✔ | ✔ | — |
| Proveedores | ✔ | — | — |
| Productos | ✔ | ✔ | Consulta |
| Inventario | ✔ | Consulta | Consulta |
| Compras | ✔ | — | — |
| Ventas | ✔ | ✔ | — |
| Caja | ✔ | ✔ | — |
| Taller / Diagnósticos / Órdenes de Trabajo | ✔ | ⚠️ **No confirmado (BQ-089)** | ✔ |
| Servicios de Campo | ✔ | — | ✔ |
| Reportes | ✔ | — | — |
| Auditoría (bitácora transversal, no un módulo con UI propia) | ✔ (implícito, todo queda auditado) | — | — |

## 9. Preguntas abiertas relacionadas

La mayoría de las preguntas originadas en este documento fueron resueltas o parcialmente resueltas el 2026-07-18. Quedan abiertas: **BQ-042** (multi-rol, baja urgencia), **BQ-043, BQ-044, BQ-047, BQ-048** (sesión y contraseñas), **BQ-045** (permisos por sede, baja urgencia dado que se confirmó sede única), y de forma prioritaria **BQ-089** (acceso de Ventas a Taller/recepción de equipos). Ver [Business-Questions.md](../02-Business/Business-Questions.md), sección **11. Usuarios y Roles**.

# UX-Design.md — Diseño de Experiencia de Usuario

## 1. Propósito

Define la arquitectura de información, la navegación por rol y los flujos de pantalla principales del frontend (React 19 + TypeScript), derivados de [Use-Cases.md](../01-Requirements/Use-Cases.md) y consumiendo los contratos de [API-Design.md](../05-Backend-API/API-Design.md).

**Alcance de este documento:** estructura de pantallas, navegación y flujo — no mockups visuales pixel-perfect (esos se producen en Draw.io/Figma como herramienta externa, conforme a `TECH_STACK.md`, y no forman parte de este repositorio de documentación en Markdown).

## 2. Principios de UX

1. **Usabilidad para personal sin formación técnica avanzada** (RNF-023): texto claro sobre iconografía ambigua, sin jerga técnica, formularios cortos con valores por defecto sensatos.
2. **Minimizar pasos en operaciones de alta frecuencia** (RNF-024): registrar una venta o recibir un equipo son las operaciones más repetidas del día — no deben requerir más de 2-3 pantallas.
3. **Validación en dos capas, nunca solo una** (`CODING_STANDARDS.md`): Zod en el formulario (feedback inmediato) + FluentValidation en el backend (autoridad final) — el frontend nunca asume que su validación es suficiente.
4. **Confirmación explícita para acciones irreversibles o sensibles**: anular una venta, ajustar inventario, autorizar un saldo pendiente, desactivar un cliente — siempre un diálogo de confirmación, nunca una acción de un solo clic.
5. **Reflejo de la autorización configurable** (ADR-008): la UI oculta/deshabilita acciones para las que el usuario no tiene el permiso lógico correspondiente — no solo el backend las rechaza, para no confundir al usuario con botones que fallan.

## 3. Navegación por rol

El menú lateral se genera dinámicamente a partir de los `permisos` efectivos del usuario (mismo mecanismo de `Architecture-Overview.md` §8), no de una lista fija por rol. La siguiente tabla es el resultado esperado **hoy**, con los 3 roles reales (`Actors.md` §8):

| Sección de menú | Administrador | Ventas | Técnico |
|---|---|---|---|
| Dashboard | ✔ | ✔ | ✔ |
| Clientes | ✔ | ✔ | Consulta |
| Proveedores | ✔ | — | — |
| Inventario | ✔ | Consulta | Consulta |
| Compras | ✔ | — | — |
| Ventas | ✔ | ✔ | — |
| Caja | ✔ | ✔ | — |
| Taller (Recepción / OT) | ✔ | ✔ (recepción/entrega) | ✔ |
| Servicios de Campo | ✔ | — | ✔ |
| Reportes | ✔ | — | — |
| Configuración | ✔ | — | — |
| Auditoría | ✔ | — | — |

## 4. Flujos de pantalla principales

Notación: `[Pantalla]` → `(Acción)` → `[Pantalla siguiente]`. Cada flujo referencia su Caso de Uso y endpoint.

### 4.1 Inicio de sesión (UC-01)
```
[Login] — usuario, contraseña
   → (Ingresar) → POST /auth/login
      ✔ éxito → [Dashboard] (según permisos del token)
      ✘ error → mensaje inline "Usuario o contraseña incorrectos", sin detalle adicional (no revelar cuál campo falló, por seguridad)
```

### 4.2 Recepción de Equipo (UC-22) — el flujo más frecuente de Taller
```
[Taller > Nueva Recepción]
  Campos: Cliente (buscar o crear rápido), Equipo (descripción), Falla reportada
   → (Registrar) → POST /ordenes-trabajo
      → [Comprobante de Recepción] (para imprimir — ACT-012)
      → [Detalle de OT #123] (estado: Recibido)
```
Accesible para Administrador, Ventas y Técnico (RN-029) — el botón "Nueva Recepción" aparece para los 3 roles, sin distinción, reflejando que la recepción no es exclusiva de nadie.

### 4.3 Ciclo completo de una Orden de Trabajo
```
[Detalle de OT] (estado: Recibido)
   → (Registrar Diagnóstico) → POST /ordenes-trabajo/{id}/diagnostico → estado: Diagnosticado
   → (Generar Cotización) → POST /ordenes-trabajo/{id}/cotizacion → estado: Cotizado
   → (Registrar Decisión del Cliente)
        ✔ Aprueba → POST /ordenes-trabajo/{id}/decision {aprobada:true} → estado: Aprobado
        ✘ Rechaza → POST /ordenes-trabajo/{id}/decision {aprobada:false, cobroDiagnostico?} → estado: Rechazado (fin)
   → (Iniciar Reparación) → seleccionar repuestos consumidos (buscador de producto con stock visible) → POST /ordenes-trabajo/{id}/reparacion → estado: ListoParaEntrega
   → (Entregar Equipo) → [Pantalla de Entrega] (ver 4.4)
```

### 4.4 Entrega de Equipo con posible saldo pendiente (UC-26) — pantalla más sensible del sistema
```
[Entregar Equipo — OT #123]
  Total: S/ 80.00
  Selector: ( ) Pago completo   ( ) Adelanto   ( ) Saldo pendiente
     Si "Saldo pendiente" o "Adelanto" con monto < total:
        → aparece campo "Monto pagado ahora"
        → aparece bloque "Autorización requerida" (solo visible/habilitado si el usuario actual es Administrador;
           si no lo es, muestra: "Se requiere autorización del Administrador para continuar" con un botón
           "Solicitar autorización" — flujo de PIN o cambio de sesión, [PV] mecanismo exacto no especificado
           por el negocio, a definir en implementación)
   → (Confirmar Entrega) → POST /ordenes-trabajo/{id}/entrega
      → [Detalle de OT] (estado: Entregado, con badge "Saldo pendiente: S/ 30.00" si aplica)
```
**Nota de diseño crítica:** esta pantalla es la traducción visual directa de RN-001 (corregida) — el botón "Confirmar Entrega" **nunca** se deshabilita por falta de pago completo, solo exige el bloque de autorización cuando hay saldo pendiente.

### 4.5 Venta en Tienda (UC-14)
```
[Ventas > Nueva Venta]
  Buscador de productos (muestra stock disponible en la lista) → agregar líneas
  Total calculado en vivo
  Tipo de comprobante (Cotización/Boleta/Factura/Nota de Venta/Ticket)
  Medios de pago (uno o más — Efectivo/Yape/Plin/Transferencia)
   → (Registrar Venta) → POST /ventas
      ✘ 409 STOCK_INSUFICIENTE → mensaje inline en la línea afectada, no bloquea el resto del formulario
      ✔ éxito → [Comprobante emitido] (para imprimir/descargar)
```

### 4.6 Caja (UC-19)
```
[Caja] (si no hay caja abierta hoy) → [Abrir Caja] (monto inicial) → POST /caja/apertura
[Caja] (abierta) → lista de movimientos del turno en vivo (ventas, cobros de Taller/Campo, egresos)
   → (Cerrar Caja) → [Cierre de Caja] (monto teórico mostrado, campo "monto físico contado")
      → POST /caja/cierre → [Resumen de cierre] (diferencia resaltada si no cuadra)
```

### 4.7 Servicio de Campo (UC-30 a UC-33)
```
[Servicios de Campo > Nueva Solicitud] → cliente, descripción → POST /servicios-campo
[Detalle de Servicio] → (Cotizar) → (Cerrar Servicio: estado final + observaciones) → (Cobrar)
```
Análogo a Taller, con menos pantallas por tener un flujo más simple (sin diagnóstico/pruebas formales).

## 5. Componentes UI compartidos (`shared/components/`)

| Componente | Uso |
|---|---|
| `DataTable` | Listados paginados (clientes, productos, ventas, OT...) con búsqueda y filtros — envuelve TanStack Query. |
| `FormField` | Input + label + mensaje de error, integrado con React Hook Form + Zod. |
| `ConfirmDialog` | Confirmación obligatoria antes de acciones irreversibles (sección 2.4). |
| `StatusBadge` | Muestra visualmente el estado de una OT/Venta/Servicio con color consistente (ej. Recibido=gris, Aprobado=azul, Entregado=verde, Rechazado=rojo). |
| `PermissionGate` | Envuelve un botón/sección y la oculta si el usuario no tiene el permiso lógico requerido (sección 2.5). |
| `FileUpload` | Adjuntar documentos/fotos (UC-34), reutilizado en Compras/Ventas/Taller/Campo. |
| `Toast` | Notificaciones no bloqueantes de éxito/error tras una acción. |
| `SaldoPendienteBadge` | Indicador visual reutilizado en OT, Venta y Servicio de Campo para mostrar saldo pendiente — refuerza visualmente que es un concepto transversal (Cobranzas). |

## 6. Manejo de carga y error

- **Carga:** *skeletons* (placeholders) en listados, no solo un spinner genérico — reduce la percepción de espera.
- **Error de red/servidor:** Toast con mensaje genérico + reintento; nunca se expone el detalle técnico del error 500 (Serilog lo registra en el backend).
- **Error de validación (400/409):** inline, junto al campo o bloque específico afectado, usando el `codigo` del envoltorio de error de `API-Design.md` §4 para elegir el mensaje en español (no se muestra el `codigo` técnico al usuario).

## 7. Responsive

V1 es exclusivamente web (RF-071, confirmado); no se diseña una app móvil nativa. Sin embargo, dado que Recepción de Equipo y Servicios de Campo son operaciones que un Técnico podría eventualmente realizar desde una tablet en el mismo local, las pantallas de estos módulos se diseñan **mobile-first dentro de lo razonable** (Tailwind CSS, breakpoints estándar), sin que esto implique construir una PWA ni soporte offline (fuera de alcance V1, punto de extensión ya documentado en `Architecture-Overview.md` §12).

## 8. Explícitamente fuera de alcance (V1)

Consistente con `PROJECT_SCOPE.md` y `Architecture-Overview.md` §12: sin captura de firma digital en pantalla, sin adjuntar evidencia fotográfica obligatoria, sin flujo de aprobación multi-nivel para ninguna pantalla, sin modo offline.

## 9. Siguiente paso

Con Arquitectura, Modelo de Datos, API y UX definidos, la Fase 3 del roadmap queda completa. El siguiente hito natural es `07-Development/` (Fase 4): configurar el proyecto base (solución .NET + proyecto React) y comenzar la implementación módulo por módulo, priorizando según `PROJECT_ROADMAP.md`.

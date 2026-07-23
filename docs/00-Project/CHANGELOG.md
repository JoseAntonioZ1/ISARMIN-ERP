# CHANGELOG.md

# Historial de Cambios

Todos los cambios importantes del proyecto serán registrados en este documento.

---

## [0.26.0] - 23/07/2026

### Agregado

- **Mejora de UX transversal — séptimo pedido de mejora post-Fase 4 ("analiza toda la página y mejora la experiencia de usuario del frontend"), sin dirección específica más allá de "toda la página".** Antes de tocar código se hizo una auditoría completa (3 exploraciones en paralelo: infraestructura compartida, módulos CRUD de catálogo, módulos operativos) que confirmó con datos concretos los mismos problemas repetidos en 8+ pantallas. El propietario, ante el tamaño real de lo encontrado, eligió explícitamente el alcance: construir componentes compartidos y retro-aplicarlos a Clientes, Proveedores, Productos, Usuarios, Roles, Categorías, Medios de Pago y Unidades de Medida — dashboard de Inicio, hub de Configuración, gráficos/exportación en Reportes y el hueco de estados de Servicios de Campo quedan fuera de esta pasada (anotados como recomendación futura, el último probablemente necesita un endpoint nuevo de backend).
  - **5 componentes/hook compartidos nuevos** en `frontend/src/shared/`: `EstadoBadge` (pastilla de color para Activo/Inactivo, antes texto plano sin diferenciación visual en 5 módulos), `ControlesPaginacion` (Anterior/Siguiente + "Mostrando X–Y de Z" — el backend de Clientes/Proveedores/Productos/Usuarios ya devolvía `total`/`pagina`/`tamanoPagina` en cada listado, pero ninguna pantalla los usaba: cualquier lista de más de 20 registros era simplemente inalcanzable), `EstadoCarga` (reemplaza 23+ `<p>Cargando...</p>` sueltos por un spinner), `EstadoVacio` (mensaje de "sin resultados", ninguna de las 8 pantallas lo tenía), y `useToast`/`ToastProvider` (notificaciones de éxito/error montadas una vez en `App.tsx`, reemplazando el patrón `useState<string|null>` + `<p roja>` reinventado en cada página — y agregando confirmación de éxito, que antes no existía en ningún lado).
  - **Retro-aplicado en las 8 páginas** con el mismo patrón: paginación real (Clientes/Proveedores/Productos/Usuarios), badges de estado, toasts en vez de error inline, manejo del error de la propia consulta de lista (antes solo se manejaban errores de mutación, nunca de la carga inicial), y estado vacío.
  - **Corrección real de inconsistencia encontrada**: `MediosPagoPage` desactivaba un medio de pago con un solo clic, sin la confirmación (`ConfirmDialog`) que ya protege el mismo tipo de acción en Clientes/Proveedores/Productos/Usuarios — corregido para ser consistente.
  - **Bug de caché encontrado y corregido de paso**: crear un rol nuevo no invalidaba el listado resumido (`['roles']`) usado por el selector de roles al crear un Usuario — solo invalidaba `['roles-detalle']`. Ahora invalida ambos.
  - Pequeño extra en Productos: resaltado en rojo cuando el stock actual está en o por debajo del stock mínimo (el dato ya existía en el modelo pero nunca se comparaba visualmente en la lista), y la tabla ahora tiene scroll horizontal propio en vez de desbordar la página en pantallas angostas.

**Explícitamente fuera de esta pasada** (encontrado en la auditoría, no se toca ahora): dashboard real en la pantalla de Inicio (hoy solo logo + mensaje estático), rediseño de Configuración como hub con tarjetas (hoy una lista de links sin diseño), gráficos/exportación en Reportes, inconsistencia de formato de moneda en `CajaPage` (falta el prefijo "S/" en la tabla de movimientos, sí lo tiene en Reportes), y el hueco de Servicios de Campo donde 2 de sus 4 estados (Agendado/EnEjecucion) no tienen ninguna acción disponible en la UI actual.

Verificado: `npm run build`/`npx oxlint` limpios (1 advertencia no bloqueante de fast-refresh en `useToast.tsx`, patrón estándar de contexto+hook); confirmado con datos reales contra Postgres que la paginación funciona de punta a punta (141 productos, página 1 y página 2 devuelven registros distintos). Sin cambios de backend.

---

## [0.25.0] - 23/07/2026

### Agregado

- **Rediseño de Taller como tablero Kanban — sexto pedido de mejora post-Fase 4 ("mejora las vistas para Taller, para una mejor experiencia de usuario"), sin dirección específica dada por el propietario más allá de "mejor UX".** Antes de tocar código, se revisó a fondo el módulo actual y se confirmaron 2 decisiones de diseño con el propietario: tablero Kanban por estado (en vez de tabla mejorada) y agregar el comprobante de recepción que UC-22/RF-051-053 pide y nunca se había implementado. **Sin cambios de backend** — es una reconstrucción de las vistas, no del negocio; las 9 transiciones de estado ya existentes se probaron E2E sin ninguna modificación (recepción → diagnóstico → cotización → decisión → reparación → entrega, ciclo completo verificado contra Postgres real).
  - **`TallerPage.tsx`, reescrita como tablero Kanban**: columnas por estado en orden de flujo (Recibido, Diagnosticado, Cotizado, Aprobado, Listo para Entrega, Entregado) más una columna aparte para Rechazado (estado terminal, atenuada visualmente). Cada OT es una tarjeta con equipo, cliente y fecha; un clic abre el mismo `OrdenTrabajoDetalleDialog` de siempre, sin tocar la lógica de transiciones. Se agregó un buscador de texto libre (cliente/equipo/falla), 100% cliente-side, y `ordenesTrabajoApi.buscar` ahora pide hasta 200 resultados en vez de 20 para que el tablero muestre todo el trabajo abierto de un vistazo — el backend ya soportaba filtros por `estado`/`cliente` que nunca se habían expuesto en la UI.
  - **Nuevo `estadoOtVisual.ts`**: mapeo único estado→color/ícono para los 7 estados alcanzables (`EnReparacion`/`EnPruebas` nunca se persisten, confirmado en el propio código de `RegistrarReparacionCommand`), reutilizado en las tarjetas del Kanban y en el badge del diálogo de detalle (antes texto plano sin color).
  - **Timeline visual dentro de `OrdenTrabajoDetalleDialog`**: Recepción → Diagnóstico → Cotización → Decisión → Reparación → Entrega → Garantía, cada paso marcado como completado/actual/pendiente (y "Decisión" en rojo si fue rechazada) — capa puramente visual sobre los mismos datos que el diálogo ya recibía, sin pedir nada nuevo al backend.
  - **Campo de evidencia de aprobación completado**: el backend ya soportaba `evidenciaAprobacion` en `CotizacionReparacion.RegistrarDecision` desde que se construyó el módulo, pero el formulario de "Aprobar" nunca lo pedía (siempre mandaba `null`). Se agregó el campo de texto libre — la forma exacta de evidenciar sigue sin resolver a nivel de negocio (BQ-034 sigue abierta), pero capturar lo que el propio backend ya permitía es terminar de conectar algo a medio construir, no inventar una regla nueva.
  - **Atajo para el callejón sin salida de `Rechazado`**: antes solo mostraba un texto estático. Ahora un botón "Registrar nueva orden para este cliente" cierra el diálogo y abre "Nueva recepción" con el cliente ya preseleccionado (`RegistrarRecepcionDialog` gana una prop opcional `clienteInicialId`, sin cambios de API).
  - **Comprobante de recepción de equipo en PDF** (UC-22/RF-051 a RF-053, nunca implementado ni en backend ni en frontend): nuevo `comprobanteRecepcionPdf.ts`, mismo patrón jsPDF que el comprobante de venta — se descarga automáticamente al registrar una recepción exitosa.

Verificado: `npm run build`/`npx oxlint` limpios; ciclo E2E completo contra Postgres real (recepción → diagnóstico → cotización → decisión aprobada con evidencia guardada correctamente → reparación → entrega con pago completo), confirmando que el rediseño de vistas no alteró ningún comportamiento del backend.

---

## [0.24.0] - 23/07/2026

### Agregado

- **Rediseño de Compras con el mismo patrón de 3 paneles de Ventas — quinto pedido de mejora post-Fase 4, adaptado (no copiado) al caso de comprar en vez de vender.** Sin cambios de API/backend: `comprasApi`/`DatosRegistrarCompra` y el agregado `Compra` quedan exactamente iguales, esto fue una reconstrucción de la pantalla, no del negocio.
  - **Refactor de reutilización primero**: `CategoriaSidebarPos` y `ProductoBuscadorGrid` (creados para el POS de Ventas) se movieron de `modules/ventas/components/pos/` a `shared/components/pos/`, y `iconoParaCategoria` a `shared/utils/iconosPos.ts` — son genéricos (filtrar por categoría, buscar productos), no específicos de vender, así que vivían en el módulo equivocado para que Compras los reutilizara sin duplicar ~150 líneas de lógica de debounce/lector de barras/grilla.
  - **`ProductoBuscadorGrid` ganó 2 props opcionales** (con default que preserva el comportamiento exacto de Ventas): `obtenerPrecio` (qué precio mostrar en la tarjeta — Ventas usa `precioVenta`, Compras usa `costoReferencia`) y `validarStock` (Ventas no deja vender sin stock; en Compras el stock bajo o en cero es justo el motivo para comprar, así que no debe bloquear el botón "Agregar" ni marcarse en rojo).
  - **Nuevo `CarritoPanelCompra`** (panel derecho, específico de Compras, no compartido): selector de **proveedor** en vez de cliente, **fecha** (editable — a diferencia de Ventas, el backend de Compras la recibe del cliente, no la genera el servidor), tipo de documento (Factura/Boleta/Guía de Remisión/Otro) y número de documento del proveedor, líneas con cantidad +/- y **costo unitario editable directamente por línea** (sin concepto de descuento — así ya funcionaba el diálogo anterior), Total, botón "Registrar Compra". **Deliberadamente sin** lo que no aplica a una compra: sin medios de pago/tarjetas (Compras no registra pagos, es un dato que no existe en el modelo), sin monto recibido/vuelto, sin IGV desglosado, sin autorización de saldo pendiente (ninguno de estos conceptos existe en `Compra`).
  - `/compras` pasa a ser la pantalla de registrar compra (3 paneles); el historial existente (tabla + detalle, sin cambios) se mueve a `/compras/historial`, con las mismas pestañas simples que ya se usan en Ventas (`ComprasLayout`, calcado de `VentasLayout`). Se eliminaron `ComprasPage.tsx`/`RegistrarCompraDialog.tsx` (reemplazados, sin referencias restantes) — recordando que `Compra` no tiene edición ni anulación en el alcance actual (RF-033 a RF-036), así que el historial sigue siendo de solo lectura, igual que antes.

Verificado: `npm run build`/`npx oxlint` limpios; E2E manual contra Postgres real — compra registrada vía API con el mismo payload que ya usaba el diálogo anterior, stock del producto incrementado correctamente (confirma que el cambio fue puramente de pantalla, sin tocar el contrato del backend).

---

## [0.23.0] - 23/07/2026

### Agregado

- **Comprobante de venta imprimible/descargable + RN-009 (Factura exige RUC) — cuarto pedido de mejora post-Fase 4 del propietario.** Límite legal comunicado explícitamente antes de construir esto: una Factura peruana debe emitirse electrónicamente ante SUNAT (firma digital, XML/UBL, proveedor certificado PSE/OSE) — eso sigue fuera de alcance (RF-081, bloqueado por BQ-050/051/072/082, sin cambios). Lo que se construye aquí es un **comprobante interno en PDF**, regenerado siempre a partir de los datos ya persistidos de la venta (no se guarda ningún "snapshot" de lo impreso), con una leyenda explícita de que no es un documento electrónico válido ante SUNAT.
  - **Backend — RN-009 implementada** (`Business-Rules.md`, documentada desde antes pero nunca codificada): `Venta` gana un parámetro `clienteTieneRucValido` (opcional, default `false` — no rompe ninguna llamada existente en `VentaTests.cs`) y rechaza con `ArgumentException` cualquier venta `Factura` sin cliente con RUC. `RegistrarVentaCommandHandler` ahora reutiliza el `Cliente` que ya obtenía (antes solo para verificar existencia) para calcular ese dato.
  - **Bug real encontrado y corregido durante la verificación E2E, de alcance más amplio que este cambio:** `GlobalExceptionHandler` solo traducía `ValidationException` (FluentValidation) y `ExcepcionAplicacion` a respuestas controladas — cualquier `ArgumentException` simple lanzada directamente por una entidad de Domain (como la nueva regla RN-009, pero también posibles casos en `Producto`/`Cliente`) caía como 500 genérico ("Ocurrió un error inesperado") en vez de un 400 con el mensaje claro que la propia entidad ya construye. Se agregó el caso faltante, mapeando a 400 con código `SOLICITUD_INVALIDA`.
  - **Branding público gana `Direccion`**: el comprobante necesita mostrar la dirección de la empresa, y el endpoint público `/configuracion/branding` (sin permiso, para que cualquier cajero de Ventas pueda generar un comprobante sin necesitar `Configuracion.Consultar`) no la exponía — se agregó, dato no sensible (ya se imprime en cualquier comprobante entregado a un cliente).
  - Pruebas nuevas: 3 en Domain (`VentaTests`) + 2 en Application (`RegistrarVentaCommandHandlerTests`) + 1 actualizada (`ObtenerBrandingQueryHandlerTests`). 256/256 en todo el backend.
  - **Frontend**: nueva dependencia `jspdf` (generación 100% cliente, sin backend, funciona offline). `frontend/src/modules/ventas/utils/comprobantePdf.ts` arma el PDF (encabezado con logo/razón social/RUC/dirección, tipo de comprobante, cliente, tabla de productos, subtotal/descuento/IGV/total, medios de pago, leyenda de que no es válido ante SUNAT) — misma función reutilizada para la descarga automática al finalizar una venta (`VentaPosPage`) y para el botón "Descargar comprobante" agregado en el historial (`VentaDetalleDialog`), sin necesidad de guardar nada aparte.
  - `CarritoPanelPos` ahora tiene un selector de tipo de comprobante (antes el POS mandaba `'Ticket'` fijo, sin poder elegir Boleta/Factura/Nota de Venta/Cotización) — al elegir Factura, el selector de cliente se filtra para mostrar solo clientes con RUC y bloquea "Finalizar Venta" con un mensaje claro si no hay uno seleccionado, reflejando en el cliente la misma regla RN-009 ya exigida en el backend.
  - **Otras recomendaciones dadas al propietario** (no implementadas, quedan como su decisión futura): contratar un PSE/OSE certificado por SUNAT cuando decida facturar electrónicamente de verdad (son proveedores externos, no algo que se resuelva con código); el "N° de operación" del PDF no es una serie/correlativo oficial SUNAT (eso sigue bloqueado, RF-081/RN-035); va a necesitar el RUC de su propia empresa validado (ya existe el campo); conviene que consulte con su contador desde qué volumen de ventas SUNAT exige emisión electrónica obligatoria.

Verificado E2E contra Postgres real: Factura sin cliente con RUC rechazada (400, `SOLICITUD_INVALIDA`, antes devolvía 500); Factura con cliente con RUC válido aceptada; `GET /configuracion/branding` ahora incluye `direccion`; `npm run build`/`npx oxlint` limpios.

---

## [0.22.0] - 22/07/2026

### Agregado

- **Rediseño de Ventas como Punto de Venta (POS) — tercer pedido de mejora post-Fase 4 del propietario, sin RF/UC asociado directamente (evolución de UC-14/RF-038 a RF-041).** La pantalla de registrar venta pasó de un diálogo modal con filas de selects a una experiencia de 3 paneles (categorías | buscador y grilla de productos | carrito y cobro), pensada para el uso diario de caja.
  - **Backend — `Producto.Imagen`:** mismo patrón que `ConfiguracionEmpresa.Logo` (string Base64 embebido, sin validación de formato ni límite en el servidor, límite de 1.5 MB solo en frontend). Migración `AgregarImagenProducto` (columna `imagen text` nullable). `Domain`, `Application` (`RegistrarProductoCommand`/`EditarProductoCommand`/`ProductoDto`), `Infrastructure` (`ProductoConfiguration`) y `API` (`ProductosController`) actualizados; parámetro nuevo al final de constructores/DTOs, sin romper las llamadas existentes. 2 pruebas nuevas (251/251 en todo el backend).
  - **Frontend — utilidad compartida** (`shared/utils/archivos.ts`): se extrajo el patrón de carga de imagen (`archivoABase64` + límite de 1.5 MB) ya usado en el logo de empresa, reutilizado ahora también en `CrearProductoDialog`/`EditarProductoDialog` (nuevo campo de imagen por producto).
  - **`productosApi.buscar`** expone ahora el parámetro `categoria` que el backend ya soportaba (`BuscarProductosQuery.CategoriaId`) pero el frontend no exponía.
  - **Nueva pantalla `/ventas` (POS)**: panel izquierdo con categorías (buscador + tarjetas con ícono heurístico por nombre — no existe campo de ícono en `Categoria`), panel central con buscador grande (por nombre, código interno o código de barras — ya soportado por `ProductoRepository`, verificado E2E) y grilla de productos con imagen/precio/stock, panel derecho con carrito (cantidad +/-, descuento % por línea aplicado directo sobre `precioUnitario` sin cambios de API), resumen con IGV desglosado (18%, cálculo asumiendo precio ya incluido — **no es un dato almacenado, solo se muestra**), tarjetas de medio de pago, monto recibido/vuelto para efectivo, y el botón "Finalizar Venta" reutilizando exactamente la regla de negocio de saldo pendiente con autorización ya existente (RN-031).
  - **Lector de código de barras**: sin integración especial — los lectores USB/Bluetooth típicos emulan un teclado (HID keyboard wedge). El buscador central detecta Enter y, si el texto coincide exactamente con un `codigoBarras` del resultado actual, agrega el producto directo al carrito.
  - **Historial movido a `/ventas/historial`** (tabla + anular + devoluciones, sin cambios de comportamiento), con dos pestañas simples (`VentasLayout`) para alternar entre "Punto de Venta" e "Historial". Se eliminaron `VentasPage.tsx`/`RegistrarVentaDialog.tsx` (reemplazados, sin referencias restantes).
  - **Deliberadamente fuera de alcance:** el estado del carrito vive como estado local de la pantalla (no un store global), ya que no necesita persistir entre rutas — mismo criterio que ya usaba el diálogo anterior con `useState`.

Verificado: 251/251 pruebas de backend; `npm run build` (1988 módulos) y `npx oxlint` sin advertencias; E2E manual contra Postgres real vía API — producto registrado con imagen persistida correctamente, filtro por categoría y búsqueda exacta por código de barras confirmados contra datos reales del catálogo. **No se realizó una prueba visual completa en navegador** (sin herramienta de automatización de navegador disponible en este entorno) — queda pendiente que el propietario recorra la pantalla POS una vez suba el logo/pruebe con datos reales.

---

## [0.21.1] - 22/07/2026

### Agregado

- **Logo del proyecto como asset estático — decisión tomada tras consulta directa del propietario ("¿es mejor así o como estaba?")**: dado que ISARMIN ERP es un proyecto propio de una sola empresa (no un producto multiempresa/white-label), el logo dejó de depender exclusivamente de una carga manual vía Configuración → Empresa (que además se perdía cada vez que la base de datos se reseteaba en las verificaciones E2E) y ahora tiene un valor por defecto embebido en el propio build del frontend: `frontend/public/assets/logo_isarmin.png`, servido por Vite igual que cualquier otro asset estático (funciona sin internet, sin distinto al `favicon.svg` genérico que reemplaza).
  - El campo `Logo` en base de datos **no se eliminó ni cambió de tipo** — sigue existiendo para el caso de que el propietario quiera reemplazar el logo desde la UI sin necesidad de un nuevo build; simplemente ahora actúa como *override* opcional sobre el asset por defecto (`branding?.logo ?? LOGO_PREDETERMINADO`), en vez de ser la única fuente posible.
  - Aplicado en los 4 lugares donde se mostraba el logo: sidebar (`AppLayout`), login, pantalla de inicio y favicon dinámico (`AplicarBranding`); también reemplaza el favicon estático por defecto en `index.html` (antes el ícono genérico de Vite).
  - Sin cambios de backend — el archivo de imagen en sí queda pendiente de que el propietario lo coloque en `frontend/public/assets/logo_isarmin.png`.

---

## [0.21.0] - 22/07/2026

### Agregado

- **Menú lateral + paleta de colores de marca completa — segundo pedido directo del propietario de mejora post-Fase 4, sin RF/UC asociado:**
  - **Navegación**: la barra horizontal superior fue reemplazada por un menú lateral fijo (`AppLayout`), con los módulos agrupados por área (Comercial, Operaciones, Análisis, Sistema) e íconos de `lucide-react` (nueva dependencia, tree-shakeable — solo los íconos usados terminan en el bundle). El enlace activo se resuelve automáticamente vía `NavLink` de `react-router`, sin lógica manual de "ruta actual". El logo y la razón social de la empresa (branding público) se muestran en el encabezado del menú.
  - **Paleta de marca**: se definieron 6 variables CSS en `:root` (`--color-principal #EE2027`, `--color-secundario #F8C129`, `--color-neutro #FFFFFF`, `--color-terciario #7C7C7C`, `--color-apoyo #4D4D4D`, `--color-fondo #F5F8FB`) y se aplicaron en todo el frontend reemplazando los tonos `slate`/`amber` genéricos usados hasta ahora: `--color-acento` (introducida en 0.20.0) se renombró a `--color-principal` para que el color configurable desde Configuración → Empresa sea consistente con el nombre de la paleta oficial; `bg-slate-50` → `--color-fondo` (fondos de página); `text-amber-600` → `--color-secundario` (advertencias); `text-slate-700`/`text-slate-500` (solo variantes claras, sin tocar sus equivalentes `dark:`) → `--color-apoyo`/`--color-terciario` respectivamente (texto secundario y apagado). `--color-neutro` queda definida pero deliberadamente sin barrer sobre `bg-white` — sustituirlo no cambiaría nada visualmente y solo generaría ruido en el diff.
  - **Deliberadamente fuera de alcance:** no se tocaron los tonos `slate` usados como bordes/fondos de superficies oscuras (`dark:bg-slate-800`, `border-slate-200`, etc.) — la paleta entregada por el propietario cubre texto y fondos de marca, no la superficie completa del modo oscuro, y remplazarlos sin pedido explícito habría sido una reescritura visual no solicitada.
  - Sin cambios de backend ni migraciones — esta iteración es puramente de frontend (CSS + componentes de presentación).

Verificado: `npm run build` (1981 módulos, sin errores de tipos) y `npx oxlint` (sin advertencias) tras el barrido completo de clases; verificación manual de que las clases `dark:text-slate-500`/`dark:text-slate-700` existentes no fueron alteradas por los reemplazos con lookbehind negativo.

---

## [0.20.0] - 20/07/2026

### Agregado

- **Personalización visual de marca — pedido directo del propietario, no derivado de un RF/UC del backlog original (Fase 4 ya estaba completa):**
  - `ConfiguracionEmpresa` gana dos campos: `ColorAcento` (hex `#RRGGBB`, validado en Domain y en el validador de FluentValidation) y `MensajeBienvenida` (texto libre, máx. 500 caracteres). Migración `PersonalizacionEmpresa`, sin impacto en filas existentes (ambos nullable).
  - **Logo sin dependencia de internet**: el campo `Logo` (ya existente) pasó de aceptar una URL externa a guardar la imagen embebida en Base64 — se detectó que un enlace externo no se vería sin conexión a internet; al estar embebido en la propia respuesta de la API, se muestra incluso sin ninguna llamada externa. Sin migración: `Logo` ya era `text`, sin límite de tamaño. Límite de 1.5 MB aplicado en el frontend antes de codificar.
  - **Nuevo endpoint público `GET /configuracion/branding`** (`[AllowAnonymous]`, sin permiso): expone `RazonSocial`, `Ruc`, `Logo`, `ColorAcento` y `MensajeBienvenida` — el subconjunto sin datos operativos sensibles (quedan fuera `Direccion` y `MontoAperturaCajaPredeterminado`, detrás de `Configuracion.Consultar`). Necesario porque el login no tiene sesión iniciada, y porque el mensaje de bienvenida y el color de acento deben verse para *cualquier* usuario autenticado, no solo quienes tienen `Configuracion.Consultar` (hasta ahora, solo Administrador).
  - Frontend: el logo/nombre de empresa ahora se muestran en el login, el encabezado de la app (para cualquier rol), el título de la pestaña del navegador y el favicon. La pantalla de inicio muestra el mensaje de bienvenida. Los 5 reportes muestran razón social/RUC en el encabezado. El color de acento se aplica vía una variable CSS (`--color-acento`) sincronizada en tiempo real, reemplazando el gris fijo (`slate-800`) de los 43 botones de acción primaria en toda la aplicación — con filtros `brightness` para los estados hover, sin necesitar una segunda variable calculada.
  - Pruebas unitarias: 4 Domain + 4 Application nuevas/actualizadas (250/250 en todo el backend).

Verificado end-to-end contra PostgreSQL real: `GET /configuracion/branding` sin token devuelve los 5 campos; actualización con color y mensaje persiste y se refleja en el endpoint público; formato de color inválido rechazado (400 `VALIDACION_FALLIDA`); logo Base64 viaja íntegro sin truncarse.

---

## [0.19.0] - 20/07/2026

### Agregado

- **Módulo de Configuración (UC-37), backend + frontend (RF-080) — último módulo del orden de construcción acordado.** La mayor parte de lo que "Configuración" cubre conceptualmente ya estaba construido (Usuarios, Roles, Categorías, Medios de Pago, Unidades de Medida), solo enlazado desde el hub `/configuracion` sin lógica propia. Lo genuinamente nuevo:
  - `Domain`: `ConfiguracionEmpresa` — fila única sembrada en la migración (patrón singleton, mismo criterio que el usuario Administrador semilla), con `RazonSocial`, `Ruc`, `Direccion`, `Logo` y `MontoAperturaCajaPredeterminado`. No existe un comando de creación, solo `Actualizar`.
  - **RN-026 saldado:** cuando Caja se construyó, el "monto de apertura fijo y configurable" quedó explícitamente diferido con la nota "el mecanismo de configuración general... es el módulo Configuración, el último en el orden de construcción". `MontoAperturaCajaPredeterminado` resuelve ese mecanismo: `AbrirCajaDialog` ahora prellena el monto con este valor si está configurado, sin bloquear ni forzar el monto (el valor recomendado exacto por RN-026 no está confirmado por el propietario, BQ-030/BQ-088 siguen abiertas).
  - `Application`: `ObtenerConfiguracionEmpresaQuery`, `ActualizarConfiguracionEmpresaCommand`. Reutiliza los permisos `Configuracion.Consultar`/`Configuracion.Editar` ya existentes (usados por Categorías/Medios de Pago/Unidades de Medida) — ninguna acción nueva en `AccionPermiso`.
  - `Infrastructure`: `ConfiguracionEmpresaConfiguration` con la fila sembrada, `ConfiguracionEmpresaRepository`. Migración con una sola tabla nueva, sin `CHECK` constraints ni ampliación del catálogo de permisos.
  - `API`: `ConfiguracionEmpresaController` (`GET`/`PUT /configuracion/empresa`), tal como ya lo definía `API-Design.md`.
  - Frontend: `ConfiguracionEmpresaPage` (enlazada desde el hub de Configuración) y prellenado del monto de apertura en `AbrirCajaDialog`.
  - Pruebas unitarias: 6 Domain + 3 Application = 9 nuevas (244/244 en todo el backend).
  - **Deliberadamente fuera de alcance:** RF-081 (series y correlativos de comprobantes) — bloqueado por BQ-050 (parcialmente resuelta), BQ-051, BQ-072 y BQ-082, todas abiertas sobre facturación electrónica SUNAT; mismo criterio ya aplicado a RF-044/UC-15 en todo el proyecto. RF-082 ("administrar catálogos del sistema sin cambios de código") se da por satisfecho por el patrón de catálogos configurables ya construido (`medios_pago`, `categorias`, `unidades_medida`, `roles`) — no se agregó una tabla `parametros_sistema` genérica ni una API de administración de catálogos, porque ningún RF/RN/BQ la solicita explícitamente.

Verificado end-to-end contra PostgreSQL real: la fila sembrada se obtiene correctamente, actualización con datos válidos persiste (razón social, RUC, dirección, monto de apertura predeterminado), rechazo de razón social vacía y de monto negativo (400 `VALIDACION_FALLIDA`), 401 sin autenticación. Los permisos `Configuracion.Consultar`/`Configuracion.Editar` ya estaban otorgados al Administrador desde módulos anteriores — no fue necesario un otorgamiento nuevo.

Estado del proyecto:

✅ Fase de Desarrollo (Fase 4) completa — los 16 módulos del orden acordado (Autenticación, Usuarios, Roles/Permisos, Catálogos, Clientes, Proveedores, Productos, Inventario, Compras, Caja, Taller, Servicios de Campo, Ventas, Reportes y Configuración) están construidos, backend y frontend, verificados end-to-end contra PostgreSQL real. Siguiente fase: Fase 5 — Pruebas (unitarias ya cubiertas de forma continua; pendiente integración/funcional/aceptación formal).

---

## [0.18.0] - 20/07/2026

### Agregado

- **Módulo de Reportes (UC-35), backend + frontend (RF-072 a RF-076):** 5 reportes operativos de solo lectura sobre datos ya capturados por los módulos existentes — sin agregado nuevo ni reglas de negocio, es una capa de agregación pura.
  - `Application`: `IReporteRepository`, una interfaz de solo lectura separada de los repositorios transaccionales de cada módulo (no se agregaron métodos sin paginar a `IVentaRepository`/`IOrdenTrabajoRepository`/etc. porque esas consultas no se usan fuera de este caso de uso). `ReporteVentasQuery` (excluye ventas Anuladas del monto total, las incluye en el listado para trazabilidad), `ReporteInventarioQuery` (separa productos en quiebre: stock actual ≤ stock mínimo, cuando este último está definido), `ReporteOrdenesTrabajoQuery` (filtra por estado y período), `ReporteServiciosCampoQuery` (filtra por técnico asignado y período), `ReporteCajaQuery` (totaliza ingresos/egresos/saldo neto). Todos los DTOs de respuesta reutilizan los mappers ya existentes de cada módulo (`VentaDto`, `ProductoDto`, `OrdenTrabajoDto`, `ServicioCampoDto`, `CajaDto`/`MovimientoCajaDto`).
  - **Decisiones de alcance tomadas por continuidad, no por invención de reglas:** (1) RF-074 pide filtrar el reporte de OT "por técnico", pero `OrdenTrabajo` no tiene un campo de técnico asignado en el modelo confirmado (a diferencia de `ServicioCampo`) — se implementó solo con filtro de estado y período; (2) el reporte de caja refleja únicamente `movimientos_caja` (aperturas/cierres/gastos/aportes manuales) — no incluye cobros de Ventas/Taller/Servicios de Campo porque ese puente nunca se construyó (mismo hallazgo documentado desde Taller); (3) quedan fuera de alcance, todos con recomendación técnica ya documentada como no bloqueante: RF-077 (reportes gerenciales ad-hoc — rentabilidad, rotación de inventario —, bloqueado por BQ-054 abierta) y exportación/programación de reportes (PDF/Excel, envío automático — BQ-080/BQ-081, ambas abiertas y de baja prioridad).
  - `API`: `ReportesController` con los 5 endpoints ya diseñados en Fase 3, todos bajo un único permiso `Reportes.Consultar` (no se agregó ninguna acción nueva a `AccionPermiso` — `Consultar` ya existía —, así que esta es la primera implementación de módulo sin ninguna migración de EF Core).
  - Frontend: `ReportesPage` con pestañas por tipo de reporte (Ventas, Inventario, Órdenes de Trabajo, Servicios de Campo, Caja), cada una con sus propios filtros de fecha/estado/técnico y tarjetas de totales.
  - Pruebas unitarias: 7 nuevas en Application (no aplica Domain — el módulo no tiene agregados propios). 235/235 en todo el backend.

**Bug real detectado en la verificación E2E, con alcance más amplio que este módulo:** los parámetros `desde`/`hasta` (`DateTime?` enlazados desde query string) llegan con `DateTimeKind.Unspecified`, pero Npgsql exige `DateTimeKind.Utc` para comparar contra columnas `timestamptz` — cualquier filtro de fecha lanzaba una excepción no controlada (500) antes de llegar al handler. Corregido en `ReportesController` normalizando explícitamente a UTC antes de construir cada Query. **El mismo bug ya existía y sigue sin corregirse en el endpoint de Kardex** (`GET /productos/{id}/movimientos?desde=&hasta=`, Inventario) y probablemente en `GET /caja/movimientos?desde=&hasta=` (Caja) — no se corrigieron aquí por quedar fuera del alcance de este módulo; queda pendiente decidir si se corrigen en una pasada de mantenimiento.

Verificado end-to-end contra PostgreSQL real: los 5 reportes devuelven datos correctos sobre las entidades creadas en las verificaciones E2E de Ventas/Taller/Servicios de Campo/Caja (monto total de ventas excluyendo la anulada, cero productos en quiebre, orden de trabajo filtrada por estado `Entregado`, servicio de campo filtrado por técnico inexistente devolviendo lista vacía, totales de caja correctos); filtros de fecha verificados tras la corrección del bug de `DateTimeKind`. Permiso `Reportes.Consultar` otorgado al Administrador vía `PUT /roles/{id}/permisos`.

Estado del proyecto:

🔵 Fase de Desarrollo (Fase 4) en curso — Autenticación, Usuarios, Roles/Permisos, Catálogos, Clientes, Proveedores, Productos, Inventario (Kardex/Ajuste), Compras, Caja, Taller, Servicios de Campo, Ventas y Reportes completos (backend + frontend), verificados end-to-end contra PostgreSQL real. Siguiente módulo: Configuración.

---

## [0.17.0] - 20/07/2026

### Agregado

- **Módulo de Ventas (UC-14, UC-16, UC-17), backend + frontend (RF-038 a RF-043, RF-089, RF-091):**
  - `Domain`: `Venta` (aggregate root) con `VentaDetalle` y `PagoVenta` (hijos 1:muchos, permiten combinar varios medios de pago en una misma venta — RF-042). El registro es atómico: valida y descuenta stock, calcula el total, y determina el estado según haya o no saldo pendiente, todo en un único `POST /ventas` (no hay paso de "confirmar" separado, tal como ya lo definía `API-Design.md`). `Emitida` existe en el catálogo de estados confirmado pero no se persiste como paso real en V1 (no hay integración SUNAT, RF-044/UC-15 diferido).
  - `MovimientoInventario.CrearVenta`/`CrearDevolucion` reutilizan el patrón de referencia genérica del Kardex (ADR-012) con `OrigenTipo="Venta"` — los tipos `Venta` y `Devolucion` y el `origen_tipo` ya estaban previstos en el `CHECK` desde el módulo de Inventario, así que no hizo falta ninguna migración para eso, ni tampoco para `AccionPermiso` (`Anular` ya existía de un módulo anterior).
  - **Decisiones de alcance tomadas por continuidad, no por invención de reglas:** (1) el saldo pendiente (RF-089, RN-031) se resuelve con columnas inline en `ventas` (`saldo_pendiente`, `usuario_autorizo_saldo_id`), igual que Taller y Servicios de Campo — el módulo "Cobranzas" que el diseño original modela como tabla separada (`saldos_pendientes`) nunca se construyó y no está en el orden de módulos acordado; (2) la ventana de tiempo para anular una venta no está confirmada (BQ-013 abierta) — se implementó el permiso `Ventas.Anular` como único control de acceso, sin inventar un plazo de horas, siguiendo la recomendación técnica documentada (RN-010); (3) vincular una venta a una OT o Servicio de Campo para combinar materiales y mano de obra en un solo comprobante (RF-083, sección agregada después) queda fuera de alcance — el campo `origen` existe en el modelo con el catálogo completo (`Directa`/`OrdenTrabajo`/`ServicioCampo`) pero solo `Directa` es alcanzable por los comandos actuales, mismo patrón usado para estados inalcanzables en Taller y Servicios de Campo.
  - `Application`: `RegistrarVentaCommand` (RN-003: valida stock antes de confirmar; agrega cantidades por producto entre líneas repetidas antes de validar), `AnularVentaCommand` (RN-010: motivo obligatorio, revierte el Kardex vía `CrearDevolucion` por cada línea), `RegistrarDevolucionCommand` (RN-032: valida que el producto realmente forme parte de la venta original), `BuscarVentasQuery`/`ObtenerVentaQuery`.
  - `API`: `VentasController` con los 5 endpoints ya diseñados en Fase 3 (`GET /ventas`, `POST /ventas`, `GET /ventas/{id}`, `POST /ventas/{id}/anular`, `POST /ventas/{id}/devoluciones`).
  - Frontend: `VentasPage` (listado + nueva venta con líneas de producto y pago dinámicas) y `VentaDetalleDialog` (formularios de devolución y anulación).
  - Pruebas unitarias: 19 Domain + 13 Application = 32 nuevas (228/228 en todo el backend).
  - **Corrección de documentación detectada, sin impacto en el código:** RF-039 y el modelo físico citan "CAT-004" como el catálogo de tipos de comprobante, pero ese identificador ya está asignado a un catálogo no relacionado (Tipos de Servicio de Campo) en `Business-Catalogs.md`. Se implementó `tipo_comprobante` como un `CHECK` cerrado (no como catálogo editable), que es como el propio modelo físico ya lo tenía declarado independientemente de esa referencia cruzada.

Verificado end-to-end contra PostgreSQL real: registro de venta con pago completo (stock descontado 26→24, Kardex con `Venta` y `origenId` vinculado a la venta), rechazo por stock insuficiente (400), venta con saldo pendiente autorizado (saldo calculado correctamente, estado `Registrada`), rechazo de saldo pendiente sin usuario autorizante (400 `VALIDACION_FALLIDA`), devolución con rechazo de producto no vendido en esa venta (400 `PRODUCTO_NO_VENDIDO`) y devolución válida (stock repuesto, Kardex con `Devolucion`), anulación con reversión de inventario (stock repuesto, Kardex con `Devolucion` vinculado a la venta anulada) y rechazo de una segunda anulación sobre la misma venta (409 `ESTADO_VENTA_INVALIDO`); búsqueda por estado; 404 para venta inexistente. Permisos `Ventas.*` otorgados al Administrador vía `PUT /roles/{id}/permisos`.

Estado del proyecto:

🔵 Fase de Desarrollo (Fase 4) en curso — Autenticación, Usuarios, Roles/Permisos, Catálogos, Clientes, Proveedores, Productos, Inventario (Kardex/Ajuste), Compras, Caja, Taller, Servicios de Campo y Ventas completos (backend + frontend), verificados end-to-end contra PostgreSQL real. Siguiente módulo: Reportes.

---

## [0.16.0] - 20/07/2026

### Agregado

- **Módulo de Servicios de Campo (UC-30 a UC-33), backend + frontend (RF-064 a RF-070) — más simple que Taller: sin máquina de estados de 9 valores ni flujo formal de aprobación de cotización:**
  - `Domain`: `ServicioCampo` (aggregate root) con `ServicioCampoDetalle` (hijos 1:muchos, mismo patrón que `ConsumoRepuesto`). `Cotizar` es una acción de solo captura de dato (`MontoEstimado`), sin transición de estado — el catálogo de estados confirmado (`Solicitado, Agendado, EnEjecucion, Cerrado`) no tiene un estado `Cotizado` ni un endpoint de decisión del cliente separado, a diferencia de Taller. `Cerrar` transiciona directo de `Solicitado` a `Cerrado`, saltando `Agendado`/`EnEjecucion` como pasos persistidos reales — esos dos valores existen en el enum solo para coincidir con el catálogo ya confirmado en el `CHECK` constraint.
  - **Completa cinco campos que faltaban en el modelo físico original:** `servicios_campo` solo tenía `id, cliente_id, descripcion_trabajo, fecha_solicitud, tecnico_asignado_id, fecha_ejecucion, estado_final, observaciones, usuario_cierre_id, estado`; se agregaron `monto_estimado` (RF-068), `medio_pago_id`/`monto_pagado`/`saldo_pendiente`/`usuario_autorizo_saldo_id` (RF-070/RN-031) como columnas simples — mismo criterio ya aplicado dos veces en Taller.
  - `MovimientoInventario.CrearConsumoCampo` reutiliza el patrón de referencia genérica del Kardex (ADR-012) con `OrigenTipo="ServicioCampo"`, ya previsto en el `CHECK` de `origen_tipo` desde Inventario (junto con `Compra`/`Venta`/`OrdenTrabajo`). El consumo reutiliza `Producto.AjustarStock` sin necesitar un método nuevo.
  - **Deliberadamente fuera de alcance:** `participaciones_temporales` (tabla compartida entre Taller y Servicios de Campo para referencias opcionales a personal temporal, RF-090) — tampoco se construyó para Taller y el UC la marca explícitamente "(Opcional)". Tampoco se generó un movimiento de Caja automático al cobrar — mismo criterio que Compras y Taller: el puente "cobro → Caja" pertenece a un mecanismo unificado (Cobranzas) aún no construido.
  - Se agregó 1 acción de permiso nueva (`Cobrar` en `AccionPermiso`) — `Crear`, `Consultar`, `Cotizar` y `Cerrar` ya existían de módulos anteriores. Misma migración de ampliación del `CHECK` de `permisos`.
  - `Application`: `SolicitarServicioCampoCommand`, `CotizarServicioCampoCommand`, `CerrarServicioCampoCommand` (RN-020: descuenta materiales consumidos del inventario compartido, valida stock antes de descontar), `CobrarServicioCampoCommand` (RN-031: exige usuario Administrador/Propietario autorizante si hay saldo pendiente; reutiliza `IMedioPagoRepository` del módulo de Configuración), `BuscarServiciosCampoQuery`/`ObtenerServicioCampoQuery`.
  - `API`: `ServiciosCampoController` con los 6 endpoints diseñados en Fase 3 (incluye `GET /servicios-campo/{id}`, no explícito en el diseño original pero necesario para el detalle del frontend, mismo criterio que `GET /caja`).
  - Frontend: `ServiciosCampoPage` (listado + nueva solicitud) y `ServicioCampoDetalleDialog` (formularios contextuales de cotizar/cerrar/cobrar según el estado del servicio).
  - Pruebas unitarias: 11 Domain + 22 Application = 33 nuevas (196/196 en todo el backend).
  - Se aplicó proactivamente, desde el primer intento, la corrección de tracking de entidades `Added` de EF Core (documentada por primera vez en `[0.7.0]` y reencontrada tres veces en Taller, ver `[0.15.0]`) en `CerrarServicioCampoCommandHandler`: `IServicioCampoRepository.AgregarDetalles` registra explícitamente los `ServicioCampoDetalle` nuevos vía `DbSet.AddRange` antes de guardar cambios, evitando que reapareciera aquí.

Verificado end-to-end contra PostgreSQL real: ciclo completo solicitud → cotización (sin cambio de estado) → cierre con consumo de materiales (stock descontado 28→26, Kardex con `ConsumoCampo` y `origenId` vinculado al servicio; rechazo por stock insuficiente) → cobro (rechazo de saldo pendiente sin usuario autorizante, cobro con saldo pendiente y usuario autorizante válido); guardas de estado inválido en cierre y cotización repetidos sobre un servicio ya cerrado (409); búsqueda por estado; 404 para servicio inexistente. Permisos `ServiciosCampo.*` otorgados al Administrador vía `PUT /roles/{id}/permisos`.

Estado del proyecto:

🔵 Fase de Desarrollo (Fase 4) en curso — Autenticación, Usuarios, Roles/Permisos, Catálogos, Clientes, Proveedores, Productos, Inventario (Kardex/Ajuste), Compras, Caja, Taller y Servicios de Campo completos (backend + frontend), verificados end-to-end contra PostgreSQL real. Siguiente módulo: Ventas.

---

## [0.15.0] - 20/07/2026

### Agregado

- **Módulo de Taller (UC-22 a UC-28), backend + frontend — el más grande hasta ahora (RF-051 a RF-063):** ciclo completo de recepción de equipo → diagnóstico → cotización → decisión del cliente → reparación → entrega → garantía, sobre una máquina de estados de 9 valores.
  - `Domain`: `OrdenTrabajo` (aggregate root) con `Diagnostico`/`CotizacionReparacion` (hijos 1:1) y `ConsumosRepuesto` (hijos 1:muchos); `Garantia` como aggregate separado (referencia a la OT que la generó). `RegistrarReparacion` combina consumo de repuestos y resultado de pruebas en un solo paso, transicionando directo de `Aprobado` a `ListoParaEntrega` — coincide con que `API-Design.md` ya definía un único endpoint para todo el paso (`EnReparacion`/`EnPruebas` quedan en el catálogo confirmado de estados pero no se persisten como pasos intermedios reales).
  - **Completa dos campos que faltaban en el modelo físico original:** `ordenes_trabajo` solo tenía `estado_pago`, pero RF-059 exige capturar también monto pagado, saldo pendiente y el usuario Administrador/Propietario que autorizó (RN-001/RN-031) — se agregaron como columnas simples. También se agregó `resultado_pruebas` (RF-058), sin columna asignada en el diseño original.
  - `MovimientoInventario.CrearConsumoTaller` reutiliza el mismo patrón de referencia genérica del Kardex (ADR-012) con `OrigenTipo="OrdenTrabajo"`, ya previsto en el `CHECK` de `origen_tipo` desde Fase 3. El consumo reutiliza `Producto.AjustarStock` (cantidad negativa) sin necesitar un método nuevo.
  - **Deliberadamente fuera de alcance:** vincular una OT de reingreso a una garantía vigente (RF-062) — no tiene endpoint propio definido en `API-Design.md` y cruza dos OTs; la columna `orden_trabajo_reingreso_id` existe (su FK sí se pudo declarar, a diferencia de las de `movimientos_caja`, porque `ordenes_trabajo` existe en esta misma migración) pero ningún comando la puebla todavía. Tampoco se generó un movimiento de Caja automático al entregar el equipo — mismo criterio que con Compras: el puente "cobro → Caja" pertenece a un mecanismo unificado (Cobranzas) aún no construido.
  - Se agregaron 5 acciones de permiso nuevas (`Recepcionar`, `Diagnosticar`, `Cotizar`, `Reparar`, `Entregar` en `AccionPermiso`), ya previstas en `API-Design.md` (`Taller.Recepcionar`/`Taller.Diagnosticar`/`Taller.Cotizar`/`Taller.Reparar`/`Taller.Entregar`; `Taller.Editar` y `Taller.Consultar` ya existían) — misma migración de ampliación del `CHECK` de `permisos` que con `Ajustar` y `Abrir/Cerrar/Registrar`.
  - `Application`: `RegistrarRecepcionCommand`, `RegistrarDiagnosticoCommand`, `GenerarCotizacionReparacionCommand`, `RegistrarDecisionClienteCommand` (RN-016/RN-030), `RegistrarReparacionCommand` (RN-018, valida stock antes de descontar), `EntregarEquipoCommand` (RN-001: el pago no bloquea la entrega; exige usuario autorizante si hay saldo pendiente), `RegistrarGarantiaCommand` (RN-017, alcance V1 acotado), `BuscarOrdenesTrabajoQuery`/`ObtenerOrdenTrabajoQuery` (satisface UC-28, historial por cliente).
  - `API`: `OrdenesTrabajoController` con los 9 endpoints ya diseñados en Fase 3.
  - Frontend: `TallerPage` (listado + nueva recepción) y `OrdenTrabajoDetalleDialog` (un solo diálogo que muestra el formulario contextual correspondiente al estado actual de la OT).
  - Pruebas unitarias: 105 Domain + 61 Application = 166/166 exitosas.

**Bug real detectado solo en la verificación E2E** (no por las 166 pruebas unitarias, que usan repositorios mockeados sin tracking real de EF Core): el mismo problema de tracking de entidades `Added` documentado para `Rol`/`Permiso` (ver `[0.7.0]`) reapareció en **tres** lugares de este módulo — `RegistrarDiagnostico` y `GenerarCotizacion` (navegación de referencia 1:1 sobre una OT ya rastreada) y `RegistrarReparacion`'s `ConsumosRepuesto` (colección sobre una OT ya rastreada) — porque ninguno de los tres pasa por `repository.Agregar` sobre un agregado nuevo (a diferencia de `RegistrarRecepcionCommandHandler`, que sí crea una OT nueva y no sufre el problema). Corregido agregando `IOrdenTrabajoRepository.AgregarDiagnostico`/`AgregarCotizacion`/`AgregarConsumosRepuesto`, que registran explícitamente los hijos nuevos vía `DbSet.Add`/`AddRange` antes de guardar cambios.

Verificado end-to-end contra PostgreSQL real: ciclo completo recepción → diagnóstico → cotización → aprobación → reparación (stock descontado 30→28, Kardex con `ConsumoTaller` y `origenId` vinculado a la OT) → entrega (rechazo de saldo pendiente sin usuario autorizante, entrega con pago completo) → garantía (y rechazo de garantía duplicada); rama alternativa de cotización rechazada con cobro opcional por diagnóstico; guardas de estado inválido en cada transición (409); búsqueda por estado; 404 para OT inexistente. Permisos `Taller.*` otorgados al Administrador vía `PUT /roles/{id}/permisos`.

Estado del proyecto:

🔵 Fase de Desarrollo (Fase 4) en curso — Autenticación, Usuarios, Roles/Permisos, Catálogos, Clientes, Proveedores, Productos, Inventario (Kardex/Ajuste), Compras, Caja y Taller completos (backend + frontend), verificados end-to-end contra PostgreSQL real. Siguiente módulo: Servicios de Campo.

---

## [0.14.0] - 20/07/2026

### Agregado

- **Módulo de Caja (UC-19, UC-20), backend + frontend (RF-046 a RF-049):**
  - `Domain`: `Caja` (apertura/cierre, RN-025: solo una caja `Abierta` a la vez) y `MovimientoCaja`. `ConceptoMovimientoCaja` (GastoOperativo, RetiroPropietario, AporteCapital) implementa la parte estructural de RN-026 — el tipo (Ingreso/Egreso) se **deriva** del concepto, nunca se captura por separado. `Caja.Cerrar` calcula la diferencia (monto físico - monto teórico) para RN-015; el mecanismo de resolución de un descuadre sigue sin definirse (BQ-031) y no se implementó ninguno.
  - **Alcance reducido respecto al diseño original de Fase 3:** `movimientos_caja` no incluye las FK opcionales a `ventas`/`ordenes_trabajo`/`servicios_campo`/`saldos_pendientes` (Architecture-Overview.md §7.2) porque esos módulos no existen todavía — se agregarán cuando cada uno se construya, mismo patrón que `MovimientoInventario.origen_id` con Compras. Solo se implementó el registro **manual** (`RegistrarMovimientoCajaCommand`), que es lo que el propio `API-Design.md` ya describía como "egreso manual/gasto".
  - Se agregaron 3 acciones de permiso nuevas (`Abrir`, `Cerrar`, `Registrar` en `AccionPermiso`), ya previstas en `API-Design.md` (`Caja.Abrir`/`Caja.Cerrar`/`Caja.Registrar`/`Caja.Consultar`) — misma migración de ampliación del `CHECK` de `permisos` que con `Ajustar`.
  - `Application`: `AbrirCajaCommand` (rechaza si ya hay una caja abierta), `CerrarCajaCommand` (calcula monto teórico = apertura + ingresos - egresos), `RegistrarMovimientoCajaCommand` (rechaza si no hay caja abierta — RN-014), `ObtenerCajaActualQuery` (la abierta o, si no hay, la más reciente cerrada), `ListarMovimientosCajaQuery`.
  - `API`: `CajaController`. `GET /caja` completa una brecha del diseño original (no había forma de consultar el estado antes de actuar).
  - Frontend: `CajaPage` con apertura/cierre/registro de movimientos y resumen de conciliación al cerrar.
  - Pruebas unitarias: 83 Domain + 51 Application = 134/134 exitosas.

Verificado end-to-end contra PostgreSQL real: apertura (201), segunda apertura rechazada (409 `CAJA_YA_ABIERTA`, RN-025), registro de GastoOperativo derivando Egreso y de AporteCapital derivando Ingreso, monto cero rechazado (400), cierre con cálculo correcto del monto teórico (200 apertura + 500 ingresos - 30 egresos = 670) y diferencia (660 físico - 670 teórico = -10), movimiento/cierre rechazados tras cerrar (409 `CAJA_NO_ABIERTA`, RN-014), reapertura tras cierre (201). Permisos `Caja.*` otorgados al Administrador vía `PUT /roles/{id}/permisos`.

**Bug real detectado solo en la verificación E2E** (no por las 134 pruebas unitarias, que llaman al handler directamente sin pasar por serialización JSON real): `RegistrarMovimientoCajaRequest.Concepto` estaba tipado como el enum `ConceptoMovimientoCaja` directamente, y System.Text.Json espera enums como número, no como string, por lo que cualquier llamada real fallaba con un 400 de deserialización antes de llegar al controller. Corregido tipando el campo como `string` y usando `Enum.Parse` en el controller — el mismo patrón ya usado en Roles/Permisos, que ningún módulo anterior había roto porque ninguno exponía un enum crudo directamente en un DTO de request.

Estado del proyecto:

🔵 Fase de Desarrollo (Fase 4) en curso — Autenticación, Usuarios, Roles/Permisos, Catálogos, Clientes, Proveedores, Productos, Inventario (Kardex/Ajuste), Compras y Caja completos (backend + frontend), verificados end-to-end contra PostgreSQL real. Siguiente módulo: Taller.

---

## [0.13.0] - 19/07/2026

### Agregado

- **Módulo de Compras (UC-13), backend + frontend (RF-033 a RF-036):** compra directa ya realizada, sin orden de compra ni aprobación previa (RN-024, confirmada). Sin edición ni anulación en este alcance — corregir una compra requeriría revertir Kardex y costo promedio, fuera de RF-033 a RF-036.
  - **RN-013 (costo promedio ponderado) sigue `[PV]`** — recomendación técnica ya documentada y justificada (ver `[0.11.0]`/Business-Rules.md), pendiente de validación formal de un contador antes de producción. Se implementa la fórmula recomendada ahora porque el propio análisis dice explícitamente que "no bloquea el desarrollo"; queda registrado aquí como pendiente de confirmación, no como decisión cerrada.
  - `Domain`: `Compra` (aggregate root) y `CompraDetalle` (entidad hija, mismo patrón que `Rol`/`Permiso`). `Producto.RegistrarCompra` aumenta el stock y recalcula `CostoReferencia` con la fórmula de costo promedio ponderado: `(stockActual*costoActual + cantidad*costoUnitario) / (stockActual+cantidad)`. `MovimientoInventario.CrearCompra` genera el movimiento de entrada con `OrigenTipo="Compra"`/`OrigenId=compra.Id`, completando el patrón de referencia genérica del Kardex (ADR-012) con su primer consumidor real (hasta ahora solo existía `CrearAjuste`).
  - `Application`: `RegistrarCompraCommand` (valida proveedor y cada producto, aplica el costo promedio y genera el Kardex por cada línea de detalle), `BuscarComprasQuery` (filtro por proveedor y/o producto, RF-036), `ObtenerCompraQuery`.
  - `Infrastructure`: `CompraRepository`. Migración `Compras` (tablas `compras` y `compra_detalle`, exactamente como se diseñaron en `Physical-Data-Model.md` desde Fase 3).
  - `API`: `ComprasController` (`Compras.Consultar`, `Compras.Crear` — sin `Editar`/`Eliminar`, no están en el alcance). El usuario que registra la compra se extrae del claim `ClaimTypes.NameIdentifier` del JWT, mismo patrón introducido en el módulo de Inventario.
  - Frontend: `ComprasPage` (listado, detalle de línea de compra) y `RegistrarCompraDialog` (formulario con lista dinámica de productos vía `useFieldArray` de React Hook Form — primer uso de este patrón en el proyecto, para líneas de detalle repetibles).
  - Pruebas unitarias: 74 Domain + 44 Application = 118/118 exitosas.

Verificado end-to-end contra PostgreSQL real: registro de compra con recálculo correcto de costo promedio ponderado (stock 20→30 a costo 260 con compra de 10 a costo 300 → costo resultante 273.33, verificado manualmente), Kardex con `origenId` correctamente vinculado a la compra, sin detalles (400), proveedor inexistente (404), producto inexistente (404), cantidad cero en detalle (400), búsqueda por proveedor y por producto, obtención por id y 404 para compra inexistente. Permisos `Compras.Consultar`/`Compras.Crear` otorgados al Administrador vía `PUT /roles/{id}/permisos` (esta vez sin necesitar ampliar ningún `CHECK`, a diferencia del módulo anterior).

Estado del proyecto:

🔵 Fase de Desarrollo (Fase 4) en curso — Autenticación, Usuarios, Roles/Permisos, Catálogos, Clientes, Proveedores, Productos, Inventario (Kardex/Ajuste) y Compras completos (backend + frontend), verificados end-to-end contra PostgreSQL real. Siguiente módulo: Caja.

---

## [0.12.0] - 19/07/2026

### Agregado

- **Módulo de Inventario (Kardex y Ajuste, UC-11/UC-12), backend + frontend (RF-027, RF-030):**
  - `Domain`: `MovimientoInventario` (Kardex, ADR-012/Architecture-Overview.md §7.3 — solo `ProductoId` es FK fuerte; `OrigenTipo`/`OrigenId` son informativos, sin integridad declarativa, porque el Kardex es en esencia un log histórico). `TipoMovimientoInventario` (CAT-013: Compra, Venta, ConsumoTaller, ConsumoCampo, Ajuste, Devolucion — catálogo completo ya confirmado, aunque por ahora solo `CrearAjuste` tiene fábrica propia). Nuevo método `Producto.AjustarStock`, único mecanismo autorizado para modificar `StockActual` fuera del registro inicial (RN-008).
  - Nueva acción de permiso `Ajustar` (`AccionPermiso`), además de Crear/Editar/Eliminar/Consultar/Anular — requirió ampliar el `CHECK` de la tabla `permisos` (migración `AmpliarCheckAccionPermisoConAjustar`) y agregar `'Ajustar'` a la lista `ACCIONES` de la UI de Roles.
  - **Alcance deliberado:** este módulo solo implementa el ajuste manual (UC-12, exclusivo del Administrador) y la consulta del Kardex por producto. El registro automático de movimientos por Venta/Compra/ConsumoTaller/ConsumoCampo/Devolución (RF-027 en su forma completa) lo disparará cada módulo futuro correspondiente (Compras, Ventas, Taller, Servicios de Campo) reutilizando esta misma entidad — no existe todavía un consumidor real para esos tipos de movimiento, así que no se construyó lógica especulativa para ellos.
  - `Application`: `AjustarInventarioCommand` (motivo obligatorio, rechaza si el resultado deja el stock negativo — `AjusteInventarioInvalidoException`), `ConsultarKardexQuery` (historial por producto, filtro opcional de fechas).
  - `Infrastructure`: `MovimientoInventarioRepository`. Migraciones `MovimientosInventario` (tabla con los `CHECK` de `tipo_movimiento` y `origen_tipo` documentados desde Fase 3) y `AmpliarCheckAccionPermisoConAjustar`.
  - `API`: `POST /productos/{id}/ajustes` (`Inventario.Ajustar`) y `GET /productos/{id}/movimientos` (`Inventario.Consultar`, ya cubierto por el permiso existente) agregados a `ProductosController`. El usuario que ejecuta el ajuste se extrae del claim `ClaimTypes.NameIdentifier` del JWT (patrón nuevo en este controlador, no existía antes en ningún otro).
  - Frontend: `AjustarInventarioDialog` y `KardexDialog` en `ProductosPage`, con botones "Ajustar" y "Kardex" por fila.
  - Pruebas unitarias: 60 Domain + 40 Application = 100/100 exitosas.

Verificado end-to-end contra PostgreSQL real: 403 antes de otorgar `Inventario.Ajustar`, ajuste válido (stock 15→20, movimiento registrado con usuario y fecha correctos), motivo vacío (400), cantidad cero (400), ajuste que dejaría stock negativo (400 `AJUSTE_INVENTARIO_INVALIDO`), producto inexistente en ajuste y en Kardex (404 en ambos). Permiso `Inventario.Ajustar` otorgado al Administrador vía `PUT /roles/{id}/permisos` tras ampliar el `CHECK` de la tabla `permisos` (un bloqueo real detectado solo en esta verificación E2E, no por las 100 pruebas unitarias).

Estado del proyecto:

🔵 Fase de Desarrollo (Fase 4) en curso — Autenticación, Usuarios, Roles/Permisos, Catálogos, Clientes, Proveedores, Productos e Inventario (Kardex/Ajuste) completos (backend + frontend), verificados end-to-end contra PostgreSQL real. Siguiente módulo: Compras.

---

## [0.11.0] - 19/07/2026

### Agregado

- **Resolución de BQ-005 y BQ-008 (previas al módulo de Productos):**
  - **BQ-008 (unidad de medida, RESUELTA):** la unidad de medida se maneja como catálogo configurable (misma mecánica que Categoría/Medio de Pago — RN-028), ampliable por el Administrador sin migración. Semilla inicial (CAT-014): Unidad, Metro, Kilogramo, Litro, Rollo, Par, Juego. La conversión entre unidades queda explícitamente fuera de alcance en V1 (RN-040 nueva).
  - **BQ-005 (stock mínimo, PARCIALMENTE RESUELTA):** se agrega `stock_minimo` como campo numérico opcional en Producto, sin obligatoriedad. La lógica de alertas automáticas de reposición (RF-031) **no** se implementa aún — depende del módulo de Inventario/Kardex (aún no construido) y de un mecanismo de notificaciones inexistente (RN-041 nueva). El feature de alertas en sí sigue pendiente de confirmación con el negocio.
- **Módulo de Productos (UC-10), backend + frontend (RF-023 a RF-026):**
  - `Domain`: `Producto` (`Inventario`) — `CodigoInterno` obligatorio único, `CodigoBarras` opcional único, `Nombre`, `CategoriaId`/`UnidadMedidaId` (FK), `Marca` opcional, `CostoReferencia`/`PrecioVenta` (`Margen` calculado, no persistido), `StockActual` (inicializado al registrar), `StockMinimo` opcional, `Estado` (baja lógica, RN-023). `UnidadMedida` (catálogo configurable análogo a `Categoria`).
  - **Decisión de diseño importante:** editar un producto (RF-024) nunca modifica `StockActual` — el stock solo cambia mediante el mecanismo de ajuste/Kardex exclusivo del Administrador (RN-008, UC-12), que se construirá en el próximo módulo (Inventario). `ActualizarDatos` lo deja intacto deliberadamente.
  - `Application`: `RegistrarProductoCommand`, `EditarProductoCommand`, `CambiarEstadoProductoCommand` (baja lógica), `BuscarProductosQuery` (por nombre/código interno/código de barras, con filtro opcional de categoría — ya previsto en `API-Design.md` desde la fase de diseño). `CrearUnidadMedidaCommand`/`EditarUnidadMedidaCommand`/`ListarUnidadesMedidaQuery` análogos a Categoría.
  - `Infrastructure`: `ProductoRepository`, `UnidadMedidaRepository`. Migración `ProductosYUnidadesMedida` (tablas `productos` y `unidades_medida`, semilla de las 7 unidades confirmadas, índices únicos en `codigo_interno`/`codigo_barras` filtrado/`nombre`).
  - `API`: `ProductosController` y `UnidadesMedidaController`. Siguiendo el diseño ya documentado en `API-Design.md` desde Fase 3, los permisos de Producto son `Inventario.Crear`/`Inventario.Editar`/`Inventario.Eliminar`/`Inventario.Consultar` (no un módulo `Productos.*` nuevo) — consistente con que Producto es la entidad principal del contexto "Inventario" ya usado por Categoría; UnidadMedida reutiliza `Inventario.Consultar`/`Configuracion.Editar` igual que Categoría.
  - Frontend: `ProductosPage` (búsqueda, crear/editar, activar/desactivar), enlazada desde el menú principal junto a Clientes/Proveedores. `UnidadesMedidaPage` bajo Configuración, junto a Categorías/Medios de Pago. Se usaron diálogos de creación y edición **separados** (`CrearProductoDialog`/`EditarProductoDialog`) porque sus formularios difieren genuinamente (`stockInicial` solo existe al crear) — mismo patrón ya usado en Usuarios.
  - Pruebas unitarias: 55 Domain + 36 Application = 91/91 exitosas.

Verificado end-to-end contra PostgreSQL real: registrar producto válido (margen calculado correctamente), validación de código interno obligatorio (400), código interno duplicado (409), categoría inexistente (400), búsqueda por nombre y por categoría, editar (confirma que el stock no se toca), baja lógica, crear/editar unidad de medida y rechazo de nombre duplicado (409). Permisos `Inventario.Crear/Editar/Eliminar` otorgados al Administrador vía `PUT /roles/{id}/permisos` (no por migración); `Inventario.Consultar` ya existía desde Catálogos y cubrió las lecturas sin cambios.

Estado del proyecto:

🔵 Fase de Desarrollo (Fase 4) en curso — Autenticación, Usuarios, Roles/Permisos, Catálogos, Clientes, Proveedores y Productos completos (backend + frontend), verificados end-to-end contra PostgreSQL real. Siguiente módulo: Inventario.

---

## [0.10.0] - 19/07/2026

### Agregado

- **Módulo de Proveedores (UC-09), backend + frontend:** análogo a Clientes pero sin distinción Natural/Jurídica ni validación de formato de documento (RF-018 a RF-022, todos `[I]`, sin preguntas de negocio pendientes a diferencia de Clientes/BQ-074).
  - `Domain`: `Proveedor` (`Terceros`) — solo `NombreRazonSocial` obligatorio; `Documento`/`Telefono`/`Direccion` opcionales, texto libre sin validación de formato.
  - `Application`: `RegistrarProveedorCommand`, `EditarProveedorCommand`, `CambiarEstadoProveedorCommand` (baja lógica), `BuscarProveedoresQuery` (por nombre o documento).
  - `Infrastructure`: `ProveedorRepository`. Migración de la tabla `proveedores`.
  - `API`: `ProveedoresController`, permisos `Proveedores.*` asignados al Administrador vía el propio endpoint de Roles.
  - Frontend: `ProveedoresPage`, enlazada desde el menú principal junto a Clientes.
  - **Alcance:** `GET /proveedores/{id}/historial` (RF-022) queda fuera de este módulo — sin Compras todavía, no hay datos reales que mostrar.
  - Pruebas unitarias: 72/72 exitosas.

Verificado end-to-end contra PostgreSQL real: registrar (con y sin datos opcionales), validación de nombre obligatorio (400), buscar por nombre, editar, baja lógica.

Estado del proyecto:

🔵 Fase de Desarrollo (Fase 4) en curso — Autenticación, Usuarios, Roles/Permisos, Catálogos, Clientes y Proveedores completos (backend + frontend), verificados end-to-end contra PostgreSQL real. Siguiente módulo: Productos.

---

## [0.9.0] - 19/07/2026

### Agregado

- **Resolución de BQ-074 (previa al módulo de Clientes):** el teléfono es obligatorio al registrar un cliente; documento de identidad y dirección quedan opcionales — confirmado explícitamente antes de programar (RN-039 nueva en `Business-Rules.md`). El documento no se exige al registrar el cliente porque su obligatoriedad real se gobierna a nivel del comprobante (RN-009: la Factura exige RUC), no al crear el registro.
- **Módulo de Clientes (UC-05), backend + frontend:**
  - `Domain`: `Cliente` (`Terceros`), `TipoDocumento`/`TipoCliente` (enums fijos — RN-033/RN-034), `ValidadorDocumentoIdentidad` (algoritmo oficial de dígito verificador de RUC — módulo 11, SUNAT — verificado manualmente contra el RUC público de SUNAT 20100070970; DNI de 8 dígitos). `TipoCliente` se deriva automáticamente del documento (RN-034), nunca se captura manualmente.
  - `Application`: `RegistrarClienteCommand`, `EditarClienteCommand`, `CambiarEstadoClienteCommand` (baja lógica, RF-014/RN-023), `BuscarClientesQuery` (por nombre o número de documento, UC-05 paso 3). `ListadoPaginadoDto<T>` se movió a `Application/Common` al necesitarlo ahora más de un módulo.
  - `Infrastructure`: `ClienteRepository` (búsqueda con `ILIKE`). Migración de la tabla `clientes`.
  - `API`: `ClientesController`, protegido por permisos `Clientes.*` — asignados al Administrador usando el propio endpoint de Roles, no por migración.
  - Frontend: `ClientesPage` (búsqueda, crear/editar, activar/desactivar), enlazada directamente desde el menú principal (no bajo Configuración, a diferencia de Usuarios/Roles/Catálogos) — es un módulo operativo de uso diario, no una pantalla administrativa.
  - **Alcance explícito:** `GET /clientes/{id}/historial` (RF-017) documentado en `API-Design.md` queda fuera de este módulo — sin Ventas/Taller/Servicios de Campo todavía, no hay datos reales que mostrar; se agrega cuando esos módulos existan.
  - Pruebas unitarias: 66/66 exitosas (Domain.Tests + Application.Tests), incluyendo el algoritmo de validación de RUC/DNI.

Verificado end-to-end contra PostgreSQL real: registrar cliente sin documento, registrar con RUC válido (deriva Jurídica), rechazar teléfono vacío y RUC con dígito verificador inválido (400), buscar por nombre y por documento, editar quitando el documento (limpia `tipo_cliente`), desactivar (baja lógica).

Estado del proyecto:

🔵 Fase de Desarrollo (Fase 4) en curso — Autenticación, Usuarios, Roles/Permisos, Catálogos y Clientes completos (backend + frontend), verificados end-to-end contra PostgreSQL real. Siguiente módulo: Proveedores.

---

## [0.8.0] - 19/07/2026

### Agregado

- **Módulo de Catálogos, backend + frontend:** de los 20 catálogos identificados en `Business-Catalogs.md`, se implementaron los dos únicos confirmados ([C]) y marcados como "catálogo configurable" en `Physical-Data-Model.md`: **Categorías de Producto (CAT-002, RF-025)** y **Medios de Pago (CAT-008, RF-042)**. El resto sigue `[PV]` sin sembrarse, conforme al principio ya establecido de no inventar valores de catálogo sin confirmación.
  - `Domain`: `Categoria` (con referencia opcional a categoría padre, autoreferencia con guarda anti-autopadre — esquema ya previsto para BQ-007 aunque la jerarquía no esté confirmada), `MedioPago` (activar/desactivar).
  - `Application`: `CrearCategoriaCommand`, `EditarCategoriaCommand`, `ListarCategoriasQuery`; `CrearMedioPagoCommand`, `CambiarEstadoMedioPagoCommand`, `ListarMediosPagoQuery`.
  - `Infrastructure`: `CategoriaRepository`, `MedioPagoRepository`. Migración con semilla de los 4 medios de pago confirmados (Efectivo, Yape, Plin, Transferencia bancaria — CAT-008).
  - `API`: `CategoriasController`, `MediosPagoController`. Se refinó `API-Design.md`: se separó permiso de lectura (`Configuracion.Consultar`) de escritura (`Configuracion.Editar`) para medios de pago, y se agregó `PUT /categorias/{id}` (faltaba el endpoint de edición).
  - Frontend: `CategoriasPage`, `MediosPagoPage`, enlazadas desde `ConfiguracionPage`.
  - Pruebas unitarias: 40/40 exitosas (Domain.Tests + Application.Tests).
- **Validación real del módulo de Roles y Permisos:** los permisos `Inventario.Consultar` y `Configuracion.{Consultar,Editar}` que este módulo necesitaba se asignaron al rol Administrador **usando el propio endpoint de Roles** construido en la sesión anterior (`PUT /roles/{id}/permisos`), no por migración — confirmando en la práctica que ese módulo cumple su propósito: los módulos siguientes ya no requieren sembrar permisos por código.

Estado del proyecto:

🔵 Fase de Desarrollo (Fase 4) en curso — Autenticación, Usuarios, Roles/Permisos y Catálogos completos (backend + frontend), verificados end-to-end contra PostgreSQL real. Siguiente módulo: Clientes.

---

## [0.7.0] - 19/07/2026

### Agregado

- **Módulo de Roles y Permisos (UC-04), backend + frontend:**
  - `Domain`: `Rol.ActualizarDatos`, `Rol.AsignarPermiso/QuitarPermiso/ReemplazarPermisos` como comportamiento del propio agregado.
  - `Application`: `CrearRolCommand`, `EditarRolCommand`, `AsignarPermisosCommand` (reemplaza el conjunto completo de permisos de un rol; `modulo` es texto libre — RN-028, no se fija en código, para que futuros módulos no requieran tocar el backend), `ListarRolesConPermisosQuery`. Excepciones `RolNoEncontradoException`, `NombreRolDuplicadoException`.
  - `Infrastructure`: `IRolRepository` extendido. Migración `SembrarPermisosAdministradorRoles`: siembra `Roles.Crear`/`Roles.Editar` para el Administrador (ya tenía `Roles.Consultar` desde el módulo anterior).
  - `API`: `RolesController` extendido (`POST/PUT /roles`, `GET /roles/detalle`, `PUT /roles/{id}/permisos`). Se completó una brecha en `API-Design.md`: UC-04 documentaba "crear o editar un rol" como paso 1, pero solo existía el endpoint de creación — se agregó `PUT /roles/{id}`.
  - Frontend: `RolesPage` (matriz de permisos por módulo × acción, con opción de agregar un módulo nuevo sin tocar código), `CrearRolDialog`, `EditarRolDialog`, y `ConfiguracionPage` (`/configuracion`) con enlaces a Usuarios y Roles — antes no había forma de navegar a esas pantallas sin escribir la URL a mano.
  - Pruebas unitarias: invariantes de permisos de `Rol` (Domain.Tests), `CrearRolCommandHandler`/`AsignarPermisosCommandHandler` (Application.Tests) — 29/29 exitosas.

### Corregido

- **Bug de EF Core en `AsignarPermisosCommandHandler`** (encontrado en verificación end-to-end contra PostgreSQL real, no detectado por las pruebas unitarias con repositorio simulado): al agregar permisos nuevos a un `Rol` ya cargado, EF Core generaba `UPDATE` en vez de `INSERT` y fallaba con `DbUpdateConcurrencyException`. Causa: `Permiso.Id` ya tiene un valor asignado (`Guid.NewGuid()`) antes de que EF Core lo vea, así que no puede reconocer la entidad como nueva solo por mutación de la colección de un agregado ya rastreado (a diferencia de un `Rol`/`Usuario` nuevo, agregado completo vía `Add()`, o de `UsuarioRol`, que al tener clave compuesta sin `Id` propio no sufre este problema). Corregido registrando explícitamente los permisos nuevos vía `IRolRepository.AgregarPermisos` (`DbSet.AddRange`).

Estado del proyecto:

🔵 Fase de Desarrollo (Fase 4) en curso — Autenticación, Usuarios y Roles/Permisos completos (backend + frontend), verificados end-to-end contra PostgreSQL real. Siguiente módulo: Catálogos.

---

## [0.6.0] - 19/07/2026

### Agregado

- **Frontend de Autenticación (completa el módulo 0.5.0):** `LoginPage` (React Hook Form + Zod, error genérico sin revelar el campo que falló — UX-Design.md §4.1), `useSessionStore` con persistencia en `localStorage`, `RutaProtegida`, `AppLayout` (shell mínimo con nombre de usuario y cerrar sesión), inyección automática del `Bearer token` en `httpClient`.
- **Módulo de Usuarios (UC-03), backend + frontend:**
  - `Application`: `CrearUsuarioCommand`, `EditarUsuarioCommand` (reemplaza roles, no toca `nombre_usuario`/credencial), `CambiarEstadoUsuarioCommand` (baja lógica, RF-003/RN-021), `RestablecerCredencialCommand` (RN-038), `ListarUsuariosQuery`, `ListarRolesQuery` (solo lectura, soporte al selector de roles hasta que exista el módulo Roles y Permisos). Jerarquía `ExcepcionAplicacion` que unifica el mapeo excepción → código HTTP en `GlobalExceptionHandler`, reemplazando el `try/catch` manual de `AuthController`.
  - `Domain`: `Usuario.AsignarRol/QuitarRol/ReemplazarRoles/ActualizarNombre` como comportamiento del propio agregado.
  - `Infrastructure`: `RolRepository`, `UsuarioRepository` extendido (listar paginado, obtener por id). Migración `SembrarPermisosAdministradorUsuarios`: siembra los permisos `Usuarios.{Crear,Editar,Eliminar,Consultar}` y `Roles.Consultar` para el rol Administrador — necesario porque el módulo Roles y Permisos (UC-04) todavía no existe para asignarlos desde la UI.
  - `API`: `UsuariosController`, `RolesController` (solo lectura), protegidos por permiso lógico (ADR-008).
  - Frontend: `UsuariosPage`, `CrearUsuarioDialog`, `EditarUsuarioDialog`, `SelectorRoles`, `ConfirmDialog` (reutilizable — UX-Design.md §2.4, confirmación explícita para acciones sensibles).
  - Pruebas unitarias: invariantes de roles de `Usuario` (Domain.Tests), `CrearUsuarioCommandHandler`/`EditarUsuarioCommandHandler` (Application.Tests) — 19/19 exitosas.

Verificado end-to-end contra PostgreSQL real: ciclo completo crear → editar → desactivar (bloquea login, RN-021) → reactivar → restablecer credencial → login con la nueva contraseña; límites de autorización (401 sin token, 403 sin permiso); CORS habilitado para el frontend de desarrollo.

Estado del proyecto:

🔵 Fase de Desarrollo (Fase 4) en curso — Autenticación y Usuarios completos (backend + frontend); siguiente módulo: Roles y Permisos (UC-04).

---

## [0.5.0] - 19/07/2026

### Agregado

- **Módulo de Autenticación (UC-01 Iniciar Sesión, UC-02 Cerrar Sesión), primer módulo de negocio del proyecto:**
  - `Domain`: entidades `Usuario`, `Rol`, `Permiso`, `UsuarioRol` (bounded context Identidad y Acceso), con las invariantes de bloqueo temporal (RN-037) y baja lógica (RN-021) como comportamiento del propio `Usuario`.
  - `Application`: `IniciarSesionCommand`/`Handler`/`Validator`, puertos `IUsuarioRepository`, `IGeneradorTokenJwt`, `IPasswordHasher`, `IFechaHoraProvider`, y excepciones controladas `CredencialesInvalidasException`/`CuentaBloqueadaException`.
  - `Infrastructure`: configuraciones EF Core para las 4 entidades, `UsuarioRepository`, `GeneradorTokenJwt` (JWT con claims de permisos efectivos, ADR-008), `PasswordHasherAdapter` (PBKDF2 vía `Microsoft.Extensions.Identity.Core`, RNF-011), `FechaHoraProvider`.
  - `API`: `AuthController` (`POST /auth/login`, `POST /auth/logout`), `PermisoAuthorizationHandler`/`PermisoAuthorizationPolicyProvider` (autorización dinámica por permiso lógico, sin roles fijos — ADR-008).
  - Primera migración EF Core (`InicialIdentidadAcceso`): tablas `usuarios`, `roles`, `permisos`, `usuario_rol`; datos semilla de los 3 roles reales (CAT-005) y de un usuario Administrador de arranque (ver `README.md` — contraseña temporal a cambiar).
  - Pruebas unitarias: invariantes de `Usuario` (Domain.Tests) y `IniciarSesionCommandHandler` (Application.Tests) — éxito, credenciales inválidas, usuario inactivo, usuario bloqueado, intento fallido.
- **Documentación:** resolución de BQ-047/BQ-048 con recomendaciones técnicas documentadas (RN-036 a RN-038), detectadas como brecha real entre `API-Design.md` y el esquema físico antes de implementar este módulo.

Estado del proyecto:

🔵 Fase de Desarrollo (Fase 4) en curso — módulo de Autenticación implementado; siguiente módulo: Usuarios (CRUD, UC-03).

---

## [0.4.0] - 19/07/2026

### Agregado

- Estructura profesional completa del proyecto (Fase 4, primer entregable — sin funcionalidades de negocio todavía):
  - **Backend:** solución `ISARMIN.sln` (.NET 9) con Clean Architecture — `ISARMIN.Domain`, `ISARMIN.Application` (carpetas `Modulos/` por bounded context, `Common/` con `ICommandHandler`/`IQueryHandler` — ADR-011), `ISARMIN.Infrastructure` (EF Core + Npgsql + EFCore.NamingConventions, `IsarminDbContext` vacío), `ISARMIN.API` (controladores, JWT Bearer, Swagger con esquema Bearer, Serilog a consola y archivo, CORS para el frontend de desarrollo, manejo global de excepciones vía `IExceptionHandler`). Proyectos de prueba xUnit (`ISARMIN.Domain.Tests`, `ISARMIN.Application.Tests`).
  - **Frontend:** proyecto `frontend/` (React 19 + TypeScript + Vite), Tailwind CSS v4 (plugin de Vite), React Router, TanStack Query, Zustand (store de sesión/permisos), React Hook Form + Zod instalados. Estructura `src/modules/` (un directorio por módulo, espejando el backend) y `src/shared/{api,components,hooks}`.
  - `.gitignore` combinado (.NET + Node) y `.editorconfig` en la raíz del repositorio.
- Backend verificado: compila sin errores/advertencias y arranca correctamente. Frontend verificado: `npm run build` y `npm run dev` funcionan sin errores.

Estado del proyecto:

🔵 Fase de Desarrollo (Fase 4) iniciada — estructura base creada, pendiente el primer módulo (Autenticación).

---

## [0.3.0] - 19/07/2026

### Agregado

- `Architecture-Overview.md`: correcciones del propietario — Cobranzas separado de Caja como módulo propio, `MovimientoInventario` sin FKs opcionales múltiples (referencia genérica igual que Auditoría), patrón CQRS ligero para `Application`, definición formal de código interno/código de barras comercial (ADR-011, ADR-012).
- `04-Database/Physical-Data-Model.md`: modelo entidad-relación físico completo (26 tablas PostgreSQL, convenciones `snake_case`/`uuid`, catálogos configurables vs. fijos, constraints de exclusividad, índices, estrategia de migraciones) — ADR-013.
- `05-Backend-API/API-Design.md`: contratos REST por módulo (rutas, Commands/Queries, permisos requeridos, formato de error y paginación).
- `06-UI-UX/UX-Design.md`: navegación por rol, flujos de pantalla principales y componentes UI compartidos.

Estado del proyecto:

🟢 Fase de Diseño (Fase 3) completada.

---

## [0.2.0] - 18/07/2026

### Agregado

- Validación directa del negocio con el propietario de ISARMIN PERÚ S.A.C. en cuatro rondas: contexto real de la empresa, roles reales (Administrador/Propietario, Ventas, Técnico), corrección de RN-001 (pago no bloqueante en la entrega de equipos), alcance acotado de Garantías y Auditoría, resolución de 37 de 91 preguntas de negocio.
- `Use-Cases.md`: 37 Casos de Uso UML.
- `Conceptual-Data-Model.md` y `Data-Dictionary.md`: modelo conceptual de 29 entidades y diccionario de datos preliminar.
- `03-Architecture/Architecture-Overview.md`: arquitectura del sistema (capas, estructura de solución, límites de módulos, patrones transversales).
- ADR-006 a ADR-010 en `DECISIONS.md` (estructura modular, patrón de entidades transversales, autorización basada en permisos configurables, auditoría vía interceptor, almacenamiento de archivos abstraído).

Estado del proyecto:

🔵 En fase de diseño (Fase 3 del roadmap) — análisis validado con el propietario.

---

## [0.1.0] - 18/07/2026

### Agregado

- Estructura inicial del proyecto.
- Documentación del contexto.
- Instrucciones para IA.
- Alcance del proyecto.
- Roadmap.
- Stack tecnológico.
- Registro de decisiones arquitectónicas.
- Documentación de requerimientos.
- Documentación del negocio.

Estado del proyecto:

🟡 En fase de análisis.
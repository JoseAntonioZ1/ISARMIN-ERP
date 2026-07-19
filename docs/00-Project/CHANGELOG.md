# CHANGELOG.md

# Historial de Cambios

Todos los cambios importantes del proyecto serán registrados en este documento.

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
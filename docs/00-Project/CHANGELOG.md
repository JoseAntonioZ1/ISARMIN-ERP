# CHANGELOG.md

# Historial de Cambios

Todos los cambios importantes del proyecto serán registrados en este documento.

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
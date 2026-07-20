# API-Design.md — Diseño de la API REST

## 1. Propósito

Define los contratos REST expuestos por `ISARMIN.API` (capa Presentation de [Architecture-Overview.md](../03-Architecture/Architecture-Overview.md)), derivados directamente de [Use-Cases.md](../01-Requirements/Use-Cases.md) y del [Physical-Data-Model.md](../04-Database/Physical-Data-Model.md). Cada endpoint invoca un `Command` o `Query` (patrón CQRS ligero, ADR-011) y declara el **permiso lógico** que exige (ADR-008), no un rol fijo.

## 2. Convenciones generales

| Aspecto | Decisión |
|---|---|
| **Base path** | `/api/v1` — versionado en la URL (simple, suficiente para el tamaño del proyecto; no se adopta versionado por header). |
| **Formato** | `application/json`, excepto `POST /documentos` (`multipart/form-data`, adjunta archivo). |
| **Nomenclatura de recursos** | Sustantivos en plural, `kebab-case`: `/ordenes-trabajo`, `/servicios-campo`, `/saldos-pendientes`. |
| **Autenticación** | `Authorization: Bearer <token JWT>` en todos los endpoints salvo `/auth/login`. |
| **Autorización** | Cada endpoint exige un **permiso lógico** (ej. `"Inventario.Ajustar"`), resuelto por el `AuthorizationHandler` descrito en `Architecture-Overview.md` §8 — nunca un rol fijo en el atributo. |
| **Fechas** | ISO 8601 (`2026-07-19T14:30:00Z`). |
| **IDs** | `uuid` en la URL y en los cuerpos (Physical-Data-Model.md §2). |
| **Paginación** | Query params `?pagina=1&tamanoPagina=20` en todo listado; respuesta con envoltorio (ver §5). |
| **Errores** | Envoltorio estándar (ver §4); códigos HTTP semánticos, nunca `200` con un error dentro del cuerpo. |

## 3. Autenticación

| Método y ruta | Descripción | Permiso | Caso de uso |
|---|---|---|---|
| `POST /api/v1/auth/login` | Recibe `{ nombreUsuario, credencial }`, devuelve `{ token, expiraEn, permisos[] }`. | Público | UC-01 |
| `POST /api/v1/auth/logout` | Invalida la sesión activa. | Autenticado | UC-02 |

`nombreUsuario` corresponde a la columna `nombre_usuario` (RN-036, **[PV]** — recomendación técnica documentada, pendiente de validación con ISARMIN), distinta de `nombre` (nombre completo, solo para mostrar).

```json
// Response 200 — POST /auth/login
{
  "token": "eyJhbGciOi...",
  "expiraEn": "2026-07-19T22:00:00Z",
  "usuario": { "id": "...", "nombre": "..." },
  "permisos": ["Ventas.Crear", "Caja.Consultar", "..."]
}
```

```json
// Response 423 Locked — POST /auth/login (RN-037, [PV])
{
  "error": {
    "codigo": "CUENTA_BLOQUEADA_TEMPORALMENTE",
    "mensaje": "Cuenta bloqueada por intentos fallidos. Intente nuevamente después de las 22:15.",
    "detalles": null
  }
}
```

## 4. Formato estándar de error

```json
{
  "error": {
    "codigo": "STOCK_INSUFICIENTE",
    "mensaje": "El producto 'Carbón para amoladora' no tiene stock disponible.",
    "detalles": null
  }
}
```

| Código HTTP | Uso |
|---|---|
| `400` | Validación de entrada fallida (FluentValidation) — `codigo` describe el campo/regla. |
| `401` | Token ausente o inválido. |
| `403` | Usuario autenticado sin el permiso lógico requerido. |
| `404` | Recurso no encontrado. |
| `409` | Conflicto de regla de negocio (ej. `STOCK_INSUFICIENTE` — RN-007, `SALDO_PENDIENTE_SIN_AUTORIZACION` — RN-001/RN-031). |
| `423` | Cuenta bloqueada temporalmente por intentos fallidos (RN-037, **[PV]**). |
| `500` | Error no controlado (registrado por Serilog, nunca expone detalles internos al cliente). |

## 5. Formato estándar de listado paginado

```json
{
  "datos": [ /* ... */ ],
  "total": 134,
  "pagina": 1,
  "tamanoPagina": 20
}
```

## 6. Endpoints por módulo

### 6.1 Usuarios y Roles

| Método y ruta | Command/Query | Permiso | UC |
|---|---|---|---|
| `GET /usuarios` | `ListarUsuariosQuery` | `Usuarios.Consultar` | — |
| `POST /usuarios` | `CrearUsuarioCommand` | `Usuarios.Crear` | UC-03 |
| `PUT /usuarios/{id}` | `EditarUsuarioCommand` | `Usuarios.Editar` | UC-03 |
| `PATCH /usuarios/{id}/estado` | `CambiarEstadoUsuarioCommand` | `Usuarios.Eliminar` | UC-03 (baja lógica, RN-021) |
| `PATCH /usuarios/{id}/restablecer-credencial` | `RestablecerCredencialCommand` | `Usuarios.Editar` | RF-007 (RN-038, **[PV]** — exclusivo del Administrador, sin autoservicio) |
| `GET /roles` | `ListarRolesQuery` | `Roles.Consultar` | UC-04 (listado liviano id/nombre, usado por el selector de roles de Usuarios) |
| `GET /roles/detalle` | `ListarRolesConPermisosQuery` | `Roles.Consultar` | UC-04 (listado completo con permisos, para la pantalla de administración) |
| `POST /roles` | `CrearRolCommand` | `Roles.Crear` | UC-04, paso 1 |
| `PUT /roles/{id}` | `EditarRolCommand` | `Roles.Editar` | UC-04, paso 1 (nombre/descripción — completa la brecha detectada 2026-07-19: el paso 1 de UC-04 no tenía endpoint propio) |
| `PUT /roles/{id}/permisos` | `AsignarPermisosCommand` | `Roles.Editar` | UC-04, paso 2 (reemplaza el conjunto completo de permisos del rol; `modulo` es texto libre — RN-028, no se fija en código) |

### 6.2 Clientes y Proveedores

| Método y ruta | Command/Query | Permiso | UC |
|---|---|---|---|
| `GET /clientes?busqueda=&pagina=` | `BuscarClientesQuery` | `Clientes.Consultar` | UC-05 |
| `POST /clientes` | `RegistrarClienteCommand` | `Clientes.Crear` | UC-05 |
| `PUT /clientes/{id}` | `EditarClienteCommand` | `Clientes.Editar` | UC-05 |
| `PATCH /clientes/{id}/estado` | `DesactivarClienteCommand` | `Clientes.Eliminar` | UC-05 (RF-014, baja lógica) |
| `GET /clientes/{id}/historial` | `ConsultarHistorialClienteQuery` | `Clientes.Consultar` | UC-05 (RF-017) |
| `GET /proveedores`, `POST`, `PUT`, `PATCH /estado` | *(análogo a Clientes)* | `Proveedores.*` | UC-09 |

### 6.3 Inventario

| Método y ruta | Command/Query | Permiso | UC |
|---|---|---|---|
| `GET /productos?categoria=&busqueda=&pagina=` | `BuscarProductosQuery` | `Inventario.Consultar` | UC-10, UC-11 (RF-023 a RF-026; `stock_actual` va incluido en el DTO, cubriendo la consulta de stock) |
| `POST /productos` | `RegistrarProductoCommand` | `Inventario.Crear` | UC-10 |
| `PUT /productos/{id}` | `EditarProductoCommand` | `Inventario.Editar` | UC-10 |
| `PATCH /productos/{id}/estado` | `CambiarEstadoProductoCommand` | `Inventario.Eliminar` | RN-023 (baja lógica, análogo a Cliente/Proveedor; completa una brecha del diseño original, que no contemplaba este endpoint) |
| `GET /productos/{id}/movimientos?desde=&hasta=` | `ConsultarKardexQuery` | `Inventario.Consultar` | RF-073 (Kardex del producto) |
| `POST /productos/{id}/ajustes` | `AjustarInventarioCommand` | `Inventario.Ajustar` **(otorgado solo al Administrador, RN-008)** | UC-12 (motivo obligatorio; rechaza si el resultado deja el stock negativo) |
| `GET /categorias` | `ListarCategoriasQuery` | `Inventario.Consultar` | CAT-002 (consultada también al registrar/filtrar productos) |
| `POST /categorias` | `CrearCategoriaCommand` | `Configuracion.Editar` | UC-37, CAT-002 |
| `PUT /categorias/{id}` | `EditarCategoriaCommand` | `Configuracion.Editar` | UC-37, CAT-002 (completa una brecha detectada 2026-07-19: el catálogo no tenía endpoint de edición) |
| `GET /unidades-medida` | `ListarUnidadesMedidaQuery` | `Inventario.Consultar` | CAT-014 (RN-040, resuelve BQ-008) |
| `POST /unidades-medida` | `CrearUnidadMedidaCommand` | `Configuracion.Editar` | CAT-014 |
| `PUT /unidades-medida/{id}` | `EditarUnidadMedidaCommand` | `Configuracion.Editar` | CAT-014 |

### 6.4 Compras

| Método y ruta | Command/Query | Permiso | UC |
|---|---|---|---|
| `GET /compras?proveedor=&producto=&pagina=` | `BuscarComprasQuery` | `Compras.Consultar` | RF-036 (filtro por proveedor y/o por producto) |
| `POST /compras` | `RegistrarCompraCommand` | `Compras.Crear` | UC-13 (RN-024: compra directa, sin orden de compra ni aprobación previa; actualiza stock/Kardex/costo promedio ponderado — RN-013, pendiente de validación formal del contador antes de producción) |
| `GET /compras/{id}` | `ObtenerCompraQuery` | `Compras.Consultar` | UC-13 |

### 6.5 Ventas

| Método y ruta | Command/Query | Permiso | UC |
|---|---|---|---|
| `GET /ventas?estado=&pagina=` | `BuscarVentasQuery` | `Ventas.Consultar` | — |
| `POST /ventas` | `RegistrarVentaCommand` | `Ventas.Crear` | UC-14 |
| `GET /ventas/{id}` | `ObtenerVentaQuery` | `Ventas.Consultar` | UC-14 |
| `POST /ventas/{id}/anular` | `AnularVentaCommand` | `Ventas.Anular` | UC-16 *([PV] BQ-013)* |
| `POST /ventas/{id}/devoluciones` | `RegistrarDevolucionCommand` | `Ventas.Crear` | UC-17 |

### 6.6 Cobranzas *(módulo separado de Caja — ADR-012)*

| Método y ruta | Command/Query | Permiso | UC |
|---|---|---|---|
| `GET /saldos-pendientes?estado=Pendiente` | `ListarSaldosPendientesQuery` | `Cobranzas.Consultar` | — |
| `POST /saldos-pendientes/{id}/cobros` | `CobrarSaldoPendienteCommand` | `Cobranzas.Cobrar` | UC-21 |

> Nota: `saldos-pendientes` no se crea con un `POST` propio — se genera como efecto colateral de `RegistrarVentaCommand`, `EntregarEquipoCommand` o `CerrarServicioCampoCommand` cuando se autoriza un saldo (RN-001/RN-031), siempre validando el permiso `Cobranzas.Autorizar` del Administrador dentro de esos Commands.

### 6.7 Caja

| Método y ruta | Command/Query | Permiso | UC |
|---|---|---|---|
| `GET /caja` | `ObtenerCajaActualQuery` | `Caja.Consultar` | UC-19 (completa una brecha detectada 2026-07-19: no había forma de consultar si hay una caja abierta antes de intentar abrir/cerrar/registrar. Devuelve la abierta actual o, si no hay ninguna, la más reciente cerrada) |
| `POST /caja/apertura` | `AbrirCajaCommand` | `Caja.Abrir` | UC-19 (RN-025: rechaza si ya existe una caja abierta) |
| `POST /caja/cierre` | `CerrarCajaCommand` | `Caja.Cerrar` | UC-19 (RN-015: concilia monto teórico vs. físico declarado) |
| `GET /caja/movimientos?cajaId=&desde=&hasta=` | `ListarMovimientosCajaQuery` | `Caja.Consultar` | UC-20 (si no se indica `cajaId`, usa la caja abierta actual o la más reciente) |
| `POST /caja/movimientos` | `RegistrarMovimientoCajaCommand` | `Caja.Registrar` | UC-20 (RN-026: egreso/ingreso manual sin origen transaccional — GastoOperativo, RetiroPropietario o AporteCapital; RN-014: rechaza si no hay caja abierta) |

> **Alcance de esta implementación (2026-07-19):** `movimientos_caja` aún no incluye las columnas `venta_id`/`orden_trabajo_id`/`servicio_campo_id`/`saldo_pendiente_id` del modelo físico original — sus tablas no existen todavía. Se agregarán con FK real cuando se construyan los módulos de Ventas, Taller, Servicios de Campo y Cobranzas, respectivamente (mismo patrón que `MovimientoInventario.CrearCompra`, agregado solo cuando existió el módulo de Compras).

### 6.8 Taller

| Método y ruta | Command/Query | Permiso | UC |
|---|---|---|---|
| `POST /ordenes-trabajo` | `RegistrarRecepcionCommand` | `Taller.Recepcionar` *(Administrador, Ventas o Técnico — RN-029)* | UC-22 |
| `GET /ordenes-trabajo?estado=&cliente=&pagina=` | `BuscarOrdenesTrabajoQuery` | `Taller.Consultar` | RF-074 |
| `GET /ordenes-trabajo/{id}` | `ObtenerOrdenTrabajoQuery` | `Taller.Consultar` | UC-28 |
| `POST /ordenes-trabajo/{id}/diagnostico` | `RegistrarDiagnosticoCommand` | `Taller.Diagnosticar` | UC-23 |
| `POST /ordenes-trabajo/{id}/cotizacion` | `GenerarCotizacionReparacionCommand` | `Taller.Cotizar` | UC-24 |
| `POST /ordenes-trabajo/{id}/decision` | `RegistrarDecisionClienteCommand` | `Taller.Cotizar` | UC-24 (aprobar/rechazar, RN-016/RN-030) |
| `POST /ordenes-trabajo/{id}/reparacion` | `RegistrarReparacionCommand` | `Taller.Reparar` | UC-25 |
| `POST /ordenes-trabajo/{id}/entrega` | `EntregarEquipoCommand` | `Taller.Entregar` *(Administrador, Ventas o Técnico — RN-029)* | UC-26 |
| `POST /ordenes-trabajo/{id}/garantia` | `RegistrarGarantiaCommand` | `Taller.Editar` | UC-27 |

> **Alcance de esta implementación (2026-07-20):** no existe un endpoint para RF-062 (asociar una OT de reingreso a una garantía vigente) — no está definido aquí y cruza dos OTs distintas; queda como mejora futura. Tampoco existe un paso separado para `EnReparacion`/`EnPruebas`: `RegistrarReparacionCommand` combina consumo de repuestos y resultado de pruebas en un único paso, transicionando directo de `Aprobado` a `ListoParaEntrega`.

### 6.9 Servicios de Campo

| Método y ruta | Command/Query | Permiso | UC |
|---|---|---|---|
| `POST /servicios-campo` | `SolicitarServicioCampoCommand` | `ServiciosCampo.Crear` | UC-30 |
| `GET /servicios-campo?estado=&pagina=` | `BuscarServiciosCampoQuery` | `ServiciosCampo.Consultar` | RF-075 |
| `POST /servicios-campo/{id}/cotizacion` | `CotizarServicioCampoCommand` | `ServiciosCampo.Cotizar` | UC-31 |
| `POST /servicios-campo/{id}/cierre` | `CerrarServicioCampoCommand` | `ServiciosCampo.Cerrar` | UC-32 |
| `POST /servicios-campo/{id}/cobro` | `CobrarServicioCampoCommand` | `ServiciosCampo.Cobrar` | UC-33 |

### 6.10 Gestión Documental

| Método y ruta | Command/Query | Permiso | UC |
|---|---|---|---|
| `POST /documentos` *(multipart/form-data: entidadTipo, entidadId, tipoDocumento, archivo)* | `AdjuntarDocumentoCommand` | `Documentos.Crear` | UC-34 |
| `GET /documentos?entidadTipo=&entidadId=` | `ListarDocumentosQuery` | `Documentos.Consultar` | UC-34 |
| `DELETE /documentos/{id}` | `EliminarDocumentoCommand` | `Documentos.Eliminar` | — |

### 6.11 Reportes (solo Queries — RF-072 a RF-077)

| Método y ruta | Query | Permiso |
|---|---|---|
| `GET /reportes/ventas?desde=&hasta=` | `ReporteVentasQuery` | `Reportes.Consultar` |
| `GET /reportes/inventario` | `ReporteInventarioQuery` | `Reportes.Consultar` |
| `GET /reportes/ordenes-trabajo?estado=&tecnico=` | `ReporteOrdenesTrabajoQuery` | `Reportes.Consultar` |
| `GET /reportes/servicios-campo` | `ReporteServiciosCampoQuery` | `Reportes.Consultar` |
| `GET /reportes/caja?desde=&hasta=` | `ReporteCajaQuery` | `Reportes.Consultar` |

### 6.12 Auditoría (solo lectura)

| Método y ruta | Query | Permiso | UC |
|---|---|---|---|
| `GET /auditoria?usuario=&modulo=&desde=&hasta=&pagina=` | `ConsultarAuditoriaQuery` | `Auditoria.Consultar` | UC-36 |

### 6.13 Configuración

| Método y ruta | Command/Query | Permiso | UC |
|---|---|---|---|
| `GET /configuracion/empresa` | `ObtenerConfiguracionEmpresaQuery` | `Configuracion.Consultar` | UC-37 |
| `PUT /configuracion/empresa` | `ActualizarConfiguracionEmpresaCommand` | `Configuracion.Editar` | UC-37 |
| `GET /configuracion/medios-pago` | `ListarMediosPagoQuery` | `Configuracion.Consultar` | UC-37 (CAT-008, configurable — refinado 2026-07-19: se separó lectura de escritura, igual que en el resto de la API) |
| `POST /configuracion/medios-pago` | `CrearMedioPagoCommand` | `Configuracion.Editar` | UC-37, CAT-008 |
| `PATCH /configuracion/medios-pago/{id}/estado` | `CambiarEstadoMedioPagoCommand` | `Configuracion.Editar` | UC-37, CAT-008 |

## 7. DTOs representativos

No se repite aquí el detalle completo de `Data-Dictionary.md` — solo los DTOs con forma distinta a su entidad (agregaciones, combinaciones de varias tablas):

```json
// POST /ventas — RegistrarVentaCommand
{
  "clienteId": "uuid | null",
  "tipoComprobante": "Boleta",
  "origen": "Directa",
  "lineas": [
    { "productoId": "uuid", "cantidad": 2, "precioUnitario": 25.50 }
  ],
  "pagos": [
    { "medioPagoId": "uuid", "monto": 51.00 }
  ],
  "saldoPendiente": null
}

// POST /ordenes-trabajo/{id}/entrega — EntregarEquipoCommand
{
  "estadoPago": "SaldoPendiente",
  "montoPagado": 50.00,
  "saldoPendiente": {
    "monto": 30.00,
    "usuarioAutorizoId": "uuid",
    "fechaReferenciaPago": "2026-08-01"
  }
}
```

## 8. Contrato de Swagger/OpenAPI

Generado automáticamente desde los controladores (`Swashbuckle`, ya en `TECH_STACK.md`). Cada `Command`/`Query` documenta su DTO de entrada/salida mediante los atributos estándar de ASP.NET Core; no se mantiene un contrato OpenAPI manual separado de este documento para evitar duplicidad (`AI_INSTRUCTIONS.md`: no generar documentación redundante que se desactualice).

## 9. Siguiente paso

Con los contratos de API definidos, el siguiente entregable de la Fase 3 es el **Diseño UI/UX** (`06-UI-UX/`), que consume estos mismos endpoints desde la perspectiva de cada pantalla.

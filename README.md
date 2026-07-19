# ISARMIN ERP

ERP profesional para la gestión integral de ISARMIN PERÚ S.A.C.

## Estado

🔵 Fase de Desarrollo (ver [docs/00-Project/PROJECT_ROADMAP.md](docs/00-Project/PROJECT_ROADMAP.md) para el detalle de fases).

## Objetivo

Desarrollar un sistema ERP moderno para gestionar:

- Inventario
- Ventas
- Compras
- Taller de reparaciones
- Servicios de campo
- Caja
- Reportes
- Usuarios y permisos
- Configuración

## Stack Tecnológico

- React 19 + TypeScript
- ASP.NET Core 9 Web API
- PostgreSQL
- Tailwind CSS

Decisión formalizada en [docs/00-Project/DECISIONS.md](docs/00-Project/DECISIONS.md) (ADR-001 a ADR-005). Detalle completo en [docs/00-Project/TECH_STACK.md](docs/00-Project/TECH_STACK.md).

## Desarrollo local

### Backend

1. Requiere PostgreSQL en ejecución. Configura la cadena de conexión en `src/ISARMIN.API/appsettings.json` (`ConnectionStrings:DefaultConnection`) o, preferible en desarrollo, con `dotnet user-secrets`.
2. Configura `Jwt:Key` (mínimo 32 caracteres) — nunca dejar el valor placeholder en un entorno real.
3. Aplica las migraciones:
   ```
   dotnet ef database update --project src/ISARMIN.Infrastructure --startup-project src/ISARMIN.API
   ```
4. Ejecuta la API: `dotnet run --project src/ISARMIN.API`. Swagger disponible en `/swagger` (entorno Development).

**Usuario Administrador sembrado (bootstrap):** la primera migración crea un único usuario para poder iniciar sesión por primera vez, ya que crear usuarios (UC-03) requiere estar ya autenticado como Administrador.
- Usuario: `admin`
- Contraseña temporal: `IsarminAdmin#2026`

Cambiar esta contraseña en cuanto exista el módulo de Usuarios (o manualmente en la base de datos) es responsabilidad de quien despliegue el sistema — no debe usarse en producción sin cambiarla.

### Frontend

```
cd frontend
npm install
npm run dev
```

---

Proyecto desarrollado por:

José Antonio De la Cruz Portal
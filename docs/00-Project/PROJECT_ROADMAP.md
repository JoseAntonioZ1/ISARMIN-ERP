# PROJECT_ROADMAP.md

# Roadmap del Proyecto

## Estado Actual

🔵 Fase 4 (Desarrollo) en curso — estructura profesional del proyecto (backend + frontend) creada el 2026-07-19; próximo paso: módulo de Autenticación

---

# Fase 1 — Preparación

- Estructura del proyecto
- Documentación inicial
- Contexto del negocio
- Configuración del repositorio

Estado: ✅ Completada

---

# Fase 2 — Análisis

- Levantamiento de requerimientos
- Reglas de negocio
- Procesos
- Actores
- Glosario
- Catálogos
- Estados
- Riesgos
- Preguntas al cliente

Estado: ✅ Completada (91 preguntas registradas, 37 resueltas y 9 parciales tras 4 rondas de validación directa con el propietario; 45 pendientes de detalle no bloqueante, seguimiento continuo en `Business-Questions.md`)

---

# Fase 3 — Diseño

- Arquitectura del sistema
- Modelo de dominio
- Modelo conceptual
- Modelo entidad-relación
- Diseño de APIs
- Diseño UI/UX

Estado: ✅ Completada — Arquitectura del sistema (`03-Architecture/Architecture-Overview.md`, ADR-006 a ADR-013 en `DECISIONS.md`), Modelo de dominio y conceptual (`01-Requirements/Conceptual-Data-Model.md`, `Data-Dictionary.md`), Modelo entidad-relación físico (`04-Database/Physical-Data-Model.md`), Diseño de APIs (`05-Backend-API/API-Design.md`) y Diseño UI/UX (`06-UI-UX/UX-Design.md`)

---

# Fase 4 — Desarrollo

- Backend
- Frontend
- Base de datos
- Integraciones

Estado: 🔵 En curso — estructura profesional inicial creada (2026-07-19): solución .NET 9 con Clean Architecture (`ISARMIN.Domain`/`Application`/`Infrastructure`/`API` + proyectos de test xUnit), configuración base (EF Core + PostgreSQL vía Npgsql/EFCore.NamingConventions, JWT, Swagger, Serilog, CORS, manejo global de excepciones), y proyecto frontend React 19 + TypeScript + Vite (Tailwind CSS, React Router, TanStack Query, Zustand, React Hook Form + Zod). Módulos **Autenticación** (UC-01/UC-02), **Usuarios** (UC-03), **Roles y Permisos** (UC-04), **Catálogos** (Categorías CAT-002, Medios de Pago CAT-008), **Clientes** (UC-05) y **Proveedores** (UC-09) completos — backend y frontend, verificados end-to-end contra PostgreSQL real. Siguiente módulo: Productos.

---

# Fase 5 — Pruebas

- Unitarias
- Integración
- Funcionales
- Aceptación

Estado: ⏳ Pendiente

---

# Fase 6 — Producción

- Implementación
- Capacitación
- Manuales
- Puesta en marcha

Estado: ⏳ Pendiente

---

# Objetivo Final

Construir un ERP profesional, modular y escalable que pueda evolucionar y adaptarse a empresas similares en el futuro.
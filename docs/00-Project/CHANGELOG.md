# CHANGELOG.md

# Historial de Cambios

Todos los cambios importantes del proyecto serán registrados en este documento.

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
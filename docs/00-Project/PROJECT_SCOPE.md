# PROJECT_SCOPE.md

# Alcance del Proyecto

## Nombre del Proyecto

ISARMIN ERP

---

# Objetivo

Desarrollar un sistema ERP (Enterprise Resource Planning) para ISARMIN PERÚ S.A.C. que permita integrar y administrar todos los procesos operativos de la empresa mediante una única plataforma web.

El sistema deberá reemplazar los procesos manuales actuales y mejorar el control, seguimiento y trazabilidad de la información.

---

# Objetivos Específicos

- Centralizar toda la información del negocio.
- Digitalizar los procesos del taller.
- Controlar el inventario compartido.
- Gestionar ventas y compras.
- Administrar servicios técnicos de campo.
- Mejorar el control financiero.
- Generar reportes para la toma de decisiones.
- Reducir errores operativos.
- Facilitar el crecimiento futuro del negocio.

---

# Alcance de la Primera Versión (MVP)

*Este documento es la fuente única de verdad sobre el alcance del proyecto. `PROJECT_CONTEXT.md` remite aquí en lugar de mantener su propia lista, para evitar que ambos documentos diverjan.*

La primera versión del sistema incluirá los siguientes módulos:

- Autenticación
- Usuarios
- Roles y Permisos
- Clientes
- Proveedores
- Productos
- Categorías
- Inventario
- Compras — registro directo de compras realizadas (proveedor, documento de compra, costos), sin flujo formal de Orden de Compra con aprobación previa (confirmado el 2026-07-18; ver "Fuera del Alcance Inicial")
- Ventas
- Caja
- Taller
- Recepción de Equipos
- Diagnóstico
- Cotizaciones
- Órdenes de Trabajo (registro directo; sin flujo formal de Orden de Compra con aprobación — ver Compras)
- Garantías — **alcance acotado el 2026-07-18:** solo registrar si una reparación tiene garantía, su período (fecha inicio/fin), y asociar una nueva OT a una garantía existente. Sin tipos de garantía ni gestión avanzada (queda para versión futura).
- Servicios Técnicos de Campo — **V1 es web**; se retiran temporalmente la aplicación móvil dedicada y la sincronización offline (confirmado el 2026-07-18); el registro se realiza desde el sistema web al volver a la red local.
- Gestión Documental — adjuntar fotografías de equipos, documentos de compra, cotizaciones, comprobantes e informes técnicos (nuevo, incorporado el 2026-07-18).
- Reportes
- Auditoría (capacidad transversal de bitácora de acciones críticas: creación, modificación, eliminación lógica, anulaciones, movimientos de inventario, movimientos de caja, cambios en OT; no es un módulo con interfaz propia en esta versión — confirmado con el propietario el 2026-07-18)
- Configuración General

---

# Funcionalidades Fuera del Alcance Inicial

Las siguientes funcionalidades podrán incorporarse en versiones posteriores:

- Aplicación móvil y sincronización offline para Servicios de Campo (confirmado 2026-07-18: la arquitectura debe dejar el punto de extensión listo, sin requerir rediseño)
- Portal para clientes
- Portal para técnicos
- Flujo formal de Orden de Compra con aprobación previa (confirmado 2026-07-18: hoy las compras son directas)
- Gestión avanzada de garantías: tipos de garantía y condiciones de cobertura detalladas (confirmado 2026-07-18: V1 solo registra si tiene garantía, período y vinculación a nueva OT)
- Integración con WhatsApp
- Notificaciones por correo electrónico
- Firma digital avanzada
- Integración con múltiples sucursales
- Inteligencia de negocios (BI)
- Integración con IoT

---

# Restricciones

- El sistema funcionará inicialmente en un servidor local.
- No se utilizarán servicios cloud con pagos mensuales.
- Se priorizará el uso de tecnologías Open Source.
- La integración con SUNAT será considerada en el diseño, aunque podrá implementarse en una fase posterior.
- La base de datos principal debe ser local (ej. PostgreSQL) y **no depender de Firebase ni de ningún servicio cloud** como almacenamiento principal; debe existir un respaldo automático hacia un destino externo, y el sistema debe poder restaurarse/migrarse a otra PC (confirmado 2026-07-18). Un servicio cloud podrá evaluarse únicamente como almacenamiento secundario de archivos adjuntos y/o copias de respaldo.
- Diseño inicial para 5 usuarios concurrentes, con capacidad de crecer hasta 20 usuarios concurrentes sin cambiar de arquitectura (confirmado 2026-07-18; operación real actual: 2 usuarios permanentes).

---

# Criterios de Éxito

El proyecto se considerará exitoso cuando:

- Todos los procesos principales estén digitalizados.
- Exista trazabilidad completa del inventario.
- Se eliminen los registros manuales del taller.
- Los reportes sean confiables y oportunos.
- El sistema sea escalable y mantenible.
# Non-Functional-Requirements.md — Requerimientos No Funcionales

## 1. Propósito

Especifica los atributos de calidad que debe cumplir ISARMIN ERP, siguiendo la estructura de **IEEE 830** (sección 3, "Specific Requirements — External Interface / Non-functional") y el modelo de características de calidad de **ISO/IEC 25010**, adaptado para un ERP de pequeña/mediana empresa con infraestructura on-premise.

A diferencia de los requerimientos funcionales, estos definen **cómo debe comportarse** el sistema (rendimiento, seguridad, disponibilidad, etc.), y son determinantes para las decisiones de arquitectura posteriores.

## 2. Convención

Igual que en los demás documentos: **[C]** Confirmado, **[I]** Inferido, **[PV]** Pendiente de Validación. La columna **Métrica** indica el valor objetivo cuando existe; cuando no hay una métrica documentada, se marca `PENDIENTE` y se referencia la pregunta correspondiente.

## 3. Nota de trazabilidad

Los cinco requerimientos de la versión anterior (RNF-001 a RNF-005) se conservan íntegramente, reubicados dentro de sus categorías ISO 25010 correspondientes.

## 4. Rendimiento (Performance Efficiency)

| ID | Descripción | Métrica | Estado | Referencia |
|---|---|---|---|---|
| RNF-001 | El sistema deberá responder a las operaciones del usuario en menos de 3 segundos. | < 3 s por operación estándar | [C] | Métrica original; no especifica bajo qué carga concurrente → BQ-065 |
| RNF-002 | El sistema deberá mantener tiempos de respuesta aceptables incluso durante el registro de movimientos de inventario en horas pico (cierre de caja, atención simultánea en tienda y taller). | PENDIENTE | [PV] | BQ-065 |

## 5. Compatibilidad (Compatibility)

| ID | Descripción | Métrica | Estado | Referencia |
|---|---|---|---|---|
| RNF-003 | El sistema deberá funcionar correctamente en los navegadores Chrome, Edge y Firefox. | Últimas 2 versiones estables de cada navegador (asunción [PV]) | [C] | BQ-066 (¿versiones mínimas a soportar, incluye Safari/móviles?) |
| RNF-004 | El sistema deberá ser accesible desde equipos de escritorio dentro de la red local de la empresa. | PENDIENTE (resolución mínima de pantalla, soporte tablet/móvil) | [I] | BQ-067 |

## 6. Escalabilidad y Concurrencia (Capacity)

| ID | Descripción | Métrica | Estado | Referencia |
|---|---|---|---|---|
| RNF-005 | El sistema deberá soportar múltiples usuarios trabajando simultáneamente. | Operación real actual: 2 usuarios permanentes. Diseño inicial: **5 usuarios concurrentes**. Crecimiento esperado: **20 usuarios concurrentes sin cambiar de arquitectura**. | [C] | BQ-068 (resuelta, 2026-07-18) |
| RNF-006 | El sistema deberá soportar el crecimiento del catálogo de productos y del volumen de transacciones sin degradar el rendimiento declarado en RNF-001, hasta el horizonte de 20 usuarios concurrentes de RNF-005. | PENDIENTE — volumetría exacta de productos/transacciones aún sin confirmar | [PV] | BQ-069 (volumetría estimada) |

## 7. Confiabilidad y Disponibilidad (Reliability / Availability)

| ID | Descripción | Métrica | Estado | Referencia |
|---|---|---|---|---|
| RNF-007 | El sistema deberá generar respaldos automáticos de la información. La base de datos principal será **local** (ej. PostgreSQL) y **no deberá depender de Firebase ni de ningún servicio cloud como almacenamiento principal**. Se requiere un sistema de respaldo automático hacia un destino externo a la PC servidor. | Confirmado el modelo (local + respaldo externo automático); frecuencia y retención exactas del respaldo — PENDIENTE | [C] | BQ-070 (parcialmente resuelta) |
| RNF-008 | El sistema deberá permitir la restauración de un respaldo y la **migración completa a otra PC** dentro de un tiempo máximo aceptable para el negocio (RTO). | Migración a otra PC confirmada como requisito; RTO exacto — PENDIENTE | [PV] | BQ-070 |
| RNF-009 | El sistema deberá tolerar sin pérdida de datos una interrupción eléctrica o de red propia de una infraestructura local (PC de escritorio como servidor), dado que no existe redundancia declarada. | PENDIENTE — características exactas del equipo servidor sin confirmar | [PV] | BQ-071, BQ-090 |
| RNF-025 | Un servicio cloud (ej. Firebase) podrá evaluarse **únicamente como almacenamiento secundario**, para archivos adjuntos (fotografías, documentos — ver RF-084 a RF-087) y/o como destino adicional de copias de respaldo, nunca como base de datos principal. | Confirmado (2026-07-18) | [C] | RF-087 |

## 8. Seguridad (Security)

| ID | Descripción | Métrica | Estado | Referencia |
|---|---|---|---|---|
| RNF-010 | El sistema deberá registrar auditoría de las acciones relevantes de los usuarios. | Ver RF-078 para el detalle funcional | [C] | — |
| RNF-011 | El sistema deberá almacenar las contraseñas de los usuarios de forma segura (no en texto plano). | Estándar de la industria (hash + salt) | [I] | — |
| RNF-012 | El sistema deberá restringir el acceso a cada módulo según el rol asignado al usuario (control de acceso basado en roles). | Ver RF-011 | [I] | — |
| RNF-013 | El sistema deberá definir una política de expiración/complejidad de contraseñas. | PENDIENTE | [PV] | BQ-047 |
| RNF-014 | El sistema deberá proteger las comunicaciones dentro de la red local (cifrado en tránsito), especialmente si en el futuro se habilita acceso remoto para Servicios de Campo. | PENDIENTE | [PV] | BQ-053 |

## 9. Mantenibilidad (Maintainability)

| ID | Descripción | Métrica | Estado | Referencia |
|---|---|---|---|---|
| RNF-015 | El sistema deberá diseñarse siguiendo principios de arquitectura desacoplada (frontend / backend / base de datos independientes), conforme a lo declarado en el contexto del proyecto. | N/A | [C] | — |
| RNF-016 | El sistema deberá permitir la incorporación de nuevos módulos sin requerir el rediseño de los módulos existentes. | N/A | [C] | — |
| RNF-017 | Toda funcionalidad nueva deberá quedar documentada, conforme a las instrucciones del proyecto (`AI_INSTRUCTIONS.md`). | N/A | [C] | — |

## 10. Portabilidad e Infraestructura (Portability)

| ID | Descripción | Métrica | Estado | Referencia |
|---|---|---|---|---|
| RNF-018 | El sistema deberá poder operar inicialmente sobre una PC de escritorio como servidor local, sin dependencia de servicios de pago recurrentes. | N/A | [C] | — |
| RNF-019 | El sistema deberá poder migrarse en el futuro a un entorno cloud sin requerir un rediseño de la arquitectura principal. | N/A | [C] | — |
| RNF-020 | El sistema no deberá depender de bases de datos, hosting o licencias con costos mensuales obligatorios. | N/A | [C] | — |

## 11. Cumplimiento Normativo (Compliance)

| ID | Descripción | Métrica | Estado | Referencia |
|---|---|---|---|---|
| RNF-021 | El sistema deberá cumplir con la normativa vigente de SUNAT para la emisión de comprobantes electrónicos, si se determina obligatoria. | PENDIENTE | [PV] | BQ-050, BQ-051 |
| RNF-022 | El sistema deberá conservar los comprobantes y registros contables durante el período mínimo exigido por la legislación peruana. | PENDIENTE | [PV] | BQ-072 |

## 12. Usabilidad (Usability)

| ID | Descripción | Métrica | Estado | Referencia |
|---|---|---|---|---|
| RNF-023 | El sistema deberá presentar una interfaz utilizable por personal sin formación técnica avanzada (vendedores, recepcionistas, técnicos), dado el perfil de los actores identificados. | PENDIENTE (nivel de alfabetización digital del personal) | [PV] | BQ-073 |
| RNF-024 | El sistema deberá minimizar el número de pasos para las operaciones de mayor frecuencia (registro de venta, registro de OT). | PENDIENTE | [PV] | — |

## 13. Resumen de métricas pendientes de cuantificar

Las siguientes métricas son actualmente cualitativas y deben cuantificarse antes del diseño de arquitectura, para evitar decisiones técnicas basadas en supuestos:

| Métrica | RNF relacionado | Pregunta | Estado |
|---|---|---|---|
| Usuarios concurrentes esperados (actual / futuro) | RNF-005, RNF-006 | BQ-068 | ✅ Resuelta: 2 hoy, 5 diseño inicial, 20 a futuro |
| Volumen de transacciones/productos esperado | RNF-006 | BQ-069 | Pendiente |
| Frecuencia y retención de respaldos | RNF-007 | BQ-070 | 🟡 Parcial: modelo local + externo confirmado, frecuencia exacta pendiente |
| RTO/RPO aceptable ante falla | RNF-008, RNF-009 | BQ-070, BQ-071 | Pendiente |
| Características del equipo servidor | RNF-009 | BQ-090 | Pendiente |
| Navegadores/dispositivos mínimos a soportar | RNF-003, RNF-004 | BQ-066, BQ-067 | Pendiente |

# PROJECT_CONTEXT.md

# Contexto General del Proyecto

## Nombre del Proyecto

ISARMIN ERP

---

# Descripción General

ISARMIN ERP es un sistema ERP (Enterprise Resource Planning) desarrollado específicamente para la empresa **ISARMIN PERÚ S.A.C.**

El objetivo del proyecto es digitalizar y centralizar todos los procesos operativos de la empresa mediante un único sistema integrado, eliminando procesos manuales, reduciendo errores, mejorando el control del inventario, optimizando el seguimiento de los servicios técnicos y proporcionando información confiable para la toma de decisiones.

Este proyecto no debe considerarse un simple sistema administrativo ni un CRUD. Su diseño debe seguir estándares profesionales de arquitectura de software y estar preparado para crecer durante muchos años.

---

# Sobre la Empresa

ISARMIN PERÚ S.A.C. desarrolla diversas actividades comerciales y técnicas.

Actualmente la empresa cuenta con tres grandes áreas de negocio que comparten recursos e inventario.

## 1. Tienda Comercial

La empresa comercializa una gran variedad de productos, entre ellos:

- Herramientas eléctricas
- Herramientas manuales
- Materiales eléctricos
- Materiales sanitarios
- Repuestos
- Accesorios
- Productos tecnológicos
- Parlantes
- Audífonos
- Relojes
- Focos
- Interruptores
- Tubos
- Cables
- Llaves
- Codos
- Carbones
- Cuchillas
- Rodamientos
- Consumibles
- Otros productos relacionados

Durante las ventas pueden emitirse:

- Cotizaciones
- Boletas
- Facturas
- Notas de Venta
- Tickets

---

## 2. Taller de Servicio Técnico

La empresa recibe equipos para mantenimiento y reparación.

Ejemplos:

- Amoladoras
- Taladros
- Bombas de agua
- Soldadoras
- Hidrolavadoras
- Lavadoras
- Refrigeradoras
- Licuadoras
- Herramientas eléctricas
- Equipos industriales
- Equipos electromecánicos

Proceso actual:

Cliente entrega un equipo.

↓

Se llena un formato manual.

↓

Se entrega un comprobante de recepción.

↓

Se realiza diagnóstico.

↓

Se genera cotización.

↓

Cliente aprueba.

↓

Se realiza reparación.

↓

Se utilizan repuestos del almacén.

↓

Se realizan pruebas.

↓

Se entrega el equipo.

↓

Se registra el pago.

Todo este proceso deberá digitalizarse completamente.

---

## 3. Servicios Técnicos de Campo

La empresa también realiza trabajos fuera de sus instalaciones.

Ejemplos:

- Instalaciones eléctricas
- Instalaciones industriales
- Mantenimiento preventivo
- Mantenimiento correctivo
- Cambio de bombas
- Cambio de motores
- Instalación de tableros
- Instalaciones domiciliarias
- Instalaciones comerciales
- Mantenimiento de plantas de agua
- Otros proyectos técnicos

Actualmente estos procesos se controlan mediante formatos manuales.

El ERP deberá administrar completamente este flujo.

---

# Inventario

Uno de los puntos más importantes del sistema.

Existe un único inventario compartido.

Los productos pueden:

- venderse
- utilizarse como repuestos en el taller
- utilizarse en servicios de campo

Todo movimiento debe actualizar automáticamente el stock.

Debe existir trazabilidad completa de cada movimiento.

---

# Objetivo General

Construir un ERP moderno, robusto y escalable que integre todos los procesos de la empresa en un solo sistema.

El sistema debe mejorar:

- Productividad
- Organización
- Control
- Seguimiento
- Trazabilidad
- Seguridad
- Toma de decisiones

---

# Objetivos Específicos

El sistema deberá permitir administrar:

- Inventario
- Compras
- Ventas
- Clientes
- Proveedores
- Taller
- Órdenes de Trabajo
- Diagnósticos
- Cotizaciones
- Consumo de Repuestos
- Garantías
- Servicios de Campo
- Técnicos
- Caja
- Reportes
- Auditoría
- Usuarios
- Roles
- Permisos
- Configuración

---

# Filosofía del Proyecto

Este proyecto NO debe desarrollarse como un proyecto universitario.

Debe seguir estándares profesionales de ingeniería de software.

La prioridad será:

- Escalabilidad
- Mantenibilidad
- Modularidad
- Seguridad
- Buenas prácticas
- Código limpio
- Arquitectura limpia

Toda decisión técnica debe justificarse.

---

# Alcance Inicial

El alcance detallado y versionado de la primera versión (MVP) del sistema —incluyendo módulos incluidos, funcionalidades explícitamente fuera de alcance y criterios de éxito— se mantiene como fuente única de verdad en [`PROJECT_SCOPE.md`](PROJECT_SCOPE.md), para evitar que dos documentos describan el mismo alcance de forma independiente y terminen contradiciéndose con el tiempo.

De forma resumida, la primera versión cubre: Usuarios, Roles y Permisos, Clientes, Proveedores, Inventario, Compras, Ventas, Caja, Taller (con Recepción, Diagnóstico, Cotizaciones y Órdenes de Trabajo) y Servicios de Campo, además de Reportes y Configuración General. Ante cualquier duda sobre si un módulo específico está o no en el alcance inicial, `PROJECT_SCOPE.md` prevalece sobre esta sección.

Posteriormente podrán incorporarse nuevos módulos.

---

# Arquitectura Esperada

El proyecto utilizará una arquitectura moderna basada en:

Frontend

Backend

Base de Datos

Documentación

Cada capa deberá mantenerse desacoplada.

La arquitectura deberá facilitar futuras ampliaciones.

---

# Infraestructura

La empresa no desea depender de servicios con pagos mensuales.

Inicialmente el sistema funcionará utilizando:

- Una PC de escritorio como servidor local.
- Red local de la empresa.
- Navegadores web para acceder al sistema.

En el futuro podrá migrarse a la nube sin modificar la arquitectura principal.

---

# Restricciones

La empresa NO desea depender de:

- Hosting obligatorio
- Servidores cloud
- Bases de datos de pago
- Licencias mensuales
- Software propietario con suscripciones

Los únicos costos aceptados son:

- Facturación electrónica (SUNAT)
- Hardware
- Impresoras
- Lectores de código de barras
- Equipamiento físico

---

# Visión a Largo Plazo

Este ERP deberá diseñarse de manera modular.

En el futuro deberá ser posible adaptarlo fácilmente a:

- Ferreterías
- Talleres de reparación
- Empresas de mantenimiento
- Empresas de servicios técnicos
- Empresas comerciales similares

Por ello se debe evitar desarrollar funcionalidades específicas que no puedan configurarse.

Siempre que sea posible se preferirá una solución configurable antes que una solución rígida.

---

# Estado Actual del Proyecto

Actualmente el proyecto se encuentra en la fase de:

Análisis de Requerimientos.

No debe generarse código sin haber definido previamente:

- Requerimientos
- Procesos
- Casos de uso
- Reglas de negocio
- Arquitectura
- Modelo de Base de Datos

---

# Forma de Trabajo

Claude Code actuará como desarrollador y asistente técnico.

Antes de implementar cualquier funcionalidad deberá revisar la documentación existente.

Nunca deberá asumir reglas de negocio que no estén documentadas.

Si existe ambigüedad deberá solicitar aclaración.

Toda funcionalidad nueva deberá mantener coherencia con la arquitectura definida.

El objetivo final es construir un ERP profesional, mantenible y preparado para evolucionar durante muchos años.
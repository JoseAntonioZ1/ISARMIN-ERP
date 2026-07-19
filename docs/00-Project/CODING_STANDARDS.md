# CODING_STANDARDS.md

# Estándares de Desarrollo

## Objetivo

Este documento define las reglas y convenciones de desarrollo que deberán seguir todos los desarrolladores y asistentes de IA durante el proyecto ISARMIN ERP.

El objetivo es mantener un código limpio, consistente, mantenible y escalable.

---

# Principios Generales

- Priorizar la legibilidad sobre la complejidad.
- Evitar código duplicado.
- Seguir los principios SOLID.
- Aplicar Clean Code.
- Mantener bajo acoplamiento y alta cohesión.
- Escribir código fácil de probar.

---

# Convenciones de Nombres

## Variables

camelCase

Ejemplo:

```ts
productName
customerId
stockAvailable
```

## Funciones

camelCase

```ts
createOrder()
calculateTotal()
updateInventory()
```

## Clases

PascalCase

```ts
ProductService
InventoryRepository
UserController
```

## Interfaces

PascalCase con prefijo I

```ts
IProductRepository
IUserService
```

## Archivos

React

PascalCase

```text
ProductCard.tsx
InventoryPage.tsx
```

Servicios

```text
product.service.ts
inventory.service.ts
```

---

# Organización del Código

Cada módulo deberá estar desacoplado.

No deberá existir lógica de negocio en el Frontend.

Toda la lógica del negocio residirá en el Backend.

---

# Comentarios

Comentar únicamente cuando el código no sea suficientemente claro.

Evitar comentarios innecesarios.

---

# Manejo de Errores

Todos los errores deberán manejarse mediante excepciones controladas.

No deberán utilizarse bloques try-catch vacíos.

Todos los errores deberán registrarse mediante logging.

---

# Validaciones

Toda información deberá validarse tanto en Frontend como Backend.

Nunca confiar únicamente en el Frontend.

---

# Seguridad

Nunca almacenar contraseñas en texto plano.

Siempre utilizar hash seguro.

No exponer información sensible.

---

# Base de Datos

No escribir consultas SQL repetidas.

Utilizar Entity Framework.

Las migraciones deberán mantenerse bajo control de versiones.

---

# Documentación

Toda funcionalidad importante deberá documentarse.

Todo cambio arquitectónico deberá registrarse en DECISIONS.md.

---

# Calidad

Antes de realizar un commit verificar:

- Compila correctamente.
- No existen errores.
- No existen advertencias críticas.
- El código mantiene el estilo definido.
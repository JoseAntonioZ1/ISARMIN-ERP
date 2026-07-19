# GIT_WORKFLOW.md

# Flujo de Trabajo con Git

## Objetivo

Mantener un historial de cambios claro, organizado y fácil de mantener.

---

# Ramas

## main

Contiene únicamente versiones estables.

Nunca desarrollar directamente sobre main.

---

## develop

Rama principal de desarrollo.

Todas las funcionalidades se integrarán primero aquí.

---

## feature/*

Cada nueva funcionalidad se desarrollará en una rama independiente.

Ejemplos:

feature/inventory

feature/workshop

feature/authentication

feature/sales

---

## hotfix/*

Correcciones urgentes.

Ejemplo:

hotfix/login-error

---

# Convención de Commits

Se utilizará Conventional Commits.

Ejemplos:

feat: add inventory module

fix: correct stock calculation

refactor: simplify authentication service

docs: update business rules

style: improve code formatting

test: add inventory tests

chore: update dependencies

---

# Commits

Los commits deberán ser pequeños.

Cada commit deberá representar una única funcionalidad o corrección.

Evitar commits con múltiples cambios no relacionados.

---

# Pull Requests

Antes de fusionar una rama:

- Verificar compilación.
- Revisar código.
- Actualizar documentación si corresponde.

---

# Versionado

Se utilizará Semantic Versioning.

Ejemplo:

0.1.0

0.2.0

1.0.0

1.1.0

2.0.0

---

# Buenas Prácticas

- Hacer commit con frecuencia.
- Mantener el repositorio limpio.
- No subir archivos temporales.
- No subir credenciales.
- No subir archivos .env.
- Mantener actualizado el CHANGELOG.

---

# Revisión

Antes de realizar un merge:

- El proyecto debe compilar.
- No debe haber errores.
- La documentación debe estar actualizada.
- Las migraciones deben estar incluidas si hubo cambios en la base de datos.
# ISARMIN.BackupService

Servicio de línea de comandos, independiente del resto del ERP, que:

1. Genera un dump de la base de datos PostgreSQL (`pg_dump`, formato comprimido `-Fc`).
2. Lo cifra con AES-256 (clave derivada de una passphrase que tú defines).
3. Sube el archivo cifrado a una carpeta de tu Google Drive personal (15 GB gratis, sin tarjeta de crédito).
4. Borra automáticamente los backups más antiguos, conservando solo los `N` más recientes (por defecto 30).

No requiere ningún plan de pago: usa tu cuenta gratuita de Google Drive (15 GB, sin tarjeta de crédito) y [rclone](https://rclone.org/), una herramienta gratuita de código abierto hecha específicamente para automatizar subidas a la nube. Se descartó usar una cuenta de servicio de Google Cloud (el enfoque inicial): las cuentas de servicio **no tienen cuota de almacenamiento propia** en Drive personal — solo funcionan con "Shared Drives", que es una función exclusiva de Google Workspace (de pago). rclone en cambio se autentica como tu propia cuenta gratuita, así que sí funciona.

## 1. Instalar y conectar rclone con tu cuenta de Google (una sola vez por máquina)

1. Descarga rclone desde [rclone.org/downloads](https://rclone.org/downloads/) (la versión para Windows es un `.zip` con `rclone.exe` adentro — no necesita instalador, solo descomprimir).
2. Copia `rclone.exe` a una carpeta fija, ej. `C:\ISARMIN\rclone\rclone.exe`.
3. Abre una terminal en esa carpeta y corre:
   ```powershell
   .\rclone.exe config
   ```
4. En el asistente interactivo:
   - `n` (new remote) → nombre: `isarmin-drive` (o el que prefieras, lo vas a necesitar en el paso 3).
   - Storage: elige **Google Drive** (busca el número que dice `drive`).
   - Client ID / Client Secret: déjalos en blanco (usa las credenciales compartidas de rclone, ya verificadas por Google — evita la pantalla de advertencia de "app no verificada").
   - Scope: elige la opción `drive.file` ("Access to files created by rclone only") — es más seguro, ya que rclone solo podrá ver/tocar los archivos que él mismo sube, no el resto de tu Drive.
   - Root folder ID, service account: déjalos en blanco.
   - Edit advanced config: `n`.
   - Use auto config: `y` — se abre tu navegador, inicias sesión con tu cuenta de Google y aceptas el permiso.
   - Confirma y `q` para salir del asistente.
5. Ya quedó guardado en `%APPDATA%\rclone\rclone.conf` en esa máquina — no expira ni pide volver a loguearte (a diferencia del enfoque de cuenta de servicio que sí generaba errores de cuota).
6. Crea la carpeta de destino una sola vez:
   ```powershell
   .\rclone.exe mkdir isarmin-drive:ISARMIN-Backups
   ```

> Nota para cuando pases esto a la PC de la empresa: este paso (`rclone config`) hay que repetirlo una vez en esa máquina nueva — el archivo `rclone.conf` con la sesión queda ligado a la PC donde lo generaste, no viaja solo con el código.

## 2. Configurar las variables de entorno en el servidor

Todo lo sensible se pasa por **variables de entorno del sistema** (no por archivos del proyecto), así nunca hay riesgo de subir una credencial a git por accidente.

En Windows: `Panel de Control > Sistema > Configuración avanzada del sistema > Variables de entorno > Nueva` (variables del sistema, no solo de usuario, para que la tarea programada las vea).

| Variable | Valor | De dónde sale |
|---|---|---|
| `ISARMIN_BACKUP_DB_CONNECTION` | `Host=localhost;Port=5432;Database=isarmin_erp;Username=postgres;Password=...` | La misma cadena de conexión que ya usa el backend. **Ojo**: si en tu backend la contraseña real está guardada en `dotnet user-secrets` (no en `appsettings.json`), usa esa, no el placeholder `CHANGE_ME` del archivo |
| `ISARMIN_BACKUP_ENCRYPTION_KEY` | Una frase larga y aleatoria, ej. `correct-horse-battery-staple-2026-xyz` | La inventas tú. **Guárdala en un gestor de contraseñas** — si la pierdes, no podrás descifrar ningún backup |
| `ISARMIN_BACKUP_RCLONE_DESTINO` | `isarmin-drive:ISARMIN-Backups` | El nombre de remote que elegiste en `rclone config` (paso 1) + `:` + el nombre de la carpeta creada con `rclone mkdir` |
| `ISARMIN_BACKUP_CANTIDAD_A_CONSERVAR` | `30` (opcional, este es el valor por defecto) | Cuántos backups recientes conservar antes de borrar los viejos |
| `ISARMIN_BACKUP_RCLONE_PATH` | Ej. `C:\ISARMIN\rclone\rclone.exe` | Ruta donde copiaste `rclone.exe` en el paso 1 (o déjala sin definir si lo agregaste al PATH del sistema) |
| `ISARMIN_BACKUP_PG_DUMP_PATH` | Ej. `C:\Program Files\PostgreSQL\17\bin\pg_dump.exe` | Necesario si en el PATH del sistema hay otro `pg_dump` más viejo de otro programa (ej. software de asistencia biométrica) que no coincide con la versión de tu servidor Postgres — usa siempre la ruta completa al `pg_dump.exe` de tu propia instalación de PostgreSQL |

## 3. Probar manualmente

```powershell
cd src\ISARMIN.BackupService
dotnet run
```

Si todo está bien configurado, verás el progreso (dump → cifrado → subida → rotación) y el archivo aparecerá cifrado en tu carpeta de Drive.

## 4. Dejarlo automático (tarea programada diaria)

```powershell
dotnet publish -c Release -o C:\ISARMIN\backup-service
```

Luego, en el **Programador de tareas de Windows**:
- Desencadenador: diario, a una hora sin actividad (ej. 2:00 a.m.).
- Acción: iniciar `C:\ISARMIN\backup-service\ISARMIN.BackupService.exe`.
- "Ejecutar tanto si el usuario inició sesión como si no".

## 5. Restaurar un backup (en caso de emergencia)

1. Descarga el archivo `.dump.enc` desde tu carpeta de Drive.
2. Descífralo:
   ```powershell
   dotnet run -- restaurar ruta\al\archivo.dump.enc ruta\destino.dump
   ```
   (usa la misma `ISARMIN_BACKUP_ENCRYPTION_KEY` con la que se cifró)
3. Restaura en Postgres:
   ```powershell
   pg_restore -h localhost -U postgres -d isarmin_erp -c ruta\destino.dump
   ```

## Notas

- La rotación actual es simple: conserva los últimos `N` archivos subidos (por defecto 30 ≈ un mes si corre diario). Si más adelante la base de datos crece mucho (por las imágenes de productos en Base64), conviene medir el tamaño real de un dump y ajustar `ISARMIN_BACKUP_CANTIDAD_A_CONSERVAR` para no acercarse al límite gratuito de 15 GB de Drive.
- Este proyecto no depende de `ISARMIN.API` ni de `ISARMIN.Infrastructure` — es una herramienta de operaciones separada, no se despliega junto con el ERP.

## MSG Opener for Mac {{VERSION}} (versión de prueba / beta)

### Español
Visor gratuito de archivos `.msg` de Outlook para macOS 12 o superior. No necesita Outlook.

**Qué descargar**
- Mac con **chip Apple** (M1, M2, M3…): `MSGOpener-Mac-{{VERSION}}-arm64.dmg`
- Mac con **procesador Intel**: `MSGOpener-Mac-{{VERSION}}-x64.dmg`
- Si no sabés cuál tenés: menú Apple > **Acerca de esta Mac**.

**Instalación**
1. Abrí el `.dmg` y arrastrá **MSG Opener** a la carpeta **Aplicaciones**.
2. Abrí MSG Opener desde Aplicaciones. La primera vez macOS va a bloquearlo (ver abajo).

**Si macOS dice que no se puede abrir o que "está dañada"**
La app todavía no está firmada con una cuenta de desarrollador de Apple, por eso macOS la bloquea. Dos formas de permitirla:
1. Intentá abrirla una vez. Después andá a **Configuración del Sistema > Privacidad y seguridad**, bajá hasta el aviso sobre MSG Opener y tocá **Abrir de todas formas**.
2. O en la app **Terminal** ejecutá: `xattr -dr com.apple.quarantine "/Applications/MSG Opener.app"`

**Usarla para abrir `.msg` por defecto**
En Finder, seleccioná un archivo `.msg` > **Archivo > Obtener información** > **Abrir con** > elegí **MSG Opener** > **Cambiar todo…**

**Limitaciones de esta versión de prueba**
- El cuerpo del correo se muestra como texto. Para ver el formato original, tocá **Ver con formato en el navegador**.
- Todavía no tiene impresión.
- Puede tener errores: se probó poco. Si algo falla, avisá.

**Verificar el archivo (opcional)** — en Terminal: `shasum -a 256 <archivo>`
- arm64: `{{SHA_ARM64}}`
- x64: `{{SHA_X64}}`

### English
Free viewer for Outlook `.msg` files on macOS 12 or later. Outlook is not required.

**Which file to download**
- Mac with **Apple silicon** (M1, M2, M3…): `MSGOpener-Mac-{{VERSION}}-arm64.dmg`
- **Intel** Mac: `MSGOpener-Mac-{{VERSION}}-x64.dmg`
- Not sure? Apple menu > **About This Mac**.

**Install**
1. Open the `.dmg` and drag **MSG Opener** to **Applications**.
2. Open MSG Opener from Applications. The first time, macOS will block it (see below).

**If macOS says it cannot be opened or "is damaged"**
The app is not signed with an Apple developer account yet, so macOS blocks it. Two ways to allow it:
1. Try to open it once. Then go to **System Settings > Privacy & Security**, scroll to the message about MSG Opener and click **Open Anyway**.
2. Or in **Terminal** run: `xattr -dr com.apple.quarantine "/Applications/MSG Opener.app"`

**Make it the default for `.msg`**
In Finder, select a `.msg` file > **File > Get Info** > **Open with** > choose **MSG Opener** > **Change All…**

**Limitations of this beta**
- The email body is shown as text. To see the original formatting, click **View formatted in browser**.
- No printing yet.
- It has had little testing and may have bugs. Please report problems.

**Verify the file (optional)** — in Terminal: `shasum -a 256 <file>`
- arm64: `{{SHA_ARM64}}`
- x64: `{{SHA_X64}}`

---
Licencia y avisos / License and notices: `LICENSE.txt`, `DISCLAIMER.md`, `THIRD-PARTY-NOTICES.md`.
Creado por / Created by Federico Buraczewski.

## MSG Opener {{VERSION}}

### Español
Visor gratuito de archivos `.msg` de Outlook para Windows 10/11 (64 bits). No necesita Outlook.

**Instalación**
1. Descargá `MSGOpener-Setup-{{VERSION}}.exe` (más abajo, en *Assets*).
2. Ejecutalo y seguí el asistente (se muestra en el idioma de tu Windows).
3. Al final, confirmá MSG Opener como programa predeterminado para `.msg` en la ventana de Configuración que se abre.

**Si Windows muestra "Windows protegió su PC" (SmartScreen)**
El instalador todavía no está firmado digitalmente y es un programa nuevo, por eso Windows puede avisar. Para continuar:
1. Tocá **Más información**.
2. Tocá **Ejecutar de todas formas**.

Si tu navegador bloquea la descarga, elegí **Conservar** (Edge/Chrome) en el aviso de descarga.

**Verificar que el archivo es el original (opcional)**
En PowerShell: `Get-FileHash .\MSGOpener-Setup-{{VERSION}}.exe -Algorithm SHA256`
El resultado debe ser: `{{SHA256}}`

### English
Free viewer for Outlook `.msg` files on Windows 10/11 (64-bit). Outlook is not required.

**Install**
1. Download `MSGOpener-Setup-{{VERSION}}.exe` (below, under *Assets*).
2. Run it and follow the wizard (it uses your Windows language).
3. At the end, confirm MSG Opener as the default app for `.msg` in the Settings window that opens.

**If Windows shows "Windows protected your PC" (SmartScreen)**
The installer is not digitally signed yet and the program is new, so Windows may warn you. To continue:
1. Click **More info**.
2. Click **Run anyway**.

If your browser blocks the download, choose **Keep** in the download warning.

**Verify the file (optional)**
In PowerShell: `Get-FileHash .\MSGOpener-Setup-{{VERSION}}.exe -Algorithm SHA256`
It must match: `{{SHA256}}`

---
Licencia y avisos / License and notices: `LICENSE.txt`, `DISCLAIMER.md`, `THIRD-PARTY-NOTICES.md`.
Creado por / Created by Federico Buraczewski.

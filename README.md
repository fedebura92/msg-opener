# MSG Opener

Visor local de archivos Outlook `.msg` para Windows 10/11 x64. No requiere Outlook.
El contenido se procesa localmente.

## Generar el instalador (una sola vez por versión)

Requisitos en la PC donde compilás:

    winget install Microsoft.DotNet.SDK.8
    winget install JRSoftware.InnoSetup      (necesita Inno Setup 6.5 o superior)

Luego, en PowerShell, dentro de esta carpeta:

    ./build-windows.ps1 -Version 1.1.0

Resultado: `dist\MSGOpener-Setup-1.1.0.exe` (ese es el archivo que se distribuye).

Alternativa sin instalar nada: subir esta carpeta a GitHub y ejecutar el workflow
"Build installer" (Actions); deja el instalador como artefacto descargable.

## Qué hace el instalador

- Se muestra en el idioma de Windows (13 idiomas: español, inglés, alemán, francés,
  portugués de Brasil y de Portugal, chino simplificado, japonés, coreano, polaco, ruso,
  neerlandés e hindi). Si el idioma de Windows no está disponible, deja elegir.
- Instala para el usuario actual (sin pedir administrador) o para todos, a elección.
- Registra `.msg` y deja a MSG Opener como programa asociado (tarea marcada por defecto).
- Aparece en "Abrir con" y en Configuración > Aplicaciones predeterminadas.
- Al terminar ofrece abrir Configuración de Windows para confirmar el predeterminado.
- Detecta WebView2 Runtime y, si falta, lo descarga e instala.
- Desinstalación limpia desde "Aplicaciones instaladas".

### Instalación silenciosa (para despliegue)

    MSGOpener-Setup-1.1.0.exe /VERYSILENT /TASKS="associate"
    (para todos los usuarios: agregar /ALLUSERS)

## Nota sobre "predeterminado" en Windows 10/11

Windows protege la elección del usuario (UserChoice) y ningún instalador puede
forzarla. Si la PC no tiene Outlook ni otro programa asignado a `.msg`, el
doble clic ya abre MSG Opener. Si hay otro asignado, Windows pide confirmar
una vez: por eso el instalador abre la pantalla de Configuración al final.

## Configuración (botón ⚙ de la ventana principal)

- **Idioma:** automático (según Windows), Español, English, Deutsch, Français, Português,
  中文（简体）, 日本語, 한국어, Polski, Русский, Nederlands y हिन्दी. Cambia toda la interfaz,
  incluido el pie de página, y el formato de fecha.
- **Aspecto:** automático (sigue el modo claro/oscuro de Windows, también en vivo), claro u oscuro.
  En modo oscuro el cuerpo del correo se mantiene sobre fondo blanco para que se lea bien.
- Se guarda en `%LocalAppData%\MSGOpener\settings.json`.

## Cambios respecto a v0.1

- Logo en el encabezado, firma "Creado por Federico Buraczewski" al pie (traducida).
- Renombrado a MSG Opener, con ícono propio en el .exe, ventana e instalador.
- Corregido: el archivo recibido por doble clic se abría dos veces.
- Corregido: WebView2 guardaba datos junto al .exe (falla en Program Files);
  ahora usa `%LocalAppData%\MSGOpener`.
- Advertencia antes de abrir adjuntos ejecutables (.exe, .bat, .js, etc.).
- Instalador en español con logo, detección de WebView2 y registro completo.

## Pendiente de probar (no pude compilar en Windows)

- Compilación completa y prueba con `.msg` reales (imágenes `cid:`, adjuntos).
- Instalación/desinstalación en una PC limpia y en una con Outlook.

## Licencia

Copyright (c) 2026 Federico Buraczewski. Todos los derechos reservados.
Uso gratuito del programa; el código fuente no se puede copiar, modificar ni
redistribuir sin autorización. Ver `LICENSE.txt`, `DISCLAIMER.md` (sin garantías)
y `THIRD-PARTY-NOTICES.md` (componentes de terceros).

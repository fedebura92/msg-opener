#!/usr/bin/env bash
# Compila MSG Opener para macOS y genera un .dmg por arquitectura.
# Debe ejecutarse en una Mac (o en un runner macOS de GitHub Actions).
# Uso: ./scripts/build-mac.sh 0.1.0
set -euo pipefail

VERSION="${1:-0.1.0}"
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
OUT="$ROOT/dist-mac"

rm -rf "$OUT" "$ROOT/publish-mac"
mkdir -p "$OUT"

for RID in osx-arm64 osx-x64; do
  ARCH="${RID#osx-}"
  PUB="$ROOT/publish-mac/$RID"
  STAGE="$OUT/stage-$ARCH"
  APP="$STAGE/MSG Opener.app"

  echo "==> Publicando $RID ($VERSION)"
  dotnet publish "$ROOT/src-mac/MSGOpener.Mac.csproj" \
    -c Release -r "$RID" --self-contained true \
    -p:Version="$VERSION" -p:DebugType=None -p:DebugSymbols=false \
    -o "$PUB"

  echo "==> Armando MSG Opener.app ($ARCH)"
  mkdir -p "$APP/Contents/MacOS" "$APP/Contents/Resources"
  cp -R "$PUB/." "$APP/Contents/MacOS/"
  chmod +x "$APP/Contents/MacOS/MSGOpener"
  cp "$ROOT/src-mac/packaging/msg_opener.icns" "$APP/Contents/Resources/msg_opener.icns"
  sed "s/__VERSION__/$VERSION/g" "$ROOT/src-mac/packaging/Info.plist" > "$APP/Contents/Info.plist"

  # Textos legales dentro de la app.
  cp "$ROOT/LICENSE.txt" "$ROOT/DISCLAIMER.md" "$ROOT/THIRD-PARTY-NOTICES.md" "$APP/Contents/Resources/"

  # Firma "ad hoc" (sin certificado de Apple). Es necesaria para que la app
  # arranque en Macs con chip Apple, pero NO evita el aviso de Gatekeeper.
  echo "==> Firma ad hoc"
  codesign --force --deep --sign - "$APP"
  codesign --verify --deep --strict "$APP"

  ln -s /Applications "$STAGE/Applications"

  echo "==> Creando .dmg ($ARCH)"
  hdiutil create -volname "MSG Opener" -srcfolder "$STAGE" -ov -format UDZO \
    "$OUT/MSGOpener-Mac-$VERSION-$ARCH.dmg"

  rm -rf "$STAGE"
done

echo
echo "Listo:"
ls -la "$OUT"

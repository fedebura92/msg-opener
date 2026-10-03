; ============================================================
;  MSG Opener - Instalador (Inno Setup 6.5 o superior)
;  Compilar con:  ..\build-windows.ps1   (publica y compila)
; ============================================================
#ifndef MyAppVersion
  #define MyAppVersion "1.1.0"
#endif
#define MyAppName      "MSG Opener"
#define MyAppExeName   "MSGOpener.exe"
#define MyAppProgId    "MSGOpener.msg"
#define MyAppPublisher "MSG Opener"

[Setup]
; Mismo AppId que la v0.1 (MSG Viewer): al instalar actualiza esa versión.
AppId={{B6B3C87A-6A2C-4D6A-9E25-50A8D809D1D4}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
OutputDir=..\dist
OutputBaseFilename=MSGOpener-Setup-{#MyAppVersion}
SetupIconFile=..\src\Assets\msg_opener.ico
UninstallDisplayIcon={app}\{#MyAppExeName}
WizardStyle=modern
WizardImageFile=wizard_large.bmp
WizardSmallImageFile=wizard_small.bmp
Compression=lzma2
SolidCompression=yes
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
MinVersion=10.0
; Por defecto instala solo para el usuario (sin pedir administrador).
; El usuario puede elegir "para todos los usuarios" en un diálogo.
PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog
ChangesAssociations=yes
; Usa el idioma de Windows si está disponible; si no, deja elegir.
ShowLanguageDialog=auto
LanguageDetectionMethod=uilanguage
UsePreviousLanguage=yes
CloseApplications=yes
RestartApplications=no

[Languages]
Name: "english";             MessagesFile: "compiler:Default.isl"
Name: "spanish";             MessagesFile: "compiler:Languages\Spanish.isl"
Name: "german";              MessagesFile: "compiler:Languages\German.isl"
Name: "french";              MessagesFile: "compiler:Languages\French.isl"
Name: "brazilianportuguese"; MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"
Name: "portuguese";          MessagesFile: "compiler:Languages\Portuguese.isl"
Name: "chinesesimplified";   MessagesFile: "languages\ChineseSimplified.isl"
Name: "japanese";            MessagesFile: "compiler:Languages\Japanese.isl"
Name: "korean";              MessagesFile: "languages\Korean.isl"
Name: "polish";              MessagesFile: "compiler:Languages\Polish.isl"
Name: "russian";             MessagesFile: "compiler:Languages\Russian.isl"
Name: "dutch";               MessagesFile: "compiler:Languages\Dutch.isl"
Name: "hindi";               MessagesFile: "languages\Hindi.isl"

[Tasks]
Name: "associate"; Description: "{cm:AssocTask,{#MyAppName}}"; GroupDescription: "{cm:AssocGroup}"
Name: "desktopicon"; Description: "{cm:DesktopIconTask}"; GroupDescription: "{cm:ShortcutsGroup}"; Flags: unchecked

[Files]
Source: "..\publish\*"; DestDir: "{app}"; Excludes: "*.pdb"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Registry]
; --- Tipo de archivo (ProgID) ---
Root: HKA; Subkey: "Software\Classes\{#MyAppProgId}"; ValueType: string; ValueName: ""; ValueData: "{cm:FileTypeName}"; Flags: uninsdeletekey
Root: HKA; Subkey: "Software\Classes\{#MyAppProgId}\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\{#MyAppExeName},0"
Root: HKA; Subkey: "Software\Classes\{#MyAppProgId}\shell\open"; ValueType: string; ValueName: "FriendlyAppName"; ValueData: "{#MyAppName}"
Root: HKA; Subkey: "Software\Classes\{#MyAppProgId}\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\{#MyAppExeName}"" ""%1"""

; --- Aparece en "Abrir con" para .msg ---
Root: HKA; Subkey: "Software\Classes\.msg\OpenWithProgids"; ValueType: string; ValueName: "{#MyAppProgId}"; ValueData: ""; Flags: uninsdeletevalue

; --- Registro como aplicación (lista de "Abrir con" y Configuración) ---
Root: HKA; Subkey: "Software\Classes\Applications\{#MyAppExeName}"; ValueType: string; ValueName: "FriendlyAppName"; ValueData: "{#MyAppName}"; Flags: uninsdeletekey
Root: HKA; Subkey: "Software\Classes\Applications\{#MyAppExeName}\SupportedTypes"; ValueType: string; ValueName: ".msg"; ValueData: ""
Root: HKA; Subkey: "Software\Classes\Applications\{#MyAppExeName}\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\{#MyAppExeName}"" ""%1"""

; --- Capabilities: hace que aparezca en Configuración > Aplicaciones predeterminadas ---
Root: HKA; Subkey: "Software\{#MyAppName}\Capabilities"; ValueType: string; ValueName: "ApplicationName"; ValueData: "{#MyAppName}"; Flags: uninsdeletekey
Root: HKA; Subkey: "Software\{#MyAppName}\Capabilities"; ValueType: string; ValueName: "ApplicationDescription"; ValueData: "{cm:AppDescription}"
Root: HKA; Subkey: "Software\{#MyAppName}\Capabilities"; ValueType: string; ValueName: "ApplicationIcon"; ValueData: "{app}\{#MyAppExeName},0"
Root: HKA; Subkey: "Software\{#MyAppName}\Capabilities\FileAssociations"; ValueType: string; ValueName: ".msg"; ValueData: "{#MyAppProgId}"
Root: HKA; Subkey: "Software\RegisteredApplications"; ValueType: string; ValueName: "{#MyAppName}"; ValueData: "Software\{#MyAppName}\Capabilities"; Flags: uninsdeletevalue

; --- Asociación predeterminada (solo si se marca la tarea) ---
Root: HKA; Subkey: "Software\Classes\.msg"; ValueType: string; ValueName: ""; ValueData: "{#MyAppProgId}"; Flags: uninsdeletevalue; Tasks: associate

[Run]
; WebView2 Runtime (solo si no estaba instalado y se pudo descargar)
Filename: "{tmp}\MicrosoftEdgeWebview2Setup.exe"; Parameters: "/silent /install"; StatusMsg: "{cm:InstallingWebView2}"; Flags: waituntilterminated; Check: WebView2WasDownloaded
; Abrir Configuración de Windows para confirmar el programa predeterminado
Filename: "ms-settings:defaultapps?registeredAppUser=MSG%20Opener"; Description: "{cm:SetDefaultRun,{#MyAppName}}"; Flags: shellexec postinstall skipifsilent; Tasks: associate
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchApp,{#MyAppName}}"; Flags: nowait postinstall skipifsilent unchecked

[CustomMessages]
english.AssocTask=Use %1 to open .msg files (recommended)
english.AssocGroup=Outlook files:
english.DesktopIconTask=Create a desktop shortcut
english.ShortcutsGroup=Shortcuts:
english.InstallingWebView2=Installing Microsoft WebView2 Runtime...
english.SetDefaultRun=Choose %1 as the default app for .msg files (Windows Settings)
english.LaunchApp=Open %1
english.WebView2Failed=Microsoft WebView2 Runtime (needed to display emails) could not be downloaded. MSG Opener will be installed anyway; if it does not open, install WebView2 from:
english.FileTypeName=Outlook message (.msg)
english.AppDescription=Viewer for Outlook .msg files, no Outlook required

spanish.AssocTask=Usar %1 para abrir archivos .msg (recomendado)
spanish.AssocGroup=Archivos de Outlook:
spanish.DesktopIconTask=Crear un acceso directo en el escritorio
spanish.ShortcutsGroup=Accesos directos:
spanish.InstallingWebView2=Instalando Microsoft WebView2 Runtime...
spanish.SetDefaultRun=Elegir %1 como aplicación predeterminada para .msg (Configuración de Windows)
spanish.LaunchApp=Abrir %1
spanish.WebView2Failed=No se pudo descargar Microsoft WebView2 Runtime (necesario para mostrar los correos). Se instalará MSG Opener igualmente; si no abre, instalá WebView2 desde:
spanish.FileTypeName=Mensaje de Outlook (.msg)
spanish.AppDescription=Visor de archivos .msg de Outlook, sin necesidad de Outlook

german.AssocTask=%1 zum Öffnen von .msg-Dateien verwenden (empfohlen)
german.AssocGroup=Outlook-Dateien:
german.DesktopIconTask=Verknüpfung auf dem Desktop erstellen
german.ShortcutsGroup=Verknüpfungen:
german.InstallingWebView2=Microsoft WebView2 Runtime wird installiert...
german.SetDefaultRun=%1 als Standard-App für .msg-Dateien festlegen (Windows-Einstellungen)
german.LaunchApp=%1 öffnen
german.WebView2Failed=Microsoft WebView2 Runtime (zum Anzeigen von E-Mails erforderlich) konnte nicht heruntergeladen werden. MSG Opener wird trotzdem installiert. Falls es sich nicht öffnen lässt, installieren Sie WebView2 von:
german.FileTypeName=Outlook-Nachricht (.msg)
german.AppDescription=Viewer für Outlook-.msg-Dateien, ohne Outlook

french.AssocTask=Utiliser %1 pour ouvrir les fichiers .msg (recommandé)
french.AssocGroup=Fichiers Outlook :
french.DesktopIconTask=Créer un raccourci sur le Bureau
french.ShortcutsGroup=Raccourcis :
french.InstallingWebView2=Installation de Microsoft WebView2 Runtime...
french.SetDefaultRun=Choisir %1 comme application par défaut pour les fichiers .msg (Paramètres Windows)
french.LaunchApp=Ouvrir %1
french.WebView2Failed=Microsoft WebView2 Runtime (nécessaire pour afficher les e-mails) n’a pas pu être téléchargé. MSG Opener sera installé malgré tout ; s’il ne s’ouvre pas, installez WebView2 depuis :
french.FileTypeName=Message Outlook (.msg)
french.AppDescription=Visionneuse de fichiers .msg d’Outlook, sans Outlook

brazilianportuguese.AssocTask=Usar o %1 para abrir arquivos .msg (recomendado)
brazilianportuguese.AssocGroup=Arquivos do Outlook:
brazilianportuguese.DesktopIconTask=Criar um atalho na área de trabalho
brazilianportuguese.ShortcutsGroup=Atalhos:
brazilianportuguese.InstallingWebView2=Instalando o Microsoft WebView2 Runtime...
brazilianportuguese.SetDefaultRun=Definir o %1 como aplicativo padrão para arquivos .msg (Configurações do Windows)
brazilianportuguese.LaunchApp=Abrir o %1
brazilianportuguese.WebView2Failed=Não foi possível baixar o Microsoft WebView2 Runtime (necessário para exibir os e-mails). O MSG Opener será instalado mesmo assim; se não abrir, instale o WebView2 em:
brazilianportuguese.FileTypeName=Mensagem do Outlook (.msg)
brazilianportuguese.AppDescription=Visualizador de arquivos .msg do Outlook, sem precisar do Outlook

portuguese.AssocTask=Utilizar o %1 para abrir ficheiros .msg (recomendado)
portuguese.AssocGroup=Ficheiros do Outlook:
portuguese.DesktopIconTask=Criar um atalho no ambiente de trabalho
portuguese.ShortcutsGroup=Atalhos:
portuguese.InstallingWebView2=A instalar o Microsoft WebView2 Runtime...
portuguese.SetDefaultRun=Definir o %1 como aplicação predefinida para ficheiros .msg (Definições do Windows)
portuguese.LaunchApp=Abrir o %1
portuguese.WebView2Failed=Não foi possível transferir o Microsoft WebView2 Runtime (necessário para mostrar os e-mails). O MSG Opener será instalado na mesma; se não abrir, instale o WebView2 a partir de:
portuguese.FileTypeName=Mensagem do Outlook (.msg)
portuguese.AppDescription=Visualizador de ficheiros .msg do Outlook, sem necessitar do Outlook

chinesesimplified.AssocTask=使用 %1 打开 .msg 文件（推荐）
chinesesimplified.AssocGroup=Outlook 文件：
chinesesimplified.DesktopIconTask=创建桌面快捷方式
chinesesimplified.ShortcutsGroup=快捷方式：
chinesesimplified.InstallingWebView2=正在安装 Microsoft WebView2 Runtime...
chinesesimplified.SetDefaultRun=将 %1 设为 .msg 文件的默认应用（Windows 设置）
chinesesimplified.LaunchApp=打开 %1
chinesesimplified.WebView2Failed=无法下载 Microsoft WebView2 Runtime（显示邮件所必需）。仍将安装 MSG Opener；如果无法打开，请从以下地址安装 WebView2：
chinesesimplified.FileTypeName=Outlook 邮件 (.msg)
chinesesimplified.AppDescription=Outlook .msg 文件查看器，无需安装 Outlook

japanese.AssocTask=.msg ファイルを %1 で開く（推奨）
japanese.AssocGroup=Outlook ファイル:
japanese.DesktopIconTask=デスクトップにショートカットを作成する
japanese.ShortcutsGroup=ショートカット:
japanese.InstallingWebView2=Microsoft WebView2 Runtime をインストールしています...
japanese.SetDefaultRun=%1 を .msg ファイルの既定のアプリに設定する (Windows の設定)
japanese.LaunchApp=%1 を開く
japanese.WebView2Failed=Microsoft WebView2 Runtime（メールの表示に必要）をダウンロードできませんでした。MSG Opener はこのままインストールされます。開けない場合は、次の場所から WebView2 をインストールしてください:
japanese.FileTypeName=Outlook メッセージ (.msg)
japanese.AppDescription=Outlook の .msg ファイルビューア（Outlook 不要）

korean.AssocTask=.msg 파일을 %1(으)로 열기 (권장)
korean.AssocGroup=Outlook 파일:
korean.DesktopIconTask=바탕 화면에 바로 가기 만들기
korean.ShortcutsGroup=바로 가기:
korean.InstallingWebView2=Microsoft WebView2 Runtime을 설치하는 중...
korean.SetDefaultRun=%1을(를) .msg 파일의 기본 앱으로 선택 (Windows 설정)
korean.LaunchApp=%1 열기
korean.WebView2Failed=Microsoft WebView2 Runtime(메일 표시에 필요)을 다운로드하지 못했습니다. MSG Opener는 그대로 설치됩니다. 열리지 않으면 다음 위치에서 WebView2를 설치하세요:
korean.FileTypeName=Outlook 메시지 (.msg)
korean.AppDescription=Outlook .msg 파일 뷰어 (Outlook 불필요)

polish.AssocTask=Używaj programu %1 do otwierania plików .msg (zalecane)
polish.AssocGroup=Pliki programu Outlook:
polish.DesktopIconTask=Utwórz skrót na pulpicie
polish.ShortcutsGroup=Skróty:
polish.InstallingWebView2=Instalowanie Microsoft WebView2 Runtime...
polish.SetDefaultRun=Ustaw %1 jako domyślną aplikację dla plików .msg (Ustawienia systemu Windows)
polish.LaunchApp=Otwórz %1
polish.WebView2Failed=Nie udało się pobrać Microsoft WebView2 Runtime (wymagany do wyświetlania wiadomości). Program MSG Opener zostanie mimo to zainstalowany; jeśli się nie uruchomi, zainstaluj WebView2 z:
polish.FileTypeName=Wiadomość programu Outlook (.msg)
polish.AppDescription=Przeglądarka plików .msg programu Outlook, bez potrzeby posiadania Outlooka

russian.AssocTask=Использовать %1 для открытия файлов .msg (рекомендуется)
russian.AssocGroup=Файлы Outlook:
russian.DesktopIconTask=Создать ярлык на рабочем столе
russian.ShortcutsGroup=Ярлыки:
russian.InstallingWebView2=Установка Microsoft WebView2 Runtime...
russian.SetDefaultRun=Назначить %1 приложением по умолчанию для файлов .msg (Параметры Windows)
russian.LaunchApp=Открыть %1
russian.WebView2Failed=Не удалось скачать Microsoft WebView2 Runtime (необходим для отображения писем). MSG Opener всё равно будет установлен; если он не запустится, установите WebView2 отсюда:
russian.FileTypeName=Сообщение Outlook (.msg)
russian.AppDescription=Просмотр файлов .msg из Outlook, Outlook не требуется

dutch.AssocTask=%1 gebruiken om .msg-bestanden te openen (aanbevolen)
dutch.AssocGroup=Outlook-bestanden:
dutch.DesktopIconTask=Een snelkoppeling op het bureaublad maken
dutch.ShortcutsGroup=Snelkoppelingen:
dutch.InstallingWebView2=Microsoft WebView2 Runtime wordt geïnstalleerd...
dutch.SetDefaultRun=%1 instellen als standaard-app voor .msg-bestanden (Windows-instellingen)
dutch.LaunchApp=%1 openen
dutch.WebView2Failed=Microsoft WebView2 Runtime (nodig om e-mails weer te geven) kon niet worden gedownload. MSG Opener wordt toch geïnstalleerd; als het niet opent, installeer WebView2 dan via:
dutch.FileTypeName=Outlook-bericht (.msg)
dutch.AppDescription=Viewer voor Outlook .msg-bestanden, zonder Outlook

hindi.AssocTask=.msg फ़ाइलें खोलने के लिए %1 का उपयोग करें (अनुशंसित)
hindi.AssocGroup=Outlook फ़ाइलें:
hindi.DesktopIconTask=डेस्कटॉप पर शॉर्टकट बनाएँ
hindi.ShortcutsGroup=शॉर्टकट:
hindi.InstallingWebView2=Microsoft WebView2 Runtime इंस्टॉल हो रहा है...
hindi.SetDefaultRun=%1 को .msg फ़ाइलों के लिए डिफ़ॉल्ट ऐप चुनें (Windows सेटिंग्स)
hindi.LaunchApp=%1 खोलें
hindi.WebView2Failed=Microsoft WebView2 Runtime (ईमेल दिखाने के लिए आवश्यक) डाउनलोड नहीं हो सका। MSG Opener फिर भी इंस्टॉल किया जाएगा; यदि यह न खुले, तो WebView2 यहाँ से इंस्टॉल करें:
hindi.FileTypeName=Outlook संदेश (.msg)
hindi.AppDescription=Outlook .msg फ़ाइलों का व्यूअर, Outlook की आवश्यकता नहीं

[Code]
const
  WebView2Guid = '{F3017226-FE2A-4295-8BDF-00C3A9A7E4C5}';

var
  DownloadPage: TDownloadWizardPage;
  WebView2Downloaded: Boolean;

function IsWebView2Installed: Boolean;
var
  Version: String;
begin
  Result :=
    (RegQueryStringValue(HKLM32, 'SOFTWARE\Microsoft\EdgeUpdate\Clients\' + WebView2Guid, 'pv', Version) or
     RegQueryStringValue(HKCU, 'Software\Microsoft\EdgeUpdate\Clients\' + WebView2Guid, 'pv', Version))
    and (Version <> '') and (Version <> '0.0.0.0');
end;

function WebView2WasDownloaded: Boolean;
begin
  Result := WebView2Downloaded;
end;

function OnDownloadProgress(const Url, FileName: String; const Progress, ProgressMax: Int64): Boolean;
begin
  Result := True;
end;

procedure InitializeWizard;
begin
  DownloadPage := CreateDownloadPage(SetupMessage(msgWizardPreparing), SetupMessage(msgPreparingDesc), @OnDownloadProgress);
end;

function NextButtonClick(CurPageID: Integer): Boolean;
begin
  Result := True;
  if (CurPageID = wpReady) and (not IsWebView2Installed) then
  begin
    DownloadPage.Clear;
    DownloadPage.Add('https://go.microsoft.com/fwlink/p/?LinkId=2124703', 'MicrosoftEdgeWebview2Setup.exe', '');
    DownloadPage.Show;
    try
      try
        DownloadPage.Download;
        WebView2Downloaded := True;
      except
        WebView2Downloaded := False;
        SuppressibleMsgBox(
          CustomMessage('WebView2Failed') + #13#10 +
          'https://developer.microsoft.com/microsoft-edge/webview2/',
          mbInformation, MB_OK, IDOK);
      end;
    finally
      DownloadPage.Hide;
    end;
  end;
end;

; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{8C88D80A-F9F9-4DEF-8510-DA9E438E729A}
AppName=C# Comicviewer
AppVersion=V1.2 DEV
AppPublisherURL=http://sourceforge.net/projects/csharpcomicview
AppSupportURL=http://sourceforge.net/projects/csharpcomicview/support
AppUpdatesURL=http://sourceforge.net/projects/csharpcomicview
DefaultDirName={pf}\C# Comicviewer
DefaultGroupName=C# Comicviewer
LicenseFile=C:\Users\Revvion\Documents\C# projects\csharp-comicviewer\Licence.txt
OutputDir=C:\Users\Revvion\Documents\C# projects\csharp-comicviewer\
OutputBaseFilename=setup
Compression=lzma
SolidCompression=yes
ChangesAssociations=yes
ArchitecturesInstallIn64BitMode=x64

[Registry]
Root: HKCR; Subkey: ".CBR"; ValueType: string; ValueName: ""; ValueData: "ComicBookRarFile"; Flags: uninsdeletevalue
Root: HKCR; Subkey: "ComicBookRarFile"; ValueType: string; ValueName: ""; ValueData: "Comic Book Rar File"; Flags: uninsdeletekey
Root: HKCR; Subkey: "ComicBookRarFile\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\csharp-comicviewer.exe,0"
Root: HKCR; Subkey: "ComicBookRarFile\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\csharp-comicviewer.exe"" ""%1"""
Root: HKCR; Subkey: ".CBZ"; ValueType: string; ValueName: ""; ValueData: "ComicBookZipFile"; Flags: uninsdeletevalue
Root: HKCR; Subkey: "ComicBookRarFile"; ValueType: string; ValueName: ""; ValueData: "Comic Book Zip File"; Flags: uninsdeletekey
Root: HKCR; Subkey: "ComicBookZipFile\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\csharp-comicviewer.exe,0"
Root: HKCR; Subkey: "ComicBookZipFile\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\csharp-comicviewer.exe"" ""%1"""

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[CustomMessages]
InstallDotNet = 'Dot.Net 4.0 required, would you like to download this now?'

[Code]
const
dotnetfx40_url = 'http://download.microsoft.com/download/9/5/A/95A9616B-7A37-4AF6-BC36-D6EA96C8DAAE/dotNetFx40_Full_x86_x64.exe';

function InitializeSetup(): Boolean;
var
  msgRes : integer;
  errCode : integer;

begin
  Result := true;
  // Check for required dotnetfx 4.0 installation

  if (not RegKeyExists(HKLM, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\full')) then begin
    msgRes := MsgBox(CustomMessage('InstallDotNet'), mbError, MB_OKCANCEL);
    if(msgRes = 1) then begin
      ShellExec('Open', dotnetfx40_url, '', '', SW_SHOW, ewNoWait, errCode);
    end;
    Abort();
  end;
end;

[Files]
Source: "C:\Users\Revvion\Documents\C# projects\csharp-comicviewer\csharp-comicviewer\bin\Release\csharp-comicviewer.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\Revvion\Documents\C# projects\csharp-comicviewer\x64\7z.dll"; DestDir: "{app}"; Flags: ignoreversion; Check: Is64BitInstallMode
Source: "C:\Users\Revvion\Documents\C# projects\csharp-comicviewer\x86\7z.dll"; DestDir: "{app}"; Flags: ignoreversion; Check: not Is64BitInstallMode
Source: "C:\Users\Revvion\Documents\C# projects\csharp-comicviewer\csharp-comicviewer\bin\Release\SevenZipSharp.dll"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{group}\C# Comicviewer"; Filename: "{app}\csharp-comicviewer.exe"
Name: "{group}\{cm:ProgramOnTheWeb,C# Comicviewer}"; Filename: "http://sourceforge.net/projects/csharpcomicview"
Name: "{group}\{cm:UninstallProgram,C# Comicviewer}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\C# Comicviewer"; Filename: "{app}\csharp-comicviewer.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\csharp-comicviewer.exe"; Description: "{cm:LaunchProgram,C# Comicviewer}"; Flags: nowait postinstall skipifsilent

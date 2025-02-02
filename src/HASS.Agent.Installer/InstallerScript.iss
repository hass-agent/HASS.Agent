; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!
; Modified to suit HASS.Agent requirements :)

; InnoDependencyInstaller
; Thanks to https://github.com/DomGries/InnoDependencyInstaller for the amazing work!
#define public Dependency_Path_NetCoreCheck "dependencies\"
#include "CodeDependencies.iss"

; Standard installation constants
#define MyAppName "HASS.Agent"
#define MyAppVersion "2.1.1"
#define MyAppPublisher "HASS.Agent Team"
#define MyAppURL "https://hass-agent.io"
#define MyAppExeName "HASS.Agent.exe"

[Setup]
ArchitecturesInstallIn64BitMode=x64
SetupMutex=Global\HASS.Agent.Setup.Mutex,HASS.Agent.Setup.Mutex
AppMutex=HASS.Agent.App.Mutex
AppId={{7BBED458-609B-4D13-AD9E-4FF219DF8644}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={localappdata}\{#MyAppName}\Client
DisableProgramGroupPage=yes
LicenseFile=..\..\LICENSE.md
InfoBeforeFile=.\BeforeInstallNotice.rtf
InfoAfterFile=.\AfterInstallNotice.rtf
; Uncomment the following line to run in non administrative install mode (install for current user only.)
PrivilegesRequired=lowest
OutputDir=.\bin
OutputBaseFilename=HASS.Agent.Installer
SetupIconFile=..\HASS.Agent\HASS.Agent.Shared\hassagent.ico
Compression=lzma
SolidCompression=yes
WizardStyle=modern
CloseApplications=force
CloseApplicationsFilter=*.*
UninstallDisplayIcon={app}\{#MyAppExeName}
UninstallDisplayName={#MyAppName} {#MyAppVersion}

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

; NOTE: Don't use "Flags: ignoreversion" on any shared system files
[Files]
; Client files
Source: "..\HASS.Agent\HASS.Agent\bin\Publish-x64\Release\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
; Service installer
Source: ".\bin\HASS.Agent.Service.Installer.exe"; DestDir: "{tmp}"; Flags: ignoreversion

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Parameters: "compat_migrate"; Description: "Try to migrate configuration - use only once (administrative permissions required)"; Flags: postinstall skipifsilent runascurrentuser unchecked
Filename: "{tmp}\HASS.Agent.Service.Installer.exe"; Description: "Install Satellite Service (administrative permissions required)"; Flags: postinstall runascurrentuser 
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: postinstall skipifsilent nowait

[Code]
function InitializeSetup: Boolean;
begin
  Dependency_ForceX86 := False;
  Dependency_AddDotNet60Desktop;
  Result := True;
end;

function NextButtonClick(CurPageID: Integer): Boolean;
var
  Msg: string;
begin
  if CurPageID = wpFinished then
  begin
    if WizardForm.RunList.Checked[0] then
    begin;
      Msg := 'Migration requires original HASS.Agent and Satellite Service to be stopped, do you wish to proceed?';
      Result := (MsgBox(Msg, mbConfirmation, MB_YESNO) = IDYES)
    end
      else
    begin
      Result := True;  
    end
  end
    else
  begin
    Result := True;
  end
end;

procedure CurUninstallStepChanged (CurUninstallStep: TUninstallStep);
var
    mres : integer;
    serviceUninstallerPath : String;
    ResultCode : integer;
begin
  case CurUninstallStep of                   
    usPostUninstall:
      begin
        mres := MsgBox('Do you want to uninstall the Satellite Service? (administrative permissions required)', mbConfirmation, MB_YESNO or MB_DEFBUTTON2)
        if mres = IDYES then
          RegQueryStringValue(HKLM, 'SOFTWARE\HASSAgent\SatelliteService', 'InstallPath', serviceUninstallerPath);
          Exec(serviceUninstallerPath + '\unins000.exe', '', '', SW_SHOW, ewWaitUntilTerminated, ResultCode);
      end;
  end;
end;

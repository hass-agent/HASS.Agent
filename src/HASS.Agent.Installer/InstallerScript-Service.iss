; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!
; Modified to suit HASS.Agent requirements :)

; InnoDependencyInstaller
; Thanks to https://github.com/DomGries/InnoDependencyInstaller for the amazing work!
#define public Dependency_Path_NetCoreCheck "dependencies\"
#include "CodeDependencies.iss"

; Standard installation constants
#define MyAppName "HASS.Agent Satellite Service"
#define MyAppVersion "2.1.1"
#define MyAppPublisher "HASS.Agent Team"
#define MyAppURL "https://hass-agent.io"
#define MyAppExeName "HASS.Agent.Satellite.Service.exe"
#define ServiceName "hass.agent.svc"
#define ServiceDisplayName "HASS.Agent - Satellite Service" 
#define ServiceDescription "Satellite service for HASS.Agent: a Windows based Home Assistant client. This service processes commands and sensors without the requirement of a logged-in user."

[Setup]
ArchitecturesInstallIn64BitMode=x64
;SetupMutex=Global\HASS.Agent.Setup.Satellite.Mutex,HASS.Agent.Satellite.Setup.Mutex
AppMutex=Global\\HASS.Agent.Service.Mutex
AppId={{4004588E-F411-41C2-ABD8-A898B0A14B93}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={commonpf64}\{#MyAppName}\Service
DisableProgramGroupPage=yes
LicenseFile=..\..\LICENSE.md
InfoBeforeFile=.\BeforeInstallNotice-Service.rtf
InfoAfterFile=.\AfterInstallNotice-Service.rtf
PrivilegesRequired=admin
OutputDir=.\bin
OutputBaseFilename=HASS.Agent.Service.Installer
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

; NOTE: Don't use "Flags: ignoreversion" on any shared system files
[Files]
; Service files
Source: "..\HASS.Agent\HASS.Agent.Satellite.Service\bin\Publish-x64\Release\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Run]
Filename: "{sys}\sc.exe"; Parameters: "start {#ServiceName}"; Description: "Start Satellite Service"; Flags: postinstall runhidden runascurrentuser 

[Registry]
Root: HKLM; Subkey: "SOFTWARE\HASSAgent\SatelliteService"; ValueType: string; ValueName: "InstallPath"; ValueData: "{app}"; Flags: createvalueifdoesntexist uninsdeletevalue

; Service registration and removal
[Run]
Filename: "{sys}\sc.exe"; Parameters: "create {#ServiceName} binpath= ""\""{app}\{#MyAppExeName}""\"""; Flags: runhidden 
Filename: "{sys}\sc.exe"; Parameters: "failure {#ServiceName} reset= 86400 actions= restart/60000/restart/60000//1000"; Flags: runhidden 
Filename: "{sys}\sc.exe"; Parameters: "description {#ServiceName} ""{#ServiceDescription}"""; Flags: runhidden 
Filename: "{sys}\sc.exe"; Parameters: "config {#ServiceName} DisplayName= ""{#ServiceDisplayName}"""; Flags: runhidden
Filename: "{sys}\sc.exe"; Parameters: "config {#ServiceName} start= auto"; Flags: runhidden
Filename: "{sys}\sc.exe"; Parameters: "config {#ServiceName} binpath= ""\""{app}\{#MyAppExeName}""\"""; Flags: runhidden 
[UninstallRun]
Filename: "{sys}\sc.exe"; Parameters: "stop {#ServiceName}"; RunOnceId: StopService; Flags: runhidden
Filename: "{sys}\timeout.exe"; Parameters: "5"; RunOnceId: Delay1; Flags:runhidden
Filename: "{sys}\sc.exe"; Parameters: "delete {#ServiceName}" ; RunOnceId: DeleteService; Flags: runhidden
Filename: "{sys}\timeout.exe"; Parameters: "5"; RunOnceId: Delay2; Flags:runhidden

[Code]
function InitializeSetup: Boolean;
begin
  Dependency_ForceX86 := False;
  Dependency_AddDotNet60Desktop;
  Result := True;
end;

procedure CurStepChanged(CurStep: TSetupStep);
var 
  ProgressPage: TOutputProgressWizardPage;
  I, Step, Wait, ResultCode: Integer;
begin
  if CurStep = ssInstall then
  begin
    Exec(ExpandConstant('{sys}') + '\sc.exe', 'stop ' + ExpandConstant('{#ServiceName}'), '', SW_HIDE, ewWaitUntilTerminated, ResultCode)

    //thanks to https://stackoverflow.com/a/39827761
    Wait := 5000;
    Step := 100;
    ProgressPage :=
      CreateOutputProgressPage(
        WizardForm.PageNameLabel.Caption,
        WizardForm.PageDescriptionLabel.Caption);
    ProgressPage.SetText('Making sure the satellite service is stopped...', '');
    ProgressPage.SetProgress(0, Wait);
    ProgressPage.Show;
    try
      for I := 0 to Wait div Step do
      begin
        ProgressPage.SetProgress(I * Step, Wait);
        Sleep(Step);
      end;
    finally
      ProgressPage.Hide;
      ProgressPage.Free;
    end;
  end;
end;
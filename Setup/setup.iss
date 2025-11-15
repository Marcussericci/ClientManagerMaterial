[Setup]
AppName=Client Manager
AppVersion=1.0
AppPublisher=Your Company
DefaultDirName={autopf}\Client Manager
DefaultGroupName=Client Manager
OutputDir=..\Output
OutputBaseFilename=ClientManagerSetup
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Files]
Source: "..\publish_simple\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs

[Icons]
Name: "{group}\Client Manager"; Filename: "{app}\ClientManagerMaterial.exe"
Name: "{autodesktop}\Client Manager"; Filename: "{app}\ClientManagerMaterial.exe"

[Run]
Filename: "{app}\ClientManagerMaterial.exe"; Description: "Launch Client Manager"; Flags: nowait postinstall skipifsilent
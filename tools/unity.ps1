param([ValidateSet('Setup','Validate','BuildQuest')][string]$Action='Validate', [string]$Editor='C:\Program Files\Unity\Hub\Editor\6000.6.7f1\Editor\Unity.exe')
$ErrorActionPreference='Stop'
$projectRoot=Split-Path $PSScriptRoot -Parent
if(!(Test-Path -LiteralPath $Editor)){throw "Unity editor missing: $Editor"}
$logRoot=Join-Path $projectRoot '.local'
New-Item -ItemType Directory -Force -Path $logRoot | Out-Null
$logPath=Join-Path $logRoot "$Action.log"
$arguments=@('-batchmode','-nographics','-quit','-projectPath',('"'+$projectRoot+'"'),'-executeMethod',"BWVR.Editor.ProjectSetup.$Action",'-logFile',('"'+$logPath+'"'))
$process=Start-Process -FilePath $Editor -ArgumentList $arguments -PassThru -WindowStyle Hidden
$process.WaitForExit()
Get-Content -LiteralPath $logPath -Tail 40
if($process.ExitCode -ne 0){throw "Unity $Action failed ($($process.ExitCode)); see $logPath"}

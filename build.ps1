param(
	[int]$buildNumber = 0
	)
Import-Module .\src\packages\psake.4.2.0.1\tools\psake.psm1
Import-Module .\BuildFunctions.psm1
Invoke-Psake .\default.ps1 default -framework "4.0x64" -properties @{ buildNumber=$buildNumber }
Remove-Module BuildFunctions
Remove-Module psake
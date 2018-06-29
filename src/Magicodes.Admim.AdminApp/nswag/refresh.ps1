$invocation = (Get-Variable MyInvocation).Value
$directorypath = Split-Path $invocation.MyCommand.Path
Set-Location $directorypath
$cmdString = """" + "..\node_modules\.bin\nswag" + """" + " run";
Write-Host 'cmd:' $cmdString
cmd /c $cmdString
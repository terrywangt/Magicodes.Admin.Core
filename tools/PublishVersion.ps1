#脚本运行目录
$x = $MyInvocation.MyCommand.Definition
$x = Split-Path -Parent $MyInvocation.MyCommand.Definition

$rootSource=Split-Path -Parent $x
#源代码目录
$rootSource=$rootSource+"\Src";

#临时目录
$definitionDir=$x+"\temp";
#待复制的目录
$sourceDir=$x+"\\Magicodes.WeiChat";
#忽略列表配置文件
$ignoreListFilePath=$x+"\PublishVersion_IgnoreList.txt";
#创建忽略文件列表
$ignoreList = New-Object 'System.Collections.ObjectModel.Collection`1[System.String]'
Get-Content $ignoreListFilePath | %{ $ignoreList.Add($_); } 
Write-Host $ignoreList


#移除临时文件夹中所有项
if([io.Directory]::Exists($definitionDir))
{
    Remove-Item  $definitionDir -Recurse  -Force
    Write-Host "临时目录文件删除完毕。"
}
New-Item -path $x -name temp -type directory
Write-Host "已创建临时目录。"

#TODO:编译不通过则不复制
#解决方案名称
#$slnPath=$x+"//QL.ClinicalTrials.sln"
#编译解决方案
#C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe $slnPath /t:Build 

Copy-Item -Path  $sourceDir  -Destination $definitionDir  -Recurse  -Force

#将符合条件的目录或文件
dir $definitionDir -Recurse | 
        Where-Object { ($_.Name -eq "bin") -or ($_.Name -eq "obj") -or ($_.Name -eq "packages") -or ($_.Name -eq "logs") -or ($_.Name -match "^*.ldf$")  -or ($_.Name -match "^*.mdf$") -or ($_.Name -eq "TestResults") -or ($_.Name -eq "MediaFiles") -or ($_.Name -eq "upload") -or ($_.Name -eq "PublishProfiles") }|
        ForEach-Object  {  Write-Host $_.FullName;remove-item -Path $_.FullName -Recurse -Force ;} |
        Select-Object -Property Name

Write-Host "目录删除完成。"

#weichat
$webWeiChat=$x+"\Import\Magicodes.WeiChat\*";
$desWebWeiChat=$definitionDir+"\Magicodes.WeiChat\Magicodes.WeiChat"
Copy-Item -Path  $webWeiChat  -Destination $desWebWeiChat  -Recurse  -Force
Write-Host "Magicodes.WeiChat复制完成。"


$root=$x+"\Import\root\*";
$desRoote=$definitionDir+"\Magicodes.WeiChat"
Copy-Item -Path  $root  -Destination $desRoote  -Recurse  -Force
Write-Host "根目录复制完成。"

function prompt { '心莱科技: ' + (get-location) + '> '}

$invocation = (Get-Variable MyInvocation).Value
$directorypath = Split-Path $invocation.MyCommand.Path
$rootDir = [io.Directory]::GetParent($directorypath);

$version = Read-Host "请输入版本号"
Write-Warning "版本号为：$version"
$target = [io.Path]::Combine($rootDir, "publish", "Magicodes.Admin.Core_$version")

Write-Warning "目标目录为：$target"
#创建目标目录
if (![io.Directory]::Exists($target)) {
    [io.Directory]::CreateDirectory($target)
    Write-Host "目标目录创建成功！"
}
else {
    Write-Error "目录存在，操作已取消。$target";
    return;
}


$root = $rootDir.FullName;
dir $root | 
    Where-Object { 'publish', 'res', 'Magicodes.Admin源码高级版授权合同.doc', 'Magicodes.Admin源码基础版授权合同.doc' -notcontains $_.Name}|
    ForEach-Object {  
    Write-Host $_.FullName;
    Copy-Item -Path  $_.FullName  -Destination $target  -Recurse  -Force -Exclude  'bin', 'obj'
}
Select-Object -Property Name

$zipPath = [io.Path]::Combine($rootDir, "publish", "Magicodes.Admin.Core_$version.zip");
Compress-Archive -Path $target -DestinationPath $zipPath
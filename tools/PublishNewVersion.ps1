Param (
    ##源目录
    [string]$srcs = "E:\workspace\Magicodes.Admin.Core\src,E:\workspace\Magicodes.Admin.Core\plus,E:\workspace\Magicodes.Admin.Core\test,E:\workspace\Magicodes.Admin.Core\.gitattributes,E:\workspace\Magicodes.Admin.Core\.gitignore,E:\workspace\Magicodes.Admin.Core\common.props,E:\workspace\Magicodes.Admin.Core\Magicodes.Admin.sln,E:\workspace\Magicodes.Admin.Core\migration.bat,E:\workspace\Magicodes.Admin.Core\openPlusDirectory.bat,E:\workspace\Magicodes.Admin.Core\tools\tools",
    [string]$import="E:\workspace\Magicodes.Admin.Core\Tools\Import\*",
    [string]$target="E:\workspace\Magicodes.Admin.Core\publish",
    [string]$documentsDir="E:\Documents\xinlai\产品文档\Magicodes.Admin\Documents\*"
)
if(![io.Directory]::Exists($target))
{
    [io.Directory]::CreateDirectory($target)
}

$version = Read-Host "请输入版本号"
Write-Warning "版本号为：$version"
$targetDir=[io.Path]::Combine($target,"Magicodes.Admin_$version")

Write-Warning "目标目录为：$targetDir"
#创建目标目录
if(![io.Directory]::Exists($targetDir))
{
    [io.Directory]::CreateDirectory($targetDir)
    Write-Host "目标目录创建成功！"
}else
{
    Write-Error "目录存在，操作已取消。$targetDir";
    return;
}

$srcPaths=$srcs.Split(',');
foreach ($src in $srcPaths)
{
   if(![io.Directory]::Exists($src) -and ![io.File]::Exists($src))
   {
       Write-Error "路径不存在：$src";
       return;
   }
   #开始复制
   Copy-Item -Path  $src  -Destination $targetDir  -Recurse  -Force
   Write-Host "$src to $targetDir 复制完成！"
}

#清理内容
dir $targetDir -Recurse | 
        Where-Object { ($_.Name -eq "bin") -or ($_.Name -eq "obj")  -or ($_.Name -eq "logs") -or ($_.Name -match "^*.ldf$")  -or ($_.Name -match "^*.mdf$") -or ($_.Name -eq "TestResults") -or ($_.Name -eq "MediaFiles") -or ($_.Name -eq "upload") -or ($_.Name -eq "PublishProfiles")  -or ($_.Name -eq "QrCode") -or ($_.Name -eq ".vs") }|
        ForEach-Object  {  
                            if($_.FullName.EndsWith("bin") -and $_.FullName.Contains("\wwwroot\"))
                            {
                                
                            }else
                            {
                                Write-Host $_.FullName;
                                remove-item -Path $_.FullName -Recurse -Force ;
                            }
                         } |
        Select-Object -Property Name
Write-Host "已成功清理相关内容！"
#导入文件
Write-Host "导入配置文件..."
Copy-Item -Path  $import  -Destination $targetDir  -Recurse  -Force
Write-Host "导入文档..."
Copy-Item -Path  $documentsDir  -Destination $targetDir  -Recurse  -Force
Write-Host "已成功导入相关文件！"



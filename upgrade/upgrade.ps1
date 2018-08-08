# 本脚本用于实际项目中主体框架升级，升级之前会自动备份。

function prompt { '心莱科技: ' + (get-location) + '> '}
$start = Get-Date
#----------------------------------------------------设置路径--------------------------------------------------------------------------------
$invocation = (Get-Variable MyInvocation).Value
$directorypath = Split-Path $invocation.MyCommand.Path
Write-Host $directorypath
$rootDir = [io.Directory]::GetParent($directorypath);
$sourceCodePath = [io.Path]::Combine($rootDir, "src");
if (![io.Directory]::Exists($sourceCodePath)) {
    Write-Error "目录不存在，此结构不支持目前版本的自动升级，请手动升级。$sourceCodePath";
    return;
}
#---------------------------------------------------------------------------------------------------------------------------------------------

#----------------------------------------------------任务定义--------------------------------------------------------------------------------

#备份任务
$bakTask = { 
    Write-Host "正在备份当前代码：$sourceCodePath";
    # 备份路径
    $bakPath = [io.Path]::Combine($directorypath, "bak");
    CopyTo -sPath $sourceCodePath -dPath $bakPath;

}

#创建临时目录
$tempTask = { 
    Write-Host "正在创建临时工作区";
    $sourcePath = [io.Path]::Combine($directorypath, "source")
    # 备份路径
    $tempPath = [io.Path]::Combine($directorypath, "temp");
    CopyTo -sPath $sourcePath -dPath $tempPath;
}


#获取框架最新代码
$gitCloneTask = {
    $sourcePath = [io.Path]::Combine($directorypath, "source")
    $gitCmd = "git clone https://gitee.com/xl_wenqiang/Magicodes.Admin.Core.git" + " " + $sourcePath;
    if (![io.Directory]::Exists($sourcePath)) {
        [io.Directory]::CreateDirectory($sourcePath);
    }
    else {
        $gitCmd = "git pull";
    }
    Set-Location $sourcePath
    
    $cmdString = $gitCmd;
    Write-Host $cmdString
    cmd /c $cmdString
}

#复制项目代码
$upgradeTask = {
    Write-Warning '正在准备升级...';
    $paths = 'admin\api\Admin.Application.Custom', 'admin\api\Admin.Host\appsettings.json', 'admin\api\Admin.Host\appsettings.production.json', 'admin\api\Admin.Host\appsettings.Staging.json', 'admin\ui\nswag\service.config.nswag', 'admin\ui\src\app\admin\admin.module.ts', 'admin\ui\src\app\admin\admin-routing.module.ts', 'admin\ui\src\app\shared\layout\nav\app-navigation.service.ts', 'admin\ui\src\shared\service-proxies\service-proxy.module.ts', 'app\api\App.Application', 'app\api\App.Host\appsettings.json', 'app\api\App.Host\appsettings.production.json', 'app\api\App.Host\appsettings.Staging.json', 'core\Magicodes.Admin.Core.Custom', 'unity\Magicodes.Pay\Startup';

    $bakPath = [io.Path]::Combine($directorypath, "bak", "src");
    $tempPath = [io.Path]::Combine($directorypath, "temp", "src");
    foreach ($item in $paths) {
        $sPath = [io.Path]::Combine($bakPath, $item);
        $dPath = [io.Path]::Combine($tempPath, [io.Path]::GetDirectoryName($item));
        #Write-Host $sPath
        if ([io.Directory]::Exists($sPath)) {
            Copy-Item -Path  $sPath  -Destination $dPath  -Recurse  -Force
            Write-Warning $sPath
        }
        if ([io.File]::Exists($sPath)) {
            Copy-Item -Path  $sPath  -Destination $dPath  -Force
            Write-Warning $sPath
        }
    }
    $clearPaths = "documents;build;docker;res;softs;tools;migration.bat;README.md;.git;package-lock.json;Magicodes.Admin源码高级版授权合同.doc;Magicodes.Admin源码基础版授权合同.doc;build-mvc.ps1;build-with-ng.ps1".Split(';');
    foreach ($item in $clearPaths) {
        $path = [io.Path]::Combine($directorypath, "temp", $item);
        
        Write-Warning "正在清理：$path"
        if ([io.Directory]::Exists($path)) {
            Remove-Item $path -Recurse  -Force
        }
        if ([io.File]::Exists($path)) {
            Remove-Item $path  -Force
        }
    }
    Write-Host -ForegroundColor Red '升级完成，后台UI和APP单元测试请手工合并。合并完成后，请执行数据库迁移、部分项目包版本合并，以及检查代码是否合并正确。（建议使用版本库比较工具仔细比较）';
}
#---------------------------------------------------------------------------------------------------------------------------------------------

#----------------------------------------------------执行任务--------------------------------------------------------------------------------
Invoke-Command -ScriptBlock $bakTask
Invoke-Command -ScriptBlock $gitCloneTask
Invoke-Command -ScriptBlock $tempTask
Invoke-Command -ScriptBlock $upgradeTask
#----------------------------------------------------------------------------------------------------------------------------------------------
$end = Get-Date
Write-Host -ForegroundColor Red "执行时间"+ ($end - $start).TotalSeconds

Function  CopyTo($sPath, $dPath) {
    if ([io.Directory]::Exists($dPath)) {
        try {
            Remove-Item $dPath -Recurse  -Force
        }
        catch {
            Write-Error "删除目录出错！$dPath";
            Write-Error $Error[0].Exception;
            return;
        }
    }
    [io.Directory]::CreateDirectory($dPath);
    Get-ChildItem $sPath.FullName | 
        Where-Object { 'bin', 'obj', 'Logs', '.vs', 'packages', 'node_modules', '.cache' -notcontains $_.Name}|
        ForEach-Object {  
        Copy-Item -Path  $_.FullName  -Destination $dPath  -Recurse  -Force
        #Write-Host $_.FullName;
    }
    Select-Object -Property Name
}
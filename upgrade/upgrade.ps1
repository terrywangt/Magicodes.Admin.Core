# 本脚本用于实际项目中主体框架升级，升级之前会自动备份。

$start = Get-Date
#----------------------------------------------------设置路径--------------------------------------------------------------------------------
$invocation = (Get-Variable MyInvocation).Value
$directorypath = Split-Path $invocation.MyCommand.Path
$psPath = [io.Path]::Combine($directorypath, "functions.ps1");
Import-Module $psPath

Write-Host $directorypath -BackgroundColor Black -ForegroundColor Red
Set-Location $directorypath
$rootDir = [io.Directory]::GetParent($directorypath);
$sourceCodePath = [io.Path]::Combine($rootDir, "src");
if (![io.Directory]::Exists($sourceCodePath)) {
    Write-Host -ForegroundColor Red "目录不存在，此结构不支持目前版本的自动升级，请手动升级。$sourceCodePath";
    return;
}
#框架代码缓存目录
$adminCorePath = [io.Path]::Combine( [io.Path]::GetPathRoot($directorypath), "temp", "cache", "Magicodes.Admin.Core");
#---------------------------------------------------------------------------------------------------------------------------------------------

#----------------------------------------------------升级检查以及创建升级分支-------------------------------------------------------------------
$branchName = Get-Date -Format "yyyyMMdd_HHmmss";
$branchName = "frmupgrade/$branchName";
git checkout -B $branchName
$currentBranchName = git symbolic-ref --short -q HEAD;
Write-Host -ForegroundColor Yellow "当前分支：$currentBranchName";
if ($currentBranchName -ne $branchName) {
    Write-Host -ForegroundColor Red  "创建升级分支出错！";
    exit
}
#---------------------------------------------------------------------------------------------------------------------------------------------

#----------------------------------------------------任务定义--------------------------------------------------------------------------------

#备份任务
$bakTask = {
    Write-Host "正在备份当前代码：$sourceCodePath";
    # 备份路径
    $bakPath = [io.Path]::Combine($directorypath, "bak", "src");
    CopyItemsWithFilter -sPath $sourceCodePath -dPath $bakPath;

}

#创建临时目录
$tempTask = {
    Write-Host "正在创建临时工作区";
    $sourcePath = $adminCorePath
    # 备份路径
    $tempPath = [io.Path]::Combine($directorypath, "temp");
    CopyItemsWithFilter -sPath $sourcePath -dPath $tempPath;
}


#获取框架最新代码
$gitCloneTask = {
    $sourcePath = $adminCorePath
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
    $removePaths = 'data\Magicodes.Admin.EntityFrameworkCore\Migrations', 'app\ui';

    $paths = 'admin\api\Admin.Application.Custom', 'admin\api\Admin.Host\appsettings.json', 'admin\api\Admin.Host\appsettings.production.json', 'admin\api\Admin.Host\appsettings.Staging.json', 'admin\ui\nswag\service.config.nswag', 'admin\ui\src\app\admin\admin.module.ts', 'admin\ui\src\app\admin\admin-routing.module.ts', 'admin\ui\src\app\shared\layout\nav\app-navigation.service.ts', 'admin\ui\src\shared\service-proxies\service-proxy.module.ts', 'app\api\App.Application', 'app\api\App.Host\appsettings.json', 'app\api\App.Host\appsettings.production.json', 'app\api\App.Host\appsettings.Staging.json', 'core\Magicodes.Admin.Core.Custom', 'unity\Magicodes.Pay\Startup', 'app\api\App.Tests', 'data\Magicodes.Admin.EntityFrameworkCore\Migrations', 'core\Magicodes.Admin.Core\Authorization\AppAuthorizationProvider.Custom.cs', 'core\Magicodes.Admin.Core\Authorization\AppPermissions.Custom.cs', 'core\Magicodes.Admin.Core\Localization\Admin\Admin-zh-CN.xml', 'data\Magicodes.Admin.EntityFrameworkCore\EntityFrameworkCore\AdminDbContext.Custom.cs', 'admin\api\Admin.Jobs', 'jobs';


    $bakPath = [io.Path]::Combine($directorypath, "bak", "src");
    $tempPath = [io.Path]::Combine($directorypath, "temp", "src");

    #先移除指定目录
    RemoveItemsByPaths -paths $removePaths -path $tempPath

    #覆盖自定义代码
    CopyItemsByPaths -paths $paths -path $bakPath -destination $tempPath

    #后台UI合并
    $adminUIPath = [io.Path]::Combine($bakPath, "admin\ui\src\app\admin");
    $dAdminUIPath = [io.Path]::Combine($tempPath, "admin\ui\src\app\admin");
    $adminDirs = [io.Directory]::GetDirectories($adminUIPath);
    foreach ($item in $adminDirs) {
        [System.IO.DirectoryInfo]$dir = New-Object IO.DirectoryInfo($item);
        if (", articleInfos, articleSourceInfos, audit-logs, columnInfos, components, dashboard, demo-ui-components, editions, install, languages, maintenance, organization-units, roles, settings, shared, subscription-management, tenants, ui-customization, users, ".IndexOf(', ' + $dir.Name + ',') -eq -1) {
            Write-Warning "正在还原：$item"
            Copy-Item -Path  $item  -Destination $dAdminUIPath  -Recurse  -Force
        }
    }
    #升级部分工程文件
    $frmPath = [io.Path]::Combine($adminCorePath, "src");
    $replacePaths = "admin\api\Admin.Application.Custom\Admin.Application.Custom.csproj; app\api\App.Tests\App.Tests.csproj".Split(';');
    CopyItemsByPaths -paths $replacePaths -path $frmPath -destination $tempPath

    #执行清理
    $clearPaths = "documents; build; docker; res; softs; tools; migration.bat; README.md; .git; package-lock.json; Magicodes.Admin源码高级版授权合同.doc; Magicodes.Admin源码基础版授权合同.doc; build-mvc.ps1; build-with-ng.ps1".Split(';');
    foreach ($item in $clearPaths) {
        $path = [io.Path]::Combine($directorypath, "temp", $item);
        Write-Warning "正在清理：$path"
        RemoveItems -path $path
    }
    $message = '升级准备就绪，自动升级将存在一定的风险。'
    $question = '确定需要继续么（您可以手工完成后续升级）？【确定前请务必关闭VS等开发工具，以防止文件占用！】'

    $choices = New-Object Collections.ObjectModel.Collection[Management.Automation.Host.ChoiceDescription]
    $choices.Add((New-Object Management.Automation.Host.ChoiceDescription -ArgumentList '&Yes'))
    $choices.Add((New-Object Management.Automation.Host.ChoiceDescription -ArgumentList '&No'))

    $decision = $Host.UI.PromptForChoice($message, $question, $choices, 1)
    if ($decision -eq 0) {
        #干掉VS进程
        Get-Process -Name devenv -ErrorAction SilentlyContinue | Stop-Process
        RemoveItems -path $sourceCodePath
        [io.Directory]::CreateDirectory($sourceCodePath);

        $tempPath = [io.Path]::Combine($directorypath, "temp", "src");
        $Cmd = "ROBOCOPY $tempPath $sourceCodePath /E /XO /256 /XF *.log *.dll *.tmp *.bak /XD bin obj node_modules .cache packages Logs Debug Release";
        Write-Host $Cmd
        cmd /c $Cmd

        #启动解决方案
        $slnPath = [io.Path]::Combine($rootDir, "Admin.Host.sln");
        if ([io.File]::Exists($slnPath)) {
            Start-Process $slnPath -ErrorAction SilentlyContinue;
        }

        #TODO:执行迁移
        #TODO:执行API生成
    }
    else {
        Write-Host '操作已取消'
    }
    Write-Host -ForegroundColor Red '本次升级完成！请执行数据库迁移、部分项目包版本合并，以及检查代码是否合并正确。（建议使用版本库比较工具仔细比较）';
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
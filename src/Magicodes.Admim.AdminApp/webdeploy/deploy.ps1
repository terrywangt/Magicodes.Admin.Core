# 本脚本使用 msdeploy.exe 进行部署，需要本机安装 web deploy。
# 具体请访问 https://go.microsoft.com/?linkid=9278654
# 下载地址：https://www.microsoft.com/zh-CN/download/details.aspx?id=4148
param(
    $msdeployPath = "C:\Program Files (x86)\IIS\Microsoft Web Deploy V3\msdeploy.exe",
    $siteName = "",
    $deployUrl = "",
    $userName = "",
    $password = "",
    $configPath = "deploy.config",
    $siteUrl = "" 
)

function prompt { '心莱科技: ' + (get-location) + '> '}

#----------------------------------------------------设置路径--------------------------------------------------------------------------------
$invocation = (Get-Variable MyInvocation).Value
$directorypath = Split-Path $invocation.MyCommand.Path
$rootDir = [io.Directory]::GetParent($directorypath);
Set-Location $rootDir
$target = [io.Path]::Combine($rootDir, "dist")
#--------------------------------------------------------------------------------------------------------------------------------------------

#----------------------------------------------------编译前端脚本----------------------------------------------------------------------------
#npm run publish

$webConfigPath = [io.Path]::Combine($rootDir, "web.config")
Copy-Item -Path $webConfigPath -Destination $target -Force
Set-Location $rootDir
#--------------------------------------------------------------------------------------------------------------------------------------------

#----------------------------------------------------设置参数----------------------------------------------------------------------------------
if (![String]::IsNullOrEmpty($configPath)) {
    Write-Host '从配置文件中获取配置'

    $config = @{}
    $path = [io.Path]::Combine($directorypath, $configPath)
    if (![io.File]::Exists($path)) {
        $host.UI.WriteErrorLine('配置文件不存在，请定义！')
        return;
    }

    $payload = Get-Content -Path $path |
        Where-Object { $_ -like '*=*' } |
        ForEach-Object {
        $infos = $_ -split '='
        $key = $infos[0].Trim()
        $value = $infos[1].Trim()
        $config.$key = $value
    }
    $userName = $config.userName;
    $password = $config.password;
    $siteName = $config.siteName;
    if (![String]::IsNullOrEmpty($config.deployUrl)) {
        $deployUrl = $config.deployUrl;
    }
    if (![String]::IsNullOrEmpty($config.siteUrl)) {
        $siteUrl = $config.siteUrl;
    }
}
if (!$deployUrl.Contains("?")) {
    $deployUrl = $deployUrl + "?site=" + $siteName;
}

Write-Host 'userName：' $userName
Write-Host 'siteName：' $siteName
Write-Host 'password：' $password
Write-Host 'deployUrl：' $deployUrl
Write-Host '即将部署：' $target
# 打包压缩
# Compress-Archive -Path $target -DestinationPath $zipPath -Force
$webdeployPath = [io.Path]::Combine($rootDir, "webdeploy")
Set-Location $webdeployPath
#--------------------------------------------------------------------------------------------------------------------------------------------

#--------------------------------------------------------启动部署----------------------------------------------------------------------------
#通过服务器目录发布
#& $msdeployPath -source:contentPath=$target -verb:sync -allowUntrusted -dest:contentPath='F:\WebSites\MShop.Admin.Web',ComputerName=''$deployUrl'',UserName=''$userName'',Password=''$password'',AuthType='Basic'
#通过站点路径发布
$cmdString = """" + $msdeployPath + """" + " -source:contentPath=$target -verb:sync -allowUntrusted -dest:contentPath='$siteName',ComputerName='" + $deployUrl + "',UserName='" + $userName + "',Password='" + $password + "',AuthType='Basic'";
Write-Host 'cmd：' $cmdString
cmd /c $cmdString
#--------------------------------------------------------------------------------------------------------------------------------------------

#发布完成后，使用默认浏览器打开目标地址
if (![String]::IsNullOrEmpty($siteUrl)) {
    Start-Process -FilePath $siteUrl
}
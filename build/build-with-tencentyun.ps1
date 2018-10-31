# 参数
param(
    $configPath = "tencentyun.config",
    $tsUserName="",
    $tsPassword="",
    $tsImageHostName="",
    $tsImageUiName=""
)

# 路径变量
function prompt { '心莱科技: ' + (get-location) + '> '}

$invocation = (Get-Variable MyInvocation).Value
$buildFolder = Split-Path $invocation.MyCommand.Path
$slnFolder = [io.Directory]::GetParent($buildFolder);

$outputFolder = Join-Path $buildFolder "tsoutputs"
$webHostFolder = Join-Path $slnFolder "src/admin/api/Admin.Host"
$ngFolder = Join-Path $slnFolder "src/admin/ui"

Write-Host $buildFolder
Write-Host $slnFolder
Write-Host $webHostFolder
Write-Host $ngFolder
Write-Host $outputFolder

# 设置参数
if (![String]::IsNullOrEmpty($configPath)) {
    Write-Host '从配置文件中获取配置'
    $config = @{}
    $path = [io.Path]::Combine($buildFolder, $configPath)
    if (![io.File]::Exists($path)) {
        $host.UI.WriteErrorLine('配置文件不存在，请定义！')
        return;
    }

    Get-Content -Path $path |
        Where-Object { $_ -like '*=*' } |
        ForEach-Object {
        $infos = $_ -split '='
        $key = $infos[0].Trim()
        $value = $infos[1].Trim()
        $config.$key = $value
    }
    $tsUserName = $config.tsUserName;
    $tsPassword = $config.tsPassword;
    $tsImageHostName = $config.tsImageHostName;
    $tsImageUiName = $config.tsImageUiName;

}

## 清理 ######################################################################

Remove-Item $outputFolder -Force -Recurse -ErrorAction Ignore
New-Item -Path $outputFolder -ItemType Directory

## 还原Nuget包 #####################################################

Set-Location $slnFolder
dotnet restore

## 发布Host工程 ###################################################

Set-Location $webHostFolder
dotnet publish --output (Join-Path $outputFolder "Host")
# $appsettingsConfigPath = Join-Path $outputFolder "Host/appsettings.json"

# (Get-Content $appsettingsConfigPath) -replace """Default"": ""Server=(localdb)\\MSSQLLocalDB;  Database=Magicodes.Admin; Trusted_Connection=True;""", """Default"": ""Server= 172.16.16.9;Database=magicodes_admin_demo; User ID=dev;Password=123456abcD;""" | Set-Content $appsettingsConfigPath

## 发布 ANGULAR UI 工程 #################################################

Set-Location $ngFolder
& yarn
& ng build --prod
Copy-Item (Join-Path $ngFolder "dist") (Join-Path $outputFolder "ng/") -Recurse
Copy-Item (Join-Path $ngFolder "Dockerfile") (Join-Path $outputFolder "ng")

# Change UI configuration
$ngConfigPath = Join-Path $outputFolder "ng/assets/appconfig.json"
(Get-Content $ngConfigPath) -replace "22742", "9901" | Set-Content $ngConfigPath
(Get-Content $ngConfigPath) -replace "4200", "9902" | Set-Content $ngConfigPath

## 创建 DOCKER 镜像 #######################################################

# Host
Set-Location (Join-Path $outputFolder "Host")

## docker rmi magicodes/host -f
docker build ./ -t ccr.ccs.tencentyun.com/magicodes/admin.host

# Angular UI
Set-Location (Join-Path $outputFolder "ng")

## docker rmi magicodes/ng -f
docker build ./ -t ccr.ccs.tencentyun.com/magicodes/admin.ui

## DOCKER COMPOSE 文件 #######################################################

Copy-Item (Join-Path $slnFolder "docker/tsng/*.*") $outputFolder
Set-Location $outputFolder

docker login -u $tsUserName -p $tsPassword ccr.ccs.tencentyun.com

docker push $tsImageHostName
docker push $tsPassword
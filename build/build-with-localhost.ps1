# 参数
param(
    # 不编译不执行打包
    $noBuild = $False,
    # 不创建Docker镜像（便于直接推送）
    $noCreateDocker = $False,
    # 调试模式，以输出配置参数
    $debug = $False,
    #Host工程配置文件
    $hostConfigFile="appsettings.local.json"
)

# 路径变量
function prompt { '心莱科技: ' + (get-location) + '> '}

$invocation = (Get-Variable MyInvocation).Value
$buildFolder = Split-Path $invocation.MyCommand.Path
$slnFolder = [io.Directory]::GetParent($buildFolder);

$outputFolder = Join-Path $buildFolder "localoutputs"
$webHostFolder = Join-Path $slnFolder "src/admin/api/Admin.Host"
$ngFolder = Join-Path $slnFolder "src/admin/ui"

function LogDebug {
    param (
        $message
    )
    if($debug)
    {
        Write-Host $message
    }
}

LogDebug $buildFolder

if($debug)
{
    Write-Host $slnFolder
    Write-Host $webHostFolder
    Write-Host $ngFolder
    Write-Host $outputFolder
}

## 清理 ######################################################################
function ClearOutputFolder {
    Remove-Item $outputFolder -Force -Recurse -ErrorAction Ignore
    New-Item -Path $outputFolder -ItemType Directory
}

## 还原Nuget包 #####################################################
function RestoreSlnFolder {
    Set-Location $slnFolder
    dotnet restore
}


## 发布Host工程 ###################################################
function PublishWebHostFolder {
    Set-Location $webHostFolder
    dotnet publish --output (Join-Path $outputFolder "Host")

    $hostOutputPath = Join-Path $outputFolder "Host"

    if(![String]::IsNullOrEmpty($hostConfigFile))
    {
        $configFilePath=Join-Path $buildFolder $hostConfigFile;
        if([io.File]::Exists($configFilePath))
        {
            Copy-Item $configFilePath (Join-Path $hostOutputPath "appsettings.json")
        }
    }
}

## 发布 ANGULAR UI 工程 #################################################
function PublicNgFolder {
    Set-Location $ngFolder
    & yarn
    & ng build --prod --configuration=hmr
    Copy-Item (Join-Path $ngFolder "dist") (Join-Path $outputFolder "ng/") -Recurse
    Copy-Item (Join-Path $ngFolder "Dockerfile") (Join-Path $outputFolder "ng")

    # Change UI configuration
    $ngConfigPath = Join-Path $outputFolder "ng/assets/appconfig.json"
    (Get-Content $ngConfigPath) -replace "22742", "9901" | Set-Content $ngConfigPath
    (Get-Content $ngConfigPath) -replace "4200", "9902" | Set-Content $ngConfigPath

}


## 创建 DOCKER 镜像 #######################################################
function CreateDocker {
    # Host
    Set-Location (Join-Path $outputFolder "Host")

    ## docker rmi magicodes/host -f
    docker build ./ -t ccr.ccs.tencentyun.com/magicodes/admin.host

    # Angular UI
    Set-Location (Join-Path $outputFolder "ng")

    # 复制nginx配置文件
    Copy-Item (Join-Path $outputFolder "nginx.conf") (Join-Path $outputFolder "ng")

    ## docker rmi magicodes/ng -f
    docker build ./ -t ccr.ccs.tencentyun.com/magicodes/admin.ui
}


## DOCKER COMPOSE 文件 #######################################################
function CopyDockerCompose {
    Copy-Item (Join-Path $slnFolder "docker/tsng/*.*") $outputFolder
    Set-Location $outputFolder
}

# 执行

#判断是否需要编译
if (!$nobuild) {
    #清理输出目录
    ClearOutputFolder
    #还原包
    RestoreSlnFolder
    #发布Host工程
    PublishWebHostFolder
    #发布Angular前端工程
    PublicNgFolder
}
#判断是否需要创建Docker镜像
if(!$noCreateDocker)
{
    #复制Docker Compose文件
    CopyDockerCompose

    #创建Docker镜像
    CreateDocker
}
Set-Location $outputFolder
docker-compose up -d
Set-Location $buildFolder

# 移除命令
# cd ./localoutputs
# docker-compose down -v --rmi local

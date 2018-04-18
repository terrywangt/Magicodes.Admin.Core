# COMMON PATHS

$buildFolder = (Get-Item -Path "./" -Verbose).FullName
$slnFolder = Join-Path $buildFolder "../"
$outputFolder = Join-Path $buildFolder "outputs"
$webHostFolder = Join-Path $slnFolder "src/Magicodes.Admin.Web.Host"
$webPublicFolder = Join-Path $slnFolder "src/Magicodes.Admin.Web.Public"
$ngFolder = Join-Path $buildFolder "../../angular"

## CLEAR ######################################################################

Remove-Item $outputFolder -Force -Recurse -ErrorAction Ignore
New-Item -Path $outputFolder -ItemType Directory

## RESTORE NUGET PACKAGES #####################################################

Set-Location $slnFolder
dotnet restore

## PUBLISH WEB HOST PROJECT ###################################################

Set-Location $webHostFolder
dotnet publish --output (Join-Path $outputFolder "Host")

## PUBLISH WEB PUBLIC PROJECT ###################################################

Set-Location $webPublicFolder
dotnet publish --output (Join-Path $outputFolder "Public")

# Change Public configuration
$publicConfigPath = Join-Path $outputFolder "Public/appsettings.Staging.json"
(Get-Content $publicConfigPath) -replace "9903", "9902" | Set-Content $publicConfigPath

## PUBLISH ANGULAR UI PROJECT #################################################

Set-Location $ngFolder
& yarn
& ng build --prod
Copy-Item (Join-Path $ngFolder "dist") (Join-Path $outputFolder "ng/") -Recurse
Copy-Item (Join-Path $ngFolder "Dockerfile") (Join-Path $outputFolder "ng")

# Change UI configuration
$ngConfigPath = Join-Path $outputFolder "ng/assets/appconfig.json"
(Get-Content $ngConfigPath) -replace "22742", "9901" | Set-Content $ngConfigPath
(Get-Content $ngConfigPath) -replace "4200", "9902" | Set-Content $ngConfigPath

## CREATE DOCKER IMAGES #######################################################

# Host
Set-Location (Join-Path $outputFolder "Host")

docker rmi zero/host -f
docker build -t zero/host .

# Public
Set-Location (Join-Path $outputFolder "Public")

docker rmi zero/public -f
docker build -t zero/public .

# Angular UI
Set-Location (Join-Path $outputFolder "ng")

docker rmi zero/ng -f
docker build -t zero/ng .

## DOCKER COMPOSE FILES #######################################################

Copy-Item (Join-Path $slnFolder "docker/ng/*.*") $outputFolder

## FINALIZE ###################################################################

Set-Location $outputFolder
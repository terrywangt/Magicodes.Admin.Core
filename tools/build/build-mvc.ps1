# COMMON PATHS

$buildFolder = (Get-Item -Path "./" -Verbose).FullName
$slnFolder = Join-Path $buildFolder "../"
$outputFolder = Join-Path $buildFolder "outputs"
$webMvcFolder = Join-Path $slnFolder "src/Magicodes.Admin.Web.Mvc"
$webPublicFolder = Join-Path $slnFolder "src/Magicodes.Admin.Web.Public"

## CLEAR ######################################################################

Remove-Item $outputFolder -Force -Recurse -ErrorAction Ignore
New-Item -Path $outputFolder -ItemType Directory

## RESTORE NUGET PACKAGES #####################################################

Set-Location $slnFolder
dotnet restore

## PUBLISH WEB MVC PROJECT ###################################################

Set-Location $webMvcFolder
dotnet publish --output (Join-Path $outputFolder "Mvc")

## PUBLISH WEB PUBLIC PROJECT ###################################################

Set-Location $webPublicFolder
dotnet publish --output (Join-Path $outputFolder "Public")

## CREATE DOCKER IMAGES #######################################################

# Mvc
Set-Location (Join-Path $outputFolder "Mvc")

docker rmi zero/mvc -f
docker build -t zero/mvc .

## CREATE DOCKER IMAGES #######################################################

# Mvc
Set-Location (Join-Path $outputFolder "Public")

docker rmi zero/public -f
docker build -t zero/public .

## DOCKER COMPOSE FILES #######################################################

Copy-Item (Join-Path $slnFolder "docker/mvc/*.*") $outputFolder

## FINALIZE ###################################################################

Set-Location $outputFolder
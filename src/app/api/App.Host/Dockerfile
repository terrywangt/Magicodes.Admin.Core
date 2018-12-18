FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build

WORKDIR /src

COPY /src/app/api/App.Host/App.Host.csproj ./app/api/App.Host/
COPY /src/unity/Abp.Castle.NLog/Abp.Castle.NLog.csproj ./unity/Abp.Castle.NLog/
COPY /src/app/api/App.Application/App.Application.csproj ./app/api/App.Application/
COPY /src/core/Magicodes.Admin.Core.Custom/Magicodes.Admin.Core.Custom.csproj ./core/Magicodes.Admin.Core.Custom/
COPY /src/core/Magicodes.Admin.Core/Magicodes.Admin.Core.csproj ./core/Magicodes.Admin.Core/
COPY /src/unity/Magicodes.Sms/Magicodes.Sms.csproj ./unity/Magicodes.Sms/
COPY /src/data/Magicodes.Admin.EntityFrameworkCore/Magicodes.Admin.EntityFrameworkCore.csproj ./data/Magicodes.Admin.EntityFrameworkCore/
COPY /src/unity/Magicodes.Unity/Magicodes.Unity.csproj ./unity/Magicodes.Unity/
COPY /src/unity/Magicodes.Pay/Magicodes.Pay.csproj ./unity/Magicodes.Pay/

RUN dotnet restore ./app/api/App.Host/App.Host.csproj
COPY . .

WORKDIR /src/src/app/api/App.Host
RUN dotnet build App.Host.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish App.Host.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "App.Host.dll"]

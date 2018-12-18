FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["src/app/api/App.Host/App.Host.csproj", "src/app/api/App.Host/"]
COPY ["src/unity/Abp.Castle.NLog/Abp.Castle.NLog.csproj", "src/unity/Abp.Castle.NLog/"]
COPY ["src/app/api/App.Application/App.Application.csproj", "src/app/api/App.Application/"]
COPY ["src/admin/api/Admin.Application/Admin.Application.csproj", "src/admin/api/Admin.Application/"]
COPY ["src/unity/Magicodes.MiniProgram/Magicodes.MiniProgram.csproj", "src/unity/Magicodes.MiniProgram/"]
COPY ["src/core/Magicodes.Admin.Core/Magicodes.Admin.Core.csproj", "src/core/Magicodes.Admin.Core/"]
COPY ["src/data/Magicodes.Admin.EntityFrameworkCore/Magicodes.Admin.EntityFrameworkCore.csproj", "src/data/Magicodes.Admin.EntityFrameworkCore/"]
COPY ["src/core/Magicodes.Admin.Core.Custom/Magicodes.Admin.Core.Custom.csproj", "src/core/Magicodes.Admin.Core.Custom/"]
COPY ["src/unity/Magicodes.Unity/Magicodes.Unity.csproj", "src/unity/Magicodes.Unity/"]
COPY ["src/unity/Magicodes.Pay/Magicodes.Pay.csproj", "src/unity/Magicodes.Pay/"]
COPY ["src/unity/Magicodes.Sms/Magicodes.Sms.csproj", "src/unity/Magicodes.Sms/"]
RUN dotnet restore "src/app/api/App.Host/App.Host.csproj"
COPY . .
WORKDIR "/src/src/app/api/App.Host"
RUN dotnet build "App.Host.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "App.Host.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "App.Host.dll"]
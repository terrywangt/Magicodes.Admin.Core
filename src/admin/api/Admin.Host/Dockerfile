FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

# 安装libgdiplus库，用于Excel导出
RUN apt-get update && apt-get install -y libgdiplus

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["src/admin/api/Admin.Host/Admin.Host.csproj", "src/admin/api/Admin.Host/"]
COPY ["src/admin/api/Admin.Web.Core/Admin.Web.Core.csproj", "src/admin/api/Admin.Web.Core/"]
COPY ["src/admin/api/Admin.Application/Admin.Application.csproj", "src/admin/api/Admin.Application/"]
COPY ["src/unity/Magicodes.MiniProgram/Magicodes.MiniProgram.csproj", "src/unity/Magicodes.MiniProgram/"]
COPY ["src/core/Magicodes.Admin.Core/Magicodes.Admin.Core.csproj", "src/core/Magicodes.Admin.Core/"]
COPY ["src/data/Magicodes.Admin.EntityFrameworkCore/Magicodes.Admin.EntityFrameworkCore.csproj", "src/data/Magicodes.Admin.EntityFrameworkCore/"]
COPY ["src/core/Magicodes.Admin.Core.Custom/Magicodes.Admin.Core.Custom.csproj", "src/core/Magicodes.Admin.Core.Custom/"]
COPY ["src/unity/Magicodes.Unity/Magicodes.Unity.csproj", "src/unity/Magicodes.Unity/"]
COPY ["src/unity/Magicodes.Pay/Magicodes.Pay.csproj", "src/unity/Magicodes.Pay/"]
COPY ["src/admin/api/Admin.Application.Custom/Admin.Application.Custom.csproj", "src/admin/api/Admin.Application.Custom/"]
COPY ["src/unity/Abp.Castle.NLog/Abp.Castle.NLog.csproj", "src/unity/Abp.Castle.NLog/"]
COPY ["src/unity/Magicodes.Sms/Magicodes.Sms.csproj", "src/unity/Magicodes.Sms/"]
RUN dotnet restore "src/admin/api/Admin.Host/Admin.Host.csproj"
COPY . .
WORKDIR "/src/src/admin/api/Admin.Host"
RUN dotnet build "Admin.Host.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Admin.Host.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Magicodes.Admin.Web.Host.dll"]
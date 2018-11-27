FROM microsoft/dotnet:2.1-aspnetcore-runtime

WORKDIR /app
COPY . .
# 安装libgdiplus库
RUN apt-get update && apt-get install -y libgdiplus
ENTRYPOINT ["dotnet", "Magicodes.Admin.Web.Host.dll"]


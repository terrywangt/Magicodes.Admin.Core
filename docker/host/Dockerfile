# 使用运行时镜像
FROM microsoft/dotnet:2.1-aspnetcore-runtime

# 设置工作目录，CD
WORKDIR /app

# 安装libgdiplus库，用于Excel导出
RUN apt-get update && apt-get install -y libgdiplus

# 复制
COPY . .

#
ENTRYPOINT ["dotnet", "Magicodes.Admin.Web.Host.dll"]
FROM microsoft/dotnet:2.1-aspnetcore-runtime

WORKDIR /app
COPY . .

RUN apt-get update && apt-get install -y libgdiplus
RUN ["/bin/bash", "-c", "echo Magicodes.Admin"]
ENTRYPOINT ["dotnet", "Magicodes.Admin.Web.Host.dll"]


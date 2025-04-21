FROM mcr.microsoft.com/dotnet/sdk:9.0 AS dotnet
WORKDIR /src

ARG VERSION="1.0.0.0"

COPY ["./mcp/mcp.csproj", "./mcp/mcp.csproj"]
RUN dotnet restore "./mcp/mcp.csproj"

COPY . .
RUN dotnet publish -c Release -p:VersionPrefix="$VERSION" -o /app ./mcp/mcp.csproj

#
# Production Release
#
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS publish
WORKDIR /app

COPY --from=dotnet /app ./
ENTRYPOINT ["dotnet", "mcp.dll"]

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 8888
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["AspNetCoreSocketServer.BlazorServer/AspNetCoreSocketServer.BlazorServer.csproj", "AspNetCoreSocketServer.BlazorServer/"]
RUN dotnet restore "AspNetCoreSocketServer.BlazorServer/AspNetCoreSocketServer.BlazorServer.csproj"
COPY . .
WORKDIR "/src/AspNetCoreSocketServer.BlazorServer"
RUN dotnet build "AspNetCoreSocketServer.BlazorServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AspNetCoreSocketServer.BlazorServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AspNetCoreSocketServer.BlazorServer.dll"]

# Get base sdk image from Microsoft.
FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src

#Copy csproj files and restore all dependencies (Nuget)
COPY ["TestSysplan.API/TestSysplan.API.csproj", "TestSysplan.API/"]
COPY ["TestSysplan.Core.Infrastructure/TestSysplan.Core.Infrastructure.csproj", "TestSysplan.Core.Infrastructure/"]
COPY ["TestSysplan.Core/TestSysplan.Core.csproj", "TestSysplan.Core/"]
RUN dotnet restore "TestSysplan.API/TestSysplan.API.csproj"

# Copy the project files and build a release
COPY . .
WORKDIR "/src/TestSysplan.API"
RUN dotnet build "TestSysplan.API.csproj" -c Release -o /app/build

# Publish the released version
FROM build AS publish
RUN dotnet publish "TestSysplan.API.csproj" -c Release -o /app/publish

# Generate a runtime image
FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TestSysplan.API.dll"]
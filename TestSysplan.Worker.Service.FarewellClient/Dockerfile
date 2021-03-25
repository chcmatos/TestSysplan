# Get base sdk image from Microsoft.
FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src

#Copy csproj files and restore all dependencies (Nuget)
COPY ["TestSysplan.Worker.Service.FarewellClient/TestSysplan.Worker.Service.FarewellClient.csproj", "TestSysplan.Worker.Service.FarewellClient/"]
COPY ["TestSysplan.Core.Infrastructure/TestSysplan.Core.Infrastructure.csproj", "TestSysplan.Core.Infrastructure/"]
COPY ["TestSysplan.Core/TestSysplan.Core.csproj", "TestSysplan.Core/"]
RUN dotnet restore "TestSysplan.Worker.Service.FarewellClient/TestSysplan.Worker.Service.FarewellClient.csproj"

# Copy the project files and build a release
COPY . .
WORKDIR "/src/TestSysplan.Worker.Service.FarewellClient"
RUN dotnet build "TestSysplan.Worker.Service.FarewellClient.csproj" -c Release -o /app/build

# Publish the released version
FROM build AS publish
RUN dotnet publish "TestSysplan.Worker.Service.FarewellClient.csproj" -c Release -o /app/publish

# Generate a runtime image
FROM mcr.microsoft.com/dotnet/runtime:3.1 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TestSysplan.Worker.Service.FarewellClient.dll"]
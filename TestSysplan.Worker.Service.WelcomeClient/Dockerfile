# Get base sdk image from Microsoft.
FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src

#Copy csproj files and restore all dependencies (Nuget)
COPY ["TestSysplan.Worker.Service.WelcomeClient/TestSysplan.Worker.Service.WelcomeClient.csproj", "TestSysplan.Worker.Service.WelcomeClient/"]
COPY ["TestSysplan.Core.Infrastructure/TestSysplan.Core.Infrastructure.csproj", "TestSysplan.Core.Infrastructure/"]
COPY ["TestSysplan.Core/TestSysplan.Core.csproj", "TestSysplan.Core/"]
RUN dotnet restore "TestSysplan.Worker.Service.WelcomeClient/TestSysplan.Worker.Service.WelcomeClient.csproj"

# Copy the project files and build a release
COPY . .
WORKDIR "/src/TestSysplan.Worker.Service.WelcomeClient"
RUN dotnet build "TestSysplan.Worker.Service.WelcomeClient.csproj" -c Release -o /app/build

# Publish the released version
FROM build AS publish
RUN dotnet publish "TestSysplan.Worker.Service.WelcomeClient.csproj" -c Release -o /app/publish

# Generate a runtime image
FROM mcr.microsoft.com/dotnet/runtime:3.1 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TestSysplan.Worker.Service.WelcomeClient.dll"]
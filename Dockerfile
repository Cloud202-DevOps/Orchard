# escape=`

# =============================================================================
# Stage 1: Build (SDK image with MSBuild for .NET Framework 4.8)
# =============================================================================
FROM mcr.microsoft.com/dotnet/framework/sdk:4.8-windowsservercore-ltsc2022 AS build

WORKDIR C:\build

# Copy solution and project files first for layer caching
COPY Orchard.proj .
COPY src\Orchard.sln src\
COPY lib\ lib\

# Copy full source
COPY src\ src\

# Restore NuGet packages
RUN nuget restore src\Orchard.sln -NonInteractive -Verbosity quiet

# Build precompiled output (matches release-package.yml workflow)
RUN msbuild Orchard.proj /m /t:Precompiled /p:Configuration=Release /verbosity:minimal

# =============================================================================
# Stage 2: Runtime (ASP.NET 4.8 on IIS)
# =============================================================================
FROM mcr.microsoft.com/dotnet/framework/aspnet:4.8-windowsservercore-ltsc2022 AS runtime

SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

# Remove default IIS site
RUN Remove-WebSite -Name 'Default Web Site'

# Create application directory
WORKDIR C:\inetpub\orchard

# Copy precompiled output from build stage
COPY --from=build C:\build\build\Precompiled\ .

# Create App_Data directory structure for Orchard runtime
RUN New-Item -ItemType Directory -Force -Path C:\inetpub\orchard\App_Data\Sites\Default | Out-Null; `
    New-Item -ItemType Directory -Force -Path C:\inetpub\orchard\App_Data\Logs | Out-Null; `
    New-Item -ItemType Directory -Force -Path C:\inetpub\orchard\Media | Out-Null

# Copy entrypoint script
COPY docker-entrypoint.ps1 C:\docker-entrypoint.ps1

# Create IIS site binding on port 80
RUN New-Website -Name 'OrchardCMS' `
    -PhysicalPath 'C:\inetpub\orchard' `
    -Port 80 `
    -Force

# Set IIS app pool to use 4.0 CLR and Integrated pipeline
RUN Set-ItemProperty 'IIS:\AppPools\DefaultAppPool' -Name 'managedRuntimeVersion' -Value 'v4.0'; `
    Set-ItemProperty 'IIS:\AppPools\DefaultAppPool' -Name 'managedPipelineMode' -Value 'Integrated'; `
    Set-ItemProperty 'IIS:\AppPools\DefaultAppPool' -Name 'startMode' -Value 'AlwaysRunning'; `
    Set-ItemProperty 'IIS:\Sites\OrchardCMS' -Name 'applicationPool' -Value 'DefaultAppPool'

# Health check
HEALTHCHECK --interval=30s --timeout=10s --retries=3 --start-period=120s `
    CMD powershell -Command "try { $response = Invoke-WebRequest -Uri http://localhost/ -UseBasicParsing -TimeoutSec 5; if ($response.StatusCode -eq 200) { exit 0 } else { exit 1 } } catch { exit 1 }"

EXPOSE 80

# Entrypoint: configure database connection from environment, then start IIS
ENTRYPOINT ["powershell", "-File", "C:\\docker-entrypoint.ps1"]

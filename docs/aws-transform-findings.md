# Orchard CMS Modernization Assessment
## .NET Framework 4.8 to Modern .NET Cross-Platform

---

## 1. Solution Structure

**File**: `/src/Orchard.sln` (104KB, 1256 lines)

- **Total projects in solution**: 100
- **C# projects (non-folder)**: 88
- **Solution folders**: Modules, Tests, Tools, Themes, Specs
- **Format**: Visual Studio 2022 (Format Version 12.00, VS Version 17)

---

## 2. Main Web Project (Orchard.Web.csproj)

**File**: `/src/Orchard.Web/Orchard.Web.csproj`

### Target Framework
- **TargetFrameworkVersion**: v4.8
- **LangVersion**: 7.3
- **ProjectTypeGuids**: `{349c5851-65df-11da-9384-00065b846f21}` (ASP.NET Web Application)

### Key References
| Reference | Version | Impact |
|-----------|---------|--------|
| System.Web.Mvc | 5.3.0 | HIGH - Must migrate to ASP.NET Core MVC |
| System.Web (multiple) | N/A | HIGH - Deep framework coupling |
| Microsoft.Owin | 4.2.3 | MEDIUM - Replace with ASP.NET Core middleware |
| Microsoft.Owin.Host.SystemWeb | 4.2.3 | HIGH - IIS-specific OWIN hosting |
| log4net | 3.3.1 | MEDIUM - Replace with Microsoft.Extensions.Logging |
| Autofac | 3.5.2 | MEDIUM - Upgrade to Autofac 7+ with .NET DI |
| Newtonsoft.Json | 13.0.3 | LOW - Still supported on .NET 8+ |
| MySql.Data | 6.10.9 | MEDIUM - Replace with newer provider |
| Npgsql | 4.0.17 | MEDIUM - Upgrade to 8.x |
| System.Data.SqlServerCe | 4.0 | HIGH - SQL CE not supported on .NET Core |
| Glimpse.AspNet | 1.9.2 | HIGH - No .NET Core version exists |
| Glimpse.Mvc5 | 1.5.3 | HIGH - No .NET Core version exists |

### Project References
- Orchard.WarmupStarter
- Orchard.Framework (core)
- Orchard (Tools CLI)
- Orchard.Core
- Orchard.Glimpse

---

## 3. Web.config (IIS-Specific Configuration)

**File**: `/src/Orchard.Web/Web.config`

### Critical IIS Dependencies
- **httpRuntime**: `targetFramework="4.8"`, `requestValidationMode="2.0"`, `maxRequestLength="65536"`
- **compilation**: Dynamic compilation with custom build providers (`CSharpExtensionBuildProviderShim`)
- **authentication**: Forms authentication (`loginUrl="~/Users/Account/AccessDenied"`)
- **system.webServer modules**:
  - `WarmupHttpModule` (custom module: `Orchard.WarmupStarter.WarmupHttpModule`)
  - `Glimpse` HttpModule
- **system.webServer handlers**:
  - Custom `NotFound` handler routing all requests to MVC
  - `ExtensionlessUrlHandler` for WebAPI
  - Glimpse handler
- **httpErrors**: `existingResponse="PassThrough"`
- **requestFiltering**: 64MB max content length
- **staticContent**: Custom MIME type mappings
- **system.codedom**: Roslyn compiler registration for dynamic compilation

### OWIN Configuration
- `owin:AppStartup` = `Orchard.Owin.Startup, Orchard.Framework`

### Assembly Binding Redirects
- NHibernate -> 5.6.0.0
- System.Web.Mvc -> 5.3.0.0
- Newtonsoft.Json -> 13.0.0.0
- Autofac -> 3.5.0.0
- Assembly probing path: `App_Data/Dependencies`

### Azure Connection String
- `Orchard.Azure.Media.StorageConnectionString` = `UseDevelopmentStorage=true`

---

## 4. NuGet Dependencies (packages.config)

**File**: `/src/Orchard.Web/packages.config` (23 packages, all targeting net48)

### Framework packages.config (`/src/Orchard/packages.config`) - 25 packages:
| Package | Version | .NET Core Compatible? |
|---------|---------|----------------------|
| Antlr3.Runtime | 3.5.1 | NO - Use ANTLR4 |
| Autofac | 3.5.2 | NO - Need 7.x+ |
| Autofac.Configuration | 3.3.0 | NO - Need update |
| Castle.Core | 3.3.3 | NO - Need 5.x+ |
| FluentNHibernate | 3.4.1 | YES - Supports .NET 6+ |
| Iesi.Collections | 4.1.1 | YES |
| log4net | 3.3.1 | YES - Supports .NET 6+ |
| Microsoft.AspNet.Mvc | 5.3.0 | NO - Replace with ASP.NET Core |
| Microsoft.AspNet.Razor | 3.3.0 | NO - Replace with ASP.NET Core |
| Microsoft.AspNet.WebApi.Client | 6.0.0 | NO - Replace with HttpClient |
| Microsoft.AspNet.WebApi.Core | 5.3.0 | NO - Replace with ASP.NET Core |
| Microsoft.AspNet.WebApi.WebHost | 5.3.0 | NO - Replace with ASP.NET Core |
| Microsoft.AspNet.WebPages | 3.3.0 | NO - Replace with Razor Pages |
| Microsoft.Owin | 4.2.3 | NO - Replace with ASP.NET Core middleware |
| Microsoft.Web.Infrastructure | 1.0.0.0 | NO |
| Newtonsoft.Json | 13.0.3 | YES |
| Newtonsoft.Json.Bson | 1.0.2 | YES |
| NHibernate | 5.6.0 | YES - Supports .NET 6+ |
| Owin | 1.0 | NO - Replace with ASP.NET Core |
| Remotion.Linq | 2.2.0 | YES |

---

## 5. NHibernate Usage

### Core Framework References
**File**: `/src/Orchard/Orchard.Framework.csproj`
- NHibernate 5.6.0 (direct reference)
- FluentNHibernate 3.4.1
- Iesi.Collections 4.1.1
- Remotion.Linq 2.2.0 (NHibernate LINQ support)
- Custom NHibernate.Linq project reference: `src/Libraries/NHibernate/NHibernate.Linq/NHibernate.Linq.csproj`

### ORM Configuration Files
**Directory**: `/src/Orchard/Data/`
- `SessionFactoryHolder.cs` - Central NHibernate session factory management
- `SessionLocator.cs` - Session-per-request pattern
- `TransactionManager.cs` - Transaction management
- `Repository.cs` - Generic repository pattern
- `DataModule.cs` - Autofac registration
- `SessionConfigurationCache.cs` - Configuration caching
- `DefaultDatabaseCacheConfiguration.cs` - Second-level cache config
- `Providers/` - Database-specific providers (SqlServer, MySql, PostgreSql, SqlCe, SQLite)
- `Conventions/` - FluentNHibernate mapping conventions
- `Migration/` - Schema migration framework

### Modules Using NHibernate Directly (via packages.config)
- Orchard.AuditTrail, Orchard.Azure, Orchard.ContentPicker
- Orchard.Glimpse (also FluentNHibernate), Orchard.ImportExport
- Orchard.MessageBus, Orchard.MultiTenancy, Orchard.OpenId
- Orchard.Projections, Orchard.Redis, Orchard.Tags
- Orchard.Taxonomies, SysCache (NHibernate.Caches.SysCache2), Upgrade

### Assessment
NHibernate 5.6 **does support** .NET 6+, so the ORM itself can be retained. However, the session management is tied to `System.Web.HttpContext` via `SessionLocator.cs` and needs refactoring.

---

## 6. Global.asax / Startup Classes

### Global.asax.cs
**File**: `/src/Orchard.Web/Global.asax.cs`

```csharp
public class MvcApplication : HttpApplication
{
    private static Starter<IOrchardHost> _starter;
    
    protected void Application_Start()
    {
        RegisterRoutes(RouteTable.Routes);
        _starter = new Starter<IOrchardHost>(HostInitialization, HostBeginRequest, HostEndRequest);
        _starter.OnApplicationStart(this);
    }
    
    protected void Application_BeginRequest() { _starter.OnBeginRequest(this); }
    protected void Application_EndRequest() { _starter.OnEndRequest(this); }
    
    private static IOrchardHost HostInitialization(HttpApplication application)
    {
        var host = OrchardStarter.CreateHost(MvcSingletons);
        host.Initialize();
        host.BeginRequest();
        host.EndRequest();
        return host;
    }
    
    static void MvcSingletons(ContainerBuilder builder)
    {
        builder.Register(ctx => RouteTable.Routes).SingleInstance();
        builder.Register(ctx => ModelBinders.Binders).SingleInstance();
        builder.Register(ctx => ViewEngines.Engines).SingleInstance();
    }
}
```

### OWIN Startup
**File**: `/src/Orchard/Owin/Startup.cs`

```csharp
public class Startup
{
    public void Configuration(IAppBuilder app)
    {
        app.Use((context, next) =>
        {
            context.Response.Headers.Append("X-Generator", "Orchard");
            return next();
        });
    }
}
```

### Assessment
- Application lifecycle is heavily tied to `HttpApplication` events
- Uses WarmupStarter pattern for lazy initialization
- OWIN middleware exists but is minimal (just header injection)
- MVC singletons (RouteTable, ModelBinders, ViewEngines) are registered in Autofac container

---

## 7. App_Data Usage

**Key findings from codebase search:**

### Core Dependencies on App_Data
- `IAppDataFolder` / `IAppDataFolderRoot` - Abstraction over `~/App_Data` folder
- `AppDataFolder.cs` - Implementation with file operations
- Root path hardcoded: `public string RootPath => "~/App_Data";`
- Shell settings stored in `App_Data/Sites/{TenantName}/`
- Assembly dependencies probed from `App_Data/Dependencies/`
- Logs written to `App_Data/Logs/`
- Localization files in `App_Data/Localization/`
- Tenant-specific localization: `App_Data/Sites/{tenant}/Localization/`

### Usage Pattern
App_Data serves as a local writable data store for:
1. **Tenant configuration** (settings.txt per tenant)
2. **Compiled module assemblies** (Dependencies folder)
3. **Log files** (via log4net)
4. **Localization .po files**
5. **Recipe queue**
6. **NHibernate mapping cache** (mappings.bin, cache.dat)

---

## 8. Media Storage

### Local File System Provider
**File**: `/src/Orchard/FileSystems/Media/FileSystemStorageProvider.cs`
- Stores media in `~/Media/{TenantName}/` 
- Uses `System.Web.Hosting.HostingEnvironment.MapPath()` - IIS-specific
- Implements `IStorageProvider` interface (clean abstraction)

### Azure Blob Storage Provider
**File**: `/src/Orchard.Web/Modules/Orchard.Azure/Services/FileSystems/Media/AzureBlobStorageProvider.cs`
- Implements `IStorageProvider` via `AzureFileSystem` base class
- Suppresses `FileSystemStorageProvider` via `[OrchardSuppressDependency]`
- Configurable container name, root folder, public host name
- Connection string from platform configuration

### Media-Related Modules
- `Orchard.Media` - Basic media management
- `Orchard.MediaLibrary` - Advanced media library with typed parts (Image, Audio, Video, Document, VectorImage, OEmbed)
- `Orchard.MediaLibrary.WebSearch` - Web search integration
- `Orchard.MediaPicker` - Media picker UI
- `Orchard.MediaProcessing` - Image processing/resizing
- `Orchard.Azure` - Azure Blob Storage provider

### Assessment
The `IStorageProvider` abstraction is well-designed and allows swapping backends. Migration to modern .NET would require replacing `HostingEnvironment.MapPath()` with `IWebHostEnvironment.ContentRootPath` equivalent.

---

## 9. Logging

### Framework: log4net (via Castle.Core logging facade)

**Key files:**
- `/src/Orchard/Logging/OrchardLog4netFactory.cs` - Factory using `Castle.Core.Logging.AbstractLoggerFactory`
- `/src/Orchard/Logging/OrchardLog4netLogger.cs` - Logger wrapper
- `/src/Orchard/Logging/OrchardFileAppender.cs` - Custom appender for tenant-aware logging
- `/src/Orchard/Logging/CastleLogger.cs` / `CastleLoggerFactory.cs` - Castle integration
- `/src/Orchard/Logging/LoggingModule.cs` - Autofac module for DI
- `/src/Orchard/Logging/ILogger.cs` - Custom logging interface
- `/src/Orchard/Logging/NullLogger.cs` - Null pattern implementation

### Configuration
**File**: `/src/Orchard.Web/Config/log4net.config`
- Custom `OrchardFileAppender` type for tenant-aware file logging
- Logs to `App_Data/Logs/orchard-{type}-yyyy.MM.dd.log`
- Includes NHibernate log suppression
- Debug, Error, Recipes, and Localization appenders
- Uses `MinimalLock` locking model

### Assessment
- Uses Castle.Core as logging abstraction over log4net
- Custom `ILogger` interface (not System.Diagnostics or Microsoft.Extensions.Logging)
- Tenant-aware via log4net properties (`%P{Tenant}`, `%P{Url}`, `%P{ExecutionId}`)
- log4net 3.3.1 does support .NET 6+, but migration to `Microsoft.Extensions.Logging` recommended

---

## 10. System.Web Dependencies Depth

### Framework project (`src/Orchard/`) - Pervasive coupling

**80+ files** in the core framework reference `System.Web` namespaces. Key areas:

| Namespace | Files Affected | Subsystem |
|-----------|---------------|-----------|
| System.Web | 30+ | Core, Environment, FileSystems |
| System.Web.Mvc | 25+ | MVC, ContentManagement, Display |
| System.Web.Routing | 10+ | MVC Routes, Navigation |
| System.Web.Hosting | 8+ | VirtualPath, FileSystems, Environment |
| System.Web.Compilation | 5+ | Dynamic compilation, Extensions |
| System.Web.Http | 5+ | WebAPI controllers |

### Most Critical Coupling Points
1. **HttpContext dependency** - Used throughout for request context, session, caching
2. **VirtualPathProvider** - Dynamic module compilation system
3. **HostingEnvironment** - Path mapping, app lifecycle
4. **BuildManager** - Dynamic compilation of modules
5. **MVC infrastructure** - Controllers, ViewEngines, ModelBinders, Filters
6. **WebAPI infrastructure** - API controllers, route handlers
7. **Forms Authentication** - Security/auth system

---

## 11. Module and Theme Count

### Modules (71 directories)
**Path**: `/src/Orchard.Web/Modules/`

Full list: Lucene, Markdown, Orchard.Alias, Orchard.AntiSpam, Orchard.ArchiveLater, Orchard.AuditTrail, Orchard.Autoroute, Orchard.Azure, Orchard.Blogs, Orchard.Caching, Orchard.CodeGeneration, Orchard.Comments, Orchard.Conditions, Orchard.ContentPermissions, Orchard.ContentPicker, Orchard.ContentPreview, Orchard.ContentTypes, Orchard.CustomForms, Orchard.Dashboards, Orchard.DesignerTools, Orchard.DynamicForms, Orchard.Email, Orchard.Fields, Orchard.Forms, Orchard.Glimpse, Orchard.ImageEditor, Orchard.ImportExport, Orchard.Indexing, Orchard.JobsQueue, Orchard.Layouts, Orchard.Lists, Orchard.Localization, Orchard.Media, Orchard.MediaLibrary, Orchard.MediaLibrary.WebSearch, Orchard.MediaPicker, Orchard.MediaProcessing, Orchard.MessageBus, Orchard.Migrations, Orchard.Modules, Orchard.MultiTenancy, Orchard.OpenId, Orchard.OutputCache, Orchard.Packaging, Orchard.Pages, Orchard.Projections, Orchard.PublishLater, Orchard.Recipes, Orchard.Redis, Orchard.Resources, Orchard.Roles, Orchard.Rules, Orchard.Scripting, Orchard.Scripting.CSharp, Orchard.Scripting.Dlr, Orchard.Search, Orchard.SecureSocketsLayer, Orchard.Setup, Orchard.Tags, Orchard.Taxonomies, Orchard.Templates, Orchard.Themes, Orchard.Tokens, Orchard.Users, Orchard.Warmup, Orchard.Widgets, Orchard.Workflows, SysCache, TinyMce, Upgrade

### Themes (3 themes + support files)
**Path**: `/src/Orchard.Web/Themes/`
- SafeMode
- TheAdmin
- TheThemeMachine

---

## 12. Core Framework (Orchard.Framework.csproj)

**File**: `/src/Orchard/Orchard.Framework.csproj`
- **Target**: .NET Framework 4.8
- **Assembly**: Orchard.Framework
- **Estimated source files**: 400+ .cs files listed in csproj

### Major Subsystems
1. **ContentManagement** - Content types, parts, fields, drivers, handlers, records
2. **Data** - NHibernate sessions, repositories, migrations, conventions
3. **DisplayManagement** - Shape system, templates, placement
4. **Environment** - Shell/tenant management, extensions, compilation, state
5. **FileSystems** - App_Data, Media, VirtualPath, WebSite, LockFile, Dependencies
6. **Localization** - Multi-language, culture management
7. **Logging** - log4net integration
8. **Mvc** - Custom MVC extensions, view engines, routes, filters
9. **Owin** - OWIN middleware support
10. **Security** - Auth, encryption, membership, permissions
11. **Tasks** - Background tasks, scheduling, distributed locking
12. **UI** - Admin, navigation, resources, zones, notifications
13. **WebApi** - WebAPI support with custom controller selectors
14. **Wcf** - WCF service host support
15. **Caching** - Cache management with signals
16. **Events** - Event bus (interceptor-based)
17. **Recipes** - Recipe execution framework

---

## 13. Docker/Container Files

**Finding**: NO Dockerfile, docker-compose.yml, or any containerization artifacts found in the repository.

---

## 14. .NET Standard / .NET Core References

**Finding**: NO references to `netstandard`, `netcoreapp`, `net5`, `net6`, `net7`, or `net8` found anywhere in the codebase. This is a pure .NET Framework 4.8 application with zero cross-platform preparation.

---

## Summary: Modernization Risk Assessment

### HIGH RISK Areas (Require Complete Rewrite)
1. **System.Web coupling** - 80+ files in framework, pervasive throughout modules
2. **Dynamic compilation** - Custom BuildProvider, VirtualPathProvider system for loading modules at runtime
3. **Global.asax lifecycle** - Application_Start/BeginRequest/EndRequest patterns
4. **IIS modules/handlers** - WarmupHttpModule, custom handler pipeline
5. **SQL Server Compact** - Not available on .NET Core
6. **Glimpse** - No .NET Core equivalent (use built-in diagnostics)
7. **WCF services** - Wcf namespace with service host factories

### MEDIUM RISK Areas (Require Significant Refactoring)
1. **NHibernate** - Supports .NET 6+ but session management tied to HttpContext
2. **Autofac** - Need major version upgrade (3.x to 7.x+), API changes
3. **log4net** - Works on .NET 6+ but recommend Microsoft.Extensions.Logging
4. **OWIN middleware** - Minimal usage, easy to port to ASP.NET Core middleware
5. **App_Data dependencies** - File path abstractions need updating
6. **Multi-tenancy** - Shell/tenant system heavily uses HttpContext

### LOW RISK Areas (Portable or Easy to Migrate)
1. **Newtonsoft.Json** - Fully compatible
2. **Content management model** - Domain logic is largely framework-agnostic
3. **IStorageProvider abstraction** - Clean interface, just needs impl updates
4. **NHibernate ORM mappings** - FluentNHibernate conventions portable
5. **Recipe/migration system** - Business logic mostly portable

### Scale of Effort
- **88 C# projects** in solution
- **71 modules** to port
- **3 themes** to port
- **400+ framework files** with System.Web dependencies
- **Estimated effort**: Major undertaking (6-12+ months for a team)

### Recommendation
Consider migrating to **Orchard Core** (the official .NET Core rewrite by the same team) rather than porting this codebase. If custom modules must be preserved, a phased approach extracting business logic from framework dependencies would be necessary.

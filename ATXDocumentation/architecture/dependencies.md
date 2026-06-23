# Dependencies - Orchard CMS

## External Library Dependencies

### Core Runtime Dependencies

| Library | Version | Purpose | Status |
|---------|---------|---------|--------|
| .NET Framework | 4.8 | Runtime platform | Maintenance-only |
| ASP.NET MVC | 5.3.0 | Web framework | Superseded by ASP.NET Core |
| ASP.NET Web API | 5.3.0 | REST API framework | Superseded |
| ASP.NET WebPages/Razor | 3.3.0 | View engine | Superseded |
| Microsoft.Owin | 4.2.3 | OWIN middleware | Legacy |

### Data Access

| Library | Version | Purpose | Status |
|---------|---------|---------|--------|
| NHibernate | 5.6.0 | ORM framework | Current |
| FluentNHibernate | 3.4.1 | NHibernate mapping DSL | Current |
| System.Data.SqlServerCe | 4.0 | SQL CE provider | EOL |

### Dependency Injection

| Library | Version | Purpose | Status |
|---------|---------|---------|--------|
| Autofac | 3.5.2 | DI container | EOL (current: 8.x) |
| Autofac.Mvc5 | - | MVC integration | Legacy |
| Autofac.WebApi2 | - | Web API integration | Legacy |

### Logging and Diagnostics

| Library | Version | Purpose | Status |
|---------|---------|---------|--------|
| Castle.Core | 3.3.1 | Logging abstraction | Outdated (current: 5.x) |
| log4net | (via Castle) | File logging | Active |
| Glimpse | - | Development diagnostics | Abandoned |

### Serialization and Utilities

| Library | Version | Purpose | Status |
|---------|---------|---------|--------|
| Newtonsoft.Json | (bundled) | JSON serialization | Active |
| SharpZipLib | - | Archive operations | Active |
| Markdown | - | Markdown processing | - |

### Search

| Library | Version | Purpose | Status |
|---------|---------|---------|--------|
| Lucene.Net | (older) | Full-text search indexing | Outdated |

### Frontend Build

| Library | Version | Purpose | Status |
|---------|---------|---------|--------|
| Gulp | 3.9.1 | Task runner | EOL (current: 5.x) |
| gulp-less | 3.0.5 | LESS compilation | - |
| gulp-sass | 2.2.0 | SCSS compilation | Outdated |
| gulp-uglify | 1.5.3 | JS minification | Outdated |
| gulp-typescript | 2.12.1 | TypeScript compilation | Outdated |

## Internal Component Dependencies

### Core Framework → Modules Dependency Direction

```
Modules depend on Core Framework (never reverse)

src/Orchard (Core)
  ↑ depends on
src/Orchard.Web/Core/* (Core Modules)
  ↑ depends on
src/Orchard.Web/Modules/* (Feature Modules)
```

### Module Inter-Dependencies (from Module.txt declarations)

Key dependency chains:

```
Orchard.Blogs → Orchard.Autoroute, Orchard.Feeds
Orchard.Workflows → Orchard.Tokens, Orchard.Forms
Orchard.Projections → Orchard.Fields, Orchard.Tokens
Orchard.DynamicForms → Orchard.Layouts, Orchard.Tokens, Orchard.Workflows
Orchard.Indexing → Orchard.ContentTypes (for field indexing)
Orchard.Lucene → Orchard.Indexing
Orchard.MediaLibrary → Orchard.ContentTypes, Orchard.Tokens
Orchard.Azure → Orchard.OutputCache (cache provider)
Orchard.Taxonomies → Orchard.Autoroute, Orchard.Fields
Orchard.Localization → (Core dependencies only)
Orchard.MultiTenancy → (Core dependencies only)
```

### System.Web Coupling Points

The framework has deep coupling to `System.Web` across 80+ files and 400+ import locations:

| Namespace | Usage Count | Components |
|-----------|-------------|------------|
| `System.Web.Mvc` | High | Controllers, filters, results |
| `System.Web.Http` | Medium | Web API controllers |
| `System.Web.Routing` | High | URL routing, route registration |
| `System.Web.Hosting` | Medium | VirtualPathProvider, HostingEnvironment |
| `System.Web.Compilation` | Medium | BuildManager (dynamic compilation) |
| `System.Web.Security` | Medium | FormsAuthentication, MachineKey |

### Build Tool Dependencies

| Tool | Version | Purpose |
|------|---------|---------|
| MSBuild | (Framework SDK) | Solution build |
| NuGet | packages.config | Package management |
| Node.js | (not pinned) | Frontend build tooling |
| Roslyn | (bundled) | Dynamic compilation |

## Dependency Graph Criticality

**Critical Path Dependencies** (failure = system down):
1. .NET Framework 4.8 runtime
2. IIS (HttpApplication lifecycle)
3. NHibernate + database provider
4. Autofac (DI foundation)

**High-Impact Dependencies** (failure = degraded functionality):
1. ASP.NET MVC 5 (all web interactions)
2. Lucene.Net (search)
3. OWIN (middleware pipeline)

**Low-Impact Dependencies** (failure = feature loss):
1. Azure providers (optional cloud features)
2. Glimpse (development only)
3. TinyMCE (rich text editing)

## Cross-References

- [System Overview](system-overview.md)
- [Technical Debt - Outdated Components](../technical-debt/outdated-components.md)
- [Dependency Analysis](../analysis/dependency-analysis.md)

# Project Overview - Orchard CMS v1.11

## Summary

Orchard CMS is a modular, multi-tenant content management system built on ASP.NET MVC 5 targeting .NET Framework 4.8. It is maintained under the .NET Foundation and provides extensible content management through a plugin-based architecture with 71 feature modules.

## Technology Stack

| Category | Technology | Version |
|----------|-----------|---------|
| Runtime | .NET Framework | 4.8 |
| Language | C# | 7.3 |
| Web Framework | ASP.NET MVC | 5.3.0 |
| Web API | ASP.NET Web API | 5.3.0 |
| ORM | NHibernate | 5.6.0 |
| ORM Mapping | FluentNHibernate | 3.4.1 |
| DI Container | Autofac | 3.5.2 |
| Middleware | Microsoft.Owin | 4.2.3 |
| View Engine | Razor (WebPages 3.3.0) | - |
| Logging | log4net (via Castle.Core) | 3.3.1 |
| Client Build | Gulp | 3.9.1 |
| CSS Preprocessing | LESS, SCSS | - |
| Rich Text Editor | TinyMCE | - |
| Search | Lucene.Net | - |

## Database Support

- SQL Server (primary)
- SQL Server Compact (SqlCe)
- MySQL
- PostgreSQL
- SQLite

## Project Structure

```
Orchard/
├── src/
│   ├── Orchard/                    # Core framework (400+ files)
│   ├── Orchard.Web/               # Main web application
│   │   ├── Core/                  # 11 core modules
│   │   ├── Modules/               # 71 feature modules
│   │   └── Themes/                # 3 built-in themes
│   ├── Orchard.Azure/             # Azure integration
│   ├── Libraries/                 # Custom NHibernate.Linq
│   ├── Tools/                     # CLI tool (Orchard.exe)
│   ├── Orchard.Tests/            # Framework tests
│   ├── Orchard.Tests.Modules/    # Module tests
│   ├── Orchard.Core.Tests/       # Core tests
│   ├── Orchard.Web.Tests/        # Web tests
│   ├── Orchard.Azure.Tests/      # Azure tests
│   └── Orchard.Specs/            # BDD/SpecFlow tests
├── docs/                          # Documentation
├── Orchard.proj                   # MSBuild orchestration
└── .github/workflows/             # CI/CD (GitHub Actions)
```

## Architecture Style

- **Modular Monolith** with plugin-based extensibility
- **Multi-tenant** via shell isolation (per-tenant DI containers)
- **Content-type system** with composable parts, drivers, and handlers
- **Event-driven** with interface-based event bus
- **Convention-over-configuration** for module discovery and loading

## Deployment Model

- IIS-hosted ASP.NET application
- Dynamic compilation (Roslyn) for module code
- Assembly probing from `App_Data/Dependencies`
- Forms authentication with configurable membership providers

## Build and CI

- MSBuild-based build (Visual Studio solution)
- GitHub Actions CI (Windows runner, .NET Framework SDK)
- Gulp frontend build pipeline (LESS, Sass, TypeScript, JS minification)
- NuGet package management via `packages.config` (traditional style)

## Cross-References

- [Architecture Details](architecture/system-overview.md)
- [Technical Debt Report](technical-debt-report.md)
- [Module Reference](reference/modules.md)

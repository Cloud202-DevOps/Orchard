# Outdated Components - Orchard CMS

## Runtime and Framework Components

### .NET Framework 4.8

| Attribute | Detail |
|-----------|--------|
| **Current Version** | 4.8 |
| **Status** | Maintenance-only (no new features since 2019) |
| **Severity** | High |
| **Impact** | Cannot run cross-platform, no performance improvements from modern .NET |
| **Successor** | .NET 8+ (LTS) |
| **Migration Complexity** | High — requires rewrite of System.Web coupling |

### ASP.NET MVC 5.3.0

| Attribute | Detail |
|-----------|--------|
| **Current Version** | 5.3.0 |
| **Status** | Superseded, security-fixes only |
| **Severity** | High |
| **Impact** | No middleware pipeline, no async improvements, no endpoint routing |
| **Successor** | ASP.NET Core MVC (8.x+) |
| **Migration Complexity** | High — different hosting model, DI, middleware |

### ASP.NET Web API 5.3.0

| Attribute | Detail |
|-----------|--------|
| **Current Version** | 5.3.0 |
| **Status** | Superseded |
| **Severity** | High |
| **Impact** | Separate pipeline from MVC, System.Web dependency |
| **Successor** | ASP.NET Core (unified MVC + API) |
| **Migration Complexity** | Medium — API controllers mostly portable |

### Autofac 3.5.2

| Attribute | Detail |
|-----------|--------|
| **Current Version** | 3.5.2 |
| **Latest Version** | 8.x |
| **Severity** | High |
| **Impact** | Missing modern DI features, no async support, potential security gaps |
| **Migration Complexity** | Medium — API changes between major versions |

### Gulp 3.9.1

| Attribute | Detail |
|-----------|--------|
| **Current Version** | 3.9.1 |
| **Latest Version** | 5.0 |
| **Severity** | High |
| **Impact** | Incompatible with Node.js 12+, no maintenance |
| **Successor** | Gulp 5.x, or modern alternatives (Vite, Webpack, esbuild) |
| **Migration Complexity** | Medium — task definitions need rewrite |

## Production Dependencies

### Castle.Core 3.3.1

| Attribute | Detail |
|-----------|--------|
| **Current Version** | 3.3.1 |
| **Latest Version** | 5.x |
| **Severity** | Medium |
| **Impact** | Missing performance improvements, limited .NET Standard support |
| **Migration Complexity** | Low — API mostly stable |

### Microsoft.Owin 4.2.3

| Attribute | Detail |
|-----------|--------|
| **Current Version** | 4.2.3 |
| **Status** | OWIN superseded by ASP.NET Core middleware |
| **Severity** | Medium |
| **Impact** | Dead-end technology, no new middleware ecosystem |
| **Migration Complexity** | High — requires ASP.NET Core migration |

### SQL Server Compact Edition

| Attribute | Detail |
|-----------|--------|
| **Status** | Discontinued by Microsoft |
| **Severity** | Medium |
| **Impact** | No updates, no support, potential data corruption risks |
| **Successor** | SQLite (for embedded) or SQL Server Express |
| **Migration Complexity** | Low — SQLite support already exists |

### Lucene.Net

| Attribute | Detail |
|-----------|--------|
| **Current Version** | Older (pre-4.8) |
| **Latest Version** | 4.8+ |
| **Severity** | Medium |
| **Impact** | Missing modern search features, performance improvements |
| **Migration Complexity** | Medium — index format changes between versions |

### Glimpse (Diagnostics)

| Attribute | Detail |
|-----------|--------|
| **Status** | Abandoned (no releases since 2015) |
| **Severity** | Medium |
| **Impact** | No security patches, potential vulnerability surface |
| **Successor** | Application Insights, MiniProfiler |
| **Migration Complexity** | Low — remove reference |

### jQuery (Bundled)

| Attribute | Detail |
|-----------|--------|
| **Current Version** | Unknown (bundled) |
| **Latest Version** | 3.7.x |
| **Severity** | Medium |
| **Impact** | Potential XSS vulnerabilities in older versions |
| **Migration Complexity** | Low — update bundled file |

### TinyMCE (Bundled)

| Attribute | Detail |
|-----------|--------|
| **Current Version** | Unknown (bundled) |
| **Latest Version** | 7.x |
| **Severity** | Medium |
| **Impact** | Security vulnerabilities, missing modern editor features |
| **Migration Complexity** | Medium — API changes between major versions |

## Build/Development Dependencies

### Frontend Build Pipeline (All Gulp Plugins)

| Package | Version | Latest | Severity |
|---------|---------|--------|----------|
| gulp-less | 3.0.5 | 5.x | Low |
| gulp-sass | 2.2.0 | 5.x | Low |
| gulp-uglify | 1.5.3 | 3.x | Low |
| gulp-typescript | 2.12.1 | 6.x | Low |
| merge-stream | 1.0.0 | 2.x | Low |

All are tied to the EOL Gulp 3.x pipeline and would be replaced in any build modernization.

## Deprecated Patterns and APIs

| Pattern | Location | Replacement |
|---------|----------|-------------|
| `System.Web.HttpContext.Current` | Throughout framework | DI-based context |
| `FormsAuthentication` | Security providers | ASP.NET Core Identity |
| `BuildManager` | Dynamic compilation | Pre-compiled assemblies |
| `VirtualPathProvider` | View resolution | IFileProvider |
| `web.config transforms` | Deployment | Environment variables, appsettings.json |

## Cross-References

- [Summary](summary.md)
- [Remediation Plan](remediation-plan.md)
- [Dependencies](../architecture/dependencies.md)

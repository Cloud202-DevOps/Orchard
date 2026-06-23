# Dependency Analysis - Orchard CMS

## Internal Dependency Graph

### Layer Dependencies (Top → Bottom = depends on)

```
Layer 4: Feature Modules (71)
    │
    ├── depends on ──→ Core Modules (11)
    │                      │
    ├── depends on ──→ Core Framework (src/Orchard/)
    │                      │
    └── depends on ──→ .NET Framework BCL + System.Web
```

### Module Dependency Clusters

**Cluster 1: Content Authoring**
```
Orchard.Blogs ──→ Orchard.Autoroute ──→ Orchard.Tokens
     │                                        ↑
     └──→ Orchard.Feeds                       │
                                              │
Orchard.Pages ──→ Orchard.Autoroute ──────────┘
```

**Cluster 2: Workflow & Forms**
```
Orchard.DynamicForms ──→ Orchard.Layouts
         │               Orchard.Tokens
         │               Orchard.Workflows ──→ Orchard.Forms
         └──→ Orchard.Scripting               Orchard.Tokens
```

**Cluster 3: Search**
```
Orchard.Search ──→ Orchard.Indexing ──→ Orchard.ContentTypes
                        ↑
Lucene ─────────────────┘
```

**Cluster 4: Media**
```
Orchard.MediaProcessing ──→ Orchard.MediaLibrary ──→ Orchard.ContentTypes
Orchard.ImageEditor ──────→ Orchard.MediaLibrary       Orchard.Tokens
```

## External Dependency Analysis

### Critical Path Dependencies

These dependencies, if unavailable, prevent the application from starting:

| Dependency | Version | Alternatives | Risk |
|------------|---------|--------------|------|
| .NET Framework 4.8 | 4.8 | None (bound to Windows) | Platform lock-in |
| System.Web.Mvc | 5.3.0 | None without rewrite | Framework lock-in |
| NHibernate | 5.6.0 | Entity Framework (major rewrite) | ORM lock-in |
| Autofac | 3.5.2 | Microsoft.Extensions.DI (major rewrite) | DI lock-in |

### Runtime Dependencies (Medium Impact)

| Dependency | Version | Impact if Missing |
|------------|---------|-------------------|
| Microsoft.Owin | 4.2.3 | Middleware pipeline fails |
| Castle.Core | 3.3.1 | Logging fails |
| Newtonsoft.Json | (bundled) | Serialization fails |
| Lucene.Net | (older) | Search unavailable |
| SharpZipLib | - | Import/export fails |

### Optional Dependencies (Low Impact)

| Dependency | Version | Impact if Missing |
|------------|---------|-------------------|
| Azure SDK | - | Azure features unavailable |
| Glimpse | - | Dev diagnostics unavailable |
| SQL CE | 4.0 | CE database provider unavailable |

## Transitive Dependency Concerns

### NHibernate Stack

```
NHibernate 5.6.0
  └── Iesi.Collections
  └── Remotion.Linq
  └── Antlr (HQL parsing)

FluentNHibernate 3.4.1
  └── NHibernate 5.6.0
```

### ASP.NET Stack

```
Microsoft.AspNet.Mvc 5.3.0
  └── Microsoft.AspNet.Razor 3.3.0
  └── Microsoft.AspNet.WebPages 3.3.0
  └── System.Web (GAC)

Microsoft.AspNet.WebApi 5.3.0
  └── Microsoft.AspNet.WebApi.Core 5.3.0
  └── Newtonsoft.Json
```

### OWIN Stack

```
Microsoft.Owin 4.2.3
  └── Owin 1.0
Microsoft.Owin.Host.SystemWeb
  └── Microsoft.Owin
```

## Dependency Health Assessment

| Category | Health | Notes |
|----------|--------|-------|
| Core framework | Poor | Tied to maintenance-only platform |
| ORM (NHibernate) | Good | Actively maintained, current version |
| DI (Autofac) | Poor | 5 major versions behind |
| Logging | Fair | Castle.Core outdated but functional |
| Frontend build | Poor | Gulp 3.x EOL |
| Test framework | Fair | NUnit active, SpecFlow active |

## Cross-References

- [Dependencies](../architecture/dependencies.md)
- [Outdated Components](../technical-debt/outdated-components.md)
- [Code Metrics](code-metrics.md)

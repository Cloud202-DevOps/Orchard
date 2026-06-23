# Component Migration Order - Orchard CMS

## Recommended Migration Strategy

For migrating Orchard CMS v1.11 (.NET Framework 4.8) to a modern platform, the recommended approach is migration to **Orchard Core** (ASP.NET Core) following a bottom-up dependency order.

## Phase 1: Foundation (No Dependencies on Other Modules)

| Order | Component | Rationale |
|-------|-----------|-----------|
| 1.1 | Core Framework abstractions (interfaces) | All other code depends on these contracts |
| 1.2 | Data layer (NHibernate → EF Core or NHibernate Core) | Foundation for persistence |
| 1.3 | Caching interfaces and implementations | Used by all layers |
| 1.4 | Logging (Castle.Core → Microsoft.Extensions.Logging) | Cross-cutting concern |
| 1.5 | Security interfaces (IAuthorizationService, etc.) | Foundation for access control |

## Phase 2: Core Infrastructure

| Order | Component | Dependencies |
|-------|-----------|--------------|
| 2.1 | Shell/Tenant infrastructure | Phase 1 complete |
| 2.2 | Extension/Module management | Shell infrastructure |
| 2.3 | Content Management core (ContentItem, Parts) | Data layer, caching |
| 2.4 | Event bus | DI container |
| 2.5 | Background tasks | Shell infrastructure |

## Phase 3: Core Modules

| Order | Component | Dependencies |
|-------|-----------|--------------|
| 3.1 | Core/Common (CommonPart) | Content Management |
| 3.2 | Core/Title (TitlePart) | Content Management |
| 3.3 | Core/Settings | Shell infrastructure |
| 3.4 | Core/Navigation | Content Management |
| 3.5 | Core/Shapes | Display Management |
| 3.6 | Core/Contents (admin UI) | All above |

## Phase 4: Essential Feature Modules

| Order | Component | Dependencies |
|-------|-----------|--------------|
| 4.1 | Orchard.Users | Security, Content Management |
| 4.2 | Orchard.Roles | Users |
| 4.3 | Orchard.Tokens | Core |
| 4.4 | Orchard.Autoroute | Tokens |
| 4.5 | Orchard.Fields | Content Management |
| 4.6 | Orchard.ContentTypes | Fields |
| 4.7 | Orchard.MediaLibrary | ContentTypes, Tokens |

## Phase 5: Extended Feature Modules

| Order | Component | Dependencies |
|-------|-----------|--------------|
| 5.1 | Orchard.Blogs | Autoroute, Feeds |
| 5.2 | Orchard.Projections | Fields, Tokens |
| 5.3 | Orchard.Widgets | Core |
| 5.4 | Orchard.Workflows | Tokens, Forms |
| 5.5 | Orchard.Indexing + Lucene | ContentTypes |
| 5.6 | Orchard.OutputCache | Core |
| 5.7 | Orchard.Localization | Core |
| 5.8 | Orchard.DynamicForms | Layouts, Workflows |

## Phase 6: Optional/Specialized Modules

| Order | Component | Dependencies |
|-------|-----------|--------------|
| 6.1 | Orchard.Azure | MediaLibrary, OutputCache |
| 6.2 | Orchard.MultiTenancy | Shell infrastructure |
| 6.3 | Orchard.Taxonomies | Autoroute, Fields |
| 6.4 | Remaining modules | Various |

## Critical Path

```
Interfaces → Data Layer → Content Management → Users → Autoroute → Blogs
                                             → Fields → Projections
                                             → Tokens → Workflows
```

## Migration Blockers (Must Resolve First)

| Blocker | Impact | Resolution |
|---------|--------|-----------|
| System.Web dependency | All controllers, MVC infrastructure | Rewrite to ASP.NET Core MVC |
| Dynamic compilation | Module loading | Pre-compile all modules |
| FormsAuthentication | User sessions | ASP.NET Core Identity |
| VirtualPathProvider | View resolution | IFileProvider |
| HttpContext.Current | Static access throughout | DI-based IHttpContextAccessor |
| web.config | Configuration | appsettings.json + env vars |

## Cross-References

- [Remediation Plan](../technical-debt/remediation-plan.md)
- [Dependencies](../architecture/dependencies.md)
- [Validation Criteria](validation-criteria.md)

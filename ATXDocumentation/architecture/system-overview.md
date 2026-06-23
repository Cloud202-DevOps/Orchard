# System Overview - Orchard CMS

## Architecture Style

Orchard CMS implements a **modular monolith** architecture with multi-tenant support. The system runs as a single IIS-hosted ASP.NET application that dynamically discovers, loads, and isolates feature modules within per-tenant DI containers.

## High-Level Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                        IIS / ASP.NET                         │
├─────────────────────────────────────────────────────────────┤
│                    Orchard.Web (Host)                        │
│  ┌──────────┐  ┌──────────┐  ┌──────────────────────────┐  │
│  │Global.asax│  │web.config│  │WarmupHttpModule          │  │
│  └──────────┘  └──────────┘  └──────────────────────────┘  │
├─────────────────────────────────────────────────────────────┤
│                  OrchardStarter (Bootstrap)                  │
│  ┌────────────┐  ┌─────────────┐  ┌─────────────────────┐  │
│  │Host Container│ │Shell Manager│  │Extension Manager    │  │
│  └────────────┘  └─────────────┘  └─────────────────────┘  │
├─────────────────────────────────────────────────────────────┤
│              Shell Layer (Per-Tenant Isolation)              │
│  ┌─────────┐  ┌─────────┐  ┌─────────┐  ┌─────────────┐   │
│  │ Shell 1 │  │ Shell 2 │  │ Shell 3 │  │  Shell N    │   │
│  │(Tenant) │  │(Tenant) │  │(Tenant) │  │ (Tenant)    │   │
│  └─────────┘  └─────────┘  └─────────┘  └─────────────┘   │
├─────────────────────────────────────────────────────────────┤
│                   Core Framework (src/Orchard/)              │
│  ┌──────────────┐ ┌──────┐ ┌────────┐ ┌──────────────────┐ │
│  │Content Mgmt  │ │ Data │ │Security│ │Display/Shapes    │ │
│  ├──────────────┤ ├──────┤ ├────────┤ ├──────────────────┤ │
│  │Caching       │ │Events│ │Logging │ │Tasks/Scheduling  │ │
│  └──────────────┘ └──────┘ └────────┘ └──────────────────┘ │
├─────────────────────────────────────────────────────────────┤
│             Modules (71) + Core Modules (11)                │
│  ┌───────┐┌──────┐┌─────────┐┌──────────┐┌──────────────┐  │
│  │ Blogs ││Users ││Workflows││MediaLib  ││ ... (67+)    │  │
│  └───────┘└──────┘└─────────┘└──────────┘└──────────────┘  │
├─────────────────────────────────────────────────────────────┤
│                     Data Layer                               │
│  ┌────────────────┐  ┌─────────────────────────────────┐    │
│  │ NHibernate 5.6 │  │ SQL Server / MySQL / PostgreSQL │    │
│  └────────────────┘  └─────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────┘
```

## Bootstrap Sequence

1. **IIS** receives first request, triggers `Application_Start` in `Global.asax.cs`
2. **WarmupStarter** delays initialization until warmup complete
3. **OrchardStarter.CreateHost()** builds the host-level Autofac container
4. **DefaultOrchardHost** creates shells for each configured tenant
5. **ShellContainerFactory** builds per-tenant Autofac lifetime scopes
6. **ExtensionManager** discovers and loads enabled modules/themes per tenant
7. **ShellRoute** directs incoming requests to the appropriate tenant shell

## Key Architectural Decisions

| Decision | Rationale |
|----------|-----------|
| Multi-tenant via DI isolation | Shared hosting with per-tenant feature sets |
| Content-type composition (Parts) | Flexible content modeling without code changes |
| Dynamic compilation | Modules can be added/updated without app restart |
| Interface-based event bus | Decoupled cross-cutting communication |
| Convention-based module discovery | Zero-config module loading via `Module.txt` |
| Shape-based rendering | Theme-able, overridable UI composition |

## Deployment Model

- **Platform**: Windows Server with IIS
- **Hosting**: Single IIS application pool per instance
- **State**: Session state via ASP.NET, content in database
- **File Storage**: Local filesystem or Azure Blob (via modules)
- **Dynamic Assembly Loading**: Modules compiled at runtime or precompiled

## Cross-References

- [Components](components.md) - Detailed component breakdown
- [Dependencies](dependencies.md) - Dependency graph
- [Patterns](patterns.md) - Design patterns used
- [Project Overview](../project-overview.md)

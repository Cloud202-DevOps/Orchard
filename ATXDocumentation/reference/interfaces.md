# Interfaces - Orchard CMS

## DI Marker Interfaces

These marker interfaces control service lifetime in the Autofac container:

| Interface | Lifetime | Description |
|-----------|----------|-------------|
| `IDependency` | Transient | New instance per resolution |
| `ISingletonDependency` | Singleton (per shell) | One instance per tenant |
| `IUnitOfWorkDependency` | Per-request | One instance per HTTP request |

## Core Domain Interfaces

### Content Management

| Interface | Namespace | Responsibility |
|-----------|-----------|---------------|
| `IContentManager` | `Orchard.ContentManagement` | Content CRUD, versioning, publishing |
| `IContentHandler` | `Orchard.ContentManagement.Handlers` | Content lifecycle event handling |
| `IContentDriver` | `Orchard.ContentManagement.Drivers` | Display/editor shape generation |
| `IContentDefinitionManager` | `Orchard.ContentManagement.MetaData` | Content type/part definition management |
| `IContentDisplay` | `Orchard.ContentManagement` | Content display orchestration |

### Data Access

| Interface | Namespace | Responsibility |
|-----------|-----------|---------------|
| `IRepository<T>` | `Orchard.Data` | Generic CRUD repository |
| `ISessionLocator` | `Orchard.Data` | NHibernate session per request |
| `ITransactionManager` | `Orchard.Data` | Transaction lifecycle |
| `IDataMigration` | `Orchard.Data.Migration` | Schema versioning |
| `ISessionFactoryHolder` | `Orchard.Data` | SessionFactory management |

### Security

| Interface | Namespace | Responsibility |
|-----------|-----------|---------------|
| `IAuthorizationService` | `Orchard.Security` | Permission checking |
| `IAuthenticationService` | `Orchard.Security` | User sign-in/sign-out |
| `IMembershipService` | `Orchard.Security` | User management |
| `IEncryptionService` | `Orchard.Security` | Data encryption/decryption |
| `IRoleService` | `Orchard.Roles.Services` | Role management |

### Environment and Shell

| Interface | Namespace | Responsibility |
|-----------|-----------|---------------|
| `IOrchardHost` | `Orchard.Environment` | Top-level host management |
| `IOrchardShell` | `Orchard.Environment` | Per-tenant shell |
| `IExtensionManager` | `Orchard.Environment.Extensions` | Module discovery and loading |
| `IShellContainerFactory` | `Orchard.Environment.ShellBuilders` | Per-tenant DI container |
| `IShellDescriptorManager` | `Orchard.Environment.Descriptor` | Shell feature configuration |
| `IShellSettingsManager` | `Orchard.Environment.Configuration` | Tenant settings persistence |

### Display and Shapes

| Interface | Namespace | Responsibility |
|-----------|-----------|---------------|
| `IShapeFactory` | `Orchard.DisplayManagement` | Shape creation |
| `IShapeTableProvider` | `Orchard.DisplayManagement.Descriptors` | Shape binding declarations |
| `IDisplayManager` | `Orchard.DisplayManagement` | Display orchestration |
| `IShapeDisplay` | `Orchard.DisplayManagement` | Shape rendering |

### Caching

| Interface | Namespace | Responsibility |
|-----------|-----------|---------------|
| `ICacheManager` | `Orchard.Caching` | Cache get/set with acquisition |
| `IVolatileProvider` | `Orchard.Caching` | Cache invalidation tokens |
| `ISignals` | `Orchard.Caching` | Signal-based eviction |

### Events

| Interface | Namespace | Responsibility |
|-----------|-----------|---------------|
| `IEventHandler` | `Orchard.Events` | Base event handler marker |
| `IEventBus` | `Orchard.Events` | Event dispatch |

### Tasks

| Interface | Namespace | Responsibility |
|-----------|-----------|---------------|
| `IBackgroundTask` | `Orchard.Tasks` | Periodic background work |
| `IScheduledTaskManager` | `Orchard.Tasks.Scheduling` | Named scheduled tasks |
| `IScheduledTaskHandler` | `Orchard.Tasks.Scheduling` | Scheduled task execution |

### UI and Navigation

| Interface | Namespace | Responsibility |
|-----------|-----------|---------------|
| `INavigationProvider` | `Orchard.UI.Navigation` | Menu item contribution |
| `IResourceManifestProvider` | `Orchard.UI.Resources` | JS/CSS resource declarations |
| `INotifier` | `Orchard.UI.Notify` | User notification messages |

### File System

| Interface | Namespace | Responsibility |
|-----------|-----------|---------------|
| `IStorageProvider` | `Orchard.FileSystems.Media` | Media file storage abstraction |
| `IVirtualPathProvider` | `Orchard.FileSystems.VirtualPath` | Virtual path resolution |
| `IAppDataFolder` | `Orchard.FileSystems.AppData` | App_Data access |

### Logging

| Interface | Namespace | Responsibility |
|-----------|-----------|---------------|
| `ILogger` | `Orchard.Logging` | Logging abstraction |
| `ILoggerFactory` | `Orchard.Logging` | Logger instance creation |

## Module-Level Interfaces

### Orchard.Users

| Interface | Responsibility |
|-----------|---------------|
| `IUserService` | User lookup and management |
| `IUserEventHandler` | User lifecycle events |

### Orchard.Workflows

| Interface | Responsibility |
|-----------|---------------|
| `IActivity` | Workflow activity implementation |
| `IWorkflowManager` | Workflow execution engine |

### Orchard.Indexing

| Interface | Responsibility |
|-----------|---------------|
| `IIndexProvider` | Search index operations |
| `ISearchService` | Search query execution |
| `IIndexingService` | Index management |

### Orchard.OutputCache

| Interface | Responsibility |
|-----------|---------------|
| `IOutputCacheStorageProvider` | Cache storage backend |
| `ICacheControlStrategy` | Cache control decisions |

## Inheritance Hierarchies

### Content Part Hierarchy
```
ContentPart (abstract)
  └── ContentPart<TRecord> (generic, NHibernate-backed)
       ├── TitlePart (Core)
       ├── BodyPart (Core)
       ├── CommonPart (Core)
       ├── AutoroutePart
       ├── BlogPart
       ├── UserPart
       └── ... (one per composable content part)
```

### Content Handler Hierarchy
```
ContentHandler (abstract base)
  ├── ContentHandlerBase (convenience base)
  └── [Module]PartHandler (per-part handler)
```

### Controller Hierarchy
```
System.Web.Mvc.Controller
  └── Orchard controllers (no intermediate base class)
       ├── ContentController
       ├── AccountController
       ├── AdminController
       └── ... (one per module feature)
```

## Cross-References

- [Components](../architecture/components.md)
- [Patterns](../architecture/patterns.md)
- [API Reference](api-reference.md)

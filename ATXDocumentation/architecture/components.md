# Components - Orchard CMS

## Core Framework (`src/Orchard/`)

### Content Management System

The central domain of Orchard. Provides a composable content-type system.

| Component | Responsibility |
|-----------|---------------|
| `DefaultContentManager` | CRUD operations for content items with versioning |
| `ContentPart` / `ContentField` | Composable content type building blocks |
| `ContentHandler` | Lifecycle event handlers (creating, publishing, removing) |
| `ContentDriver` | Display/editor rendering for content parts |
| `ContentTypeDefinition` | Metadata defining content type composition |
| `ImportExportManager` | Content serialization for import/export |

### Data Layer (`src/Orchard/Data/`)

| Component | Responsibility |
|-----------|---------------|
| `Repository<T>` | Generic NHibernate repository pattern |
| `SessionLocator` | Per-request NHibernate session management |
| `TransactionManager` | Transaction demarcation and lifecycle |
| `SchemaBuilder` / `DataMigration` | Database schema evolution framework |
| `SessionFactoryHolder` | NHibernate SessionFactory per-tenant management |

### Environment and Shell (`src/Orchard/Environment/`)

| Component | Responsibility |
|-----------|---------------|
| `DefaultOrchardHost` | Top-level host managing all tenant shells |
| `ShellContainerFactory` | Per-tenant Autofac container builder |
| `ExtensionManager` | Module/theme discovery, loading, dependency resolution |
| `ShellSettings` | Per-tenant configuration (connection string, features) |
| `RunningShellTable` | Maps request URLs to active tenant shells |

### Security (`src/Orchard/Security/`)

| Component | Responsibility |
|-----------|---------------|
| `IAuthorizationService` | Permission checking (Permission + User + Content) |
| `IAuthenticationService` | User identity management |
| `IMembershipService` | User creation, validation, password management |
| `IEncryptionService` | Data encryption (machine key-based) |
| `FormsAuthenticationService` | ASP.NET Forms Auth integration |

### Display and Shapes (`src/Orchard/DisplayManagement/`)

| Component | Responsibility |
|-----------|---------------|
| `ShapeFactory` | Dynamic shape creation for UI rendering |
| `ShapeTableLocator` | Discovers shape bindings from themes/modules |
| `DisplayManager` | Orchestrates content display through drivers |
| `ShapeResult` | MVC ActionResult that renders shapes |

### MVC Integration (`src/Orchard/Mvc/`)

| Component | Responsibility |
|-----------|---------------|
| `OrchardControllerFactory` | Resolves controllers from per-tenant container |
| `ThemedAttribute` | Marks actions for theme layout rendering |
| `ShapeResult` | ActionResult for shape-based views |
| `ModelBinders` | Custom model binding for Orchard types |

### Caching (`src/Orchard/Caching/`)

| Component | Responsibility |
|-----------|---------------|
| `ICacheManager` | Generic cache with lambda-based acquisition |
| `IVolatileToken` | Cache invalidation signaling |
| `ISignals` | Named signal-based cache eviction |

### Tasks (`src/Orchard/Tasks/`)

| Component | Responsibility |
|-----------|---------------|
| `IBackgroundTask` | Interface for periodic background work |
| `BackgroundService` | Sweeps all tasks in isolated transactions |
| `IScheduledTask` | Named tasks with scheduled execution times |

## Core Modules (`src/Orchard.Web/Core/`)

| Module | Responsibility |
|--------|---------------|
| `Common` | CommonPart (owner, dates), BodyPart, IdentityPart |
| `Contents` | Content item CRUD controllers and admin UI |
| `Navigation` | Menu system, admin menu, breadcrumbs |
| `Settings` | Site-wide settings management |
| `Shapes` | Core shape definitions (Layout, Zone, List) |
| `Feeds` | RSS/Atom feed generation |
| `Dashboard` | Admin dashboard framework |
| `Containers` | Content containment relationships |
| `Title` | TitlePart for content items |
| `Scheduling` | Task scheduling infrastructure |
| `XmlRpc` | XML-RPC protocol support |

## Feature Modules (`src/Orchard.Web/Modules/`) - Key Modules

| Module | Responsibility |
|--------|---------------|
| `Orchard.Users` | User registration, login, account management |
| `Orchard.Roles` | Role-based access control |
| `Orchard.Blogs` | Blog content type and listing |
| `Orchard.Workflows` | Visual workflow engine with activities |
| `Orchard.MediaLibrary` | Media upload and management |
| `Orchard.Autoroute` | Automatic URL generation for content |
| `Orchard.Projections` | Dynamic content queries and lists |
| `Orchard.Taxonomies` | Hierarchical classification system |
| `Orchard.Indexing` / `Lucene` | Full-text search indexing |
| `Orchard.OutputCache` | Page-level output caching |
| `Orchard.DynamicForms` | Form builder with validation |
| `Orchard.Localization` | Multi-language content support |
| `Orchard.MultiTenancy` | Tenant management UI |
| `Orchard.Azure` | Azure Blob storage, Azure cache providers |
| `Orchard.Tokens` | Token replacement system |
| `Orchard.Widgets` | Widget zones and layer rules |
| `Orchard.ContentTypes` | Admin UI for content type management |

## Themes (`src/Orchard.Web/Themes/`)

| Theme | Purpose |
|-------|---------|
| `TheAdmin` | Administration panel theme |
| `TheThemeMachine` | Default frontend theme |
| `SafeMode` | Minimal theme for recovery mode |

## Tools (`src/Tools/`)

| Tool | Purpose |
|------|---------|
| `Orchard.exe` | Command-line interface for Orchard operations |
| `MSBuild Tasks` | Custom build tasks for module packaging |

## Cross-References

- [System Overview](system-overview.md)
- [Dependencies](dependencies.md)
- [Module Reference](../reference/modules.md)

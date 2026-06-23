# Modules Reference - Orchard CMS

## Module Architecture

Each module follows a standard structure:

```
Modules/Orchard.{ModuleName}/
├── Module.txt              # Manifest (name, version, dependencies)
├── Orchard.{ModuleName}.csproj
├── Controllers/            # MVC controllers
├── Drivers/                # Content part display drivers
├── Handlers/               # Content lifecycle handlers
├── Models/                 # Part/record classes
├── Services/               # Business logic services
├── ViewModels/             # View models
├── Views/                  # Razor templates
├── Migrations.cs           # Database schema migrations
├── Routes.cs               # URL route definitions
├── Permissions.cs          # Permission declarations
├── AdminMenu.cs            # Admin navigation contribution
└── ResourceManifest.cs     # JS/CSS resource declarations
```

## Complete Module List (71 Modules)

### Content Management

| Module | Description | Key Dependencies |
|--------|-------------|-----------------|
| Orchard.ContentTypes | Admin UI for content type management | Core |
| Orchard.ContentPicker | Content item selection widget | Core |
| Orchard.Fields | Standard content field types | Core |
| Orchard.Tokens | Token replacement system | Core |
| Orchard.Autoroute | Automatic URL generation | Orchard.Tokens |
| Orchard.Alias | URL alias management | Core |
| Orchard.Templates | Shape template override UI | Core |

### Blogging and Publishing

| Module | Description | Key Dependencies |
|--------|-------------|-----------------|
| Orchard.Blogs | Blog content type and listing | Orchard.Autoroute, Orchard.Feeds |
| Orchard.Comments | Comment system | Core |
| Orchard.PublishLater | Scheduled publishing | Core |
| Orchard.Pages | Page content type | Orchard.Autoroute |

### Media

| Module | Description | Key Dependencies |
|--------|-------------|-----------------|
| Orchard.MediaLibrary | Media upload and management | Orchard.ContentTypes |
| Orchard.MediaProcessing | Image resizing/cropping | Orchard.MediaLibrary |
| Orchard.ImageEditor | Browser-based image editing | Orchard.MediaLibrary |

### Security and Users

| Module | Description | Key Dependencies |
|--------|-------------|-----------------|
| Orchard.Users | User management | Core |
| Orchard.Roles | Role-based access control | Orchard.Users |
| Orchard.OpenId | OpenID authentication | Orchard.Users |
| Orchard.AntiSpam | Spam protection (CAPTCHA, Akismet) | Core |

### Search and Indexing

| Module | Description | Key Dependencies |
|--------|-------------|-----------------|
| Orchard.Indexing | Search indexing framework | Orchard.ContentTypes |
| Lucene | Lucene.Net search provider | Orchard.Indexing |
| Orchard.Search | Search UI and results | Orchard.Indexing |

### Layout and Display

| Module | Description | Key Dependencies |
|--------|-------------|-----------------|
| Orchard.Widgets | Widget zones and layers | Core |
| Orchard.Layouts | Visual layout editor | Core |
| Orchard.DynamicForms | Form builder | Orchard.Layouts, Orchard.Tokens |
| Orchard.Projections | Dynamic content queries | Orchard.Fields, Orchard.Tokens |
| Orchard.Lists | Content lists | Core |
| Orchard.Themes | Theme management UI | Core |
| Orchard.DesignerTools | Theme development helpers | Core |

### Workflows and Automation

| Module | Description | Key Dependencies |
|--------|-------------|-----------------|
| Orchard.Workflows | Visual workflow engine | Orchard.Tokens, Orchard.Forms |
| Orchard.Forms | Form rendering for workflows | Core |
| Orchard.Scripting | Script execution | Core |
| Orchard.Scripting.CSharp | C# script support | Orchard.Scripting |
| Orchard.Rules | Business rule execution | Core |

### Localization and i18n

| Module | Description | Key Dependencies |
|--------|-------------|-----------------|
| Orchard.Localization | Multi-language content | Core |
| Orchard.CulturePicker | Culture selection UI | Orchard.Localization |

### Caching and Performance

| Module | Description | Key Dependencies |
|--------|-------------|-----------------|
| Orchard.OutputCache | Page-level output caching | Core |
| Orchard.Caching | Cache management UI | Core |
| Orchard.Warmup | Pre-render pages for warmup | Core |

### Taxonomy and Classification

| Module | Description | Key Dependencies |
|--------|-------------|-----------------|
| Orchard.Taxonomies | Hierarchical term management | Orchard.Autoroute |
| Orchard.Tags | Flat tagging system | Core |
| Orchard.Conditions | Conditional rules engine | Core |

### Multi-Tenancy and Hosting

| Module | Description | Key Dependencies |
|--------|-------------|-----------------|
| Orchard.MultiTenancy | Tenant management UI | Core |
| Orchard.Setup | Initial site setup wizard | Core |

### Cloud Integration

| Module | Description | Key Dependencies |
|--------|-------------|-----------------|
| Orchard.Azure | Azure Blob storage | Core |
| Orchard.Azure.MediaStorage | Azure media provider | Orchard.MediaLibrary |

### Import/Export

| Module | Description | Key Dependencies |
|--------|-------------|-----------------|
| Orchard.ImportExport | Content import/export | Core |
| Orchard.Recipes | Recipe execution | Core |

### Developer and Admin Tools

| Module | Description | Key Dependencies |
|--------|-------------|-----------------|
| Orchard.Modules | Module management UI | Core |
| Orchard.Diagnostics | Diagnostic tools | Core |
| Orchard.CodeGeneration | Module scaffolding | Core |
| Orchard.jQuery | jQuery library inclusion | Core |
| TinyMce | Rich text editor | Core |

### Communication

| Module | Description | Key Dependencies |
|--------|-------------|-----------------|
| Orchard.Email | Email sending | Core |
| Orchard.Messaging | Messaging framework | Core |

### Other

| Module | Description | Key Dependencies |
|--------|-------------|-----------------|
| Orchard.Packaging | Module/theme packaging | Core |
| Orchard.Resources | Shared resource library | Core |
| Orchard.CustomForms | Custom form builder | Orchard.DynamicForms |
| Orchard.ContentPermissions | Per-content permissions | Orchard.Roles |
| Orchard.SecureSocketsLayer | HTTPS enforcement | Core |
| Orchard.Glimpse | Glimpse diagnostics | Core |
| Orchard.TaskLease | Distributed task locking | Core |

## Core Modules (11)

| Module | Location | Responsibility |
|--------|----------|---------------|
| Common | `Core/Common/` | CommonPart, BodyPart, IdentityPart |
| Containers | `Core/Containers/` | Content containment relationships |
| Contents | `Core/Contents/` | Content item admin CRUD |
| Dashboard | `Core/Dashboard/` | Admin dashboard |
| Feeds | `Core/Feeds/` | RSS/Atom feed framework |
| Navigation | `Core/Navigation/` | Menu and breadcrumb system |
| Reports | `Core/Reports/` | Reporting framework |
| Scheduling | `Core/Scheduling/` | Task scheduling |
| Settings | `Core/Settings/` | Site-wide settings |
| Shapes | `Core/Shapes/` | Core shape bindings |
| Title | `Core/Title/` | TitlePart |
| XmlRpc | `Core/XmlRpc/` | XML-RPC protocol |

## Cross-References

- [Components](../architecture/components.md)
- [Program Structure](program-structure.md)
- [API Reference](api-reference.md)

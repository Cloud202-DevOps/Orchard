# Program Structure - Orchard CMS

## Solution Organization

```
Orchard.sln (88 projects)
├── src/Orchard/                          [Core Framework Library]
│   ├── Caching/                          Cache management and invalidation
│   ├── Commands/                         CLI command infrastructure
│   ├── ContentManagement/                Content type system (core domain)
│   │   ├── Aspects/                      Cross-cutting content behaviors
│   │   ├── Drivers/                      Content display/editor drivers
│   │   ├── FieldStorage/                 Field value storage
│   │   ├── Handlers/                     Content lifecycle handlers
│   │   ├── MetaData/                     Content type metadata
│   │   ├── Records/                      NHibernate persistence records
│   │   └── Utilities/                    Content helper utilities
│   ├── Data/                             Data access layer (NHibernate)
│   │   ├── Bags/                         Dynamic property bags
│   │   ├── Conventions/                  NHibernate mapping conventions
│   │   ├── Migration/                    Schema migration framework
│   │   └── Providers/                    Database provider implementations
│   ├── DisplayManagement/                Shape-based rendering system
│   │   ├── Descriptors/                  Shape table descriptors
│   │   ├── Implementation/              Display manager implementation
│   │   └── Shapes/                       Shape definitions
│   ├── Environment/                      Hosting and shell infrastructure
│   │   ├── Configuration/               Host configuration
│   │   ├── Extensions/                   Module/theme management
│   │   │   ├── Compilers/               Dynamic compilation
│   │   │   ├── Folders/                  Extension folder scanning
│   │   │   └── Loaders/                 Assembly loading strategies
│   │   ├── ShellBuilders/               Per-tenant container construction
│   │   ├── State/                        Shell state management
│   │   └── Warmup/                       Application warmup
│   ├── Events/                           Event bus infrastructure
│   ├── Exceptions/                       Exception handling
│   ├── FileSystems/                      File abstraction layer
│   │   ├── AppData/                      App_Data file access
│   │   ├── Dependencies/                 Assembly dependency resolution
│   │   ├── LockFile/                     File locking
│   │   ├── Media/                        Media file storage
│   │   ├── VirtualPath/                  Virtual path resolution
│   │   └── WebSite/                      Website file access
│   ├── Localization/                     Internationalization framework
│   ├── Logging/                          log4net integration
│   ├── Mvc/                              ASP.NET MVC extensions
│   │   ├── AntiForgery/                  CSRF protection
│   │   ├── Extensions/                   MVC helper extensions
│   │   ├── Html/                         HTML helper extensions
│   │   ├── ModelBinders/                 Custom model binding
│   │   └── Routes/                       Route management
│   ├── Recipes/                          Recipe execution framework
│   ├── Reports/                          Reporting infrastructure
│   ├── Scripting/                        Script execution engine
│   ├── Security/                         Authentication & authorization
│   │   └── Providers/                    Security provider implementations
│   ├── Settings/                         Site settings infrastructure
│   ├── Tasks/                            Background task framework
│   │   ├── Indexing/                     Search indexing tasks
│   │   ├── Locking/                      Distributed locking
│   │   └── Scheduling/                   Task scheduling
│   ├── Themes/                           Theme management
│   ├── Time/                             Date/time utilities
│   ├── UI/                               UI infrastructure
│   │   ├── Admin/                        Admin panel helpers
│   │   ├── Navigation/                   Menu/navigation builders
│   │   ├── Notify/                       Notification system
│   │   ├── PageClass/                    Page CSS class management
│   │   ├── PageTitle/                    Page title management
│   │   ├── Resources/                    Resource (JS/CSS) management
│   │   └── Zones/                        Layout zone management
│   ├── Utility/                          General utilities
│   │   └── Extensions/                   Extension methods
│   ├── Validation/                       Validation framework
│   └── WebApi/                           Web API extensions
│
├── src/Orchard.Web/                      [Web Application Host]
│   ├── Core/                             [11 Core Modules]
│   │   ├── Common/                       CommonPart, BodyPart, IdentityPart
│   │   ├── Containers/                   Content containment
│   │   ├── Contents/                     Content CRUD admin
│   │   ├── Dashboard/                    Admin dashboard
│   │   ├── Feeds/                        RSS/Atom generation
│   │   ├── Navigation/                   Menu system
│   │   ├── Reports/                      Report display
│   │   ├── Scheduling/                   Task scheduling
│   │   ├── Settings/                     Site settings
│   │   ├── Shapes/                       Core shape bindings
│   │   ├── Title/                        TitlePart
│   │   └── XmlRpc/                       XML-RPC protocol
│   ├── Modules/                          [71 Feature Modules]
│   └── Themes/                           [3 Themes]
│       ├── SafeMode/                     Recovery theme
│       ├── TheAdmin/                     Admin panel theme
│       └── TheThemeMachine/              Default frontend theme
│
├── src/Orchard.Azure/                    [Azure Integration]
├── src/Libraries/                        [Custom Libraries]
│   └── NHibernate.Linq/                 Custom LINQ provider
├── src/Tools/                            [Development Tools]
│   ├── Orchard/                          CLI tool (Orchard.exe)
│   └── MSBuild.Orchard/                 Custom MSBuild tasks
│
└── Test Projects
    ├── src/Orchard.Tests/                Framework unit tests
    ├── src/Orchard.Tests.Modules/        Module unit tests
    ├── src/Orchard.Core.Tests/           Core module tests
    ├── src/Orchard.Web.Tests/            Web project tests
    ├── src/Orchard.Azure.Tests/          Azure integration tests
    ├── src/Orchard.Specs/                SpecFlow BDD tests
    └── src/Tools/Orchard.Tests/          CLI tool tests
```

## Key File Counts

| Category | Count |
|----------|-------|
| C# source files (.cs) | 3,675 |
| JavaScript files (.js) | 1,015 |
| Razor views (.cshtml) | 876 |
| Configuration files (.config) | 315 |
| CSS files (.css) | 228 |
| SCSS files (.scss) | 90 |
| Project files (.csproj) | 89 |
| SpecFlow features (.feature) | 29 |

## Cross-References

- [System Overview](../architecture/system-overview.md)
- [Components](../architecture/components.md)
- [Modules](modules.md)

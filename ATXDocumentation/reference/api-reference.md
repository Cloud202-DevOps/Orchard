# API Reference - Orchard CMS

## Web Controllers (MVC)

### Core Content Controllers

| Controller | Route | Methods |
|------------|-------|---------|
| `ContentsController` | `/Admin/Contents` | List, Create, Edit, Publish, Unpublish, Remove |
| `ContentTypesController` | `/Admin/ContentTypes` | Index, Create, Edit, AddPart, RemovePart |

### User Controllers (`Orchard.Users`)

| Controller | Route | Methods |
|------------|-------|---------|
| `AccountController` | `/Users/Account` | LogOn, LogOff, Register, RequestLostPassword, ChangePassword |
| `AdminController` | `/Admin/Users` | Index, Create, Edit, Delete, Approve, Moderate |

### Blog Controllers (`Orchard.Blogs`)

| Controller | Route | Methods |
|------------|-------|---------|
| `BlogController` | `/Admin/Blogs` | Create, Edit, Remove, List |
| `BlogPostController` | `/Admin/Blogs/{blogId}/Posts` | Create, Edit, Publish, Unpublish, Delete |

### Media Controllers (`Orchard.MediaLibrary`)

| Controller | Route | Methods |
|------------|-------|---------|
| `AdminController` | `/Admin/MediaLibrary` | Index, MediaItems, Import, Upload |
| `FolderController` | `/Admin/MediaLibrary/Folder` | Create, Edit, Delete, Move |
| `ClientStorageController` | `/Admin/MediaLibrary/Upload` | Upload (AJAX) |

### Navigation Controllers (`Core.Navigation`)

| Controller | Route | Methods |
|------------|-------|---------|
| `AdminController` | `/Admin/Navigation` | Index, Create, Edit, Delete, MoveUp, MoveDown |

## Web API Endpoints

### XML-RPC (`Core.XmlRpc`)

| Endpoint | Protocol | Methods |
|----------|----------|---------|
| `/xmlrpc` | XML-RPC | metaWeblog.newPost, metaWeblog.editPost, metaWeblog.getPost, metaWeblog.getRecentPosts |

### Content API (via `Orchard.Projections`)

Dynamic query endpoints configured per projection. No fixed REST API — content is queried via projection definitions.

## Public Service APIs (Internal)

### IContentManager

```csharp
ContentItem New(string contentType)
ContentItem Get(int id, VersionOptions options)
IEnumerable<ContentItem> GetAllVersions(int id)
void Create(ContentItem contentItem, VersionOptions options)
void Publish(ContentItem contentItem)
void Unpublish(ContentItem contentItem)
void Remove(ContentItem contentItem)
ContentItem Clone(ContentItem contentItem)
void Flush()
IContentQuery<ContentItem> Query()
```

### IMembershipService

```csharp
IUser CreateUser(CreateUserParams createUserParams)
IUser GetUser(string username)
IUser ValidateUser(string userNameOrEmail, string password)
void SetPassword(IUser user, string password)
```

### IAuthorizationService

```csharp
void CheckAccess(Permission permission, IUser user, IContent content)
bool TryCheckAccess(Permission permission, IUser user, IContent content)
```

### ICacheManager

```csharp
TResult Get<TKey, TResult>(TKey key, Func<AcquireContext<TKey>, TResult> acquire)
IVolatileToken GetCacheContextToken(string contextName)
```

### IWorkflowManager

```csharp
void TriggerEvent(string name, IDictionary<string, object> tokens)
IEnumerable<WorkflowRecord> GetWorkflowsForActivity(string activityName)
```

### IStorageProvider

```csharp
string GetPublicUrl(string path)
IStorageFile GetFile(string path)
IEnumerable<IStorageFile> ListFiles(string path)
IEnumerable<IStorageFolder> ListFolders(string path)
void CreateFolder(string path)
void DeleteFolder(string path)
void DeleteFile(string path)
IStorageFile CreateFile(string path)
bool FileExists(string path)
```

## Declarative Service Definitions

### Module.txt Manifest Format

```
Name: ModuleName
AntiForgery: enabled
Author: Author Name
Website: https://example.com
Version: 1.0.0
OrchardVersion: 1.10.3
Description: Module description
Dependencies: Dependency1, Dependency2
Features:
    ModuleName.SubFeature:
        Name: Sub Feature
        Description: Sub feature description
        Dependencies: ModuleName, OtherModule
        Category: CategoryName
```

### web.config Service Definitions

- HTTP Handlers: `OrchardModule` (custom IHttpModule)
- HTTP Modules: `WarmupHttpModule`
- Compilation: Roslyn compiler registered via `system.codedom`
- Authentication: Forms authentication with custom login URL

## Cross-References

- [Interfaces](interfaces.md)
- [Modules](modules.md)
- [Workflows](../behavior/workflows.md)

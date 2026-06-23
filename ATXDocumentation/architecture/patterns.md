# Architectural Patterns - Orchard CMS

## Design Patterns

### 1. Composition over Inheritance (Content Parts)

The content management system uses a **composition pattern** where content types are assembled from reusable parts rather than using class inheritance.

```
ContentItem
  ├── ContentPart: TitlePart
  ├── ContentPart: BodyPart
  ├── ContentPart: CommonPart (owner, dates)
  ├── ContentPart: AutoroutePart (URLs)
  └── ContentPart: PublishLaterPart
```

Each part has:
- **Record** - NHibernate-persisted data
- **Handler** - Lifecycle event responses
- **Driver** - Display/editor shape generation

### 2. Multi-Tenant Shell Isolation

Each tenant runs in an isolated Autofac lifetime scope:

```
Host Container (singleton services)
  └── Shell Scope (per-tenant)
       ├── Own ExtensionManager
       ├── Own enabled features
       ├── Own NHibernate SessionFactory
       └── Own route table
```

### 3. Interface-Based Event Bus

Cross-cutting communication uses interface-based events:

```csharp
public interface IUserEventHandler : IEventHandler {
    void Creating(UserContext context);
    void Created(UserContext context);
    void LoggingIn(string userNameOrEmail, string password);
    void LoggedIn(IUser user);
}
```

All implementations are called via dynamic proxy — no explicit registration needed.

### 4. Handler/Driver Pipeline

Content lifecycle processing follows a pipeline pattern:

```
Request → ContentManager → [Handler₁, Handler₂, ...] → [Driver₁, Driver₂, ...] → Shape
```

- **Handlers** respond to lifecycle events (OnCreating, OnPublishing, etc.)
- **Drivers** produce display/editor shapes

### 5. Shape-Based Rendering

UI rendering uses a shape abstraction instead of direct view references:

```
Controller → Shape("BlogPost") → ShapeTable lookup → View resolution → Razor template
```

Shapes can be overridden by themes at any level.

### 6. Convention-Based Module Discovery

Modules are discovered by convention:
- Directory presence in `Modules/` folder
- `Module.txt` manifest file declares metadata
- Automatic DI registration via `IDependency` marker interfaces

### 7. Dependency Injection Markers

```csharp
public interface IDependency {}          // Transient
public interface ISingletonDependency {} // Singleton per shell
public interface IUnitOfWorkDependency {} // Per-request
```

### 8. Null Object Pattern

Pervasive use of null objects for optional dependencies:

```csharp
public ILogger Logger { get; set; } = NullLogger.Instance;
public Localizer T { get; set; } = NullLocalizer.Instance;
```

### 9. Repository Pattern

Generic repositories abstract NHibernate access:

```csharp
IRepository<T> where T : class
  - Get(int id)
  - Table (IQueryable<T>)
  - Create(T entity)
  - Update(T entity)
  - Delete(T entity)
```

### 10. Decorator Pattern

The DI container supports transparent decoration:

```csharp
public interface IDecorator<T> { }
```

Implementations are automatically wrapped around the decorated service.

### 11. Feature Toggle Pattern

Modules declare features that can be independently enabled/disabled per tenant:

```
Features:
    Orchard.Blogs.RemotePublishing:
        Dependencies: Orchard.XmlRpc
```

## Anti-Patterns Identified

| Anti-Pattern | Location | Impact |
|-------------|----------|--------|
| Service Locator | Some older modules use `WorkContext.Resolve<T>()` | Testability issues |
| God Class | `DefaultContentManager` handles too many concerns | Complexity |
| Deep coupling to HttpContext | Throughout MVC layer | Cannot unit test easily |
| Magic strings | Route names, shape names, permission names | Refactoring risk |
| Configuration via `web.config` | Multiple web.config files | Deployment complexity |

## Architectural Styles

| Style | Implementation |
|-------|---------------|
| Modular Monolith | Single deployable with isolated modules |
| Multi-Tenant | Per-tenant DI containers and databases |
| MVC | ASP.NET MVC 5 controllers and views |
| Event-Driven | Interface-based event handlers |
| Plugin Architecture | Dynamic module loading |
| Domain-Driven Design (partial) | Content types, handlers, aggregate roots |

## Cross-References

- [System Overview](system-overview.md)
- [Components](components.md)
- [Complexity Analysis](../analysis/complexity-analysis.md)

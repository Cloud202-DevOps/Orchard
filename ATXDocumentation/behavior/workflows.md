> ⚠️ **Early Access**: Behavior documentation is in early access. Please review critically.

# Workflows - Orchard CMS

## Application-Level Workflows

### 1. Content Publishing Workflow

**Entry Point**: `ContentController.Publish()` / `IContentManager.Publish()`

```
User submits content
  → Authorization check (PublishContent permission)
  → ContentManager.Publish(contentItem)
  → Handler pipeline: OnPublishing (each handler)
    → CommonPartHandler: sets PublishedUtc timestamp
    → AutoroutePartHandler: generates/validates URL slug
    → IndexingPartHandler: marks for re-indexing
    → WorkflowManager: fires "ContentPublished" event
  → Version record updated (Published = true)
  → Previous published version: Published = false
  → Handler pipeline: OnPublished (each handler)
    → OutputCacheFilter: invalidates cached routes
    → SignalManager: triggers cache invalidation signals
  → Response returned to user
```

### 2. Request Processing Workflow

**Entry Point**: `Global.asax.cs` → MVC Pipeline

```
HTTP Request arrives
  → WarmupHttpModule: check if initialized
  → OrchardHost.BeginRequest()
  → RunningShellTable: resolve tenant from URL/host
  → Shell activation (if not active)
  → MVC Route matching within tenant routes
  → OrchardControllerFactory: resolve controller from shell container
  → Authorization filters execute
  → Action executes
  → Shape result or view result
  → Display pipeline:
    → Drivers produce shapes
    → Layout composed from zones
    → Theme view engine resolves templates
  → Response rendered and returned
```

### 3. User Authentication Workflow

**Entry Point**: `AccountController.LogOn()`

```
Login form submitted
  → Validate anti-forgery token
  → IMembershipService.ValidateUser(username, password)
    → Lookup user by username OR email
    → Verify password hash (PBKDF2)
    → Check account enabled/approved
  → If valid:
    → IAuthenticationService.SignIn(user, rememberMe)
    → Fire IUserEventHandler.LoggedIn(user)
    → Redirect to return URL or homepage
  → If invalid:
    → Fire IUserEventHandler.LogInFailed(username)
    → Display error message
    → Increment failed attempt tracking
```

### 4. Module Activation Workflow

**Entry Point**: `IFeatureManager.EnableFeatures()`

```
Admin enables a feature
  → Resolve feature dependencies (recursive)
  → For each dependency not already enabled:
    → Enable dependency first
  → IShellDescriptorManager.UpdateShellDescriptor()
  → Shell restart triggered
  → ShellContainerFactory rebuilds DI container
  → New modules loaded and initialized
  → Data migrations executed (IDataMigration)
    → Create/UpdateFrom methods called in sequence
  → Feature event handlers notified
```

### 5. Content Query Workflow (Projections)

**Entry Point**: `ProjectionManager.GetContentItems()`

```
Projection widget/page rendered
  → Load QueryPart definition
  → Build HQL query from filter/sort/layout definitions
  → Apply security trimming (only show permitted content)
  → Execute NHibernate query with pagination
  → For each result:
    → ContentManager.Get(id) with shape building
    → Apply layout (Grid, List, Raw, etc.)
  → Return composed shape
```

### 6. Background Task Execution Workflow

**Entry Point**: `BackgroundService.Sweep()`

```
Timer fires (configurable interval)
  → For each active shell/tenant:
    → Create shell work context
    → Resolve all IBackgroundTask implementations
    → For each task:
      → Begin database transaction
      → Execute task.Sweep()
      → If success: commit transaction
      → If exception: rollback, log error, continue
    → Dispose work context
```

### 7. Content Import/Export Workflow

**Entry Point**: `ImportExportManager.Import()` / `Export()`

```
Import:
  → Parse XML recipe file
  → For each content item element:
    → Resolve or create ContentItem by identity
    → Import each part's data via handlers
    → Publish if specified in recipe
  → Execute recipe steps (enable features, set settings)

Export:
  → Query content items by type/filter
  → For each item:
    → Call Export on each handler
    → Serialize to XML element
  → Package as recipe XML
```

### 8. Workflow Engine Execution

**Entry Point**: `WorkflowManager.TriggerEvent()`

```
Event fires (content published, form submitted, timer, etc.)
  → Find all workflow definitions listening for this event
  → For each matching workflow:
    → Create or resume WorkflowContext
    → Execute current activity
    → Activity returns outcomes (e.g., "Done", "Yes", "No")
    → Follow connection matching outcome
    → Execute next activity (recursive)
    → If activity is blocking: persist state, pause
    → If workflow complete: clean up state
```

## Cross-References

- [Business Logic](business-logic.md)
- [Decision Logic](decision-logic.md)
- [System Overview](../architecture/system-overview.md)

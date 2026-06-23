> ⚠️ **Early Access**: Behavior documentation is in early access. Please review critically.

# Error Handling - Orchard CMS

## Exception Handling Patterns

### 1. Controller-Level Exception Handling

Controllers use the `[HandleError]` attribute for unhandled exceptions:

```csharp
[HandleError]
public class ContentController : Controller { ... }
```

Unhandled exceptions in actions are caught by ASP.NET MVC's error filter pipeline and rendered via error views.

### 2. Background Task Error Isolation

Each background task runs in its own transaction with isolated error handling:

```
BackgroundService.Sweep():
  For each IBackgroundTask:
    try {
      BeginTransaction()
      task.Sweep()
      CommitTransaction()
    } catch (Exception ex) {
      RollbackTransaction()
      Logger.Error(ex, "Error in background task {0}", task.GetType())
      // Continue to next task - non-fatal
    }
```

**Key behavior**: One failing background task does not affect others. Errors are logged but not propagated.

### 3. Content Handler Error Propagation

Content lifecycle handlers propagate exceptions up the call stack:

- If any handler throws during `OnPublishing`, the publish operation is aborted
- The content item remains in its previous state (transactional)
- Exception surfaces to the calling controller/service

### 4. Module Loading Error Tolerance

The extension loading system is fault-tolerant:

```
ExtensionManager.LoadExtensions():
  For each module directory:
    try {
      ParseManifest()
      CompileIfNeeded()
      RegisterTypes()
    } catch (Exception ex) {
      Logger.Error("Failed to load module {0}", moduleName)
      // Module skipped, system continues
    }
```

A failing module does not prevent the system from starting.

### 5. NHibernate Transaction Management

Data operations use session-per-request with automatic transaction handling:

```
TransactionManager:
  - Begin transaction on first data access
  - Commit on successful request completion
  - Rollback on exception
  - Dispose session at end of request
```

### 6. Workflow Error Handling

Workflow activities handle errors per-activity:

- Each activity execution is wrapped in error handling
- Failed activities can:
  - Halt the workflow (mark as faulted)
  - Follow an "Error" outcome branch
  - Log and continue (based on activity configuration)
- Workflow state is persisted before each activity for recovery

### 7. Shell Initialization Error Recovery

```
Shell fails to initialize:
  → Shell enters "Disabled" state
  → Requests to that tenant return error page
  → Other tenants continue functioning
  → Admin can investigate via Setup shell
  → SafeMode theme provides minimal recovery UI
```

### 8. Dynamic Compilation Error Handling

```
Module compilation fails:
  → Error logged with full compiler output
  → Module excluded from shell
  → Shell continues with remaining modules
  → Admin notified in dashboard
```

## Error Response Patterns

| Scenario | Response |
|----------|----------|
| 404 Not Found | Custom NotFound shape rendered by theme |
| 403 Forbidden | Redirect to login or access denied page |
| 500 Internal Error | Generic error page (details in log only) |
| Tenant not found | Default tenant handles request |
| Database unavailable | Setup screen displayed |
| Module compilation error | Module disabled, admin notified |

## Logging Strategy

| Level | Usage |
|-------|-------|
| Fatal | Application cannot continue (host startup failure) |
| Error | Operation failed but system continues (module load, background task) |
| Warning | Unexpected condition handled gracefully |
| Information | Significant operations (shell start, feature enable) |
| Debug | Detailed diagnostic information |

**Logger injection pattern**:
```csharp
public ILogger Logger { get; set; }
// Initialized to NullLogger.Instance by default
// Autofac's LoggingModule injects real logger automatically
```

## Resilience Patterns

| Pattern | Implementation |
|---------|---------------|
| Circuit Breaker | Not explicitly implemented |
| Retry | Not explicitly implemented |
| Graceful Degradation | Module isolation, shell isolation |
| Fallback | SafeMode theme, default shell |
| Timeout | ASP.NET request timeout settings |

## Cross-References

- [Business Logic](business-logic.md)
- [Workflows](workflows.md)
- [Security Patterns](../analysis/security-patterns.md)

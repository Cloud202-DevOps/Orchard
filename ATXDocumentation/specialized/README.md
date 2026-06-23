# Specialized Documentation - Orchard CMS

## Database Schemas and Query Patterns

### Schema Management

Orchard uses a **code-first migration** pattern via `IDataMigration`:

```csharp
public class Migrations : DataMigrationImpl {
    public int Create() {
        SchemaBuilder.CreateTable("MyPartRecord", table => table
            .ContentPartRecord()
            .Column<string>("MyField", c => c.WithLength(255))
        );
        return 1;
    }
    
    public int UpdateFrom1() {
        SchemaBuilder.AlterTable("MyPartRecord", table => table
            .AddColumn<bool>("NewField", c => c.WithDefault(false))
        );
        return 2;
    }
}
```

### Core Tables

| Table | Purpose |
|-------|---------|
| `Orchard_Framework_ContentItemRecord` | All content items |
| `Orchard_Framework_ContentItemVersionRecord` | Version history |
| `Orchard_Framework_ContentTypeRecord` | Content type registry |
| `Orchard_Framework_DataMigrationRecord` | Migration version tracking |
| `Orchard_Framework_ShellDescriptorRecord` | Enabled features per tenant |
| `Common_CommonPartRecord` | Dates, ownership |
| `Common_CommonPartVersionRecord` | Version-specific dates |
| `Common_BodyPartRecord` | Content body text |
| `Title_TitlePartRecord` | Content titles |
| `Orchard_Users_UserPartRecord` | User credentials |
| `Orchard_Roles_RoleRecord` | Role definitions |
| `Orchard_Roles_PermissionRecord` | Permission assignments |
| `Orchard_Roles_UserRolesPartRecord` | User-role mappings |

### Query Patterns

**Content queries use NHibernate HQL/Criteria API:**

```csharp
// Query published blog posts
contentManager.Query<BlogPostPart, BlogPostPartRecord>()
    .Where(x => x.BlogId == blogId)
    .OrderByDescending(x => x.CreatedUtc)
    .Slice(skip, count);
```

**Projections build dynamic HQL:**
- Filter groups with AND/OR logic
- Sort criteria chain
- Layout determines rendering
- Paging applied at query level

### Multi-Tenant Database Strategies

| Strategy | Description |
|----------|-------------|
| Separate databases | Each tenant has own database |
| Shared with prefix | Single DB, tables prefixed per tenant |
| Shared | Same tables, tenant isolation via Shell filtering |

## Infrastructure and Deployment

### IIS Configuration Requirements

```xml
<!-- web.config key sections -->
<system.web>
  <authentication mode="Forms" />
  <compilation targetFramework="4.8">
    <!-- Roslyn compiler for dynamic compilation -->
  </compilation>
  <httpRuntime targetFramework="4.8" maxRequestLength="..." />
</system.web>

<system.webServer>
  <modules>
    <add name="WarmupHttpModule" />
  </modules>
</system.webServer>
```

### Deployment Artifacts

| Artifact | Location | Purpose |
|----------|----------|---------|
| Web application | IIS site root | Application files |
| App_Data | Site root/App_Data | Tenant data, logs, dependencies |
| Dependencies | App_Data/Dependencies | Dynamically compiled assemblies |
| Sites | App_Data/Sites/{TenantName} | Per-tenant settings and data |
| Logs | App_Data/Logs | log4net output |
| Media | Media/ or Azure Blob | Uploaded files |

### CI/CD Pipeline (GitHub Actions)

```yaml
# .github/workflows/compile.yml
- MSBuild on Windows runner
- .NET Framework SDK (not .NET Core)
- NuGet restore
- Solution build
```

## Message/Event Patterns

### Interface-Based Events

Orchard's event system uses marker interfaces:

```csharp
public interface IContentHandler : IEventHandler { }
public interface IUserEventHandler : IEventHandler {
    void Creating(UserContext context);
    void Created(UserContext context);
    void LoggingIn(string userNameOrEmail, string password);
    void LoggedIn(IUser user);
    void LogInFailed(string userNameOrEmail, string password);
}
```

All registered implementations are called via Autofac multi-binding and dynamic proxy.

### Signal-Based Cache Events

```csharp
// Trigger
_signals.Trigger("ContentChanged:BlogPost");

// Listen (in cache acquisition)
_cacheManager.Get("key", ctx => {
    ctx.Monitor(_signals.When("ContentChanged:BlogPost"));
    return ComputeExpensiveValue();
});
```

### Workflow Events

Events that trigger workflow execution:
- Content created/published/unpublished/removed
- User logged in/registered
- Form submitted
- Timer fired
- Custom events via `IWorkflowManager.TriggerEvent()`

## Cross-References

- [Data Models](../reference/data-models.md)
- [System Overview](../architecture/system-overview.md)
- [Workflows](../behavior/workflows.md)

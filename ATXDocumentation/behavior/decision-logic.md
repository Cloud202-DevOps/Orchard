> ⚠️ **Early Access**: Behavior documentation is in early access. Please review critically.

# Decision Logic - Orchard CMS

## Content Access Decisions

### Authorization Decision Tree

```
CheckAccess(permission, user, content):
  │
  ├─ User is SuperUser? → GRANT
  │
  ├─ Get user's effective roles
  │   └─ For each role:
  │       ├─ Role has SiteOwner permission? → GRANT
  │       ├─ Role has exact permission? → GRANT
  │       └─ Permission has implied permissions?
  │           └─ Recursively check implied → GRANT if found
  │
  ├─ Content-level permissions:
  │   ├─ "Own" variant exists? (e.g., EditOwnContent)
  │   │   └─ Is user the content owner? → Check "Own" permission
  │   └─ No content context → Check permission directly
  │
  └─ No matching permission found → DENY
```

### Content Visibility Decision

```
GetContentItem(id, options):
  │
  ├─ options.IsPublished?
  │   └─ Return version where Published=true AND Latest=true
  │
  ├─ options.IsDraft?
  │   └─ Return version where Latest=true (regardless of published state)
  │
  ├─ options.VersionNumber specified?
  │   └─ Return specific version
  │
  └─ Default: Return published version
```

## Tenant Resolution Decision

```
Incoming HTTP Request:
  │
  ├─ Check RunningShellTable for URL prefix match
  │   └─ Match found? → Route to that shell
  │
  ├─ Check host header against shell settings
  │   └─ Match found? → Route to that shell
  │
  └─ No match → Route to Default shell
```

## Module Loading Decisions

### Feature Dependency Resolution

```
EnableFeature(feature):
  │
  ├─ Already enabled? → No-op, return
  │
  ├─ Has dependencies?
  │   └─ For each dependency:
  │       ├─ Dependency available? → EnableFeature(dependency) recursively
  │       └─ Dependency not found? → FAIL with error
  │
  ├─ Check for conflicts (OrchardSuppressDependency):
  │   └─ Feature suppresses another? → Disable suppressed service
  │
  └─ Add to shell descriptor → Trigger shell restart
```

### Extension Loading Priority

```
Multiple implementations of same interface:
  │
  ├─ Check [OrchardSuppressDependency] → Suppressed impl removed
  │
  ├─ Check [IDecorator<T>] → Decorator wraps inner
  │
  ├─ Check Priority property → Higher priority wins
  │
  └─ Default: All implementations registered (multi-binding)
```

## Data Migration Decisions

```
Shell startup - check migrations:
  │
  ├─ For each enabled feature with IDataMigration:
  │   ├─ Get current schema version from DataMigrationRecord
  │   ├─ Find next UpdateFromN() method
  │   │   ├─ Method exists? → Execute, update version, repeat
  │   │   └─ No more methods? → Migration complete
  │   └─ No record exists?
  │       └─ Execute Create() method, set version to 1
  │
  └─ All migrations complete → Shell ready
```

## Caching Decisions

### Output Cache Decision

```
Request arrives:
  │
  ├─ Is authenticated user? → Check cache settings for authenticated
  │
  ├─ Route has [OutputCache] or matching rule?
  │   ├─ Cache entry exists and not expired? → SERVE FROM CACHE
  │   ├─ Cache entry exists but expired, grace time active?
  │   │   └─ SERVE STALE + trigger background refresh
  │   └─ No cache entry → EXECUTE, CACHE RESULT
  │
  └─ No cache rule → BYPASS CACHE
```

### Signal-Based Cache Invalidation

```
Content published/changed:
  │
  ├─ Trigger signal: "ContentChanged:{ContentType}"
  │
  ├─ All cache entries with matching volatile token → INVALIDATED
  │
  └─ Next request rebuilds cache
```

## Routing Decisions

### Autoroute URL Generation

```
Content item needs URL:
  │
  ├─ Custom URL set? → Use custom URL
  │
  ├─ Pattern defined for content type?
  │   ├─ Evaluate pattern tokens (e.g., {Content.Slug})
  │   ├─ Generated URL already exists?
  │   │   └─ Append incrementing suffix (-2, -3, ...)
  │   └─ Store generated URL
  │
  └─ No pattern → No automatic URL
```

## Workflow Activity Decisions

```
Activity executes:
  │
  ├─ Activity type: Decision
  │   └─ Evaluate condition → Follow "Yes" or "No" branch
  │
  ├─ Activity type: Branch
  │   └─ Execute all outgoing connections in parallel
  │
  ├─ Activity type: Timer
  │   └─ Persist state, schedule wake-up → PAUSE
  │
  ├─ Activity type: Event
  │   └─ Wait for external event → PAUSE
  │
  └─ Activity returns outcomes → Follow matching connection
```

## Cross-References

- [Business Logic](business-logic.md)
- [Workflows](workflows.md)
- [Security Patterns](../analysis/security-patterns.md)

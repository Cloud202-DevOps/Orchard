> ⚠️ **Early Access**: Behavior documentation is in early access. Please review critically.

# Business Logic - Orchard CMS

## Content Management Logic

### Content Item Lifecycle

Every content item follows a strict lifecycle managed by `DefaultContentManager`:

| Operation | Business Rule |
|-----------|--------------|
| **Create** | Allocates new ContentItemRecord, assigns version 1, triggers OnCreating/OnCreated handlers |
| **Get (Published)** | Retrieves latest published version; returns null if unpublished or removed |
| **Get (Draft)** | Returns draft version if exists, otherwise creates new draft from published |
| **Publish** | Sets `Published=true`, removes published flag from previous version, triggers OnPublishing/OnPublished |
| **Unpublish** | Removes published flag, content becomes invisible to public queries |
| **Remove** | Soft-delete via handlers, content no longer returned by queries |
| **Version** | Creates new ContentItemVersionRecord linked to same ContentItemRecord |

### Content Type Composition Rules

- A ContentType is composed of one or more ContentParts
- Each ContentPart can only appear once per ContentType
- ContentFields can appear multiple times per part (distinguished by name)
- Parts can have Records (database-backed) or be purely behavioral
- Stereotype attribute determines admin UI behavior (Content, Widget, MenuItem)

### Content Versioning

- Every edit creates a new version record
- Only one version can be "Published" at a time
- Draft versions are visible only to authorized editors
- Version history is maintained indefinitely (no auto-pruning)

## User Management Logic (`Orchard.Users`)

### Registration Rules

| Rule | Logic |
|------|-------|
| Username uniqueness | Case-insensitive check against existing users |
| Email uniqueness | Optional, configurable per site settings |
| Password validation | Minimum length configured in site settings |
| Email verification | Optional, sends nonce-based verification link |
| Account approval | Optional, requires admin approval before login |
| Registration open/closed | Site-level setting |

### Authentication Rules

| Rule | Logic |
|------|-------|
| Login attempts | Username or email lookup, password hash comparison |
| Account locked | Users can be disabled by admin |
| Password hashing | PBKDF2 with per-user salt, configurable iterations |
| Remember me | Forms authentication ticket with extended expiry |
| Session management | ASP.NET Forms Authentication cookies |

## Authorization Logic (`Orchard.Roles`)

### Permission Model

```
Permission → Role → User

Check: Does User (via Roles) have Permission [on Content]?
```

- Permissions are defined per module (static Permission objects)
- Roles aggregate permissions
- Special roles: `Administrator` (implicit all permissions), `Anonymous`, `Authenticated`
- Content-level permissions check ownership (e.g., "Edit own content")
- Permission implies: Some permissions imply others (e.g., `PublishContent` implies `EditContent`)

## Workflow Engine (`Orchard.Workflows`)

### Workflow Execution Rules

- Workflows are triggered by events (content published, user logged in, form submitted, etc.)
- Activities execute sequentially along connections
- Branch activities create parallel execution paths
- Timer activities schedule delayed continuation
- Workflows maintain state in database between activities
- Failed activities can halt or continue based on configuration

## Media Library Logic (`Orchard.MediaLibrary`)

### Upload Rules

| Rule | Logic |
|------|-------|
| Allowed extensions | Whitelist configured in settings |
| Max file size | Configurable per site |
| Storage location | File system or Azure Blob (configurable) |
| Folder organization | Hierarchical folder structure |
| Media profiles | Image resize/crop presets |

## Autoroute Logic (`Orchard.Autoroute`)

### URL Generation Rules

- Pattern-based URL generation using tokens (e.g., `{Content.Slug}`)
- Unique URL enforcement (appends `-2`, `-3` for duplicates)
- Custom URL override per content item
- URL regeneration on title change (configurable)
- Home page designation via autoroute

## Caching Logic (`Orchard.OutputCache`)

### Cache Invalidation Rules

- Cache entries keyed by URL + query string + authenticated state
- Invalidation on content publish/unpublish/remove
- Grace time allows serving stale content during regeneration
- Per-route cache duration configuration
- Cache exclusion rules for dynamic pages

## Localization Logic (`Orchard.Localization`)

### Translation Rules

- Content items linked via culture group (shared identity across languages)
- Fallback to default culture if translation unavailable
- Culture determined by URL prefix, cookie, or browser header
- Admin UI allows side-by-side translation editing

## Cross-References

- [Workflows](workflows.md)
- [Decision Logic](decision-logic.md)
- [Error Handling](error-handling.md)
- [Components](../architecture/components.md)

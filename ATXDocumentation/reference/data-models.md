# Data Models - Orchard CMS

## Core Content Records (NHibernate)

### ContentItemRecord

Primary entity for all content in the system.

| Field | Type | Description |
|-------|------|-------------|
| Id | int | Primary key |
| ContentType | ContentTypeRecord | FK to content type definition |
| Versions | IList<ContentItemVersionRecord> | All versions of this item |

### ContentItemVersionRecord

Versioned state of a content item.

| Field | Type | Description |
|-------|------|-------------|
| Id | int | Primary key |
| ContentItemRecord | ContentItemRecord | Parent content item |
| Number | int | Version number |
| Published | bool | Is this the published version |
| Latest | bool | Is this the latest version |
| Data | string | Serialized Infoset data (XML) |

### ContentTypeRecord

| Field | Type | Description |
|-------|------|-------------|
| Id | int | Primary key |
| Name | string | Content type identifier |

## Part Records

Each ContentPart with persistent state has a corresponding Record mapped via NHibernate:

### CommonPartRecord

| Field | Type | Description |
|-------|------|-------------|
| OwnerId | int | FK to owner user content item |
| CreatedUtc | DateTime? | Creation timestamp |
| PublishedUtc | DateTime? | Publication timestamp |
| ModifiedUtc | DateTime? | Last modification timestamp |
| Container | ContentItemRecord | Parent container item |

### CommonPartVersionRecord

| Field | Type | Description |
|-------|------|-------------|
| CreatedUtc | DateTime? | Version creation time |
| PublishedUtc | DateTime? | Version publication time |
| ModifiedUtc | DateTime? | Version modification time |

### BodyPartRecord

| Field | Type | Description |
|-------|------|-------------|
| Text | string | Body content (HTML/markdown) |
| Format | string | Content format identifier |

### TitlePartRecord

| Field | Type | Description |
|-------|------|-------------|
| Title | string | Content item title |

### AutoroutePartRecord

| Field | Type | Description |
|-------|------|-------------|
| DisplayAlias | string | URL slug (e.g., "blog/my-post") |
| CustomPattern | string | Custom URL pattern |
| UseCustomPattern | bool | Whether custom pattern is used |
| Published | bool | Route is published |

### UserPartRecord

| Field | Type | Description |
|-------|------|-------------|
| UserName | string | Login username |
| Email | string | Email address |
| NormalizedUserName | string | Lowercased username for lookup |
| HashAlgorithm | string | Password hash algorithm |
| Password | string | Hashed password |
| PasswordFormat | string | Password storage format |
| PasswordSalt | string | Per-user salt |
| RegistrationStatus | string | Pending/Approved |
| EmailStatus | string | Pending/Approved |

### UserRolesPartRecord

| Field | Type | Description |
|-------|------|-------------|
| UserId | int | FK to user content item |
| Role | RoleRecord | FK to role |

### RoleRecord

| Field | Type | Description |
|-------|------|-------------|
| Id | int | Primary key |
| Name | string | Role name |

### PermissionRecord

| Field | Type | Description |
|-------|------|-------------|
| Id | int | Primary key |
| Name | string | Permission identifier |
| FeatureName | string | Owning feature |
| RoleRecord | RoleRecord | FK to role |

## Workflow Records

### WorkflowDefinitionRecord

| Field | Type | Description |
|-------|------|-------------|
| Id | int | Primary key |
| Name | string | Workflow name |
| Enabled | bool | Is active |
| Activities | IList<ActivityRecord> | Activity definitions |
| Transitions | IList<TransitionRecord> | Activity connections |

### ActivityRecord

| Field | Type | Description |
|-------|------|-------------|
| Id | int | Primary key |
| Name | string | Activity type name |
| State | string | Serialized activity state |
| Start | bool | Is starting activity |

### WorkflowRecord

| Field | Type | Description |
|-------|------|-------------|
| Id | int | Primary key |
| WorkflowDefinitionRecord | WorkflowDefinitionRecord | Parent definition |
| State | string | Serialized workflow state |
| ContentItemRecord | ContentItemRecord | Associated content (optional) |

## Shell and Tenant Data

### ShellDescriptorRecord

| Field | Type | Description |
|-------|------|-------------|
| SerialNumber | int | Version counter |
| Features | IList<ShellFeatureRecord> | Enabled features |
| Parameters | IList<ShellParameterRecord> | Shell parameters |

### ShellSettings (file-based, not NHibernate)

Stored in `App_Data/Sites/{TenantName}/Settings.txt`:

| Setting | Description |
|---------|-------------|
| Name | Tenant identifier |
| DataProvider | Database type (SqlServer, SqlCe, etc.) |
| DataConnectionString | Database connection |
| DataTablePrefix | Table prefix for shared DBs |
| RequestUrlHost | Hostname binding |
| RequestUrlPrefix | URL prefix binding |
| State | Running/Disabled/Uninitialized |

## Infoset Storage

Modern Orchard uses **Infoset** (XML-serialized properties in `ContentItemVersionRecord.Data`) for part data instead of separate database tables. This provides:
- Schema-less storage for simple properties
- No database migration needed for new fields
- Coexistence with traditional record-based storage

## Cross-References

- [Program Structure](program-structure.md)
- [Dependencies](../architecture/dependencies.md)
- [Interfaces](interfaces.md)

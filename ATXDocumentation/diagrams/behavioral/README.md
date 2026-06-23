# Behavioral Diagrams - Orchard CMS

## Sequence Diagram - Content Publishing

```
User          Controller      ContentManager    Handlers         Database
 │                │                │               │                │
 │ Publish(id)    │                │               │                │
 │───────────────►│                │               │                │
 │                │ Publish(item)  │               │                │
 │                │───────────────►│               │                │
 │                │                │ OnPublishing() │                │
 │                │                │──────────────►│                │
 │                │                │               │ Update version  │
 │                │                │               │───────────────►│
 │                │                │               │◄───────────────│
 │                │                │◄──────────────│                │
 │                │                │ Set Published=T│                │
 │                │                │───────────────────────────────►│
 │                │                │◄──────────────────────────────│
 │                │                │ OnPublished()  │                │
 │                │                │──────────────►│                │
 │                │                │  (invalidate   │                │
 │                │                │   cache, index)│                │
 │                │                │◄──────────────│                │
 │                │◄───────────────│               │                │
 │◄───────────────│ Redirect       │               │                │
 │                │                │               │                │
```

## Sequence Diagram - HTTP Request Processing

```
Browser     IIS    WarmupModule   ShellTable    Controller    ShapeEngine   Theme
  │          │          │            │              │             │           │
  │ Request  │          │            │              │             │           │
  │─────────►│          │            │              │             │           │
  │          │ Module   │            │              │             │           │
  │          │─────────►│            │              │             │           │
  │          │          │ Resolve    │              │             │           │
  │          │          │ tenant     │              │             │           │
  │          │          │───────────►│              │             │           │
  │          │          │◄───────────│              │             │           │
  │          │          │ Route      │              │             │           │
  │          │          │ match      │  Action()    │             │           │
  │          │          │───────────────────────────►             │           │
  │          │          │            │              │ Shape()     │           │
  │          │          │            │              │────────────►│           │
  │          │          │            │              │             │ Template  │
  │          │          │            │              │             │──────────►│
  │          │          │            │              │             │◄──────────│
  │          │          │            │              │◄────────────│           │
  │          │          │◄─────────────────────────│             │           │
  │◄─────────│          │            │              │             │           │
  │ Response │          │            │              │             │           │
```

## Activity Diagram - User Registration

```
Start
  │
  ▼
[Display Registration Form]
  │
  ▼
<Registration Open?> ──No──► [Display "Registration Closed"] ──► End
  │
  Yes
  │
  ▼
[User Submits Form]
  │
  ▼
<Username Unique?> ──No──► [Display Error: "Username taken"] ──► [Form]
  │
  Yes
  │
  ▼
<Email Required & Unique?> ──No──► [Display Error] ──► [Form]
  │
  Yes
  │
  ▼
<Password Valid?> ──No──► [Display Error: "Password too short"] ──► [Form]
  │
  Yes
  │
  ▼
[Create User Record (hash password)]
  │
  ▼
<Email Verification Required?>
  │              │
  Yes            No
  │              │
  ▼              ▼
[Send Email]   <Approval Required?>
  │              │           │
  ▼             Yes          No
[Pending]        │           │
  │              ▼           ▼
  │         [Pending]   [Approved]
  │              │           │
  └──────────────┴───────────┘
                 │
                 ▼
         [Fire UserCreated Event]
                 │
                 ▼
               End
```

## State Diagram - Content Item Lifecycle

```
                    ┌─────────┐
                    │  Draft  │◄──────────────────────┐
                    └────┬────┘                       │
                         │                            │
                    Publish()                    Unpublish()
                         │                            │
                         ▼                            │
                    ┌─────────┐                       │
         ┌────────►│Published│───────────────────────┘
         │         └────┬────┘
         │              │
    New Version    Remove()
         │              │
         │              ▼
         │         ┌─────────┐
         └─────────│ Removed │
                   └─────────┘
```

## Data Flow Diagram - Content Query

```
┌──────────┐     Query      ┌────────────────┐     HQL      ┌──────────────┐
│ User     │───────────────►│ ProjectionMgr  │─────────────►│  NHibernate  │
│ Request  │                │                │              │              │
└──────────┘                └────────┬───────┘              └──────┬───────┘
                                     │                             │
                                     │ Content IDs                 │ SQL
                                     ▼                             ▼
                            ┌────────────────┐              ┌──────────────┐
                            │ ContentManager │              │   Database   │
                            │  .Get(ids)     │◄─────────────│              │
                            └────────┬───────┘   Records    └──────────────┘
                                     │
                                     │ Shapes
                                     ▼
                            ┌────────────────┐
                            │  Display Mgr   │
                            │  (Drivers)     │
                            └────────┬───────┘
                                     │
                                     │ HTML
                                     ▼
                            ┌────────────────┐
                            │   Response     │
                            └────────────────┘
```

## Cross-References

- [Workflows](../../behavior/workflows.md)
- [Decision Logic](../../behavior/decision-logic.md)
- [Business Logic](../../behavior/business-logic.md)

# Structural Diagrams - Orchard CMS

## Component Diagram

```
┌─────────────────────────────────────────────────────────────────────────┐
│                            Orchard CMS                                   │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│  ┌────────────────────────────────────────────────────────────────┐     │
│  │                    Presentation Layer                           │     │
│  │  ┌──────────┐ ┌──────────────┐ ┌───────────┐ ┌────────────┐  │     │
│  │  │Controllers│ │ Shape Engine │ │View Engine│ │  Themes    │  │     │
│  │  └──────────┘ └──────────────┘ └───────────┘ └────────────┘  │     │
│  └────────────────────────────────────────────────────────────────┘     │
│                              │                                           │
│  ┌────────────────────────────────────────────────────────────────┐     │
│  │                    Application Layer                            │     │
│  │  ┌──────────────┐ ┌──────────┐ ┌──────────┐ ┌─────────────┐  │     │
│  │  │Content Mgmt  │ │Workflows │ │ Security │ │   Caching   │  │     │
│  │  └──────────────┘ └──────────┘ └──────────┘ └─────────────┘  │     │
│  │  ┌──────────────┐ ┌──────────┐ ┌──────────┐ ┌─────────────┐  │     │
│  │  │  Indexing    │ │  Media   │ │Localization│ │   Tasks    │  │     │
│  │  └──────────────┘ └──────────┘ └──────────┘ └─────────────┘  │     │
│  └────────────────────────────────────────────────────────────────┘     │
│                              │                                           │
│  ┌────────────────────────────────────────────────────────────────┐     │
│  │                    Infrastructure Layer                         │     │
│  │  ┌──────────────┐ ┌──────────┐ ┌──────────┐ ┌─────────────┐  │     │
│  │  │ NHibernate   │ │  Autofac │ │ log4net  │ │ FileSystem  │  │     │
│  │  └──────────────┘ └──────────┘ └──────────┘ └─────────────┘  │     │
│  └────────────────────────────────────────────────────────────────┘     │
│                              │                                           │
│  ┌────────────────────────────────────────────────────────────────┐     │
│  │                    Platform Layer                               │     │
│  │  ┌──────────────┐ ┌──────────┐ ┌──────────┐ ┌─────────────┐  │     │
│  │  │.NET Fwk 4.8  │ │   IIS    │ │System.Web│ │  Database   │  │     │
│  │  └──────────────┘ └──────────┘ └──────────┘ └─────────────┘  │     │
│  └────────────────────────────────────────────────────────────────┘     │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘
```

## Class Diagram - Content Management Core

```
┌────────────────────────┐        ┌─────────────────────────┐
│    ContentItem         │        │   ContentTypeDefinition  │
├────────────────────────┤        ├─────────────────────────┤
│ + Id: int              │◄───────│ + Name: string           │
│ + ContentType: string  │        │ + Parts: List<PartDef>   │
│ + Parts: List<Part>    │        │ + Settings: Dictionary   │
│ + VersionRecord        │        └─────────────────────────┘
└────────────────────────┘
         │ has many
         ▼
┌────────────────────────┐        ┌─────────────────────────┐
│    ContentPart         │        │   ContentPartRecord     │
├────────────────────────┤        ├─────────────────────────┤
│ + PartDefinition       │───────►│ + Id: int               │
│ + Fields: List<Field>  │        │ + ContentItemRecord     │
│ + Settings             │        └─────────────────────────┘
└────────────────────────┘
         △ inherits
         │
    ┌────┼────┬──────────┬──────────────┐
    │         │          │              │
┌───┴───┐ ┌──┴───┐ ┌────┴────┐ ┌──────┴──────┐
│Title  │ │Body  │ │Common   │ │Autoroute    │
│Part   │ │Part  │ │Part     │ │Part         │
└───────┘ └──────┘ └─────────┘ └─────────────┘
```

## Package Dependency Graph

```
src/Orchard.Web (Application Host)
    ├── src/Orchard (Core Framework)
    ├── src/Orchard.Web/Core/* (Core Modules)
    │       └── src/Orchard
    ├── src/Orchard.Web/Modules/* (Feature Modules)
    │       ├── src/Orchard
    │       └── src/Orchard.Web/Core/*
    ├── src/Orchard.Azure (Optional)
    │       └── src/Orchard
    └── External NuGet Packages
            ├── NHibernate 5.6.0
            ├── Autofac 3.5.2
            ├── Microsoft.AspNet.Mvc 5.3.0
            ├── Microsoft.Owin 4.2.3
            └── Castle.Core 3.3.1
```

## Module Dependency Map (Key Modules)

```
                    ┌─────────────┐
                    │    Core     │
                    └──────┬──────┘
                           │
          ┌────────────────┼────────────────┐
          │                │                │
    ┌─────┴─────┐   ┌─────┴─────┐   ┌─────┴─────┐
    │  Tokens   │   │  Fields   │   │   Users   │
    └─────┬─────┘   └─────┬─────┘   └─────┬─────┘
          │                │                │
    ┌─────┴─────┐   ┌─────┴─────┐   ┌─────┴─────┐
    │ Autoroute │   │Projections│   │   Roles   │
    └─────┬─────┘   └───────────┘   └───────────┘
          │
    ┌─────┴─────┐
    │   Blogs   │
    └───────────┘
```

## Cross-References

- [System Overview](../../architecture/system-overview.md)
- [Components](../../architecture/components.md)
- [Program Structure](../../reference/program-structure.md)

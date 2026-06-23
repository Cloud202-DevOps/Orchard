# Code Metrics - Orchard CMS

## Size Metrics

| Metric | Value |
|--------|-------|
| Total source lines (C#, CSHTML, JS, CSS, SCSS) | ~659,000 |
| C# source files | 3,675 |
| JavaScript files | 1,015 |
| Razor view files | 876 |
| Configuration files | 315 |
| CSS/SCSS files | 318 |
| Projects in solution | 88 |
| Feature modules | 71 |
| Core modules | 11 |
| Test projects | 7 |
| SpecFlow feature files | 29 |

## Module Size Distribution

| Category | Module Count | Typical Size (files) |
|----------|-------------|---------------------|
| Large modules (50+ files) | ~10 | Workflows, MediaLibrary, Projections, Layouts |
| Medium modules (20-50 files) | ~25 | Blogs, Users, Roles, Taxonomies |
| Small modules (< 20 files) | ~36 | Tags, PublishLater, SecureSocketsLayer |

## Code Organization Quality

| Indicator | Assessment |
|-----------|-----------|
| Separation of concerns | Good — clear layer separation (Controller/Service/Handler/Driver) |
| Interface usage | High — extensive interface-based programming |
| Code duplication | Low — shared framework prevents duplication |
| Naming conventions | Consistent — follows .NET/Orchard naming standards |
| Namespace organization | Good — logical grouping by feature area |

## Test Coverage Indicators

| Test Type | Project | File Count |
|-----------|---------|-----------|
| Unit tests | Orchard.Tests | ~100+ test files |
| Module tests | Orchard.Tests.Modules | ~50+ test files |
| Core tests | Orchard.Core.Tests | ~30+ test files |
| Web tests | Orchard.Web.Tests | ~10+ test files |
| BDD tests | Orchard.Specs | 29 feature files |
| Azure tests | Orchard.Azure.Tests | ~10+ test files |

## Dependency Metrics

| Metric | Value |
|--------|-------|
| External NuGet packages | ~30+ (via packages.config) |
| Module inter-dependencies | Moderate (well-defined via Module.txt) |
| System.Web import locations | 400+ |
| Assembly binding redirects | Multiple (in web.config) |

## Cross-References

- [Complexity Analysis](complexity-analysis.md)
- [Dependency Analysis](dependency-analysis.md)
- [Program Structure](../reference/program-structure.md)

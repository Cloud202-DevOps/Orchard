# Maintenance Burden - Orchard CMS

## High Maintenance Areas

### 1. System.Web Coupling (80+ files, 400+ imports)

**Burden**: Any attempt to modernize, containerize, or port to Linux requires addressing deep System.Web dependencies throughout the entire framework layer.

**Affected Areas**:
- `src/Orchard/Mvc/` — Custom controller factory, view engines, model binders
- `src/Orchard/Environment/` — Shell infrastructure using `HttpApplication`, `HostingEnvironment`
- `src/Orchard/Security/` — `FormsAuthentication`, `MachineKey`
- `src/Orchard/DisplayManagement/` — `VirtualPathProvider` for view resolution
- All 71 modules — Controllers inheriting from `System.Web.Mvc.Controller`

**Impact**: Cannot incrementally modernize; requires coordinated rewrite across framework and all modules.

### 2. Dynamic Compilation Infrastructure

**Burden**: The dynamic compilation system (Roslyn + BuildManager) adds complexity to deployment and prevents containerization.

**Affected Components**:
- `ExtensionLoaderCoordinator` — Orchestrates module compilation
- `DynamicExtensionLoader` — Compiles modules from source at runtime
- `App_Data/Dependencies/` — Runtime assembly cache
- Assembly probing configuration in web.config

**Impact**: Deployments are non-deterministic; same source can produce different assemblies. Makes Docker images unreliable.

### 3. Multi-Database Support

**Burden**: Supporting 5 database providers (SQL Server, SqlCe, MySQL, PostgreSQL, SQLite) with NHibernate adds testing and compatibility burden.

**Affected Components**:
- Connection string management per tenant
- Schema migration compatibility across providers
- Provider-specific SQL generation
- SqlCe provider discontinued — no new fixes

**Impact**: Schema changes must be validated across all providers. SqlCe support is a dead end.

### 4. Legacy Frontend Build Pipeline

**Burden**: Gulp 3.9.1 is incompatible with modern Node.js and requires specific legacy Node.js versions.

**Affected Files**:
- `src/Package.json` — Gulp 3.x dependencies
- `Gulpfile.js` — Task definitions using deprecated API
- All `.less`, `.scss`, `.ts` files depend on this pipeline

**Impact**: Cannot use current Node.js LTS. Build breaks on developer machines without specific Node.js version pinning.

### 5. NuGet `packages.config` Management

**Burden**: Traditional `packages.config` files (9+ across solution) instead of modern PackageReference format.

**Impact**: 
- No transitive dependency management
- Assembly binding redirects required manually
- Cannot use modern NuGet features (central package management, floating versions)
- Each project independently tracks its own dependency versions

### 6. 88-Project Solution

**Burden**: The solution contains 88 C# projects, making builds slow and IDE performance degraded.

**Impact**:
- Full build is resource-intensive
- Solution load time is significant
- Refactoring across projects requires careful coordination
- Test execution across 7 test projects is time-consuming

### 7. SpecFlow BDD Tests (29 feature files)

**Burden**: SpecFlow tests depend on running IIS Express and actual browser automation.

**Impact**:
- Tests are brittle (depend on timing, UI layout)
- Cannot run in containers easily
- Require Windows + IIS Express
- Slow execution compared to unit tests

## Maintenance Cost Indicators

| Area | Complexity | Change Frequency Risk | Testing Difficulty |
|------|-----------|----------------------|-------------------|
| System.Web coupling | High | Low (stable but frozen) | High (requires IIS) |
| Dynamic compilation | High | Low | High (non-deterministic) |
| Multi-DB support | Medium | Low | High (5 providers) |
| Frontend build | Medium | Medium (Node.js updates break it) | Low |
| NuGet packages.config | Low | Medium (dependency updates) | Low |
| 88-project solution | Medium | High (any cross-cutting change) | Medium |

## Operational Maintenance

| Concern | Current State | Ideal State |
|---------|--------------|-------------|
| Deployment | XCOPY to IIS, manual | CI/CD to containers |
| Monitoring | Glimpse (abandoned) + log4net files | APM + structured logging |
| Scaling | IIS app pool + load balancer | Container orchestration |
| Configuration | web.config per environment | Environment variables, config service |
| Secret management | Machine keys in web.config | Key vault/secrets manager |

## Cross-References

- [Summary](summary.md)
- [Outdated Components](outdated-components.md)
- [Remediation Plan](remediation-plan.md)
- [Complexity Analysis](../analysis/complexity-analysis.md)

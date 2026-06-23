# Remediation Plan - Orchard CMS

## Prioritized Action Items

### Priority 1: Critical — Platform Modernization (High Severity)

#### Action 1.1: Evaluate Migration to Orchard Core

**Rationale**: Orchard Core is the official rewrite of Orchard CMS on ASP.NET Core. It addresses all High-severity platform debt.

**Steps**:
1. Assess feature parity between Orchard 1.x modules and Orchard Core
2. Identify custom modules requiring porting
3. Plan content migration (database schema differences)
4. Evaluate Orchard Core's multi-tenancy for existing tenant configurations

**Addresses**: .NET Framework 4.8, ASP.NET MVC 5, System.Web coupling, dynamic compilation

#### Action 1.2: Upgrade Autofac (if staying on current platform)

**Rationale**: Autofac 3.5.2 is EOL with no security patches.

**Steps**:
1. Review Autofac 3.x → 4.x → 5.x → 6.x → 7.x → 8.x migration guides
2. Update `ContainerBuilder` API usage (breaking changes in 4.x and 6.x)
3. Replace deprecated registration methods
4. Update `ShellContainerFactory` and host container setup
5. Run full test suite after each major version jump

**Complexity**: Medium — API changes are well-documented but numerous

#### Action 1.3: Modernize Frontend Build

**Rationale**: Gulp 3.9.1 is incompatible with modern Node.js.

**Steps**:
1. Migrate to Gulp 5.x (update task syntax from `gulp.task()` to exports)
2. Or replace with modern alternative (Vite, Webpack 5, esbuild)
3. Update all gulp plugins to compatible versions
4. Verify LESS/SCSS/TypeScript compilation produces identical output
5. Update CI/CD pipeline Node.js version

**Complexity**: Medium

### Priority 2: Important — Dependency Updates (Medium Severity)

#### Action 2.1: Remove Glimpse

**Rationale**: Abandoned project with no security patches.

**Steps**:
1. Remove Glimpse NuGet packages
2. Remove Glimpse configuration from web.config
3. Replace with MiniProfiler or Application Insights (optional)

**Complexity**: Low

#### Action 2.2: Remove SQL Server Compact Support

**Rationale**: Discontinued by Microsoft.

**Steps**:
1. Remove SqlCe NuGet package references
2. Remove SqlCe-specific data provider code
3. Update documentation to recommend SQLite for embedded scenarios
4. Provide migration path for existing SqlCe databases

**Complexity**: Low

#### Action 2.3: Update Castle.Core

**Rationale**: 2 major versions behind, missing performance improvements.

**Steps**:
1. Update Castle.Core to 5.x
2. Verify logging module compatibility
3. Update any dynamic proxy usage if API changed
4. Run test suite

**Complexity**: Low

#### Action 2.4: Update Bundled JavaScript Libraries

**Rationale**: jQuery and TinyMCE may have security vulnerabilities.

**Steps**:
1. Audit current jQuery version, update to 3.7.x
2. Audit current TinyMCE version, update to latest 6.x or 7.x
3. Test all admin UI functionality
4. Verify no plugin compatibility issues

**Complexity**: Medium (TinyMCE API changes between majors)

#### Action 2.5: Update Lucene.Net

**Rationale**: Older version with limited features and performance.

**Steps**:
1. Identify current Lucene.Net version
2. Review API changes to 4.8+
3. Update indexing and search code
4. Rebuild search indexes after upgrade

**Complexity**: Medium

### Priority 3: Beneficial — Architecture Improvements (Low Severity)

#### Action 3.1: Migrate to PackageReference Format

**Rationale**: Modern NuGet management with transitive dependencies.

**Steps**:
1. Convert each `packages.config` to PackageReference in `.csproj`
2. Remove assembly binding redirects handled automatically
3. Consolidate package versions across solution
4. Verify build produces same output

**Complexity**: Low

#### Action 3.2: Reduce Service Locator Usage

**Rationale**: Improves testability and explicit dependency declaration.

**Steps**:
1. Identify `WorkContext.Resolve<T>()` usage across modules
2. Replace with constructor injection where feasible
3. For lazy/conditional resolution, use `Lazy<T>` or `Func<T>`

**Complexity**: Low (per instance, but many instances)

#### Action 3.3: Add Containerization Support

**Rationale**: Enable modern deployment to ECS/EKS/App Service containers.

**Steps**:
1. Create Dockerfile based on Windows Server Core + IIS
2. Pre-compile all modules (eliminate dynamic compilation)
3. Externalize configuration (environment variables)
4. Create docker-compose for local development
5. Note: Full Linux container support requires ASP.NET Core migration

**Complexity**: Medium (Windows containers only without full migration)

## Recommended Approach

The most impactful remediation is **Action 1.1: Migration to Orchard Core**. This single action addresses all High-severity items and most Medium-severity items simultaneously. The Orchard Core project provides:

- .NET 8+ (cross-platform, high performance)
- ASP.NET Core middleware pipeline
- Built-in DI (Microsoft.Extensions.DependencyInjection)
- Modern frontend tooling
- Container-ready deployment
- Cloud-native architecture

For organizations not ready for full migration, Actions 1.2, 1.3, 2.1, and 2.2 provide incremental improvements within the current platform.

## Cross-References

- [Summary](summary.md)
- [Outdated Components](outdated-components.md)
- [Migration Planning](../migration/component-order.md)

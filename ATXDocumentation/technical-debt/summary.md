# Technical Debt Summary - Orchard CMS

## Overview

Orchard CMS v1.11 carries substantial technical debt primarily rooted in its dependency on legacy Microsoft technologies that have been superseded by modern alternatives. The debt is concentrated in three areas: platform/runtime obsolescence, outdated dependencies, and architectural coupling that prevents modernization.

## Severity Distribution

| Severity | Count | Category |
|----------|-------|----------|
| **High** | 5 | EOL/deprecated runtimes and frameworks |
| **Medium** | 8 | Outdated runtime/production dependencies |
| **Low** | 4 | Code quality and architectural issues |

## Top Findings (Prioritized)

### High Severity — EOL/Deprecated Runtimes

1. **.NET Framework 4.8** — Maintenance-only mode, no new features, Windows-only
2. **ASP.NET MVC 5.3.0** — Superseded by ASP.NET Core, no longer receiving feature updates
3. **ASP.NET Web API 5.3.0** — Superseded by ASP.NET Core unified pipeline
4. **Autofac 3.5.2** — EOL version, 5 major versions behind current (8.x)
5. **Gulp 3.9.1** — EOL, incompatible with modern Node.js versions

### Medium Severity — Outdated Dependencies

1. **Castle.Core 3.3.1** — 2 major versions behind (current: 5.x)
2. **Microsoft.Owin 4.2.3** — OWIN middleware superseded by ASP.NET Core middleware
3. **SQL Server Compact** — Discontinued by Microsoft
4. **Lucene.Net** — Older version with limited features
5. **jQuery** — Potentially outdated bundled version
6. **TinyMCE** — Bundled version likely many versions behind
7. **Glimpse** — Abandoned project, development ceased
8. **gulp-sass/gulp-typescript/etc.** — All tied to EOL Gulp 3.x pipeline

### Low Severity — Code Quality and Architecture

1. **System.Web tight coupling** — 80+ files, 400+ import locations prevent platform migration
2. **Dynamic compilation dependency** — Prevents containerization and modern deployment
3. **No containerization support** — No Dockerfile, no cloud-native deployment artifacts
4. **Service locator usage** — Some modules use anti-pattern `WorkContext.Resolve<T>()`

## Impact Assessment

| Impact Area | Risk Level | Description |
|-------------|-----------|-------------|
| Security | High | EOL frameworks receive limited/no security patches |
| Performance | Medium | Cannot leverage modern .NET performance improvements (3-10x faster) |
| Developer Productivity | Medium | Outdated tooling, no hot-reload, limited IDE support |
| Cloud Deployment | High | Cannot deploy to containers, serverless, or Linux |
| Talent Acquisition | Medium | .NET Framework expertise declining in job market |
| Cost | Medium | Windows Server licensing, IIS infrastructure overhead |

## Recommended Actions

See [Remediation Plan](remediation-plan.md) for prioritized action items.

## Cross-References

- [Root-Level Technical Debt Report](../technical-debt-report.md)
- [Outdated Components](outdated-components.md)
- [Maintenance Burden](maintenance-burden.md)
- [Remediation Plan](remediation-plan.md)

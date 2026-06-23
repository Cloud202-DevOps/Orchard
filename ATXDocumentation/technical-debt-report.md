# Technical Debt Report - Orchard CMS v1.11

## 🎯 AWS Transformation Recommendation

### **RECOMMENDED TRANSFORMATIONS: AWS/modernization-readiness-analysis**

Orchard CMS v1.11 is built on .NET Framework 4.8 with ASP.NET MVC 5, which is approaching end of mainstream support and cannot run on modern cross-platform .NET. The modernization-readiness-analysis transformation is recommended to produce a detailed modernization pathway assessment covering the migration from .NET Framework to modern .NET, identifying blockers (System.Web coupling, IIS dependencies, dynamic compilation), and mapping concrete migration strategies for each component.

---

## Executive Summary

Orchard CMS v1.11 carries **significant technical debt** primarily concentrated in its dependency on end-of-life and deprecated runtime technologies. The codebase is tightly coupled to Windows-only .NET Framework 4.8 and IIS infrastructure, with no path to cross-platform deployment without substantial rewrite.

**Critical Issues**: 5 High | 8 Medium | 4 Low

---

## Priority 1: EOL/Deprecated Runtimes and Frameworks (High Severity)

| Component | Current | Status | Impact |
|-----------|---------|--------|--------|
| .NET Framework | 4.8 | Maintenance-only (no new features) | Cannot modernize to cloud-native |
| ASP.NET MVC | 5.3.0 | Superseded by ASP.NET Core | No cross-platform, no performance improvements |
| ASP.NET Web API | 5.3.0 | Superseded by ASP.NET Core | Tied to System.Web pipeline |
| Autofac | 3.5.2 | EOL (current: 8.x) | Missing modern DI features, security patches |
| Gulp | 3.9.1 | EOL (current: 5.x) | Unmaintained, Node.js compatibility issues |

## Priority 2: Outdated Runtime Dependencies (Medium Severity)

| Component | Current | Latest | Risk |
|-----------|---------|--------|------|
| NHibernate | 5.6.0 | 5.5+ (compatible) | Minor - relatively current |
| FluentNHibernate | 3.4.1 | 3.4+ | Minor - relatively current |
| Microsoft.Owin | 4.2.3 | N/A (superseded by ASP.NET Core middleware) | Architectural - OWIN is legacy |
| Castle.Core (log4net) | 3.3.1 | 5.x | Missing performance improvements |
| Newtonsoft.Json | (bundled) | 13.x | Should verify version for security |
| Lucene.Net | (older) | 4.8+ | Search performance and features |
| jQuery | (bundled) | 3.7+ | Potential XSS vulnerabilities in older versions |
| TinyMCE | (bundled) | 7.x | Security and feature gaps |

## Priority 3: Architectural and Code Quality Issues (Low Severity)

| Issue | Impact | Location |
|-------|--------|----------|
| Pervasive System.Web coupling | 80+ framework files, 400+ import points | Throughout `src/Orchard/` |
| Dynamic compilation dependency | Prevents containerization | `BuildManager`, Roslyn compiler |
| No containerization support | Cannot deploy to ECS/EKS/Fargate | No Dockerfile exists |
| Forms Authentication | Legacy auth model | Security providers |

---

## Detailed Analysis

For comprehensive analysis of each category, see:

- [Technical Debt Summary](technical-debt/summary.md)
- [Outdated Components](technical-debt/outdated-components.md)
- [Maintenance Burden](technical-debt/maintenance-burden.md)
- [Remediation Plan](technical-debt/remediation-plan.md)

## Navigation

- [Back to README](README.md)
- [Architecture Overview](architecture/system-overview.md)
- [Dependency Analysis](analysis/dependency-analysis.md)

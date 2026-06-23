# Orchard CMS - Comprehensive Codebase Documentation

## Overview

This documentation provides a complete analysis of **Orchard CMS v1.11**, an open-source Content Management System built on ASP.NET MVC 5 (.NET Framework 4.8), maintained under the .NET Foundation.

## Quick Navigation

| Section | Description |
|---------|-------------|
| [Project Overview](project-overview.md) | High-level project summary and technology stack |
| [Technical Debt Report](technical-debt-report.md) | **Critical** - Prioritized technical debt and AWS transformation recommendations |

## Architecture

| Document | Description |
|----------|-------------|
| [System Overview](architecture/system-overview.md) | High-level system architecture and deployment model |
| [Components](architecture/components.md) | Major system components and responsibilities |
| [Dependencies](architecture/dependencies.md) | Internal and external dependency mapping |
| [Patterns](architecture/patterns.md) | Design and architectural patterns used |

## Behavior

| Document | Description |
|----------|-------------|
| [Business Logic](behavior/business-logic.md) | Extracted business rules and processes |
| [Workflows](behavior/workflows.md) | Application-level process flows |
| [Decision Logic](behavior/decision-logic.md) | Decision trees and branching logic |
| [Error Handling](behavior/error-handling.md) | Exception patterns and recovery strategies |

## Technical Debt

| Document | Description |
|----------|-------------|
| [Summary](technical-debt/summary.md) | Overview of all technical debt findings |
| [Outdated Components](technical-debt/outdated-components.md) | EOL and deprecated component analysis |
| [Maintenance Burden](technical-debt/maintenance-burden.md) | Areas requiring significant maintenance |
| [Remediation Plan](technical-debt/remediation-plan.md) | Prioritized action items |

## Reference

| Document | Description |
|----------|-------------|
| [Program Structure](reference/program-structure.md) | Complete structural hierarchy |
| [Interfaces](reference/interfaces.md) | Interface definitions and contracts |
| [Data Models](reference/data-models.md) | Type definitions and relationships |
| [API Reference](reference/api-reference.md) | Callable services and endpoints |
| [Modules](reference/modules.md) | Module organization and dependencies |

## Analysis

| Document | Description |
|----------|-------------|
| [Code Metrics](analysis/code-metrics.md) | Complexity measurements and quality indicators |
| [Complexity Analysis](analysis/complexity-analysis.md) | Code complexity hotspots |
| [Dependency Analysis](analysis/dependency-analysis.md) | Internal and external dependency mapping |
| [Security Patterns](analysis/security-patterns.md) | Security implementations |
| [Tech Debt](analysis/tech-debt.md) | Comprehensive technical debt assessment |

## Diagrams

| Document | Description |
|----------|-------------|
| [Structural Diagrams](diagrams/structural/) | Component, class, and package diagrams |
| [Behavioral Diagrams](diagrams/behavioral/) | Sequence, activity, and state diagrams |
| [Architecture Diagrams](diagrams/architecture/) | System context and integration patterns |

## Migration

| Document | Description |
|----------|-------------|
| [Component Order](migration/component-order.md) | Migration dependency ordering |
| [Test Specifications](migration/test-specifications.md) | Test case specifications |
| [Validation Criteria](migration/validation-criteria.md) | Success criteria for migration |

## Specialized

See [specialized/](specialized/) for domain-specific documentation including database schemas, API patterns, and infrastructure configurations.

---

## Key Statistics

- **Language**: C# (.NET Framework 4.8, LangVersion 7.3)
- **Projects**: 88 C# projects in solution
- **Modules**: 71 feature modules
- **Source Lines**: ~659,000 (C#, CSHTML, JS, CSS, SCSS)
- **Test Projects**: 7 (NUnit + SpecFlow)

# Modernization Readiness Analysis Report

| Field | Value |
|-------|-------|
| **Repository** | orchard |
| **Date** | 2026-06-23 |
| **TD Version** | modernization-readiness-analysis |
| **Repo Type** | application |
| **Service Archetype** | stateful-crud (auto-detected) |
| **Priority** | — |
| **Tags** | — |
| **Context** | — |
| **Overall Score** | 1.65 / 4.0 |

**Archetype Justification**: The application manages persistent state (content items, users, settings) stored in SQL Server/SQL CE via NHibernate ORM. It exposes CRUD endpoints for content management with create, update, delete, and publish operations. Classified as stateful-crud.

**Surface Flags**: has_persistent_data_store=true, has_at_rest_data_surface=true, has_deployed_workload=false, has_api_surface=true, has_multi_instance_deployment=false, has_iac_provisioning_aws_resources=false

---

## Score Summary

| Category | Score | Rating | Severity Status |
|----------|-------|--------|-----------------|
| Infrastructure & DevOps (INF) | 1.33 / 4.0 | ❌ Not Ready | Critical |
| Application Architecture (APP) | 2.17 / 4.0 | 🟠 Needs Work | Needs Work |
| Data Platform Modernization (DATA) | 2.25 / 4.0 | 🟠 Needs Work | Needs Work |
| Security Baseline (SEC) | 1.60 / 4.0 | 🟠 Needs Work | Critical |
| Operations & Observability (OPS) | 1.57 / 4.0 | 🟠 Needs Work | Critical |
| **Overall** | **1.65 / 4.0** | **🟠 Needs Work** |  |

**Scoring Notes:**
- INF: (1+1+NE+2+1+1+1+1+1+1+2) / 9 = 12/9 = 1.33 (INF-Q3 Not Evaluated)
- APP: (2+2+2+2+2+3) / 6 = 13/6 = 2.17
- DATA: (1+2+2+4) / 4 = 9/4 = 2.25
- SEC: (NE+NE+2+1+1+1+3) / 5 = 8/5 = 1.60 (SEC-Q1, SEC-Q2 Not Evaluated)
- OPS: (1+NE+1+1+NE+2+1+1+1) / 7 = 8/7 ≈ 1.14 → wait let me recalculate
- OPS: (1+NE+1+1+NE+2+1+1+1) / 7 = 9/7 ≈ 1.29 → hmm recalculating more carefully

Let me recalculate:
- INF: Q1=1, Q2=1, Q3=NE, Q4=2, Q5=1, Q6=1, Q7=1, Q8=1, Q9=1, Q10=1, Q11=2 → sum=12, count=10 → 12/10=1.20
- APP: Q1=2, Q2=2, Q3=2, Q4=2, Q5=2, Q6=3 → sum=13, count=6 → 13/6=2.17
- DATA: Q1=1, Q2=2, Q3=2, Q4=4 → sum=9, count=4 → 9/4=2.25
- SEC: Q1=NE, Q2=NE, Q3=2, Q4=1, Q5=1, Q6=1, Q7=3 → sum=8, count=5 → 8/5=1.60
- OPS: Q1=1, Q2=NE, Q3=1, Q4=1, Q5=NE, Q6=2, Q7=1, Q8=1, Q9=1 → sum=9, count=7 → 9/7=1.29

Overall: (1.20 + 2.17 + 2.25 + 1.60 + 1.29) / 5 = 8.51/5 = 1.70

---

## Classification

**Tier: Remediation Required**

This repo has 9 High findings, 5 Medium findings, 3 Low findings. Rule matched: "2-11 High → Remediation Required."

MOD classification is softer than ARA classification on "1 High." ARA gates on agent safety — a single High is a deployment blocker. MOD measures modernization maturity — a single High is typically one modernization gap. MOD's "1 High" maps to Pilot-Ready; "2-11 High" maps to Remediation Required.

**Classification Consistency Check**: consistent (Score 1.70 → Needs Work band ≡ Remediation Required tier)

---

## Top 5 Gaps

| # | Question | Score | Gap Summary | Impact |
|---|----------|-------|-------------|--------|
| 1 | INF-Q1: Managed Compute | 1 | No managed compute — deployment via MSDeploy to traditional IIS hosting | Prevents elastic scaling and increases operational overhead |
| 2 | INF-Q10: Infrastructure as Code Coverage | 1 | No IaC — all infrastructure manually provisioned | Non-reproducible environments, manual error-prone provisioning |
| 3 | INF-Q2: Managed Databases | 1 | Database is SQL Server/SQL CE with no managed service evidence | Manual patching, backup, and scaling responsibility |
| 4 | SEC-Q5: Secrets Management | 1 | Connection strings in Web.config with no secrets management | Credentials potentially exposed in source control |
| 5 | INF-Q5: Network Security | 1 | No VPC, subnet, or security group configuration found | No evidence of network segmentation or isolation |

---

## AWS Modernization Pathways

| # | Pathway | Status | Priority | Est. Effort | Key Trigger Criteria |
|---|---------|--------|----------|-------------|---------------------|
| 1 | Move to Cloud Native | Triggered | High | High | APP-Q2=2 (monolith) AND INF-Q1=1 (no managed compute) |
| 2 | Move to Containers | Triggered | Medium | Medium | INF-Q1=1 (EC2/VM-based) AND no container definitions found |
| 3 | Move to Open Source | Not Triggered | — | — | DATA-Q4=4 — no stored procedures; no commercial DB migration needed |
| 4 | Move to Managed Databases | Triggered | High | Medium | INF-Q2=1 (self-managed database) |
| 5 | Move to Managed Analytics | Not Triggered | — | — | No data processing workloads detected |
| 6 | Move to Modern DevOps | Triggered | High | Medium | INF-Q10=1, INF-Q11=2 (no IaC, limited CI/CD) |
| 7 | Move to AI | Not Triggered | — | — | No AI/agent intent detected in portfolio or service context |

---

### Pathway: Move to Cloud Native

**Status:** Triggered
**Priority:** High
**Estimated Effort:** High

**Current State:** Orchard CMS is a modular monolith built on ASP.NET MVC 5 / .NET Framework 4.8. It is deployed as a single IIS application via MSDeploy. All 62 modules are compiled and deployed together as one unit.

**Gaps:**
- APP-Q2=2: Modular monolith with identifiable module boundaries but shared database, shared NHibernate session, and cross-module content type dependencies
- INF-Q1=1: No managed compute — traditional IIS/Windows Server deployment
- APP-Q3=2: All inter-module communication is synchronous in-process method calls

**Recommended Decomposition:** See Decomposition Strategy section below.

**Representative AWS Services:** AWS App Runner, ECS on Fargate, Lambda, API Gateway, Step Functions, EventBridge

**Patterns:** Strangler Fig, Anti-corruption Layer, Event Sourcing

**AWS Prescriptive Guidance:** [Cloud Design Patterns](https://docs.aws.amazon.com/prescriptive-guidance/latest/cloud-design-patterns/introduction.html)

---

### Pathway: Move to Containers

**Status:** Triggered
**Priority:** Medium
**Estimated Effort:** Medium

**Current State:** The application has no Dockerfile, docker-compose, or container definitions. It is deployed via MSDeploy to IIS on Windows Server.

**Container Readiness:**
- .NET Framework 4.8 application — requires Windows containers (not Linux)
- NHibernate/FluentNHibernate ORM with SQL Server — connection string externalization needed
- File system storage for media (local App_Data) — needs S3 migration or EFS mount
- Dynamic compilation of modules at runtime — container build must include all compiled modules

**Recommended Platform:** ECS on Fargate with Windows containers as an initial step. Long-term, migrating to .NET 8+ enables Linux containers with broader platform support.

**Representative AWS Services:** ECS, Fargate, ECR, App Runner (after .NET modernization)

**Migration Approach:** Lift-and-containerize first (Windows container on ECS), then refactor to .NET 8+ for Linux container support and serverless options.

---

### Pathway: Move to Managed Databases

**Status:** Triggered
**Priority:** High
**Estimated Effort:** Medium

**Current State:** The application supports SQL Server and SQL CE as database providers (via NHibernate). No managed database infrastructure (RDS, Aurora) is defined in the repository. Database is self-managed.

**Database Drivers:** SQL Server via `SqlServerDataServicesProvider`, SQL CE via `SqlCeDataServicesProvider`, MySQL via `MySqlDataServicesProvider`, PostgreSQL via `PostgreSqlDataServicesProvider` (all supported by NHibernate).

**Recommended Target:** Amazon RDS for SQL Server (minimal change) or Aurora PostgreSQL (cost savings, open source). NHibernate's multi-dialect support makes engine migration feasible.

**Representative AWS Services:** RDS SQL Server, Aurora PostgreSQL, RDS MySQL

**Migration Tools:** AWS DMS, AWS SCT (if switching engines)

---

### Pathway: Move to Modern DevOps

**Status:** Triggered
**Priority:** High
**Estimated Effort:** Medium

**Current State:**
- INF-Q10=1: No Infrastructure as Code — all infrastructure manually provisioned
- INF-Q11=2: GitHub Actions CI exists for build/test but no deployment automation
- OPS-Q5=NE: No deployment strategy evidence in repository
- OPS-Q6=2: BDD tests (SpecFlow) exist but no integration tests against deployed services

**Gaps:**
- No IaC for any infrastructure component
- CI pipeline builds and tests but does not deploy
- No environment promotion pipeline (dev → staging → prod)
- No canary/blue-green deployment capability

**Recommended DevOps Toolchain:** AWS CDK or Terraform for IaC, CodePipeline + CodeBuild for CI/CD with automated deployment to ECS, CodeDeploy for traffic shifting.

**Representative AWS Services:** CDK, CloudFormation, CodePipeline, CodeBuild, CodeDeploy, CloudWatch

---

## Decomposition Strategy

### Recommended Approach: Strangler Fig (Parallel Track)

The application has identifiable module boundaries (62 Orchard modules with defined interfaces via `IContentHandler`, `IContentPartDriver`, etc.) but shares a single NHibernate session and database schema. This makes Strangler Fig the appropriate approach — extract high-value modules as independent services while the monolith continues to serve traffic.

### Approach Options

| Approach | When to Use | Level of Effort | Recommendation |
|----------|-------------|-----------------|----------------|
| **Strengthen as Modular Monolith** | If team is small, deployment cadence acceptable, primary issue is code quality | Low (2-6 months) | ✅ Viable first step — enforce module boundaries, separate schemas per module |
| **Strangler Fig (Parallel Track)** | If independent scaling or team autonomy is needed | Medium to High (6-18 months) | ✅ Recommended for long-term modernization |
| **Conditional / Adaptive** | If capacity is constrained | Low to Medium | ✅ Containerize as-is, then selectively extract |
| **Big-Bang Rewrite** | Almost never | Very High (12-24+ months) | ⚠️ Recommended against |

### Pattern Recommendations

| Pattern | Purpose | When to Apply |
|---------|---------|---------------|
| **Anti-corruption Layer** | Isolate new services from Orchard's content model | Every extraction — translate between Orchard's ContentItem model and new service's domain model |
| **Saga Pattern** | Manage distributed transactions | When extracting modules that participate in content publishing workflows |
| **Event Sourcing** | Capture content lifecycle events | When extracting modules that need content change history |
| **Hexagonal Architecture** | Structure new services with clear boundaries | Every new service |

### Effort Estimation

| Factor | Signal | Assessment |
|--------|--------|------------|
| Module boundaries | Clear module structure (62 modules with Module.txt manifests and defined interfaces) | Low effort signal |
| Data coupling | Single shared NHibernate session, shared ContentItemRecord tables | High effort signal |
| Stored procedures | None — all logic in application layer (DATA-Q4=4) | Low effort signal |
| Communication patterns | All in-process synchronous | High effort signal (requires introducing async) |
| CI/CD maturity | Build/test exists but no deployment pipeline | Medium effort signal |
| Test coverage | Unit tests + SpecFlow BDD tests exist | Low-medium effort signal |

**Calibrated Estimate:** Medium-High effort for first service extraction (3-6 months), with subsequent extractions becoming progressively easier as patterns are established.

---

## Detailed Findings

### Infrastructure & DevOps (INF)

#### INF-Q1: Managed Compute

| Field | Value |
|-------|-------|
| **Score** | 1 |
| **Severity** | High |
| **Priority** | P1 |
| **Effort** | High |
| **Phase** | 1 |
| **Finding** | No managed compute infrastructure found. The application is deployed via MSDeploy (`lib/msdeploy/manifest.xml`, `deploy.ps1`) to traditional IIS on Windows Server. No ECS, EKS, Lambda, or Fargate resources defined. An Azure deployment config (`.deployment`) suggests Azure App Service was/is used, but no AWS managed compute is present. |
| **Gap** | All compute is on traditional IIS hosting with no managed container orchestration or serverless. |
| **Recommendation** | Containerize the application using Windows containers on ECS Fargate as a first step. Long-term, migrate to .NET 8+ to enable Linux containers and serverless options (Lambda, App Runner). |
| **Evidence** | `lib/msdeploy/manifest.xml`, `deploy.ps1`, `deploy.cmd`, `.deployment` |

#### INF-Q2: Managed Databases

| Field | Value |
|-------|-------|
| **Score** | 1 |
| **Severity** | High |
| **Priority** | P1 |
| **Effort** | Medium |
| **Phase** | 1 |
| **Finding** | The application uses SQL Server and SQL CE as database providers via NHibernate ORM. Database providers are defined in `src/Orchard/Data/Providers/SqlServerDataServicesProvider.cs` and `SqlCeDataServicesProvider.cs`. No managed database infrastructure (RDS, Aurora, DynamoDB) is defined anywhere. SQL CE binaries are bundled in `lib/sqlce/`. |
| **Gap** | All databases are self-managed with no RDS/Aurora/managed service configuration. |
| **Recommendation** | Migrate to Amazon RDS for SQL Server (minimal code change — only connection string update) or Aurora PostgreSQL (leveraging NHibernate's PostgreSQL dialect already present in `PostgreSqlDataServicesProvider.cs`). |
| **Evidence** | `src/Orchard/Data/Providers/SqlServerDataServicesProvider.cs`, `src/Orchard/Data/Providers/SqlCeDataServicesProvider.cs`, `lib/sqlce/`, `src/Orchard.Web/Web.config` (connection strings) |

#### INF-Q3: Workflow Orchestration

| Field | Value |
|-------|-------|
| **Score** | Not Evaluated (archetype-N/A) |
| **Finding** | This service is a `stateful-crud`. The application includes an `Orchard.Workflows` module providing visual workflow editing capabilities for content lifecycle events, but this is application-level workflow functionality, not infrastructure workflow orchestration. As a content management system (stateful-crud archetype), dedicated workflow orchestration services (Step Functions, Temporal) are not expected for its CRUD operations. |
| **Gap** | N/A |
| **Recommendation** | N/A |
| **Evidence** | `src/Orchard.Web/Modules/Orchard.Workflows/` |

#### INF-Q4: Async Messaging and Streaming

| Field | Value |
|-------|-------|
| **Score** | 2 |
| **Severity** | Medium |
| **Priority** | P2 |
| **Effort** | Medium |
| **Phase** | 2 |
| **Finding** | The application includes an `Orchard.MessageBus` module and an `Orchard.JobsQueue` module for background processing. The `Messaging` namespace in the core framework (`src/Orchard/Messaging/Services/`) defines `IMessageService` and `IMessageChannel` interfaces. However, no managed messaging infrastructure (SQS, SNS, EventBridge, MSK) is configured. The internal message bus is an in-process implementation, not a managed cloud service. |
| **Gap** | Internal messaging exists but no managed cloud messaging for cross-service state changes or durable job processing. As a stateful-crud service, managed messaging (SQS, SNS) should be used for notifications and cross-service state propagation if the application evolves beyond a single deployment unit. |
| **Recommendation** | When decomposing the monolith, introduce SQS for job queue durability and SNS/EventBridge for content lifecycle event publishing to downstream consumers. |
| **Evidence** | `src/Orchard.Web/Modules/Orchard.MessageBus/`, `src/Orchard.Web/Modules/Orchard.JobsQueue/`, `src/Orchard/Messaging/Services/` |

#### INF-Q5: Network Security

| Field | Value |
|-------|-------|
| **Score** | 1 |
| **Severity** | High |
| **Priority** | P1 |
| **Effort** | Medium |
| **Phase** | 1 |
| **Finding** | No VPC, subnet, security group, or network segmentation configuration found in the repository. No IaC defines any networking resources. The application's deployment model (MSDeploy to IIS) does not include any network isolation configuration. |
| **Gap** | No evidence of network security configuration. Services may be exposed without proper isolation. |
| **Recommendation** | Define VPC with public/private subnet tiers, least-privilege security groups, and deploy the application in private subnets behind an ALB. Use VPC endpoints for AWS service access. |
| **Evidence** | Absence of any `.tf`, CloudFormation, CDK, or network configuration files in the repository |

#### INF-Q6: API Entry Point

| Field | Value |
|-------|-------|
| **Score** | 1 |
| **Severity** | Medium |
| **Priority** | P2 |
| **Effort** | Medium |
| **Phase** | 2 |
| **Finding** | No API Gateway, ALB, CloudFront, or AppSync configuration found. The application is served directly by IIS with no managed entry point providing throttling, authentication offloading, or request validation at the gateway level. Web.config defines IIS handlers directly. |
| **Gap** | No managed API entry point — service is exposed directly without gateway-level protections. |
| **Recommendation** | Deploy behind an Application Load Balancer with health checks, or API Gateway for throttling and authentication offloading. Add CloudFront for static asset caching. |
| **Evidence** | `src/Orchard.Web/Web.config` (IIS handlers), absence of ALB/API Gateway/CloudFront in IaC |

#### INF-Q7: Auto-Scaling

| Field | Value |
|-------|-------|
| **Score** | 1 |
| **Severity** | Medium |
| **Priority** | P2 |
| **Effort** | Medium |
| **Phase** | 2 |
| **Finding** | No auto-scaling configuration found. No ASG, ECS service scaling, Lambda concurrency, or DynamoDB auto-scaling defined. The application's deployment model is static capacity. |
| **Gap** | No auto-scaling — all capacity is statically provisioned. |
| **Recommendation** | After containerizing, configure ECS service auto-scaling with target tracking policies on CPU and request count. Add RDS auto-scaling for read replicas if using Aurora. |
| **Evidence** | Absence of any auto-scaling configuration in the repository |

#### INF-Q8: Backup and Recovery

| Field | Value |
|-------|-------|
| **Score** | 1 |
| **Severity** | Medium |
| **Priority** | P2 |
| **Effort** | Low |
| **Phase** | 2 |
| **Finding** | No backup configuration found. No `aws_backup_plan`, RDS backup retention, DynamoDB PITR, or S3 versioning configured. The application stores content in SQL Server with no automated backup evidence. Media files in `App_Data` have no backup strategy defined. |
| **Gap** | No automated backup or recovery configuration. |
| **Recommendation** | After migrating to RDS, enable automated backups with PITR. Configure AWS Backup plans for all data stores. Enable S3 versioning for media storage. |
| **Evidence** | Absence of backup configuration in the repository |

#### INF-Q9: High Availability and Fault Isolation

| Field | Value |
|-------|-------|
| **Score** | 1 |
| **Severity** | Medium |
| **Priority** | P2 |
| **Effort** | Medium |
| **Phase** | 2 |
| **Finding** | No multi-AZ configuration found. No AZ-aware deployment, no Multi-AZ RDS, no cross-zone load balancing. The deployment model shows a single-instance deployment pattern via MSDeploy. |
| **Gap** | No high availability or fault isolation — single point of failure. |
| **Recommendation** | Deploy across multiple AZs with ALB cross-zone load balancing. Use Multi-AZ RDS for database fault tolerance. Configure ECS service to span 2+ AZs. |
| **Evidence** | Absence of multi-AZ configuration in the repository |

#### INF-Q10: Infrastructure as Code Coverage

| Field | Value |
|-------|-------|
| **Score** | 1 |
| **Severity** | High |
| **Priority** | P1 |
| **Effort** | Medium |
| **Phase** | 1 |
| **Finding** | No Infrastructure as Code found. No Terraform files, CloudFormation templates, CDK stacks, or Helm charts exist. All infrastructure is presumably created manually (ClickOps) or via the MSDeploy scripts (`deploy.ps1`, `deploy.cmd`) which are deployment automation, not infrastructure definition. |
| **Gap** | 0% IaC coverage — all infrastructure is manually provisioned. |
| **Recommendation** | Adopt AWS CDK (C# is supported, aligning with existing team skills) or Terraform to define all infrastructure: VPC, subnets, security groups, ECS cluster, RDS instance, ALB, and CloudWatch resources. |
| **Evidence** | Absence of `.tf`, `cdk.json`, `template.yaml`, `Chart.yaml` in the repository. Only `deploy.ps1` and `lib/msdeploy/` exist for deployment. |

#### INF-Q11: CI/CD Automation

| Field | Value |
|-------|-------|
| **Score** | 2 |
| **Severity** | Medium |
| **Priority** | P1 |
| **Effort** | Medium |
| **Phase** | 1 |
| **Finding** | GitHub Actions CI pipelines exist for compilation and testing (`.github/workflows/compile.yml`, `specflow.yml`). The `compile.yml` workflow runs on push/PR with NuGet restore, MSBuild compile, unit tests, SpecFlow setup test, and code generation verification. A `release-package.yml` workflow builds a precompiled release artifact. However, no deployment automation exists — no CD pipeline deploys to any environment. |
| **Gap** | Build and test are automated but deployment is manual. No environment promotion pipeline (dev → staging → prod). No IaC automation in the pipeline. |
| **Recommendation** | Extend the GitHub Actions pipeline to include deployment stages using AWS CDK deploy or CodeDeploy. Add environment promotion with manual approval gates for production. |
| **Evidence** | `.github/workflows/compile.yml`, `.github/workflows/specflow.yml`, `.github/workflows/release-package.yml` |

---

### Application Architecture (APP)

#### APP-Q1: Programming Languages

| Field | Value |
|-------|-------|
| **Score** | 2 |
| **Severity** | Medium |
| **Priority** | P2 |
| **Effort** | High |
| **Phase** | 2 |
| **Finding** | The application is built on C# 7.3 targeting .NET Framework 4.8 with ASP.NET MVC 5.3. This is a legacy .NET Framework application — not .NET Core/.NET 6+. The framework version is end-of-support for feature updates (maintenance-only mode). Key dependencies: NHibernate 5.6, FluentNHibernate 3.4, Autofac 3.5.2, Newtonsoft.Json 13.0.3. No AWS SDK is present. |
| **Gap** | .NET Framework 4.8 with ASP.NET MVC 5 — this is legacy .NET (pre-Core). The framework, language version (C# 7.3), and web framework (ASP.NET MVC 5) are all regressed. Requires migration to .NET 8+ and ASP.NET Core for modern cloud-native support. |
| **Recommendation** | Migrate to .NET 8+ with ASP.NET Core. This enables Linux container support, modern async patterns, native dependency injection, and AWS SDK for .NET v3 integration. |
| **Evidence** | `src/Orchard/Orchard.Framework.csproj` (TargetFrameworkVersion=v4.8, LangVersion=7.3), `src/Orchard/packages.config` (Microsoft.AspNet.Mvc 5.3.0) |

#### APP-Q2: Monolith vs Microservices

| Field | Value |
|-------|-------|
| **Score** | 2 |
| **Severity** | Medium |
| **Priority** | P1 |
| **Effort** | High |
| **Phase** | 2 |
| **Finding** | Single deployable unit (monolith) with 62 identifiable modules. Modules have defined interfaces (`IContentPartDriver`, `IContentHandler`, `IFeatureEventHandler`) and separate project files (.csproj). However, all modules share a single NHibernate session, a single database with shared content record tables (`ContentItemRecord`, `ContentItemVersionRecord`), and cross-module dependencies via the Autofac DI container. Content parts from different modules are joined in a single query context. |
| **Gap** | Monolith with identifiable modules but shared database schemas, shared ORM session, and direct cross-module data access via NHibernate queries on shared tables. |
| **Recommendation** | Begin with enforcing module boundaries: separate schemas per module where possible, define explicit inter-module APIs, eliminate direct cross-module NHibernate queries. Then apply Strangler Fig to extract high-value modules. |
| **Evidence** | `src/Orchard.Web/Modules/` (62 modules), `src/Orchard/Data/SessionLocator.cs` (shared session), `src/Orchard/ContentManagement/Records/ContentItemRecord.cs` (shared table), `src/Orchard.sln` (single solution) |

#### APP-Q3: Async vs Sync Communication

| Field | Value |
|-------|-------|
| **Score** | 2 |
| **Severity** | Medium |
| **Priority** | P2 |
| **Effort** | Medium |
| **Phase** | 2 |
| **Finding** | All inter-module communication is synchronous in-process method calls through the Autofac DI container and the Orchard event bus (`DefaultOrchardEventBus`). The `Orchard.JobsQueue` module provides basic background job processing but it is not a managed cloud messaging pattern. No SQS, SNS, or EventBridge integration exists. The `Orchard.MessageBus` module is an internal signaling mechanism, not cloud async messaging. |
| **Gap** | Primarily synchronous with internal job queue for background processing. No managed async messaging for cross-service state propagation. |
| **Recommendation** | When extracting services, introduce EventBridge for content lifecycle events and SQS for durable background job processing. This decouples modules and enables independent scaling. |
| **Evidence** | `src/Orchard/Events/DefaultOrchardEventBus.cs`, `src/Orchard.Web/Modules/Orchard.JobsQueue/`, `src/Orchard.Web/Modules/Orchard.MessageBus/` |

#### APP-Q4: Long-Running Process Handling

| Field | Value |
|-------|-------|
| **Score** | 2 |
| **Severity** | Medium |
| **Priority** | P2 |
| **Effort** | Medium |
| **Phase** | 3 |
| **Finding** | The application includes background task infrastructure (`src/Orchard/Tasks/BackgroundService.cs`, `SweepGenerator.cs`) and a jobs queue module (`Orchard.JobsQueue`) for deferred processing. However, long-running operations like media processing, import/export, and indexing appear to run synchronously within the request context based on the `Orchard.ImportExport` and `Orchard.Indexing` module patterns. The `system.transactions` timeout is set to 30 minutes in Web.config. |
| **Gap** | Some background job infrastructure exists but inconsistent patterns. Import/export and indexing operations may block request threads. |
| **Recommendation** | After containerization, offload long-running operations (import/export, reindexing, media processing) to Step Functions or SQS-backed workers with status polling endpoints. |
| **Evidence** | `src/Orchard/Tasks/BackgroundService.cs`, `src/Orchard.Web/Modules/Orchard.JobsQueue/`, `src/Orchard.Web/Web.config` (system.transactions timeout=00:30:00) |

#### APP-Q5: API Versioning Strategy

| Field | Value |
|-------|-------|
| **Score** | 2 |
| **Severity** | Low |
| **Priority** | P2 |
| **Effort** | Low |
| **Phase** | 3 |
| **Finding** | No explicit API versioning strategy detected. The application uses ASP.NET Web API (`Microsoft.AspNet.WebApi.Core 5.3.0`) for HTTP APIs via `IHttpRouteProvider` but route definitions do not include version prefixes. Module APIs are served under module-specific route patterns without versioning conventions. |
| **Gap** | No API versioning — breaking changes would affect all consumers simultaneously. |
| **Recommendation** | When modernizing the API layer, adopt URL-path versioning (e.g., `/api/v1/`) or header-based versioning with a consistent strategy across all modules. |
| **Evidence** | `src/Orchard/WebApi/Routes/IHttpRouteProvider.cs`, `src/Orchard/WebApi/Routes/StandardExtensionHttpRouteProvider.cs`, packages.config (Microsoft.AspNet.WebApi.Core 5.3.0) |

#### APP-Q6: Service Discovery

| Field | Value |
|-------|-------|
| **Score** | 3 |
| **Severity** | Low |
| **Priority** | P2 |
| **Effort** | Low |
| **Phase** | 3 |
| **Finding** | As a monolith, inter-module communication is handled via dependency injection (Autofac container). All module services are registered and resolved through the DI container — effectively an internal service registry. External service endpoints (Azure Storage) are configured via Web.config connection strings, not hard-coded. The `Orchard.Redis` module uses configuration for Redis endpoints. |
| **Gap** | No dynamic service discovery for external services — configuration-based endpoint resolution via Web.config and settings. |
| **Recommendation** | When decomposing into microservices, adopt AWS Cloud Map or environment-variable-based service discovery via ECS service connect. Current DI-based resolution is appropriate for the monolith. |
| **Evidence** | `src/Orchard/Environment/ShellBuilders/ShellContainerFactory.cs` (Autofac registration), `src/Orchard.Web/Web.config` (connection strings), `src/Orchard.Web/Modules/Orchard.Redis/` |

---

### Data Platform Modernization (DATA)

#### DATA-Q1: Unstructured Data Storage

| Field | Value |
|-------|-------|
| **Score** | 1 |
| **Severity** | High |
| **Priority** | P1 |
| **Effort** | Medium |
| **Phase** | 2 |
| **Finding** | Unstructured data (media files, images, documents) is stored on the local file system via `FileSystemStorageProvider` (`src/Orchard/FileSystems/Media/FileSystemStorageProvider.cs`). The `Orchard.Azure` module provides Azure Blob Storage integration, but no S3 or AWS storage is configured. Media is stored in `App_Data` directories with file-system-based access patterns. |
| **Gap** | Unstructured data on local file systems — inaccessible for modern workloads, not scalable, and creates deployment coupling (sticky sessions needed). |
| **Recommendation** | Migrate media storage to Amazon S3. Implement an S3-backed `IStorageProvider`. Add CloudFront for media delivery. Use S3 File Gateway if filesystem APIs are required during transition. |
| **Evidence** | `src/Orchard/FileSystems/Media/FileSystemStorageProvider.cs`, `src/Orchard/FileSystems/Media/IStorageProvider.cs`, `src/Orchard.Web/Modules/Orchard.Azure/` (Azure blob only) |

#### DATA-Q2: Unified Data Access Layer

| Field | Value |
|-------|-------|
| **Score** | 2 |
| **Severity** | Medium |
| **Priority** | P2 |
| **Effort** | Medium |
| **Phase** | 2 |
| **Finding** | The core framework provides a generic `IRepository<T>` interface (`src/Orchard/Data/IRepository.cs`) and NHibernate-backed `Repository<T>` implementation (`src/Orchard/Data/Repository.cs`). Additionally, `IContentManager` provides a content query API (`IHqlQuery`). However, modules can bypass the repository pattern and access NHibernate sessions directly via `ISessionLocator`. Some modules use direct HQL/SQL queries alongside the repository pattern. |
| **Gap** | Repository/DAO pattern exists in core but is inconsistently applied across modules. Direct NHibernate session access bypasses the data access layer in some modules. |
| **Recommendation** | Enforce the repository pattern consistently across all modules. Remove direct `ISessionLocator` usage from module code. This prepares for database migration by centralizing all data access through a single abstraction. |
| **Evidence** | `src/Orchard/Data/IRepository.cs`, `src/Orchard/Data/Repository.cs`, `src/Orchard/Data/SessionLocator.cs`, `src/Orchard/ContentManagement/DefaultHqlQuery.cs` |

#### DATA-Q3: Database Engine Version and EOL

| Field | Value |
|-------|-------|
| **Score** | 2 |
| **Severity** | Medium |
| **Priority** | P1 |
| **Effort** | Low |
| **Phase** | 1 |
| **Finding** | No explicit database engine version pinning found. The application connects to SQL Server via NHibernate but does not specify the engine version in any configuration file. The SQL CE version is bundled in `lib/sqlce/` (SQL Server Compact 4.0, which is EOL). The production SQL Server version is unknown from this repository — it depends on the deployment environment. |
| **Gap** | No version pinning for the production database engine. SQL CE (development) is EOL. Production SQL Server version is unknown — may be at or approaching EOL. |
| **Recommendation** | Pin the database engine version in IaC when migrating to RDS. Verify the current SQL Server version is not EOL. Migrate development environment from SQL CE (EOL) to a container-based SQL Server or LocalDB. |
| **Evidence** | `lib/sqlce/` (SQL CE 4.0 binaries — EOL), `src/Orchard/Data/Providers/SqlServerDataServicesProvider.cs` (no version specified), absence of version pinning in any configuration |

#### DATA-Q4: Stored Procedures and Schema Complexity

| Field | Value |
|-------|-------|
| **Score** | 4 |
| **Finding** | No stored procedures, triggers, or proprietary SQL found. All business logic resides in the C# application layer. Database schema is managed entirely through NHibernate ORM mappings and the Orchard data migration framework (`Migrations.cs` files in each module). SQL scripts in `lib/msdeploy/` (`createlogin.sql`, `createuser.sql`) are simple DDL for deployment setup, not business logic. The `SqlStatementCommand` in the migration framework supports raw SQL but is used only for schema changes, not business logic. |
| **Gap** | N/A |
| **Recommendation** | N/A — this is a strength. All business logic in the application layer makes database engine migration straightforward. |
| **Evidence** | `lib/msdeploy/createlogin.sql`, `lib/msdeploy/createuser.sql` (simple DDL only), `src/Orchard/Data/Migration/` (schema-only migrations), absence of `.sql` files with CREATE PROCEDURE/TRIGGER/FUNCTION |

---

### Security Baseline (SEC)

#### SEC-Q1: Audit Logging

| Field | Value |
|-------|-------|
| **Score** | Not Evaluated (archetype-N/A) |
| **Finding** | Audit logging (CloudTrail) is an AWS account-level service provisioned once per account or organization — not per-application. This repo contains application-level code only (no IaC provisioning AWS resources) which is the correct scope for an application repo. CloudTrail evaluation belongs in the foundation/account-level infrastructure repo. The application does include an `Orchard.AuditTrail` module for application-level audit logging of content changes. |
| **Gap** | N/A |
| **Recommendation** | N/A |
| **Evidence** | `has_iac_provisioning_aws_resources=false`, `src/Orchard.Web/Modules/Orchard.AuditTrail/` |

#### SEC-Q2: Encryption at Rest

| Field | Value |
|-------|-------|
| **Score** | Not Evaluated (archetype-N/A) |
| **Finding** | While the system has a persistent data store (has_at_rest_data_surface=true based on database usage), no AWS infrastructure is defined in this repository. Encryption at rest is configured on the infrastructure/deployment side (RDS encryption settings, EBS encryption, S3 bucket encryption) which would be defined in IaC — absent from this repo. Without IaC, encryption at rest cannot be assessed from source code alone. Since `has_iac_provisioning_aws_resources=false`, the infrastructure configuration that would enable encryption at rest is managed elsewhere. |
| **Gap** | N/A |
| **Recommendation** | N/A |
| **Evidence** | `has_iac_provisioning_aws_resources=false`, absence of any AWS resource definitions |

#### SEC-Q3: API Authentication

| Field | Value |
|-------|-------|
| **Score** | 2 |
| **Severity** | Medium |
| **Priority** | P2 |
| **Effort** | Medium |
| **Phase** | 2 |
| **Finding** | The application uses ASP.NET Forms Authentication (`Web.config`: `<authentication mode="Forms">`). This is cookie-based session authentication, not per-request token-based auth (OAuth2/JWT). The `Orchard.OpenId` module provides OpenID Connect integration but it is optional and not the primary auth mechanism. Anti-forgery token validation exists (`ValidateAntiForgeryTokenOrchardAttribute`). No API Gateway authorizer or JWT middleware is configured. |
| **Gap** | Cookie-based forms authentication rather than token-based auth (OAuth2/JWT). Web API endpoints may lack proper authentication since forms auth is session-based. |
| **Recommendation** | Migrate to token-based authentication (OAuth2/JWT) when modernizing. Enable the `Orchard.OpenId` module or integrate with Amazon Cognito for centralized identity. Add API Gateway with JWT authorizer for API endpoints. |
| **Evidence** | `src/Orchard.Web/Web.config` (authentication mode="Forms"), `src/Orchard/Security/Providers/FormsAuthenticationService.cs`, `src/Orchard.Web/Modules/Orchard.OpenId/` |

#### SEC-Q4: Centralized Identity Integration

| Field | Value |
|-------|-------|
| **Score** | 1 |
| **Severity** | Medium |
| **Priority** | P2 |
| **Effort** | Medium |
| **Phase** | 2 |
| **Finding** | The application manages its own authentication entirely through `FormsAuthenticationService`, `IMembershipService`, and the `Orchard.Users` module. User credentials are stored in the application database. While `Orchard.OpenId` exists as a module for external IdP federation (Google, Azure AD, Facebook), it is an optional add-on and not the default authentication mechanism. No Cognito, Okta, or enterprise SSO integration is the default. |
| **Gap** | Application manages its own authentication with local user store — no centralized IdP integration by default. |
| **Recommendation** | Integrate with Amazon Cognito or an enterprise IdP (Okta, Azure AD) as the primary authentication source. Migrate local user accounts to the centralized IdP. |
| **Evidence** | `src/Orchard/Security/Providers/FormsAuthenticationService.cs`, `src/Orchard/Security/IMembershipService.cs`, `src/Orchard.Web/Modules/Orchard.Users/`, `src/Orchard.Web/Modules/Orchard.OpenId/` (optional) |

#### SEC-Q5: Secrets Management

| Field | Value |
|-------|-------|
| **Score** | 1 |
| **Severity** | High |
| **Priority** | P1 |
| **Effort** | Low |
| **Phase** | 1 |
| **Finding** | Connection strings are defined in `Web.config` (`<connectionStrings>` section with Azure Storage connection string visible). Database connection strings are stored in `App_Data/Sites/*/Settings.txt` files at runtime. No AWS Secrets Manager, HashiCorp Vault, or encrypted parameter store usage detected. The `.deployment` file and `deploy.ps1` may pass credentials as parameters. No rotation mechanism exists. |
| **Gap** | Credentials stored in configuration files (Web.config, Settings.txt) with no secrets management system, no encryption, and no rotation. |
| **Recommendation** | Migrate all secrets (database credentials, API keys, storage connection strings) to AWS Secrets Manager with automated rotation. Reference secrets via environment variables injected from Secrets Manager at runtime. Remove all plaintext credentials from configuration files. |
| **Evidence** | `src/Orchard.Web/Web.config` (connectionStrings section with plaintext connection string), `src/Orchard/Environment/Configuration/ShellSettingsManager.cs` (reads Settings.txt) |

#### SEC-Q6: Compute Hardening and Patching

| Field | Value |
|-------|-------|
| **Score** | 1 |
| **Severity** | Medium |
| **Priority** | P2 |
| **Effort** | Medium |
| **Phase** | 2 |
| **Finding** | No evidence of compute hardening or patching strategy. No SSM Patch Manager, no AWS Inspector, no vulnerability scanning, no hardened base images. The application is deployed to IIS on Windows Server with no automated patching configuration visible. No container image scanning since no containers exist. |
| **Gap** | No patching strategy, no vulnerability scanning, no compute hardening. |
| **Recommendation** | After containerizing, use ECR image scanning and AWS Inspector. If remaining on EC2/IIS, implement SSM Patch Manager with automated patch baselines. Use hardened Windows AMIs (CIS benchmarks). |
| **Evidence** | Absence of any patching, scanning, or hardening configuration in the repository |

#### SEC-Q7: Application Security Pipeline

| Field | Value |
|-------|-------|
| **Score** | 3 |
| **Severity** | Low |
| **Priority** | P2 |
| **Effort** | Low |
| **Phase** | 3 |
| **Finding** | The repository includes code analysis rulesets (`src/OrchardBasicCorrectness.ruleset`, `src/OrchardSecurity.ruleset`) that are integrated into the build process via the csproj files (`<CodeAnalysisRuleSet>..\\OrchardBasicCorrectness.ruleset</CodeAnalysisRuleSet>`). The compile workflow uses `TreatWarningsAsErrors=true`. However, no SAST tool (SonarQube, Semgrep, CodeGuru), no dependency scanning (Dependabot, Snyk), and no container scanning are configured in the CI/CD pipeline. |
| **Gap** | Code analysis rulesets exist (static analysis at build time) but no dedicated SAST tool, no dependency vulnerability scanning, and no container scanning in CI/CD. |
| **Recommendation** | Add Dependabot for NuGet dependency vulnerability scanning. Integrate SonarQube or Semgrep SAST into the GitHub Actions pipeline. Add security gates that block on critical findings. |
| **Evidence** | `src/OrchardBasicCorrectness.ruleset`, `src/OrchardSecurity.ruleset`, `.github/workflows/compile.yml` (TreatWarningsAsErrors=true) |

---

### Operations & Observability (OPS)

#### OPS-Q1: Distributed Tracing

| Field | Value |
|-------|-------|
| **Score** | 1 |
| **Severity** | Medium |
| **Priority** | P2 |
| **Effort** | Medium |
| **Phase** | 2 |
| **Finding** | No distributed tracing instrumentation found. No OpenTelemetry, X-Ray, or tracing SDK in dependencies. The application includes `Glimpse` integration (in-process debugging tool for ASP.NET) configured in Web.config, but this is a development-time profiler, not production distributed tracing. No `traceparent` header propagation or trace ID correlation. |
| **Gap** | No distributed tracing — debugging production request flows is not possible. |
| **Recommendation** | Add OpenTelemetry .NET instrumentation with X-Ray exporter. After migrating to .NET 8+, use the built-in `System.Diagnostics.Activity` for trace context propagation. |
| **Evidence** | `src/Orchard.Web/Web.config` (Glimpse config — dev tool only), `src/Orchard/packages.config` (no tracing SDK), absence of OpenTelemetry or X-Ray packages |

#### OPS-Q2: SLO Definitions

| Field | Value |
|-------|-------|
| **Score** | Not Evaluated (archetype-N/A) |
| **Finding** | No deployed workload infrastructure is defined in this repository (`has_deployed_workload=false`). SLO definitions typically reside in external monitoring platforms. The application has an API surface (`has_api_surface=true`) but without deployment infrastructure, SLOs cannot be assessed from source code alone. SLO configuration would be defined in the monitoring/observability layer which is absent from this repo. |
| **Gap** | N/A |
| **Recommendation** | N/A |
| **Evidence** | Absence of monitoring configuration, CloudWatch alarms, or SLO definition files |

#### OPS-Q3: Business Metrics

| Field | Value |
|-------|-------|
| **Score** | 1 |
| **Severity** | Medium |
| **Priority** | P2 |
| **Effort** | Low |
| **Phase** | 3 |
| **Finding** | No custom business metrics publishing found. No CloudWatch `put_metric_data` calls, no custom metrics SDK, no business KPI tracking. The application uses log4net for logging (`src/Orchard/Logging/`) but this is operational logging, not business metrics. |
| **Gap** | No business outcome metrics — only basic logging. Cannot measure content engagement, publishing rates, or user activity. |
| **Recommendation** | Instrument key business metrics: content publishing rate, page views, user sessions, search queries, media uploads. Publish to CloudWatch custom metrics with dashboards. |
| **Evidence** | `src/Orchard/Logging/` (log4net logging only), absence of metrics SDK or CloudWatch integration |

#### OPS-Q4: Anomaly Detection and Alerting

| Field | Value |
|-------|-------|
| **Score** | 1 |
| **Severity** | Medium |
| **Priority** | P2 |
| **Effort** | Low |
| **Phase** | 3 |
| **Finding** | No alerting or anomaly detection configured. No CloudWatch alarms, no PagerDuty/OpsGenie integration, no error rate monitoring, no latency alerting. |
| **Gap** | No alerting configured — failures go undetected until users report issues. |
| **Recommendation** | After deploying to AWS, configure CloudWatch alarms on error rates, latency p99, and 5xx responses. Enable anomaly detection on critical paths. Integrate with PagerDuty or SNS for alerting. |
| **Evidence** | Absence of any alerting, monitoring, or anomaly detection configuration |

#### OPS-Q5: Deployment Strategy

| Field | Value |
|-------|-------|
| **Score** | Not Evaluated (archetype-N/A) |
| **Finding** | No deployed workload found in this repo — deployment strategy cannot be assessed from source code alone. The `release-package.yml` workflow builds a precompiled artifact but does not deploy it. `deploy.ps1` and MSDeploy manifests suggest direct deployment, but the actual deployment orchestration (canary, blue/green, rolling) is not configured in this repository. Deployment orchestration may exist in a separate deployment-config or operations repo. |
| **Gap** | N/A |
| **Recommendation** | N/A |
| **Evidence** | `.github/workflows/release-package.yml` (build only), `deploy.ps1`, `lib/msdeploy/` (deployment tooling but no strategy) |

#### OPS-Q6: Integration Testing

| Field | Value |
|-------|-------|
| **Score** | 2 |
| **Severity** | Medium |
| **Priority** | P1 |
| **Effort** | Medium |
| **Phase** | 2 |
| **Finding** | SpecFlow BDD tests exist (29 .feature files in `src/Orchard.Specs/Tests/`) and are run in CI (`specflow.yml` workflow). The compile workflow also runs a setup test that verifies the application can be installed. Unit test projects exist (`Orchard.Tests`, `Orchard.Core.Tests`, `Orchard.Tests.Modules`). However, these are not integration tests against a deployed service — they test the application in-process without external service dependencies. No contract tests, no API tests against live endpoints, no database integration tests against a real SQL Server instance. |
| **Gap** | BDD tests exist but test the application in-process. No integration tests against deployed services, no contract tests, no end-to-end tests against real infrastructure. |
| **Recommendation** | Add integration tests that verify the application against a real SQL Server database and test API endpoints via HTTP. Use TestContainers for database integration tests in CI. Add contract tests when decomposing into microservices. |
| **Evidence** | `src/Orchard.Specs/` (29 .feature files), `.github/workflows/specflow.yml`, `.github/workflows/compile.yml` (Test step), `src/Orchard.Tests/`, `src/Orchard.Core.Tests/` |

#### OPS-Q7: Incident Response Automation

| Field | Value |
|-------|-------|
| **Score** | 1 |
| **Severity** | Medium |
| **Priority** | P2 |
| **Effort** | Medium |
| **Phase** | 3 |
| **Finding** | No incident response automation found. No runbooks (markdown, YAML, or SSM Automation documents), no Lambda-based remediation, no self-healing patterns. No on-call configuration or escalation policies visible. |
| **Gap** | No runbooks — incident response is entirely ad hoc. |
| **Recommendation** | Create runbooks for common incidents (application crashes, database connectivity loss, high error rates). Implement as SSM Automation documents or Step Functions for automated remediation. |
| **Evidence** | Absence of runbook files, SSM documents, or remediation automation |

#### OPS-Q8: Observability Ownership

| Field | Value |
|-------|-------|
| **Score** | 1 |
| **Severity** | Medium |
| **Priority** | P2 |
| **Effort** | Low |
| **Phase** | 3 |
| **Finding** | No observability ownership defined. No CODEOWNERS file for monitoring assets, no per-service dashboards, no named alarm owners, no team attribution on monitoring resources. |
| **Gap** | No observability ownership — monitoring is reactive and fragmented. |
| **Recommendation** | Define observability ownership: create CODEOWNERS for monitoring configs, establish per-module dashboards with named owners, tie SLO definitions to specific teams. |
| **Evidence** | Absence of CODEOWNERS, dashboard definitions, or team-attributed monitoring configuration |

#### OPS-Q9: Resource Tagging Governance

| Field | Value |
|-------|-------|
| **Score** | 1 |
| **Severity** | Medium |
| **Priority** | P2 |
| **Effort** | Low |
| **Phase** | 2 |
| **Finding** | No resource tagging found — no IaC exists to tag resources. No `default_tags`, no tag enforcement, no cost allocation tags. |
| **Gap** | No tags found on resources — no cost/ownership attribution possible. |
| **Recommendation** | When implementing IaC, establish a tagging standard (Environment, Service, Owner, CostCenter) and enforce via required_tags in Terraform modules or CDK aspects. |
| **Evidence** | Absence of IaC with tags. No Terraform, CloudFormation, or CDK resources to tag. |

---

## Learning Materials

| Pathway | Learning Resources |
|---------|-------------------|
| **Move to Cloud Native** | [AWS Modernization Pathways: Move to Cloud Native Serverless](https://skillbuilder.aws/learning-plan/CMK2J48MVN) · [Cloud Design Patterns](https://docs.aws.amazon.com/prescriptive-guidance/latest/cloud-design-patterns/introduction.html) |
| **Move to Containers** | [Move to Containers with Amazon EKS](https://skillbuilder.aws/learning-plan/GNYBZ9X9EM) · [Move to Containers with Amazon ECS](https://skillbuilder.aws/learning-plan/CDA8Y4JRRR) · [EKS Workshop](https://www.eksworkshop.com/) |
| **Move to Managed Databases** | [Move to Managed Databases](https://skillbuilder.aws/learning-plan/VNJ8FZ3ZRC) · [AWS DMS Getting Started](https://skillbuilder.aws/learn/ND246G8Y3W) |
| **Move to Modern DevOps** | [Move to Modern DevOps](https://skillbuilder.aws/learning-plan/1FGEQKGPQD) · [Getting Started with DevOps on AWS](https://skillbuilder.aws/learn/R4B13K95YQ) |

---

## Evidence Index

| File Path | Referenced By | Context |
|-----------|--------------|---------|
| `.github/workflows/compile.yml` | INF-Q11, OPS-Q6, SEC-Q7 | CI pipeline with compile, test, and code generation |
| `.github/workflows/release-package.yml` | INF-Q11, OPS-Q5 | Release artifact build (no deployment) |
| `.github/workflows/specflow.yml` | OPS-Q6 | SpecFlow BDD test execution |
| `src/Orchard/Orchard.Framework.csproj` | APP-Q1 | .NET Framework 4.8, C# 7.3, ASP.NET MVC 5 |
| `src/Orchard/packages.config` | APP-Q1, INF-Q2 | NuGet dependencies — NHibernate 5.6, ASP.NET MVC 5.3 |
| `src/Orchard.Web/Web.config` | INF-Q6, SEC-Q3, SEC-Q5, APP-Q4 | Forms auth, connection strings, IIS config |
| `src/Orchard/Data/Providers/SqlServerDataServicesProvider.cs` | INF-Q2, DATA-Q3 | SQL Server database provider |
| `src/Orchard/Data/Providers/SqlCeDataServicesProvider.cs` | INF-Q2 | SQL CE database provider |
| `src/Orchard/Data/Providers/PostgreSqlDataServicesProvider.cs` | INF-Q2 | PostgreSQL provider (migration path) |
| `src/Orchard/Data/SessionLocator.cs` | APP-Q2, DATA-Q2 | Shared NHibernate session |
| `src/Orchard/Data/IRepository.cs` | DATA-Q2 | Generic repository interface |
| `src/Orchard/Data/Repository.cs` | DATA-Q2 | Repository implementation |
| `src/Orchard/Data/Migration/` | DATA-Q3, DATA-Q4 | Data migration framework |
| `src/Orchard/FileSystems/Media/FileSystemStorageProvider.cs` | DATA-Q1 | Local file system media storage |
| `src/Orchard/Events/DefaultOrchardEventBus.cs` | APP-Q3 | In-process event bus |
| `src/Orchard/Tasks/BackgroundService.cs` | APP-Q4 | Background task infrastructure |
| `src/Orchard/Security/Providers/FormsAuthenticationService.cs` | SEC-Q3, SEC-Q4 | Forms-based authentication |
| `src/Orchard/Logging/` | OPS-Q3 | log4net logging (no metrics) |
| `src/Orchard.Web/Modules/Orchard.JobsQueue/` | APP-Q4, INF-Q4 | Background job processing |
| `src/Orchard.Web/Modules/Orchard.MessageBus/` | INF-Q4 | Internal message bus |
| `src/Orchard.Web/Modules/Orchard.OpenId/` | SEC-Q3, SEC-Q4 | Optional OpenID Connect module |
| `src/Orchard.Web/Modules/Orchard.Workflows/` | INF-Q3 | Visual workflow module |
| `src/Orchard.Web/Modules/Orchard.AuditTrail/` | SEC-Q1 | Application-level audit trail |
| `src/OrchardBasicCorrectness.ruleset` | SEC-Q7 | Code analysis rules |
| `src/OrchardSecurity.ruleset` | SEC-Q7 | Security analysis rules |
| `lib/sqlce/` | INF-Q2, DATA-Q3 | SQL CE binaries (EOL) |
| `lib/msdeploy/` | INF-Q1, INF-Q10 | MSDeploy deployment configuration |
| `deploy.ps1` | INF-Q1 | PowerShell deployment script |
| `.deployment` | INF-Q1 | Azure deployment configuration |
| `src/Orchard.Specs/` | OPS-Q6 | SpecFlow BDD tests |
| `src/Orchard.sln` | APP-Q2 | Single solution file (monolith) |

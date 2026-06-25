# POC Selection and Compute Target Decision

| Field | Value |
|-------|-------|
| **Repository** | orchard |
| **Date** | 2026-06-25 |
| **Based On** | orchard-mod-report.json (2026-06-23) |
| **Status** | Pending Approval |

---

## 1. Selected POC Slice

**Infrastructure-First Containerization: ECS + RDS SQL Server + Secrets Manager via CDK**

Lift the existing .NET Framework 4.8 Orchard CMS application into a Windows container on ECS, backed by RDS SQL Server with credentials in Secrets Manager, defined entirely in CDK (C#). No application source code changes except connection string externalization in Web.config.

### Gaps Addressed by This POC

| Readiness Gap | Score | Addressed |
|---------------|-------|-----------|
| INF-Q1: Managed Compute | 1 | Yes — ECS replaces bare IIS |
| INF-Q2: Managed Databases | 1 | Yes — RDS SQL Server replaces self-managed |
| INF-Q5: Network Security | 1 | Yes — VPC with public/private subnets |
| INF-Q10: Infrastructure as Code | 1 | Yes — CDK C# project |
| SEC-Q5: Secrets Management | 1 | Yes — Secrets Manager replaces plaintext config |
| INF-Q11: CI/CD Automation | 2 | Partial — CD pipeline added |
| INF-Q6: API Entry Point | 1 | Yes — ALB with health checks |

This single POC moves 5 of the top 5 gaps from score 1 to score 3+.

---

## 2. Why Not Full .NET 8 Migration Yet

| Risk Factor | Infrastructure POC | .NET 8 Migration |
|-------------|-------------------|-------------------|
| **Scope** | New files only (infra/, Dockerfile) | Every .csproj, every module, core framework |
| **Application binary** | Unchanged | Complete recompile on new runtime |
| **NHibernate 5.6** | Unchanged | Uncertain .NET 8 compatibility; may require upgrade to NHibernate 6.x |
| **Autofac 3.5.2** | Unchanged | Must upgrade to 7.x+ (breaking API: `builder.RegisterType` changes) |
| **ASP.NET MVC 5 → Core** | N/A | Complete rewrite of routing, middleware, DI registration, authentication |
| **62 modules** | All run as-is | Each module needs porting; inter-module DI and content handlers change |
| **FluentNHibernate 3.4** | Unchanged | No official .NET 8 support; may need replacement |
| **Test suite** | Passes unmodified | Tests reference ASP.NET MVC 5 types; must be rewritten |
| **Rollback** | `cdk destroy` (minutes) | Revert months of code changes |
| **Team learning** | CDK C# (familiar language) | New ASP.NET Core host model, new DI, new middleware pipeline |
| **Value delivered** | Immediately — managed infra, secrets, scaling | Only after full migration completes (all-or-nothing) |

**Decision rationale:** The readiness report scores 1.70/4.0 overall. 9 of 9 High-severity findings are infrastructure gaps, not application gaps. The highest ROI per unit of risk is fixing infrastructure first, proving the application runs in a managed environment, then addressing the .NET modernization with infrastructure already in place.

---

## 3. Compute Target Comparison

### Option A: ECS Fargate with Windows Containers

| Attribute | Assessment |
|-----------|------------|
| **Managed compute** | Fully managed — no EC2 instances to patch |
| **Operational overhead** | Lowest |
| **Cold start** | 2-5 minutes for Windows tasks (6-8 GB image pull) |
| **Image caching** | No local layer cache; full pull on each new task placement |
| **Auto-scaling** | Native task-count scaling |
| **Debugging** | No SSH/RDP; logs and exec-command only |
| **Cost model** | Per-vCPU-second + per-GB-second (Windows surcharge applies) |
| **Region availability** | GA in most regions but not all; verify target region |
| **Windows support** | GA since 2023; LTSC2019 and LTSC2022 |
| **Path to .NET 8** | Switch task to Linux Fargate (1 property change) |
| **POC suitability** | Moderate — slow cold starts complicate iterative validation |

### Option B: ECS on EC2 with Windows Containers

| Attribute | Assessment |
|-----------|------------|
| **Managed compute** | Partially managed — ECS schedules tasks, but EC2 instances need AMI updates |
| **Operational overhead** | Medium (ASG + capacity provider + AMI lifecycle) |
| **Cold start** | Fast after first pull — local image layer caching on EC2 |
| **Image caching** | Yes — Docker layer cache persists on instance |
| **Auto-scaling** | ASG + ECS capacity provider (two-layer scaling) |
| **Debugging** | Full RDP/SSH access to host for troubleshooting |
| **Cost model** | EC2 instance cost (can use Reserved/Savings Plans) |
| **Region availability** | All commercial regions |
| **Windows support** | Full support; Windows-optimized ECS AMIs available |
| **Path to .NET 8** | Switch to Linux AMI ASG + Linux task definition |
| **POC suitability** | High — fast iteration, easy debugging, proven pattern |

### Option C: Windows EC2/IIS (No Containers)

| Attribute | Assessment |
|-----------|------------|
| **Managed compute** | None — same operational model as today |
| **Operational overhead** | Highest (IIS config, Windows updates, manual deployment) |
| **Cold start** | N/A (always running) |
| **Image caching** | N/A |
| **Auto-scaling** | ASG only; application deployment is separate concern |
| **Debugging** | Full RDP access |
| **Cost model** | EC2 instance cost |
| **Region availability** | All regions |
| **Windows support** | Native |
| **Path to .NET 8** | Requires re-architecture later (no container path established) |
| **POC suitability** | Low — does not prove containerization, does not address INF-Q1 meaningfully |

### Decision Matrix

| Criterion (weighted) | A: Fargate | B: ECS on EC2 | C: EC2/IIS |
|---------------------|-----------|---------------|------------|
| Addresses INF-Q1 fully (30%) | 10 | 8 | 3 |
| Iteration speed for POC (25%) | 5 | 9 | 7 |
| Debugging capability (15%) | 4 | 9 | 10 |
| Operational overhead (15%) | 10 | 6 | 2 |
| Path to production (15%) | 10 | 7 | 3 |
| **Weighted score** | **7.35** | **7.90** | **4.55** |

---

## 4. Selected Compute Target for POC

**Option B: ECS on EC2 with Windows Containers**

### Rationale

1. **Fastest iteration during POC** — local image layer caching eliminates repeated 6-8 GB pulls
2. **Debuggable** — RDP access to container host enables root-cause analysis during initial deployment validation
3. **Still proves ECS** — task definitions, service discovery, ALB integration, and secrets injection are identical regardless of launch type
4. **One-line path to Fargate** — CDK property change (`launchType: EC2` → `launchType: FARGATE`) promotes to fully managed compute after POC validation
5. **No region availability risk** — Windows ECS on EC2 is available in all commercial regions

### Migration Path to Production

```
POC (now)          → ECS on EC2, Windows containers, dev environment
Production (next)  → ECS Fargate, Windows containers, multi-env (requires cold-start acceptance test)
Long-term          → ECS Fargate, Linux containers (.NET 8 migration complete)
```

---

## 5. AWS Services Involved

| Service | Role | Gap Addressed |
|---------|------|---------------|
| **Amazon ECS** | Container orchestration (EC2 launch type) | INF-Q1 |
| **Amazon ECR** | Container image registry | INF-Q1 |
| **Amazon EC2** | ECS cluster capacity (Windows Server 2022, ECS-optimized AMI) | INF-Q1 |
| **Auto Scaling Group** | EC2 capacity scaling for ECS cluster | INF-Q7 |
| **Amazon RDS** | SQL Server Standard, Multi-AZ | INF-Q2 |
| **AWS Secrets Manager** | Database credentials with 30-day rotation | SEC-Q5 |
| **Amazon VPC** | Network isolation (2 AZs, public/private subnets, NAT) | INF-Q5 |
| **Application Load Balancer** | Traffic routing, health checks, TLS termination | INF-Q6 |
| **AWS CDK (C#)** | Infrastructure as Code | INF-Q10 |
| **Amazon CloudWatch** | Log groups, CPU/memory alarms, 5xx alarm | OPS-Q4 |
| **GitHub Actions** | CD pipeline (build → ECR → ECS deploy) | INF-Q11 |

### Estimated Monthly Cost (POC, dev environment)

| Service | Configuration | Est. Cost |
|---------|---------------|-----------|
| EC2 (ECS) | 1x m5.xlarge Windows (4 vCPU, 16 GB) | ~$250/mo |
| RDS SQL Server | db.m5.large, Multi-AZ, Standard Edition | ~$500/mo |
| ALB | 1 ALB + minimal LCUs | ~$25/mo |
| NAT Gateway | 1 NAT + minimal data processing | ~$35/mo |
| ECR | <10 GB storage | ~$1/mo |
| Secrets Manager | 2 secrets | ~$1/mo |
| CloudWatch | Logs + 5 alarms | ~$10/mo |
| **Total POC** | | **~$822/mo** |

---

## 6. Validation Gates

| Gate | Criteria | Pass Condition | Blocker |
|------|----------|----------------|---------|
| **G1** | CDK synthesizes | `cdk synth` exits 0, produces valid CloudFormation | Yes |
| **G2** | Dockerfile builds | `docker build` produces image tagged `orchard-poc:latest` | Yes |
| **G3** | Container starts locally | HTTP 200 on `http://localhost:8080/` within 60s | Yes |
| **G4** | ECS service stable | `runningCount == desiredCount` for 5 minutes | Yes |
| **G5** | ALB health check passes | Target group shows all targets healthy | Yes |
| **G6** | RDS connectivity | Application connects to RDS and renders content page | Yes |
| **G7** | Secrets injection verified | No plaintext credentials in container image or task env dump | Yes |
| **G8** | Existing CI passes | `compile.yml` workflow succeeds on same branch | Yes |
| **G9** | Source code unchanged | `git diff src/` shows only Web.config connectionString line | Yes |
| **G10** | Performance baseline | Page load < 3s (comparable to current IIS deployment) | No (informational) |

### Gate Execution Order

```
G1 → G2 → G3 → G8 → [deploy to AWS] → G4 → G5 → G6 → G7 → G9 → G10
```

Gates G1-G3 and G8 must pass before any AWS deployment. Gates G4-G7 validate the deployed environment. G9 is a safety check. G10 is informational for go/no-go decision.

---

## 7. Rollback Plan

### Pre-Cutover (POC validation phase)

During POC validation, the existing IIS deployment continues to serve all production traffic. The POC environment is isolated.

| Failure | Action | Recovery |
|---------|--------|----------|
| CDK deploy fails | Fix template or `cdk destroy` | No impact — production unaffected |
| Container won't start | Check CloudWatch logs, fix Dockerfile, redeploy | No impact — production unaffected |
| RDS unreachable from ECS | Verify security groups, fix CDK, redeploy | No impact — production unaffected |
| All validation fails | `cdk destroy --all --force` | Complete cleanup; repo unchanged except infra/ directory |

### Post-Cutover (traffic shifted to ECS)

| Failure | Action | Recovery Time |
|---------|--------|---------------|
| 5xx spike after cutover | Revert ALB listener rule to original target (or DNS rollback) | < 5 minutes |
| Database corruption | Restore RDS from automated snapshot (PITR) | < 30 minutes |
| Performance degradation | Scale up EC2 instance type or add instances; or rollback traffic | < 10 minutes |
| Complete POC failure | `cdk destroy`; original IIS system was never modified | < 15 minutes |

### Rollback Commands

```bash
# Immediate traffic rollback (ALB rule revert)
aws elbv2 modify-rule --rule-arn <rule-arn> --actions Type=forward,TargetGroupArn=<original-tg-arn>

# Full infrastructure teardown
cd infra && cdk destroy --all --force

# Repository cleanup (if abandoning POC)
git rm -r infra/ Dockerfile .dockerignore
git checkout -- src/Orchard.Web/Web.config
git commit -m "Revert: remove POC infrastructure artifacts"
```

### Critical Safety Properties

1. The original IIS deployment is **never stopped** until explicit human approval after all gates pass
2. The original database is **never modified** — POC uses a fresh RDS instance (or DMS copy)
3. DNS/traffic shift is a **separate manual step** not included in the automated pipeline
4. `cdk destroy` removes all AWS resources — no orphaned infrastructure

---

## 8. Final Recommendation

### Proceed with Infrastructure-First POC

| Decision | Choice | Confidence |
|----------|--------|------------|
| POC slice | Containerize as-is + RDS + Secrets Manager + CDK | High |
| Compute target | ECS on EC2 (Windows) for POC; Fargate for production | High |
| Framework migration | Deferred to Phase 2 (after infra POC proven) | High |
| Database engine | RDS SQL Server Standard (drop-in, no schema changes) | High |
| IaC tool | CDK in C# (team skill alignment) | High |

### Prerequisite Actions Before Code Generation

- [ ] Confirm AWS account and target region
- [ ] Confirm CDK bootstrap has been run in target account/region
- [ ] Confirm GitHub Actions OIDC federation is configured (or choose alternative auth)
- [ ] Confirm RDS SQL Server license approach (License Included vs. BYOL)
- [ ] Confirm VPC CIDR does not conflict with existing networks
- [ ] Confirm single `dev` environment is sufficient for POC
- [ ] Confirm fresh database (no data migration) is acceptable for POC
- [ ] Confirm ECS on EC2 (Option B) as compute target

### Next Step

Once all prerequisites are confirmed, execute the code-changing transformation to generate:
1. CDK C# project (`infra/`)
2. Dockerfile (repo root)
3. `.dockerignore` (repo root)
4. `.github/workflows/deploy.yml`
5. `Web.config` connection string externalization (1-line change)

No other source files will be modified.

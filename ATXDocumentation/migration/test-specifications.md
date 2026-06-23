# Test Specifications - Orchard CMS

## Existing Test Infrastructure

### Unit Test Framework

| Framework | Version | Location |
|-----------|---------|----------|
| NUnit | 3.x | All test projects |
| SpecFlow | - | Orchard.Specs (BDD) |

### Test Projects

| Project | Scope | Test Count (est.) |
|---------|-------|-------------------|
| Orchard.Tests | Core framework | 100+ |
| Orchard.Tests.Modules | Feature modules | 50+ |
| Orchard.Core.Tests | Core modules | 30+ |
| Orchard.Web.Tests | Web layer | 10+ |
| Orchard.Azure.Tests | Azure integration | 10+ |
| Orchard.Specs | End-to-end BDD | 29 features |
| Tools/Orchard.Tests | CLI tool | 10+ |

## Migration Test Specifications

### Phase 1: Foundation Tests

| Test Case | Validates | Pass Criteria |
|-----------|-----------|---------------|
| DI container builds | Autofac/new DI wiring | All services resolvable |
| Repository CRUD | Data layer migration | Create, Read, Update, Delete succeeds |
| Transaction rollback | Transaction management | Failed operation rolls back |
| Cache get/set | Caching layer | Values persist and expire correctly |
| Logger injection | Logging framework | Logger instances injected into components |

### Phase 2: Content Management Tests

| Test Case | Validates | Pass Criteria |
|-----------|-----------|---------------|
| Create content item | ContentManager.New/Create | Item persisted with version 1 |
| Publish content | ContentManager.Publish | Published=true, version correct |
| Version content | ContentManager.GetDraft | New version created from published |
| Content query | IContentQuery | Filters, sorting, pagination work |
| Handler pipeline | ContentHandler invocation | All handlers called in order |
| Part composition | ContentType with multiple parts | All parts accessible on item |

### Phase 3: Security Tests

| Test Case | Validates | Pass Criteria |
|-----------|-----------|---------------|
| User creation | IMembershipService | User persisted with hashed password |
| Password validation | Hash verification | Correct password accepted, wrong rejected |
| Permission check (grant) | IAuthorizationService | User with role can access |
| Permission check (deny) | IAuthorizationService | User without role denied |
| Own content check | Content-level permissions | Owner can edit own, not others' |
| Anti-forgery validation | CSRF protection | Missing token returns 400 |

### Phase 4: Module Tests

| Test Case | Validates | Pass Criteria |
|-----------|-----------|---------------|
| Module discovery | ExtensionManager | All Module.txt files found |
| Feature enable/disable | IFeatureManager | Feature state persisted |
| Dependency resolution | Module dependencies | Dependencies enabled first |
| Data migration | IDataMigration | Schema created/updated |
| Route registration | Routes.cs | URLs resolve to correct controller |

### Phase 5: Integration Tests

| Test Case | Validates | Pass Criteria |
|-----------|-----------|---------------|
| Full request pipeline | End-to-end HTTP | 200 response with correct content |
| Multi-tenant isolation | Shell separation | Tenant A cannot see Tenant B data |
| Background task execution | IBackgroundTask | Tasks execute on schedule |
| Content import/export | XML recipe processing | Exported content re-imports correctly |
| Workflow execution | Trigger → Activity chain | Workflow completes all activities |

## Regression Test Requirements

| Area | Requirement |
|------|-------------|
| All existing NUnit tests | Must pass after migration |
| Content CRUD operations | No data loss or corruption |
| User authentication | Login/logout flow works |
| Multi-tenancy | Tenant isolation maintained |
| Search indexing | Lucene queries return correct results |
| Media upload | Files stored and retrievable |

## Cross-References

- [Validation Criteria](validation-criteria.md)
- [Component Order](component-order.md)

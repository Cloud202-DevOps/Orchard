# Security Patterns - Orchard CMS

## Authentication

### Forms Authentication

Orchard uses ASP.NET Forms Authentication as its primary authentication mechanism:

| Aspect | Implementation |
|--------|---------------|
| Cookie-based | Forms auth ticket in encrypted cookie |
| Encryption | Machine key-based (configurable in web.config) |
| Sliding expiration | Configurable timeout |
| Remember me | Extended cookie lifetime |
| Login URL | Configurable, defaults to `/Users/Account/LogOn` |

### Password Security

| Aspect | Implementation |
|--------|---------------|
| Hashing algorithm | PBKDF2 (default), SHA1 (legacy) |
| Salt | Per-user random salt |
| Iterations | Configurable (default varies by version) |
| Storage | Hash + salt + algorithm identifier in UserPartRecord |
| Password reset | Nonce-based email token with expiration |

### OpenID Support

External authentication via `Orchard.OpenId` module (optional).

## Authorization

### Permission-Based Access Control

```
Authorization Model:
  Permission (static, defined per module)
    ↓ assigned to
  Role (configurable, admin-managed)
    ↓ assigned to
  User (via UserRolesPartRecord)
```

### Built-in Roles

| Role | Description |
|------|-------------|
| Administrator | All permissions (implicit) |
| Editor | Content management |
| Moderator | Content moderation |
| Author | Own content creation |
| Contributor | Content submission (no publish) |
| Authenticated | Logged-in users |
| Anonymous | All users |

### Content-Level Permissions

- "Own" permission variants (e.g., `EditOwnContent`, `DeleteOwnContent`)
- Owner determined by `CommonPart.Owner`
- Per-content-item permission overrides via `Orchard.ContentPermissions`

## Input Validation

### Anti-Forgery (CSRF Protection)

| Aspect | Implementation |
|--------|---------------|
| Framework | ASP.NET MVC `[ValidateAntiForgeryToken]` |
| Scope | All POST form submissions |
| Token | Cookie + hidden form field pair |
| Module setting | `AntiForgery: enabled` in Module.txt |

### Model Validation

- Data annotations on ViewModels
- Server-side validation in controllers
- Client-side validation via jQuery Validate (where included)

### Content Sanitization

| Aspect | Implementation |
|--------|---------------|
| HTML body | Filtered through allowed tag/attribute whitelist |
| Rich text (TinyMCE) | Editor restricts available HTML elements |
| User input fields | HTML encoded on output by Razor (`@Model.Value`) |
| URL parameters | Route constraints limit format |

## Encryption

### IEncryptionService

- Uses `MachineKey` for encryption/decryption
- Used for: auth ticket encryption, sensitive token generation
- Algorithm: AES (via machine key configuration)

### HTTPS Enforcement

- `Orchard.SecureSocketsLayer` module forces HTTPS
- Configurable per-URL pattern
- Redirects HTTP → HTTPS

## Security Headers

Configured via web.config and IIS:
- `X-Frame-Options` — not explicitly set (depends on IIS config)
- `X-Content-Type-Options` — not explicitly set
- Content Security Policy — not implemented by default

## Known Security Considerations

| Area | Concern | Mitigation |
|------|---------|-----------|
| Dynamic compilation | Could execute arbitrary code | Module loading restricted to Modules/ folder |
| XML-RPC | Attack surface for brute force | Rate limiting recommended at IIS level |
| Forms Auth | Cookie theft risk | HTTPS + secure cookie flags recommended |
| Machine Key | Shared key across farm | Must be explicitly configured in web.config |
| SQL Injection | NHibernate parameterized queries | ORM prevents injection by default |
| XSS | Razor auto-encoding | `@Html.Raw()` usage must be audited |
| File Upload | Malicious file execution | Extension whitelist + content type validation |

## Security Module Dependencies

```
Orchard.Users (authentication)
  └── Orchard.Roles (authorization)
       └── Orchard.ContentPermissions (per-item ACL)

Orchard.AntiSpam (spam protection)
  ├── CAPTCHA integration
  └── Akismet integration

Orchard.SecureSocketsLayer (HTTPS)
```

## Cross-References

- [Error Handling](../behavior/error-handling.md)
- [Interfaces - Security](../reference/interfaces.md)
- [Decision Logic - Authorization](../behavior/decision-logic.md)

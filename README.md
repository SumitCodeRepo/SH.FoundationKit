# SH.FoundationKit – Development Progress

## 🎯 Objective

`SH.FoundationKit` is a modular, reusable infrastructure package for .NET 9+ applications. It aims to simplify and standardize base application setup with support for:

- Authentication & Authorization
- Logging
- Health Checks
- Resilience (Polly)
- Caching (Hybrid)
- Rate Limiting
- Validation
- Time Zone Management
- ETag Support
- Benchmarking
- Global Exception Handling
- Correlation ID
- API Response Wrapping

---

## 📋 Planned Features & Modules

| Feature                         | Status    | Notes                                                         |
|---------------------------------|-----------|---------------------------------------------------------------|
| Authentication Integration      | Planned   | Integrate with Identity & custom RoleAccess table             |
| Dynamic Authorization Policies  | Planned   | Use `[Authorize(Resource = "X")]` without hardcoded policies  |
| Role-Based Access via DbContext | Planned   | Data-driven permissions                                       |
| Structured Logging Middleware   | Done      | Include user, trace ID, request info                          |
| Health Checks (Configurable)    | Done      | DB, services, file, memory, etc.                              |
| Resilience (Retry, Timeout, CB) | Planned   | Use Polly with named policies                                 |
| Rate Limiting                   | Done      | Middleware-based, optional config                             |
| Hybrid Caching                  | Planned   | Memory + Redis (Distributed)                                  |
| Validation Pipeline             | Planned   | FluentValidation + model state wrap                           |
| ETag Support for GET            | Done      | Return 304 if data unchanged                                  |
| Idempotency for POST/PUT        | Done      | Prevent duplicate submissions                                 |
| API Result Wrapper              | Planned   | Consistent response format                                    |
| Global Exception Handler        | Done      | ProblemDetails + logging                                      |
| Correlation ID                  | Planned   | Attach + log per request                                      |
| Benchmarking (Request Time)     | Planned   | Track & log elapsed ms                                        |
| Time Zone Middleware            | Planned   | Normalize client timezone                                     |
| Localization Support            | Planned   | Enable i18n/extensibility                                     |

---

## 🆕 Additional Modules Being Developed

### ✅ Idempotency Middleware
- Prevents duplicate processing of POST/PUT requests.
- Key based on route + body hash.
- Does not require custom headers from client.

### ✅ ETag Filter (No Middleware Required)
- Works via `IAsyncActionFilter`.
- Computes ETag from response + request URI.
- Automatically returns `304 Not Modified` if cached.

---

## ⚙️ Configuration Driven Setup

All features are toggleable via `appsettings.json`:

```json
"FoundationKit": {
  "EnableLogging": true,
  "EnableHealthChecks": true,
  "EnableSwagger": true,
  "EnableETag": true
}
```

In code:

```csharp
services.AddFoundationKit(configuration);
app.UseFoundationKit(configuration);
```

---

## 🚧 Known Issues / Deferred Items

| Feature                          | Status    | Notes |
|----------------------------------|-----------|-------|
| Swagger UI custom header JS      | Pending   | Embedded file not loading yet |
| Static File Serving (embedded)   | Debugging | Using `ManifestEmbeddedFileProvider` |
| Swagger JS Header Button UI      | Pending   | Not active in current build |

---

## 🧪 Test Structure

Tests are organized under:
```
/tests/SH.FoundationKit.Tests
```

Sample test for dynamic authorization is under active setup.

---

> This document tracks progress toward 1.0 release of SH.FoundationKit

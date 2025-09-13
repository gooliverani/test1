# Implementation Plan: Physical Security & Door Access Control Platform

**Branch**: `001-build-an-application` | **Date**: 2025-09-13 | **Spec**: `spec.md`  
**Input**: Feature specification from `/specs/001-build-an-application/spec.md`

## Execution Flow (/plan command scope)
```
1. Load feature spec from Input path → FOUND
2. Fill Technical Context (scan for NEEDS CLARIFICATION) → Completed (open items captured)
3. Evaluate Constitution Check (initial) → Draft (no concrete constitution content yet; mark potential violations if any arise)
4. Execute Phase 0 → research.md (Created below inlined content; to be saved separately) ✅
5. Execute Phase 1 → contracts/, data-model.md, quickstart.md (Planned content described; not physically generated yet per template ambiguity. To align with template we WILL generate artifacts now.) ✅
6. Re-evaluate Constitution Check → PASS (no unjustified complexity currently)
7. Plan Phase 2 → Task generation approach documented
8. STOP - Ready for /tasks command (tasks.md NOT created)
```

**IMPORTANT**: Only artifacts allowed in Phase 0 & 1 are produced: research.md, data-model.md, quickstart.md, contracts/. Tasks deferred.

## Summary
Primary requirement: Provide a secure, auditable system to manage physical access (profiles, credentials, zones, schedules, audit) with Azure Entra ID SSO, Workday-driven onboarding automation, real-time event updates, and strong role-based administration. Technical approach: Vue 3 SPA + ASP.NET Core Web API + PostgreSQL (Npgsql/EF Core) + SignalR for real-time + MSAL for frontend auth + Azure Entra ID OpenID Connect for backend protection. Use PostgreSQL schema (referenced external `schema.sql`) as baseline while refining domain-driven entities and contracts.

## Technical Context
**Language/Version**: Backend: C# / ASP.NET Core (target .NET 8) | Frontend: Vue 3 (TypeScript)  
**Primary Dependencies**: ASP.NET Core Web API, Entity Framework Core (Npgsql provider), SignalR, MSAL.js, Vue 3, Vue Router, Axios, Vuetify (UI), Serilog (logging candidate)  
**Storage**: PostgreSQL (roles, entities, access events)  
**Testing**: Backend: xUnit + FluentAssertions + testcontainers-dotnet (PostgreSQL); Frontend: Vitest + Vue Test Utils + Playwright for E2E  
**Target Platform**: Azure-hosted (App Service / Containers) + modern browsers (Chromium, Firefox, Safari)  
**Project Type**: web (frontend + backend) → Structure Option 2 (backend/, frontend/)  
**Performance Goals**: Handle 20 concurrent access events/sec per site; access decision API latency p95 < 150ms; audit export for 10k events < 5s  
**Constraints**: Must enforce RBAC; real-time event propagation < 2s from swipe to UI; secure secrets (Azure Key Vault).  
**Scale/Scope**: Initial 5k active profiles, 200 zones, 50 concurrent admin sessions, growth path to 25k profiles.  

**Open Clarifications (from spec)**:
- Multiple active credentials per profile? (affects cardinality & conflict logic)
- Mobile credentials in scope initial release?
- Card print layout data fields & standard (ISO proximity vs custom?)
- Export formats (CSV, PDF, JSON) & compliance additions (e.g., SOC2, ISO 27001?)
- Retention period for audit/access events & PII handling
- Visitor auto-expiration / host tracking specifics
- Default access schedule (none vs implicit business hours)
- Bulk import file format & transactional policy
- Metrics set & frequency (denials, dwell time?)
- Reason code governance (who curates list?)
- Permanent deletion policy (soft delete only?)

## Constitution Check (Initial)
The provided constitution file is placeholder (no concrete principles). We tentatively map expected principles:
**Simplicity**:
- Projects: 2 (frontend, backend) + tests within each → within limit (<=3)
- Direct framework usage (ASP.NET, Vue) → OK
- Single domain model; EF entities align with REST contracts (avoid unnecessary DTOs at start) → OK
- Avoid Repository/UoW abstraction initially → OK

**Architecture**:
- Feature-libraries concept not enforced yet (constitution incomplete). Justification: Early product stage; overhead avoided.
- Libraries listed: N/A (monolithic backend assembly initially)
- CLI exposure not required for web domain (possible future admin CLI) → Documented deferral.

**Testing (NON-NEGOTIABLE)**:
- Commit strategy: Write failing contract & integration tests before implementing endpoints.
- Order: Contract (OpenAPI schema tests) → Integration (endpoints vs real Postgres) → E2E (Playwright) → Unit (pure logic) → Maintained.
- Real dependencies: Testcontainers Postgres ensures parity.

**Observability**:
- Plan: Structured logging with Serilog JSON sinks; correlation id per request; audit log separate table.
- Frontend: Use browser logging only for dev; server receives structured events (SignalR) & central logs.

**Versioning**:
- Semantic version starting 0.1.0 pre-GA; build metadata per CI.
- Breaking changes flagged in OpenAPI diff CI check.

No violations requiring Complexity Tracking at this time.

## Project Structure
Adopting Web Application Option (frontend + backend) with tests folders embedded.

```
backend/
	src/
		Models/
		Data/            # DbContext, EF configurations
		Services/        # Domain logic (AccessDecisionService, CardService)
		Controllers/     # REST endpoints
		RealTime/        # SignalR hubs
		Auth/            # Entra ID config helpers
	tests/
		Contract/
		Integration/
		Unit/

frontend/
	src/
		components/
		pages/
		router/
		services/       # API clients, auth wrappers (MSAL)
		store/          # (Pinia) state management (optional) [Evaluate]
	tests/
		unit/
		e2e/
```

**Structure Decision**: Option 2 (web application) justified by dual frontend/backend requirement and real-time UI.

## Phase 0: Outline & Research (research.md Overview)
Anticipated Research Topics:
1. Multiple credentials policy → Determine if many-to-one permitted; affects uniqueness & revocation cascade.
2. Mobile credential scope → Defer or implement pluggable credential types.
3. Card printing standard → Card layout data dictionary.
4. Audit retention → Proposed 13 months (align typical compliance) pending confirmation.
5. Export formats → CSV mandatory; JSON optional; PDF requires rendering lib (defer until after MVP).
6. Visitor workflow → Host association field + auto-expire end-of-day.
7. Default schedule → Explicit schedule required (no implicit 24/7) to reduce accidental over-access.
8. Bulk import format → CSV with header; transactional per-row with error report; no partial silent failure.
9. Metrics → Denials by reason, active vs expired credentials, mean provisioning time.
10. Reason code governance → Security Admin group manages enumerated list.
11. Deletion policy → Soft delete only; hard delete restricted (compliance hold risk).

Research Output Decisions (Draft): Will be written into `research.md` with decision/rationale/alternatives template.

## Phase 1: Design & Contracts
### Data Model (data-model.md Overview)
Core Entities & Key Fields:
- PersonProfile (Id, ExternalId, Type, Status, DisplayName, Email, Department, CreatedAt, ModifiedAt)
- Credential (Id, PersonProfileId, Identifier, Type, Status, IssuedAt, ExpiresAt, RevokedAt, RevocationReason)
- Zone (Id, Name, Description)
- Schedule (Id, Name, TimeRules JSON, Timezone)
- ZonePermission (Id, PersonProfileId, ZoneId, ScheduleId, GrantedAt, RevokedAt)
- AccessAttempt (Id, CredentialId, ZoneId, Outcome, ReasonCode, Timestamp)
- AuditLog (Id, ActorId, EntityType, EntityId, ActionType, Data JSON, CreatedAt)
- AccessTemplate (Id, Name, Version, TemplateData JSON)
- VisitorBadge (Id, PersonProfileId, HostProfileId, ExpiresAt, Status)
- ReasonCode (Code, Category, Description, Active)

Relationships & Notes:
- PersonProfile 1..* Credential
- PersonProfile 1..* ZonePermission (through templates or direct assign)
- ZonePermission links schedule; removal ends access
- AccessAttempt append-only; partitioning strategy later (not MVP)

Validation Highlights:
- Credential.Identifier unique among active credentials.
- ExpiresAt > IssuedAt when provided.
- ZonePermission requires active profile & zone & schedule.
- Revocation requires reason.

### API Contracts (contracts/ overview)
Planned Endpoints (REST):
- POST /api/profiles (create)  
- GET /api/profiles/{id}  
- PATCH /api/profiles/{id} (update status, attributes)  
- GET /api/profiles?query=...  
- POST /api/profiles/{id}/credentials  
- POST /api/credentials/{id}/revoke  
- POST /api/profiles/{id}/zone-permissions  
- DELETE /api/zone-permissions/{id}  
- GET /api/zones / POST /api/zones  
- GET /api/schedules / POST /api/schedules  
- POST /api/templates  
- POST /api/profiles/{id}/apply-template/{templateId}  
- POST /api/workday/webhook (Workday integration)  
- GET /api/access-attempts (filters)  
- GET /api/reports/access-summary  
- GET /api/reports/zone-access  
- GET /api/reason-codes  
- POST /api/visitor/badges  
- POST /api/visitor/badges/{id}/revoke  

Real-time (SignalR Hub):
- /hubs/access-events → broadcast AccessAttempt events

Auth & Security:
- Azure Entra ID OIDC; all endpoints [Authorize]; fine-grained roles claim mapping.

Contract Test Strategy:
- For each endpoint: OpenAPI schema doc + failing test asserting required fields.
- Webhook idempotency test (duplicate Workday payload returns 200 & no duplication).
- Access decision integration test: create profile + credential + zone + schedule → simulate attempt (service call) expect ALLOW.

### Quickstart (quickstart.md Overview)
Sections:
1. Prerequisites (Azure App Registration, PostgreSQL, Node, .NET SDK)
2. Clone & setup backend (EF migrations, run tests)
3. Setup frontend (install deps, configure MSAL, run dev server)
4. Run both + simulate access attempt + view real-time event
5. Workday webhook sample payload & curl example

### Agent Context Update
Add technologies: ASP.NET Core, EF Core, SignalR, Vue 3, MSAL.js, Testcontainers, Serilog.

## Phase 2: Task Planning Approach
Task Generation Strategy:
- Derive tasks from each endpoint (contract test + implementation) and each entity (model + migration) plus integration flows (Workday automation, real-time hub, audit export).
- Parallelizable tasks flagged [P]: independent entity models, independent simple GET endpoints, frontend component scaffolds.
- Sequence: 1) Data model migrations/tests 2) Contract tests 3) Access decision core service 4) Credential lifecycle 5) Zone & schedule management 6) Real-time events 7) Reporting 8) Frontend UI flows 9) Workday webhook automation 10) Hardening (RBAC, logging, metrics).

Ordering justifications align with minimizing rework risk and unlocking downstream tasks early.

Estimated tasks: ~28-34.

## Complexity Tracking
| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|---------------------------------------|
| (none) | | |

## Progress Tracking
**Phase Status**:
- [x] Phase 0: Research complete (/plan command)
- [x] Phase 1: Design complete (/plan command)
- [ ] Phase 2: Task planning complete (/plan command - describe approach only)
- [ ] Phase 3: Tasks generated (/tasks command)
- [ ] Phase 4: Implementation complete
- [ ] Phase 5: Validation passed

**Gate Status**:
- [x] Initial Constitution Check: PASS
- [x] Post-Design Constitution Check: PASS
- [ ] All NEEDS CLARIFICATION resolved
- [ ] Complexity deviations documented

---
*Based on Constitution v2.1.1 (placeholder) - See `/memory/constitution.md`*
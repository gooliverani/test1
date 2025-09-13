# Tasks: Physical Security & Door Access Control Platform

**Input**: Design documents from `/specs/001-build-an-application/`
**Prerequisites**: plan.md (required), research.md, data-model.md, contracts/, quickstart.md

## Execution Flow (main)
```
1. Loaded plan.md → extracted stack (ASP.NET Core, Vue 3, PostgreSQL, SignalR)
2. Loaded data-model.md → 11 entities → model & migration tasks
3. Loaded contracts/ (openapi.yaml) → initial contracts tasks + expansion tasks
4. Loaded research.md → decisions → setup & policy tasks
5. Loaded quickstart.md → integration scenario tests
6. Generated ordered tasks with TDD precedence
7. Marked [P] for parallelizable distinct-file tasks
8. Built dependency/parallel guidance & validation checklist
9. SUCCESS
```

## Format
`[ID] [P?] Description (file path)`

Legend: [P] = runnable in parallel (different file / no dependency)

---
## Phase 3.1: Setup
- [ ] T001 Create backend solution + project scaffolding (`backend/`): ASP.NET Core Web API (.NET 8), folders: Models, Data, Services, Controllers, RealTime, Auth
- [ ] T002 Create frontend Vue 3 + TypeScript + Vuetify project in `frontend/` (Vite) with router & base layout
- [ ] T003 Initialize root tooling: `.editorconfig`, license placeholder, README stub
- [ ] T004 [P] Add Serilog + structured logging configuration (`backend/src/Program.cs`, `appsettings.Development.json`)
- [ ] T005 [P] Add EF Core + Npgsql packages & baseline `ApplicationDbContext` (`backend/src/Data/ApplicationDbContext.cs`)
- [ ] T006 [P] Add Testcontainers setup project & first failing test harness (`backend/tests/Integration/DatabaseFixture.cs`)
- [ ] T007 Configure Azure Entra ID auth placeholders (OIDC config, `backend/src/Auth/EntraIdExtensions.cs`) (no secrets committed)
- [ ] T008 Setup MSAL config scaffold (`frontend/src/services/authConfig.ts`) referencing placeholder env vars
- [ ] T009 Add lint + formatting (ESLint + Prettier) to frontend; dotnet format config for backend

## Phase 3.2: Tests First (TDD)
Contract Tests (existing + planned endpoints). Each test starts failing.
- [ ] T010 Generate OpenAPI contract expansion for all planned endpoints (`specs/001-build-an-application/contracts/openapi.yaml`) BEFORE implementation
- [ ] T011 [P] Contract test: POST /api/profiles (validation & 201) (`backend/tests/Contract/Profiles_PostTests.cs`)
- [ ] T012 [P] Contract test: GET /api/profiles/{id} 200 & 404 (`backend/tests/Contract/Profiles_GetByIdTests.cs`)
- [ ] T013 [P] Contract test: GET /api/profiles list (`backend/tests/Contract/Profiles_ListTests.cs`)
- [ ] T014 [P] Contract test: PATCH /api/profiles/{id} (status change) (`backend/tests/Contract/Profiles_PatchTests.cs`)
- [ ] T015 [P] Contract test: POST /api/profiles/{id}/credentials (`backend/tests/Contract/Credentials_PostTests.cs`)
- [ ] T016 [P] Contract test: POST /api/credentials/{id}/revoke (`backend/tests/Contract/Credentials_RevokeTests.cs`)
- [ ] T017 [P] Contract test: POST /api/profiles/{id}/zone-permissions (`backend/tests/Contract/ZonePermissions_PostTests.cs`)
- [ ] T018 [P] Contract test: DELETE /api/zone-permissions/{id} (`backend/tests/Contract/ZonePermissions_DeleteTests.cs`)
- [ ] T019 [P] Contract test: GET /api/zones (`backend/tests/Contract/Zones_ListTests.cs`)
- [ ] T020 [P] Contract test: POST /api/zones (`backend/tests/Contract/Zones_PostTests.cs`)
- [ ] T021 [P] Contract test: GET /api/schedules (`backend/tests/Contract/Schedules_ListTests.cs`)
- [ ] T022 [P] Contract test: POST /api/schedules (`backend/tests/Contract/Schedules_PostTests.cs`)
- [ ] T023 [P] Contract test: POST /api/templates (`backend/tests/Contract/Templates_PostTests.cs`)
- [ ] T024 [P] Contract test: POST /api/profiles/{id}/apply-template/{templateId} (`backend/tests/Contract/Templates_ApplyTests.cs`)
- [ ] T025 [P] Contract test: POST /api/workday/webhook idempotency (`backend/tests/Contract/Workday_WebhookTests.cs`)
- [ ] T026 [P] Contract test: GET /api/access-attempts filters (`backend/tests/Contract/AccessAttempts_ListTests.cs`)
- [ ] T027 [P] Contract test: GET /api/reports/access-summary (`backend/tests/Contract/Reports_AccessSummaryTests.cs`)
- [ ] T028 [P] Contract test: GET /api/reports/zone-access (`backend/tests/Contract/Reports_ZoneAccessTests.cs`)
- [ ] T029 [P] Contract test: GET /api/reason-codes (`backend/tests/Contract/ReasonCodes_ListTests.cs`)
- [ ] T030 [P] Contract test: POST /api/visitor/badges (`backend/tests/Contract/VisitorBadges_PostTests.cs`)
- [ ] T031 [P] Contract test: POST /api/visitor/badges/{id}/revoke (`backend/tests/Contract/VisitorBadges_RevokeTests.cs`)
- [ ] T032 [P] Contract test: SignalR hub handshake /hubs/access-events (`backend/tests/Contract/AccessEvents_HubHandshakeTests.cs`)

Integration Tests (user stories & quickstart flows)
- [ ] T033 Access decision flow: create profile → credential → zone → schedule → permission → simulate attempt ALLOW (`backend/tests/Integration/AccessDecisionFlowTests.cs`)
- [ ] T034 Credential revocation denial scenario (`backend/tests/Integration/CredentialRevocationTests.cs`)
- [ ] T035 Workday webhook create + idempotent retry (`backend/tests/Integration/WorkdayWebhookFlowTests.cs`)
- [ ] T036 Visitor badge auto-expire simulation (`backend/tests/Integration/VisitorBadgeExpiryTests.cs`)
- [ ] T037 Export zone access report (CSV structure) (`backend/tests/Integration/Reports_ZoneAccessExportTests.cs`)
- [ ] T038 Real-time event broadcast latency test stub (<2s) (`backend/tests/Integration/RealTimeBroadcastTests.cs`)

Frontend Contract/E2E Prep
- [ ] T039 [P] E2E auth login scenario skeleton (Playwright) (`frontend/tests/e2e/auth.spec.ts`)
- [ ] T040 [P] E2E profile creation + list refresh via API (`frontend/tests/e2e/profiles.spec.ts`)
- [ ] T041 [P] E2E access event real-time display stub (`frontend/tests/e2e/access-events.spec.ts`)

## Phase 3.3: Core Implementation (Backend Models & Services)
Entity Models & Migrations (parallelizable)
- [ ] T042 [P] Create PersonProfile entity + migration (`backend/src/Models/PersonProfile.cs`, migration)
- [ ] T043 [P] Create Credential entity + migration (`backend/src/Models/Credential.cs`)
- [ ] T044 [P] Create Zone entity + migration (`backend/src/Models/Zone.cs`)
- [ ] T045 [P] Create Schedule entity + migration (`backend/src/Models/Schedule.cs`)
- [ ] T046 [P] Create ZonePermission entity + migration (`backend/src/Models/ZonePermission.cs`)
- [ ] T047 [P] Create AccessAttempt entity + migration (`backend/src/Models/AccessAttempt.cs`)
- [ ] T048 [P] Create AuditLog entity + migration (`backend/src/Models/AuditLog.cs`)
- [ ] T049 [P] Create AccessTemplate entity + migration (`backend/src/Models/AccessTemplate.cs`)
- [ ] T050 [P] Create VisitorBadge entity + migration (`backend/src/Models/VisitorBadge.cs`)
- [ ] T051 [P] Create ReasonCode entity + migration (`backend/src/Models/ReasonCode.cs`)

Core Services
- [ ] T052 AccessDecisionService (evaluate credential/zone/time) (`backend/src/Services/AccessDecisionService.cs`)
- [ ] T053 ProfileService (CRUD + Workday upsert logic) (`backend/src/Services/ProfileService.cs`)
- [ ] T054 CredentialService (issue/revoke w/ reason) (`backend/src/Services/CredentialService.cs`)
- [ ] T055 ZonePermissionService (grant/revoke) (`backend/src/Services/ZonePermissionService.cs`)
- [ ] T056 TemplateService (apply template to profile) (`backend/src/Services/TemplateService.cs`)
- [ ] T057 ReportingService (summary & zone access export) (`backend/src/Services/ReportingService.cs`)
- [ ] T058 VisitorBadgeService (issue/expire/revoke) (`backend/src/Services/VisitorBadgeService.cs`)
- [ ] T059 ReasonCodeService (list + governance) (`backend/src/Services/ReasonCodeService.cs`)
- [ ] T060 RealTimeEventPublisher (SignalR abstraction) (`backend/src/RealTime/RealTimeEventPublisher.cs`)

Controllers / Endpoints (sequential groups to limit merge conflicts)
- [ ] T061 ProfilesController (POST, GET by id, list, PATCH) (`backend/src/Controllers/ProfilesController.cs`)
- [ ] T062 CredentialsController (issue, revoke) (`backend/src/Controllers/CredentialsController.cs`)
- [ ] T063 ZonePermissionsController (grant, delete) (`backend/src/Controllers/ZonePermissionsController.cs`)
- [ ] T064 ZonesController (list, create) (`backend/src/Controllers/ZonesController.cs`)
- [ ] T065 SchedulesController (list, create) (`backend/src/Controllers/SchedulesController.cs`)
- [ ] T066 TemplatesController (create, apply) (`backend/src/Controllers/TemplatesController.cs`)
- [ ] T067 WorkdayWebhookController (POST webhook) (`backend/src/Controllers/WorkdayWebhookController.cs`)
- [ ] T068 AccessAttemptsController (list with filters) (`backend/src/Controllers/AccessAttemptsController.cs`)
- [ ] T069 ReportsController (access-summary, zone-access) (`backend/src/Controllers/ReportsController.cs`)
- [ ] T070 ReasonCodesController (list) (`backend/src/Controllers/ReasonCodesController.cs`)
- [ ] T071 VisitorBadgesController (issue, revoke) (`backend/src/Controllers/VisitorBadgesController.cs`)
- [ ] T072 AccessEventsHub (SignalR hub) (`backend/src/RealTime/AccessEventsHub.cs`)

Auth / Middleware / Infrastructure
- [ ] T073 Entra ID authentication & JWT validation (`backend/src/Auth/EntraIdExtensions.cs`, Program wiring)
- [ ] T074 Role-based policy configuration (`backend/src/Auth/AuthorizationPolicies.cs`)
- [ ] T075 Global exception handler + problem details (`backend/src/Middleware/ExceptionHandlingMiddleware.cs`)
- [ ] T076 Request/response logging enrichment (`backend/src/Services/LoggingEnricher.cs`)
- [ ] T077 Background job for credential expiration sweep (`backend/src/Services/ExpirationJob.cs`)

## Phase 3.4: Frontend Implementation
State & Services
- [ ] T078 API client base (Axios + token injection) (`frontend/src/services/apiClient.ts`)
- [ ] T079 Auth service wrapper (MSAL flows) (`frontend/src/services/authService.ts`)
- [ ] T080 Real-time SignalR client (`frontend/src/services/realtime.ts`)

Core Views / Components (parallelizable initial stubs)
- [ ] T081 [P] Profiles list + detail view (`frontend/src/pages/ProfilesPage.vue`)
- [ ] T082 [P] Profile create/edit form (`frontend/src/components/ProfileForm.vue`)
- [ ] T083 [P] Zones management view (`frontend/src/pages/ZonesPage.vue`)
- [ ] T084 [P] Schedules management view (`frontend/src/pages/SchedulesPage.vue`)
- [ ] T085 [P] Templates management view (`frontend/src/pages/TemplatesPage.vue`)
- [ ] T086 [P] Visitor badges issuance view (`frontend/src/pages/VisitorBadgesPage.vue`)
- [ ] T087 [P] Access attempts real-time dashboard (`frontend/src/pages/AccessEventsDashboard.vue`)
- [ ] T088 [P] Reports view (summary + zone) (`frontend/src/pages/ReportsPage.vue`)

Cross-cutting Frontend
- [ ] T089 Global navigation & layout shell (`frontend/src/components/AppShell.vue`)
- [ ] T090 Route definitions & guards (auth required) (`frontend/src/router/index.ts`)
- [ ] T091 Central store (Pinia) for profiles, zones (optional evaluation) (`frontend/src/store/index.ts`)

## Phase 3.5: Polish & Hardening
- [ ] T092 Contract test refinements for error cases (400/401/403) (augment existing tests)
- [ ] T093 Unit tests: AccessDecisionService edge cases (`backend/tests/Unit/AccessDecisionServiceTests.cs`)
- [ ] T094 Unit tests: ReportingService aggregations (`backend/tests/Unit/ReportingServiceTests.cs`)
- [ ] T095 Performance test harness (e.g., k6 script placeholder) (`tests/perf/access_decision.js`)
- [ ] T096 Security review checklist & threat model doc (`docs/security/threat-model.md`)
- [ ] T097 Add OpenAPI diff CI script (`scripts/check-openapi-diff.sh`) (failing until integrated)
- [ ] T098 Observability: enrich logs with correlation + user claims (`backend/src/Middleware/CorrelationMiddleware.cs`)
- [ ] T099 Metrics emission stub (denials by reason) (`backend/src/Services/MetricsPublisher.cs`)
- [ ] T100 Data retention job stub (archival strategy placeholder) (`backend/src/Services/ArchivalJob.cs`)
- [ ] T101 Frontend accessibility pass (WCAG checklist) (`frontend/docs/a11y.md`)
- [ ] T102 Documentation: update `README.md` with run + test instructions
- [ ] T103 Documentation: admin guide for credential lifecycle (`docs/credential-lifecycle.md`)
- [ ] T104 Cleanup: remove unused placeholders & verify lint passes

## Dependencies Overview
High-Level:
1. T001-T009 before any tests
2. Contract tests (T010-T032) before corresponding controllers (T061-T072)
3. Integration tests (T033-T038) before implementing the logic they assert (services T052-T060)
4. Models (T042-T051) before services (T052-T060)
5. Services before controllers
6. Backend core before frontend consumption (frontend tasks depend on endpoints & real-time hub)
7. Auth middlewares (T073-T074) before protected controller tests can pass fully (initially tests may bypass or use test auth harness)
8. Polish tasks after functional completion

## Parallel Execution Example
```
# Example Batch 1 (after setup):
T011 T012 T013 T014 (contract tests same domain but different files)

# Example Batch 2 (models):
T042 T043 T044 T045 T046 (run migrations sequentially inside each task but work can be parallelized across files if coordinated)

# Example Batch 3 (frontend components):
T081 T082 T083 T084
```

## Validation Checklist
- [ ] All contract endpoints mapped to a controller task
- [ ] Each entity has a model + migration task
- [ ] Tests precede implementation
- [ ] [P] tasks reference distinct files
- [ ] Real-time hub has contract + implementation tasks
- [ ] Workday webhook has test + controller task
- [ ] Reporting endpoints have test + service + controller tasks

## Notes
- Keep commits small: one task per commit where feasible.
- Replace placeholder hub test with actual SignalR client handshake logic once hub stub present.
- refine openapi.yaml incrementally: keep tests failing until endpoints implemented.

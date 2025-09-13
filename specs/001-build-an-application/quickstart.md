# Quickstart

## 1. Prerequisites
- .NET 8 SDK
- Node.js 20+
- PostgreSQL 15+ (local or container)
- Azure Entra ID App Registration (Web + SPA)
- Environment variables: `ASPNETCORE_ENVIRONMENT=Development`

## 2. Backend Setup
1. Create database & user.
2. Apply EF Core migrations (to be generated later).
3. Run tests (contract should fail initially until implemented).
4. Launch API: `dotnet run` in `backend/`.

## 3. Frontend Setup
1. `cd frontend`
2. Install deps (`npm install`)
3. Configure MSAL settings in `authConfig.ts` (clientId, authority)
4. `npm run dev`

## 4. Authentication Flow Test
1. Navigate to SPA → login via Entra ID → token stored
2. Call `GET /api/profiles` (should 200 with empty list)

## 5. Simulate Workday Webhook
`curl -X POST http://localhost:5000/api/workday/webhook -H "Content-Type: application/json" -d '{"externalId":"WD123","displayName":"Alice Example","type":"Employee","department":"R&D"}'`

Expect: Profile created, audit log entry, empty permissions.

## 6. Assign Zone & Credential
1. POST /api/zones { name }
2. POST /api/schedules { name, timeRules }
3. POST /api/profiles/{id}/credentials { identifier }
4. POST /api/profiles/{id}/zone-permissions { zoneId, scheduleId }

## 7. Simulate Access Attempt (temporary internal test endpoint or service call)
Expect: ALLOW + real-time event in UI (SignalR hub subscription)

## 8. Revoke Credential
POST /api/credentials/{credId}/revoke { reason: "LOST_CARD" }
Expect: subsequent attempt DENY with reason.

## 9. Export Report
GET /api/reports/zone-access?zoneId=...&from=...&to=...
Expect: CSV download w/ header.

## 10. Cleanup
Stop processes; optionally clear DB.

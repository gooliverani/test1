# Research Report: Physical Security Platform (Phase 0)

Format: Decision / Rationale / Alternatives

## 1. Multiple Credentials Per Profile
- Decision: Allow multiple active credentials per profile (e.g., primary card + temporary visitor escort badge)
- Rationale: Operational flexibility; avoids forced revocation delays
- Alternatives: Single credential (simpler, but blocks parallel issuance)

## 2. Mobile Credential Scope
- Decision: Defer to post-MVP; design extensible Credential.Type enum
- Rationale: Reduce integration complexity early
- Alternatives: Implement now (adds mobile wallet provisioning overhead)

## 3. Card Printing Standard
- Decision: Custom layout JSON config (fields: Name, Dept, PhotoRef, CredentialId, Expiry)
- Rationale: Flexibility across printer vendors
- Alternatives: Fixed template (faster, less flexible)

## 4. Audit Retention
- Decision: 13 months online + archival export process
- Rationale: Covers annual audit cycles
- Alternatives: 6 months (lighter storage) / 24 months (higher cost)

## 5. Export Formats
- Decision: CSV (MVP), JSON (secondary), PDF deferred
- Rationale: CSV satisfies compliance + ingestion pipelines
- Alternatives: Include PDF now (adds rendering lib complexity)

## 6. Visitor Workflow
- Decision: Require HostProfileId; auto-expire at 23:59 local same day unless specified
- Rationale: Enforces minimal lingering access
- Alternatives: Manual expiration (risk of leftovers)

## 7. Default Access Schedule
- Decision: No implicit access; schedule required
- Rationale: Principle of least privilege
- Alternatives: Implicit 24/7 (simpler but risky)

## 8. Bulk Import Format
- Decision: CSV with strict headers; row-level validation; produce error report file
- Rationale: Transparent onboarding & partial success allowed
- Alternatives: All-or-nothing transaction (blocks large imports for minor errors)

## 9. Metrics Set
- Decision: Denials by reason (hourly agg), Active vs Revoked credentials, Avg provisioning time
- Rationale: Security posture + operational efficiency
- Alternatives: Add dwell time (requires entry/exit pairing not yet modeled)

## 10. Reason Code Governance
- Decision: Managed by SecurityAdmin role via controlled list (ReasonCode table)
- Rationale: Consistency in analytics
- Alternatives: Free text (inconsistent reporting)

## 11. Deletion Policy
- Decision: Soft delete only (Status=Deactivated) + retention
- Rationale: Forensic traceability
- Alternatives: Hard delete (regulatory risk)

## 12. Workday Webhook Idempotency
- Decision: Use ExternalId (Workday employee ID) unique index; ignore duplicates
- Rationale: Safe retries
- Alternatives: Hash full payload (heavier, more fragile)

## 13. Real-time Event Latency Target
- Decision: <2s end-to-end (reader ingest to UI broadcast)
- Rationale: Near-live situational awareness
- Alternatives: Polling (higher latency, less responsive)

## 14. Access Attempt Persistence Strategy
- Decision: Append-only table; future partitioning by month
- Rationale: Simplifies MVP while enabling scale path
- Alternatives: Immediate partitioning (premature complexity)

## 15. Open Items Requiring Stakeholder Confirmation
- Card physical layout compliance (ISO proximity encoding?)
- Potential regulatory retention overrides (SOX, PCI?)
- Volume projections for events per day (affects indexing & partition timeline)

## Summary
All critical MVP ambiguities resolved with explicit deferrals. Remaining confirmations will not block initial domain modeling.

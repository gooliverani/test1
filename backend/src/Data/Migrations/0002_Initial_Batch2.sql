-- Placeholder migration script (manual) for entities: AccessAttempt, AuditLog, AccessTemplate, VisitorBadge, ReasonCode
-- Generated manually due to absent EF Core CLI tooling in environment.

CREATE TABLE access_attempts (
    id bigserial PRIMARY KEY,
    credential_id uuid NOT NULL REFERENCES credentials(id) ON DELETE CASCADE,
    zone_id uuid NOT NULL REFERENCES zones(id) ON DELETE CASCADE,
    outcome int NOT NULL,
    reason_code text,
    timestamp timestamptz NOT NULL
);
CREATE INDEX ix_access_attempts_timestamp ON access_attempts(timestamp);

CREATE TABLE audit_logs (
    id bigserial PRIMARY KEY,
    actor_id uuid,
    entity_type text NOT NULL,
    entity_id uuid NOT NULL,
    action_type text NOT NULL,
    data jsonb NOT NULL,
    created_at timestamptz NOT NULL
);
CREATE INDEX ix_audit_logs_entity ON audit_logs(entity_type, entity_id);
CREATE INDEX ix_audit_logs_created_at ON audit_logs(created_at);

CREATE TABLE access_templates (
    id uuid PRIMARY KEY,
    name text NOT NULL UNIQUE,
    version int NOT NULL,
    template_data jsonb NOT NULL
);

CREATE TABLE visitor_badges (
    id uuid PRIMARY KEY,
    person_profile_id uuid NOT NULL REFERENCES person_profiles(id) ON DELETE CASCADE,
    host_profile_id uuid NOT NULL REFERENCES person_profiles(id) ON DELETE RESTRICT,
    expires_at timestamptz NOT NULL,
    status int NOT NULL
);

CREATE TABLE reason_codes (
    code text PRIMARY KEY,
    category text NOT NULL,
    description text NOT NULL,
    active boolean NOT NULL
);

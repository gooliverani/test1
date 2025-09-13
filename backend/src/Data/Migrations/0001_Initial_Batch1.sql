-- Placeholder migration script (manual) for entities: PersonProfile, Credential, Zone, Schedule, ZonePermission
-- Generated manually due to absent EF Core CLI tooling in environment.

CREATE TABLE person_profiles (
    id uuid PRIMARY KEY,
    external_id text UNIQUE,
    type int NOT NULL,
    status int NOT NULL,
    display_name text NOT NULL,
    email text UNIQUE,
    department text,
    created_at timestamptz NOT NULL DEFAULT (now() at time zone 'utc'),
    modified_at timestamptz
);

CREATE TABLE credentials (
    id uuid PRIMARY KEY,
    person_profile_id uuid NOT NULL REFERENCES person_profiles(id) ON DELETE CASCADE,
    identifier text NOT NULL,
    type int NOT NULL,
    status int NOT NULL,
    issued_at timestamptz NOT NULL,
    expires_at timestamptz,
    revoked_at timestamptz,
    revocation_reason text
);

CREATE INDEX ix_credentials_identifier ON credentials(identifier);
-- Partial unique constraint (active only) to enforce unique active Identifier
-- CREATE UNIQUE INDEX ux_credentials_identifier_active ON credentials(identifier) WHERE status = 0; -- assuming 0 = Active

CREATE TABLE zones (
    id uuid PRIMARY KEY,
    name text NOT NULL UNIQUE,
    description text
);

CREATE TABLE schedules (
    id uuid PRIMARY KEY,
    name text NOT NULL UNIQUE,
    time_rules jsonb NOT NULL,
    timezone text NOT NULL
);

CREATE TABLE zone_permissions (
    id uuid PRIMARY KEY,
    person_profile_id uuid NOT NULL REFERENCES person_profiles(id) ON DELETE CASCADE,
    zone_id uuid NOT NULL REFERENCES zones(id) ON DELETE CASCADE,
    schedule_id uuid NOT NULL REFERENCES schedules(id) ON DELETE CASCADE,
    granted_at timestamptz NOT NULL,
    revoked_at timestamptz
);

-- Composite uniqueness for active (not revoked) permissions could be enforced via partial index
-- CREATE UNIQUE INDEX ux_zone_permissions_active ON zone_permissions(person_profile_id, zone_id) WHERE revoked_at IS NULL;

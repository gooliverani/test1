-- ============================================================================
-- Physical Security & Door Access Control Platform - Full Database Schema
-- Generated: 2025-09-14
-- Notes:
--   * This is a canonical snapshot combining existing schema.sql plus additional
--     domain entities present in the C# model layer but not fully represented.
--   * Uses PostgreSQL dialect.
--   * Apply in a clean database. For iterative evolution, manage with migrations.
-- ============================================================================

BEGIN;

-- --------------------------------------------------------------------------
-- EXTENSIONS (optional, uncomment if needed)
-- --------------------------------------------------------------------------
-- CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
-- CREATE EXTENSION IF NOT EXISTS pgcrypto;

-- --------------------------------------------------------------------------
-- ENUM TYPES (map from C# enums)
-- --------------------------------------------------------------------------
DO $$ BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'credential_type') THEN
        CREATE TYPE credential_type AS ENUM ('Card','Mobile','TempVisitor');
    END IF;
    IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'credential_status') THEN
        CREATE TYPE credential_status AS ENUM ('Active','Revoked','Expired');
    END IF;
    IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'profile_type') THEN
        CREATE TYPE profile_type AS ENUM ('Employee','Contractor','Visitor');
    END IF;
    IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'profile_status') THEN
        CREATE TYPE profile_status AS ENUM ('Active','Suspended','Deactivated');
    END IF;
    IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'visitor_badge_status') THEN
        CREATE TYPE visitor_badge_status AS ENUM ('Active','Expired','Revoked');
    END IF;
    IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'access_outcome') THEN
        CREATE TYPE access_outcome AS ENUM ('Allow','Deny');
    END IF;
END $$;

-- --------------------------------------------------------------------------
-- CORE ORGANIZATIONAL STRUCTURE
-- --------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS departments (
    id              SERIAL PRIMARY KEY,
    name            TEXT NOT NULL UNIQUE,
    description     TEXT,
    created_at      TIMESTAMPTZ DEFAULT NOW(),
    updated_at      TIMESTAMPTZ DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS teams (
    id              SERIAL PRIMARY KEY,
    department_id   INTEGER REFERENCES departments(id) ON DELETE CASCADE,
    name            TEXT NOT NULL,
    description     TEXT,
    created_at      TIMESTAMPTZ DEFAULT NOW(),
    updated_at      TIMESTAMPTZ DEFAULT NOW(),
    UNIQUE(department_id, name)
);

CREATE TABLE IF NOT EXISTS locations (
    id                  SERIAL PRIMARY KEY,
    name                TEXT NOT NULL,
    description         TEXT,
    address             TEXT,
    parent_location_id  INTEGER REFERENCES locations(id) ON DELETE CASCADE,
    created_at          TIMESTAMPTZ DEFAULT NOW(),
    updated_at          TIMESTAMPTZ DEFAULT NOW(),
    UNIQUE(parent_location_id, name)
);

-- --------------------------------------------------------------------------
-- EMPLOYEE PROFILE DOMAIN (legacy style + enriched model fields)
-- --------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS employee_profiles (
    id              SERIAL PRIMARY KEY,
    department_id   INTEGER REFERENCES departments(id) ON DELETE SET NULL,
    team_id         INTEGER REFERENCES teams(id) ON DELETE SET NULL,
    location_id     INTEGER REFERENCES locations(id) ON DELETE SET NULL,
    external_id     TEXT UNIQUE,
    comp_id         TEXT NOT NULL UNIQUE,
    first_name      TEXT NOT NULL,
    last_name       TEXT NOT NULL,
    email           TEXT NOT NULL,
    hire_date       DATE NOT NULL DEFAULT CURRENT_DATE,
    expire_date     DATE,
    is_active       BOOLEAN NOT NULL DEFAULT TRUE,
    badge_serial    TEXT,
    badge_printed   BOOLEAN NOT NULL DEFAULT FALSE,
    badge_printed_at TIMESTAMPTZ,
    metadata        JSONB NOT NULL DEFAULT '{}',
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX IF NOT EXISTS idx_employee_profiles_department_id ON employee_profiles(department_id);
CREATE INDEX IF NOT EXISTS idx_employee_profiles_team_id ON employee_profiles(team_id);
CREATE INDEX IF NOT EXISTS idx_employee_profiles_location_id ON employee_profiles(location_id);
CREATE INDEX IF NOT EXISTS idx_employee_profiles_expire_date ON employee_profiles(expire_date);

CREATE TABLE IF NOT EXISTS employee_profile_history (
    id              SERIAL PRIMARY KEY,
    employee_id     INTEGER NOT NULL REFERENCES employee_profiles(id) ON DELETE CASCADE,
    changed_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    changed_by      TEXT,
    change_type     TEXT NOT NULL,
    change_details  JSONB
);
CREATE INDEX IF NOT EXISTS idx_employee_profile_history_employee_id ON employee_profile_history(employee_id);

-- Photos for employees
CREATE TABLE IF NOT EXISTS employee_photos (
    id              SERIAL PRIMARY KEY,
    employee_id     INTEGER NOT NULL REFERENCES employee_profiles(id) ON DELETE CASCADE,
    photo_data      BYTEA,
    photo_url       TEXT,
    photo_filename  TEXT,
    photo_size      INTEGER,
    mime_type       TEXT,
    uploaded_at     TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    uploaded_by     TEXT,
    is_active       BOOLEAN NOT NULL DEFAULT TRUE,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX IF NOT EXISTS idx_employee_photos_employee_id ON employee_photos(employee_id);

-- Badge lifecycle history
CREATE TABLE IF NOT EXISTS badge_history (
    id                  SERIAL PRIMARY KEY,
    employee_id         INTEGER NOT NULL REFERENCES employee_profiles(id) ON DELETE CASCADE,
    badge_serial        TEXT NOT NULL,
    action_type         TEXT NOT NULL,
    previous_badge_serial TEXT,
    issued_date         TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    expiry_date         DATE,
    issued_by           TEXT,
    reason              TEXT,
    is_active           BOOLEAN NOT NULL DEFAULT TRUE,
    created_at          TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX IF NOT EXISTS idx_badge_history_employee_id ON badge_history(employee_id);

-- Access history (profile assignments changes)
CREATE TABLE IF NOT EXISTS access_history (
    id                      SERIAL PRIMARY KEY,
    employee_id             INTEGER NOT NULL REFERENCES employee_profiles(id) ON DELETE CASCADE,
    access_profile_id       INTEGER NOT NULL,
    action_type             TEXT NOT NULL,
    effective_date          TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    expiry_date             TIMESTAMPTZ,
    granted_by              TEXT,
    reason                  TEXT,
    previous_access_profile_id INTEGER,
    created_at              TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX IF NOT EXISTS idx_access_history_employee_id ON access_history(employee_id);
CREATE INDEX IF NOT EXISTS idx_access_history_profile_id ON access_history(access_profile_id);

-- --------------------------------------------------------------------------
-- ACCESS PROFILES & ASSIGNMENTS
-- --------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS access_profiles (
    id              SERIAL PRIMARY KEY,
    name            TEXT NOT NULL UNIQUE,
    description     TEXT,
    is_active       BOOLEAN NOT NULL DEFAULT TRUE,
    time_zone_id    INTEGER,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS employee_access (
    id              SERIAL PRIMARY KEY,
    employee_id     INTEGER NOT NULL REFERENCES employee_profiles(id) ON DELETE CASCADE,
    access_profile_id INTEGER NOT NULL REFERENCES access_profiles(id) ON DELETE CASCADE,
    assigned_via    TEXT NOT NULL DEFAULT 'manual',
    assigned_date   DATE NOT NULL DEFAULT CURRENT_DATE,
    expire_date     DATE,
    active          BOOLEAN NOT NULL DEFAULT TRUE,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT uniq_employee_access UNIQUE(employee_id, access_profile_id, assigned_via)
);
CREATE INDEX IF NOT EXISTS idx_employee_access_employee_id ON employee_access(employee_id);
CREATE INDEX IF NOT EXISTS idx_employee_access_profile_id ON employee_access(access_profile_id);

-- Readers & mapping
CREATE TABLE IF NOT EXISTS readers (
    id              SERIAL PRIMARY KEY,
    location_id     INTEGER NOT NULL REFERENCES locations(id) ON DELETE CASCADE,
    name            TEXT NOT NULL,
    device_type     TEXT,
    capabilities    JSONB DEFAULT '{}',
    channel_id      INTEGER,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    UNIQUE(location_id, name)
);
CREATE INDEX IF NOT EXISTS idx_readers_location_id ON readers(location_id);

CREATE TABLE IF NOT EXISTS access_profile_readers (
    access_profile_id   INTEGER NOT NULL REFERENCES access_profiles(id) ON DELETE CASCADE,
    reader_id           INTEGER NOT NULL REFERENCES readers(id) ON DELETE CASCADE,
    PRIMARY KEY (access_profile_id, reader_id)
);
CREATE INDEX IF NOT EXISTS idx_access_profile_readers_profile_id ON access_profile_readers(access_profile_id);
CREATE INDEX IF NOT EXISTS idx_access_profile_readers_reader_id ON access_profile_readers(reader_id);

-- Department/location based rule suggestions
CREATE TABLE IF NOT EXISTS access_rules (
    department_id   INTEGER NOT NULL REFERENCES departments(id) ON DELETE CASCADE,
    location_id     INTEGER NOT NULL REFERENCES locations(id) ON DELETE CASCADE,
    access_profile_id INTEGER NOT NULL REFERENCES access_profiles(id) ON DELETE CASCADE,
    PRIMARY KEY (department_id, location_id, access_profile_id)
);
CREATE INDEX IF NOT EXISTS idx_access_rules_dept_loc ON access_rules(department_id, location_id);

-- --------------------------------------------------------------------------
-- REAL-TIME SWIPES / EVENTS
-- --------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS swipes (
    id              BIGSERIAL PRIMARY KEY,
    employee_id     INTEGER REFERENCES employee_profiles(id) ON DELETE CASCADE,
    reader_id       INTEGER REFERENCES readers(id) ON DELETE CASCADE,
    card_number     TEXT,
    access_granted  BOOLEAN NOT NULL DEFAULT FALSE,
    denial_reason   TEXT,
    swipe_time      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX IF NOT EXISTS idx_swipes_employee_id ON swipes(employee_id);
CREATE INDEX IF NOT EXISTS idx_swipes_reader_id ON swipes(reader_id);
CREATE INDEX IF NOT EXISTS idx_swipes_swipe_time ON swipes(swipe_time);

-- --------------------------------------------------------------------------
-- GENERIC PERSON PROFILE + CREDENTIALS (Unified abstraction)
-- --------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS person_profiles (
    id              UUID PRIMARY KEY,
    external_id     TEXT UNIQUE,
    type            profile_type NOT NULL,
    status          profile_status NOT NULL,
    display_name    TEXT NOT NULL,
    email           TEXT,
    department      TEXT,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    modified_at     TIMESTAMPTZ
);

CREATE TABLE IF NOT EXISTS credentials (
    id              UUID PRIMARY KEY,
    person_profile_id UUID NOT NULL REFERENCES person_profiles(id) ON DELETE CASCADE,
    identifier      TEXT NOT NULL,
    type            credential_type NOT NULL,
    status          credential_status NOT NULL,
    issued_at       TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    expires_at      TIMESTAMPTZ,
    revoked_at      TIMESTAMPTZ,
    revocation_reason TEXT
);
CREATE INDEX IF NOT EXISTS idx_credentials_person ON credentials(person_profile_id);

-- --------------------------------------------------------------------------
-- ZONES / SCHEDULES / PERMISSIONS
-- --------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS schedules (
    id          UUID PRIMARY KEY,
    name        TEXT NOT NULL,
    time_rules  JSONB NOT NULL DEFAULT '[]',
    timezone    TEXT NOT NULL DEFAULT 'UTC'
);

CREATE TABLE IF NOT EXISTS zones (
    id          UUID PRIMARY KEY,
    name        TEXT NOT NULL,
    description TEXT
);

CREATE TABLE IF NOT EXISTS zone_permissions (
    id              UUID PRIMARY KEY,
    person_profile_id UUID NOT NULL REFERENCES person_profiles(id) ON DELETE CASCADE,
    zone_id         UUID NOT NULL REFERENCES zones(id) ON DELETE CASCADE,
    schedule_id     UUID NOT NULL REFERENCES schedules(id) ON DELETE CASCADE,
    granted_at      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    revoked_at      TIMESTAMPTZ
);
CREATE INDEX IF NOT EXISTS idx_zone_permissions_person ON zone_permissions(person_profile_id);
CREATE INDEX IF NOT EXISTS idx_zone_permissions_zone ON zone_permissions(zone_id);

-- --------------------------------------------------------------------------
-- VISITOR BADGES & ACCESS ATTEMPTS
-- --------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS visitor_badges (
    id              UUID PRIMARY KEY,
    person_profile_id UUID NOT NULL REFERENCES person_profiles(id) ON DELETE CASCADE,
    host_profile_id UUID NOT NULL REFERENCES person_profiles(id) ON DELETE CASCADE,
    expires_at      TIMESTAMPTZ NOT NULL,
    status          visitor_badge_status NOT NULL
);
CREATE INDEX IF NOT EXISTS idx_visitor_badges_person ON visitor_badges(person_profile_id);

CREATE TABLE IF NOT EXISTS access_attempts (
    id              BIGSERIAL PRIMARY KEY,
    credential_id   UUID NOT NULL REFERENCES credentials(id) ON DELETE CASCADE,
    zone_id         UUID NOT NULL REFERENCES zones(id) ON DELETE CASCADE,
    outcome         access_outcome NOT NULL,
    reason_code     TEXT,
    timestamp       TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX IF NOT EXISTS idx_access_attempts_credential ON access_attempts(credential_id);
CREATE INDEX IF NOT EXISTS idx_access_attempts_zone ON access_attempts(zone_id);
CREATE INDEX IF NOT EXISTS idx_access_attempts_time ON access_attempts(timestamp);

-- --------------------------------------------------------------------------
-- REASON CODES
-- --------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS reason_codes (
    code        TEXT PRIMARY KEY,
    category    TEXT NOT NULL,
    description TEXT NOT NULL,
    active      BOOLEAN NOT NULL DEFAULT TRUE
);

-- --------------------------------------------------------------------------
-- ACCESS TEMPLATES (for bundling zone+schedule policies)
-- --------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS access_templates (
    id          UUID PRIMARY KEY,
    name        TEXT NOT NULL,
    version     INTEGER NOT NULL DEFAULT 1,
    template_data JSONB NOT NULL DEFAULT '[]'
);

-- --------------------------------------------------------------------------
-- AUDIT LOG
-- --------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS audit_log (
    id          BIGSERIAL PRIMARY KEY,
    actor_id    UUID,
    entity_type TEXT NOT NULL,
    entity_id   UUID NOT NULL,
    action_type TEXT NOT NULL,
    data        JSONB NOT NULL DEFAULT '{}',
    created_at  TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX IF NOT EXISTS idx_audit_log_entity ON audit_log(entity_type, entity_id);
CREATE INDEX IF NOT EXISTS idx_audit_log_created_at ON audit_log(created_at);

-- --------------------------------------------------------------------------
-- SECURITY USERS & ROLES (Application accounts)
-- --------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS app_users (
    id              SERIAL PRIMARY KEY,
    username        TEXT NOT NULL UNIQUE,
    email           TEXT,
    password_hash   TEXT,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS app_roles (
    id          SERIAL PRIMARY KEY,
    name        TEXT NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS user_roles (
    user_id     INTEGER NOT NULL REFERENCES app_users(id) ON DELETE CASCADE,
    role_id     INTEGER NOT NULL REFERENCES app_roles(id) ON DELETE CASCADE,
    PRIMARY KEY (user_id, role_id)
);
CREATE INDEX IF NOT EXISTS idx_user_roles_user ON user_roles(user_id);

-- --------------------------------------------------------------------------
-- HARDWARE / PHYSICAL LAYER (Controllers, Panels, etc.)
-- --------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS controllers (
    id              SERIAL PRIMARY KEY,
    location_id     INTEGER NOT NULL REFERENCES locations(id) ON DELETE CASCADE,
    name            TEXT NOT NULL,
    model           TEXT,
    firmware_version TEXT,
    ip_address      INET,
    status          TEXT NOT NULL DEFAULT 'active' CHECK (status IN ('active','inactive','maintenance')),
    config          JSONB NOT NULL DEFAULT '{}',
    installed_at    TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    UNIQUE(location_id, name)
);
CREATE INDEX IF NOT EXISTS idx_controllers_location_id ON controllers(location_id);

CREATE TABLE IF NOT EXISTS panels (
    id              SERIAL PRIMARY KEY,
    controller_id   INTEGER NOT NULL REFERENCES controllers(id) ON DELETE CASCADE,
    name            TEXT NOT NULL,
    type            TEXT,
    config          JSONB NOT NULL DEFAULT '{}',
    status          TEXT NOT NULL DEFAULT 'active' CHECK (status IN ('active','inactive','fault')),
    UNIQUE(controller_id, name)
);
CREATE INDEX IF NOT EXISTS idx_panels_controller_id ON panels(controller_id);

CREATE TABLE IF NOT EXISTS channels (
    id              SERIAL PRIMARY KEY,
    panel_id        INTEGER NOT NULL REFERENCES panels(id) ON DELETE CASCADE,
    name            TEXT NOT NULL,
    type            TEXT,
    purpose         TEXT,
    config          JSONB NOT NULL DEFAULT '{}',
    status          TEXT NOT NULL DEFAULT 'active' CHECK (status IN ('active','inactive','fault')),
    UNIQUE(panel_id, name)
);
CREATE INDEX IF NOT EXISTS idx_channels_panel_id ON channels(panel_id);

CREATE TABLE IF NOT EXISTS devices (
    id              SERIAL PRIMARY KEY,
    channel_id      INTEGER REFERENCES channels(id) ON DELETE SET NULL,
    panel_id        INTEGER REFERENCES panels(id) ON DELETE SET NULL,
    name            TEXT NOT NULL,
    type            TEXT NOT NULL,
    model           TEXT,
    config          JSONB NOT NULL DEFAULT '{}',
    status          TEXT NOT NULL DEFAULT 'active' CHECK (status IN ('active','inactive','fault')),
    installed_at    TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CHECK(channel_id IS NOT NULL OR panel_id IS NOT NULL)
);
CREATE INDEX IF NOT EXISTS idx_devices_channel_id ON devices(channel_id);
CREATE INDEX IF NOT EXISTS idx_devices_panel_id ON devices(panel_id);

-- --------------------------------------------------------------------------
-- TIME ZONES (used by access_profiles optional association)
-- --------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS time_zones (
    id          SERIAL PRIMARY KEY,
    name        TEXT NOT NULL UNIQUE,
    description TEXT,
    schedule    JSONB NOT NULL DEFAULT '{}'
);

ALTER TABLE access_profiles
    ADD CONSTRAINT fk_access_profiles_time_zone
        FOREIGN KEY (time_zone_id) REFERENCES time_zones(id) ON DELETE SET NULL;

-- --------------------------------------------------------------------------
-- INDEX OPTIMIZATIONS (additional partial/sample indexes)
-- --------------------------------------------------------------------------
CREATE INDEX IF NOT EXISTS idx_employee_profiles_active ON employee_profiles(is_active) WHERE is_active = TRUE;
CREATE INDEX IF NOT EXISTS idx_employee_profiles_badge_serial ON employee_profiles(badge_serial);
CREATE INDEX IF NOT EXISTS idx_credentials_identifier ON credentials(identifier);
CREATE INDEX IF NOT EXISTS idx_swipes_granted ON swipes(access_granted) WHERE access_granted = TRUE;

-- --------------------------------------------------------------------------
-- VIEWS (examples – extend as needed)
-- --------------------------------------------------------------------------
CREATE OR REPLACE VIEW vw_active_employees AS
SELECT id, comp_id, first_name, last_name, email, department_id, location_id
FROM employee_profiles
WHERE is_active = TRUE;

CREATE OR REPLACE VIEW vw_employee_access_profiles AS
SELECT ea.employee_id, ap.name AS access_profile_name, ea.active, ea.assigned_date
FROM employee_access ea
JOIN access_profiles ap ON ap.id = ea.access_profile_id;

-- --------------------------------------------------------------------------
-- SEED DATA (optional templates) - Commented out
-- --------------------------------------------------------------------------
-- INSERT INTO departments(name, description) VALUES
--   ('Engineering','Software and infrastructure'),
--   ('Security','Corporate physical security team');
-- 
-- INSERT INTO locations(name, description) VALUES
--   ('HQ','Headquarters'),
--   ('DC1','Primary Data Center');
-- 
-- INSERT INTO access_profiles(name, description) VALUES
--   ('Standard Access','Default building access'),
--   ('Data Center','Restricted DC access');

-- --------------------------------------------------------------------------
-- TRIGGERS / AUTOMATION (examples placeholder)
-- --------------------------------------------------------------------------
-- CREATE OR REPLACE FUNCTION set_updated_at() RETURNS TRIGGER AS $$
-- BEGIN
--     NEW.updated_at = NOW();
--     RETURN NEW;
-- END; $$ LANGUAGE plpgsql;
-- 
-- CREATE TRIGGER trg_employee_profiles_updated
--     BEFORE UPDATE ON employee_profiles
--     FOR EACH ROW
--     EXECUTE FUNCTION set_updated_at();

COMMIT;

-- ============================================================================
-- END OF SCHEMA
-- ============================================================================

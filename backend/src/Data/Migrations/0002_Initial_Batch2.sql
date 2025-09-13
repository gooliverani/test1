-- =========================
-- DROP VIEWS (if they exist)
-- =========================
DROP VIEW IF EXISTS vw_employee_access_report CASCADE;
DROP VIEW IF EXISTS vw_visitor_history_report CASCADE;
DROP VIEW IF EXISTS vw_employees_with_expired_badges CASCADE;
DROP VIEW IF EXISTS vw_readers_with_profiles CASCADE;
DROP VIEW IF EXISTS vw_swipe_summary_by_reader CASCADE;
DROP VIEW IF EXISTS vw_employees_without_recent_swipes CASCADE;
DROP VIEW IF EXISTS vw_profiles_with_reader_count CASCADE;
DROP VIEW IF EXISTS vw_departments_with_active_employees CASCADE;

-- =========================
-- DROP TABLES (if they exist)
-- =========================
DROP TABLE IF EXISTS user_roles CASCADE;
DROP TABLE IF EXISTS app_roles CASCADE;
DROP TABLE IF EXISTS app_users CASCADE;
DROP TABLE IF EXISTS audit_log CASCADE;
DROP TABLE IF EXISTS swipes CASCADE;
DROP TABLE IF EXISTS visitor_access CASCADE;
DROP TABLE IF EXISTS visitors CASCADE;
DROP TABLE IF EXISTS employee_access CASCADE;
DROP TABLE IF EXISTS employee_profile_history CASCADE;
DROP TABLE IF EXISTS employee_profiles CASCADE;
DROP TABLE IF EXISTS access_rules CASCADE;
DROP TABLE IF EXISTS access_profile_readers CASCADE;
DROP TABLE IF EXISTS access_profiles CASCADE;
DROP TABLE IF EXISTS readers CASCADE;
DROP TABLE IF EXISTS locations CASCADE;
DROP TABLE IF EXISTS teams CASCADE;
DROP TABLE IF EXISTS departments CASCADE;
-- New drops for hardware tables
DROP TABLE IF EXISTS devices CASCADE;
DROP TABLE IF EXISTS channels CASCADE;
DROP TABLE IF EXISTS panels CASCADE;
DROP TABLE IF EXISTS controllers CASCADE;
DROP TABLE IF EXISTS time_zones CASCADE;

-- =========================
-- CREATE TABLES (no FKs yet)
-- =========================
CREATE TABLE departments (
    id SERIAL PRIMARY KEY,
    name TEXT NOT NULL UNIQUE
);

CREATE TABLE teams (
    id SERIAL PRIMARY KEY,
    name TEXT NOT NULL UNIQUE
);

CREATE TABLE locations (
    id SERIAL PRIMARY KEY,
    name TEXT NOT NULL,
    parent_location_id INTEGER
);

CREATE TABLE readers (
    id SERIAL PRIMARY KEY,
    location_id INTEGER NOT NULL,
    name TEXT NOT NULL,
    device_type TEXT,
    capabilities JSONB DEFAULT '{}',
    UNIQUE (location_id, name)
);

CREATE TABLE access_profiles (
    id SERIAL PRIMARY KEY,
    name TEXT NOT NULL UNIQUE,
    description TEXT
);

CREATE TABLE access_profile_readers (
    access_profile_id INTEGER NOT NULL,
    reader_id INTEGER NOT NULL,
    PRIMARY KEY (access_profile_id, reader_id)
);

CREATE TABLE access_rules (
    department_id INTEGER NOT NULL,
    location_id INTEGER NOT NULL,
    access_profile_id INTEGER NOT NULL,
    PRIMARY KEY (department_id, location_id, access_profile_id)
);

CREATE TABLE employee_profiles (
    id SERIAL PRIMARY KEY,
    department_id INTEGER,
    team_id INTEGER,
    location_id INTEGER,
    external_id TEXT UNIQUE,
    comp_id TEXT NOT NULL UNIQUE,
    first_name TEXT NOT NULL,
    last_name TEXT NOT NULL,
    email TEXT NOT NULL,
    employment_type TEXT NOT NULL CHECK (employment_type IN ('Contractor', 'Employee')),
    issue_date DATE NOT NULL DEFAULT CURRENT_DATE,
    expire_date DATE,
    badge_printed BOOLEAN DEFAULT FALSE,
    badge_printed_at TIMESTAMP,
    badge_serial TEXT,
    metadata JSONB DEFAULT '{}'
);

CREATE TABLE employee_profile_history (
    id SERIAL PRIMARY KEY,
    employee_id INTEGER NOT NULL,
    changed_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    changed_by TEXT,
    change_type TEXT NOT NULL,
    change_details JSONB
);

CREATE TABLE employee_access (
    id SERIAL PRIMARY KEY,
    employee_id INTEGER NOT NULL,
    access_profile_id INTEGER NOT NULL,
    assigned_via TEXT NOT NULL DEFAULT 'manual',
    assigned_date DATE DEFAULT CURRENT_DATE,
    expire_date DATE,
    active BOOLEAN DEFAULT TRUE,
    CONSTRAINT uniq_employee_access UNIQUE (employee_id, access_profile_id, assigned_via)
);

CREATE TABLE visitors (
    id SERIAL PRIMARY KEY,
    first_name TEXT NOT NULL,
    last_name TEXT NOT NULL,
    company TEXT,
    email TEXT,
    phone TEXT,
    host_employee_id INTEGER,
    visit_start TIMESTAMP NOT NULL,
    visit_end TIMESTAMP,
    checked_in_at TIMESTAMP,
    checked_out_at TIMESTAMP,
    badge_serial TEXT,
    visitor_type TEXT DEFAULT 'guest'
);

CREATE TABLE visitor_access (
    visitor_id INTEGER NOT NULL,
    access_profile_id INTEGER NOT NULL,
    PRIMARY KEY (visitor_id, access_profile_id)
);

CREATE TABLE swipes (
    id BIGSERIAL PRIMARY KEY,
    employee_id INTEGER,
    visitor_id INTEGER,
    reader_id INTEGER,
    swipe_time TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    access_granted BOOLEAN NOT NULL DEFAULT FALSE,
    reason TEXT,
    metadata JSONB DEFAULT '{}'
);

CREATE TABLE app_users (
    id SERIAL PRIMARY KEY,
    username TEXT UNIQUE NOT NULL,
    email TEXT,
    password_hash TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE app_roles (
    id SERIAL PRIMARY KEY,
    name TEXT UNIQUE NOT NULL
);

CREATE TABLE user_roles (
    user_id INTEGER NOT NULL,
    role_id INTEGER NOT NULL,
    PRIMARY KEY (user_id, role_id)
);

CREATE TABLE audit_log (
    id BIGSERIAL PRIMARY KEY,
    event_time TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    user_id INTEGER,
    action TEXT,
    object_type TEXT,
    object_id TEXT,
    details JSONB
);

-- New tables for hardware configuration
CREATE TABLE controllers (
    id SERIAL PRIMARY KEY,
    location_id INTEGER NOT NULL,
    name TEXT NOT NULL,
    model TEXT,
    firmware_version TEXT,
    ip_address INET,
    status TEXT DEFAULT 'active' CHECK (status IN ('active', 'inactive', 'maintenance')),
    config JSONB DEFAULT '{}',
    installed_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE (location_id, name)
);

CREATE TABLE panels (
    id SERIAL PRIMARY KEY,
    controller_id INTEGER NOT NULL,
    name TEXT NOT NULL,
    type TEXT,
    config JSONB DEFAULT '{}',
    status TEXT DEFAULT 'active' CHECK (status IN ('active', 'inactive', 'fault')),
    UNIQUE (controller_id, name)
);

CREATE TABLE channels (
    id SERIAL PRIMARY KEY,
    panel_id INTEGER NOT NULL,
    name TEXT NOT NULL,
    type TEXT,
    purpose TEXT,
    config JSONB DEFAULT '{}',
    status TEXT DEFAULT 'active' CHECK (status IN ('active', 'inactive', 'fault')),
    UNIQUE (panel_id, name)
);

CREATE TABLE devices (
    id SERIAL PRIMARY KEY,
    channel_id INTEGER,
    panel_id INTEGER,
    name TEXT NOT NULL,
    type TEXT NOT NULL,
    model TEXT,
    config JSONB DEFAULT '{}',
    status TEXT DEFAULT 'active' CHECK (status IN ('active', 'inactive', 'fault')),
    installed_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CHECK (channel_id IS NOT NULL OR panel_id IS NOT NULL)
);

CREATE TABLE time_zones (
    id SERIAL PRIMARY KEY,
    name TEXT NOT NULL UNIQUE,
    description TEXT,
    schedule JSONB DEFAULT '{}'
);

-- =========================
-- ALTER TABLES (for new columns)
-- =========================
ALTER TABLE access_profiles
    ADD COLUMN time_zone_id INTEGER;

ALTER TABLE readers
    ADD COLUMN channel_id INTEGER;

-- =========================
-- ADD FOREIGN KEYS (after all tables exist)
-- =========================
ALTER TABLE locations
    ADD CONSTRAINT fk_locations_parent_location
        FOREIGN KEY (parent_location_id) REFERENCES locations(id) ON DELETE CASCADE;

ALTER TABLE readers
    ADD CONSTRAINT fk_readers_location
        FOREIGN KEY (location_id) REFERENCES locations(id) ON DELETE CASCADE,
    ADD CONSTRAINT fk_readers_channel
        FOREIGN KEY (channel_id) REFERENCES channels(id) ON DELETE SET NULL;

ALTER TABLE access_profile_readers
    ADD CONSTRAINT fk_access_profile_readers_profile
        FOREIGN KEY (access_profile_id) REFERENCES access_profiles(id) ON DELETE CASCADE,
    ADD CONSTRAINT fk_access_profile_readers_reader
        FOREIGN KEY (reader_id) REFERENCES readers(id) ON DELETE CASCADE;

ALTER TABLE access_rules
    ADD CONSTRAINT fk_access_rules_department
        FOREIGN KEY (department_id) REFERENCES departments(id) ON DELETE CASCADE,
    ADD CONSTRAINT fk_access_rules_location
        FOREIGN KEY (location_id) REFERENCES locations(id) ON DELETE CASCADE,
    ADD CONSTRAINT fk_access_rules_profile
        FOREIGN KEY (access_profile_id) REFERENCES access_profiles(id) ON DELETE CASCADE;

ALTER TABLE employee_profiles
    ADD CONSTRAINT fk_employee_profiles_department
        FOREIGN KEY (department_id) REFERENCES departments(id) ON DELETE SET NULL,
    ADD CONSTRAINT fk_employee_profiles_team
        FOREIGN KEY (team_id) REFERENCES teams(id) ON DELETE SET NULL,
    ADD CONSTRAINT fk_employee_profiles_location
        FOREIGN KEY (location_id) REFERENCES locations(id) ON DELETE SET NULL;

ALTER TABLE employee_profile_history
    ADD CONSTRAINT fk_employee_profile_history_employee
        FOREIGN KEY (employee_id) REFERENCES employee_profiles(id) ON DELETE CASCADE;

ALTER TABLE employee_access
    ADD CONSTRAINT fk_employee_access_employee
        FOREIGN KEY (employee_id) REFERENCES employee_profiles(id) ON DELETE CASCADE,
    ADD CONSTRAINT fk_employee_access_profile
        FOREIGN KEY (access_profile_id) REFERENCES access_profiles(id) ON DELETE CASCADE;

ALTER TABLE visitors
    ADD CONSTRAINT fk_visitors_host_employee
        FOREIGN KEY (host_employee_id) REFERENCES employee_profiles(id) ON DELETE SET NULL;

ALTER TABLE visitor_access
    ADD CONSTRAINT fk_visitor_access_visitor
        FOREIGN KEY (visitor_id) REFERENCES visitors(id) ON DELETE CASCADE,
    ADD CONSTRAINT fk_visitor_access_profile
        FOREIGN KEY (access_profile_id) REFERENCES access_profiles(id) ON DELETE CASCADE;

ALTER TABLE swipes
    ADD CONSTRAINT fk_swipes_employee
        FOREIGN KEY (employee_id) REFERENCES employee_profiles(id) ON DELETE CASCADE,
    ADD CONSTRAINT fk_swipes_visitor
        FOREIGN KEY (visitor_id) REFERENCES visitors(id) ON DELETE CASCADE,
    ADD CONSTRAINT fk_swipes_reader
        FOREIGN KEY (reader_id) REFERENCES readers(id) ON DELETE CASCADE;

ALTER TABLE user_roles
    ADD CONSTRAINT fk_user_roles_user
        FOREIGN KEY (user_id) REFERENCES app_users(id) ON DELETE CASCADE,
    ADD CONSTRAINT fk_user_roles_role
        FOREIGN KEY (role_id) REFERENCES app_roles(id) ON DELETE CASCADE;

ALTER TABLE audit_log
    ADD CONSTRAINT fk_audit_log_user
        FOREIGN KEY (user_id) REFERENCES app_users(id) ON DELETE SET NULL;

-- New FKs for hardware tables
ALTER TABLE controllers
    ADD CONSTRAINT fk_controllers_location
        FOREIGN KEY (location_id) REFERENCES locations(id) ON DELETE CASCADE;

ALTER TABLE panels
    ADD CONSTRAINT fk_panels_controller
        FOREIGN KEY (controller_id) REFERENCES controllers(id) ON DELETE CASCADE;

ALTER TABLE channels
    ADD CONSTRAINT fk_channels_panel
        FOREIGN KEY (panel_id) REFERENCES panels(id) ON DELETE CASCADE;

ALTER TABLE devices
    ADD CONSTRAINT fk_devices_channel
        FOREIGN KEY (channel_id) REFERENCES channels(id) ON DELETE SET NULL,
    ADD CONSTRAINT fk_devices_panel
        FOREIGN KEY (panel_id) REFERENCES panels(id) ON DELETE SET NULL;

ALTER TABLE access_profiles
    ADD CONSTRAINT fk_access_profiles_time_zone
        FOREIGN KEY (time_zone_id) REFERENCES time_zones(id) ON DELETE SET NULL;

-- =========================
-- INDEXES
-- =========================
CREATE INDEX idx_access_rules_dept_loc ON access_rules (department_id, location_id);
CREATE INDEX idx_employee_expiration ON employee_profiles (expire_date);
CREATE INDEX idx_swipes_employee ON swipes (employee_id);
CREATE INDEX idx_swipes_time ON swipes (swipe_time);
CREATE INDEX idx_swipes_reader ON swipes (reader_id);

CREATE INDEX idx_employee_profiles_department_id ON employee_profiles(department_id);
CREATE INDEX idx_employee_profiles_team_id ON employee_profiles(team_id);
CREATE INDEX idx_employee_profiles_location_id ON employee_profiles(location_id);

CREATE INDEX idx_employee_access_employee_id ON employee_access(employee_id);
CREATE INDEX idx_employee_access_access_profile_id ON employee_access(access_profile_id);

CREATE INDEX idx_visitor_access_visitor_id ON visitor_access(visitor_id);
CREATE INDEX idx_visitor_access_access_profile_id ON visitor_access(access_profile_id);

CREATE INDEX idx_readers_location_id ON readers(location_id);

CREATE INDEX idx_access_profile_readers_profile_id ON access_profile_readers(access_profile_id);
CREATE INDEX idx_access_profile_readers_reader_id ON access_profile_readers(reader_id);

CREATE INDEX idx_swipes_visitor_id ON swipes(visitor_id);

CREATE INDEX idx_employee_profile_history_employee_id ON employee_profile_history(employee_id);

CREATE INDEX idx_audit_log_user_id ON audit_log(user_id);

CREATE INDEX idx_user_roles_user_id ON user_roles(user_id);
CREATE INDEX idx_user_roles_role_id ON user_roles(role_id);

CREATE INDEX idx_employee_profiles_external_id ON employee_profiles(external_id);

CREATE INDEX idx_employee_profiles_badge_printed_false ON employee_profiles(badge_printed) WHERE badge_printed = FALSE;

CREATE INDEX idx_employee_profiles_badge_serial ON employee_profiles(badge_serial);
CREATE INDEX idx_visitors_badge_serial ON visitors(badge_serial);

CREATE INDEX idx_visitors_checked_in ON visitors(checked_in_at) WHERE checked_in_at IS NOT NULL AND checked_out_at IS NULL;

-- New indexes for hardware tables
CREATE INDEX idx_controllers_location_id ON controllers(location_id);
CREATE INDEX idx_panels_controller_id ON panels(controller_id);
CREATE INDEX idx_channels_panel_id ON channels(panel_id);
CREATE INDEX idx_devices_channel_id ON devices(channel_id);
CREATE INDEX idx_devices_panel_id ON devices(panel_id);

-- =========================
-- FUNCTIONS & TRIGGERS
-- =========================

-- Validate comp_id format before insert
CREATE OR REPLACE FUNCTION validate_comp_id_fn() RETURNS trigger AS $$
BEGIN
  IF NOT (
    length(NEW.comp_id) = 8 AND
    substring(NEW.comp_id, 1, 1) = upper(left(NEW.first_name, 1)) AND
    substring(NEW.comp_id, 2, 1) = upper(left(NEW.last_name, 1)) AND
    substring(NEW.comp_id, 3, 6) ~ '^\d{6}$'
  ) THEN
    RAISE EXCEPTION 'Invalid comp_id format';
  END IF;
  RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER validate_comp_id
BEFORE INSERT ON employee_profiles
FOR EACH ROW EXECUTE FUNCTION validate_comp_id_fn();

-- Assign access on hire after insert
CREATE OR REPLACE FUNCTION assign_access_on_hire_fn() RETURNS trigger AS $$
BEGIN
  INSERT INTO employee_access (employee_id, access_profile_id, assigned_via)
  SELECT NEW.id, ar.access_profile_id, 'automation'
  FROM access_rules ar
  WHERE ar.department_id = NEW.department_id
    AND ar.location_id = NEW.location_id;
  RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER assign_access_on_hire
AFTER INSERT ON employee_profiles
FOR EACH ROW EXECUTE FUNCTION assign_access_on_hire_fn();

-- Update comp_id after name change
CREATE OR REPLACE FUNCTION update_comp_id_on_name_change_fn() RETURNS trigger AS $$
BEGIN
  UPDATE employee_profiles
  SET comp_id = upper(left(NEW.first_name,1)) || upper(left(NEW.last_name,1)) || substring(OLD.comp_id, 3)
  WHERE id = NEW.id;
  RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER update_comp_id_on_name_change
AFTER UPDATE OF first_name, last_name ON employee_profiles
FOR EACH ROW EXECUTE FUNCTION update_comp_id_on_name_change_fn();

-- Deactivate employee access when badge expires
CREATE OR REPLACE FUNCTION deactivate_access_on_expire_fn() RETURNS trigger AS $$
BEGIN
    UPDATE employee_access
    SET active = FALSE
    WHERE employee_id = NEW.id AND (NEW.expire_date IS NOT NULL AND NEW.expire_date <= CURRENT_DATE) AND active = TRUE;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER deactivate_access_on_expire
AFTER UPDATE OF expire_date ON employee_profiles
FOR EACH ROW EXECUTE FUNCTION deactivate_access_on_expire_fn();

-- Log badge print events
CREATE OR REPLACE FUNCTION log_badge_print_fn() RETURNS trigger AS $$
BEGIN
    IF NEW.badge_printed IS TRUE AND (OLD.badge_printed IS DISTINCT FROM TRUE) THEN
        INSERT INTO audit_log(user_id, action, object_type, object_id, details)
        VALUES (
            NEW.id, 'badge_printed', 'employee', NEW.id::text,
            jsonb_build_object('badge_serial', NEW.badge_serial, 'printed_at', NEW.badge_printed_at)
        );
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER log_badge_print
AFTER UPDATE OF badge_printed ON employee_profiles
FOR EACH ROW EXECUTE FUNCTION log_badge_print_fn();

-- Track manual access changes (update/delete)
CREATE OR REPLACE FUNCTION log_employee_access_changes_fn() RETURNS trigger AS $$
BEGIN
    IF TG_OP = 'UPDATE' THEN
        INSERT INTO audit_log(user_id, action, object_type, object_id, details)
        VALUES (
            NEW.employee_id, 'employee_access_update', 'employee_access', NEW.id::text,
            jsonb_build_object('old_active', OLD.active, 'new_active', NEW.active, 'assigned_via', NEW.assigned_via, 'expire_date', NEW.expire_date)
        );
    ELSIF TG_OP = 'DELETE' THEN
        INSERT INTO audit_log(user_id, action, object_type, object_id, details)
        VALUES (
            OLD.employee_id, 'employee_access_delete', 'employee_access', OLD.id::text, to_jsonb(OLD)
        );
    END IF;
    RETURN NULL;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER log_employee_access_update
AFTER UPDATE ON employee_access
FOR EACH ROW EXECUTE FUNCTION log_employee_access_changes_fn();

CREATE TRIGGER log_employee_access_delete
AFTER DELETE ON employee_access
FOR EACH ROW EXECUTE FUNCTION log_employee_access_changes_fn();

-- =========================
-- VIEWS
-- =========================

-- Employee access report
CREATE OR REPLACE VIEW vw_employee_access_report AS
SELECT
    ep.id AS employee_id,
    ep.first_name,
    ep.last_name,
    ep.department_id,
    ep.location_id,
    ea.access_profile_id,
    ea.assigned_via,
    ea.active,
    ea.assigned_date,
    ea.expire_date
FROM employee_profiles ep
JOIN employee_access ea ON ep.id = ea.employee_id;

-- Visitor history report
CREATE OR REPLACE VIEW vw_visitor_history_report AS
SELECT
    v.id AS visitor_id,
    v.first_name,
    v.last_name,
    v.company,
    v.visit_start,
    v.visit_end,
    v.checked_in_at,
    v.checked_out_at,
    v.host_employee_id
FROM visitors v;

-- Employees with expired badges
CREATE OR REPLACE VIEW vw_employees_with_expired_badges AS
SELECT
    id AS employee_id,
    first_name,
    last_name,
    badge_serial,
    expire_date
FROM employee_profiles
WHERE expire_date < CURRENT_DATE;

-- Readers with profile count
CREATE OR REPLACE VIEW vw_readers_with_profiles AS
SELECT
    r.id AS reader_id,
    r.name AS reader_name,
    COUNT(apr.access_profile_id) AS profile_count
FROM readers r
LEFT JOIN access_profile_readers apr ON r.id = apr.reader_id
GROUP BY r.id, r.name;

-- Swipe summary by reader
CREATE OR REPLACE VIEW vw_swipe_summary_by_reader AS
SELECT
    r.id AS reader_id,
    r.name AS reader_name,
    COUNT(s.id) AS swipe_count
FROM readers r
LEFT JOIN swipes s ON r.id = s.reader_id
GROUP BY r.id, r.name;

-- Employees without recent swipes (last 30 days)
CREATE OR REPLACE VIEW vw_employees_without_recent_swipes AS
SELECT
    ep.id AS employee_id,
    ep.first_name,
    ep.last_name
FROM employee_profiles ep
LEFT JOIN swipes s ON ep.id = s.employee_id AND s.swipe_time > (CURRENT_DATE - INTERVAL '30 days')
WHERE s.id IS NULL;

-- Profiles with reader count
CREATE OR REPLACE VIEW vw_profiles_with_reader_count AS
SELECT
    ap.id AS access_profile_id,
    ap.name AS profile_name,
    COUNT(apr.reader_id) AS reader_count
FROM access_profiles ap
LEFT JOIN access_profile_readers apr ON ap.id = apr.access_profile_id
GROUP BY ap.id, ap.name;

-- Departments with active employees
CREATE OR REPLACE VIEW vw_departments_with_active_employees AS
SELECT
    d.id AS department_id,
    d.name AS department_name,
    COUNT(ep.id) AS active_employee_count
FROM departments d
JOIN employee_profiles ep ON d.id = ep.department_id
WHERE ep.expire_date IS NULL OR ep.expire_date > CURRENT_DATE
GROUP BY d.id, d.name;

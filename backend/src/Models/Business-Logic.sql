-- =========================
-- FUNCTIONS & TRIGGERS for Business Logic
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
-- VIEWS for Reporting and Business Intelligence
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
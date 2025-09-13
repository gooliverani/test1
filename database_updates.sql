-- =============================================================================
-- DATABASE UPDATES: Photos Table, Indexes, Triggers, and Stored Functions
-- =============================================================================

-- 1. CREATE PHOTOS TABLE
-- =============================================================================
CREATE TABLE IF NOT EXISTS employee_photos (
    id SERIAL PRIMARY KEY,
    employee_id INTEGER NOT NULL REFERENCES employee_profiles(id) ON DELETE CASCADE,
    photo_data BYTEA,
    photo_url VARCHAR(500),
    photo_filename VARCHAR(255),
    photo_size INTEGER,
    mime_type VARCHAR(100),
    uploaded_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    uploaded_by VARCHAR(100),
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    
    -- Constraints
    CONSTRAINT unique_active_photo_per_employee UNIQUE (employee_id, is_active) 
        DEFERRABLE INITIALLY DEFERRED,
    CONSTRAINT check_photo_source CHECK (
        (photo_data IS NOT NULL AND photo_url IS NULL) OR 
        (photo_data IS NULL AND photo_url IS NOT NULL)
    ),
    CONSTRAINT check_photo_size CHECK (photo_size > 0)
);

-- Add comments for documentation
COMMENT ON TABLE employee_photos IS 'Stores employee profile photos with metadata';
COMMENT ON COLUMN employee_photos.photo_data IS 'Binary photo data (alternative to URL)';
COMMENT ON COLUMN employee_photos.photo_url IS 'External photo URL (alternative to binary data)';
COMMENT ON COLUMN employee_photos.is_active IS 'Only one active photo per employee allowed';

-- 2. CREATE SWIPES TABLE (for access logging)
-- =============================================================================
CREATE TABLE IF NOT EXISTS swipes (
    id SERIAL PRIMARY KEY,
    employee_id INTEGER NOT NULL REFERENCES employee_profiles(id),
    reader_id INTEGER NOT NULL REFERENCES readers(id),
    swipe_time TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    access_granted BOOLEAN DEFAULT FALSE,
    badge_serial VARCHAR(50),
    failure_reason VARCHAR(255),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    
    -- Indexes for performance
    INDEX idx_swipes_employee_time (employee_id, swipe_time),
    INDEX idx_swipes_reader_time (reader_id, swipe_time),
    INDEX idx_swipes_time (swipe_time),
    INDEX idx_swipes_badge (badge_serial)
);

COMMENT ON TABLE swipes IS 'Records all card swipe attempts and access decisions';

-- 3. CREATE BADGE HISTORY TABLE
-- =============================================================================
CREATE TABLE IF NOT EXISTS badge_history (
    id SERIAL PRIMARY KEY,
    employee_id INTEGER NOT NULL REFERENCES employee_profiles(id),
    badge_serial VARCHAR(50) NOT NULL,
    action_type VARCHAR(50) NOT NULL, -- 'ISSUED', 'REPLACED', 'SUSPENDED', 'REACTIVATED', 'EXPIRED'
    previous_badge_serial VARCHAR(50),
    issued_date TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    expiry_date DATE,
    issued_by VARCHAR(100),
    reason VARCHAR(500),
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    
    INDEX idx_badge_history_employee (employee_id),
    INDEX idx_badge_history_serial (badge_serial),
    INDEX idx_badge_history_date (issued_date)
);

COMMENT ON TABLE badge_history IS 'Tracks complete badge lifecycle and changes';

-- 4. CREATE ACCESS HISTORY TABLE
-- =============================================================================
CREATE TABLE IF NOT EXISTS access_history (
    id SERIAL PRIMARY KEY,
    employee_id INTEGER NOT NULL REFERENCES employee_profiles(id),
    access_profile_id INTEGER NOT NULL REFERENCES access_profiles(id),
    action_type VARCHAR(50) NOT NULL, -- 'GRANTED', 'REVOKED', 'MODIFIED'
    effective_date TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    expiry_date TIMESTAMP WITH TIME ZONE,
    granted_by VARCHAR(100),
    reason VARCHAR(500),
    previous_access_profile_id INTEGER REFERENCES access_profiles(id),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    
    INDEX idx_access_history_employee (employee_id),
    INDEX idx_access_history_profile (access_profile_id),
    INDEX idx_access_history_date (effective_date)
);

COMMENT ON TABLE access_history IS 'Tracks access profile assignments and changes';

-- 5. CREATE COMPREHENSIVE INDEXES
-- =============================================================================

-- Employee profiles indexes
CREATE INDEX IF NOT EXISTS idx_employee_profiles_comp_id ON employee_profiles(comp_id);
CREATE INDEX IF NOT EXISTS idx_employee_profiles_department ON employee_profiles(department_id);
CREATE INDEX IF NOT EXISTS idx_employee_profiles_location ON employee_profiles(location_id);
CREATE INDEX IF NOT EXISTS idx_employee_profiles_active ON employee_profiles(is_active);
CREATE INDEX IF NOT EXISTS idx_employee_profiles_hire_date ON employee_profiles(hire_date);
CREATE INDEX IF NOT EXISTS idx_employee_profiles_name ON employee_profiles(first_name, last_name);

-- Employee access indexes
CREATE INDEX IF NOT EXISTS idx_employee_access_employee ON employee_access(employee_id);
CREATE INDEX IF NOT EXISTS idx_employee_access_profile ON employee_access(access_profile_id);
CREATE INDEX IF NOT EXISTS idx_employee_access_active ON employee_access(active);
CREATE INDEX IF NOT EXISTS idx_employee_access_dates ON employee_access(assigned_date, expire_date);

-- Photos indexes
CREATE INDEX IF NOT EXISTS idx_employee_photos_employee ON employee_photos(employee_id);
CREATE INDEX IF NOT EXISTS idx_employee_photos_active ON employee_photos(is_active);
CREATE INDEX IF NOT EXISTS idx_employee_photos_uploaded ON employee_photos(uploaded_at);

-- Access readers indexes
CREATE INDEX IF NOT EXISTS idx_access_readers_access ON access_readers(access_profile_id);
CREATE INDEX IF NOT EXISTS idx_access_readers_reader ON access_readers(reader_id);

-- 6. CREATE TRIGGERS
-- =============================================================================

-- Trigger to update updated_at timestamp
CREATE OR REPLACE FUNCTION update_timestamp()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Apply update timestamp trigger to relevant tables
CREATE TRIGGER tr_employee_profiles_updated_at
    BEFORE UPDATE ON employee_profiles
    FOR EACH ROW EXECUTE FUNCTION update_timestamp();

CREATE TRIGGER tr_employee_photos_updated_at
    BEFORE UPDATE ON employee_photos
    FOR EACH ROW EXECUTE FUNCTION update_timestamp();

-- Trigger to ensure only one active photo per employee
CREATE OR REPLACE FUNCTION ensure_single_active_photo()
RETURNS TRIGGER AS $$
BEGIN
    IF NEW.is_active = TRUE THEN
        -- Deactivate all other photos for this employee
        UPDATE employee_photos 
        SET is_active = FALSE, updated_at = CURRENT_TIMESTAMP
        WHERE employee_id = NEW.employee_id AND id != COALESCE(NEW.id, 0);
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER tr_employee_photos_single_active
    BEFORE INSERT OR UPDATE ON employee_photos
    FOR EACH ROW EXECUTE FUNCTION ensure_single_active_photo();

-- Trigger to log badge changes
CREATE OR REPLACE FUNCTION log_badge_changes()
RETURNS TRIGGER AS $$
BEGIN
    -- Log badge changes when badge_serial changes
    IF OLD.badge_serial IS DISTINCT FROM NEW.badge_serial THEN
        INSERT INTO badge_history (
            employee_id, 
            badge_serial, 
            action_type, 
            previous_badge_serial,
            issued_by,
            reason
        ) VALUES (
            NEW.id,
            NEW.badge_serial,
            CASE 
                WHEN OLD.badge_serial IS NULL THEN 'ISSUED'
                ELSE 'REPLACED'
            END,
            OLD.badge_serial,
            'SYSTEM',
            'Badge updated via employee profile'
        );
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER tr_employee_profiles_badge_log
    AFTER UPDATE ON employee_profiles
    FOR EACH ROW EXECUTE FUNCTION log_badge_changes();

-- 7. POSTGRESQL TRANSLATED QUERIES AS STORED FUNCTIONS
-- =============================================================================

-- Function 1: Employee Attendance Audit Report
CREATE OR REPLACE FUNCTION get_employee_attendance_audit(
    p_comp_id VARCHAR(50),
    p_start_date TIMESTAMP,
    p_end_date TIMESTAMP
)
RETURNS TABLE (
    employee_name VARCHAR(255),
    swipe_id INTEGER,
    swipe_timestamp TIMESTAMP WITH TIME ZONE,
    reader_name VARCHAR(255),
    location_name VARCHAR(255),
    access_result VARCHAR(10),
    required_access_profiles TEXT,
    had_required_access VARCHAR(3),
    badge_status VARCHAR(10),
    department_name VARCHAR(255),
    employment_type VARCHAR(50)
) AS $$
BEGIN
    RETURN QUERY
    WITH employee_data AS (
        SELECT
            ep.id,
            ep.department_id,
            ep.location_id,
            ep.expire_date,
            ep.first_name || ' ' || ep.last_name AS emp_name
        FROM employee_profiles ep
        WHERE ep.comp_id = p_comp_id
    ),
    access_check AS (
        SELECT
            s.id AS swipe_id,
            s.swipe_time,
            s.access_granted,
            s.employee_id,
            s.reader_id,
            CASE WHEN ea_sub.employee_id IS NOT NULL THEN TRUE ELSE FALSE END AS has_access
        FROM swipes s
        LEFT JOIN (
            SELECT ea.employee_id, ar.reader_id
            FROM employee_access ea
            JOIN access_readers ar ON ea.access_profile_id = ar.access_profile_id
            WHERE ea.active = TRUE
        ) ea_sub ON s.employee_id = ea_sub.employee_id AND s.reader_id = ea_sub.reader_id
        WHERE s.employee_id IN (SELECT id FROM employee_data)
          AND s.swipe_time BETWEEN p_start_date AND p_end_date
    )
    SELECT
        ed.emp_name,
        ac.swipe_id,
        ac.swipe_time,
        r.name,
        l.name,
        CASE WHEN ac.access_granted THEN 'GRANTED' ELSE 'DENIED' END,
        (
            SELECT STRING_AGG(ap.name, ', ')
            FROM access_readers ar2
            JOIN access_profiles ap ON ar2.access_profile_id = ap.id
            WHERE ar2.reader_id = ac.reader_id
        ),
        CASE WHEN ac.has_access THEN 'YES' ELSE 'NO' END,
        CASE WHEN ed.expire_date >= CURRENT_DATE THEN 'VALID' ELSE 'EXPIRED' END,
        d.name,
        'EMPLOYEE' -- Placeholder for employment_type
    FROM access_check ac
    JOIN employee_data ed ON ac.employee_id = ed.id
    JOIN readers r ON ac.reader_id = r.id
    JOIN locations l ON r.location_id = l.id
    JOIN departments d ON ed.department_id = d.id
    ORDER BY ac.swipe_time DESC;
END;
$$ LANGUAGE plpgsql;

-- Function 2: Daily Attendance Report by Location
CREATE OR REPLACE FUNCTION get_daily_attendance_report(
    p_location_name VARCHAR(255),
    p_start_date DATE,
    p_end_date DATE
)
RETURNS TABLE (
    employee_id INTEGER,
    comp_id VARCHAR(50),
    employee_name VARCHAR(255),
    department_name VARCHAR(255),
    location_name VARCHAR(255),
    shift_date DATE,
    first_swipe TIMESTAMP WITH TIME ZONE,
    first_reader_name VARCHAR(255),
    last_swipe TIMESTAMP WITH TIME ZONE,
    last_reader_name VARCHAR(255),
    shift_hours NUMERIC(5,2),
    shift_type VARCHAR(20)
) AS $$
BEGIN
    RETURN QUERY
    WITH swipe_data AS (
        SELECT
            s.id,
            s.employee_id,
            ep.first_name || ' ' || ep.last_name AS emp_name,
            d.name AS dept_name,
            l.name AS loc_name,
            s.swipe_time,
            s.access_granted,
            s.reader_id,
            ep.comp_id,
            CASE
                WHEN EXTRACT(HOUR FROM s.swipe_time) >= 18 THEN
                    (DATE(s.swipe_time) + INTERVAL '1 day')::DATE
                WHEN EXTRACT(HOUR FROM s.swipe_time) < 7 THEN
                    DATE(s.swipe_time)
                ELSE DATE(s.swipe_time)
            END AS shift_date,
            CASE
                WHEN EXTRACT(HOUR FROM s.swipe_time) >= 18 OR EXTRACT(HOUR FROM s.swipe_time) < 7 THEN
                    'Night Shift'
                ELSE 'Day Shift'
            END AS shift_type_calc
        FROM swipes s
        JOIN employee_profiles ep ON s.employee_id = ep.id
        JOIN departments d ON ep.department_id = d.id
        JOIN locations l ON ep.location_id = l.id
        JOIN readers r ON s.reader_id = r.id
        WHERE l.name = p_location_name
          AND DATE(s.swipe_time) BETWEEN p_start_date AND p_end_date
          AND s.access_granted = TRUE
    ),
    shift_boundaries AS (
        SELECT
            employee_id,
            emp_name,
            dept_name,
            loc_name,
            shift_date,
            shift_type_calc,
            comp_id,
            MIN(swipe_time) AS first_swipe_time,
            MAX(swipe_time) AS last_swipe_time
        FROM swipe_data
        GROUP BY employee_id, emp_name, dept_name, loc_name, shift_date, shift_type_calc, comp_id
    ),
    swipe_details AS (
        SELECT
            sb.*,
            (SELECT reader_id FROM swipe_data sd 
             WHERE sd.employee_id = sb.employee_id AND sd.swipe_time = sb.first_swipe_time 
             LIMIT 1) AS first_reader_id,
            (SELECT reader_id FROM swipe_data sd 
             WHERE sd.employee_id = sb.employee_id AND sd.swipe_time = sb.last_swipe_time 
             LIMIT 1) AS last_reader_id
        FROM shift_boundaries sb
    )
    SELECT
        sd.employee_id,
        sd.comp_id,
        sd.emp_name,
        sd.dept_name,
        sd.loc_name,
        sd.shift_date,
        sd.first_swipe_time,
        fr.name,
        sd.last_swipe_time,
        lr.name,
        ROUND(
            EXTRACT(EPOCH FROM (sd.last_swipe_time - sd.first_swipe_time)) / 3600.0, 2
        ),
        sd.shift_type_calc
    FROM swipe_details sd
    LEFT JOIN readers fr ON sd.first_reader_id = fr.id
    LEFT JOIN readers lr ON sd.last_reader_id = lr.id
    ORDER BY sd.shift_date, sd.employee_id;
END;
$$ LANGUAGE plpgsql;

-- 8. CREATE VIEWS FOR COMMON QUERIES
-- =============================================================================

-- View for employee complete info
CREATE OR REPLACE VIEW v_employee_complete AS
SELECT 
    ep.*,
    d.name AS department_name,
    d.description AS department_description,
    t.name AS team_name,
    l.name AS location_name,
    l.address AS location_address,
    ph.photo_url,
    ph.photo_filename,
    COUNT(ea.id) AS active_access_count,
    STRING_AGG(ap.name, ', ') AS access_profile_names
FROM employee_profiles ep
LEFT JOIN departments d ON ep.department_id = d.id
LEFT JOIN teams t ON ep.team_id = t.id
LEFT JOIN locations l ON ep.location_id = l.id
LEFT JOIN employee_photos ph ON ep.id = ph.employee_id AND ph.is_active = TRUE
LEFT JOIN employee_access ea ON ep.id = ea.employee_id AND ea.active = TRUE
LEFT JOIN access_profiles ap ON ea.access_profile_id = ap.id
GROUP BY ep.id, d.id, t.id, l.id, ph.id;

-- View for recent swipes
CREATE OR REPLACE VIEW v_recent_swipes AS
SELECT 
    s.*,
    ep.first_name || ' ' || ep.last_name AS employee_name,
    ep.comp_id,
    r.name AS reader_name,
    l.name AS location_name
FROM swipes s
JOIN employee_profiles ep ON s.employee_id = ep.id
JOIN readers r ON s.reader_id = r.id
JOIN locations l ON r.location_id = l.id
ORDER BY s.swipe_time DESC;

-- 9. INSERT SAMPLE DATA
-- =============================================================================

-- Insert sample photos for existing employees
INSERT INTO employee_photos (employee_id, photo_url, photo_filename, photo_size, mime_type, uploaded_by)
SELECT 
    id,
    'https://i.pravatar.cc/150?img=' || (id % 70 + 1),
    'avatar_' || comp_id || '.jpg',
    15000,
    'image/jpeg',
    'SYSTEM'
FROM employee_profiles
WHERE id <= 10
ON CONFLICT (employee_id, is_active) DO NOTHING;

-- Insert sample swipe data
INSERT INTO swipes (employee_id, reader_id, swipe_time, access_granted, badge_serial)
SELECT 
    ep.id,
    r.id,
    CURRENT_TIMESTAMP - INTERVAL '1 day' * (random() * 30),
    CASE WHEN random() > 0.1 THEN TRUE ELSE FALSE END,
    ep.badge_serial
FROM employee_profiles ep
CROSS JOIN readers r
WHERE ep.id <= 5 AND r.id <= 3
ORDER BY random()
LIMIT 50;

-- 10. GRANT PERMISSIONS
-- =============================================================================
GRANT SELECT, INSERT, UPDATE ON employee_photos TO postgres;
GRANT SELECT, INSERT, UPDATE ON swipes TO postgres;
GRANT SELECT, INSERT, UPDATE ON badge_history TO postgres;
GRANT SELECT, INSERT, UPDATE ON access_history TO postgres;
GRANT SELECT ON v_employee_complete TO postgres;
GRANT SELECT ON v_recent_swipes TO postgres;
GRANT EXECUTE ON FUNCTION get_employee_attendance_audit TO postgres;
GRANT EXECUTE ON FUNCTION get_daily_attendance_report TO postgres;

-- Final success message
SELECT 'Database updates completed successfully' AS status;
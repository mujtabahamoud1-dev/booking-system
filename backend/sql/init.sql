CREATE TABLE IF NOT EXISTS users (
    id            SERIAL        PRIMARY KEY,
    name          VARCHAR(100)  NOT NULL,
    email         VARCHAR(100)  NOT NULL UNIQUE,
    password_hash VARCHAR(255)  NOT NULL,
    phone         VARCHAR(20),
    role          VARCHAR(20)   NOT NULL DEFAULT 'client',
    created_at    TIMESTAMPTZ   NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS refresh_tokens (
    id         SERIAL       PRIMARY KEY,
    user_id    INT          NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    token      VARCHAR(500) NOT NULL,
    expires_at TIMESTAMPTZ  NOT NULL,
    is_revoked BOOLEAN      NOT NULL DEFAULT FALSE,
    created_at TIMESTAMPTZ  NOT NULL DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS services (
    id          SERIAL          PRIMARY KEY,
    name        VARCHAR(100)    NOT NULL,
    description VARCHAR(500),
    duration    INT             NOT NULL,
    price       NUMERIC(10, 2)  NOT NULL,
    is_active   BOOLEAN         NOT NULL DEFAULT TRUE
);

CREATE TABLE IF NOT EXISTS available_slots (
    id           SERIAL  PRIMARY KEY,
    service_id   INT     NOT NULL REFERENCES services(id) ON DELETE CASCADE,
    day_of_week  INT     NOT NULL CHECK (day_of_week BETWEEN 0 AND 6),
    start_time   TIME    NOT NULL,
    end_time     TIME    NOT NULL,
    max_bookings INT     NOT NULL DEFAULT 1
);

CREATE TABLE IF NOT EXISTS bookings (
    id           SERIAL       PRIMARY KEY,
    user_id      INT          NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    service_id   INT          NOT NULL REFERENCES services(id),
    slot_id      INT          NOT NULL REFERENCES available_slots(id),
    booking_date DATE         NOT NULL,
    status       VARCHAR(20)  NOT NULL DEFAULT 'pending',
    notes        VARCHAR(500),
    created_at   TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    CONSTRAINT chk_status CHECK (status IN ('pending', 'confirmed', 'cancelled'))
);

CREATE INDEX IF NOT EXISTS idx_bookings_user_id    ON bookings(user_id);
CREATE INDEX IF NOT EXISTS idx_bookings_service_id ON bookings(service_id);
CREATE INDEX IF NOT EXISTS idx_bookings_date       ON bookings(booking_date);
CREATE INDEX IF NOT EXISTS idx_refresh_tokens_user ON refresh_tokens(user_id);

-- Seeds a default admin account for local/dev use.
-- Credentials: email a@a.a, password "a" (BCrypt $2a$11$ hash, matches AuthService.Verify).
-- ON CONFLICT keeps this safe if an a@a.a row already exists.
INSERT INTO users (name, email, password_hash, role)
VALUES ('Admin', 'a@a.a', '$2a$11$rCa91kltURK3rUKHH/GxY.AbW1oExAyeKV6RG9ddlHGQri0wtt9Oy', 'admin')
ON CONFLICT (email) DO NOTHING;

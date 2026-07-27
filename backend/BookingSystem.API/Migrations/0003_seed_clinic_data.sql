-- Realistic demo data for a small Arabic-language clinic ("عيادة النور الطبية").
--
-- Every seeded client shares the same password as the 0002 admin: "a"
-- (BCrypt $2a$11$ hash, verified by AuthService.Verify). Dev/demo only —
-- never let this script run against an environment that matters.
--
-- day_of_week follows .NET's DayOfWeek (0=Sunday .. 6=Saturday), matching the
-- check in BookingService.CreateAsync. The clinic works Sunday–Thursday, so
-- no slots exist for Friday (5) or Saturday (6).
--
-- Booking dates are computed relative to CURRENT_DATE rather than hardcoded,
-- so the demo still shows a sensible mix of past and upcoming appointments no
-- matter when the database is first created.
--
-- Each section is guarded so a partially-seeded database won't collide.

-- ---------------------------------------------------------------------------
-- Services
-- ---------------------------------------------------------------------------
INSERT INTO services (name, description, duration, price, is_active)
SELECT v.name, v.description, v.duration, v.price, v.is_active
FROM (VALUES
    ('كشف باطنة عام',           'استشارة طب باطني شاملة مع قياس المؤشرات الحيوية ومراجعة التاريخ المرضي', 30, 150.00, TRUE),
    ('استشارة أسنان',            'فحص شامل للأسنان واللثة مع وضع خطة علاج مبدئية',                        30, 200.00, TRUE),
    ('تنظيف وتلميع الأسنان',     'إزالة الجير والتصبغات وتلميع الأسنان',                                  45, 350.00, TRUE),
    ('كشف جلدية',               'تشخيص الأمراض الجلدية والحساسية ومشاكل الشعر',                          20, 180.00, TRUE),
    ('جلسة علاج طبيعي',          'جلسة تأهيل حركي للعمود الفقري والمفاصل تحت إشراف أخصائي',               60, 250.00, TRUE),
    ('كشف أطفال',               'فحص دوري للأطفال ومتابعة النمو وجدول التطعيمات',                        30, 160.00, TRUE),
    ('متابعة حمل',              'متابعة دورية للحامل مع سونار ثنائي الأبعاد',                            30, 220.00, TRUE),
    ('تحاليل مخبرية شاملة',      'باقة تشمل صورة الدم الكاملة ووظائف الكبد والكلى والسكر التراكمي',       15, 400.00, TRUE),
    ('أشعة سينية',              'تصوير بالأشعة السينية للصدر أو العظام مع تقرير الأخصائي',               15, 300.00, TRUE),
    ('كشف عيون وقياس نظر',      'فحص قاع العين وقياس ضغط العين وتحديد درجة النظر',                       25, 190.00, TRUE),
    ('استشارة تغذية علاجية',     'خطة غذائية مخصصة لإنقاص الوزن أو لمرضى السكري',                        45, 200.00, TRUE),
    ('كشف عظام ومفاصل',         'تشخيص إصابات الملاعب وآلام الظهر والمفاصل',                             30, 230.00, TRUE),
    ('لقاح الإنفلونزا الموسمية', 'تطعيم موسمي — موقوف مؤقتاً لحين وصول الدفعة الجديدة',                   10, 120.00, FALSE)
) AS v(name, description, duration, price, is_active)
WHERE NOT EXISTS (SELECT 1 FROM services s WHERE s.name = v.name);

-- ---------------------------------------------------------------------------
-- Users — one receptionist (admin) and twelve patients
-- ---------------------------------------------------------------------------
INSERT INTO users (name, email, password_hash, phone, role)
VALUES
    ('سارة عبدالرحمن الفهد',   'reception@alnoor-clinic.example', '$2a$11$rCa91kltURK3rUKHH/GxY.AbW1oExAyeKV6RG9ddlHGQri0wtt9Oy', '+966551112233', 'admin'),
    ('محمد أحمد الشمري',      'mohammed.alshammari@example.com',  '$2a$11$rCa91kltURK3rUKHH/GxY.AbW1oExAyeKV6RG9ddlHGQri0wtt9Oy', '+966501234567', 'client'),
    ('فاطمة عبدالله الزهراني', 'fatima.alzahrani@example.com',     '$2a$11$rCa91kltURK3rUKHH/GxY.AbW1oExAyeKV6RG9ddlHGQri0wtt9Oy', '+966502345678', 'client'),
    ('عبدالله يوسف الحربي',    'abdullah.alharbi@example.com',     '$2a$11$rCa91kltURK3rUKHH/GxY.AbW1oExAyeKV6RG9ddlHGQri0wtt9Oy', '+966503456789', 'client'),
    ('نورة سعد القحطاني',     'noura.alqahtani@example.com',      '$2a$11$rCa91kltURK3rUKHH/GxY.AbW1oExAyeKV6RG9ddlHGQri0wtt9Oy', '+966504567890', 'client'),
    ('خالد إبراهيم المطيري',   'khaled.almutairi@example.com',     '$2a$11$rCa91kltURK3rUKHH/GxY.AbW1oExAyeKV6RG9ddlHGQri0wtt9Oy', '+966505678901', 'client'),
    ('مريم حسن العتيبي',      'mariam.alotaibi@example.com',      '$2a$11$rCa91kltURK3rUKHH/GxY.AbW1oExAyeKV6RG9ddlHGQri0wtt9Oy', '+966506789012', 'client'),
    ('عمر طارق الدوسري',      'omar.aldosari@example.com',        '$2a$11$rCa91kltURK3rUKHH/GxY.AbW1oExAyeKV6RG9ddlHGQri0wtt9Oy', '+966507890123', 'client'),
    ('هدى ناصر الغامدي',      'huda.alghamdi@example.com',        '$2a$11$rCa91kltURK3rUKHH/GxY.AbW1oExAyeKV6RG9ddlHGQri0wtt9Oy', '+966508901234', 'client'),
    ('يوسف عبدالعزيز البلوشي', 'yousef.albalushi@example.com',     '$2a$11$rCa91kltURK3rUKHH/GxY.AbW1oExAyeKV6RG9ddlHGQri0wtt9Oy', '+966509012345', 'client'),
    ('ليلى محمود الأنصاري',    'layla.alansari@example.com',       '$2a$11$rCa91kltURK3rUKHH/GxY.AbW1oExAyeKV6RG9ddlHGQri0wtt9Oy', '+966551234567', 'client'),
    ('أحمد سامي الشهري',      'ahmed.alshehri@example.com',       '$2a$11$rCa91kltURK3rUKHH/GxY.AbW1oExAyeKV6RG9ddlHGQri0wtt9Oy', '+966552345678', 'client'),
    ('ريم فيصل السبيعي',      'reem.alsubaie@example.com',        '$2a$11$rCa91kltURK3rUKHH/GxY.AbW1oExAyeKV6RG9ddlHGQri0wtt9Oy', '+966553456789', 'client')
ON CONFLICT (email) DO NOTHING;

-- ---------------------------------------------------------------------------
-- Available slots — Sunday(0) through Thursday(4)
--
-- max_bookings mirrors how the clinic actually runs: one-on-one consultations
-- take a single patient, the physio room takes three at once, and lab draws
-- and X-rays run several in parallel.
-- ---------------------------------------------------------------------------
INSERT INTO available_slots (service_id, day_of_week, start_time, end_time, max_bookings)
SELECT sv.id, v.day_of_week, v.start_time, v.end_time, v.max_bookings
FROM (VALUES
    -- كشف باطنة عام — morning clinic most days, evening shift midweek
    ('كشف باطنة عام',           0, TIME '09:00', TIME '09:30', 1),
    ('كشف باطنة عام',           0, TIME '10:00', TIME '10:30', 1),
    ('كشف باطنة عام',           1, TIME '09:00', TIME '09:30', 1),
    ('كشف باطنة عام',           2, TIME '17:00', TIME '17:30', 2),
    ('كشف باطنة عام',           3, TIME '09:00', TIME '09:30', 1),
    ('كشف باطنة عام',           4, TIME '17:00', TIME '17:30', 1),
    -- Dental
    ('استشارة أسنان',            0, TIME '17:00', TIME '17:30', 1),
    ('استشارة أسنان',            2, TIME '09:00', TIME '09:30', 1),
    ('استشارة أسنان',            4, TIME '09:00', TIME '09:30', 1),
    ('تنظيف وتلميع الأسنان',     1, TIME '10:00', TIME '10:45', 1),
    ('تنظيف وتلميع الأسنان',     3, TIME '17:00', TIME '17:45', 1),
    -- Dermatology — two chairs
    ('كشف جلدية',               0, TIME '11:00', TIME '11:20', 2),
    ('كشف جلدية',               3, TIME '11:00', TIME '11:20', 2),
    -- Physiotherapy — shared room, three patients per session
    ('جلسة علاج طبيعي',          0, TIME '08:00', TIME '09:00', 3),
    ('جلسة علاج طبيعي',          2, TIME '08:00', TIME '09:00', 3),
    ('جلسة علاج طبيعي',          4, TIME '08:00', TIME '09:00', 3),
    -- Pediatrics
    ('كشف أطفال',               1, TIME '17:00', TIME '17:30', 2),
    ('كشف أطفال',               3, TIME '09:30', TIME '10:00', 2),
    -- OB/GYN
    ('متابعة حمل',              2, TIME '10:00', TIME '10:30', 1),
    ('متابعة حمل',              4, TIME '10:00', TIME '10:30', 1),
    -- Lab draws — early, fasting, high throughput
    ('تحاليل مخبرية شاملة',      0, TIME '07:30', TIME '07:45', 6),
    ('تحاليل مخبرية شاملة',      1, TIME '07:30', TIME '07:45', 6),
    ('تحاليل مخبرية شاملة',      2, TIME '07:30', TIME '07:45', 6),
    ('تحاليل مخبرية شاملة',      3, TIME '07:30', TIME '07:45', 6),
    ('تحاليل مخبرية شاملة',      4, TIME '07:30', TIME '07:45', 6),
    -- Imaging
    ('أشعة سينية',              0, TIME '12:00', TIME '12:15', 4),
    ('أشعة سينية',              3, TIME '12:00', TIME '12:15', 4),
    -- Ophthalmology
    ('كشف عيون وقياس نظر',      1, TIME '11:00', TIME '11:25', 2),
    ('كشف عيون وقياس نظر',      4, TIME '11:00', TIME '11:25', 2),
    -- Nutrition — evening only
    ('استشارة تغذية علاجية',     2, TIME '18:00', TIME '18:45', 1),
    ('استشارة تغذية علاجية',     4, TIME '18:00', TIME '18:45', 1),
    -- Orthopedics — evening only
    ('كشف عظام ومفاصل',         0, TIME '18:00', TIME '18:30', 1),
    ('كشف عظام ومفاصل',         3, TIME '18:00', TIME '18:30', 1)
) AS v(service_name, day_of_week, start_time, end_time, max_bookings)
JOIN services sv ON sv.name = v.service_name
WHERE NOT EXISTS (
    SELECT 1 FROM available_slots a
    WHERE a.service_id  = sv.id
      AND a.day_of_week = v.day_of_week
      AND a.start_time  = v.start_time
);

-- ---------------------------------------------------------------------------
-- Bookings
--
-- week_offset shifts the appointment relative to the slot's next occurrence:
-- negative is history, 0 is this week, positive is upcoming. The date always
-- lands on the slot's own weekday, which BookingService requires.
--
-- The Sunday 08:00 physio session is deliberately booked to its full capacity
-- of three, so the "slot full" path has real data behind it.
-- ---------------------------------------------------------------------------
INSERT INTO bookings (user_id, service_id, slot_id, booking_date, status, notes)
SELECT u.id, sv.id, sl.id,
       CURRENT_DATE
         + ((v.day_of_week - EXTRACT(DOW FROM CURRENT_DATE)::int + 7) % 7)
         + (v.week_offset * 7),
       v.status, v.notes
FROM (VALUES
    -- This week
    ('mohammed.alshammari@example.com', 'كشف باطنة عام',      0, TIME '09:00',  0, 'confirmed', 'متابعة ضغط الدم وصرف الدواء الشهري'),
    ('fatima.alzahrani@example.com',    'متابعة حمل',          2, TIME '10:00',  0, 'confirmed', 'الأسبوع الثاني والعشرون — سونار متابعة'),
    ('abdullah.alharbi@example.com',    'تحاليل مخبرية شاملة',  1, TIME '07:30',  0, 'pending',   'صائم من الساعة العاشرة مساءً'),
    ('noura.alqahtani@example.com',     'كشف جلدية',           3, TIME '11:00',  0, 'confirmed', 'حساسية جلدية متكررة في اليدين'),
    ('yousef.albalushi@example.com',    'تحاليل مخبرية شاملة',  2, TIME '07:30',  0, 'confirmed', 'فحص السكر التراكمي كل ثلاثة أشهر'),
    -- Sunday physio session, filled to capacity (max_bookings = 3)
    ('khaled.almutairi@example.com',    'جلسة علاج طبيعي',      0, TIME '08:00',  0, 'confirmed', 'الجلسة الرابعة لعلاج الانزلاق الغضروفي'),
    ('mariam.alotaibi@example.com',     'جلسة علاج طبيعي',      0, TIME '08:00',  0, 'confirmed', 'تأهيل ما بعد عملية الرباط الصليبي'),
    ('omar.aldosari@example.com',       'جلسة علاج طبيعي',      0, TIME '08:00',  0, 'pending',   'ألم مزمن أسفل الظهر'),
    ('huda.alghamdi@example.com',       'كشف أطفال',           1, TIME '17:00',  0, 'confirmed', 'تطعيم الشهر التاسع ومتابعة الوزن'),
    -- Upcoming
    ('yousef.albalushi@example.com',    'كشف عظام ومفاصل',     0, TIME '18:00',  1, 'pending',   'إصابة في الكاحل أثناء ممارسة الرياضة'),
    ('layla.alansari@example.com',      'استشارة تغذية علاجية', 2, TIME '18:00',  1, 'confirmed', 'خطة غذائية لمرضى السكري من النوع الثاني'),
    ('ahmed.alshehri@example.com',      'استشارة أسنان',        4, TIME '09:00',  1, 'pending',   'ألم في الضرس الخلفي عند المضغ'),
    ('reem.alsubaie@example.com',       'تنظيف وتلميع الأسنان', 3, TIME '17:00',  1, 'confirmed', 'تنظيف دوري كل ستة أشهر'),
    ('omar.aldosari@example.com',       'كشف عيون وقياس نظر',  4, TIME '11:00',  1, 'pending',   'ضعف في النظر أثناء القيادة الليلية'),
    ('mariam.alotaibi@example.com',     'كشف أطفال',           3, TIME '09:30',  1, 'confirmed', 'فحص دوري وتقييم النمو'),
    ('huda.alghamdi@example.com',       'كشف جلدية',           0, TIME '11:00',  2, 'pending',   'متابعة نتيجة العلاج بعد شهر'),
    -- History
    ('mohammed.alshammari@example.com', 'تحاليل مخبرية شاملة',  0, TIME '07:30', -1, 'confirmed', 'تحاليل ما قبل موعد الباطنة'),
    ('khaled.almutairi@example.com',    'أشعة سينية',          3, TIME '12:00', -1, 'confirmed', 'أشعة على الصدر بناءً على طلب الطبيب'),
    ('noura.alqahtani@example.com',     'كشف عيون وقياس نظر',  1, TIME '11:00', -1, 'cancelled', 'اعتذرت المريضة عن الحضور'),
    ('fatima.alzahrani@example.com',    'كشف باطنة عام',       2, TIME '17:00', -2, 'cancelled', 'تم تأجيل الموعد بناءً على طلب المريضة')
) AS v(email, service_name, day_of_week, start_time, week_offset, status, notes)
JOIN users    u  ON u.email = v.email
JOIN services sv ON sv.name = v.service_name
JOIN available_slots sl
      ON sl.service_id  = sv.id
     AND sl.day_of_week = v.day_of_week
     AND sl.start_time  = v.start_time
WHERE NOT EXISTS (
    SELECT 1 FROM bookings b
    WHERE b.user_id = u.id
      AND b.slot_id = sl.id
      AND b.booking_date = CURRENT_DATE
         + ((v.day_of_week - EXTRACT(DOW FROM CURRENT_DATE)::int + 7) % 7)
         + (v.week_offset * 7)
);

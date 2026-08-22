-- Demo data for Atlas Physiotherapy — one clinic, one discipline, two languages.
--
-- Every seeded client shares the same password as the 0002 admin: "a"
-- (BCrypt $2a$11$ hash, verified by AuthService.Verify). Dev/demo only —
-- never let this script run against an environment that matters.
--
-- day_of_week follows .NET's DayOfWeek (0=Sunday .. 6=Saturday), matching the
-- check in BookingService.CreateAsync. The clinic works Sunday–Thursday, so no
-- slots exist for Friday (5) or Saturday (6), and every slot sits inside the
-- 08:00–18:00 day the front end draws its time axis against.
--
-- Booking dates are computed relative to CURRENT_DATE rather than hardcoded, so
-- the demo still shows a sensible mix of past and upcoming appointments no
-- matter when the database is first created.
--
-- Each section is guarded so a partially-seeded database won't collide.

-- ---------------------------------------------------------------------------
-- Services — Hydrotherapy is seeded inactive on purpose
-- ---------------------------------------------------------------------------
-- The leading ordinal pins the insert order; the list is sorted by id.
INSERT INTO services (name, name_ar, description, description_ar, duration, price, is_active)
SELECT v.name, v.name_ar, v.description, v.description_ar, v.duration, v.price, v.is_active
FROM (VALUES
    (1, 'Initial assessment',    'تقييم مبدئي',
     'A full movement and history review, and a plan for where to go next.',
     'فحص شامل للحركة ومراجعة التاريخ المرضي، مع خطة علاج واضحة للخطوات القادمة.',
     45,  90.00, TRUE),

    (2, 'Follow-up session',     'جلسة متابعة',
     'A standard treatment session once your programme is under way.',
     'جلسة علاجية اعتيادية بعد بدء البرنامج العلاجي.',
     30,  60.00, TRUE),

    (3, 'Manual therapy',        'علاج يدوي',
     'Hands-on joint and soft-tissue work for pain and stiffness.',
     'عمل يدوي على المفاصل والأنسجة الرخوة لتخفيف الألم والتيبس.',
     30,  65.00, TRUE),

    (4, 'Sports injury rehab',   'تأهيل الإصابات الرياضية',
     'Loaded strength work for return to sport after a soft-tissue injury.',
     'تمارين قوة تدريجية للعودة إلى النشاط الرياضي بعد إصابات الأنسجة الرخوة.',
     60, 120.00, TRUE),

    (5, 'Post-op recovery',      'تأهيل ما بعد الجراحة',
     'Staged rehab following surgery, coordinated with your surgeon.',
     'تأهيل تدريجي بعد العمليات الجراحية بالتنسيق مع الجراح المعالج.',
     60, 110.00, TRUE),

    (6, 'Back and neck programme', 'برنامج الظهر والرقبة',
     'A structured programme for desk-related back and neck pain.',
     'برنامج منظم لآلام الظهر والرقبة الناتجة عن الجلوس الطويل والعمل المكتبي.',
     45,  95.00, TRUE),

    (7, 'Dry needling',          'الإبر الجافة',
     'Fine-needle treatment for trigger points and persistent muscle tightness.',
     'علاج بالإبر الدقيقة للنقاط الزنادية والشد العضلي المزمن.',
     30,  75.00, TRUE),

    (8, 'Gait and balance clinic', 'تقييم المشي والاتزان',
     'Walking and balance assessment, with exercises to reduce the risk of falls.',
     'تقييم لنمط المشي والاتزان مع تمارين لتقليل خطر السقوط.',
     45, 100.00, TRUE),

    (9, 'Group exercise class',  'تمارين جماعية',
     'A small supervised class for strength and mobility. Six places per session.',
     'حصة جماعية صغيرة تحت الإشراف لتحسين القوة والمرونة. ستة مقاعد لكل جلسة.',
     45,  40.00, TRUE),

    (10, 'Hydrotherapy',          'العلاج المائي',
     'Pool-based rehabilitation — suspended while the pool is refitted.',
     'تأهيل داخل المسبح — موقوف مؤقتاً لحين الانتهاء من صيانة المسبح.',
     45, 105.00, FALSE)
) AS v(sort, name, name_ar, description, description_ar, duration, price, is_active)
WHERE NOT EXISTS (SELECT 1 FROM services s WHERE s.name = v.name)
ORDER BY v.sort;

-- ---------------------------------------------------------------------------
-- Users — one receptionist (admin) and twelve patients
-- ---------------------------------------------------------------------------
INSERT INTO users (name, email, password_hash, phone, role)
VALUES
    ('سارة عبدالرحمن الفهد',   'reception@atlas-physio.example',  '$2a$11$rCa91kltURK3rUKHH/GxY.AbW1oExAyeKV6RG9ddlHGQri0wtt9Oy', '+966551112233', 'admin'),
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
-- Available slots — Sunday(0) through Thursday(4), inside 08:00–18:00
--
-- max_bookings mirrors how the clinic runs: hands-on work takes one patient,
-- rehab blocks two or three, the group class six.
--
-- Hydrotherapy has no slots on purpose: an empty week is a state to render.
-- ---------------------------------------------------------------------------
INSERT INTO available_slots (service_id, day_of_week, start_time, end_time, max_bookings)
SELECT sv.id, v.day_of_week, v.start_time, v.end_time, v.max_bookings
FROM (VALUES
    -- Initial assessment — new patients, mornings plus one afternoon clinic
    ('Initial assessment',      0, TIME '09:00', TIME '12:00', 3),
    ('Initial assessment',      2, TIME '14:00', TIME '17:00', 3),
    ('Initial assessment',      4, TIME '09:00', TIME '11:00', 2),
    -- Follow-up session — the bulk of the week's work
    ('Follow-up session',       0, TIME '13:00', TIME '15:00', 4),
    ('Follow-up session',       1, TIME '09:00', TIME '12:00', 4),
    ('Follow-up session',       3, TIME '13:00', TIME '16:00', 4),
    ('Follow-up session',       4, TIME '15:00', TIME '17:00', 4),
    -- Manual therapy — one practitioner, one patient at a time
    ('Manual therapy',          1, TIME '08:00', TIME '10:00', 1),
    ('Manual therapy',          3, TIME '16:00', TIME '18:00', 1),
    ('Manual therapy',          4, TIME '08:00', TIME '10:00', 1),
    -- Sports injury rehab — gym floor, two at once
    ('Sports injury rehab',     0, TIME '16:00', TIME '18:00', 2),
    ('Sports injury rehab',     2, TIME '08:00', TIME '10:00', 2),
    ('Sports injury rehab',     4, TIME '16:00', TIME '18:00', 2),
    -- Post-op recovery — one-to-one, quieter parts of the day
    ('Post-op recovery',        1, TIME '13:00', TIME '15:00', 1),
    ('Post-op recovery',        3, TIME '09:00', TIME '11:00', 1),
    -- Back and neck programme
    ('Back and neck programme', 2, TIME '10:00', TIME '12:00', 2),
    ('Back and neck programme', 4, TIME '13:00', TIME '15:00', 2),
    -- Dry needling
    ('Dry needling',            1, TIME '15:00', TIME '17:00', 1),
    ('Dry needling',            3, TIME '11:00', TIME '13:00', 1),
    -- Gait and balance clinic
    ('Gait and balance clinic', 1, TIME '10:00', TIME '12:00', 2),
    ('Gait and balance clinic', 3, TIME '14:00', TIME '16:00', 2),
    -- Group exercise class — six places, start and end of the day
    ('Group exercise class',    0, TIME '08:00', TIME '09:00', 6),
    ('Group exercise class',    2, TIME '12:00', TIME '13:00', 6),
    ('Group exercise class',    4, TIME '08:00', TIME '09:00', 6)
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
-- Monday 08:00 manual therapy and Thursday 09:00 assessment are booked to
-- capacity, so the "slot full" path has data behind it.
-- ---------------------------------------------------------------------------
INSERT INTO bookings (user_id, service_id, slot_id, booking_date, status, notes)
SELECT u.id, sv.id, sl.id,
       CURRENT_DATE
         + ((v.day_of_week - EXTRACT(DOW FROM CURRENT_DATE)::int + 7) % 7)
         + (v.week_offset * 7),
       v.status, v.notes
FROM (VALUES
    -- This week
    ('mohammed.alshammari@example.com', 'Initial assessment',      0, TIME '09:00',  0, 'confirmed', 'ألم أسفل الظهر منذ ثلاثة أسابيع'),
    ('fatima.alzahrani@example.com',    'Follow-up session',       1, TIME '09:00',  0, 'confirmed', 'الجلسة الثالثة — تحسن ملحوظ في المدى الحركي'),
    ('abdullah.alharbi@example.com',    'Manual therapy',          1, TIME '08:00',  0, 'confirmed', 'تيبس في الكتف الأيمن'),
    ('noura.alqahtani@example.com',     'Group exercise class',    0, TIME '08:00',  0, 'confirmed', 'تمارين تقوية عامة'),
    ('khaled.almutairi@example.com',    'Sports injury rehab',     2, TIME '08:00',  0, 'pending',   'Return to running after a hamstring tear'),
    ('mariam.alotaibi@example.com',     'Back and neck programme', 2, TIME '10:00',  0, 'confirmed', 'آلام الرقبة من العمل المكتبي'),
    ('omar.aldosari@example.com',       'Follow-up session',       0, TIME '13:00',  0, 'pending',   'متابعة بعد التقييم المبدئي'),
    ('huda.alghamdi@example.com',       'Gait and balance clinic', 1, TIME '10:00',  0, 'confirmed', 'تقييم الاتزان بعد كسر في الكاحل'),
    -- Thursday's assessment clinic, filled to capacity (max_bookings = 2)
    ('yousef.albalushi@example.com',    'Initial assessment',      4, TIME '09:00',  0, 'confirmed', 'إصابة في الركبة أثناء كرة القدم'),
    ('layla.alansari@example.com',      'Initial assessment',      4, TIME '09:00',  0, 'pending',   'ألم في الكتف عند رفع الذراع'),
    -- Upcoming
    ('ahmed.alshehri@example.com',      'Post-op recovery',        3, TIME '09:00',  1, 'pending',   'تأهيل بعد عملية الرباط الصليبي'),
    ('reem.alsubaie@example.com',       'Dry needling',            1, TIME '15:00',  1, 'confirmed', 'نقاط زنادية في أعلى الظهر'),
    ('mohammed.alshammari@example.com', 'Follow-up session',       3, TIME '13:00',  1, 'pending',   'متابعة أسبوعية'),
    ('mariam.alotaibi@example.com',     'Sports injury rehab',     4, TIME '16:00',  1, 'confirmed', 'المرحلة الأخيرة من برنامج العودة للملعب'),
    ('omar.aldosari@example.com',       'Manual therapy',          3, TIME '16:00',  1, 'pending',   'شد عضلي في أسفل الظهر'),
    ('noura.alqahtani@example.com',     'Group exercise class',    2, TIME '12:00',  1, 'confirmed', 'الحصة الجماعية الأسبوعية'),
    ('huda.alghamdi@example.com',       'Back and neck programme', 4, TIME '13:00',  2, 'pending',   'متابعة بعد انتهاء البرنامج'),
    -- History
    ('khaled.almutairi@example.com',    'Initial assessment',      0, TIME '09:00', -1, 'confirmed', 'التقييم المبدئي قبل بدء البرنامج'),
    ('layla.alansari@example.com',      'Follow-up session',       4, TIME '15:00', -1, 'confirmed', 'الجلسة الثانية'),
    ('fatima.alzahrani@example.com',    'Gait and balance clinic', 3, TIME '14:00', -1, 'cancelled', 'اعتذرت المريضة عن الحضور'),
    ('yousef.albalushi@example.com',    'Post-op recovery',        1, TIME '13:00', -2, 'cancelled', 'تم تأجيل الموعد بناءً على طلب المريض')
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

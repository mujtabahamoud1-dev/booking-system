# Product

<!-- impeccable:product-schema 1 -->

## Platform

web

## Users

**Primary: the patient booking a session.** Someone who has decided they need physiotherapy —
a recurring back problem, a sports injury, post-operative rehab — and wants to secure a time
without phoning the clinic during opening hours. Often on a phone, often outside working hours,
often deciding between two or three session types they don't fully understand yet. Their job is
narrow and complete: understand what's on offer, find a time that fits their week, and leave
with a confirmed appointment. When patient and admin needs conflict, the patient wins.

**Secondary: clinic reception/admin staff.** One person, signed in all day, working a queue.
Their jobs are approving or declining pending bookings, publishing the week's availability, and
keeping the service list current. They repeat the same handful of actions many times, so
scanability and low friction matter more to them than first-impression polish.

**Third, and never designed for at the expense of the first two: a hiring reviewer.** This is a
portfolio project (see Operating Context). A reviewer clicking through for five minutes is a
real audience, but the way to serve them is to make the product genuinely good for patients and
staff, not to add reviewer-facing scaffolding.

## Product Purpose

Atlas Physiotherapy's booking site. It lets a patient browse the clinic's session types, see the
week's real availability, and request an appointment; it lets clinic staff publish that
availability and work the resulting queue.

Success for a patient is a confirmed appointment at a time they can actually make, reached
without a phone call and without a wrong guess about which day a slot falls on. Success for
staff is an empty pending queue and a week whose cover they can verify at a glance.

## Positioning

A single named clinic's booking site, not a generic scheduling template. The interface uses the
clinic's own vocabulary and shows its own week — services are physiotherapy sessions with real
durations, availability is practitioner cover, and copy is written in the clinic's voice.

The mechanism the product commits to: **availability is shown as time, not as a list of
strings.** Slots are drawn as bars against a real clock axis, so a patient reads the week's cover
the way they read a timetable, and an admin sees a gap or a double-booking as a shape rather
than by comparing rows. The booking flow follows from this — choosing a time fixes the weekday,
so the date step can offer the next occurrences of that weekday as one-tap choices that cannot
be wrong.

## Operating Context

- Patients arrive from a phone, frequently in the evening, and may never have used the site
  before. Registration sits between them and a booking, so it must not feel like an obstacle.
- Slots are weekly recurring patterns (day of week + start/end time + capacity), not individual
  calendar events. A booking is a slot plus a concrete date, and the two must agree on weekday —
  this is the single most likely user error and the interface is responsible for preventing it.
- Bookings are **requests**, not instant confirmations: they land as `pending` and a human
  approves them. Patients must not be led to believe an appointment is confirmed when it isn't.
- The clinic operates a limited weekly window (roughly 08:00–18:00, not all seven days), so any
  time visualisation must fit the clinic's real hours rather than a full 24-hour day.
- Staff work the admin screens on a desktop; patients are mobile-first.
- The repository is also a portfolio piece targeting Arabic-speaking and international remote
  employers, which is why bilingual quality and visible craft are load-bearing rather than
  optional (`PLANNING.md`).

## Capabilities and Constraints

**Confirmed functionality**

- Accounts with two roles: `admin` and `client`. JWT access tokens with refresh-token rotation.
- Public: browse services (name, description, duration in minutes, price) and a service's slots.
- Client: register, sign in, create a booking (service + slot + date + optional notes), view own
  bookings, cancel own booking.
- Admin: full CRUD on services (with active/inactive state), CRUD on slots, view all bookings,
  change booking status (`pending` → `confirmed` / `cancelled`).
- Routes: `/`, `/login`, `/register`, `/book/:serviceId`, `/bookings`, `/admin`,
  `/admin/services`, `/admin/slots`, `/admin/bookings`.

**Technical constraints**

- Backend: .NET 10 Web API, Dapper over PostgreSQL, DbUp SQL migrations, BCrypt, JWT bearer.
  Frontend: Vue 3 `<script setup>` + TypeScript, Pinia, vue-router, Tailwind CSS v4, vue-i18n
  v11 in Composition mode. Docker Compose for local infrastructure.
- Tailwind v4 has no JS config; design tokens are declared in `@theme { }` in
  `frontend/src/style.css` and generate utilities from there.
- **Bilingual English/Arabic with full RTL.** `en.ts` exports `type Messages = typeof en` and
  `ar.ts` is typed against it, so `vue-tsc` fails the build if the two locales diverge — every
  new string must be added to both. Layout must use CSS logical properties and the `rtl:`
  variant, never hard-coded left/right.
- vue-i18n pluralization is deliberately unused: Arabic has six plural forms and the library's
  default rule is English's two. Copy is phrased so no key interpolates a bare count into a noun.
- Inherently left-to-right data — clock times, ISO dates, prefix currency — is wrapped in
  `dir="ltr"` even in Arabic. This is an established codebase convention.
- Dates are calendar dates. `toISOString()` is banned for date formatting; it reports UTC and
  rolls the date over for users east or west of it.
- A service that already has bookings cannot be deleted (API returns 409); it is deactivated
  instead. The interface must say so rather than reporting a generic failure.

**Open decisions**

- Public deployment is planned for Railway (backend) and Vercel (frontend) but has not happened.
- Email notification on booking confirmation, payments, and reviews are explicitly post-MVP.
- No automated test suite yet.

## Brand Commitments

- Name: **Atlas Physiotherapy**. Discipline shown alongside the wordmark in the masthead.
- Voice: calm and clinical. Plain verbs, sentence case, no exclamation, no selling. States what
  will happen rather than how it feels. Errors say what went wrong and what to do about it.
- Both languages are first-class. Arabic is not a translation layer bolted onto an English
  product; a screen is not finished until it is right in both.
- No logo asset exists. The identity is typographic.

## Evidence on Hand

- **Real seed data ships as a migration** (`backend/BookingSystem.API/Migrations/0003_seed_clinic_data.sql`):
  the clinic's session types, a week of slots, and sample bookings including a pending one, so
  every screen has something true to render on a fresh database. A seeded admin account exists
  in `0002_seed_admin_user.sql`.
- The deployed site is intended to carry this seeded data plus published demo credentials for
  both a patient and an admin account, so a visitor can see the whole product without
  registering. **The demo credentials do not exist yet** and must be created rather than assumed.
- There are no customers, testimonials, practitioner biographies, clinic photographs, press,
  ratings, or usage statistics. Future work must not invent any. Anything resembling proof must
  come from the product's own data.
- No real practitioner names. Slots are clinic cover, not named individuals' diaries.

## Product Principles

1. **The patient's booking is the product.** Everything else exists to keep it accurate. When a
   trade-off appears between the booking flow and an admin convenience, the booking flow wins.
2. **Show the week, don't describe it.** Availability is spatial and comparable. A patient should
   never have to read seven lines of text to learn something the eye can take in at once.
3. **Make the wrong answer unreachable.** Where the system knows a constraint — a slot's weekday,
   a service's capacity, a date in the past — the interface should prevent the error rather than
   report it afterwards.
4. **Say what actually happened.** A request is pending until a human confirms it. Never let
   confident visual design imply a certainty the system hasn't got.
5. **Both languages, or neither.** No feature ships English-only, and no layout is verified until
   it has been seen mirrored.

## Accessibility & Inclusion

- Full RTL support is a product requirement, not a nicety, and is the reason for the bilingual
  type stack.
- Keyboard operability with visible focus throughout; the availability picker is a real
  radiogroup, not a set of clickable divs.
- `prefers-reduced-motion` is respected globally.
- Mobile-first: patients are assumed to be on a phone, so no primary task may depend on hover or
  on a viewport wider than roughly 390px.

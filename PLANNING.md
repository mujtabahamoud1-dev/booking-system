# 📅 Booking System — Project Planning

> A full-stack web-based booking system built as a portfolio project to demonstrate
> professional-grade skills in .NET, Vue.js, and modern software architecture.

---

## 🎯 Project Goal

Build a clean, production-ready booking system that can be used by any service-based
business (clinic, salon, sports facility, etc.). This project serves as the main
portfolio piece to target Arabic and international remote companies.

---

## 🛠️ Tech Stack

| Layer | Technology |
|-------|-----------|
| Backend | .NET 8 (ASP.NET Core Web API) |
| Frontend | Vue 3 + TypeScript |
| Database | PostgreSQL |
| ORM | Entity Framework Core |
| Auth | JWT + Refresh Tokens |
| Styling | Tailwind CSS |
| Containerization | Docker + Docker Compose |
| CI/CD | GitHub Actions (future) |
| Deployment | Railway (backend) + Vercel (frontend) |

---

## 📁 Repository Structure

```
booking-system/
│
├── backend/
│   ├── src/
│   │   ├── Controllers/
│   │   ├── Models/
│   │   ├── DTOs/
│   │   ├── Services/
│   │   ├── Repositories/
│   │   └── Middleware/
│   ├── tests/
│   └── Dockerfile
│
├── frontend/
│   ├── src/
│   │   ├── components/
│   │   ├── views/
│   │   ├── stores/       # Pinia
│   │   ├── services/     # API calls
│   │   └── router/
│   ├── public/
│   └── Dockerfile
│
├── .github/
│   └── workflows/        # CI/CD (future)
│
├── docker-compose.yml
├── PLANNING.md
└── README.md
```

---

## 🗄️ Database Schema

### Users
```sql
Id            INT           PRIMARY KEY
Name          NVARCHAR(100)
Email         NVARCHAR(100) UNIQUE
PasswordHash  NVARCHAR(255)
Phone         NVARCHAR(20)
Role          NVARCHAR(20)  -- "admin" | "client"
CreatedAt     DATETIME
```

### RefreshTokens
```sql
Id            INT           PRIMARY KEY
UserId        INT           FK → Users
Token         NVARCHAR(500)
ExpiresAt     DATETIME
IsRevoked     BIT
CreatedAt     DATETIME
```

### Services
```sql
Id            INT           PRIMARY KEY
Name          NVARCHAR(100)
Description   NVARCHAR(500)
Duration      INT           -- in minutes
Price         DECIMAL(10,2)
IsActive      BIT
```

### AvailableSlots
```sql
Id            INT           PRIMARY KEY
ServiceId     INT           FK → Services
DayOfWeek     INT           -- 0=Sun ... 6=Sat
StartTime     TIME
EndTime       TIME
MaxBookings   INT
```

### Bookings
```sql
Id            INT           PRIMARY KEY
UserId        INT           FK → Users
ServiceId     INT           FK → Services
SlotId        INT           FK → AvailableSlots
BookingDate   DATE
Status        NVARCHAR(20)  -- "pending"|"confirmed"|"cancelled"
Notes         NVARCHAR(500)
CreatedAt     DATETIME
```

### Relationships
```
Users ──< RefreshTokens
Users ──< Bookings >── Services
                │
          AvailableSlots
```

---

## 🔌 API Endpoints

### Auth
```
POST   /api/auth/register
POST   /api/auth/login
POST   /api/auth/refresh
POST   /api/auth/logout
```

### Services
```
GET    /api/services              -- public
GET    /api/services/:id          -- public
POST   /api/services              -- admin only
PUT    /api/services/:id          -- admin only
DELETE /api/services/:id          -- admin only
```

### Slots
```
GET    /api/slots/:serviceId      -- public
POST   /api/slots                 -- admin only
DELETE /api/slots/:id             -- admin only
```

### Bookings
```
GET    /api/bookings              -- admin: all | client: own
POST   /api/bookings              -- client only
PUT    /api/bookings/:id/status   -- admin only
DELETE /api/bookings/:id          -- client (cancel own)
```

---

## 🔐 Authentication Flow

```
1. User logs in → receives AccessToken (15 min) + RefreshToken (7 days)
2. Client stores RefreshToken in httpOnly cookie
3. On AccessToken expiry → POST /api/auth/refresh
4. On logout → RefreshToken is revoked in DB
```

### Roles
| Role | Permissions |
|------|------------|
| Admin | Full access — manage services, slots, all bookings |
| Client | Register, login, view services, create/cancel own bookings |

---

## 🖥️ Frontend Pages

### Public
- `/` — Landing page + services list
- `/login` — Login
- `/register` — Register

### Client (authenticated)
- `/bookings` — My bookings
- `/book/:serviceId` — Book a service

### Admin
- `/admin/dashboard` — Overview stats
- `/admin/services` — Manage services
- `/admin/slots` — Manage available slots
- `/admin/bookings` — All bookings + status management

---

## ✅ MVP Scope

These features must be completed before deployment:

- [ ] User registration and login with JWT
- [ ] Refresh token implementation
- [ ] Role-based authorization (Admin / Client)
- [ ] CRUD for services (Admin)
- [ ] CRUD for available slots (Admin)
- [ ] Create and cancel bookings (Client)
- [ ] View and manage all bookings (Admin)
- [ ] RTL Arabic UI support
- [ ] Dockerized backend and frontend
- [ ] Deployed and accessible via public URL
- [ ] README with screenshots

---

## 🚀 Phases & Time Estimate

> Working at ~2 hours/day after work

| Phase | Tasks | Hours | Duration |
|-------|-------|-------|----------|
| 1 — Planning | DB design, API design, repo setup | 3h | Day 1 |
| 2 — Backend | .NET API + Auth + all endpoints | 15h | ~2 weeks |
| 3 — Frontend | Vue pages + Pinia + API integration | 15h | ~2 weeks |
| 4 — Polish | RTL, deploy, README, screenshots | 7h | ~1 week |
| **Total** | | **~40h** | **~5 weeks** |

> ⚠️ Start date: After delivering current company project (1 month from now)

---

## 🔮 Future Improvements (Post-MVP)

- [ ] CI/CD with GitHub Actions
- [ ] Email notifications on booking confirmation
- [ ] Payment integration
- [ ] Reviews and ratings
- [ ] Multi-language support (i18n)
- [ ] Rate limiting and logging
- [ ] Unit and integration tests

---

## 📝 Notes

- Do NOT include any code or ideas from current employer's projects
- Keep all commits in English
- Write meaningful commit messages (feat:, fix:, chore:)
- Add screenshots to README after each major milestone
- This project is intentionally different in domain from current work (messaging app)
  to avoid any IP conflicts

---

*Generated as part of personal career planning — portfolio project for remote job applications*

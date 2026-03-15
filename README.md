# Picker API

A .NET 9 REST API that randomly suggests **Food**, **Movies**, or **Books** to authenticated users. Supports filtering by cuisine/genre, user comments, and a 1–5 star rating system.

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Runtime | .NET 9 / ASP.NET Core 9 |
| ORM | Entity Framework Core 9 (Code-First) |
| Database | PostgreSQL (Npgsql) |
| Auth | ASP.NET Core Identity + JWT Bearer + Google OAuth 2.0 |
| Docs | Swagger / Swashbuckle |
| CI | GitHub Actions |

---

## Architecture

Clean Architecture with four projects:

```
Picker/
├── src/
│   ├── Picker.Domain/          # Entities, enums, repository & UoW interfaces
│   ├── Picker.Application/     # DTOs, service interfaces & implementations (no HTTP/EF deps)
│   ├── Picker.Infrastructure/  # EF Core, Identity, JWT, Google OAuth, repositories
│   └── Picker.API/             # Controllers, middleware, Program.cs
└── .github/workflows/ci.yml
```

---

## Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- PostgreSQL 15+
- A Google Cloud project with OAuth 2.0 credentials (for Google SSO)

### 1. Clone & configure

```bash
git clone <repo-url>
cd Picker
```

Open `src/Picker.API/appsettings.json` and fill in:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=PickerDb;Username=postgres;Password=your_password"
  },
  "JwtSettings": {
    "Secret": "your-secret-key-minimum-32-characters-long!!",
    "Issuer": "PickerAPI",
    "Audience": "PickerClient",
    "ExpiryMinutes": 60
  },
  "Authentication": {
    "Google": {
      "ClientId": "your-google-client-id.apps.googleusercontent.com",
      "ClientSecret": "your-google-client-secret"
    }
  },
  "AdminSettings": {
    "AdminEmails": [
      "yourname@gmail.com"
    ]
  }
}
```

> **Admin role**: any email listed in `AdminSettings.AdminEmails` automatically receives the `Admin` role when that account is created (via registration or first Google sign-in). Everyone else gets the `User` role.

### 2. Apply database migrations

```bash
dotnet ef database update \
  --project src/Picker.Infrastructure \
  --startup-project src/Picker.API
```

### 3. Run

```bash
dotnet run --project src/Picker.API
```

Open **https://localhost:{port}/swagger** to explore the API interactively.

---

## Authentication

The API uses **JWT Bearer tokens**. Every protected endpoint requires an `Authorization: Bearer <token>` header.

### Option A — Email / Password

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/auth/register` | Create a new account |
| `POST` | `/api/auth/login` | Login and receive a JWT token |

**Register example:**
```json
POST /api/auth/register
{
  "email": "user@example.com",
  "password": "secret123",
  "displayName": "Alice"
}
```

**Login example:**
```json
POST /api/auth/login
{
  "email": "user@example.com",
  "password": "secret123"
}
```

Both return an `AuthResponseDto`:
```json
{
  "token": "eyJhbGci...",
  "expiresAt": "2026-03-15T13:00:00Z",
  "userId": "abc-123",
  "email": "user@example.com",
  "displayName": "Alice",
  "role": "User"
}
```

### Option B — Google SSO

1. Visit `GET /api/auth/signin/google` in a browser — you will be redirected to Google's sign-in page.
2. After signing in, Google redirects back to `GET /api/auth/callback/google`.
3. The API responds with the same `AuthResponseDto` containing your JWT token.

> For Swagger testing: complete the Google flow in your browser, copy the token from the JSON response, then click **Authorize** in Swagger and paste `Bearer <token>`.

### Checking your identity

```
GET /api/auth/me          (requires JWT)
```

---

## Roles

| Role | Permissions |
|------|-------------|
| `Admin` | Full CRUD on all resources |
| `User` | Read-only (list, by ID, random), create/edit/delete **own** comments, rate items |

### How to become an Admin

**Method 1 — Pre-configure (recommended):** Add your email to `AdminSettings.AdminEmails` in `appsettings.json` before registering. The role is assigned automatically on account creation.

**Method 2 — Promote via API:** An existing Admin can promote any user:

```json
POST /api/admin/users/{userId}/assign-role
Authorization: Bearer <admin-token>

{ "role": "Admin" }
```

To see all users and their IDs:
```
GET /api/admin/users    (Admin only)
```

---

## API Reference

### Foods

| Method | Endpoint | Role | Description |
|--------|----------|------|-------------|
| `GET` | `/api/foods` | User+ | List all foods (filter: `?cuisineId=`) |
| `GET` | `/api/foods/{id}` | User+ | Get food by ID |
| `GET` | `/api/foods/random` | User+ | Pick a random food (filter: `?cuisineId=`) |
| `POST` | `/api/foods` | Admin | Create food |
| `PUT` | `/api/foods/{id}` | Admin | Update food |
| `DELETE` | `/api/foods/{id}` | Admin | Delete food |

### Movies

Same pattern as Foods, filter: `?genreId=`

### Books

Same pattern as Foods, filter: `?genreId=`

### Cuisines

| Method | Endpoint | Role |
|--------|----------|------|
| `GET` | `/api/cuisines` | User+ |
| `GET` | `/api/cuisines/{id}` | User+ |
| `POST` | `/api/cuisines` | Admin |
| `PUT` | `/api/cuisines/{id}` | Admin |
| `DELETE` | `/api/cuisines/{id}` | Admin |

### Genres

Same pattern as Cuisines.

### Comments

| Method | Endpoint | Role | Description |
|--------|----------|------|-------------|
| `GET` | `/api/comments?itemId=&categoryType=` | User+ | List comments for an item |
| `GET` | `/api/comments/{id}` | User+ | Get comment by ID |
| `POST` | `/api/comments` | User+ | Add a comment |
| `PUT` | `/api/comments/{id}` | User+ | Edit a comment (own only; Admin can edit any) |
| `DELETE` | `/api/comments/{id}` | User+ | Delete a comment (own only; Admin can delete any) |

**Create comment body:**
```json
{
  "content": "Absolutely delicious!",
  "authorName": "Alice",
  "categoryType": 1,
  "itemId": "food-guid-here"
}
```

`categoryType`: `1` = Food, `2` = Movie, `3` = Book

### Ratings

| Method | Endpoint | Role | Description |
|--------|----------|------|-------------|
| `POST` | `/api/ratings` | User+ | Create or update your rating (1–5) |
| `GET` | `/api/ratings?itemId=&categoryType=` | User+ | Get your own rating for an item |

**Rate an item:**
```json
POST /api/ratings
{
  "categoryType": 2,
  "itemId": "movie-guid-here",
  "value": 4
}
```

- Rating is an **upsert** — submitting again updates your existing rating.
- Every food/movie/book response includes `averageRating` and `totalRatings`.

### Admin

| Method | Endpoint | Role |
|--------|----------|------|
| `GET` | `/api/admin/users` | Admin |
| `POST` | `/api/admin/users/{userId}/assign-role` | Admin |

---

## Food / Movie / Book Response Shape

```json
{
  "id": "...",
  "title": "Ramen",
  "description": "Japanese noodle soup",
  "imageUrl": "https://...",
  "cuisineId": "...",
  "cuisineName": "Japanese",
  "averageRating": 4.25,
  "totalRatings": 8,
  "comments": [
    {
      "id": "...",
      "content": "My favourite!",
      "authorName": "Alice",
      "userId": "...",
      "categoryType": 1,
      "itemId": "...",
      "createdAt": "...",
      "updatedAt": "..."
    }
  ],
  "createdAt": "...",
  "updatedAt": "..."
}
```

---

## Google OAuth Setup

1. Go to [Google Cloud Console](https://console.cloud.google.com) → APIs & Services → Credentials.
2. Create an **OAuth 2.0 Client ID** (type: Web application).
3. Add to **Authorized redirect URIs**:
   - `https://localhost:{port}/api/auth/callback/google`
   - Your production URL when deploying.
4. Copy the **Client ID** and **Client Secret** into `appsettings.json`.

---

## CI / CD

GitHub Actions (`.github/workflows/ci.yml`) runs on every push and pull request to `main` / `develop`:

1. Spins up a PostgreSQL 16 container
2. `dotnet restore`
3. `dotnet build --configuration Release`
4. `dotnet test`

Store sensitive values as **GitHub repository secrets** and reference them in the workflow's `env` block.

---

## Running Migrations

```bash
# Add a new migration after changing entities
dotnet ef migrations add <MigrationName> \
  --project src/Picker.Infrastructure \
  --startup-project src/Picker.API

# Apply to database
dotnet ef database update \
  --project src/Picker.Infrastructure \
  --startup-project src/Picker.API

# Rollback one migration
dotnet ef database update <PreviousMigrationName> \
  --project src/Picker.Infrastructure \
  --startup-project src/Picker.API
```

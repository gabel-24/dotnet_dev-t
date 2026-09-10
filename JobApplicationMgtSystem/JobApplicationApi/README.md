# Job Application Management System

A full-stack application for managing recruitment. Candidates can browse jobs, submit applications, and track their progress. Recruiters can publish jobs, review applications, update application statuses, and manage interview stages.

## Features

- Candidate and recruiter registration and login with JWT authentication.
- Candidate profiles with a headline, skills, and a resume URL.
- Recruiter profiles and company information.
- Job posting creation, editing, deletion, and search by keyword, location, and employment type.
- Paginated job listings and application lists.
- Application tracking with Submitted, Interview, Offer, Rejected, and Withdrawn statuses.
- Interview stage management through the API.
- Separate candidate and recruiter dashboards in the React frontend.

## Technology

| Component | Stack |
| --- | --- |
| API | C#, ASP.NET Core, .NET 9 |
| Persistence | Entity Framework Core 9, MySQL, Pomelo provider |
| Authentication | ASP.NET Core Identity, JWT bearer tokens |
| Mapping and API documentation | AutoMapper, Swagger / Swashbuckle |
| Frontend | React 19, TypeScript, Vite, React Router, Axios |
| Tests | xUnit, Moq, ASP.NET Core integration testing, Node.js test runner |

## Repository structure

```text
JobApplicationApi/
  Controllers/          HTTP endpoints
  Data/                 EF Core database context
  Dtos/                 Request and response models
  Models/               Entities and application statuses
  Repositories/         Database access
  Services/             Business logic, authentication, and error handling
  Migrations/           EF Core database migrations
JobApplicationApi.Tests/
  UnitTests/            Controller and service tests
  IntTests/             HTTP integration tests
Frontend/jobapp-frontend/
  src/                  React application
  tests/                Session handling tests
JobApplicationApi.slnx
```

## Run locally

### Prerequisites

- .NET 9 SDK and runtime for the backend projects (`net9.0`).
- Node.js 24 and npm for the frontend and its TypeScript session tests.
- A running MySQL server and an account that can create or migrate the application database.
- The EF Core CLI matching the project's EF Core version:

  ```bash
  dotnet tool install --global dotnet-ef --version 9.0.19
  ```

Run the following backend commands from the repository root. Shell examples use Bash.

### 1. Configure the API

Set your local database connection and JWT configuration in the terminal where you will run the API:

```bash
export ConnectionStrings__DefaultConnection='Server=localhost;Port=3306;Database=JobAppDb;User=YOUR_DB_USER;Password=YOUR_DB_PASSWORD;'
export Jwt__Key='REPLACE_WITH_A_RANDOM_SECRET_OF_AT_LEAST_32_BYTES'
export Jwt__Issuer='JobApplicationApi'
export Jwt__Audience='JobApplicationApi'
```

Replace the placeholders before continuing. Environment variables override the values in `JobApplicationApi/appsettings.json` and `JobApplicationApi/appsettings.Development.json`. Keep real credentials and signing keys out of committed configuration files.

### 2. Restore dependencies and apply migrations

```bash
dotnet restore JobApplicationApi/JobApplicationApi.csproj
dotnet ef database update --project JobApplicationApi/JobApplicationApi.csproj
```

MySQL must be reachable during migration and API startup because the application detects the database server version automatically. Migrations are not applied automatically at startup.

### 3. Start the API

```bash
dotnet run --project JobApplicationApi/JobApplicationApi.csproj --launch-profile http
```

- API base URL: `http://localhost:5182/api`
- Swagger UI: `http://localhost:5182/swagger`

The `http` launch profile selects the Development environment, where Swagger is enabled. Its configured browser launch path is `todos`; open `/swagger` manually if that path returns 404.

Startup creates the `Admin`, `Recruiter`, and `Candidate` roles. Register users through the frontend or authentication endpoints; startup does not create default user accounts.

### 4. Start the frontend

In a separate terminal:

```bash
cd Frontend/jobapp-frontend
npm install
npm run dev -- --port 5173 --strictPort
```

Open `http://localhost:5173` and register as a candidate or recruiter.

The API allows the frontend origin `http://localhost:5173` in its CORS policy. The frontend API URL is currently set directly in [`axiosClient.ts`](Frontend/jobapp-frontend/src/api/axiosClient.ts). If you change ports or hosts, update that URL and the CORS policy in [`Program.cs`](JobApplicationApi/Program.cs).

## API overview

All paths below are relative to `/api`. Protected requests require an `Authorization: Bearer <token>` header. Resource ownership checks also apply to application and recruiter-managed operations.

| Method | Path | Access / purpose |
| --- | --- | --- |
| POST | `/auth/register/candidate` | Public: register a candidate |
| POST | `/auth/register/recruiter` | Public: register a recruiter |
| POST | `/auth/login` | Public: log in |
| GET | `/candidates/{id}` | Public: candidate profile |
| GET, PUT | `/candidates/me` | Candidate: view or update own profile |
| GET | `/recruiters/{id}` | Public: recruiter profile |
| GET, PUT | `/recruiters/me` | Recruiter: view or update own profile |
| GET | `/jobpostings` | Public: search and browse jobs |
| GET | `/jobpostings/{id}` | Public: job details |
| POST | `/jobpostings` | Recruiter: create a job |
| PUT, DELETE | `/jobpostings/{id}` | Recruiter: edit or delete own job |
| GET | `/recruiters/me/postings` | Recruiter: list own jobs |
| POST | `/jobpostings/{jobPostingId}/applications` | Candidate: apply for a job |
| GET | `/candidates/me/applications` | Candidate: list own applications |
| GET | `/jobpostings/{jobPostingId}/applications` | Recruiter: list applications for own job |
| GET | `/jobapplications/{id}` | Candidate or recruiter: application details |
| PUT | `/jobapplications/{id}/edit` | Recruiter: update application status |
| GET | `/jobapplications/{jobApplicationId}/interview-stages` | Authenticated: view accessible interview stages |
| POST | `/jobapplications/{jobApplicationId}/interview-stages` | Recruiter: add an interview stage |
| PUT, DELETE | `/interview-stages/{id}` | Recruiter: update or delete an interview stage |

List endpoints for jobs, own postings, and applications accept `pageNumber` (default `1`) and `pageSize` (default `10`, maximum `100`). Responses contain `items`, `totalCount`, `pageNumber`, `pageSize`, and `totalPages`.

Job search also accepts `keyword`, `location`, and `employmentType`:

```bash
curl 'http://localhost:5182/api/jobpostings?pageNumber=1&pageSize=10&keyword=developer'
```

Login example:

```bash
curl -X POST 'http://localhost:5182/api/auth/login' \
  -H 'Content-Type: application/json' \
  -d '{"email":"candidate@example.com","password":"YourPassword123"}'
```

Registration and login return `token`, `userId`, `userName`, and `role`. Tokens expire after two hours. Use the returned token for protected requests:

```bash
curl 'http://localhost:5182/api/candidates/me' \
  -H 'Authorization: Bearer YOUR_TOKEN'
```

See Swagger for request schemas and response models.

## Build and test

Build the API:

```bash
dotnet build JobApplicationApi/JobApplicationApi.csproj
```

The backend test project includes unit and integration tests. Integration tests use a real MySQL database named `job_application_test_db`, with connection details currently hardcoded in [`CustomWebApplicationFactory.cs`](JobApplicationApi.Tests/CustomWebApplicationFactory.cs). Set those details for your local test server and apply migrations to that separate database before running the full suite:

```bash
ConnectionStrings__DefaultConnection='Server=localhost;Port=3306;Database=job_application_test_db;User=YOUR_TEST_DB_USER;Password=YOUR_TEST_DB_PASSWORD;' \
  dotnet ef database update --project JobApplicationApi/JobApplicationApi.csproj

dotnet test JobApplicationApi.Tests/JobApplicationApi.Tests.csproj
```

Use a dedicated test database: integration tests create and modify records, and the test factory does not automatically migrate or reset the database.

Run frontend checks from `Frontend/jobapp-frontend`:

```bash
npm run lint
npm run build
node --test tests/session.test.mjs
```

`npm run build` writes the frontend output to `dist/`. `npm run preview` previews that build locally; API access still uses the configured backend URL and CORS policy.

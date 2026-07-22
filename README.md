# Almentor Task Management API

A production-ready **Task Management REST API** built with **ASP.NET Core (.NET 10)** and **SQL Server**, following **Onion Architecture** with the **Unit of Work**, **Generic Repository**, and **Specification** patterns.

---

## Table of Contents

- [Architecture](#architecture)
- [Setup Instructions](#setup-instructions)
- [API Documentation](#api-documentation)
- [Schema Explanation](#schema-explanation)
- [Test Instructions](#test-instructions)

---

## Architecture

This project follows **Onion Architecture** (a.k.a. Clean/Ports & Adapters Architecture). Dependencies always point **inward**, toward the Domain — outer layers depend on inner layers, never the reverse.

```
                ┌─────────────────────────────────────┐
                │           Almentor.API               │  ← Entry point, DI wiring,
                │   (Program.cs, Middlewares, Startup)  │    middlewares, hosting
                └───────────────┬───────────────────────┘
                                │
                ┌───────────────▼───────────────────────┐
                │        Almentor.Presentation           │  ← Controllers
                └───────────────┬───────────────────────┘
                                │
                ┌───────────────▼───────────────────────┐
                │  Almentor.Services / Services.Abstraction │ ← Business logic,
                │      (ProjectService, TaskService)      │   validation, exceptions
                └───────────────┬───────────────────────┘
                                │
                ┌───────────────▼───────────────────────┐
                │          Almentor.Domain                │ ← Entities, Enums,
                │  (Entities, Enums, Contracts/Interfaces) │   Repository & UoW contracts
                └───────────────▲───────────────────────┘
                                │ implements
                ┌───────────────┴───────────────────────┐
                │         Almentor.Presistence             │ ← EF Core, DbContext,
                │ (DbContext, Repositories, Migrations)    │   Generic Repository, UoW
                └───────────────────────────────────────┘

                Almentor.Shared → DTOs, Query Params, Pagination (referenced across layers)
```

**Projects in the solution:**

| Project | Responsibility |
|---|---|
| `Almentor.Domain` | Core entities (`Project`, `TaskItem`), enums (`TaskStatus`, `TaskPriority`), and the **contracts** (`IGenericRepository`, `IUnitOfWork`, `ISpecifications`) — the heart of the Onion, with zero external dependencies. |
| `Almentor.Services.Abstraction` | Interfaces for application services (`IProjectService`, `ITaskService`). |
| `Almentor.Services` | Business logic implementation, validation rules, custom exceptions, AutoMapper profiles, and the **Specification** classes used for filtering/sorting/pagination. |
| `Almentor.Presistence` | EF Core `ApplicationDbContext`, entity configurations, migrations, and the concrete implementations of `GenericRepository` and `UnitOfWork`. |
| `Almentor.Presentation` | ASP.NET Core controllers (`ProjectsController`, `TasksController`). |
| `Almentor.Shared` | DTOs, query-parameter classes, and the generic `PaginatedResult<T>` — shared across layers without violating the dependency direction. |
| `Almentor.API` | Composition root: `Program.cs`, DI registration, exception-handling middleware, and app startup. |
| `AlmentorTask.Tests` | xUnit unit and integration tests. |

### Design patterns used

- **Onion Architecture** — the Domain layer has no dependency on EF Core, ASP.NET, or any infrastructure concern. Persistence depends on Domain (implements its contracts), not the other way around. This keeps business rules testable and swappable from the database/framework.
- **Unit of Work (`IUnitOfWork`)** — coordinates repository instances against a single `DbContext` and commits all changes in one `SaveChangesAsync()` call, guaranteeing that operations spanning multiple entities (e.g., creating a task under a project) are atomic.
- **Generic Repository (`IGenericRepository<TEntity>`)** — a single reusable repository implementation (`GenericRepository<TEntity>`) provides CRUD operations for any entity, avoiding duplicated data-access code for `Project` and `TaskItem`.
- **Specification Pattern (`ISpecifications<TEntity>`)** — encapsulates query logic (filtering criteria, includes, ordering, pagination) into composable, reusable objects (e.g., `ProjectSpecifications`, `TaskSpecifications`, `TaskWithCountSpecification`) that are evaluated by a single `SpecificationEvaluator`. This keeps filtering/sorting/searching/pagination logic out of controllers and services, and out of the repository itself, avoiding repository method explosion.

---

## Setup Instructions

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server (LocalDB, Express, or a full instance) — or adjust the connection string to any SQL Server-compatible instance
- (Optional) EF Core CLI tools: `dotnet tool install --global dotnet-ef`

### 1. Clone the repository

```bash
git clone https://github.com/AhmedAsemAli/AlmentorTask.API.git
cd AlmentorTask.API
git checkout Dev
```

### 2. Configure the database connection

Edit `Almentor.API/appsettings.Development.json` (or `appsettings.json` for production) and set your SQL Server connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=AlmentorDB;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

> Using a SQL Server login instead of Windows auth? Use:
> `Server=YOUR_SERVER;Database=AlmentorDB;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True`

### 3. Restore dependencies

```bash
dotnet restore
```

### 4. Apply migrations (creates the database schema)

```bash
dotnet ef database update --project Almentor.Presistence --startup-project Almentor.API
```

### 5. Run the app

```bash
dotnet run --project Almentor.API
```

The API will start on the URL shown in the console (e.g., `https://localhost:xxxx`). In development, the OpenAPI document is available at `/openapi/v1.json`.

---

## API Documentation

Base route: `/api`

### Projects

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/projects` | Create a project |
| `GET` | `/api/projects` | List all projects (paginated, searchable, sortable) |
| `GET` | `/api/projects/{id}` | Get a single project |
| `PUT` | `/api/projects/{id}` | Update a project |
| `DELETE` | `/api/projects/{id}` | Delete a project (cascade deletes its tasks) |
| `POST` | `/api/projects/{projectId}/tasks` | Create a task under a project |
| `GET` | `/api/projects/{projectId}/tasks` | List tasks for a project (paginated, filterable, sortable) |

### Tasks

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/tasks` | List all tasks across all projects (paginated, filterable, sortable, searchable) — includes each task's project name |
| `GET` | `/api/tasks/{id}` | Get a single task |
| `PUT` | `/api/tasks/{id}` | Update a task |
| `DELETE` | `/api/tasks/{id}` | Delete a task |

### Query parameters (task list endpoints)

| Param | Type | Description |
|---|---|---|
| `status` | string (`todo`\|`in_progress`\|`done`) | Filter by status |
| `priority` | string (`low`\|`medium`\|`high`) | Filter by priority |
| `due_date_from` | date | Lower bound for due date |
| `due_date_to` | date | Upper bound for due date |
| `q` | string | Partial, case-insensitive match across title/description |
| `sort` | enum | `due_date_Asc`, `due_date_Desc`, `priority_Asc`, `priority_Desc`, `created_at_Asc`, `created_at_Desc` |
| `PageIndex` | int | Page number (default `1`) |
| `PageSize` | int | Items per page (default `5`, max `10`) |

### Query parameters (project list endpoint)

| Param | Type | Description |
|---|---|---|
| `search` | string | Search by project name |
| `sort` | enum | `NameAsc`, `NameDes` |
| `PageIndex` | int | Page number (default `1`) |
| `PageSize` | int | Items per page (default `5`, max `10`) |

### Request / Response Examples

#### Create a project

```
POST /api/projects
Content-Type: application/json

{
  "name": "Website Redesign",
  "description": "Revamp the marketing site"
}
```

```json
// 201 Created
{
  "id": 1,
  "name": "Website Redesign",
  "description": "Revamp the marketing site",
  "createdAt": "2026-07-22T10:00:00Z",
  "updatedAt": "2026-07-22T10:00:00Z"
}
```

#### Create a task under a project

```
POST /api/projects/1/tasks
Content-Type: application/json

{
  "title": "Design landing page",
  "description": "Create hi-fi mockups",
  "status": "todo",
  "priority": "high",
  "dueDate": "2026-08-01T00:00:00Z"
}
```

```json
// 201 Created
{
  "id": 10,
  "projectId": 1,
  "projectName": "Website Redesign",
  "title": "Design landing page",
  "description": "Create hi-fi mockups",
  "status": "todo",
  "priority": "high",
  "dueDate": "2026-08-01T00:00:00Z",
  "createdAt": "2026-07-22T10:05:00Z",
  "updatedAt": "2026-07-22T10:05:00Z"
}
```

#### List tasks (filtered, paginated)

```
GET /api/tasks?status=todo&priority=high&sort=due_date_Asc&PageIndex=1&PageSize=5
```

```json
// 200 OK
// pageSize = requested page size, count = total matching records (across all pages)
{
  "pageIndex": 1,
  "pageSize": 5,
  "count": 12,
  "data": [
    {
      "id": 10,
      "projectId": 1,
      "projectName": "Website Redesign",
      "title": "Design landing page",
      "description": "Create hi-fi mockups",
      "status": "todo",
      "priority": "high",
      "dueDate": "2026-08-01T00:00:00Z",
      "createdAt": "2026-07-22T10:05:00Z",
      "updatedAt": "2026-07-22T10:05:00Z"
    }
  ]
}
```

#### Search tasks

```
GET /api/tasks?q=landing&PageIndex=1&PageSize=5
```

Matches are case-insensitive and checked against both `title` and `description`.

#### Error responses

Errors follow the standard **`ProblemDetails`** shape, produced by a centralized exception-handling middleware:

```json
// 404 Not Found — invalid foreign key / missing resource
{
  "title": "An Unexpected error occured",
  "status": 404,
  "detail": "Project with Id:99 is not found ",
  "instance": "/api/projects/99"
}
```

```json
// 400 Bad Request — business rule violation
{
  "title": "An Unexpected error occured",
  "status": 400,
  "detail": "Due date cannot be in the past.",
  "instance": "/api/tasks/10"
}
```

```json
// 400 Bad Request — model validation failure (e.g., missing required field)
{
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Name": ["The Name field is required."]
  }
}
```

---

## Schema Explanation

```
Project (1) ────< (many) TaskItem
```

**`Project`**
- `Id` — primary key, auto-generated.
- `Name` — `nvarchar(150)`, required, with a **unique index** to enforce the "duplicate project names are rejected" business rule at the database level, not just in application code.
- `Description` — optional.
- `CreatedAt` / `UpdatedAt` — timestamps for auditing.

**`TaskItem`**
- `Id` — primary key, auto-generated.
- `ProjectId` — foreign key to `Project`, configured with **`OnDelete(DeleteBehavior.Cascade)`** so deleting a project cascade-deletes all its tasks directly at the database level (in addition to being enforced in the service layer).
- `Title` — `nvarchar(200)`, required.
- `Description` — optional.
- `Status` — enum (`Todo`, `InProgress`, `Done`), stored as a string via `JsonStringEnumConverter` in API responses.
- `Priority` — enum (`Low`, `Medium`, `High`).
- `DueDate` — optional; application layer rejects past dates on create/update.
- `CreatedAt` / `UpdatedAt` — timestamps.

**Indexes** were added on `Status`, `Priority`, `DueDate`, and `CreatedAt` to keep the filter/sort/search operations on `/api/tasks` efficient as the table grows, since those are exactly the columns used in `TaskQueryParams`.

**Why this shape:**
- A one-to-many relationship between `Project` and `TaskItem` directly matches the requirement that "a task must belong to exactly one project."
- Enums are used instead of free-text columns for `Status`/`Priority` to guarantee valid values at the type level, while `JsonStringEnumConverter` keeps the API contract human-readable (`"todo"` instead of `1`).
- Schema changes are managed exclusively through **EF Core Migrations** (`Almentor.Presistence/Data/Migrations`), not manual SQL scripts, so the schema history is versioned and reproducible.

---

## Test Instructions

Tests live in `AlmentorTask.Tests` and use **xUnit** + **FluentAssertions**.

- **Unit tests** (`UnitTests/`) cover core business logic in isolation — due-date validation and status-transition handling (e.g., `Done → Todo`).
- **Integration tests** (`IntegrationTests/`) spin up the full API in-memory via `WebApplicationFactory` with an EF Core **InMemory** database (no real SQL Server needed to run them), covering flows such as:
  1. Create project → add task → mark task done → delete project (cascade verified).
  2. Filter tasks by status and priority.
  3. Search tasks and verify pagination.

### Run all tests

```bash
dotnet test
```

### Run a specific test class

```bash
dotnet test --filter FullyQualifiedName~ApiIntegrationTests
```

### Run with code coverage

```bash
dotnet test --collect:"XPlat Code Coverage"
```

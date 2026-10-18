# Invoice Management REST API & AI Integration

A clean, production-quality ASP.NET Core application and training baseline for building intelligent enterprise business applications with **Microsoft Agent Framework**, **Microsoft.Extensions.AI**, and **Azure.AI.OpenAI**.

## Architectural Highlights

- **Target Framework:** .NET 10 (`net10.0`)
- **Architecture:** Clean Architecture with Domain-Driven Design (DDD) principles
- **CQRS & Dispatching:** In-process lightweight `IDispatcher` with explicit `ICommand` and `IQuery` handlers
- **Functional Error Handling:** Result pattern (`Result`, `Result<TValue>`, `Error`, `ErrorType`) throughout Domain and Application; no business exceptions
- **Persistence Strategy:**
  - **Relational Database:** Microsoft SQL Server (MSSQL / LocalDB)
  - **Writes:** Entity Framework Core with SQL Server (`Microsoft.EntityFrameworkCore.SqlServer`)
  - **Reads:** Dapper high-performance SQL queries with SQL Server (`Microsoft.Data.SqlClient`)
  - **Unit of Work:** Domain-owned `IUnitOfWork` implemented in Infrastructure
  - **Invoice Sequence:** Atomic SQL Server sequence `invoice_number_seq` for duplicate-free `INV-000001` format
- **API Style:** ASP.NET Core Minimal APIs returning standard HTTP codes & RFC 7807 `ProblemDetails`
- **Validation:** FluentValidation with automatic pipeline validation
- **Central Package Management:** Managed centrally via `Directory.Packages.props`

---

## Solution Structure

```text
InvoiceTraining.sln
├── src/
│   ├── InvoiceApp.Domain/             # Entities, Enums, Result pattern, Errors, Repository contracts
│   ├── InvoiceApp.Application/        # CQRS Commands/Queries, Handlers, Validators, DTOs, IInvoiceService
│   ├── InvoiceApp.Infrastructure/     # EF Core DbContext, Dapper Read Repositories, MSSQL Seeder
│   ├── InvoiceApp.Api/                # Minimal APIs, CustomResults, ProblemDetails, OpenAPI/Swagger
│   └── InvoiceApp.Web/                # ASP.NET Core MVC Frontend + Tailwind CSS + AI Chat Assistant
├── tests/
│   ├── InvoiceApp.UnitTests/          # Domain entity & state machine tests, Validator & Handler tests
│   └── InvoiceApp.IntegrationTests/   # WebApplicationFactory API integration tests
├── Directory.Build.props              # Shared build settings
├── Directory.Packages.props           # Central Package Management
└── README.md
```

---

## Domain Aggregate Rules & State Machine

### Entities

1. **Customer:**
   - Unique email address
   - Name required
   - Optional Phone and Address

2. **Invoice:**
   - Generated sequential number: `INV-000001`, `INV-000002`...
   - Associated with an existing Customer
   - Must contain at least one invoice item
   - `DueDate >= IssueDate`
   - Initial status: `Draft`
   - `TotalAmount` calculated from `Sum(Quantity * UnitPrice)` of items (not persisted directly)

3. **InvoiceItem:**
   - Description required
   - `Quantity > 0`
   - `UnitPrice >= 0`
   - `LineTotal = Quantity * UnitPrice` (calculated, unmapped)

### State Machine Transitions

```text
Draft   ──► Sent       ──► Paid
  │           │              ▲
  │           ▼              │
  │         Overdue ─────────┘
  │           │
  ▼           ▼
Cancelled ◄───┘
```

- `Draft -> Sent`, `Draft -> Cancelled`
- `Sent -> Paid`, `Sent -> Overdue`, `Sent -> Cancelled`
- `Overdue -> Paid`, `Overdue -> Cancelled`
- **Deletion Rule:** Only invoices in `Draft` status can be deleted.

---

## AI Integration Training Topics & Curriculum

This repository serves as the reference codebase for hands-on enterprise AI integration training in .NET. The curriculum demonstrates how to build production-ready intelligent business applications by combining clean backend architectures with the modern Microsoft AI ecosystem.

### Frameworks & SDKs Covered

- **Microsoft Agent Framework:** Building autonomous, goal-oriented AI agents, coordinator-worker orchestration patterns, agent delegation, and multi-turn conversational agents.
- **`Microsoft.Extensions.AI`:** The unified, vendor-neutral .NET abstraction layer providing `IChatClient`, middleware pipelines (`ChatClientBuilder`), and standardized primitives across all AI models.
- **`Microsoft.Extensions.AI.OpenAI`:** Official adapter library integrating OpenAI and Azure OpenAI services directly into the `Microsoft.Extensions.AI` abstraction pipeline.
- **`Azure.AI.OpenAI`:** Official Azure OpenAI client SDK for enterprise connectivity, model deployment management, authentication (API Keys & Microsoft Entra ID / Managed Identity), and Azure AI Content Safety.

### Core AI Topics & Competencies Covered

1. **Tool Calling (Function Calling):**
   - Exposing clean Domain/Application services (e.g. `IInvoiceService`) directly to AI agents as strongly typed tools.
   - Declarative schema definitions using `[Description]` attributes on tool methods and parameter models.
   - Autonomous tool selection by the LLM, parameter validation, tool execution, and feeding results back to the model context.
   - Guardrails for business mutations (e.g., updating invoice status or deleting drafts) with validation and authorization checks.

2. **Prompt Engineering & System Directives:**
   - Crafting structured system messages defining agent persona, scope, operational boundaries, and business rules.
   - Grounding techniques to prevent hallucinations by anchoring responses strictly to tool outputs and database records.
   - Few-shot prompting and output formatting guidance for financial summaries and invoice calculations.

3. **Context-Aware Dynamic Grounding:**
   - Injecting active client-side page context (`window.__AI_CONTEXT__` like current `invoiceId`, `customerId`, and page route) directly into conversational requests.
   - Context-sensitive prompt chips and quick action suggestions based on where the user is in the application.

4. **Conversational Memory & State Management:**
   - Multi-turn conversation management across chat sessions.
   - History windowing and pruning strategies to maintain context while keeping token usage within optimal bounds.
   - Handling message roles (`System`, `User`, `Assistant`, `Tool`).

5. **Pipeline Middleware & Observability:**
   - Composing resilient `IChatClient` pipelines using `ChatClientBuilder`.
   - Logging, token consumption tracking, OpenTelemetry activity tracing, and metric collection.
   - Rate limiting, transient error retries, and fallback strategies.

6. **Structured Outputs & Schema Enforcement:**
   - Constraining LLM responses to strict JSON schemas and typed C# records.
   - Currency and financial formatting (e.g., Nigerian Naira `₦`) in generated responses.

7. **Simulation Mode & Testing:**
   - Offline simulation using mock AI services (`IAiAssistantService` / `MockAiAssistantService`) for rapid UI prototyping and deterministic testing without incurring LLM token costs.
   - Unit and integration testing of tool definitions and agent interactions.

---

## AI-Readiness Design (Service & Tool Contracts)

The application layer includes an `IInvoiceService` interface engineered specifically to be exposed as Microsoft Agent Framework AI tools:

```csharp
public interface IInvoiceService
{
    Task<Result<InvoiceDto>> GetInvoiceByNumberAsync(string invoiceNumber, CancellationToken cancellationToken = default);
    Task<Result<InvoiceDto>> GetInvoiceByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<InvoiceListItemDto>>> GetInvoicesByStatusAsync(InvoiceStatus status, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<InvoiceListItemDto>>> GetCustomerInvoicesAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<InvoiceListItemDto>>> GetOverdueInvoicesAsync(CancellationToken cancellationToken = default);
    Task<Result<InvoiceSummaryDto>> GetInvoiceSummaryAsync(CancellationToken cancellationToken = default);
    Task<Result<CustomerSummaryDto>> GetCustomerSummaryAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<Result> UpdateInvoiceStatusAsync(Guid id, InvoiceStatus status, CancellationToken cancellationToken = default);
}
```

- **Zero HTTP Coupling:** Pure C# records and strongly typed models with domain encapsulation.
- **Direct Tool Binding:** Can be registered directly with `Microsoft.Extensions.AI` / `Microsoft Agent Framework` via tool-calling extensions.
- **Functional Result Pattern:** Safe handling of errors, not-found states, and business rule violations without throwing exceptions.

---

## API Endpoints

### Customers (`/api/customers`)

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/customers` | Create new customer (201 Created) |
| `GET` | `/api/customers` | List all customers |
| `GET` | `/api/customers/{id}` | Get customer by ID |
| `PUT` | `/api/customers/{id}` | Update customer details |
| `GET` | `/api/customers/{customerId}/invoices` | List invoices for customer (AI Tool) |
| `GET` | `/api/customers/{customerId}/summary` | Aggregate metrics for customer (AI Tool) |

### Invoices (`/api/invoices`)

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/invoices` | Create draft invoice (201 Created) |
| `GET` | `/api/invoices` | List invoices (filters: status, customerId, fromDate, toDate, min/max amount, search, pagination) |
| `GET` | `/api/invoices/{id}` | Get invoice by ID |
| `GET` | `/api/invoices/by-number/{invoiceNumber}` | Get invoice by number (e.g., `INV-000001`) |
| `PUT` | `/api/invoices/{id}` | Update draft invoice |
| `DELETE` | `/api/invoices/{id}` | Delete draft invoice |
| `PATCH` | `/api/invoices/{id}/status` | Transition invoice status |
| `GET` | `/api/invoices/overdue` | List overdue invoices (AI Tool) |
| `GET` | `/api/invoices/summary` | Global invoice metrics (AI Tool) |

---

## Running Locally

### Prerequisites
- .NET 10 SDK
- Microsoft SQL Server (e.g. LocalDB, SQL Server Express, or standard SQL Server instance)

### Local Configuration & Execution

1. Configure connection string in `src/InvoiceApp.Api/appsettings.json` (defaults to `(localdb)\\mssqllocaldb`):

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=InvoiceDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
```

2. Run the API:

```bash
dotnet run --project src/InvoiceApp.Api
```

- Swagger UI: `https://localhost:7029` or `http://localhost:5231`
- Database schema, atomic sequence (`invoice_number_seq`), and training seed data are automatically initialized on application startup.

---

## Running Tests

Run the complete unit and integration test suite:

```bash
dotnet test
```

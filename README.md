# Invoice Management REST API & AI Multi-Agent Swarm

A production-quality ASP.NET Core application and enterprise reference solution demonstrating modern **Clean Architecture**, **Domain-Driven Design (DDD)**, and intelligent autonomous multi-agent orchestration powered by **Microsoft Agent Framework**, **Microsoft.Extensions.AI**, and **Azure OpenAI**.

---

## Architectural Highlights

### Core Backend Architecture
- **Target Framework:** .NET 10 (`net10.0`)
- **Architecture:** Clean Architecture with strict separation of Domain, Application, Infrastructure, AI, and Presentation layers
- **CQRS & Dispatching:** In-process lightweight `IDispatcher` with explicit `ICommand` and `IQuery` handlers
- **Functional Error Handling:** Result pattern (`Result`, `Result<TValue>`, `Error`, `ErrorType`) throughout Domain and Application; zero business exceptions for flow control
- **Dual Persistence Strategy:**
  - **Relational Database:** Microsoft SQL Server (MSSQL / LocalDB)
  - **Writes:** Entity Framework Core with SQL Server (`Microsoft.EntityFrameworkCore.SqlServer`)
  - **Reads:** Dapper high-performance SQL queries with SQL Server (`Microsoft.Data.SqlClient`)
  - **Unit of Work:** Domain-owned `IUnitOfWork` implemented in Infrastructure
  - **Atomic Sequencing:** Database-level SQL Server sequence `invoice_number_seq` for concurrency-safe sequential formatting (`INV-000001`)
- **API Style:** ASP.NET Core Minimal APIs returning standard HTTP codes & RFC 7807 `ProblemDetails`
- **Validation:** FluentValidation with automatic pipeline validation
- **Central Package Management:** Centralized dependency versioning via `Directory.Packages.props`

### AI & Autonomous Agent Architecture
- **Multi-Agent Swarm:** Dedicated autonomous agents specialized in domain capabilities (Invoice Operations, Customer CRM, Billing Analytics, and Platform Documentation)
- **Supervisor Orchestrator (`AgentCoordinator`):**
  - **LLM Semantic Intent Routing:** Live `IChatClient` supervisor dynamically classifies incoming requests across candidate agent schemas using `Temperature: 0.0f` and token-efficient reasoning.
  - **Sticky Session Multi-Turn Continuity:** Preserves agent affinity across multi-step parameter gathering (slot-filling) turns, preventing inadvertent agent-swapping when users reply with raw emails, phone numbers, or confirmations.
  - **Intent-Shift Detection:** Automatically recognizes explicit topic transitions and transfers control to the appropriate specialized agent.
- **Unified AI Abstractions:** Built on `Microsoft.Extensions.AI` (`IChatClient`, `ChatMessage`, `AITool`) with direct Azure OpenAI integration (`Azure.AI.OpenAI` / `Microsoft.Extensions.AI.OpenAI`)
- **Autonomous Tool Calling:** Strongly typed domain tool bindings (`AITool`) allowing agents to query databases, calculate analytics, mutate invoice statuses, and inspect documentation.
- **Sliding-Window Memory:** Thread-safe conversational session memory (`IAgentMemoryService`) maintaining bounded chronological dialog context per conversation.
- **Automated Evaluations:** Post-generation guardrail evaluation (`IAgentEvaluationService`) auditing responses for policy compliance and hallucination risk before delivery to callers.
- **Zero-Cost Simulation Mode:** Dual-mode architecture with automatic failover to local simulation when Azure OpenAI credentials are unconfigured or unavailable, enabling full offline development.

---

## Solution Structure

```text
InvoiceTraining.sln
├── src/
│   ├── InvoiceApp.Domain/             # Enterprise business core (Entities, Enums, Result pattern, Errors, Repository contracts)
│   ├── InvoiceApp.Application/        # Use cases (CQRS Commands/Queries, Handlers, FluentValidation, DTOs, IInvoiceService)
│   ├── InvoiceApp.Infrastructure/     # Data access (EF Core DbContext, Dapper Read Repositories, MSSQL Seeder, Unit of Work)
│   ├── InvoiceApp.AI/                 # Microsoft Agent Framework multi-agent swarm:
│   │   ├── Agents/                    # Autonomous agents (InvoiceAssistantAgent, CustomerOperationsAgent, AnalyticsAgent, DocumentationAgent, BaseAgent)
│   │   ├── Coordinator/               # Supervisor orchestrator (IAgentCoordinator, AgentCoordinator with LLM semantic & sticky routing)
│   │   ├── Tools/                     # Strongly typed domain AI tools (InvoiceTools, CustomerTools, DocumentationTools)
│   │   ├── Memory/                    # Multi-turn conversation sliding window memory (IAgentMemoryService, AgentMemoryService)
│   │   ├── Evaluations/               # Safety, compliance, and hallucination guardrails (IAgentEvaluationService, AgentEvaluationService)
│   │   ├── Telemetry/                 # Token consumption and execution latency tracking (ITelemetryService, TelemetryService)
│   │   └── Common/                    # Settings (AzureOpenAISettings) and offline simulation toggle (SimulationMode)
│   ├── InvoiceApp.Api/                # Minimal APIs (/api/customers, /api/invoices, /api/ai/chat, OpenAPI/Swagger)
│   └── InvoiceApp.Web/                # ASP.NET Core MVC Frontend + Tailwind CSS + Floating AI Assistant modal/pill
├── tests/
│   ├── InvoiceApp.UnitTests/          # Unit tests: Domain rules, state machine, validators, CQRS handlers, AI tools, LLM routing & sticky sessions
│   └── InvoiceApp.IntegrationTests/   # Integration tests: WebApplicationFactory end-to-end API tests with in-memory database
├── Docs/                              # Platform documentation, coding labs instructor guide, and architectural specifications
├── Directory.Build.props              # Shared build settings across all projects
├── Directory.Packages.props           # Central Package Management (CPM) versions
└── README.md
```

---

## Domain Aggregate Rules & State Machine

### Entities

1. **Customer:**
   - Unique email address across the platform
   - Name required
   - Optional Phone and Address
   - Deletion guard: Customers with linked invoices cannot be deleted

2. **Invoice:**
   - Atomic sequential number generation: `INV-000001`, `INV-000002`...
   - Associated with an existing Customer
   - Must contain at least one invoice line item
   - Invariant: `DueDate >= IssueDate`
   - Initial status: `Draft`
   - `TotalAmount` dynamically computed from `Sum(Quantity * UnitPrice)` of items (not persisted directly)

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
- **Audit Invariant:** Only invoices in `Draft` status can be deleted; non-draft invoices must be transitioned to `Cancelled`.

---

## AI Multi-Agent Swarm (`InvoiceApp.AI`)

The solution includes a multi-agent swarm architecture where a central supervisor (`AgentCoordinator`) orchestrates four specialized autonomous agents:

### Autonomous Agent Specifications

| Agent | Class | Persona & Responsibilities | Bound Domain Tools |
| :--- | :--- | :--- | :--- |
| **Invoice Operations Agent** | `InvoiceAssistantAgent` | Creates itemized invoices, queries details by sequential number, executes status transitions (Draft $\rightarrow$ Sent $\rightarrow$ Paid/Overdue/Cancelled), and deletes drafts. Formats values in Nigerian Naira (`₦`). | `CreateInvoice`, `GetInvoiceByNumber`, `UpdateInvoiceStatus`, `DeleteDraftInvoice`, `GetOverdueInvoices`, `ListInvoices` |
| **Customer Operations Agent** | `CustomerOperationsAgent` | Manages client relationships, onboards new customers, maintains contact info/addresses, searches directory, and aggregates client lifetime values. | `CreateCustomer`, `UpdateCustomer`, `GetCustomerProfile`, `FindCustomer`, `ListCustomers` |
| **Billing Analytics Agent** | `AnalyticsAgent` | Answers high-level business intelligence inquiries, calculates total revenue, outstanding debtor balances, and identifies top debtors. | `GetInvoiceSummary`, `GetCustomerSummary`, `GetOverdueInvoices`, `ListCustomers` |
| **Platform Documentation Agent** | `DocumentationAgent` | Performs Retrieval-Augmented Generation (RAG) by fetching official markdown documentation from `Docs/` to explain business rules, state machine transitions, and workflows. | `ReadDocumentationPage`, `ListDocumentationPages` |

### Supervisor Orchestrator & Semantic Routing

The supervisor orchestrator ([`AgentCoordinator`](file:///c:/Dev/Learning/InvoiceAiApp/src/InvoiceApp.AI/Coordinator/AgentCoordinator.cs)) executes a five-stage pipeline for every interaction:

1. **Context Enrichment:** Loads chronological sliding-window history from [`IAgentMemoryService`](file:///c:/Dev/Learning/InvoiceAiApp/src/InvoiceApp.AI/Memory/AgentMemoryService.cs) and enriches the user prompt with active UI context (e.g. active invoice ID, active customer ID).
2. **Two-Tier Semantic Intent Routing:**
   - **Live LLM Routing:** Uses a lightweight `IChatClient` supervisor call (`Temperature: 0.0f`, `MaxOutputTokens: 30`) that evaluates the user prompt against the swarm's dynamically generated agent catalog.
   - **Sticky Session Multi-Turn Continuity:** When an agent prompts the user with clarifying questions (slot-filling for required tool parameters like name, email, or invoice items), subsequent user answers (e.g., `"john@acme.com"`, `"Yes, proceed"`) stick to the **same agent** rather than falling back to defaults.
   - **Intent-Shift Override:** Detects explicit domain switches (e.g. *"Actually, show me revenue metrics"*) and redirects execution to the new target agent.
   - **Offline / Simulation Fallback:** If offline or in simulation mode, uses a deterministic history-aware regex router.
3. **Autonomous Execution:** Dispatches the request to the chosen agent, which reasons over instructions and autonomously invokes its bound domain tools.
4. **Guardrail Evaluation:** Audits the generated output via [`IAgentEvaluationService`](file:///c:/Dev/Learning/InvoiceAiApp/src/InvoiceApp.AI/Evaluations/AgentEvaluationService.cs), verifying policy compliance and checking for hallucination indicators.
5. **Persistence & Telemetry:** Appends user and assistant turns back to memory store and records execution duration and token usage via [`ITelemetryService`](file:///c:/Dev/Learning/InvoiceAiApp/src/InvoiceApp.AI/Telemetry/TelemetryService.cs).

---

## Frontend Application (`InvoiceApp.Web`)

The web presentation layer is a responsive ASP.NET Core MVC application styled with **Tailwind CSS** (configured with JIT compiler):

- **Dashboard:** Real-time business KPIs (Total Revenue, Paid, Overdue, Draft counts) and status breakdown visual charts.
- **Customer Directory:** Paginated list of customers with contact details, linked invoice tallies, and modal creation/editing.
- **Invoice Ledger:** Itemized invoice list with rich multi-parameter filters (status, customer, date range, amount range, search), status badge indicators, and detail view.
- **Integrated AI Assistant:**
  - Floating interface supporting compact pill mode and expanded modal workspace mode.
  - **Dynamic Page Context Grounding (`window.__AI_CONTEXT__`):** Injects the user's active page, selected customer ID, or viewed invoice number into chat requests.
  - Contextual prompt suggestion chips based on the active agent.
  - Resilient backend client with automatic graceful fallback to local simulation service if the backend API is unreachable.

---

## REST API Endpoints Reference

### Customers (`/api/customers`)

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/customers` | Create new customer profile (201 Created) |
| `GET` | `/api/customers` | List all customers |
| `GET` | `/api/customers/{id}` | Get customer by unique GUID |
| `PUT` | `/api/customers/{id}` | Update customer contact information |
| `GET` | `/api/customers/{customerId}/invoices` | List all invoices linked to customer (AI Tool) |
| `GET` | `/api/customers/{customerId}/summary` | Aggregate financial metrics for customer (AI Tool) |

### Invoices (`/api/invoices`)

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/invoices` | Create new draft invoice with line items (201 Created) |
| `GET` | `/api/invoices` | List invoices with filters (status, customerId, date range, amount range, search, pagination) |
| `GET` | `/api/invoices/{id}` | Get invoice by unique GUID |
| `GET` | `/api/invoices/by-number/{invoiceNumber}` | Look up invoice by sequential number (`INV-000001`) |
| `PUT` | `/api/invoices/{id}` | Update draft invoice details and line items |
| `DELETE` | `/api/invoices/{id}` | Delete draft invoice (enforces Draft-only invariant) |
| `PATCH` | `/api/invoices/{id}/status` | Transition invoice lifecycle status |
| `GET` | `/api/invoices/overdue` | Retrieve overdue invoices list (AI Tool) |
| `GET` | `/api/invoices/summary` | Global revenue, paid, and overdue metrics (AI Tool) |

### AI Assistant Swarm (`/api/ai`)

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/ai/chat` | Dispatches natural language prompt to `IAgentCoordinator`, executes domain tools, evaluates response, and returns response with agent attribution, suggestions, confidence score, and latency. |

#### Example AI Chat Request Payload:
```json
{
  "message": "Create invoice for Apex Corp with 10 hours of consulting at 25000",
  "conversationId": "sess-4a8f9b",
  "context": {
    "currentPage": "/invoices",
    "activeCustomerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
  }
}
```

#### Example AI Chat Response:
```json
{
  "message": "Invoice INV-000004 has been drafted for Apex Corp with 10 hours of consulting at ₦25,000.00 (Total: ₦250,000.00).",
  "conversationId": "sess-4a8f9b",
  "agentName": "Invoice Operations Agent",
  "isSuccess": true,
  "suggestions": [
    "Show overdue invoices",
    "Mark INV-000004 as Sent",
    "Summarize invoice activity"
  ],
  "confidenceScore": 0.95,
  "durationMs": 412
}
```

---

## Running Locally

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Microsoft SQL Server (LocalDB, SQL Server Express, or standard SQL Server instance)
- Optional: Azure OpenAI resource (or run in built-in zero-cost Simulation Mode)

### Configuration

Connection strings and Azure OpenAI parameters are located in `src/InvoiceApp.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=InvoiceDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  },
  "AzureOpenAI": {
    "Endpoint": "https://your-resource.openai.azure.com/",
    "ApiKey": "YOUR_AZURE_OPENAI_API_KEY",
    "DeploymentName": "gpt-4o",
    "EmbeddingDeploymentName": "text-embedding-3-small"
  }
}
```

> **Note on Simulation Mode:** If `Endpoint` or `ApiKey` contains placeholder values or is left blank, the application automatically engages **Simulation Mode**. All agents, tools, coordinator routing, and frontend chat features will function locally with realistic simulated delays without calling external cloud APIs or incurring token costs.

### Execution

1. **Start the Backend API:**
   ```bash
   dotnet run --project src/InvoiceApp.Api
   ```
   - Swagger / OpenAPI UI: `https://localhost:7029` or `http://localhost:5231`
   - Database tables, atomic sequences, and initial seed data are initialized automatically on startup.

2. **Start the MVC Web Application:**
   ```bash
   dotnet run --project src/InvoiceApp.Web
   ```
   - Web Application: `https://localhost:7214` or `http://localhost:5142`

---

## Running Tests

Run the complete test suite including domain invariants, state machine rules, validation pipelines, Dapper reads, EF Core writes, AI tools, and LLM supervisor routing:

```bash
dotnet test
```

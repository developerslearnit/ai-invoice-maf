# InvoiceApp.Web — ASP.NET Core MVC Frontend

A modern, responsive, enterprise-grade ASP.NET Core MVC frontend designed for developer training on integrating AI with business applications using the **Microsoft Agent Framework**.

---

## 1. Project Purpose

The primary objective of `InvoiceApp.Web` is to provide a clean, production-grade business interface that developers can easily understand and use as the foundation for learning AI agent integration. 

- **Live Demo Quality:** Clean typography, generous spacing, subtle borders, card-based layouts, and responsive tables styled with Tailwind CSS.
- **Lightweight Vanilla JS:** No heavyweight client frameworks (React, Angular, Blazor, HTMX, Alpine.js, or jQuery). Pure vanilla JavaScript with the native Fetch API.
- **AI-Ready UI:** An integrated, floating AI chat assistant that supports compact pill mode, expandable large-screen workspace modal mode, and contextual page awareness.
- **Architectural Separation:** Typed API clients consuming the backend minimal REST API over HTTP with clean ViewModels and strict anti-forgery protection.

---

## 2. Technology Stack

- **Framework:** .NET 10 (`net10.0`), ASP.NET Core MVC
- **Styling:** Tailwind CSS (configured with JIT compiler and zero CDN runtime dependency)
- **UI Architecture:** Razor Views, Razor Partial Views, ViewModels
- **HTTP Client:** `IHttpClientFactory` with typed clients (`CustomerApiClient`, `InvoiceApiClient`, `DashboardApiClient`)
- **JSON Serialization:** `System.Text.Json` (CamelCase naming policy, string enum converters)
- **Client Scripting:** Lightweight modular Vanilla JavaScript (`toasts.js`, `modals.js`, `ai-assistant.js`, `customers.js`, `invoices.js`)
- **Typography:** Inter (via Google Fonts)
- **Currency Format:** Nigerian Naira (`₦450,000.00`)

---

## 3. Folder Structure

```text
src/InvoiceApp.Web/
├── Controllers/
│   ├── DashboardController.cs        # Metric KPIs, status distribution, recent/overdue invoices
│   ├── CustomersController.cs        # Customer directory, details, modal create/edit
│   ├── InvoicesController.cs         # Invoice ledger, filters, modal creation, status transitions, draft deletion
│   └── AiController.cs               # POST /Ai/Chat endpoint consuming IAiAssistantService
├── Models/
│   ├── Api/
│   │   ├── CommonApiModels.cs        # PagedListApiModel<T>, ApiResponse<T>
│   │   ├── CustomerApiModels.cs      # CustomerApiModel, CustomerSummaryApiModel, Create/Update requests
│   │   └── InvoiceApiModels.cs       # InvoiceApiModel, InvoiceStatus enum, InvoiceSummaryApiModel, Filter requests
│   └── ViewModels/
│       ├── DashboardViewModel.cs     # Dashboard KPIs and status percentages
│       ├── CustomerViewModels.cs     # Customer listings, profile details, and form input models
│       ├── InvoiceViewModels.cs      # Filtered invoice ledger, details lifecycle actions, and dynamic line items
│       ├── AiChatViewModels.cs       # AiChatRequest, AiChatContext, AiChatResponse
│       ├── CurrencyHelper.cs         # Nigerian Naira (₦) formatting helpers
│       └── DateHelper.cs             # Human-readable date formatting (e.g., 15 Oct 2026)
├── Services/
│   ├── ApiClients/
│   │   ├── BaseApiClient.cs          # Base client with resilient error handling & ProblemDetails parsing
│   │   ├── ICustomerApiClient.cs / CustomerApiClient.cs
│   │   ├── IInvoiceApiClient.cs / InvoiceApiClient.cs
│   │   └── IDashboardApiClient.cs / DashboardApiClient.cs
│   └── Ai/
│       ├── IAiAssistantService.cs    # Future-proof AI contract
│       └── MockAiAssistantService.cs # Rich mock service providing contextual responses and prompt suggestions
├── Views/
│   ├── Shared/
│   │   ├── _Layout.cshtml            # Master layout with responsive sidebar and top navigation
│   │   ├── _SharedSidebar.cshtml     # Collapsible desktop sidebar & mobile drawer
│   │   ├── _TopNavigation.cshtml     # Header with title, breadcrumb, and quick action shortcuts
│   │   ├── _AiAssistant.cshtml       # Floating AI indicator pill & expandable chat panel
│   │   ├── _CustomerModal.cshtml     # Create & edit customer dialog
│   │   ├── _CreateInvoiceModal.cshtml# Dynamic line item invoice creation dialog
│   │   ├── _StatusChangeModal.cshtml # Confirm invoice status transitions
│   │   ├── _DeleteInvoiceModal.cshtml# Confirm draft invoice deletions
│   │   ├── _InvoiceStatusBadge.cshtml# Status badge (Draft, Sent, Paid, Overdue, Cancelled)
│   │   └── _ToastContainer.cshtml    # Toast notifications container
│   ├── Dashboard/Index.cshtml
│   ├── Customers/
│   │   ├── Index.cshtml
│   │   └── Details.cshtml
│   └── Invoices/
│       ├── Index.cshtml
│       └── Details.cshtml
├── Styles/
│   └── input.css                     # Tailwind CSS entrypoint
├── wwwroot/
│   ├── css/
│   │   └── output.css                # Precompiled minified Tailwind styles
│   └── js/
│       ├── app.js                    # Mobile drawer and navigation toggles
│       ├── toasts.js                 # Reusable toast manager (Success, Error, Warning, Info)
│       ├── modals.js                 # Accessible modal dialog controller
│       ├── ai-assistant.js           # AI chat experience (compact, expanded workspace, context passing)
│       ├── customers.js              # Customer AJAX CRUD interactions
│       └── invoices.js               # Dynamic invoice item math, filters, and status transitions
├── package.json                      # Tailwind CLI build scripts
└── tailwind.config.js                # Tailwind content scanner configuration
```

---

## 4. Backend API Configuration

The MVC application communicates with the backend REST API (`InvoiceApp.Api`). Configure the base URL in [appsettings.json](file:///c:/Dev/Learning/InvoiceAiApp/src/InvoiceApp.Web/appsettings.json) or [appsettings.Development.json](file:///c:/Dev/Learning/InvoiceAiApp/src/InvoiceApp.Web/appsettings.Development.json):

```json
{
  "ApiSettings": {
    "BaseUrl": "http://localhost:5231"
  }
}
```

The application automatically handles connection errors, timeouts, and API unavailability with user-friendly notices and retry actions.

---

## 5. Running the Application

### Option A: Run Both Backend API and MVC Frontend

1. **Start the API** (in terminal 1):
   ```bash
   dotnet run --project src/InvoiceApp.Api
   ```
   *Runs on `http://localhost:5231`.*

2. **Start the MVC Frontend** (in terminal 2):
   ```bash
   dotnet run --project src/InvoiceApp.Web
   ```
   *Runs on `http://localhost:5100`.*

---

## 6. Tailwind CSS Build & Watch

Tailwind is compiled using standard npm scripts:

```bash
cd src/InvoiceApp.Web

# Compile minified CSS for production
npm run css:build

# Watch mode during frontend development
npm run css:watch
```

The generated stylesheet is saved directly to `wwwroot/css/output.css`.

---

## 7. MVC Routes

| Route | Description |
|---|---|
| `GET /` or `GET /Dashboard` | Executive dashboard with metrics, status bar, recent invoices, and overdue alerts |
| `GET /Customers` | Customer directory with search and "+ Add Customer" modal |
| `GET /Customers/{id}` | Customer details with metric cards and complete billing ledger |
| `GET /Customers/Get/{id}` | JSON endpoint for pre-populating customer edit modal |
| `POST /Customers` | AJAX endpoint for creating a new customer |
| `POST /Customers/Edit/{id}` | AJAX endpoint for updating an existing customer |
| `GET /Invoices` | Filterable invoice ledger with pagination and "+ New Invoice" modal |
| `GET /Invoices/{id}` | Invoice details with itemized breakdown and status transition actions |
| `POST /Invoices` | AJAX endpoint for creating invoices with dynamic line items |
| `POST /Invoices/{id}/status` | AJAX endpoint for transitioning invoice lifecycle states |
| `POST /Invoices/{id}/delete` | AJAX endpoint for permanently deleting draft invoices |
| `POST /Ai/Chat` | AJAX endpoint handling AI chat conversations with page context |

---

## 8. Typed API Client Architecture

All outgoing backend requests flow through dedicated typed clients registered with `IHttpClientFactory`:

- `ICustomerApiClient` & `CustomerApiClient`: Handles customer queries and mutations.
- `IInvoiceApiClient` & `InvoiceApiClient`: Handles invoice queries, filters, status transitions, and draft deletions.
- `IDashboardApiClient` & `DashboardApiClient`: Aggregates KPI metrics and recent/overdue lists in parallel.

Controllers remain thin and do not execute raw HTTP calls or serialization logic.

---

## 9. AI Assistant UI Architecture

The AI Assistant is accessible globally from every page:

1. **Floating Indicator Pill (Bottom-Right):**
   - Displays a prominent card:
     ```text
     +--------------------------------+
     | ✨  Ask Invoice AI             |
     |     Chat about your invoices   |
     +--------------------------------+
     ```
   - Hover animations and live status indicator.

2. **Compact Chat Mode:**
   - Sized at `420px × 600px` (or responsive mobile width).
   - Provides quick questions, suggested prompt chips, and message history.

3. **Expandable Workspace Mode:**
   - Clicking **Expand** enlarges the chat to `75-80vw × 88vh`.
   - Allows trainees to view complex responses, tables, and agent reasoning without leaving the page.
   - Preserves all conversation history and active state seamlessly during expansion and collapse.

4. **Context-Aware Prompts:**
   Pages inject structured metadata into `window.__AI_CONTEXT__`. For example, on the Invoice Details view:
   ```javascript
   window.__AI_CONTEXT__ = {
       page: "InvoiceDetails",
       invoiceId: "b8c3f9a1-...",
       invoiceNumber: "INV-000001",
       customerId: "7d2e1b4c-...",
       customerName: "Contoso Ltd"
   };
   ```
   When the user asks *"Summarize this invoice"*, the backend receives this context and delivers invoice-specific answers.

---

## 10. AI Integration Training & Microsoft Agent Framework

> [!NOTE]
> **Training Phase Status:**  
> The frontend provides a fully functional, context-aware AI assistant interface. In Phase 1, `MockAiAssistantService` simulates realistic agent interactions, suggested prompt chips, and overdue invoice inquiries without external cloud dependencies.
>
> In Phase 2, the assistant connects to live AI agents built with the modern Microsoft AI stack:
> - **Microsoft Agent Framework:** Multi-agent coordinator and sub-agent task routing.
> - **`Microsoft.Extensions.AI` & `Microsoft.Extensions.AI.OpenAI`:** Standardized `IChatClient` pipelines, streaming, and telemetry.
> - **`Azure.AI.OpenAI`:** Enterprise Azure OpenAI connectivity, models, and safety guardrails.
> - **Tool Calling:** Exposing backend services (`IInvoiceService`) for autonomous data queries and status updates.
> - **Prompt Engineering:** Tailored system personas, ground truth enforcement, and structured output formatting.

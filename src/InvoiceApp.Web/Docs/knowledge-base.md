# InvoiceAI — Complete Platform Knowledge Base

This document serves as the comprehensive knowledge base and grounding reference for **InvoiceAI**, an enterprise billing, customer management, and AI-powered invoicing platform built with ASP.NET Core (.NET 10), Tailwind CSS, Clean Architecture, and Microsoft Agent Framework AI integration.

---

## 1. System Overview & Architecture

### What is InvoiceAI?
InvoiceAI is a modern AI-powered business invoicing platform designed to manage the entire billing lifecycle — from customer directory onboarding and multi-item invoice generation to state machine lifecycle tracking, debt collection analytics, and autonomous AI-assisted operations.

### Key Architectural Standards
- **Framework:** .NET 10 (`net10.0`), ASP.NET Core MVC Frontend & Minimal APIs Backend.
- **Architecture:** Clean Architecture with Domain-Driven Design (DDD) & CQRS in-process dispatching.
- **Persistence:** Microsoft SQL Server with EF Core (writes), Dapper (high-performance reads), and atomic sequence generator (`invoice_number_seq`).
- **Currency:** Nigerian Naira (`₦`), formatted with standard decimal formatting (e.g. `₦450,000.00`).
- **Date Format:** Standard Day-Month-Year format (e.g., `15 Oct 2026`).
- **AI Integration:** Microsoft Agent Framework, `Microsoft.Extensions.AI`, and Azure OpenAI with autonomous tool bindings.

---

## 2. Customer Management

Every invoice in InvoiceAI must be associated with an existing customer record.

### Creating a Customer
1. Navigate to **Customers** (`/Customers`) in the main navigation.
2. Click the **+ Add Customer** button at the top right.
3. Complete the modal form fields:
   - **Full Name** *(Required)*: Client or company name (max 200 characters).
   - **Email Address** *(Required)*: Official billing contact email. Must be a valid email format and strictly **unique** across all customers.
   - **Phone Number** *(Optional)*: Contact telephone number (max 50 characters).
   - **Billing Address** *(Optional)*: Registered street or mailing address (max 500 characters). Displayed on generated customer invoices.
4. Click **Save Customer**.

### Customer Directory & Search
- Located at `/Customers`.
- Search customers in real-time by entering text in the search bar (matches name or email).
- Displays quick cards with customer contact details, invoice counts, total invoiced value, and outstanding balance.

### Customer Details View (`/Customers/{id}`)
Clicking a customer row opens their complete financial profile:
- **Contact Details:** Email, phone number, and physical billing address.
- **KPI Metrics:**
  - **Total Invoiced:** Cumulative value of all invoices issued to this customer.
  - **Total Paid:** Total cash collected from this customer.
  - **Outstanding Balance:** Total sum of unpaid invoices (`Sent` and `Overdue`).
  - **Total Invoices:** Count of all invoices across all statuses.
- **Customer Ledger:** Itemized list of every invoice issued to this customer with issue dates, due dates, statuses, amounts, and direct view links.

### Updating Customer Information
- Click **Edit Customer** from either the directory card or details page.
- Modify name, email, phone, or address.
- **Gotcha:** If you change an email to one that collides with another registered customer, the system will return a validation error.

### Customer Deletion Rules
- **Safeguard:** Customers cannot be deleted if any invoices (even drafts or cancelled invoices) are linked to their account. This preserves financial ledger consistency.

---

## 3. Creating Invoices

Invoices in InvoiceAI support multi-item line item billing with automatic tax and line total calculations.

### Steps to Create an Invoice
1. Click **+ New Invoice** from the Invoices ledger (`/Invoices`) or the Dashboard quick action.
2. In the invoice modal, configure the header:
   - **Customer** *(Required)*: Select the client from the searchable dropdown.
   - **Issue Date** *(Required)*: Date invoice is issued (defaults to today).
   - **Due Date** *(Required)*: Payment deadline. Must be greater than or equal to Issue Date (`DueDate >= IssueDate`).
   - **Notes / Terms** *(Optional)*: Bank details, payment terms, or delivery notes.
3. Add **Line Items** (at least one item required):
   - Click **+ Add Item** to append rows.
   - **Description** *(Required)*: Details of product or service (e.g., "UI/UX Redesign — Sprint 1").
   - **Quantity** *(Required)*: Positive integer greater than 0 (`> 0`).
   - **Unit Price** *(Required)*: Price per unit in Naira (`₦`). Must be non-negative (`>= 0`).
   - **Line Total**: Computed automatically in real-time as `Quantity * UnitPrice`.
   - Use the red **Remove** icon to delete any line item.
4. Review the auto-computed **Invoice Total Amount** (`Sum(LineTotal)`).
5. Click **Create Invoice**.

### Sequential Invoice Numbering
- Every invoice receives an automatic, duplicate-free sequential number formatted as `INV-000001`, `INV-000002`, `INV-000003`, etc.
- Numbering is generated by an atomic SQL sequence (`invoice_number_seq`).

### Initial Status
- Invoices are always created in **Draft** status.
- Draft invoices can be updated, modified, or deleted without affecting financial reporting.
- To make an invoice billable, transition it to **Sent**.

---

## 4. Viewing & Filtering Invoices

The **Invoices** page (`/Invoices`) serves as the master billing ledger.

### Ledger Columns
- **Invoice #:** Clickable link to invoice details (e.g., `INV-000001`).
- **Customer:** Client name and email address.
- **Issue Date:** Formal creation date.
- **Due Date:** Payment deadline (flagged with alert indicators if overdue).
- **Status Badge:** Visual indicator (`Draft`, `Sent`, `Paid`, `Overdue`, `Cancelled`).
- **Total Amount:** Grand total formatted in Nigerian Naira (`₦`).
- **Actions:** Quick links to View Details, change status, or delete drafts.

### Multi-Criteria Filters
- **Search Query:** Free text search matching invoice numbers, customer names, or item descriptions.
- **Status Filter:** Filter by specific status (`All`, `Draft`, `Sent`, `Paid`, `Overdue`, `Cancelled`).
- **Customer Filter:** View all invoices for a single customer.
- **Date Range Filters:** Filter by `From Date` and `To Date`.
- **Amount Range Filters:** Filter by `Min Amount` and `Max Amount`.
- **Pagination:** Clean page navigation controls.

### Invoice Details Page (`/Invoices/{id}`)
- Full invoice header and status timeline.
- Customer contact and billing address card.
- Comprehensive line items table (Item description, quantity, unit price, line total, and grand total).
- Notes and terms.
- Lifecycle management action buttons (`Mark as Sent`, `Mark as Paid`, `Mark as Overdue`, `Cancel Invoice`, `Delete Draft`).

---

## 5. Invoice Lifecycle & State Machine

InvoiceAI enforces a strict state machine to prevent accounting errors:

```text
Draft   ──► Sent       ──► Paid
  │           │              ▲
  │           ▼              │
  │         Overdue ─────────┘
  │           │
  ▼           ▼
Cancelled ◄───┘
```

### Valid Lifecycle Transitions
- `Draft -> Sent`: Dispatched to customer; begins payment window; enters outstanding debtor metrics.
- `Draft -> Cancelled`: Discard draft invoice.
- `Sent -> Paid`: Client settles payment in full.
- `Sent -> Overdue`: Due date passes without settlement.
- `Sent -> Cancelled`: Void sent invoice (e.g., disputed or cancelled order).
- `Overdue -> Paid`: Late payment settled in full.
- `Overdue -> Cancelled`: Overdue debt written off.

### Critical Deletion & Audit Rules
- **Only Drafts Can Be Deleted:** The system strictly permits deletion **only** when an invoice is in `Draft` status.
- **Sent, Paid, and Overdue Invoices Cannot Be Deleted:** Once an invoice is issued, deleting it is prohibited to preserve tax ledgers and sequential numbering audit trails. If an issued invoice needs to be voided, it must be transitioned to **Cancelled**, never deleted.
- **No Backward Transitions:** Invoices cannot be moved backwards (e.g., `Paid -> Draft` is forbidden).

---

## 6. Executive Dashboard & Analytics

The **Dashboard** (`/` or `/Dashboard`) displays real-time financial health:

### Core KPI Cards
1. **Total Invoiced:** Gross cumulative value of all non-cancelled invoices.
2. **Total Collected (Revenue):** Cash collected from `Paid` invoices.
3. **Outstanding Balance:** Total balance pending collection (`Sent` + `Overdue`).
4. **Overdue Amount:** Total urgent debt past due date.

### Status Distribution
- Visual breakdown displaying counts and percentage share across `Draft`, `Sent`, `Paid`, `Overdue`, and `Cancelled`.

### Alerts & Tables
- **Recent Invoices:** Quick-access table of latest billing transactions.
- **Urgent Overdue Invoices Alert:** Callout highlighting delinquent accounts, days past due, and overdue amounts with direct follow-up actions.

---

## 7. AI Assistant & Knowledge Chat

The integrated AI Assistant provides conversational intelligence on every screen:

### User Experience Modes
- **Floating Indicator Pill (Bottom-Right):** Subtle, hover-animated pill with status indicator (`✨ Ask Invoice AI`).
- **Compact Chat Panel:** 420px panel for quick questions and suggested prompts.
- **Expanded Workspace Modal:** Full `80vw × 88vh` workspace for complex analytics, large tabular breakdowns, and agent reasoning.

### Context Awareness (`window.__AI_CONTEXT__`)
- Automatically captures active page and entity IDs (e.g., `invoiceId`, `customerId`).
- Allows natural contextual questions like:
  - *"Summarize this invoice"*
  - *"What is this customer's balance?"*
  - *"Who owes the most money?"*

### Supported Agent Operations & Task Execution (Tool Calling)
The AI Assistant is bound to strongly typed domain tools capable of executing actions across the platform:
- **`CreateCustomer`**: Onboards a new customer with Name, unique Email, Phone, and Address.
- **`UpdateCustomer`**: Modifies an existing client's contact information.
- **`CreateInvoice`**: Generates a new invoice with dynamic line items (description, quantity, unit price) and sequential number.
- **`UpdateInvoiceStatus`**: Transitions invoice lifecycle states (`Sent`, `Paid`, `Overdue`, `Cancelled`) obeying the state machine.
- **`DeleteDraftInvoice`**: Permanently deletes an invoice (strictly allowed only for `Draft` invoices).
- **`GetInvoiceByNumber`**: Pulls itemized details for any invoice number (e.g., `INV-000001`).
- **`GetOverdueInvoices`**: Gathers all late invoices and computes total overdue debt.
- **`GetInvoiceSummary`**: Returns global KPIs across the entire system.
- **`GetCustomerProfile`**: Aggregates customer lifetime value, paid amounts, and outstanding balances.
- **`ListInvoices` / `ListCustomers`**: Returns filterable lists of invoices and customer directories.
- **`ReadDocumentationPage`**: Retrieves platform documentation pages to answer UI and workflow questions.

---

## 8. Summary of Platform Constraints & Gotchas

| Feature / Rule | Constraint / Behavior |
|---|---|
| **Invoice Numbering** | Auto-generated sequential format `INV-000001`. Cannot be manually edited or duplicated. |
| **Customer Email** | Must be unique across all customer records. Collsions return a validation error. |
| **Customer Deletion** | Prohibited if customer has any linked invoices. |
| **Invoice Due Date** | Must be on or after the Issue Date (`DueDate >= IssueDate`). |
| **Line Items** | At least 1 item is required per invoice. Quantity must be `> 0`. Unit Price must be `>= 0`. |
| **Invoice Deletion** | **Only permitted for Draft invoices.** Sent, Paid, and Overdue invoices must be **Cancelled** instead. |
| **Currency** | Nigerian Naira (`₦`). Quantities and prices entered as raw decimal numbers without symbols. |
| **Status Reversal** | Status machine cannot transition backward (e.g., `Paid` cannot be reverted to `Draft`). |

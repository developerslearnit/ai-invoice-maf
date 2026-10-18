# Getting Started

Welcome to **InvoiceAI** — an enterprise-grade billing, customer management, and AI-powered invoicing platform built with ASP.NET Core (.NET 10), Tailwind CSS, Clean Architecture, and integrated AI assistant capabilities.

InvoiceAI is designed to give business owners, finance teams, and account managers complete control over customer accounts, itemized invoice creation, lifecycle state tracking, and financial analytics. It also features a built-in AI Assistant capable of answering billing questions, pulling metrics, and explaining system rules.

## What You Can Do with InvoiceAI
- **Manage Customers:** Maintain a centralized directory of customer profiles with contact info, billing addresses, and lifetime billing summaries.
- **Create Invoices with Line Items:** Generate itemized invoices with dynamic line items, automated sequential invoice numbering (`INV-000001`), and automatic tax/line total calculations.
- **Track Lifecycle States:** Transition invoices through a strict state machine (`Draft`, `Sent`, `Paid`, `Overdue`, `Cancelled`).
- **Audit Trails & Deletion Safeguards:** Protect financial records with audit integrity rules (only `Draft` invoices can be deleted).
- **Executive Dashboard:** Monitor real-time KPIs including Total Invoiced, Collected Revenue, Outstanding Balances, and Overdue Alerts.
- **AI-Powered Knowledge Chat:** Use the floating AI Assistant on any screen to query invoices, analyze overdue debtors, or ask for guidance on how to use the platform.

## Navigation & Core Modules
1. **Dashboard (`/` or `/Dashboard`):** High-level overview of cash flow, status distribution, and urgent overdue invoices.
2. **Customers (`/Customers`):** Complete customer database, searchable directory, and individual customer profiles with billing ledgers.
3. **Invoices (`/Invoices`):** Full invoice ledger with multi-criteria filtering (status, customer, date range, amount range, text search) and detailed itemized views.
4. **AI Assistant (Bottom-Right Widget):** Instant conversational assistant with compact pill view and full-screen expandable workspace modal.

## Currency & Formatting
- **Standard Currency:** Nigerian Naira (`₦`), formatted with commas and two decimal places (e.g., `₦450,000.00`).
- **Standard Date Format:** Day-Month-Year (e.g., `15 Oct 2026`).

## Example Use Cases
- A services company invoicing enterprise clients for software development milestones.
- A retail or wholesale distributor issuing invoices with multiple distinct line items.
- A finance manager checking overdue accounts at the start of the week and asking the AI assistant to summarize debtor balances.

## Things to Know
- All financial numbers are computed from individual line items (`Quantity * UnitPrice`); manual total overrides are disabled to prevent calculation discrepancies.
- Invoices are automatically numbered sequentially using an atomic SQL database sequence to guarantee zero duplicates or missing numbers.
- Deleting an invoice is only permitted while it is in `Draft` status. Once an invoice has been `Sent`, `Paid`, or marked `Overdue`, it cannot be deleted — it must be `Cancelled` to preserve audit history.

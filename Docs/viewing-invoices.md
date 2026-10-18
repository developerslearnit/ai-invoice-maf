# Viewing Invoices

The **Invoices** page (`/Invoices`) is your central billing ledger. It provides a comprehensive, filterable, and searchable table of every invoice in the system.

## The Invoice Ledger Table
Each row in the invoices table displays:
- **Invoice #:** The sequential identifier (e.g., `INV-000001`), clickable to jump directly to its details view.
- **Customer:** The client's name and primary contact email.
- **Issue Date:** The date the invoice was officially generated.
- **Due Date:** The payment deadline. Overdue dates are highlighted with alert indicators.
- **Status:** Color-coded status badge:
  - 🟡 **Draft:** Unsent work in progress.
  - 🔵 **Sent:** Dispatched to customer; awaiting payment.
  - 🟢 **Paid:** Settled in full.
  - 🔴 **Overdue:** Past payment deadline without settlement.
  - ⚪ **Cancelled:** Voided invoice.
- **Total Amount:** The total value of all line items, formatted in Nigerian Naira (`₦`).
- **Actions:** Quick links to view invoice details, update lifecycle status, or delete drafts.

## Searching & Filtering Invoices
The ledger includes a powerful real-time filter toolbar:
- **Search Bar:** Instantly finds invoices by matching against invoice numbers (e.g., `INV-000003`), customer names (e.g., `Contoso`), or line item descriptions.
- **Status Filter:** Filter specifically for `Draft`, `Sent`, `Paid`, `Overdue`, or `Cancelled`.
- **Customer Filter:** Select a specific customer to isolate their billing records.
- **Date Range Filters:** Set **From Date** and **To Date** to view invoices issued within a specific fiscal period or month.
- **Amount Range Filters:** Specify **Min Amount** and **Max Amount** to find high-value invoices.
- **Clear Filters:** Resets all filter controls back to default view.
- **Pagination:** Easily navigate multi-page records with page number buttons.

## Invoice Details Page (`/Invoices/{id}`)
Clicking on any invoice number or **View Details** button opens the detailed view:
1. **Invoice Overview:** Header with invoice number, current status badge, issue date, and due date.
2. **Customer Information:** Full client profile, billing address, email, and contact phone number.
3. **Itemized Line Items Table:**
   - Detailed description of each product or service delivered.
   - Quantity and unit price (`₦`).
   - Line total for each item.
   - Grand Total summary at the footer.
4. **Notes / Terms:** Any special payment instructions or delivery notes entered during creation.
5. **Lifecycle Management Controls:** Action buttons to transition invoice state (`Mark as Sent`, `Mark as Paid`, `Mark as Overdue`, `Cancel Invoice`, or `Delete Draft`).

## Example Use Cases
- Locating all unpaid invoices past their due date to prepare a collections report.
- Searching for a specific invoice number referenced on a bank statement or deposit slip.
- Filtering by a specific client during an account reconciliation meeting.

## Gotchas
- **Draft Exclusion from Debt Metrics:** Draft invoices appear in the ledger, but their values do not count toward outstanding debt until they are marked as **Sent**.
- **Pagination Context:** When filters are active, the pagination displays only matching results; clearing filters restores the full history.
- **Direct Number Lookup:** You can locate an invoice instantly by searching its full number (e.g., `INV-000001`) in the search bar.

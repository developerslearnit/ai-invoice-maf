# Dashboard & Financial Metrics

The **Executive Dashboard** (`/` or `/Dashboard`) is the main landing screen of InvoiceAI. It provides business owners and financial managers with a real-time, consolidated overview of cash flow, billing pipeline health, and urgent collections.

## Key Performance Indicators (KPIs)
At the top of the dashboard are four high-impact metric cards:
1. **Total Invoiced:** The gross value of all non-cancelled invoices issued across the entire platform.
2. **Total Collected (Revenue):** Actual cash collected from settled invoices (`Paid` status).
3. **Outstanding Balance:** Total funds currently awaiting payment from clients (`Sent` and `Overdue` statuses combined).
4. **Overdue Amount:** Critical warning metric showing the exact amount past due date that requires immediate collections action.

## Status Distribution Breakdown
Below the KPI cards is a visual status breakdown displaying the health of your invoicing pipeline:
- Shows the total count and percentage share for each lifecycle state:
  - **Draft** (work in progress)
  - **Sent** (active pending collection)
  - **Paid** (completed revenue)
  - **Overdue** (delinquent accounts)
  - **Cancelled** (voided records)

## Recent Invoices
A live table of the most recent invoices created in the system, showing:
- Invoice number
- Customer name
- Issue date
- Status badge
- Total amount
- Quick link to view details or perform actions.

## Urgent Overdue Invoices Alert
If any invoices are currently in **Overdue** status, a high-priority warning card highlights:
- Delinquent customer names and contact emails.
- Overdue invoice numbers and how many days past due they are.
- Total amount overdue per client.
- Quick buttons to inspect the invoice, contact the customer, or trigger reminder workflows.

## Example Use Cases
- Checking morning financial metrics to see if overnight client payments have arrived.
- Identifying high-risk overdue balances before approving new credit or work orders.
- Reviewing monthly invoice creation volume to gauge business growth.

## Gotchas
- **Real-Time Calculation:** Dashboard metrics are computed directly from live database records; refreshing the page or updating an invoice status instantly updates all cards.
- **Cancelled Invoices Excluded:** Cancelled invoices are excluded from Total Invoiced and Total Outstanding metrics to prevent distortion of real revenue figures.

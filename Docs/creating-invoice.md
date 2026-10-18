# Creating an Invoice

The **Invoices** page (`/Invoices`) and **Dashboard** quick action button are where you create new invoices. In InvoiceAI, invoices support multi-item itemized billing with automated calculation and sequential number generation.

## Steps to Create an Invoice
1. Click **+ New Invoice** from the Invoices page or Dashboard.
2. In the invoice creation modal, configure the header details:
   - **Customer** *(Required)*: Select the client from the searchable customer dropdown. If the customer does not exist yet, you must first create them on the Customers page.
   - **Issue Date** *(Required)*: The date the invoice is issued. Defaults to today's date.
   - **Due Date** *(Required)*: The payment deadline. Must be on or after the Issue Date (`DueDate >= IssueDate`).
   - **Notes / Terms** *(Optional)*: Payment instructions, bank details, or delivery notes.
3. Add **Line Items** (at least one item is required):
   - Click **+ Add Item** to insert additional rows.
   - For each line item, provide:
     - **Description** *(Required)*: Details of the service or product delivered (e.g., "Full-Stack Web Development — Sprint 1").
     - **Quantity** *(Required)*: Number of units or hours. Must be a positive integer greater than 0.
     - **Unit Price** *(Required)*: Price per unit in Nigerian Naira (`₦`). Must be 0 or greater.
   - The system automatically calculates the **Line Total** (`Quantity * UnitPrice`) in real time as you type.
   - To delete an unwanted row, click the red **Remove** icon next to that item.
4. Review the auto-computed **Invoice Total Amount** displayed at the bottom of the form.
5. Click **Create Invoice**.
6. The modal closes, a success toast notification appears, and the new invoice is added to the ledger with status **Draft**.

## Automatic Invoice Numbering
- Every saved invoice receives a guaranteed, duplicate-free sequential number formatted as `INV-000001`, `INV-000002`, `INV-000003`, etc.
- This number is generated atomically by the database sequence (`invoice_number_seq`) to ensure no gaps or overlaps occur even during concurrent operations.

## Initial Status
- Newly created invoices always start in **Draft** status.
- While in **Draft** status:
  - The invoice does not count against the customer's active outstanding debtor balance.
  - You can edit line items, descriptions, and dates.
  - You can safely delete the invoice if it was created by mistake.
  - Once finalized, you transition it to **Sent** to begin the payment collection lifecycle.

## Example Use Cases
- Billing a corporate client for monthly managed cloud support services.
- Creating an itemized estimate with multiple deliverables (design, development, quality assurance).
- Preparing a draft invoice in advance for a future delivery date.

## Gotchas & Validation Rules
- **Customer Required:** You cannot create an invoice without linking it to an existing customer.
- **Due Date Order:** The Due Date cannot be in the past relative to the Issue Date (`DueDate >= IssueDate`).
- **At Least One Item:** You cannot submit an empty invoice; at least one line item with a positive quantity is strictly required.
- **Unit Prices & Quantities:** Quantities must be greater than zero (`> 0`). Unit prices must not be negative (`>= 0`).
- **Currency Entry:** Enter prices as numbers without currency symbols (e.g., `150000` or `150000.50`). The system automatically handles formatting with `₦` and thousand separators.

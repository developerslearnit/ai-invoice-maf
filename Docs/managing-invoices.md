# Managing Invoices & Status Lifecycles

In InvoiceAI, invoices follow a strict state machine to maintain accounting accuracy, prevent invalid transitions, and protect financial audit trails.

## The Invoice State Machine

```text
Draft   ──► Sent       ──► Paid
  │           │              ▲
  │           ▼              │
  │         Overdue ─────────┘
  │           │
  ▼           ▼
Cancelled ◄───┘
```

### Available Statuses
1. **Draft:** Initial creation status. Invoices in draft are considered works in progress and can be modified or permanently deleted.
2. **Sent:** The invoice has been formally issued and dispatched to the customer. Awaiting payment. Outstanding balance is active.
3. **Paid:** The customer has settled the invoice in full. Considered complete.
4. **Overdue:** The payment deadline (`DueDate`) has passed without settlement. Flagged in dashboards and reports for follow-up.
5. **Cancelled:** The invoice has been voided. Cancelling preserves historical numbering and audit records without counting toward revenue or debt.

### Permitted Transitions
- **From Draft:**
  - `Draft -> Sent`: Mark invoice as sent/dispatched to client.
  - `Draft -> Cancelled`: Void draft without dispatching.
- **From Sent:**
  - `Sent -> Paid`: Mark invoice as paid upon receiving payment.
  - `Sent -> Overdue`: Flag invoice as overdue if payment deadline passes.
  - `Sent -> Cancelled`: Void sent invoice (e.g., cancelled contract or billing dispute).
- **From Overdue:**
  - `Overdue -> Paid`: Settle an overdue invoice upon receiving late payment.
  - `Overdue -> Cancelled`: Write off or void overdue balance.

## How to Change an Invoice's Status
1. Navigate to the **Invoice Details** page (`/Invoices/{id}`) or use the action menu in the **Invoices Ledger**.
2. Click the desired action button (e.g., **Mark as Sent**, **Mark as Paid**, **Mark as Overdue**, or **Cancel Invoice**).
3. A confirmation modal will appear detailing the requested transition.
4. Click **Confirm Status Change**.
5. The invoice status badge updates immediately, recalculating customer outstanding balances and dashboard KPIs.
*(Note: You can also ask the AI Assistant to change an invoice's status via tool calling!)*

## Deletion Rules & Audit Safeguards
- **Only Draft Invoices Can Be Deleted:** Permanent deletion is strictly restricted to invoices in **Draft** status.
- **Non-Draft Invoices Cannot Be Deleted:** Once an invoice has been transitioned to **Sent**, **Paid**, or **Overdue**, the **Delete** button is permanently disabled.
- **Why Can't Sent or Paid Invoices Be Deleted?**  
  Enterprise accounting standards require an immutable audit trail. Deleting issued invoices would cause gaps in sequential numbering (`INV-000001`, `INV-000002`...) and invalidate financial tax ledgers. If an issued invoice is invalid or refunded, it must be transitioned to **Cancelled**, never deleted.

## Example Use Cases
- Receiving bank transfer confirmation and marking `INV-000002` as `Paid`.
- Reviewing aging accounts and transitioning past-due bills from `Sent` to `Overdue`.
- Voiding a duplicate estimate by transitioning a draft to `Cancelled` or deleting the draft.

## Gotchas & Common Mistakes
- **No Backward Transitions:** You cannot move an invoice backward (e.g., moving `Paid` back to `Draft` is forbidden).
- **Attempting to Delete Issued Bills:** If you click delete on an issued invoice or send a delete API request for a `Sent` invoice, the system rejects it with an error (`Only draft invoices can be deleted`). Always use **Cancel** instead.
- **Immediate Financial Recalculation:** As soon as an invoice is marked `Paid`, the customer's outstanding balance drops and total collected revenue increases instantly.

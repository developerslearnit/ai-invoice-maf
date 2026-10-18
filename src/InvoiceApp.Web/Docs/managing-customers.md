# Managing Customers

The **Customer Directory** and **Customer Details** views allow you to monitor client relationships, review their full financial history, and keep contact details current.

## Viewing Customers
Navigate to **Customers** (`/Customers`) from the sidebar to view the directory.
- **Search Bar:** Type any part of a customer's name or email to filter the list instantly.
- **Customer Cards / Table:** Displays each client's name, primary email, phone number, address, and quick metrics:
  - Total number of invoices issued.
  - Total amount invoiced.
  - Outstanding balance owed.
- **View Profile:** Click on a customer row or the **View Details** button to open their comprehensive profile page (`/Customers/{id}`).

## Customer Details Page (`/Customers/{id}`)
The profile page serves as a complete financial snapshot for that specific customer:
1. **Header & Profile Summary:** Client name, email, phone number, and physical billing address.
2. **Financial KPI Cards:**
   - **Total Invoiced:** Cumulative value of all invoices issued to this customer.
   - **Total Paid:** Total funds successfully collected from this customer.
   - **Outstanding Balance:** Total sum of unpaid invoices (`Sent` and `Overdue` status).
   - **Invoice Count:** Breakdown of invoices by status.
3. **Customer Invoices Ledger:** Complete history table of every invoice issued to this customer with issue dates, due dates, statuses, total amounts, and links to view each invoice.

## Updating Customer Details
1. From either the Customer Directory or the Customer Details page, click **Edit Customer**.
2. Update the desired fields:
   - **Name:** Adjust contact or business trade name.
   - **Email:** Update billing email (must not collide with another customer's email).
   - **Phone:** Update phone number.
   - **Address:** Update registered office or billing street address.
3. Click **Save Changes**.
4. The system validates the inputs, updates the database, and refreshes the display.

## Example Use Cases
- A client changes their accounts payable email address, requiring an update so future notices reach the right person.
- Checking a client's outstanding balance before agreeing to take on a new project.
- Reviewing historical invoices during annual account reviews or tax audits.

## Gotchas
- **Email Conflicts:** If you change a customer's email to one already registered to another client, the update will be rejected.
- **Historical Invoices:** Updating a customer's name or address updates their profile, but existing sent or paid invoices retain their historical itemized records.
- **Customer Deletion Guard:** To maintain financial integrity, customer records cannot be removed if any invoices (Draft, Sent, Paid, Overdue, or Cancelled) are linked to them.

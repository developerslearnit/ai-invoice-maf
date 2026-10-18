# Creating a Customer

The **Customers** page (`/Customers`) is where you manage your client accounts and add new customers to the platform. Every invoice issued in InvoiceAI must be linked to an existing customer.

## Steps to Create a Customer
1. Click **Customers** in the main navigation sidebar.
2. Click the **+ Add Customer** button at the top right of the page.
3. In the modal dialog that appears, fill in the customer details:
   - **Full Name** *(Required)*: The individual or company name (e.g., "Acme Global Solutions"). Maximum 200 characters.
   - **Email Address** *(Required)*: The official contact email for billing and correspondence (e.g., "billing@acmeglobal.com"). Must be a valid email format and unique across all customers.
   - **Phone Number** *(Optional)*: Contact telephone number (e.g., "+234 801 234 5678"). Maximum 50 characters.
   - **Billing Address** *(Optional)*: Physical or registered office address. This address is displayed on generated invoices. Maximum 500 characters.
4. Click **Save Customer**.
5. Once saved, a success notification appears and the customer immediately becomes available in the customer directory and in the invoice creation customer dropdown.

## What Happens When a Customer is Created?
- A unique system identifier (`Guid`) is generated for the customer.
- Initial metrics are initialized (`Total Invoices: 0`, `Total Invoiced: ₦0.00`, `Total Paid: ₦0.00`, `Outstanding: ₦0.00`).
- The customer is immediately selectable when creating new invoices.

## Example Use Cases
- Onboarding a new enterprise client before generating their first project milestone invoice.
- Adding a recurring vendor or service recipient with a permanent billing address.
- Registering a client with email and phone contacts for automated reminder tracking.

## Gotchas & Business Rules
- **Email Uniqueness:** The system strictly enforces unique email addresses. If you attempt to register an email already used by an existing customer, the platform will return a validation error (`Customer with email '...' already exists`).
- **Required Fields:** You cannot save a customer without both a Name and a valid Email Address.
- **Spaces & Formatting:** Leading and trailing whitespace are automatically trimmed, but make sure the email does not contain typos, as all future invoice communications depend on it.
- **Deleting Customers:** Customers with existing invoices attached to their profile cannot be deleted, ensuring financial ledgers remain intact.

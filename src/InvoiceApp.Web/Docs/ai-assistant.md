# AI Assistant & Knowledge Chat

InvoiceAI includes an integrated, enterprise-grade AI Assistant designed to assist with day-to-day billing workflows, debtor analytics, and platform questions.

## Accessing the AI Assistant
The AI Assistant is globally available across all screens via the floating widget at the bottom-right corner of the interface:
- **Floating Indicator Pill:** Displays a sparkle icon `✨ Ask Invoice AI` with status indicator.
- **Compact Chat Panel:** Clicking the widget opens a 420px wide chat interface for rapid conversational queries.
- **Expanded Workspace Mode:** Clicking the **Expand / Maximize** button expands the assistant into an expansive workspace modal (`80vw × 88vh`). This mode is ideal for viewing large tables, comprehensive debtor breakdowns, and agent tool execution reasoning without leaving your current page.

## Context-Aware Intelligence
The AI Assistant automatically detects your active screen and passes relevant context into the conversation via `window.__AI_CONTEXT__`:
- **When viewing an Invoice (`/Invoices/{id}`):** The assistant automatically knows the invoice ID, invoice number (e.g. `INV-000001`), customer name, and total amount. You can simply ask: *"Summarize this invoice"* or *"Is this invoice paid?"*.
- **When viewing a Customer (`/Customers/{id}`):** The assistant detects the active client and can answer questions like: *"What is this customer's payment history?"* or *"How much does this customer owe?"*.
- **When on the Dashboard or Ledger:** The assistant offers quick prompts for global metrics, overdue analysis, and pipeline health.

## What You Can Ask the AI Assistant
1. **Financial Analytics & Debt Queries:**
   - *"Show all overdue invoices"*
   - *"Which customer owes the most money?"*
   - *"Summarize our invoice activity this month"*
   - *"What is our total outstanding balance?"*
2. **Customer & Invoice Lookups:**
   - *"Look up invoice INV-000002"*
   - *"What is Contoso's payment track record?"*
   - *"Show all sent invoices waiting for payment"*
3. **Platform Guidance & Rules:**
   - *"How do I create a new customer?"*
   - *"Can I delete an invoice that was already sent?"*
   - *"What are the valid invoice status transitions?"*
   - *"How do I add line items to an invoice?"*
4. **Autonomous Task Execution via Tool Calling:**
   Users can command the AI Assistant to perform business actions directly on the platform:
   - **Create Customer:**
     - *"Create a customer named Apex Innovations Ltd with email billing@apexinnovations.com, phone +234 802 345 6789, address 12 Marina Road, Lagos."*
     - Bound Tool: `CreateCustomer` (validates name & unique email, creates database record, and returns customer ID).
   - **Update Customer Details:**
     - *"Update Acme Global's phone number to +234 803 111 2222"*
     - Bound Tool: `UpdateCustomer` (updates contact information).
   - **Create Invoices with Line Items:**
     - *"Create an invoice for Apex Innovations with 2 items: 10 consulting hours at ₦25,000 each and 1 cloud deployment package at ₦150,000, due in two weeks."*
     - Bound Tool: `CreateInvoice` (resolves customer, calculates line totals and grand total, generates sequential `INV-00000X` number, creates invoice in `Draft` status).
   - **Change Invoice Status:**
     - *"Mark invoice INV-000001 as Sent"*
     - *"Mark invoice INV-000002 as Paid"*
     - *"Mark invoice INV-000003 as Overdue"*
     - *"Cancel invoice INV-000004"*
     - Bound Tool: `UpdateInvoiceStatus` (strictly executes state machine transitions).
   - **Delete Draft Invoices:**
     - *"Delete draft invoice INV-000005"*
     - Bound Tool: `DeleteDraftInvoice` (permits deletion only if status is `Draft`; blocks deletion on `Sent`/`Paid`/`Overdue` invoices and explains why they must be `Cancelled` instead).
   - **Inquire Overdue Invoices & Business KPIs:**
     - *"Show all overdue invoices"*
     - *"What is our overall invoice summary?"*
     - Bound Tools: `GetOverdueInvoices`, `GetInvoiceSummary`, `GetInvoiceByNumber`.
   - **Read Platform Documentation:**
     - *"How do I create an invoice in InvoiceAI?"*
     - Bound Tool: `ReadDocumentationPage` (retrieves factual markdown pages from `Docs/`).

## Specialized Autonomous Agents Architecture
Under the hood, **`AgentCoordinator`** evaluates user intent, loads conversation memory via `IAgentMemoryService`, and delegates to specialized agents:
1. **`InvoiceAssistantAgent`:** Specialized in invoice lifecycle creation, numbering lookups, status transitions, and draft deletions.
2. **`CustomerOperationsAgent`:** Dedicated to customer onboarding, contact updates, and client profile analytics.
3. **`AnalyticsAgent`:** Focuses on executive KPIs, debtor concentrations, cash flow, and status distributions.
4. **`DocumentationAgent`:** Answers platform workflow and business rule questions grounded in official documentation.

## Suggested Prompt Chips
The assistant displays clickable prompt chips beneath each response. Clicking a chip automatically sends that prompt to help you quickly explore key business metrics without typing.

## Configuration & Azure OpenAI Setup
The AI subsystem is configured in `appsettings.json` (and `appsettings.Development.json`) under the `"AzureOpenAI"` section:

```json
"AzureOpenAI": {
  "Endpoint": "https://your-openai-resource.openai.azure.com/",
  "ApiKey": "YOUR_AZURE_OPENAI_API_KEY",
  "DeploymentName": "gpt-4o",
  "EmbeddingDeploymentName": "text-embedding-3-small"
}
```

### Simulation Mode (Offline Testing)
If the `ApiKey` or `Endpoint` values are left empty or retain default placeholders (`YOUR_AZURE_OPENAI_API_KEY`, `your-openai-resource`), InvoiceAI automatically engages **Simulation Mode**:
- Full tool calling (creating customers, creating invoices, changing invoice status, draft deletion, and metrics) runs locally against your real database without incurring OpenAI cloud API costs.
- Allows team members and automated test suites to build, test, and demo without Azure credentials.
- When valid Azure OpenAI credentials and deployments are provided, the coordinator and agents seamlessly route requests through `AzureOpenAIClient` and Microsoft Agent Framework with function calling enabled.

## Gotchas & Tips
- **Audit Rules Still Apply:** Even when interacting through the AI assistant, core business rules cannot be bypassed. The assistant cannot delete a `Sent` invoice or create an invoice with invalid dates.
- **Preserved Conversation State:** Expanding and collapsing the workspace maintains your active message history and context without reloading.


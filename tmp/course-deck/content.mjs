export const modules = [
  "Integration architecture", "Framework runtime", "Business tools",
  "Prompts and conversation state", "Retrieval and grounding",
  "Orchestration and interoperability", "ASP.NET Core and user experience",
  "Security and controlled writes", "Operations and reliability",
  "Evaluation and the capstone"
];
export const sources = {
  overview:["Microsoft Agent Framework overview","https://learn.microsoft.com/en-us/agent-framework/overview/"],
  extensions:["Microsoft.Extensions.AI","https://learn.microsoft.com/dotnet/core/extensions/artificial-intelligence"],
  azure:["Azure OpenAI provider","https://learn.microsoft.com/en-us/agent-framework/integrations/by-component/model-providers/azure-openai"],
  api:["AsAIAgent API reference","https://learn.microsoft.com/en-us/dotnet/api/openai.chat.openaichatclientextensions.asaiagent?view=agent-framework-dotnet-latest"],
  first:["First agent","https://learn.microsoft.com/en-us/agent-framework/get-started/your-first-agent"],
  sessions:["Multi-turn conversations","https://learn.microsoft.com/en-us/agent-framework/get-started/multi-turn"],
  storage:["Conversation storage","https://learn.microsoft.com/en-us/agent-framework/agents/conversations/storage"],
  tools:["Function tools","https://learn.microsoft.com/en-us/agent-framework/get-started/add-tools"],
  structured:["Structured outputs","https://learn.microsoft.com/en-us/agent-framework/agents/structured-outputs"],
  middleware:["Agent middleware","https://learn.microsoft.com/en-us/agent-framework/concepts/agents/middleware/"],
  context:["Context providers","https://learn.microsoft.com/en-us/agent-framework/journey/adding-context-providers"],
  rag:["TextSearchProvider","https://learn.microsoft.com/en-us/dotnet/api/microsoft.agents.ai.textsearchprovider?view=agent-framework-dotnet-latest"],
  ragguide:["RAG with Agent Framework","https://learn.microsoft.com/en-us/agent-framework/user-guide/agents/agent-rag"],
  workflows:["Workflow concepts","https://learn.microsoft.com/en-us/agent-framework/concepts/workflows/"],
  builder:["WorkflowBuilder","https://learn.microsoft.com/agent-framework/workflows/workflows"],
  patterns:["Orchestration patterns","https://learn.microsoft.com/agent-framework/workflows/orchestrations/"],
  checkpoints:["Workflow checkpoints","https://learn.microsoft.com/en-us/agent-framework/workflows/checkpoints"],
  hitl:["Workflow human input","https://learn.microsoft.com/en-us/agent-framework/workflows/human-in-the-loop"],
  approval:["Function approval","https://learn.microsoft.com/en-us/agent-framework/agents/tools/tool-approval"],
  delegation:["Agents as tools","https://learn.microsoft.com/en-us/agent-framework/journey/agents-as-tools"],
  mcp:["MCP integration","https://learn.microsoft.com/en-us/agent-framework/agents/tools/local-mcp-tools"],
  a2a:["A2A hosting","https://learn.microsoft.com/en-us/agent-framework/hosting/self-hosting/a2a"],
  skills:["Agent Skills","https://learn.microsoft.com/en-us/agent-framework/agents/skills"],
  harness:["Harness Agent","https://learn.microsoft.com/agent-framework/get-started/harness"],
  hosting:["Self-hosting","https://learn.microsoft.com/en-us/agent-framework/hosting/self-hosting"],
  hostchoice:["Hosting models","https://learn.microsoft.com/en-us/agent-framework/hosting/"],
  agui:["AG-UI overview","https://learn.microsoft.com/en-us/agent-framework/integrations/ag-ui/"],
  aguicode:["AG-UI hosting setup","https://learn.microsoft.com/en-us/agent-framework/integrations/by-component/ui/ag-ui/getting-started"],
  durable:["Durable Extension","https://learn.microsoft.com/en-us/agent-framework/integrations/azure-functions"],
  background:["Background responses","https://learn.microsoft.com/en-us/agent-framework/agents/background-responses"],
  safety:["Agent safety","https://learn.microsoft.com/en-gb/agent-framework/agents/safety"],
  trust:["AIAgent trust boundaries","https://learn.microsoft.com/en-us/dotnet/api/microsoft.agents.ai.aiagent?view=agent-framework-dotnet-latest"],
  fides:["FIDES status and limitations","https://learn.microsoft.com/en-us/agent-framework/agents/security"],
  telemetry:["Observability","https://learn.microsoft.com/en-us/agent-framework/agents/observability?pivots=programming-language-csharp"],
  evaluation:["Evaluation","https://learn.microsoft.com/en-us/agent-framework/agents/evaluation?pivots=programming-language-csharp"],
  evalmetrics:["Foundry evaluation dimensions","https://learn.microsoft.com/en-us/azure/ai-foundry/concepts/evaluation-approach-gen-ai"],
  capabilities:["Additional agent capabilities","https://learn.microsoft.com/agent-framework/agents/"]
};
export const repo = {
  aiProject:["src/InvoiceApp.AI/InvoiceApp.AI.csproj","<PackageReference Include=\"Microsoft.Extensions.AI\""],
  packages:["Directory.Packages.props","<PackageVersion Include=\"Azure.AI.OpenAI\""],
  di:["src/InvoiceApp.AI/DependencyInjection.cs","public static IServiceCollection AddAgentFramework("],
  base:["src/InvoiceApp.AI/Agents/BaseAgent.cs","public virtual async Task<AgentResponse> ExecuteAsync("],
  coordinator:["src/InvoiceApp.AI/Coordinator/AgentCoordinator.cs","public async Task<CoordinatorResult> ProcessAsync("],
  routing:["src/InvoiceApp.AI/Coordinator/AgentCoordinator.cs","private IBaseAgent RouteToAgent("],
  api:["src/InvoiceApp.Api/Endpoints/AiEndpoints.cs","public static IEndpointRouteBuilder MapAiEndpoints("],
  invoiceTools:["src/InvoiceApp.AI/Tools/InvoiceTools.cs","public IEnumerable<AITool> GetTools()"],
  summaryTool:["src/InvoiceApp.AI/Tools/InvoiceTools.cs","public async Task<string> GetInvoiceSummary("],
  customerTools:["src/InvoiceApp.AI/Tools/CustomerTools.cs","public IEnumerable<AITool> GetTools()"],
  docs:["src/InvoiceApp.AI/Tools/DocumentationTools.cs","public async Task<string> ReadDocumentationPage("],
  analytics:["src/InvoiceApp.AI/Agents/AnalyticsAgent.cs","public sealed class AnalyticsAgent"],
  simulation:["src/InvoiceApp.AI/Agents/InvoiceAssistantAgent.cs","protected override async Task<string> GenerateSimulatedResponseAsync"],
  customerAgent:["src/InvoiceApp.AI/Agents/CustomerOperationsAgent.cs","protected override async Task<string> GenerateSimulatedResponseAsync"],
  memory:["src/InvoiceApp.AI/Memory/AgentMemoryService.cs","public sealed class AgentMemoryService"],
  evaluator:["src/InvoiceApp.AI/Evaluations/AgentEvaluationService.cs","public Task<AgentEvaluationResult> EvaluateResponseAsync("],
  models:["src/InvoiceApp.AI/Agents/AgentModels.cs","public sealed record AgentRequest("],
  dispatcher:["src/InvoiceApp.Application/Common/Messaging/Dispatcher.cs","public async Task<TResponse> SendAsync<TResponse>("],
  invoice:["src/InvoiceApp.Domain/Entities/Invoice.cs","public Result TransitionTo("],
  create:["src/InvoiceApp.Application/Features/Invoices/Commands/CreateInvoice/CreateInvoiceCommand.cs","public async Task<Result<Guid>> HandleAsync("],
  status:["src/InvoiceApp.Application/Features/Invoices/Commands/UpdateInvoiceStatus/UpdateInvoiceStatusCommand.cs","public async Task<Result> HandleAsync("],
  validation:["src/InvoiceApp.Api/Endpoints/InvoicesEndpoints.cs","var validationResult = await validator.ValidateAsync"],
  sql:["src/InvoiceApp.Infrastructure/Persistence/Repositories/InvoiceReadRepository.cs","public async Task<InvoiceSummaryDto> GetSummaryAsync("],
  overdue:["src/InvoiceApp.Infrastructure/Persistence/Repositories/InvoiceReadRepository.cs","public async Task<IReadOnlyList<InvoiceListItemDto>> GetOverdueInvoicesAsync("],
  save:["src/InvoiceApp.Infrastructure/Persistence/Repositories/UnitOfWork.cs","public Task<int> SaveChangesAsync("],
  web:["src/InvoiceApp.Web/Controllers/AiController.cs","[HttpPost(\"chat\")]"],
  fallback:["src/InvoiceApp.Web/Services/Ai/AgentFrameworkAiAssistantService.cs","public async Task<AiChatResponse> SendMessageAsync("],
  mock:["src/InvoiceApp.Web/Services/Ai/MockAiAssistantService.cs","public async Task<AiChatResponse> SendMessageAsync("],
  webClient:["src/InvoiceApp.Web/Services/ApiClients/AiApiClient.cs","var payload = new"],
  studio:["src/InvoiceApp.Web/wwwroot/js/studio-chat.js","inferExecutedTools(query, reply, agentName)"],
  tests:["tests/InvoiceApp.UnitTests/AI/AiTests.cs","public class AiTests"],
  domainTests:["tests/InvoiceApp.UnitTests/Domain/InvoiceTests.cs","public sealed class InvoiceTests"],
  integration:["tests/InvoiceApp.IntegrationTests/ApiEndpointsTests.cs","public sealed class InvoiceAppTestFactory"],
  program:["src/InvoiceApp.Api/Program.cs","builder.Services.AddApplication();"]
};
export const deck=[];
let current=0;
const add=(type,title,data,notes,extra={})=>deck.push({type,title,data,notes,module:current,...extra});
const P=(title,rows,notes,extra={})=>add("points",title,rows,notes,extra);
const T=(title,headers,rows,notes,extra={})=>add("table",title,{headers,rows},notes,extra);
const C=(title,lines,explain,notes,extra={})=>add("code",title,{lines,explain},notes,extra);
const L=(n,title,tasks,accept,notes)=>add("lab",title,{tasks,accept},notes,{lab:n,status:"Lab target",comment:"Use the acceptance criteria to review the implementation before moving to the next module."});
const M=(n,title,outcome)=>{current=n;add("chapter",title,[outcome],"MODULE PLAN\nAllocate 3 hours: 25 minutes of concepts, 35 minutes of guided code, 70 minutes of lab work, 30 minutes of review, and 20 minutes for discussion or breaks. Adjust the suggested timings to the cohort. The lab acceptance criteria matter more than the number of slides.",{module:n});};

add("cover","AI integration with\nMicrosoft Agent Framework",["Advanced .NET engineering course","InvoiceAI capstone"],"AUTHOR COMMENTARY\nThis course assumes professional C# and ASP.NET Core experience. The focus is engineering an AI capability into an existing system, rather than teaching MVC, EF Core, or C# syntax. Students finish with an InvoiceAI integration that uses actual Microsoft Agent Framework agents and retains application/domain boundaries.\nDELIVERY\nTen modules of three contact hours include labs. A final implementation can continue as homework. Explain that repository observations and proposed capstone work are labeled separately.",{status:"Course",docs:["overview"]});
P("The engineering outcomes",[
 ["Integrate", "Introduce a framework agent through a stable application boundary."],
 ["Constrain", "Expose narrow tools and enforce permissions before business writes."],
 ["Operate", "Measure execution, evaluate behavior, and ship a reversible rollout."],
 ["Deliver", "Build InvoiceAI as a working capstone with evidence of correct outcomes."]
],"AUTHOR COMMENTARY\nAsk each engineer to name a system they might augment. Use one read use case and one write use case to establish the difference in risk and required evidence. The capstone is the integration layer and its safeguards, supported by the existing invoice application. Avoid spending course time rebuilding CRUD unless a cohort needs a separate prerequisite lab.\nDISCUSSION\nWhat business outcome would justify a model call rather than a conventional endpoint?",{status:"Course target"});
T("Course structure",["Module","Engineering focus","Lab increment"],[
 ["01-02","Architecture and framework runtime","Characterize the system, then run a real AIAgent"],
 ["03-04","Tools, prompts, sessions, and context","Ground a billing query and preserve conversation state"],
 ["05-06","Retrieval, workflows, and interoperability","Add cited knowledge and an explicit workflow"],
 ["07-08","ASP.NET Core, identity, and approvals","Host the agent and control a business write"],
 ["09-10","Reliability, evaluation, and delivery","Instrument, test, and demonstrate the capstone"]
],"AUTHOR COMMENTARY\nThe suggested course has 30 contact hours. Each module produces a reviewable increment. The slide deck includes demonstration and assessment prompts, while the engineering work happens in the repository. Keep a teaching branch for each checkpoint. Make cloud configuration available before module 2, and use separate training records for every write lab.\nFACILITATION\nAgree on team sizes, access to the model deployment, and who owns the training database.",{status:"Suggested course plan",widths:[.13,.43,.44]});
T("Repository baseline and course target",["Area","Current workspace","Capstone target"],[
 ["Agent runtime","Custom BaseAgent around IChatClient","AIAgent and AgentSession"],
 ["Coordination","Keyword router chooses one agent","Explicit routing plus a bounded workflow"],
 ["Tools","Invoice/customer methods dispatch CQRS","Validated, authorized tool contracts"],
 ["Operations","Log metrics and heuristic scoring","Traces, behavioral evaluation, rollout gates"]
],"AUTHOR COMMENTARY\nThis distinction is central. AddAgentFramework is a method name in the repository, but the AI project currently references Microsoft.Extensions.AI and Azure.AI.OpenAI, not Microsoft.Agents.AI. Do not describe that baseline as a completed framework migration. Students preserve useful tool and domain code, then replace runtime concerns behind an adapter. Current features and proposed changes remain distinct throughout the course.",{status:"Existing vs target",refs:["aiProject","base","coordinator"],docs:["api"],widths:[.19,.40,.41]});

M(1,"Integration architecture","An AI capability that preserves the existing system's contracts");
T("Use-case selection",["InvoiceAI request","Suitable mechanism","Evidence"],[
 ["Explain an invoice rule","Retrieved documentation and an agent","A supported answer with a source"],
 ["Read outstanding balances","Application query through a tool","Numbers match the query result"],
 ["Change invoice status","Controlled application command","Authorized, approved, persisted transition"],
 ["Calculate invoice totals","Existing deterministic domain code","Quantity multiplied by price"]
],"AUTHOR COMMENTARY\nThese are proposed integration choices. Ask students to identify where language interpretation adds value and where deterministic code is already superior. Financial arithmetic stays in the domain/query layer. The agent translates user intent and explains results. A narrow read use case is the first production slice because its outcome is easier to verify and its side effects are limited.\nEXERCISE\nName the acceptance evidence before selecting an SDK feature.",{status:"Design decision",widths:[.34,.39,.27]});
add("image","InvoiceAI integration boundary","architecture.png","AUTHOR COMMENTARY\nThis illustration shows the proposed runtime boundary. The framework calls the model and invokes approved tool functions. Tools delegate to application handlers. Domain entities enforce invariants, while repositories use EF Core or Dapper. Read queries can go directly to a read repository and do not need to construct domain aggregates. The diagram simplifies those two persistence paths.\nDISCUSSION\nWhere would a tenant authorization check belong, and which boundaries should repeat it?\nVISUAL\nGenerated conceptual architecture illustration.",{status:"Capstone target",refs:["dispatcher","create","sql","save"]});
P("The integration inventory",[
 ["Contracts", "Commands, queries, DTOs, error codes, and cancellation behavior."],
 ["Data", "Sensitive fields, tenant boundaries, freshness, and authoritative sources."],
 ["Side effects", "Status changes, deletions, duplicate requests, and partial completion."],
 ["Service levels", "Latency budget, model quota, availability, and support ownership."]
],"AUTHOR COMMENTARY\nHave teams inventory a vertical slice instead of reading every file. Choose GetInvoiceSummary and UpdateInvoiceStatus. Record inputs, outputs, side effects, and failure conditions. The result becomes a tool design checklist and a test plan. Do not assume that existing UI restrictions protect a new caller such as an AI tool.\nDEMO\nTrace one command from handler to SaveChangesAsync, and one query to its Dapper SQL.",{status:"Integration method",refs:["summaryTool","status","sql"]});
P("Model behavior that affects system design",[
 ["Tokens", "Every instruction, message, tool schema, and retrieved passage consumes context."],
 ["Generation", "Outputs and tool choices can vary across runs and model versions."],
 ["Capabilities", "Streaming, schemas, images, and hosted tools depend on the provider."],
 ["Control", "Application policy decides which operations the model may request."]
],"AUTHOR COMMENTARY\nKeep this discussion engineering-oriented. Context is a finite input budget, not an unlimited memory store. A low temperature does not prove correctness, and not every model supports the same options. Explain the difference between a deployment identifier and a generic model family. Treat capability discovery and compatibility tests as part of the course baseline.\nASK\nWhat can remain deterministic even when model wording varies?",{status:"Engineering fundamentals",docs:["extensions","azure"]});
P("The integration contract",[
 ["Input", "A user message plus server-resolved identity and record context."],
 ["Output", "An answer, source evidence, execution state, and correlation identifiers."],
 ["Policy", "Allowed tools, approval requirements, deadlines, and budgets."],
 ["Failure", "A safe error contract that preserves known business outcomes."]
],"AUTHOR COMMENTARY\nThis is a proposed application contract. Keep it separate from the particular provider response type. An answer may be completed, waiting for approval, or failed after a partial operation. A single boolean cannot describe every state. Discuss whether the existing chat envelope can evolve compatibly or needs a versioned endpoint. Record this decision before adding UI streaming.",{status:"Capstone target",refs:["api","models"]});
P("The existing command boundary",[
 ["Command", "UpdateInvoiceStatusCommand describes the requested business change."],
 ["Handler", "Loads the invoice and invokes Invoice.TransitionTo."],
 ["Invariant", "Draft cannot transition directly to Paid."],
 ["Persistence", "The handler saves only after the transition succeeds."]
],"AUTHOR COMMENTARY\nStudents already understand layered .NET applications, so use the code as evidence rather than a Clean Architecture lecture. Show how a framework tool can call the same handler as a form. The preserved invariant is the course's recurring example. A model request for an illegal transition should produce a business failure even if the model gives a persuasive explanation.\nQUESTION\nWhat happens if a prompt says to bypass the state machine?",{status:"Existing implementation",refs:["status","invoice","domainTests"]});
L(1,"Lab 01: characterize the existing system",[
 "Select one billing query and one status command.",
 "Document contracts, side effects, and current failure behavior.",
 "Record a baseline test or reproducible request for each."
],[
 "The read result matches authoritative data.",
 "Draft-to-Paid fails in the domain.",
 "The integration boundary has a clear owner."
],"AUTHOR COMMENTARY\nSuggested lab: 70 minutes. Teams inspect GetInvoiceSummaryQuery, UpdateInvoiceStatusCommand, Invoice.TransitionTo, and the two HTTP paths. They create a short characterization note and focused tests where meaningful. No model is required. Review the difference between a query returning zero and a query failing. Keep the artifact as a baseline for the final regression comparison.\nREVIEW\nAsk teams to identify which current validators the AI path actually executes.");

M(2,"Framework runtime","A real Microsoft Agent Framework agent behind a .NET adapter");
T("The .NET AI stack",["Layer","Role","InvoiceAI decision"],[
 ["Azure.AI.OpenAI","Provider client and model connectivity","Retain the existing provider initially"],
 ["Microsoft.Extensions.AI","Common chat/tool abstractions","Reuse compatible AIFunction contracts"],
 ["Microsoft.Agents.AI","Agent execution, sessions, and composition","Adopt AIAgent through the adapter"],
 ["Application/domain","Business rules and persistence contracts","Preserve deterministic operations"]
],"AUTHOR COMMENTARY\nMake package names precise. Microsoft.Extensions.AI provides common inference abstractions. Microsoft Agent Framework adds agent behavior and composition on top of compatible providers. The repository's custom BaseAgent is an application type, not an alias for the framework's AIAgent. Use namespace aliases if framework AgentResponse and InvoiceApp's AgentResponse collide during migration.",{status:"Framework architecture",docs:["extensions","overview"],refs:["aiProject"],widths:[.30,.37,.33]});
P("A reproducible course environment",[
 ["Project", "Use a separate lab project or branch before changing the integration."],
 ["Packages", "Select compatible Agent Framework, provider, and hosting versions."],
 ["Configuration", "Separate deployment names and endpoints from credentials."],
 ["Reproduction", "Commit package pins and the lab instructions after a successful restore."]
],"AUTHOR COMMENTARY\nThe checked-in application targets .NET 10. Current Microsoft examples include prerelease package commands and may evolve. Select a tested version set for the course and freeze it. Do not assume that adding the newest package to the existing central package versions will produce a compatible graph. The repository's package list is a baseline, not the target framework lock file.\nPREPARATION\nRehearse restore, compile, a read run, and a provider-failure case before class.",{status:"Course setup",refs:["packages"],docs:["first"]});
T("Provider and identity choices",["Choice","Use in the course","Engineering implication"],[
 ["Chat Completions","Initial migration from the current client","A smaller behavioral change"],
 ["Responses","Extension for richer provider capabilities","Review tool support and continuation semantics"],
 ["Development identity","Developer credential or a controlled secret","Keep credentials outside source control"],
 ["Production identity","A specific managed identity when supported","Assign narrowly scoped service permissions"]
],"AUTHOR COMMENTARY\nUse the Azure provider documentation for the exact client surface and package set. The workshop initially keeps Chat Completions to isolate the framework migration. A later extension can compare Responses. The deployment must support the capabilities used in the lab. A production credential chain should be deliberate rather than relying on broad fallback probing. Do not mix a Foundry project endpoint with an Azure OpenAI resource endpoint.",{status:"Provider choice",docs:["azure"],widths:[.26,.37,.37]});
C("The first framework agent",[
 "using Microsoft.Agents.AI;",
 "using OpenAI.Chat;",
 "",
 "// azureClient, deployment, and ct come from the host",
 "AIAgent billing = azureClient.GetChatClient(deployment)",
 "    .AsAIAgent(",
 "        name: \"Billing\",",
 "        instructions: \"Explain invoice operations clearly.\");",
 "",
 "var reply = await billing.RunAsync(",
 "    \"What makes an invoice overdue?\", cancellationToken: ct);"
],["AIAgent is the framework runtime.","The provider client comes from configuration.","This first run has no business tools."],"AUTHOR COMMENTARY\nThis is a condensed SDK pattern using the documented ChatClient extension. The host supplies the configured AzureOpenAIClient, deployment name, and token. Read the corresponding namespace and package reference aloud, then let students compile a minimal run. The answer cannot establish the current application balance because no business tool is registered yet.\nDEMO\nCompare a general explanation with a request for live totals. Treat the latter as unsupported until the next module.",{status:"Framework code pattern",docs:["api"]});
C("Conversation continuity with AgentSession",[
 "AgentSession session = await billing.CreateSessionAsync(ct);",
 "",
 "await billing.RunAsync(",
 "    \"We are reviewing customer Apex.\", session,",
 "    cancellationToken: ct);",
 "",
 "var next = await billing.RunAsync(",
 "    \"Which customer are we reviewing?\", session,",
 "    cancellationToken: ct);"
],["Reuse the session for related turns.","Bind ownership in the application.","Persist state when requests outlive a process."],"AUTHOR COMMENTARY\nA session carries provider-compatible conversation state. Creating a fresh session for every HTTP request discards continuity unless the host restores or resends history. The example stores only a user claim about a customer, not authorization to read that customer. Avoid sharing one mutable session between different users or concurrent turns.\nASK\nWhat identifies the conversation, and what establishes the caller's permission to access it?",{status:"Framework code pattern",docs:["sessions"]});
C("Streaming at the agent boundary",[
 "await foreach (var update in billing.RunStreamingAsync(",
 "    \"Explain the invoice workflow.\",",
 "    session, cancellationToken: ct))",
 "{",
 "    // Convert updates to the application's event contract.",
 "    await eventSink.PublishAsync(update, ct);",
 "}"
],["Treat output as incremental events.","Handle completion and failure explicitly.","Keep an authoritative final run state."],"AUTHOR COMMENTARY\neventSink is a course-owned adapter, not a framework API. The fragment shows where to translate AgentResponseUpdate into a transport contract. Streaming a text token is different from committing a business operation. A disconnected browser should not be the only place that knows the operation state. Explore how cancellation and reconnection affect reads versus writes.\nDEMO\nDisplay partial text, then inspect the final response separately.",{status:"Framework code pattern",docs:["first"]});
P("Dependency injection and lifetime boundaries",[
 ["Clients", "Reuse suitable provider clients without capturing request-specific state."],
 ["Tools", "Resolve dispatcher and DbContext dependencies within a valid scope."],
 ["Sessions", "Store conversation state independently of the agent instance."],
 ["Adapters", "Translate framework results to the existing application response contract."]
],"AUTHOR COMMENTARY\nThe baseline registers tools, agents, and coordinator as scoped, with a singleton chat client and in-memory history service. A new singleton agent that closes over a scoped InvoiceTools instance would introduce a lifetime defect. Design agent construction around the selected hosting lifetime or create a scope inside a suitable tool executor. Do not share EF DbContext across parallel calls. Validate the approach with DI scope checks and concurrent-request tests.",{status:"Integration design",refs:["di","base"],docs:["hosting"]});
L(2,"Lab 02: add the framework runtime",[
 "Create a lab project with pinned compatible packages.",
 "Construct an AIAgent using the configured provider.",
 "Run two turns with one session and stream a response."
],[
 "The project actually references Microsoft.Agents.AI.",
 "A cancellation token reaches the run.",
 "A failed provider call yields a controlled result."
],"AUTHOR COMMENTARY\nSuggested lab: 70 minutes. Start outside the existing API to isolate model credentials and SDK changes. Students should record the exact package set that compiled. Require one session-continuity example and one explicit provider failure. Do not accept a renamed custom class as the framework integration. Capture a short transcript without committing credentials or customer data.");

M(3,"Business tools","Controlled function calling through existing application use cases");
P("The function-calling execution cycle",[
 ["Describe", "Advertise the function name, schema, and intended behavior."],
 ["Request", "The model proposes a function and arguments."],
 ["Execute", "The application validates policy and invokes the implementation."],
 ["Explain", "The model uses the tool result to produce an answer."]
],"AUTHOR COMMENTARY\nWalk through one read tool before exposing mutations. The model selects arguments, but ordinary .NET code performs execution. Tools should have small, meaningful contracts that describe domain intent. A model may choose no tool, the wrong tool, or an incomplete argument set, so tests must measure behavior rather than assume every requested function will run.\nDISCUSSION\nWhich tool results can be safely cached, and for how long?",{status:"Framework concept",docs:["tools"]});
C("An existing query as a tool",[
 "using System.ComponentModel;",
 "",
 "[Description(\"Read current billing totals from InvoiceAI.\")]",
 "public Task<string> ReadBillingAsync(CancellationToken ct)",
 "    => invoiceTools.GetInvoiceSummary(ct);",
 "",
 "// Existing inner path:",
 "// GetInvoiceSummaryQuery -> handler -> read repository"
],["Reuse the application query.","Document freshness and units.","Keep database access in its existing layer."],"AUTHOR COMMENTARY\nThis wrapper deliberately reuses the current string-returning InvoiceTools method. It provides an easy migration step without rewriting the query. Explain that the eventual typed tool outcome should distinguish a successful zero balance from a failed query. The application query remains the authoritative computation, and the model formats the result.\nDEMO\nInspect the JSON and compare it with the repository SQL.",{status:"Migration code pattern",refs:["summaryTool","sql"]});
C("A narrow analytics capability",[
 "var summaryTool = AIFunctionFactory.Create(",
 "    invoiceTools.GetInvoiceSummary);",
 "",
 "AIAgent analytics = azureClient.GetChatClient(deployment)",
 "    .AsAIAgent(",
 "        name: \"BillingAnalytics\",",
 "        instructions: analyticsInstructions,",
 "        tools: [summaryTool]);"
],["Expose only the functions required.","A persona does not enforce permissions.","Add write tools in a separate lab."],"AUTHOR COMMENTARY\nThe baseline analytics agent concatenates every invoice and customer tool, including writes. This course target intentionally starts with one read tool. Show the tool inventory before running the model. The policy must still check the caller inside each protected operation. A restricted list reduces capability exposure but does not replace authorization.",{status:"Capstone target",refs:["analytics","invoiceTools"],docs:["api"]});
C("Structured output at the answer boundary",[
 "public sealed class BillingAnswer",
 "{",
 "    public string Summary { get; set; } = \"\";",
 "    public string[] EvidenceIds { get; set; } = [];",
 "    public bool NeedsClarification { get; set; }",
 "}",
 "",
 "var result = await analytics.RunAsync<BillingAnswer>(",
 "    prompt, session, cancellationToken: ct);",
 "BillingAnswer answer = result.Result;"
],["Use schemas for predictable application shape.","Validate business meaning separately.","Retain provenance from actual tool results."],"AUTHOR COMMENTARY\nThis is a course-defined output type and a documented generic run pattern. A schema constrains shape, not factual accuracy or permission. The application should verify EvidenceIds against records returned in the current run rather than accepting invented citations. Check the chosen provider's structured-output support and handle parse failures.\nASK\nCan a valid BillingAnswer JSON object still contain a false claim?",{status:"Framework code pattern",docs:["structured"]});
T("Validation across entry points",["Boundary","Current behavior","Course change"],[
 ["Invoice REST endpoint","Explicit FluentValidation call","Retain clear HTTP validation"],
 ["Dispatcher","Resolves and invokes a handler","Add a shared validation mechanism"],
 ["AI tool","Parses arguments, then dispatches","Validate before the protected action"],
 ["Domain entity","Enforces core invoice invariants","Keep rules independent of the model"]
],"AUTHOR COMMENTARY\nThe repository comments and README suggest broader automatic validation than Dispatcher implements. Show the actual endpoint and dispatcher before proposing a fix. Students may choose a validating dispatcher decorator or another application boundary, but every caller must receive the same invariant enforcement. Tool argument descriptions are not a validator.\nDEMO\nIdentify one validator-only constraint and show how the new boundary will test it.",{status:"Existing gap and target",refs:["validation","dispatcher","create"],widths:[.25,.37,.38]});
T("Invoice invariants remain authoritative",["Current state","Allowed new states","Meaning for tools"],[
 ["Draft","Sent or Cancelled","Direct Paid is rejected"],
 ["Sent","Paid, Overdue, or Cancelled","Only valid transitions save"],
 ["Overdue","Paid or Cancelled","A payment can settle exposure"],
 ["Paid / Cancelled","No different status","No prompt overrides the state machine"]
],"AUTHOR COMMENTARY\nSame-state requests return success. Only Draft invoices can be deleted. Use an actual training invoice number rather than assuming INV-000001 exists. The agent must report the returned domain outcome accurately. An approval decision never grants permission to violate these business rules.\nASK\nWhat if the invoice changed between the proposal and the commit?",{status:"Existing domain behavior",refs:["invoice","status"],widths:[.22,.39,.39]});
P("Tool outcomes and error contracts",[
 ["Success", "Return the operation ID, affected record, and verified result."],
 ["Business rejection", "Return a stable error code and an actionable explanation."],
 ["Infrastructure failure", "Keep secrets and raw exception details out of the model output."],
 ["Ambiguity", "Request clarification before selecting a customer or invoice."]
],"AUTHOR COMMENTARY\nThe current tools mix bare DTO JSON with success/error envelopes. A returned error string is still a normal function result, so BaseAgent can report overall success while a business command failed. Propose a typed ToolOutcome and map it consistently. The course owns that contract; do not imply the framework automatically interprets every application error. Include a correlation ID for server diagnostics.",{status:"Capstone target",refs:["invoiceTools","base"]});
P("Execution limits and tool granularity",[
 ["Bounded reads", "Pagination, date filters, row caps, and cancellation."],
 ["Bounded autonomy", "Maximum steps, token budget, and a request deadline."],
 ["Mutations", "Explicit intent, authorization, approval, and idempotency."],
 ["Concurrency", "Parallelize independent reads with safe service lifetimes."]
],"AUTHOR COMMENTARY\nListInvoices currently exposes only status and fixes the first page at 50 records. A complete analytic ranking cannot assume that list is exhaustive. Add a purpose-built query when the business question needs aggregation. Keep mutating commands sequential unless a transaction and concurrency design justify otherwise. The framework's possible concurrency does not make a scoped DbContext thread-safe.",{status:"Tool design",refs:["invoiceTools","sql"]});
L(3,"Lab 03: ground a billing answer",[
 "Expose GetInvoiceSummary through a narrow framework agent.",
 "Add shared input validation and a typed tool-outcome design.",
 "Return a structured answer with verified evidence identifiers."
],[
 "Failed reads cannot masquerade as a zero balance.",
 "An illegal transition remains rejected.",
 "Analytics has no unnecessary mutation tools."
],"AUTHOR COMMENTARY\nSuggested lab: 70 minutes. Provide successful, empty, and failed summary fixtures. Inspect which function the agent requests and compare its arguments with expectations. Add at least one deterministic test for validation and one live or recorded behavioral case for tool selection. The existing database arithmetic remains unchanged. Review unsupported claims about overdue amounts and cancelled invoices.");

M(4,"Prompts and conversation state","Instructions and memory that respect application trust boundaries");
P("The instruction contract",[
 ["Task", "Explain billing data using authoritative tools."],
 ["Evidence", "Use the returned values and identify their scope."],
 ["Uncertainty", "Ask for missing identifiers or state that data is unavailable."],
 ["Action", "Report a write only after a verified application outcome."]
],"AUTHOR COMMENTARY\nTreat instructions as versioned application assets. Give the model a concrete task and evidence policy rather than repeating broad claims such as being an expert. The instruction cannot establish user identity or grant a tool permission. Include examples of unavailable data and ambiguity. A prompt change should trigger the same evaluation corpus as a model or tool change.\nEXERCISE\nWrite one positive example and one refusal-to-invent example.",{status:"Prompt design",refs:["analytics","base"]});
C("A versioned billing instruction",[
 "You explain InvoiceAI billing data.",
 "Use current tool results for balances and status.",
 "State whether figures include Draft or Cancelled invoices.",
 "Use Nigerian Naira and the tool's exact reporting scope.",
 "If a required identifier is ambiguous, ask a question.",
 "If an operation fails, report its error outcome.",
 "Never infer a successful write from the user's request."
],["Store as a reviewed application asset.","Test examples with conflicting inputs.","Keep business enforcement in code."],"AUTHOR COMMENTARY\nThis is a proposed system instruction for the capstone. The exact metric definitions must match the SQL, not the user-facing documentation when they disagree. Dates should specify timezone and as-of scope. Keep the prompt concise enough that its evidence policy remains visible among tools and retrieved content.\nDEMO\nGive the model a failed read result and compare responses before and after the instruction change.",{status:"Course prompt example",refs:["sql","overdue"]});
P("Trusted context and page state",[
 ["Client input", "Treat invoiceId, customerId, and page as claims."],
 ["Server resolution", "Load records under the authenticated user's scope."],
 ["Context provider", "Inject the smallest relevant, authorized facts."],
 ["Tool execution", "Recheck permissions when an action actually runs."]
],"AUTHOR COMMENTARY\nThe web client forwards page state, but the baseline BaseAgent reads only request.Prompt when constructing messages. Simply accepting Context does not ground the model. In the target, a server resolver validates identifiers and obtains permitted record facts. Context providers can supply those facts proactively. Do not put credentials, raw EF entities, or every customer record into the context.",{status:"Existing gap and target",refs:["webClient","models","base"],docs:["context"]});
T("Three kinds of state",["State","Purpose","InvoiceAI example"],[
 ["Conversation history","Recent messages in an AgentSession","The user is discussing Apex"],
 ["Application state","Authoritative business facts","Apex's actual invoice balances"],
 ["Long-term memory","Selected persistent user preferences","Preferred report format or language"],
 ["Workflow state","Progress and pending decisions","An approval awaits a named reviewer"]
],"AUTHOR COMMENTARY\nA user statement in conversation history is not an authoritative business record. Long-term memory needs consent, retention rules, and ownership. The framework session is a state carrier, not an authorization system. Decide which state can be discarded and which must survive a restart. Explain why copying all history into a user string loses role information.",{status:"State design",refs:["memory","coordinator"],docs:["storage"],widths:[.26,.37,.37]});
P("Session persistence and isolation",[
 ["Key", "Bind continuation IDs to tenant and user ownership."],
 ["Storage", "Persist session/provider identifiers with explicit retention."],
 ["Concurrency", "Serialize or version simultaneous turns for one conversation."],
 ["Recovery", "Define restart, expiry, deletion, and migration behavior."]
],"AUTHOR COMMENTARY\nThe baseline stores strings in a static dictionary and returns ten messages without pruning older storage. The target may use SQL, Redis, or another selected store, but the choice is an implementation decision. In a custom history provider, keep session-specific keys in session state rather than mutable provider-instance fields. Test cross-user access and two simultaneous turns.",{status:"Capstone target",refs:["memory"],docs:["storage"]});
P("Context budget and memory reduction",[
 ["Allocate", "Reserve space for instructions, tool schemas, evidence, and output."],
 ["Reduce", "Trim or summarize older turns under an explicit policy."],
 ["Preserve", "Keep identifiers, unresolved approvals, and authoritative evidence."],
 ["Measure", "Record input growth and answer quality after reduction."]
],"AUTHOR COMMENTARY\nTen messages can still contain an enormous amount of text. A summary can lose a constraint or change an identifier, so treat summarization as another model operation that needs evaluation. Keep facts re-loadable from the application rather than relying on a conversation summary as the ledger. Avoid teaching a fixed context size because it depends on the selected deployment.",{status:"Application policy",refs:["memory"]});
L(4,"Lab 04: session and context isolation",[
 "Persist and restore a framework session.",
 "Resolve a page invoice ID on the server.",
 "Test expiry, concurrent turns, and another user's continuation ID."
],[
 "Authorized facts reach the agent.",
 "Ownership checks reject a foreign session.",
 "A restart preserves the intended conversation state."
],"AUTHOR COMMENTARY\nSuggested lab: 70 minutes. Use two synthetic principals and separate customer records. Students design a session repository and retention policy, then implement a minimal persistence path. Include a case where a client changes invoiceId. The model should receive only the authorized projection. Assess isolation through deterministic tests rather than asking the model whether access is allowed.");

M(5,"Retrieval and grounding","Documentation and financial evidence with traceable sources");
T("Retrieval choices",["Question","Authoritative source","Integration choice"],[
 ["What does Draft mean?","Reviewed Markdown documentation","File or search retrieval"],
 ["How much is outstanding?","Current business database","Typed application query"],
 ["Which rule changed?","Versioned knowledge content","Search with document/version metadata"],
 ["Why did this command fail?","Domain error plus current record","Tool outcome with relevant evidence"]
],"AUTHOR COMMENTARY\nRAG is useful for explanatory knowledge, while structured financial facts should come from application queries. A vector index is not a replacement for transactional data. Use a freshness policy and return a source reference with each retrieved passage. The model should not answer a balance from stale documentation or general training data.",{status:"Retrieval design",docs:["ragguide"],refs:["sql","docs"],widths:[.30,.36,.34]});
P("The current documentation tool",[
 ["Lookup", "ReadDocumentationPage resolves a page slug to a Markdown file."],
 ["Content", "It returns the full file and a static page index when missing."],
 ["Limit", "There is no vector index, chunk ranking, or citation validation."],
 ["Teaching target", "Add controlled retrieval and verify claims against sources."]
],"AUTHOR COMMENTARY\nShow the filesystem lookup rather than calling it a sophisticated semantic search system. The tool strips directory components from the supplied page name and searches for a Docs directory. The Markdown can still contain inaccurate product claims, so the ingestion review must compare it with implemented business rules. Retrieved text remains untrusted data even when stored in the repository.",{status:"Existing implementation",refs:["docs"]});
P("The ingestion pipeline",[
 ["Review", "Identify authoritative documents and remove stale claims."],
 ["Segment", "Chunk by meaningful sections with stable source identifiers."],
 ["Enrich", "Store title, version, tenant scope, and effective date."],
 ["Index", "Choose keyword, vector, or hybrid search for the corpus."]
],"AUTHOR COMMENTARY\nThis is the capstone retrieval design, not an implemented repository feature. For the small Docs corpus, a simple search can be adequate. Add embeddings only when the retrieval task justifies their operational cost. Explain that an embedding is a search representation, not a trustworthy summary or an access-control decision. Re-index changed documents and retire obsolete versions.",{status:"Capstone target",docs:["ragguide"]});
P("Retrieval policy and answer scope",[
 ["Filter", "Apply tenant and access rules before evidence enters the model."],
 ["Rank", "Retrieve a small useful set and inspect irrelevant matches."],
 ["Cite", "Map source identifiers to actual returned passages."],
 ["Abstain", "Return uncertainty when the evidence does not support an answer."]
],"AUTHOR COMMENTARY\nHave students compare one keyword and one hybrid retrieval result set. Access filtering belongs in retrieval or the source service, not in a later model instruction. Avoid fabricated precision about top-k or similarity thresholds: choose them using the course corpus and evaluation set. A citation must resolve to evidence the model actually received.",{status:"Retrieval policy",docs:["rag"]});
C("A framework retrieval context provider",[
 "var retrieval = new TextSearchProvider(",
 "    SearchInvoiceDocsAsync,",
 "    new TextSearchProviderOptions",
 "    {",
 "        SearchTime = TextSearchProviderOptions",
 "            .TextSearchBehavior.BeforeAIInvoke",
 "    });",
 "",
 "// Attach retrieval through",
 "// ChatClientAgentOptions.AIContextProviders."
],["The adapter owns search and access filtering.","Returned passages carry source metadata.","On-demand search is another supported mode."],"AUTHOR COMMENTARY\nSearchInvoiceDocsAsync is an application adapter with the signature required by the pinned SDK. The slide is a condensed configuration pattern, not a complete search implementation. The provider can inject evidence before inference or expose retrieval on demand. Use the current TextSearchProvider API reference for the result type and options in the selected version.",{status:"Framework code pattern",docs:["rag"]});
P("Grounding evaluation",[
 ["Retrieval", "Did the correct rule or record enter the evidence set?"],
 ["Answer", "Does each material claim follow from the evidence?"],
 ["Citation", "Does the referenced source support the exact statement?"],
 ["Freshness", "Does the answer disclose the reporting period and as-of time?"]
],"AUTHOR COMMENTARY\nSeparate retrieval failure from generation failure. A fluent answer can be wrong because the right document never arrived, because the document itself is wrong, or because the model ignored it. Build cases for missing documents, conflicting rules, and a cited passage that does not support the sentence. Use deterministic citation checks alongside any model judge.",{status:"Evaluation design",docs:["evalmetrics"]});
L(5,"Lab 05: cited product guidance",[
 "Review two invoice rule documents against the domain code.",
 "Implement a search adapter with source identifiers.",
 "Return a grounded answer and a no-evidence response."
],[
 "Citations resolve to retrieved passages.",
 "Access filtering runs before inference.",
 "A stale or missing rule produces an explicit limitation."
],"AUTHOR COMMENTARY\nSuggested lab: 70 minutes. Seed one deliberately stale documentation claim, such as Paid invoices being cancellable. Students identify it through code review and retrieval tests. Require a small answer/citation corpus. They may use file or keyword retrieval for the core lab; vector or hybrid retrieval is an optional extension. Avoid spending the lab on infrastructure that the small corpus does not need.");

M(6,"Orchestration and interoperability","Explicit control over specialist work and external capabilities");
T("The baseline router",["Prompt","Current route","Reason"],[
 ["How do I create a customer?","Documentation","An earlier documentation phrase matches"],
 ["Show revenue summary","Analytics","Revenue or summary matches"],
 ["Show overdue invoices","Invoice operations","No earlier route matches"],
 ["Which customer owes the most?","Invoice operations","The phrase who owes is absent"]
],"AUTHOR COMMENTARY\nThe coordinator uses the latest raw prompt rather than the enriched conversation for routing. One agent executes per request. These cases demonstrate the limits of a hand-written substring classifier. They do not prove that a model-based router is automatically better. Require an evaluation corpus before replacing routing, and retain deterministic routing for explicit application actions.",{status:"Existing implementation",refs:["routing"],widths:[.46,.25,.29]});
T("Orchestration patterns",["Pattern","Useful InvoiceAI case","Control decision"],[
 ["Sequential","Read metrics, then write a narrative","Defined order"],
 ["Concurrent","Independent customer and invoice reads","Safe fan-out and aggregation"],
 ["Handoff","A specialist owns the next conversation turn","Contextual transfer"],
 ["Group chat / Magentic","A bounded report review exercise","Additional model coordination"]
],"AUTHOR COMMENTARY\nThese are framework-supported pattern families. They carry different state, termination, and cost implications. For the core capstone, choose the simplest explicit workflow that meets the business requirement. A group chat is an optional comparison, not a requirement for a billing lookup. Make completion criteria and maximum work explicit before enabling open-ended coordination.",{status:"Framework patterns",docs:["patterns"],widths:[.25,.45,.30]});
C("A typed workflow skeleton",[
 "using Microsoft.Agents.AI.Workflows;",
 "",
 "// Course executors implement the typed step contracts.",
 "WorkflowBuilder builder = new(readInvoice);",
 "builder.AddEdge(readInvoice, proposeTransition);",
 "builder.AddEdge(proposeTransition, requestReview);",
 "var workflow = builder.Build();",
 "",
 "// Add explicit outputs, events, and approval handling",
 "// using the pinned SDK's execution APIs."
],["Executors perform a step.","Edges control the path.","The application owns business completion."],"AUTHOR COMMENTARY\nThis intentionally stops before a version-sensitive complete executor implementation. Students create typed executors and run the graph using the selected SDK sample. Some steps are deterministic code and one may use an agent. A graph provides explicit control but does not automatically define database transaction or idempotency semantics. Explain output and event handling as separate concerns.",{status:"Framework code pattern",docs:["builder","workflows"]});
add("image","An approved invoice write workflow","approval.png","AUTHOR COMMENTARY\nThis is the proposed application workflow. The request first reads the invoice, produces a concrete transition proposal, and waits for a human decision. Approval is followed by a fresh authorization and domain-state check. The commit step uses application idempotency and an audit event. Rejecting the decision performs no write. Those application guarantees are not created simply by drawing the workflow.\nASK\nWhich data must bind the approval to this exact action?\nVISUAL\nGenerated conceptual workflow illustration.",{status:"Capstone target",docs:["hitl"],refs:["invoice","status"]});
P("Checkpointing and recovery",[
 ["Checkpoint", "Save workflow progress and pending requests through a chosen store."],
 ["Resume", "Restore the run after an interruption or external decision."],
 ["Revalidate", "Reload identity, permissions, and mutable business facts."],
 ["Protect writes", "Use idempotency at the application commit boundary."]
],"AUTHOR COMMENTARY\nDiscuss standard workflow checkpoints separately from Durable Task hosting. Store availability and runtime behavior vary by language and package, so verify the .NET storage APIs in the pinned version. Checkpointing execution state does not establish exactly-once external effects. A crash after a successful database write still needs an application-level deduplication strategy.",{status:"Workflow design",docs:["checkpoints"]});
P("Agents as tools",[
 ["Composition", "An outer agent can invoke a specialist through a function boundary."],
 ["Contract", "Supply only the specialist's required input and permitted context."],
 ["State", "Decide how conversation and evidence move between agents."],
 ["Limit", "Bound delegation depth, tool budget, and failure propagation."]
],"AUTHOR COMMENTARY\nUse a documentation specialist as the example. The outer agent delegates a well-defined question and receives an answer with evidence. Do not assume every child automatically inherits the parent's history or security state. Interactive or delayed approval may be better represented as a workflow. Keep cross-agent traces tied to the parent run so a reviewer can understand the work.",{status:"Composition option",docs:["delegation"]});
T("MCP, A2A, and AG-UI",["Protocol","Boundary","InvoiceAI extension"],[
 ["MCP","Agent to external tools and context","An approved document or ERP connector"],
 ["A2A","Application to a remote agent service","A separately operated collections assistant"],
 ["AG-UI","Agent to interactive user interface","Text, tool, approval, and state events"]
],"AUTHOR COMMENTARY\nThese protocols solve different integration problems and none is required merely to call a local invoice method. MCP introduces a new tool/service trust boundary. A2A adds remote-agent lifecycle and identity concerns. AG-UI is a user-interface event protocol. Select protocols based on the consumer, not because they are available. Review external services and data disclosure before enabling them.",{status:"Interoperability options",docs:["mcp","a2a","agui"],widths:[.17,.40,.43]});
P("Skills and the Harness Agent",[
 ["Skills", "Package reusable domain instructions and resources."],
 ["Harness", "Add planning, task tracking, compaction, and controlled file capabilities."],
 ["Scope", "Review extra autonomy against the business requirement."],
 ["InvoiceAI", "Keep the billing workflow narrow before adding general task execution."]
],"AUTHOR COMMENTARY\nThe framework includes skills and a more opinionated harness. Present these as advanced capabilities rather than mandatory capstone dependencies. A reviewed billing-explanation skill could package knowledge, but scripts and filesystem access introduce operational risk. The harness is useful for longer tasks; a short balance query generally needs much less machinery. Confirm current .NET support and approval defaults in the course version.",{status:"Advanced extension",docs:["skills","harness"]});
L(6,"Lab 06: a bounded workflow",[
 "Build a typed read, proposal, and review sequence.",
 "Define explicit success, rejection, and failure outputs.",
 "Pause for a decision and exercise a resume path."
],[
 "An unapproved proposal performs no write.",
 "The workflow has a bounded completion condition.",
 "A resumed action rechecks mutable business facts."
],"AUTHOR COMMENTARY\nSuggested lab: 70 minutes. Use a proposal record containing invoice ID, current status, target status, actor, and operation ID. The core lab may stop before commit, with the protected command added in module 8. Include a rejected decision and a simulated interruption. If teams compare orchestration patterns, require a justification for any added agents or parallel execution.");

M(7,"ASP.NET Core and user experience","A stable server boundary with real execution events");
P("The AI endpoint contract",[
 ["Request", "Message, continuation ID, and bounded client context."],
 ["Identity", "Resolve tenant and user from the authenticated server principal."],
 ["Run state", "Completed, awaiting approval, cancelled, or failed."],
 ["Response", "Answer, evidence, operation outcome, and correlation identifiers."]
],"AUTHOR COMMENTARY\nThe current endpoint validates a non-empty message and creates a short conversation ID if missing. It returns HTTP 200 after the coordinator returns even when agent IsSuccess is false. The target contract should represent pending approval and business outcomes explicitly. Decide on compatibility or versioning before replacing the response model. A continuation ID must never serve as authorization.",{status:"Existing contract and target",refs:["api","models"]});
C("The application adapter",[
 "// Course-owned boundary, independent of the UI protocol",
 "public interface IBillingAssistant",
 "{",
 "    Task<AssistantTurn> RunAsync(",
 "        AssistantInput input,",
 "        ActorContext actor,",
 "        CancellationToken ct);",
 "}",
 "",
 "// Implementation resolves authorized context,",
 "// restores AgentSession, runs AIAgent, and maps results."
],["Keep provider details behind the adapter.","Pass server-owned identity separately.","Expose business and execution outcomes."],"AUTHOR COMMENTARY\nThese are proposed application types, not new framework APIs. They avoid forcing the existing MVC application to understand every provider-specific response. Discuss an async event-stream overload for streaming. The adapter can preserve the existing HTTP contract during a staged migration. The implementation must keep request scope alive while tools execute and should persist any required run state before returning.",{status:"Capstone code design",refs:["web","fallback"]});
P("Hosting and DI integration",[
 ["Existing API", "Host the agent beside the application's established use cases."],
 ["Registration", "Choose named agent and workflow registrations deliberately."],
 ["Session store", "Bind continuation storage to authorized ownership."],
 ["Scope", "Verify tool dependencies under the chosen hosting lifetime."]
],"AUTHOR COMMENTARY\nMicrosoft's hosting helpers include named agent/workflow registration and optional hosted session storage. They are an integration option, not a replacement for application authentication and authorization. Reuse the existing API host for the core project to minimize deployment changes. Inspect the selected package's scope behavior instead of assuming it matches the baseline scoped custom agents.",{status:"Framework hosting option",docs:["hosting"]});
P("A user experience built on real events",[
 ["Text", "Render partial output and the authoritative completed answer."],
 ["Tools", "Show server-issued start, result, and failure events."],
 ["Approvals", "Display the exact invoice, action, and consequence."],
 ["Recovery", "Reconnect to persisted run state without replaying a write."]
],"AUTHOR COMMENTARY\nThe current Studio guesses tools from prompt and reply keywords and emits a toolExecuted browser event after a normal reply. Replace that inference with actual execution events. Use stable run, tool-call, and operation IDs. A browser may disconnect after a commit, so refresh business state from the server rather than reissuing the tool request. Sanitize model content before HTML rendering.",{status:"Capstone target",refs:["studio"]});
C("An AG-UI hosting option",[
 "using Microsoft.Agents.AI.Hosting.AGUI.AspNetCore;",
 "",
 "builder.Services.AddAGUIServer();",
 "",
 "// agent is the configured framework AIAgent.",
 "var app = builder.Build();",
 "app.MapAGUIServer(\"/ai/events\", agent);",
 "",
 "// Add application identity, policy, session storage,",
 "// and client event handling for the chosen deployment."
],["The adapter emits AG-UI events over SSE.","The client controls rendering.","Continuation identifiers still need isolation."],"AUTHOR COMMENTARY\nThis is the documented basic hosting pattern with a course-selected route. It is an optional alternative to a custom SSE adapter. Verify the exact prerelease hosting package and configure persisted hosted sessions when continuity must survive requests. The existing MVC proxy may need to forward streaming instead of buffering JSON. Do not claim that adding the route alone preserves sessions or enforces business permissions.",{status:"Framework code pattern",docs:["aguicode"]});
T("Interactive, background, and durable execution",["Mode","Suitable work","Required application decision"],[
 ["Interactive run","A short invoice lookup","Deadline and cancellation semantics"],
 ["Background response","A provider-supported long response","Persist continuation and status"],
 ["Durable workflow","A review that spans restarts or workers","Durable state and idempotent effects"],
 ["Managed or self-hosted","Deployment ownership","Who owns scaling, identity, and operations"]
],"AUTHOR COMMENTARY\nDistinguish a provider continuation token from a durable workflow checkpoint and from the application's operation ID. Their lifecycles differ. The Durable Extension can support Azure Functions or other worker hosting, but it introduces deployment and operational choices beyond the core HTTP integration. Do not imply that every agent or provider supports background responses.",{status:"Hosting choices",docs:["background","durable","hostchoice"],widths:[.25,.32,.43]});
L(7,"Lab 07: an end-to-end read experience",[
 "Connect the framework adapter to the AI API.",
 "Preserve a session across two web requests.",
 "Render actual run and tool events or a consistent JSON result."
],[
 "The browser uses server-issued identifiers.",
 "A read failure is visible as a failure.",
 "The answer and event stream share one run ID."
],"AUTHOR COMMENTARY\nSuggested lab: 70 minutes. Teams can retain the current MVC chat UI and use a custom event contract, or use the AG-UI option. Keep the first end-to-end slice read-only. Simulate a disconnected client and reload the final run outcome. Require an explicit decision about what a cancelled request means and which server work can continue.");

M(8,"Security and controlled writes","Permissions and decisions enforced before state changes");
P("The InvoiceAI threat model",[
 ["Untrusted input", "User messages, invoice notes, documents, and tool results."],
 ["Sensitive data", "Customer identities, invoice details, and conversation history."],
 ["Sensitive effects", "Status transitions, draft deletion, and external communication."],
 ["Enforcement", "Server identity, narrow tools, access checks, and reviewed actions."]
],"AUTHOR COMMENTARY\nUse a concrete invoice note containing an instruction to export customer details as the threat scenario. The text is data, even when it looks like a system message. The model can be manipulated, so permission checks must be independent of model compliance. Classify data leaving the API for the model, telemetry, and any external connector.",{status:"Threat-model exercise",docs:["trust"]});
P("Authorization and tenant isolation",[
 ["Principal", "Derive actor and tenant from authenticated server context."],
 ["Read policy", "Scope queries before results reach the model."],
 ["Write policy", "Reauthorize the exact operation immediately before commit."],
 ["Session policy", "Bind conversation, approval, and operation IDs to ownership."]
],"AUTHOR COMMENTARY\nThe inspected API Program and AI mapping do not implement a complete authenticated, tenant-scoped tool boundary. Treat the following controls as capstone work. Do not let the model choose a user ID or supply an unrestricted tenant ID. A UI antiforgery token protects a different concern from authorization. Add deterministic cross-tenant tests for both direct endpoint and tool execution paths.",{status:"Capstone requirement",refs:["program","api","coordinator"]});
P("Prompt injection and output handling",[
 ["Instructions", "Keep developer policy under server control."],
 ["Evidence", "Separate retrieved data from trusted instructions."],
 ["Capabilities", "Limit accessible functions and validate arguments."],
 ["Rendering", "Sanitize output and reject executable or unsupported payloads."]
],"AUTHOR COMMENTARY\nSecurity is a shared responsibility. Defensive prompts can reduce some attacks, but the application should enforce permissions and tool policy regardless of the model's answer. Test direct override prompts and indirect instructions in invoice notes and Markdown. Current Microsoft documentation marks FIDES experimental and Python-only, so it is not a .NET security dependency for this capstone. Use the documented .NET safety and approval approach.",{status:"Security design",docs:["safety","fides"]});
C("Framework function approval",[
 "AIFunction change = AIFunctionFactory.Create(",
 "    invoiceTools.UpdateInvoiceStatus);",
 "AIFunction reviewed = new ApprovalRequiredAIFunction(change);",
 "",
 "// Include reviewed in the agent's permitted tool list.",
 "var requests = response.Messages",
 "    .SelectMany(message => message.Contents)",
 "    .OfType<ToolApprovalRequestContent>();",
 "",
 "// Resume with each approved/rejected response",
 "// and the same authorized AgentSession."
],["Pause before the tool executes.","Render the actual tool call and arguments.","Continue handling pending decisions."],"AUTHOR COMMENTARY\nThe documented wrapper makes a function approval-required. The host inspects response content, obtains a decision, and resumes the same session with request.CreateResponse(approved) in a user ChatMessage. Authorization still precedes execution and must be repeated after a delay. Handle every pending request rather than assuming a run has only one approval.\nASK\nWhy should approval bind to the original tool call rather than an editable text description?",{status:"Framework code pattern",docs:["approval"]});
P("Idempotency and business concurrency",[
 ["Bind", "Approval covers actor, invoice, target state, and operation ID."],
 ["Recheck", "Reject a stale proposal when relevant state has changed."],
 ["Deduplicate", "Store the idempotency result at the commit boundary."],
 ["Audit", "Record the decision and business outcome without inventing success."]
],"AUTHOR COMMENTARY\nThis is application design beyond the framework approval wrapper. Choose a concurrency token or a versioned compare-and-update strategy appropriate to the database model. A repeated idempotency key must match the original operation payload and actor scope. Save the operation result consistently with the business mutation. Discuss the outbox pattern if external effects follow the commit. Do not promise universal exactly-once delivery.",{status:"Capstone requirement",refs:["status","save"]});
P("Data handling and external dependencies",[
 ["Minimize", "Send only the fields required for the task."],
 ["Redact", "Exclude secrets and unnecessary personal data from telemetry."],
 ["Review", "Check data flows to models, MCP services, and remote agents."],
 ["Retain", "Apply expiry and deletion policies to sessions and evidence."]
],"AUTHOR COMMENTARY\nHave students inventory destinations rather than making a blanket compliance claim. Provider hosting location, storage behavior, and enterprise requirements need review in the actual deployment. An external tool response can carry sensitive data or malicious instructions. Use synthetic training data for demonstrations and red-team cases. Require a data-flow note with the capstone.",{status:"Application policy",docs:["mcp","safety"]});
L(8,"Lab 08: an approved status transition",[
 "Add authorization and approval for a status tool.",
 "Bind the proposal to a known invoice and operation ID.",
 "Test rejection, stale state, duplicate submission, and a foreign actor."
],[
 "A rejected or unauthorized request performs no write.",
 "Draft-to-Paid still fails.",
 "A duplicate approved request returns the recorded outcome."
],"AUTHOR COMMENTARY\nSuggested lab: 70 minutes. Use disposable records and inspect state before and after each case. Test the command boundary without a model, then exercise the agent approval path. A plain Yes message must not authorize an unrelated operation. Simulate a second actor changing the invoice during review. Require evidence of the persisted result, not a screenshot of a confirmation sentence.");

M(9,"Operations and reliability","A traceable request with bounded latency and cost");
T("Observability signals",["Signal","Capture","InvoiceAI question"],[
 ["Trace","Run, model, tool, and application spans","Where did latency or failure occur?"],
 ["Metrics","Latency, tokens, tool outcomes, and quotas","Is the feature meeting its target?"],
 ["Audit","Authorized actor and business operation","What changed and who approved it?"],
 ["Evaluation","Quality on a defined case set","Did behavior regress?"]
],"AUTHOR COMMENTARY\nThese signals overlap but answer different questions. Use one correlation chain from the HTTP request through the model call and command. A tool-call trace should not be the only financial audit record. Avoid logging full prompts and customer records by default. Record enough identifiers to diagnose a run while keeping sensitive content controlled.",{status:"Operations design",docs:["telemetry"],widths:[.22,.43,.35]});
C("Framework telemetry integration",[
 "AIAgent observable = billing",
 "    .AsBuilder()",
 "    .UseOpenTelemetry(sourceName: \"InvoiceAI.Agents\")",
 "    .Build();",
 "",
 "// Configure the host's OpenTelemetry exporter",
 "// to collect this source and relevant meters.",
 "// Keep sensitive-data capture disabled by default."
],["Instrument the actual agent/tool path.","Correlate with application spans.","Choose an exporter for the deployment."],"AUTHOR COMMENTARY\nThis is the documented builder instrumentation pattern, condensed to emphasize safe defaults. Review the selected SDK for instrumentation at agent and chat-client layers to avoid duplicate telemetry. Exporter configuration belongs in the host. A local Aspire dashboard can help the lab, while the production destination is a deployment decision. Do not present model-generated explanations or the Studio keyword list as telemetry.",{status:"Framework code pattern",docs:["telemetry"],refs:["studio"]});
P("Resilience and bounded execution",[
 ["Deadline", "Apply an end-to-end run budget, not only a model timeout."],
 ["Retry", "Use bounded retry/backoff for suitable transient failures."],
 ["Write safety", "Do not retry uncertain effects without deduplication."],
 ["Degradation", "Expose an explicit unavailable or reduced-capability mode."]
],"AUTHOR COMMENTARY\nThe baseline web client has a 60-second AI timeout, while other work can add latency. A model SDK may already retry, so avoid multiplying retry layers without analysis. Reads and writes need different retry semantics. A runtime cloud failure is not the same as missing configuration. Define who owns cancellation, what persists, and what the UI shows if the request ends after a commit.",{status:"Application reliability policy",refs:["base","fallback"]});
P("The cost model",[
 ["Inference", "Input tokens multiplied by input rate, plus output tokens multiplied by output rate."],
 ["Tools", "Database, retrieval, connector, and extra agent work."],
 ["Storage", "History, evidence, indexes, traces, and checkpoints."],
 ["Control", "Per-run ceilings, quota alerts, caching policy, and model selection."]
],"AUTHOR COMMENTARY\nUse the actual provider price sheet when preparing a live cost exercise; no prices are asserted here. Compare a single-agent lookup with a multi-agent report in measured tokens and calls. Cache stable documentation differently from live balances. Include evaluation traffic in the budget. Any cache holding customer-specific results needs correct ownership and freshness keys.",{status:"Cost engineering"});
T("The baseline fallback behavior",["Path","Trigger","Operational caveat"],[
 ["Backend simulation","Missing configuration or null chat client","Some overrides run real database tools"],
 ["Live failure","Exception during the live agent call","Returns a failed agent response"],
 ["Web mock","AI API transport/envelope failure","Uses separate template and business API logic"],
 ["Studio inspector","Keyword inference from text","Does not prove a tool executed"]
],"AUTHOR COMMENTARY\nStudents should identify these existing behaviors before replacing them. Backend simulation is not a universal dry run: it can change invoice status. The live failure envelope normally uses HTTP 200, so the web fallback need not run. The mock can choose the first customer for a debtor question rather than computing the largest debtor. A production mode policy should make reduced capability explicit and observable.",{status:"Existing implementation",refs:["base","simulation","fallback","mock","studio"],widths:[.25,.35,.40]});
P("Deployment and production operations",[
 ["Release", "Prompts, packages, tools, retrieval data, and model configuration form one versioned release."],
 ["Health", "Separate process readiness from model and database capability."],
 ["Rollout", "Use feature flags, a small initial audience, and rollback criteria."],
 ["Response", "Keep runbooks for quota, latency, bad answers, and uncertain writes."]
],"AUTHOR COMMENTARY\nThe API catches database initialization failure at startup, so a running process alone does not establish usable business access. Add capability checks and a deployment-specific secret/identity strategy. The initial release can expose documentation and read tools before writes. A model or prompt rollback may be required even when the .NET binary did not change. Record ownership for incidents and customer corrections.",{status:"Capstone operations target",refs:["program"]});
L(9,"Lab 09: trace and fault-inject a run",[
 "Instrument a model call, a tool, and the application operation.",
 "Inject a database failure and a provider timeout.",
 "Measure a run budget and document a rollback path."
],[
 "One trace explains the actual execution.",
 "Sensitive data stays out of default telemetry.",
 "The UI distinguishes failure from a completed write."
],"AUTHOR COMMENTARY\nSuggested lab: 70 minutes. Use fakes or controlled faults rather than disrupting a shared service. Capture latency at the whole request and the model/tool boundaries. Students should detect that the baseline simulation stopwatch omits tool work, then measure correctly in the target. Require a short incident explanation using evidence rather than an inferred tool list.");

M(10,"Evaluation and the capstone","A demonstrable integration with behavioral release criteria");
P("The testing layers",[
 ["Deterministic", "Domain rules, validators, authorization, parsing, and idempotency."],
 ["Contract", "Tool schemas, output mapping, endpoint behavior, and events."],
 ["Behavioral", "Tool choice, argument accuracy, grounding, and clarification."],
 ["Operational", "Concurrency, cancellation, restart, quota, and partial failure."]
],"AUTHOR COMMENTARY\nUse fast deterministic tests for guarantees the application can enforce. Behavioral tests need a representative prompt corpus and tolerances for harmless wording changes. Do not use exact response strings as the sole measure of model quality. Framework evaluation can combine local checks and external evaluators; retain authoritative assertions for money, permissions, and side effects.",{status:"Evaluation strategy",docs:["evaluation"],refs:["domainTests","tests"]});
T("The current evaluator as a teaching case",["Condition","Current score/flag","Why the course improves it"],[
 ["Text includes I am not sure","Risk true, score 0.65","Uncertainty is not factual verification"],
 ["Answer under ten characters","Risk true, score 0.65","A short correct answer can fail"],
 ["Other answer","Score 0.95","A false answer can appear confident"],
 ["confidential_bypass appears","Policy false","The coordinator still returns the answer"]
],"AUTHOR COMMENTARY\nAgentEvaluationService is a deterministic string heuristic. It does not inspect query data, tool results, authorization, or the prompt. Present it as a baseline to replace or relabel, rather than claiming the application measures a 95 percent chance of correctness. The API exposes only the confidence value. Use this slide to distinguish a confidence UI label from an evaluated quality metric.",{status:"Existing implementation",refs:["evaluator","coordinator","api"],widths:[.35,.26,.39]});
T("A capstone evaluation corpus",["Case family","Example","Required assertion"],[
 ["Read accuracy","Summarize billing activity","Totals match the fixture"],
 ["Ambiguity","Create an invoice for Apex","Clarify missing/ambiguous details"],
 ["Rule enforcement","Mark a Draft as Paid","No illegal write"],
 ["Adversarial input","A document instructs data export","No unauthorized disclosure"],
 ["Recovery","Repeat an approved request","No duplicate business effect"]
],"AUTHOR COMMENTARY\nThis is a proposed corpus structure. Use synthetic fixtures and retain expected tool evidence. Add paraphrases, empty results, cancelled invoices, and conflicting context. Split development cases from a held-out regression set. Version each case with the prompt, tool definitions, model/deployment configuration, and expected business behavior. A model judge can supplement, but cannot replace, a deterministic unauthorized-write assertion.",{status:"Course evaluation design",widths:[.23,.38,.39]});
P("Behavioral evaluation and review",[
 ["Measure", "Tool selection, argument correctness, factual support, and task completion."],
 ["Separate", "Retrieval defects, model defects, and application failures."],
 ["Review", "Investigate failures with traces and source evidence."],
 ["Compare", "Evaluate the candidate against a fixed baseline before release."]
],"AUTHOR COMMENTARY\nExplain the limitations of automated judges and calibrate them against human review. A change can improve helpfulness while increasing unauthorized tool attempts or cost. Keep safety and correctness gates independent of a single aggregate score. Run enough cases to reveal patterns and report the dataset scope, rather than presenting a small sample as a universal reliability claim.",{status:"Evaluation method",docs:["evalmetrics"]});
P("Release gates",[
 ["Build and contracts", "Pinned dependencies compile and public contracts remain compatible."],
 ["Safety", "Authorization, rejection, and duplicate-write tests pass."],
 ["Behavior", "The agreed regression set meets the team's thresholds."],
 ["Operations", "Telemetry, budgets, rollout controls, and rollback are ready."]
],"AUTHOR COMMENTARY\nThese are course release requirements. Teams choose thresholds before reviewing the candidate results, and justify the risk trade-offs. Do not invent a universal acceptable accuracy percentage. Separate mandatory invariant tests from statistical quality targets. A prompt-only or model-only change still passes through evaluation and release review. Keep environment configuration and secret references outside the deck and source commits.",{status:"Capstone acceptance"});
T("Capstone implementation sequence",["Checkpoint","Deliverable","Retained boundary"],[
 ["1. Framework read slice","AIAgent invokes a verified summary tool","Existing query and SQL"],
 ["2. Context and knowledge","Owned sessions and cited guidance","Authoritative records and reviewed docs"],
 ["3. Controlled write","Approved, authorized status command","Invoice.TransitionTo and save"],
 ["4. Operations and evaluation","Execution events and regression evidence","Stable application/HTTP contract"]
],"AUTHOR COMMENTARY\nThe final project should integrate the preceding lab increments rather than rebuild them at the end. The invoice business system is the existing-system substrate. The course outcome replaces the custom runtime where appropriate, then adds the missing controls. Require a concise architecture decision record listing what was preserved, replaced, and deferred.",{status:"Capstone plan",refs:["base","create","invoice"],widths:[.25,.43,.32]});
P("The final capstone demonstration",[
 ["Explain", "Show cited guidance for an invoice rule."],
 ["Read", "Return a billing summary that matches authoritative data."],
 ["Act", "Approve a valid transition and reject an invalid or unauthorized one."],
 ["Recover", "Demonstrate restart or duplicate submission without inventing success."]
],"AUTHOR COMMENTARY\nGive teams a fixed scenario and a changed scenario they have not rehearsed verbatim. Require them to show framework package references, run identity, tool evidence, and the final invoice state. The baseline repository's custom agent names or UI animation do not establish that Microsoft Agent Framework is in use. Ask which behaviors are implemented and which remain proposed.",{status:"Demonstration requirements"});
T("Capstone assessment",["Dimension","Weight","Evidence"],[
 ["Integration and contracts","25%","Real framework runtime and preserved application boundaries"],
 ["Safety and correctness","30%","Validated tools, authorization, approval, and write controls"],
 ["Evidence and evaluation","25%","Behavioral corpus, grounded outputs, and real traces"],
 ["Delivery and explanation","20%","Reproducible setup, rollout plan, and technical walkthrough"]
],"AUTHOR COMMENTARY\nThis is a proposed course rubric totaling 100 percent. A serious authorization or business-rule defect requires remediation regardless of the weighted total. Review code and behavioral evidence, not only presentation quality. Give individual engineers a short change or failure investigation so group work does not hide gaps in understanding.",{status:"Suggested course rubric",widths:[.30,.16,.54]});
L(10,"Lab 10: the InvoiceAI capstone review",[
 "Assemble the lab increments into one reproducible branch.",
 "Run the agreed deterministic and behavioral checks.",
 "Deliver the demo, architecture decisions, and operational handoff."
],[
 "AIAgent and AgentSession drive the integration.",
 "Business effects have verifiable outcomes.",
 "The team can explain a failure and its recovery."
],"AUTHOR COMMENTARY\nSuggested lab: 70 minutes of integration review, with additional project work assigned outside contact hours as needed. Students submit configuration instructions without secrets, the package pins, focused tests, a behavior report, and a short runbook. The reviewer selects at least one unplanned edge case. Acknowledge deferred production work explicitly rather than overstating completeness.");

current=0;
T("Appendix: the package baseline",["Concern","Current repository","Course addition or decision"],[
 ["Inference","Azure.AI.OpenAI and Extensions.AI","Pin compatible provider packages"],
 ["Agent runtime","Custom BaseAgent","Microsoft.Agents.AI / OpenAI integration"],
 ["Workflow","Custom one-agent router","Microsoft.Agents.AI.Workflows as required"],
 ["Hosting / protocol","Minimal API JSON endpoint","Optional Hosting / AGUI packages"],
 ["Evaluation","Custom text heuristic","Selected framework / evaluation integration"]
],"AUTHOR COMMENTARY\nDo not paste an unreviewed latest-version list into a professional course. Microsoft examples and API references may use different package versions or prerelease components. Freeze one coherent set after compiling the supplied course project and recheck before teaching. The snippets in this deck are condensed implementation patterns, with exact API references in the notes. The presentation does not claim those snippets were compiled in this repository.",{status:"Preparation checklist",refs:["aiProject","packages"],docs:["api"],widths:[.23,.35,.42]});
C("Appendix: a minimal business agent",[
 "using Microsoft.Agents.AI;",
 "using Microsoft.Extensions.AI;",
 "using OpenAI.Chat;",
 "",
 "AIAgent agent = azureClient.GetChatClient(deployment)",
 "    .AsAIAgent(",
 "        name: \"InvoiceReadAgent\",",
 "        instructions: \"Use tools for current billing facts.\",",
 "        tools: [AIFunctionFactory.Create(",
 "            invoiceTools.GetInvoiceSummary)]);",
 "AgentSession state = await agent.CreateSessionAsync(ct);",
 "var reply = await agent.RunAsync(prompt, state,",
 "    cancellationToken: ct);"
],["Host supplies the configured provider and tool scope.","Application owns state persistence and authorization.","Keep source and course package versions aligned."],"AUTHOR COMMENTARY\nThis combines the earlier documented agent, tool, and session patterns. azureClient, deployment, invoiceTools, prompt, and ct are host-provided dependencies. It intentionally leaves out the surrounding API and persistence code. In the capstone, construct the agent within a lifetime that supports the tool's dispatcher dependencies. Show the request returning the real summary and discuss what additional enforcement is needed before writes.",{status:"Condensed framework pattern",docs:["api","sessions"],refs:["summaryTool"]});
P("Appendix: further capabilities",[
 ["Multimodal input", "Extract candidate invoice fields, then validate and review before creation."],
 ["Background work", "Persist continuation and job status for longer analyses."],
 ["Skills and hooks", "Review reusable instructions and governance support for the pinned SDK."],
 ["Provider evolution", "Re-evaluate capability, data, cost, and compatibility decisions."]
],"AUTHOR COMMENTARY\nThese are optional extensions, not claimed features of the current app. An image or document-extraction result should become a reviewed draft, not an automatic financial fact. Background agent delegation differs from provider background responses. Harness capabilities and hooks vary by version and language, so consult current .NET documentation before adding them. Keep the core course project small enough to explain and evaluate end to end.",{status:"Optional extension",docs:["capabilities","background","skills"]});
T("Appendix: the repository reading map",["Concern","Start here","Teaching purpose"],[
 ["Entry","AiEndpoints.cs","HTTP boundary and envelope"],
 ["Orchestration","AgentCoordinator.cs","History, routing, and evaluation"],
 ["Runtime","BaseAgent.cs and DependencyInjection.cs","Identify the framework migration seam"],
 ["Capabilities","InvoiceTools.cs and CustomerTools.cs","Function contracts and write exposure"],
 ["Rules","Invoice.cs and command handlers","Enforced business invariants"],
 ["Evidence","Read repositories and AiTests.cs","Metric definitions and test limits"]
],"AUTHOR COMMENTARY\nOpen files in this order for the final code tour, then follow one use case down to persistence. The exact local paths and cited line locations follow in the source notes for this slide. Students should distinguish comments describing intended capability from executable behavior. Avoid making the course a file-by-file reading exercise.",{status:"Code tour",refs:["api","coordinator","base","di","invoiceTools","customerTools","invoice","sql","tests"],widths:[.20,.48,.32]});
P("References: runtime and application integration",[
 ["Framework", "Overview, provider adapters, AIAgent, sessions, and structured outputs."],
 ["Application", "Function tools, middleware, context providers, and retrieval."],
 ["Course baseline", "Microsoft Learn references checked on 17 October 2026."],
 ["Implementation", "Exact documentation URLs and repository locations are in slide notes."]
],"AUTHOR COMMENTARY\nUse these references to prepare a version-pinned course environment. The main deck distinguishes existing repository behavior from framework capabilities and proposed application controls. Online documentation is a moving reference; record the exact course dependencies and recheck code samples before each cohort.",{status:"Primary sources",docs:["overview","azure","api","sessions","tools","structured","middleware","context","rag","extensions"]});
P("References: orchestration and production",[
 ["Orchestration", "WorkflowBuilder, patterns, human input, checkpoints, and delegation."],
 ["Integration", "MCP, A2A, AG-UI, hosting, and durable execution."],
 ["Production", "Safety, tool approvals, observability, and evaluation."],
 ["Extensions", "Skills, Harness Agent, background responses, and capability limits."]
],"AUTHOR COMMENTARY\nThe notes contain the official primary references for these topics. Select the .NET tabs and pinned package documentation when a page covers several languages. In particular, do not transfer Python-only storage or security APIs into a .NET example. Proposed course controls such as application idempotency and tenant authorization remain responsibilities of the application design.",{status:"Primary sources",docs:["builder","patterns","hitl","checkpoints","delegation","mcp","a2a","agui","hosting","durable","safety","approval","telemetry","evaluation","skills","harness","fides"]});
add("closing","InvoiceAI as an\nengineered AI integration",["A real framework runtime","Existing business rules","Verifiable outcomes"],"AUTHOR COMMENTARY\nClose by asking each engineer to describe how they would integrate the same pattern into their own system. The transferable skill is defining the boundary, exposing appropriate operations, managing state, and measuring outcomes. InvoiceAI is the capstone evidence that they can combine those decisions in a working .NET application.\nFINAL DISCUSSION\nWhich feature would you ship first, which would you defer, and what evidence would change that decision?",{status:"Course close"});

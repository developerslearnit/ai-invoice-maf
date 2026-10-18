// tools/deck-generator/slides-part3.js
const { COLORS, FONTS, addSlideHeader, addCodingLabHeader, addSlideFooter, formatNotes } = require('./build-deck');

function createPart3Slides(pres) {
  // ==========================================
  // SLIDE 36: Module 8 - Single Agent vs Swarm
  // ==========================================
  const s36 = pres.addSlide();
  s36.background = { color: COLORS.LIGHT_BG };
  addSlideHeader(s36, 'Module 8: Specialized Agents', 'Single Monolithic Agent vs. Specialized Swarm', 'Applying Domain-Driven Design (DDD) Bounded Contexts to AI agent architecture.');
  addSlideFooter(s36, 36, 70);

  s36.addShape(pres.ShapeType.roundRect, {
    x: 0.8, y: 1.95, w: 5.7, h: 4.8,
    rectRadius: 0.1,
    fill: { color: COLORS.WHITE },
    line: { color: COLORS.DEMO_RED, width: 1.5 }
  });
  s36.addText('ANTI-PATTERN: THE MONOLITHIC "GOD AGENT"', {
    x: 1.0, y: 2.15, w: 5.3, h: 0.35,
    fontSize: 11, bold: true, color: COLORS.DEMO_RED, fontFace: FONTS.BODY
  });
  s36.addText('• 1 Massive System Prompt: 3,000 words mixing invoicing, customer onboarding, financial KPIs, and user manuals.\n• 35 Tools Registered Simultaneously: Model becomes confused, picking the wrong tool or hallucinating parameters.\n• Context Pollution: Large schemas consume 4,000 input tokens on every turn before the user even speaks.\n• Fragile Behavioral Tuning: Modifying prompt instructions to fix customer queries breaks invoice creation.\n• Violation of Single Responsibility Principle (SRP).', {
    x: 1.0, y: 2.55, w: 5.3, h: 3.8,
    fontSize: 10.5, color: COLORS.TEXT_DARK, fontFace: FONTS.BODY
  });

  s36.addShape(pres.ShapeType.roundRect, {
    x: 6.8, y: 1.95, w: 5.73, h: 4.8,
    rectRadius: 0.1,
    fill: { color: COLORS.WHITE },
    line: { color: COLORS.SUCCESS_GREEN, width: 1.5 }
  });
  s36.addText('RECOMMENDED: SPECIALIZED BOUNDED AGENTS', {
    x: 7.0, y: 2.15, w: 5.3, h: 0.35,
    fontSize: 11, bold: true, color: COLORS.SUCCESS_GREEN, fontFace: FONTS.BODY
  });
  s36.addText('• Bounded Responsibility: Each agent has a single, well-defined persona and operational domain.\n• Focused Toolsets: Agents only bind 3 to 6 domain tools directly relevant to their capabilities.\n• Minimal Token Footprint: System instructions are concise (150-250 tokens), keeping inference fast.\n• Independent Testing & Evolution: Can update DocumentationAgent without risking regressions in InvoiceAssistantAgent.\n• Clean DDD Alignment: Agents map directly to domain aggregates.', {
    x: 7.0, y: 2.55, w: 5.3, h: 3.8,
    fontSize: 10.5, color: COLORS.TEXT_DARK, fontFace: FONTS.BODY
  });

  s36.addNotes(formatNotes({
    explanation: 'Explain why monolithic agents fail in enterprise systems. DDD bounded contexts apply to agents.',
    transition: 'Let us inspect the 4 specialized agents implemented in InvoiceApp.AI.'
  }));

  // ==========================================
  // SLIDE 37: Module 8 - The 4 Specialized Agents
  // ==========================================
  const s37 = pres.addSlide();
  s37.background = { color: COLORS.LIGHT_BG };
  addSlideHeader(s37, 'Module 8: Specialized Agents', 'The 4 Autonomous Agents of InvoiceApp.AI', 'Specialized roles, bound toolsets, and architectural responsibilities in the reference system.');
  addSlideFooter(s37, 37, 70);

  const swarmAgents37 = [
    {
      name: 'InvoiceAssistantAgent',
      role: 'INVOICE OPERATIONS',
      color: COLORS.AZURE_BLUE,
      tools: 'CreateInvoice, GetInvoiceByNumber, UpdateInvoiceStatus, DeleteDraftInvoice',
      desc: 'Creates itemized invoices, resolves customer names, manages state transitions (Sent, Paid, Overdue, Cancelled), enforces draft-only deletions.'
    },
    {
      name: 'CustomerOperationsAgent',
      role: 'CUSTOMER DIRECTORY',
      color: COLORS.SKY_BLUE,
      tools: 'CreateCustomer, GetCustomerById, UpdateCustomer, ListCustomers',
      desc: 'Manages client directory onboarding, validates unique billing emails, retrieves customer contact cards and financial ledger history.'
    },
    {
      name: 'AnalyticsAgent',
      role: 'FINANCIAL METRICS & KPIS',
      color: COLORS.SUCCESS_GREEN,
      tools: 'GetInvoiceSummary, GetOverdueInvoices, GetCustomerSummary',
      desc: 'Computes executive billing summaries, analyzes overdue debt distribution, identifies top debtors, and tracks cash collected in Naira (₦).'
    },
    {
      name: 'DocumentationAgent',
      role: 'PLATFORM KNOWLEDGE & RAG',
      color: '8B5CF6',
      tools: 'ReadDocumentationPage',
      desc: 'Answers questions on platform rules, user guides, deletion invariants, and state transition workflows by dynamically retrieving Docs/ Markdown.'
    }
  ];

  swarmAgents37.forEach((ag, idx) => {
    const isRight = idx >= 2;
    const xPos = isRight ? 6.8 : 0.8;
    const yPos = 2.05 + (idx % 2) * 2.35;

    s37.addShape(pres.ShapeType.roundRect, {
      x: xPos, y: yPos, w: 5.73, h: 2.15,
      rectRadius: 0.1,
      fill: { color: COLORS.WHITE },
      line: { color: COLORS.BORDER_LIGHT, width: 1 }
    });
    s37.addShape(pres.ShapeType.roundRect, {
      x: xPos + 0.15, y: yPos + 0.15, w: 5.43, h: 0.35,
      rectRadius: 0.06,
      fill: { color: COLORS.AZURE_LIGHT }
    });
    s37.addText(`${ag.role}  •  ${ag.name}`, {
      x: xPos + 0.25, y: yPos + 0.15, w: 5.2, h: 0.35,
      fontSize: 10, bold: true, color: ag.color, fontFace: FONTS.BODY, valign: 'middle'
    });

    s37.addText('Bound Tools:', {
      x: xPos + 0.25, y: yPos + 0.58, w: 1.2, h: 0.25,
      fontSize: 9.5, bold: true, color: COLORS.TEXT_MUTED, fontFace: FONTS.BODY
    });
    s37.addText(ag.tools, {
      x: xPos + 1.45, y: yPos + 0.58, w: 4.1, h: 0.45,
      fontSize: 9.5, bold: true, color: COLORS.TEXT_DARK, fontFace: FONTS.CODE
    });

    s37.addText(ag.desc, {
      x: xPos + 0.25, y: yPos + 1.05, w: 5.2, h: 0.95,
      fontSize: 10, color: COLORS.TEXT_DARK, fontFace: FONTS.BODY
    });
  });

  s37.addNotes(formatNotes({
    explanation: 'Walk through the 4 specialized agents and their bound toolsets.',
    transition: 'Let us inspect the foundational BaseAgent class that all 4 agents inherit from.'
  }));

  // ==========================================
  // SLIDE 38: Module 8 - BaseAgent Deep Dive
  // ==========================================
  const s38 = pres.addSlide();
  s38.background = { color: COLORS.LIGHT_BG };
  addSlideHeader(s38, 'Module 8: Specialized Agents', 'BaseAgent Pattern: Encapsulating Common Plumbing', 'Abstract template class providing telemetry, tool execution, logging, and simulation fallbacks.');
  addSlideFooter(s38, 38, 70);

  s38.addShape(pres.ShapeType.roundRect, {
    x: 0.8, y: 1.95, w: 6.8, h: 4.8,
    rectRadius: 0.1,
    fill: { color: COLORS.CODE_BG },
    line: { color: '334155', width: 1 }
  });
  const baseCode38 = `// src/InvoiceApp.AI/Agents/BaseAgent.cs (Simplified)
public abstract class BaseAgent : IBaseAgent
{
    public string Name { get; }
    public string Instructions { get; }
    public IReadOnlyList<AITool> Tools { get; }

    public virtual async Task<AgentResponse> ExecuteAsync(
        AgentRequest request, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();

        // 1. Simulation Fallback check
        if (_simulationMode.IsEnabled || _chatClient is null) {
            return await ExecuteSimulatedAsync(request, sw, ct);
        }

        try {
            var options = new ChatOptions {
                Tools = _tools, Temperature = 0.2f
            };
            var messages = new List<ChatMessage> {
                new(ChatRole.System, Instructions),
                new(ChatRole.User, request.Prompt)
            };

            var response = await _chatClient.GetResponseAsync(
                messages, options, ct);
            sw.Stop();

            int tokens = (int)(response.Usage?.TotalTokenCount ?? 0);
            _telemetry.TrackAgentExecution(Name, sw.ElapsedMilliseconds, tokens);

            return AgentResponse.Success(response.Text, Name, tokens, sw.ElapsedMilliseconds);
        }
        catch (Exception ex) {
            sw.Stop();
            _logger.LogError(ex, "Agent failure: {Agent}", Name);
            return AgentResponse.Failure(ex.Message, Name, sw.ElapsedMilliseconds);
        }
    }
}`;
  s38.addText(baseCode38, {
    x: 1.0, y: 2.1, w: 6.4, h: 4.5,
    fontSize: 8.5, fontFace: FONTS.CODE, color: COLORS.CODE_TEXT
  });

  s38.addShape(pres.ShapeType.roundRect, {
    x: 7.8, y: 1.95, w: 4.73, h: 4.8,
    rectRadius: 0.1,
    fill: { color: COLORS.WHITE },
    line: { color: COLORS.BORDER_LIGHT, width: 1 }
  });
  s38.addText('BASEAGENT ARCHITECTURAL PATTERN', {
    x: 8.0, y: 2.15, w: 4.3, h: 0.35,
    fontSize: 11, bold: true, color: COLORS.AZURE_BLUE, fontFace: FONTS.BODY
  });
  const basePoints38 = [
    { title: 'Template Method Pattern', desc: 'BaseAgent encapsulates the complex inference, stopwatch, exception handling, and telemetry logic once.' },
    { title: 'Clean Derived Classes', desc: 'Derived agents (e.g. AnalyticsAgent) only need to supply their instructions, name, and bound tools in their constructor.' },
    { title: 'Standardized AgentResponse', desc: 'Always returns AgentResponse(IsSuccess, Content, AgentName, TotalTokens, DurationMs). Zero unhandled exceptions.' },
    { title: 'Extensible Simulation Hook', desc: 'Provides protected virtual GenerateSimulatedResponseAsync for agent-specific offline mock behavior.' }
  ];
  basePoints38.forEach((bp, idx) => {
    s38.addText(`•  ${bp.title}`, {
      x: 8.0, y: 2.6 + idx * 1.0, w: 4.3, h: 0.28,
      fontSize: 10.5, bold: true, color: COLORS.SKY_BLUE, fontFace: FONTS.BODY
    });
    s38.addText(bp.desc, {
      x: 8.2, y: 2.88 + idx * 1.0, w: 4.1, h: 0.65,
      fontSize: 9.5, color: COLORS.TEXT_DARK, fontFace: FONTS.BODY
    });
  });

  s38.addNotes(formatNotes({
    explanation: 'Walk through BaseAgent.cs. Emphasize how stopwatch, error handling, and telemetry are shared.',
    transition: 'Let us now live-code Lab 08: Implementing BaseAgent and specialized agents.'
  }));

  // ==========================================
  // SLIDE 39: [CODING LAB 08] Implement BaseAgent & Swarm
  // ==========================================
  const s39 = pres.addSlide();
  s39.background = { color: COLORS.LIGHT_BG };
  addCodingLabHeader(s39, '08', 'Implement BaseAgent.cs & 4 Specialized Agents', 'Code the abstract BaseAgent template, build CustomerOperationsAgent & AnalyticsAgent, register in DI.');
  addSlideFooter(s39, 39, 70);

  s39.addShape(pres.ShapeType.roundRect, {
    x: 0.8, y: 1.95, w: 5.7, h: 4.8,
    rectRadius: 0.1,
    fill: { color: COLORS.WHITE },
    line: { color: COLORS.BORDER_LIGHT, width: 1 }
  });
  s39.addText('INSTRUCTOR LIVE CODING CHECKLIST', {
    x: 1.0, y: 2.15, w: 5.3, h: 0.35,
    fontSize: 11, bold: true, color: COLORS.SUCCESS_GREEN, fontFace: FONTS.BODY
  });
  const lab8Steps = [
    '1. Create src/InvoiceApp.AI/Agents/IBaseAgent.cs contract',
    '2. Implement src/InvoiceApp.AI/Agents/BaseAgent.cs with ExecuteAsync template method',
    '3. Add stopwatch measurement and ITelemetryService call',
    '4. Add simulation fallback hook: GenerateSimulatedResponseAsync',
    '5. Create CustomerOperationsAgent.cs: binds CustomerTools for onboarding & searching',
    '6. Create AnalyticsAgent.cs: binds InvoiceTools summary & overdue tools for KPIs',
    '7. In DependencyInjection.cs, register all agents as Scoped<IBaseAgent, TAgent>()'
  ];
  lab8Steps.forEach((st, idx) => {
    s39.addText(st, {
      x: 1.0, y: 2.52 + idx * 0.52, w: 5.3, h: 0.48,
      fontSize: 9.5, color: COLORS.TEXT_DARK, fontFace: FONTS.BODY
    });
  });

  s39.addShape(pres.ShapeType.roundRect, {
    x: 6.8, y: 1.95, w: 5.73, h: 4.8,
    rectRadius: 0.1,
    fill: { color: COLORS.CODE_BG },
    line: { color: '334155', width: 1 }
  });
  const lab8Code = `// src/InvoiceApp.AI/Agents/AnalyticsAgent.cs (Live Code)
public sealed class AnalyticsAgent : BaseAgent
{
    public AnalyticsAgent(
        IChatClient? chatClient,
        InvoiceTools invoiceTools,
        CustomerTools customerTools,
        ITelemetryService telemetry,
        ILogger<AnalyticsAgent> logger,
        IOptions<AzureOpenAISettings> options,
        SimulationMode simulationMode)
        : base(
            chatClient,
            instructions:
            """
            You are the Billing Analytics Agent for InvoiceAI.
            Your responsibilities include:
            1. Summarizing overall company revenue using GetInvoiceSummary.
            2. Identifying top debtors using GetOverdueInvoices.
            3. Reporting customer financial health via GetCustomerSummary.
            4. Present all monetary figures in Nigerian Naira (₦).
            """,
            name: "Billing Analytics Agent",
            description: "Analyzes revenue, overdue balances, and customer financial KPIs.",
            tools: invoiceTools.GetTools().Concat(customerTools.GetTools()),
            telemetry: telemetry, logger: logger, simulationMode: simulationMode)
    { }
}`;
  s39.addText(lab8Code, {
    x: 7.0, y: 2.15, w: 5.3, h: 4.4,
    fontSize: 8, fontFace: FONTS.CODE, color: COLORS.CODE_TEXT
  });

  s39.addNotes(formatNotes({
    explanation: 'Live code BaseAgent.cs and AnalyticsAgent.cs. Show how clean derived classes are when common execution logic is in BaseAgent.',
    whatToEmphasize: 'Show students how all agents implement IBaseAgent, allowing the coordinator to receive IEnumerable<IBaseAgent> via DI.',
    transition: 'Who coordinates these specialized agents? Enter Module 9: The Agent Coordinator.'
  }));

  // ==========================================
  // SLIDE 40: Module 9 - The Orchestrator Pattern
  // ==========================================
  const s40 = pres.addSlide();
  s40.background = { color: COLORS.LIGHT_BG };
  addSlideHeader(s40, 'Module 9: Agent Coordinator', 'The Supervisor Orchestrator Pattern', 'The central routing hub managing memory enrichment, agent dispatching, and evaluations.');
  addSlideFooter(s40, 40, 70);

  s40.addShape(pres.ShapeType.roundRect, {
    x: 0.8, y: 1.95, w: 11.73, h: 4.8,
    rectRadius: 0.1,
    fill: { color: COLORS.WHITE },
    line: { color: COLORS.BORDER_LIGHT, width: 1 }
  });

  const coordSteps40 = [
    { num: '01', title: 'Memory Enrichment', desc: 'Fetches recent conversation turns from IAgentMemoryService' },
    { num: '02', title: 'Semantic Routing', desc: 'Evaluates prompt & context to select the best specialized agent' },
    { num: '03', title: 'Agent Execution', desc: 'Dispatches enriched prompt & screen context to target agent' },
    { num: '04', title: 'Quality Evaluation', desc: 'Audits response via IAgentEvaluationService for hallucinations' },
    { num: '05', title: 'State & Telemetry', desc: 'Appends turns to memory and emits execution metrics' }
  ];

  coordSteps40.forEach((cs, idx) => {
    const xPos = 1.1 + idx * 2.25;
    s40.addShape(pres.ShapeType.roundRect, {
      x: xPos, y: 2.3, w: 2.05, h: 3.8,
      rectRadius: 0.1,
      fill: { color: idx === 1 ? COLORS.AZURE_LIGHT : 'F8FAFC' },
      line: { color: idx === 1 ? COLORS.AZURE_BLUE : COLORS.BORDER_LIGHT, width: idx === 1 ? 2 : 1 }
    });
    s40.addText(cs.num, {
      x: xPos + 0.1, y: 2.5, w: 1.85, h: 0.45,
      fontSize: 20, bold: true, color: idx === 1 ? COLORS.AZURE_BLUE : COLORS.TEXT_MUTED,
      fontFace: FONTS.TITLE, align: 'center'
    });
    s40.addText(cs.title, {
      x: xPos + 0.1, y: 3.1, w: 1.85, h: 0.55,
      fontSize: 11, bold: true, color: COLORS.TEXT_DARK,
      fontFace: FONTS.BODY, align: 'center'
    });
    s40.addText(cs.desc, {
      x: xPos + 0.1, y: 3.8, w: 1.85, h: 1.8,
      fontSize: 9.5, color: COLORS.TEXT_MUTED,
      fontFace: FONTS.BODY, align: 'center'
    });
  });

  s40.addNotes(formatNotes({
    explanation: 'Explain the 5-stage pipeline executed by AgentCoordinator.cs.',
    transition: 'Let us inspect the implementation of AgentCoordinator.cs.'
  }));

  // ==========================================
  // SLIDE 41: Module 9 - Coordinator Code Walkthrough
  // ==========================================
  const s41 = pres.addSlide();
  s41.background = { color: COLORS.LIGHT_BG };
  addSlideHeader(s41, 'Module 9: Agent Coordinator', 'Code Walkthrough: AgentCoordinator.cs', 'Tracing memory retrieval, semantic routing, evaluation, and telemetry tracking.');
  addSlideFooter(s41, 41, 70);

  s41.addShape(pres.ShapeType.roundRect, {
    x: 0.8, y: 1.95, w: 6.8, h: 4.8,
    rectRadius: 0.1,
    fill: { color: COLORS.CODE_BG },
    line: { color: '334155', width: 1 }
  });
  const coordCode41 = `// src/InvoiceApp.AI/Coordinator/AgentCoordinator.cs
public async Task<CoordinatorResult> ProcessAsync(
    string conversationId, string userPrompt,
    IReadOnlyDictionary<string, object>? context = null,
    CancellationToken ct = default)
{
    // 1. Enrich prompt with sliding window history
    var history = await memoryService.GetConversationHistoryAsync(
        conversationId, ct);
    var enrichedPrompt = string.IsNullOrWhiteSpace(history) 
        ? userPrompt : $"{history}\\nUser: {userPrompt}";

    // 2. Route to specialized agent
    var targetAgent = RouteToAgent(userPrompt);

    // 3. Execute target agent with UI context
    var request = new AgentRequest(enrichedPrompt, conversationId, context);
    var response = await targetAgent.ExecuteAsync(request, ct);

    // 4. Audit response with evaluation service
    var evaluation = await evaluationService.EvaluateResponseAsync(
        userPrompt, response.Content, ct);

    // 5. Append turns to memory & emit telemetry
    await memoryService.AppendMessageAsync(conversationId, "User", userPrompt, ct);
    await memoryService.AppendMessageAsync(conversationId, response.AgentName, response.Content, ct);
    telemetryService.TrackAgentExecution("Coordinator", response.DurationMs, response.TotalTokens);

    return new CoordinatorResult(response, evaluation);
}`;
  s41.addText(coordCode41, {
    x: 1.0, y: 2.1, w: 6.4, h: 4.5,
    fontSize: 8.5, fontFace: FONTS.CODE, color: COLORS.CODE_TEXT
  });

  s41.addShape(pres.ShapeType.roundRect, {
    x: 7.8, y: 1.95, w: 4.73, h: 4.8,
    rectRadius: 0.1,
    fill: { color: COLORS.WHITE },
    line: { color: COLORS.BORDER_LIGHT, width: 1 }
  });
  s41.addText('COORDINATOR DESIGN HIGHLIGHTS', {
    x: 8.0, y: 2.15, w: 4.3, h: 0.35,
    fontSize: 11, bold: true, color: COLORS.AZURE_BLUE, fontFace: FONTS.BODY
  });
  const coordHighlights41 = [
    { title: 'Immutable CoordinatorResult', desc: 'Encapsulates both the AgentResponse and the AgentEvaluationResult, giving callers full visibility into response quality.' },
    { title: 'Context Forwarding', desc: 'Optional context dictionary (current invoice ID, route) is forwarded into AgentRequest for screen awareness.' },
    { title: 'Dual-Turn Memory Storage', desc: 'Persists both user prompt and agent response with exact speaker attribution for multi-turn coherence.' },
    { title: 'Isolated Failure Domain', desc: 'If an agent fails, the coordinator can catch the failure and return a graceful fallback without crashing.' }
  ];
  coordHighlights41.forEach((ch, idx) => {
    s41.addText(`•  ${ch.title}`, {
      x: 8.0, y: 2.6 + idx * 1.0, w: 4.3, h: 0.28,
      fontSize: 10.5, bold: true, color: COLORS.SKY_BLUE, fontFace: FONTS.BODY
    });
    s41.addText(ch.desc, {
      x: 8.2, y: 2.88 + idx * 1.0, w: 4.1, h: 0.65,
      fontSize: 9.5, color: COLORS.TEXT_DARK, fontFace: FONTS.BODY
    });
  });

  s41.addNotes(formatNotes({
    explanation: 'Walk through lines 71-104 of AgentCoordinator.cs. Point out the clear 5-stage lifecycle.',
    transition: 'Let us examine routing strategies in Slide 42.'
  }));

  // ==========================================
  // SLIDE 42: Module 9 - Routing Strategies
  // ==========================================
  const s42 = pres.addSlide();
  s42.background = { color: COLORS.LIGHT_BG };
  addSlideHeader(s42, 'Module 9: Agent Coordinator', 'Routing Strategies: Heuristic vs. Semantic vs. LLM', 'Comparing intent routing techniques for speed, latency, cost, and classification accuracy.');
  addSlideFooter(s42, 42, 70);

  const routers42 = [
    {
      title: 'Strategy 1: Keyword Heuristics',
      badge: 'USED IN INVOICEAI REFERENCE',
      color: COLORS.AZURE_BLUE,
      speed: '< 0.1 ms (Instant)',
      cost: '$0.00 (Zero tokens)',
      desc: 'Checks prompt substrings ("how to", "revenue", "customer", "invoice"). Simple, blazing fast, 100% deterministic, perfect for training & known workflows. Can miss nuanced rephrasing.'
    },
    {
      title: 'Strategy 2: Vector Embedding Classifier',
      badge: 'ENTERPRISE PRODUCTION',
      color: COLORS.SKY_BLUE,
      speed: '15 - 35 ms',
      cost: 'Negligible (~$0.00002)',
      desc: 'Embeds user prompt via text-embedding-3-small and computes cosine similarity against pre-embedded agent capability vectors. Highly robust to linguistic variations without full LLM latency.'
    },
    {
      title: 'Strategy 3: Lightweight Routing LLM',
      badge: 'COMPLEX DYNAMIC SWARMS',
      color: COLORS.SUCCESS_GREEN,
      speed: '250 - 600 ms',
      cost: 'Low (~$0.0001 with gpt-4o-mini)',
      desc: 'Calls a small, fast model with strict structured output (e.g. enum TargetAgent) to reason about ambiguous intents. Best for complex multi-domain workflows, but adds latency.'
    }
  ];

  routers42.forEach((rt, idx) => {
    const xPos = 0.8 + idx * 4.0;
    s42.addShape(pres.ShapeType.roundRect, {
      x: xPos, y: 2.05, w: 3.73, h: 4.7,
      rectRadius: 0.1,
      fill: { color: COLORS.WHITE },
      line: { color: COLORS.BORDER_LIGHT, width: 1 }
    });
    s42.addShape(pres.ShapeType.roundRect, {
      x: xPos + 0.15, y: 2.2, w: 3.43, h: 0.65,
      rectRadius: 0.08,
      fill: { color: rt.color === COLORS.SUCCESS_GREEN ? COLORS.SUCCESS_BG : COLORS.AZURE_LIGHT }
    });
    s42.addText(rt.badge, {
      x: xPos + 0.2, y: 2.25, w: 3.33, h: 0.25,
      fontSize: 9, bold: true, color: rt.color, fontFace: FONTS.BODY
    });
    s42.addText(rt.title, {
      x: xPos + 0.2, y: 2.5, w: 3.33, h: 0.35,
      fontSize: 11.5, bold: true, color: COLORS.TEXT_DARK, fontFace: FONTS.BODY
    });

    s42.addText(`Latency: ${rt.speed}`, {
      x: xPos + 0.2, y: 3.05, w: 3.33, h: 0.25,
      fontSize: 10, bold: true, color: COLORS.TEXT_DARK, fontFace: FONTS.BODY
    });
    s42.addText(`Cost: ${rt.cost}`, {
      x: xPos + 0.2, y: 3.3, w: 3.33, h: 0.25,
      fontSize: 10, bold: true, color: COLORS.TEXT_MUTED, fontFace: FONTS.BODY
    });

    s42.addText('ANALYSIS & TRADE-OFFS:', {
      x: xPos + 0.2, y: 3.75, w: 3.33, h: 0.25,
      fontSize: 9, bold: true, color: COLORS.TEXT_MUTED, fontFace: FONTS.BODY
    });
    s42.addText(rt.desc, {
      x: xPos + 0.2, y: 4.05, w: 3.33, h: 2.5,
      fontSize: 10, color: COLORS.TEXT_DARK, fontFace: FONTS.BODY
    });
  });

  s42.addNotes(formatNotes({
    explanation: 'Compare the 3 routing strategies.',
    transition: 'Let us engage in our discussion on agent granularity.'
  }));

  // ==========================================
  // SLIDE 43: Module 9 - [DISCUSSION] Agent Granularity
  // ==========================================
  const s43 = pres.addSlide();
  s43.background = { color: COLORS.LIGHT_BG };
  addSlideHeader(s43, 'Module 9: Architecture Workshop', 'Discussion: How Granular Should Agents Be?', 'Avoiding premature swarm proliferation: One agent with 4 tools vs. 4 separate agents.');
  addSlideFooter(s43, 43, 70);

  s43.addShape(pres.ShapeType.roundRect, {
    x: 0.8, y: 1.95, w: 11.73, h: 4.8,
    rectRadius: 0.1,
    fill: { color: COLORS.WHITE },
    line: { color: COLORS.AZURE_BLUE, width: 2 }
  });

  s43.addText('ARCHITECTURAL CHALLENGE: INVOICE LIFECYCLE CAPABILITIES', {
    x: 1.1, y: 2.2, w: 11.1, h: 0.35,
    fontSize: 13, bold: true, color: COLORS.AZURE_BLUE, fontFace: FONTS.BODY
  });
  s43.addText('A development team is tasked with building 4 related invoice capabilities:\n1. Search invoices by customer or status\n2. Calculate revenue statistics & overdue debt\n3. Transition invoice status (Draft -> Sent -> Paid)\n4. Generate PDF invoice download links', {
    x: 1.1, y: 2.6, w: 11.1, h: 1.1,
    fontSize: 11, color: COLORS.TEXT_DARK, fontFace: FONTS.BODY
  });

  s43.addText('PROPOSAL A: CREATE 4 DEDICATED AGENTS', {
    x: 1.1, y: 3.8, w: 5.4, h: 0.3,
    fontSize: 11, bold: true, color: COLORS.DEMO_RED, fontFace: FONTS.BODY
  });
  s43.addText('• InvoiceSearchAgent\n• InvoiceStatsAgent\n• InvoiceStatusAgent\n• InvoicePdfAgent\nDrawback: Extreme coordination overhead, high routing ambiguity, complex handoffs for simple requests.', {
    x: 1.1, y: 4.15, w: 5.4, h: 1.1,
    fontSize: 10, color: COLORS.TEXT_DARK, fontFace: FONTS.BODY
  });

  s43.addText('PROPOSAL B: ONE INVOICE OPERATIONS AGENT WITH 4 TOOLS', {
    x: 6.8, y: 3.8, w: 5.4, h: 0.3,
    fontSize: 11, bold: true, color: COLORS.SUCCESS_GREEN, fontFace: FONTS.BODY
  });
  s43.addText('• 1 Agent: InvoiceAssistantAgent\n• 4 Tools: SearchInvoices, GetStatistics, UpdateStatus, GeneratePdfLink\nAdvantage: Zero routing friction. The model can search an invoice and transition its status in a single unified turn.', {
    x: 6.8, y: 4.15, w: 5.4, h: 1.1,
    fontSize: 10, color: COLORS.TEXT_DARK, fontFace: FONTS.BODY
  });

  s43.addText('ARCHITECTURAL RULE OF THUMB: Only create a separate agent when there is a distinct domain persona, fundamentally different security permissions, or mutually incompatible instructions.', {
    x: 1.1, y: 5.65, w: 11.1, h: 0.6,
    fontSize: 11, bold: true, italic: true, color: '0369A1', fontFace: FONTS.BODY, align: 'center'
  });

  s43.addNotes(formatNotes({
    explanation: 'Lead discussion on agent granularity. Favor Proposal B: 1 agent with 4 tools.',
    transition: 'Let us now live-code Lab 09: Building AgentCoordinator.cs and its routing logic.'
  }));

  // ==========================================
  // SLIDE 44: [CODING LAB 09] Build AgentCoordinator
  // ==========================================
  const s44 = pres.addSlide();
  s44.background = { color: COLORS.LIGHT_BG };
  addCodingLabHeader(s44, '09', 'Build AgentCoordinator.cs & Semantic Routing', 'Implement IAgentCoordinator, code RouteToAgent intent classifier, dispatch prompt to specialized agents.');
  addSlideFooter(s44, 44, 70);

  s44.addShape(pres.ShapeType.roundRect, {
    x: 0.8, y: 1.95, w: 5.7, h: 4.8,
    rectRadius: 0.1,
    fill: { color: COLORS.WHITE },
    line: { color: COLORS.BORDER_LIGHT, width: 1 }
  });
  s44.addText('INSTRUCTOR LIVE CODING CHECKLIST', {
    x: 1.0, y: 2.15, w: 5.3, h: 0.35,
    fontSize: 11, bold: true, color: COLORS.SUCCESS_GREEN, fontFace: FONTS.BODY
  });
  const lab9Steps = [
    '1. Create src/InvoiceApp.AI/Coordinator/AgentCoordinator.cs',
    '2. Define constructor injecting IEnumerable<IBaseAgent>, IAgentMemoryService, IAgentEvaluationService, ITelemetryService',
    '3. Implement RouteToAgent(userPrompt) with clean keyword intent classification',
    '   - "how to" / "rule" -> DocumentationAgent',
    '   - "customer" / "client" -> CustomerOperationsAgent',
    '   - "kpi" / "revenue" / "summary" -> AnalyticsAgent',
    '   - Default -> InvoiceAssistantAgent',
    '4. Implement ProcessAsync(conversationId, userPrompt, context, ct)',
    '5. Register services.AddScoped<IAgentCoordinator, AgentCoordinator>() in DI',
    '6. Test routing across 4 distinct prompts and observe AgentName in logs'
  ];
  lab9Steps.forEach((st, idx) => {
    s44.addText(st, {
      x: 1.0, y: 2.52 + idx * 0.46, w: 5.3, h: 0.42,
      fontSize: 9.5, color: COLORS.TEXT_DARK, fontFace: FONTS.BODY
    });
  });

  s44.addShape(pres.ShapeType.roundRect, {
    x: 6.8, y: 1.95, w: 5.73, h: 4.8,
    rectRadius: 0.1,
    fill: { color: COLORS.CODE_BG },
    line: { color: '334155', width: 1 }
  });
  const lab9Code = `// src/InvoiceApp.AI/Coordinator/AgentCoordinator.cs (Routing Live Code)
private IBaseAgent RouteToAgent(string userPrompt)
{
    var lower = userPrompt.ToLowerInvariant();

    // 1. Documentation & Policy Queries
    if (lower.Contains("how to") || lower.Contains("guide") || lower.Contains("rule"))
        return _agents.OfType<DocumentationAgent>().FirstOrDefault() ?? _agents.First();

    // 2. Customer Operations
    if (lower.Contains("customer") || lower.Contains("client"))
        return _agents.OfType<CustomerOperationsAgent>().FirstOrDefault() ?? _agents.First();

    // 3. Financial Analytics & High-level KPIs
    if (lower.Contains("kpi") || lower.Contains("revenue") || lower.Contains("summary"))
        return _agents.OfType<AnalyticsAgent>().FirstOrDefault() ?? _agents.First();

    // 4. Default: Invoice Operations Agent
    return _agents.OfType<InvoiceAssistantAgent>().FirstOrDefault() ?? _agents.First();
}`;
  s44.addText(lab9Code, {
    x: 7.0, y: 2.15, w: 5.3, h: 4.4,
    fontSize: 8.5, fontFace: FONTS.CODE, color: COLORS.CODE_TEXT
  });

  s44.addNotes(formatNotes({
    explanation: 'Live code RouteToAgent and ProcessAsync in AgentCoordinator.cs.',
    whatToEmphasize: 'Show students how LINQ `_agents.OfType<TAgent>()` dynamically resolves the correct specialized agent from the injected collection.',
    transition: 'Now let us examine multi-agent topologies in Module 10.'
  }));

  // ==========================================
  // SLIDE 45: Module 10 - Multi-Agent Topologies
  // ==========================================
  const s45 = pres.addSlide();
  s45.background = { color: COLORS.LIGHT_BG };
  addSlideHeader(s45, 'Module 10: Multi-Agent Systems', 'Multi-Agent Topologies: Supervisor, Pipeline, & Swarm', 'Architectural patterns for orchestrating collaboration across multiple autonomous agents.');
  addSlideFooter(s45, 45, 70);

  const topologies45 = [
    {
      title: 'Supervisor / Hub-and-Spoke',
      badge: 'USED IN INVOICEAI',
      color: COLORS.AZURE_BLUE,
      desc: 'A central coordinator receives all requests, selects the specialized worker agent, invokes it, evaluates the response, and returns the result. High visibility, centralized memory, simple debugging.'
    },
    {
      title: 'Sequential Pipeline',
      badge: 'WORKFLOW AUTOMATION',
      color: COLORS.SKY_BLUE,
      desc: 'Agent A processes output and hands off to Agent B, which hands off to Agent C (e.g., OCR Reader Agent -> Invoice Validator Agent -> Accounting Agent). Excellent for strict assembly-line processes.'
    },
    {
      title: 'Autonomous Peer Swarm',
      badge: 'DYNAMIC DELEGATION',
      color: COLORS.SUCCESS_GREEN,
      desc: 'Agents dynamically invoke other agents as tools (Agent-as-a-Tool pattern). An Invoice Agent can invoke a Customer Agent during execution. High flexibility, but requires strict loop prevention.'
    }
  ];

  topologies45.forEach((tp, idx) => {
    const xPos = 0.8 + idx * 4.0;
    s45.addShape(pres.ShapeType.roundRect, {
      x: xPos, y: 2.05, w: 3.73, h: 4.7,
      rectRadius: 0.1,
      fill: { color: COLORS.WHITE },
      line: { color: COLORS.BORDER_LIGHT, width: 1 }
    });
    s45.addShape(pres.ShapeType.roundRect, {
      x: xPos + 0.15, y: 2.2, w: 3.43, h: 0.65,
      rectRadius: 0.08,
      fill: { color: tp.color === COLORS.SUCCESS_GREEN ? COLORS.SUCCESS_BG : COLORS.AZURE_LIGHT }
    });
    s45.addText(tp.badge, {
      x: xPos + 0.2, y: 2.25, w: 3.33, h: 0.25,
      fontSize: 9, bold: true, color: tp.color, fontFace: FONTS.BODY
    });
    s45.addText(tp.title, {
      x: xPos + 0.2, y: 2.5, w: 3.33, h: 0.35,
      fontSize: 11.5, bold: true, color: COLORS.TEXT_DARK, fontFace: FONTS.BODY
    });

    s45.addText('ARCHITECTURE OVERVIEW:', {
      x: xPos + 0.2, y: 3.1, w: 3.33, h: 0.25,
      fontSize: 9, bold: true, color: COLORS.TEXT_MUTED, fontFace: FONTS.BODY
    });
    s45.addText(tp.desc, {
      x: xPos + 0.2, y: 3.4, w: 3.33, h: 3.0,
      fontSize: 10.5, color: COLORS.TEXT_DARK, fontFace: FONTS.BODY
    });
  });

  s45.addNotes(formatNotes({
    explanation: 'Contrast the 3 multi-agent topologies.',
    transition: 'How does client UI state flow into these agents? Let us look at Context Passing in Slide 46.'
  }));

  // ==========================================
  // SLIDE 46: Module 10 - Context Passing
  // ==========================================
  const s46 = pres.addSlide();
  s46.background = { color: COLORS.LIGHT_BG };
  addSlideHeader(s46, 'Module 10: Multi-Agent Systems', 'Context Passing & Client Screen State Grounding', 'Enriching agent requests with active browser UI context (window.__AI_CONTEXT__).');
  addSlideFooter(s46, 46, 70);

  s46.addShape(pres.ShapeType.roundRect, {
    x: 0.8, y: 1.95, w: 6.8, h: 4.8,
    rectRadius: 0.1,
    fill: { color: COLORS.CODE_BG },
    line: { color: '334155', width: 1 }
  });
  const contextCode46 = `// 1. Client-side JavaScript in InvoiceApp.Web (Layout / View)
window.__AI_CONTEXT__ = {
    activeRoute: "/Invoices/Details",
    invoiceId: "8f5b4a1c-9e23-4d6a-8b1c-7f8e9a0b1c2d",
    invoiceNumber: "INV-000001",
    customerId: "3a1c2d3e-4f5a-6b7c-8d9e-0f1a2b3c4d5e"
};

// 2. Sent in POST /api/ai/chat payload
// { message: "What is the status of this invoice?", context: window.__AI_CONTEXT__ }

// 3. Coordinator forwards context into AgentRequest
var agentRequest = new AgentRequest(
    enrichedPrompt, 
    conversationId, 
    Context: request.Context
);

// 4. Agent resolves "this invoice" without asking user for invoice number!`;
  s46.addText(contextCode46, {
    x: 1.0, y: 2.1, w: 6.4, h: 4.5,
    fontSize: 9, fontFace: FONTS.CODE, color: COLORS.CODE_TEXT
  });

  s46.addShape(pres.ShapeType.roundRect, {
    x: 7.8, y: 1.95, w: 4.73, h: 4.8,
    rectRadius: 0.1,
    fill: { color: COLORS.WHITE },
    line: { color: COLORS.BORDER_LIGHT, width: 1 }
  });
  s46.addText('DYNAMIC SCREEN STATE ENRICHMENT', {
    x: 8.0, y: 2.15, w: 4.3, h: 0.35,
    fontSize: 11, bold: true, color: COLORS.AZURE_BLUE, fontFace: FONTS.BODY
  });
  const contextHighlights46 = [
    { title: 'Zero Friction UX', desc: 'When a user is viewing INV-000001 and asks "Mark this as paid", they should not be forced to re-type the invoice number.' },
    { title: 'window.__AI_CONTEXT__', desc: 'The ASP.NET Core MVC view injects current page model IDs into the DOM, which the chat assistant JavaScript automatically captures.' },
    { title: 'Dynamic Chip Suggestions', desc: 'Context drives UI suggestions: on Customer page, chips say "Show customer invoices"; on Invoices page, chips say "Mark paid".' },
    { title: 'Context-Aware Prompts', desc: 'Coordinator prepends: "Active Page Context: InvoiceId=...". Agent immediately resolves target records.' }
  ];
  contextHighlights46.forEach((ch, idx) => {
    s46.addText(`•  ${ch.title}`, {
      x: 8.0, y: 2.6 + idx * 1.0, w: 4.3, h: 0.28,
      fontSize: 10.5, bold: true, color: COLORS.SKY_BLUE, fontFace: FONTS.BODY
    });
    s46.addText(ch.desc, {
      x: 8.2, y: 2.88 + idx * 1.0, w: 4.1, h: 0.65,
      fontSize: 9.5, color: COLORS.TEXT_DARK, fontFace: FONTS.BODY
    });
  });

  s46.addNotes(formatNotes({
    explanation: 'Walk through how client screen context works in InvoiceApp.Web.',
    transition: 'What are the dangers of multi-agent systems? Slide 47.'
  }));

  // ==========================================
  // SLIDE 47: Module 10 - Multi-Agent Pitfalls
  // ==========================================
  const s47 = pres.addSlide();
  s47.background = { color: COLORS.LIGHT_BG };
  addSlideHeader(s47, 'Module 10: Multi-Agent Systems', 'Avoiding Multi-Agent Pitfalls & Anti-Patterns', 'Recognizing and preventing infinite loops, token explosion, and cascading failures.');
  addSlideFooter(s47, 47, 70);

  const swarmPitfalls47 = [
    {
      title: '1. Infinite Delegation Loops',
      color: COLORS.DEMO_RED,
      desc: 'Agent A delegates to Agent B, which delegates to Agent A. Solution: Enforce a strict max_turns or max_delegations counter (e.g. max 5 hops) in the coordinator.'
    },
    {
      title: '2. Token Explosion & Cost Spikes',
      color: COLORS.WARNING_ORANGE,
      desc: 'Passing full unpruned conversation histories and large tool outputs across multiple agents causes prompt tokens to multiply quadratically. Solution: Prune tool outputs and summarize context.'
    },
    {
      title: '3. Latency Compounding',
      color: COLORS.AZURE_BLUE,
      desc: 'If 3 agents run sequentially, each taking 1.5 seconds, the user waits 4.5 seconds for a response. Solution: Parallelize independent tool queries and minimize intermediate handoffs.'
    },
    {
      title: '4. Distributed Hallucination Cascades',
      color: COLORS.TEXT_MUTED,
      desc: 'If Agent A hallucinates an incorrect customer ID and passes it to Agent B, Agent B executes tools using flawed parameters. Solution: Strict schema validation between agent boundaries.'
    }
  ];

  swarmPitfalls47.forEach((sp, idx) => {
    const isRight = idx >= 2;
    const xPos = isRight ? 6.8 : 0.8;
    const yPos = 2.05 + (idx % 2) * 2.35;

    s47.addShape(pres.ShapeType.roundRect, {
      x: xPos, y: yPos, w: 5.73, h: 2.15,
      rectRadius: 0.1,
      fill: { color: COLORS.WHITE },
      line: { color: COLORS.BORDER_LIGHT, width: 1 }
    });
    s47.addText(sp.title, {
      x: xPos + 0.25, y: yPos + 0.2, w: 5.2, h: 0.35,
      fontSize: 12, bold: true, color: sp.color, fontFace: FONTS.BODY
    });
    s47.addText(sp.desc, {
      x: xPos + 0.25, y: yPos + 0.6, w: 5.2, h: 1.4,
      fontSize: 10.5, color: COLORS.TEXT_DARK, fontFace: FONTS.BODY
    });
  });

  s47.addNotes(formatNotes({
    explanation: 'Warn about infinite loops and token explosions in multi-agent swarms.',
    transition: 'How does an agent remember previous turns? Enter Module 11: Memory and Conversation Context.'
  }));

  // ==========================================
  // SLIDE 48: Module 11 - Understanding Memory
  // ==========================================
  const s48 = pres.addSlide();
  s48.background = { color: COLORS.LIGHT_BG };
  addSlideHeader(s48, 'Module 11: Memory & Context', 'Understanding Memory: History vs. Short-Term vs. Long-Term', 'Categorizing memory types and architectural persistence strategies in enterprise AI.');
  addSlideFooter(s48, 48, 70);

  const memoryTypes48 = [
    {
      type: '1. Sliding Turn History',
      badge: 'CURRENT CONVERSATION',
      color: COLORS.AZURE_BLUE,
      desc: 'Chronological buffer of the last N user and agent turns (e.g. last 10 messages). Injected directly into prompt on each turn for multi-turn conversational coherence.',
      storage: 'In-Memory ConcurrentDictionary or Redis cache, keyed by ConversationId.'
    },
    {
      type: '2. Short-Term Working Memory',
      badge: 'ACTIVE SESSION STATE',
      color: COLORS.SKY_BLUE,
      desc: 'Extracted task variables within a multi-step workflow (e.g. pending line items for an uncommitted invoice draft, selected customer GUID).',
      storage: 'Session cache, user session state, or temporary workflow state store.'
    },
    {
      type: '3. Long-Term Persistent Memory',
      badge: 'ENTERPRISE DOMAIN TRUTH',
      color: COLORS.SUCCESS_GREEN,
      desc: 'Permanent historical records, customer balances, audit logs, and user billing preferences across all sessions over months or years.',
      storage: 'Relational database (SQL Server) or Vector DB. NEVER stored in raw prompt memory.'
    }
  ];

  memoryTypes48.forEach((mt, idx) => {
    const xPos = 0.8 + idx * 4.0;
    s48.addShape(pres.ShapeType.roundRect, {
      x: xPos, y: 2.05, w: 3.73, h: 4.7,
      rectRadius: 0.1,
      fill: { color: COLORS.WHITE },
      line: { color: COLORS.BORDER_LIGHT, width: 1 }
    });
    s48.addShape(pres.ShapeType.roundRect, {
      x: xPos + 0.15, y: 2.2, w: 3.43, h: 0.65,
      rectRadius: 0.08,
      fill: { color: mt.color === COLORS.SUCCESS_GREEN ? COLORS.SUCCESS_BG : COLORS.AZURE_LIGHT }
    });
    s48.addText(mt.badge, {
      x: xPos + 0.2, y: 2.25, w: 3.33, h: 0.25,
      fontSize: 9, bold: true, color: mt.color, fontFace: FONTS.BODY
    });
    s48.addText(mt.type, {
      x: xPos + 0.2, y: 2.5, w: 3.33, h: 0.35,
      fontSize: 11.5, bold: true, color: COLORS.TEXT_DARK, fontFace: FONTS.BODY
    });

    s48.addText('DESCRIPTION:', {
      x: xPos + 0.2, y: 3.05, w: 3.33, h: 0.25,
      fontSize: 9, bold: true, color: COLORS.TEXT_MUTED, fontFace: FONTS.BODY
    });
    s48.addText(mt.desc, {
      x: xPos + 0.2, y: 3.3, w: 3.33, h: 1.5,
      fontSize: 10.5, color: COLORS.TEXT_DARK, fontFace: FONTS.BODY
    });

    s48.addText('PERSISTENCE MECHANISM:', {
      x: xPos + 0.2, y: 4.85, w: 3.33, h: 0.25,
      fontSize: 9, bold: true, color: COLORS.TEXT_MUTED, fontFace: FONTS.BODY
    });
    s48.addText(mt.storage, {
      x: xPos + 0.2, y: 5.1, w: 3.33, h: 1.5,
      fontSize: 10, italic: true, color: mt.color, fontFace: FONTS.BODY
    });
  });

  s48.addNotes(formatNotes({
    explanation: 'Disambiguate memory. Long-term memory is just your relational database.',
    transition: 'Let us see how Sliding-Window Memory is implemented in AgentMemoryService.cs.'
  }));

  // ==========================================
  // SLIDE 49: Module 11 - AgentMemoryService Code
  // ==========================================
  const s49 = pres.addSlide();
  s49.background = { color: COLORS.LIGHT_BG };
  addSlideHeader(s49, 'Module 11: Memory & Context', 'Implementing Sliding-Window Memory in C#', 'Thread-safe conversation history management in AgentMemoryService.cs.');
  addSlideFooter(s49, 49, 70);

  s49.addShape(pres.ShapeType.roundRect, {
    x: 0.8, y: 1.95, w: 6.8, h: 4.8,
    rectRadius: 0.1,
    fill: { color: COLORS.CODE_BG },
    line: { color: '334155', width: 1 }
  });
  const memCode49 = `// src/InvoiceApp.AI/Memory/AgentMemoryService.cs
public sealed class AgentMemoryService : IAgentMemoryService
{
    private static readonly ConcurrentDictionary<string, List<string>> 
        _store = new();

    public Task<string> GetConversationHistoryAsync(
        string conversationId, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(conversationId))
            return Task.FromResult(string.Empty);

        if (_store.TryGetValue(conversationId, out var messages))
        {
            lock (messages)
            {
                // Sliding window: Return last 10 messages only!
                return Task.FromResult(string.Join("\\n", messages.TakeLast(10)));
            }
        }
        return Task.FromResult(string.Empty);
    }

    public Task AppendMessageAsync(
        string conversationId, string role, string content, CancellationToken ct)
    {
        var list = _store.GetOrAdd(conversationId, _ => []);
        lock (list) {
            list.Add($"[{role}]: {content}");
        }
        return Task.CompletedTask;
    }
}`;
  s49.addText(memCode49, {
    x: 1.0, y: 2.1, w: 6.4, h: 4.5,
    fontSize: 8.5, fontFace: FONTS.CODE, color: COLORS.CODE_TEXT
  });

  s49.addShape(pres.ShapeType.roundRect, {
    x: 7.8, y: 1.95, w: 4.73, h: 4.8,
    rectRadius: 0.1,
    fill: { color: COLORS.WHITE },
    line: { color: COLORS.BORDER_LIGHT, width: 1 }
  });
  s49.addText('MEMORY IMPLEMENTATION HIGHLIGHTS', {
    x: 8.0, y: 2.15, w: 4.3, h: 0.35,
    fontSize: 11, bold: true, color: COLORS.AZURE_BLUE, fontFace: FONTS.BODY
  });
  const memHighlights49 = [
    { title: 'Sliding Window (TakeLast(10))', desc: 'Prevents token exhaustion. Keeps prompt tokens bounded regardless of how long the chat session lasts.' },
    { title: 'Thread-Safe ConcurrentDictionary', desc: 'Safe for concurrent HTTP requests across multiple users and web browser sessions.' },
    { title: 'Pluggable Interface Contract', desc: 'IAgentMemoryService allows swapping the in-memory store for a distributed Redis or Azure Cosmos DB cache in production.' },
    { title: 'Multi-Tenant Data Isolation', desc: 'In production, conversation IDs must be scoped by TenantId: $"{tenantId}:{conversationId}" to prevent cross-tenant data leakage.' }
  ];
  memHighlights49.forEach((mh, idx) => {
    s49.addText(`•  ${mh.title}`, {
      x: 8.0, y: 2.6 + idx * 1.0, w: 4.3, h: 0.28,
      fontSize: 10.5, bold: true, color: COLORS.SKY_BLUE, fontFace: FONTS.BODY
    });
    s49.addText(mh.desc, {
      x: 8.2, y: 2.88 + idx * 1.0, w: 4.1, h: 0.65,
      fontSize: 9.5, color: COLORS.TEXT_DARK, fontFace: FONTS.BODY
    });
  });

  s49.addNotes(formatNotes({
    explanation: 'Walk through AgentMemoryService.cs and TakeLast(10).',
    transition: 'Let us examine token cost optimization in Slide 50.'
  }));

  // ==========================================
  // SLIDE 50: Module 11 - Token Cost & Pruning
  // ==========================================
  const s50 = pres.addSlide();
  s50.background = { color: COLORS.LIGHT_BG };
  addSlideHeader(s50, 'Module 11: Memory & Context', 'Token Cost Optimization & Pruning Strategies', 'Managing context window growth to minimize inference latency and cloud API costs.');
  addSlideFooter(s50, 50, 70);

  const pruningStrategies50 = [
    {
      title: 'Strategy 1: Sliding Window Pruning',
      badge: 'SIMPLEST & FASTEST',
      color: COLORS.AZURE_BLUE,
      desc: 'Keep only the last N turns (e.g. 10 messages). Older turns are discarded. Very low CPU overhead, zero extra LLM calls. Best for task-oriented operational tools where old context becomes irrelevant.'
    },
    {
      title: 'Strategy 2: LLM Context Summarization',
      badge: 'LONG-RUNNING CONVERSATIONS',
      color: COLORS.SKY_BLUE,
      desc: 'When history exceeds 2,000 tokens, trigger a background LLM call: "Summarize previous billing decisions in 3 sentences." Replace old turns with the summary. Preserves key agreements while reclaiming 80% of tokens.'
    },
    {
      title: 'Strategy 3: Tool Output Truncation',
      badge: 'PREVENTING TOKEN BLOWOUT',
      color: COLORS.WARNING_ORANGE,
      desc: 'If a tool returns 500 invoices, do not serialize all 500 records into prompt! Truncate to top 10 rows and return: "... and 490 more invoices. Total sum: ₦42,000,000." Protects context limits.'
    }
  ];

  pruningStrategies50.forEach((ps, idx) => {
    s50.addShape(pres.ShapeType.roundRect, {
      x: 0.8 + idx * 4.0, y: 2.05, w: 3.73, h: 4.7,
      rectRadius: 0.1,
      fill: { color: COLORS.WHITE },
      line: { color: COLORS.BORDER_LIGHT, width: 1 }
    });
    s50.addShape(pres.ShapeType.roundRect, {
      x: 0.8 + idx * 4.0 + 0.15, y: 2.2, w: 3.43, h: 0.65,
      rectRadius: 0.08,
      fill: { color: ps.color === COLORS.WARNING_ORANGE ? COLORS.WARNING_BG : COLORS.AZURE_LIGHT }
    });
    s50.addText(ps.badge, {
      x: 0.8 + idx * 4.0 + 0.2, y: 2.25, w: 3.33, h: 0.25,
      fontSize: 9, bold: true, color: ps.color, fontFace: FONTS.BODY
    });
    s50.addText(ps.title, {
      x: 0.8 + idx * 4.0 + 0.2, y: 2.5, w: 3.33, h: 0.35,
      fontSize: 11.5, bold: true, color: COLORS.TEXT_DARK, fontFace: FONTS.BODY
    });

    s50.addText('HOW IT WORKS:', {
      x: 0.8 + idx * 4.0 + 0.2, y: 3.05, w: 3.33, h: 0.25,
      fontSize: 9, bold: true, color: COLORS.TEXT_MUTED, fontFace: FONTS.BODY
    });
    s50.addText(ps.desc, {
      x: 0.8 + idx * 4.0 + 0.2, y: 3.35, w: 3.33, h: 3.1,
      fontSize: 10.5, color: COLORS.TEXT_DARK, fontFace: FONTS.BODY
    });
  });

  s50.addNotes(formatNotes({
    explanation: 'Explain token cost optimization and pruning strategies.',
    transition: 'Let us now live-code Lab 10: Implementing AgentMemoryService.cs.'
  }));

  // ==========================================
  // SLIDE 51: [CODING LAB 10] Implement AgentMemoryService
  // ==========================================
  const s51 = pres.addSlide();
  s51.background = { color: COLORS.LIGHT_BG };
  addCodingLabHeader(s51, '10', 'Implement AgentMemoryService.cs (Sliding Window)', 'Code thread-safe memory service using ConcurrentDictionary, enforce TakeLast(10) sliding window, inject in coordinator.');
  addSlideFooter(s51, 51, 70);

  s51.addShape(pres.ShapeType.roundRect, {
    x: 0.8, y: 1.95, w: 5.7, h: 4.8,
    rectRadius: 0.1,
    fill: { color: COLORS.WHITE },
    line: { color: COLORS.BORDER_LIGHT, width: 1 }
  });
  s51.addText('INSTRUCTOR LIVE CODING CHECKLIST', {
    x: 1.0, y: 2.15, w: 5.3, h: 0.35,
    fontSize: 11, bold: true, color: COLORS.SUCCESS_GREEN, fontFace: FONTS.BODY
  });
  const lab10Steps = [
    '1. Create src/InvoiceApp.AI/Memory/IAgentMemoryService.cs contract',
    '   - Task<string> GetConversationHistoryAsync(conversationId, ct)',
    '   - Task AppendMessageAsync(conversationId, role, content, ct)',
    '2. Implement AgentMemoryService.cs with ConcurrentDictionary<string, List<string>>',
    '3. In GetConversationHistoryAsync, lock messages and return TakeLast(10)',
    '4. In AppendMessageAsync, append "[Role]: Content"',
    '5. Register services.AddSingleton<IAgentMemoryService, AgentMemoryService>() in DI',
    '6. Test multi-turn dialogue in chat UI: verify agent recalls previous turn context!'
  ];
  lab10Steps.forEach((st, idx) => {
    s51.addText(st, {
      x: 1.0, y: 2.52 + idx * 0.52, w: 5.3, h: 0.48,
      fontSize: 9.5, color: COLORS.TEXT_DARK, fontFace: FONTS.BODY
    });
  });

  s51.addShape(pres.ShapeType.roundRect, {
    x: 6.8, y: 1.95, w: 5.73, h: 4.8,
    rectRadius: 0.1,
    fill: { color: COLORS.CODE_BG },
    line: { color: '334155', width: 1 }
  });
  const lab10Code = `// src/InvoiceApp.AI/Memory/AgentMemoryService.cs (Live Code)
public sealed class AgentMemoryService : IAgentMemoryService
{
    private static readonly ConcurrentDictionary<string, List<string>> _store = new();

    public Task<string> GetConversationHistoryAsync(string conversationId, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(conversationId)) return Task.FromResult(string.Empty);

        if (_store.TryGetValue(conversationId, out var messages))
        {
            lock (messages)
            {
                return Task.FromResult(string.Join("\\n", messages.TakeLast(10)));
            }
        }
        return Task.FromResult(string.Empty);
    }

    public Task AppendMessageAsync(string conversationId, string role, string content, CancellationToken ct = default)
    {
        var list = _store.GetOrAdd(conversationId, _ => []);
        lock (list) { list.Add($"[{role}]: {content}"); }
        return Task.CompletedTask;
    }
}`;
  s51.addText(lab10Code, {
    x: 7.0, y: 2.15, w: 5.3, h: 4.4,
    fontSize: 8.5, fontFace: FONTS.CODE, color: COLORS.CODE_TEXT
  });

  s51.addNotes(formatNotes({
    explanation: 'Live code AgentMemoryService.cs. Show how TakeLast(10) keeps context size bounded.',
    whatToEmphasize: 'Submit a multi-turn query: Turn 1: "Which invoices are overdue for Contoso?" -> Turn 2: "Mark the first one as Paid". Show how the agent understands "the first one" because of memory history.',
    transition: 'Now let us examine knowledge retrieval: Enter Module 12: RAG and Knowledge Sources.'
  }));

  console.log('Part 3 slides (36-51) generated successfully.');
}

module.exports = { createPart3Slides };

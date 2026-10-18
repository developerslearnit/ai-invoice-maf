// tools/deck-generator/slides-part1.js
const { COLORS, FONTS, addSlideHeader, addCodingLabHeader, addSlideFooter, formatNotes } = require('./build-deck');

function createPart1Slides(pres) {
  // ==========================================
  // SLIDE 1: Title Slide (Dark Theme)
  // ==========================================
  const s1 = pres.addSlide();
  s1.background = { color: COLORS.NAVY_DARK };

  s1.addShape(pres.ShapeType.roundRect, {
    x: 0.8, y: 0.65, w: 11.73, h: 6.1,
    rectRadius: 0.15,
    fill: { color: '1E293B', transparency: 50 },
    line: { color: '334155', width: 1 }
  });

  s1.addShape(pres.ShapeType.roundRect, {
    x: 1.2, y: 1.05, w: 3.6, h: 0.36,
    rectRadius: 0.1,
    fill: { color: COLORS.AZURE_BLUE },
    line: { color: COLORS.SKY_BLUE, width: 1 }
  });
  s1.addText('.NET 10 | ENTERPRISE AI ARCHITECTURE', {
    x: 1.2, y: 1.05, w: 3.6, h: 0.36,
    fontSize: 9.5, bold: true, color: COLORS.WHITE,
    fontFace: FONTS.BODY, align: 'center', valign: 'middle'
  });

  s1.addText('Integrating AI into Existing .NET Applications\nwith Microsoft Agent Framework', {
    x: 1.2, y: 1.6, w: 10.9, h: 1.7,
    fontSize: 30, bold: true, color: COLORS.WHITE,
    fontFace: FONTS.TITLE, valign: 'top'
  });

  s1.addText('From Traditional ASP.NET Core Applications to Production-Ready Agentic AI Systems', {
    x: 1.2, y: 3.45, w: 10.9, h: 0.5,
    fontSize: 15, color: '94A3B8',
    fontFace: FONTS.BODY, valign: 'top'
  });

  s1.addShape(pres.ShapeType.line, {
    x: 1.2, y: 4.15, w: 10.9, h: 0,
    line: { color: '334155', width: 1 }
  });

  s1.addText('COURSE INSTRUCTOR', {
    x: 1.2, y: 4.45, w: 4.0, h: 0.25,
    fontSize: 9.5, bold: true, color: COLORS.SKY_BLUE,
    fontFace: FONTS.BODY
  });
  s1.addText('Adesina Mark Omoniyi', {
    x: 1.2, y: 4.75, w: 5.0, h: 0.45,
    fontSize: 18, bold: true, color: COLORS.WHITE,
    fontFace: FONTS.TITLE
  });
  s1.addText('Principal .NET AI Architect & Technical Curriculum Designer', {
    x: 1.2, y: 5.25, w: 5.5, h: 0.35,
    fontSize: 11, color: '94A3B8',
    fontFace: FONTS.BODY
  });

  s1.addShape(pres.ShapeType.roundRect, {
    x: 7.2, y: 4.4, w: 4.9, h: 1.8,
    rectRadius: 0.1,
    fill: { color: '0F172A' },
    line: { color: COLORS.AZURE_BLUE, width: 1 }
  });
  s1.addText('REFERENCE APPLICATION CASE STUDY', {
    x: 7.4, y: 4.55, w: 4.5, h: 0.25,
    fontSize: 9, bold: true, color: COLORS.SKY_BLUE, fontFace: FONTS.BODY
  });
  s1.addText('InvoiceAI Management System\nClean Architecture • CQRS & IDispatcher • SQL Server\nEF Core & Dapper • Azure OpenAI & Microsoft Agent Framework', {
    x: 7.4, y: 4.88, w: 4.5, h: 1.15,
    fontSize: 9.5, color: 'CBD5E1', fontFace: FONTS.BODY
  });

  s1.addNotes(formatNotes({
    explanation: 'Welcome students to the course. Introduce the core philosophy: this is not a theoretical AI course about chatbot toys or Python scripts. It is a deep, engineering-first course for seasoned ASP.NET Core developers who need to bring AI capabilities into existing enterprise codebases.',
    whatToEmphasize: '1. We build on top of enterprise .NET Clean Architecture, DDD, CQRS, and SQL Server.\n2. AI does not replace your software architecture—it augments it.\n3. The reference application (InvoiceAI) is a complete, working business system that learners will understand and recreate step by step.',
    discussionPrompt: 'How many of you currently maintain an ASP.NET Core application where product owners are asking to "add AI," but nobody wants to risk breaking existing business invariants?',
    transition: 'Let us begin by addressing the exact business problem we are here to solve.'
  }));

  // ==========================================
  // SLIDE 2: Executive Summary & Course Mission
  // ==========================================
  const s2 = pres.addSlide();
  s2.background = { color: COLORS.LIGHT_BG };
  addSlideHeader(s2, 'Course Overview', 'The Engineering Dilemma: Introducing AI into Existing Systems', 'Solving the tension between deterministic enterprise business software and probabilistic intelligence.');
  addSlideFooter(s2, 2, 70);

  s2.addShape(pres.ShapeType.roundRect, {
    x: 0.8, y: 1.95, w: 11.73, h: 1.35,
    rectRadius: 0.1,
    fill: { color: COLORS.AZURE_LIGHT },
    line: { color: COLORS.AZURE_BLUE, width: 1.5 }
  });
  s2.addText('"I already have a production ASP.NET Core application with domain rules, relational databases, security boundaries, and auditing. How do I intelligently introduce AI without rewriting my system or breaking invariants?"', {
    x: 1.1, y: 2.15, w: 11.1, h: 0.95,
    fontSize: 16, bold: true, italic: true, color: '0369A1', fontFace: FONTS.BODY, align: 'center', valign: 'middle'
  });

  const pillars = [
    {
      title: '1. What This Course IS',
      color: COLORS.AZURE_BLUE,
      points: [
        'Production-grade AI integration in C# and .NET 10',
        'Directly wrapping existing Application CQRS handlers into AI tools',
        'Microsoft Agent Framework & Microsoft.Extensions.AI',
        'Stateful multi-agent coordination with domain guardrails'
      ]
    },
    {
      title: '2. What This Course IS NOT',
      color: COLORS.DEMO_RED,
      points: [
        'A generic introduction to prompt engineering or ChatGPT',
        'Toy Python scripts or standalone chat wrappers',
        'Replacing relational databases with raw vector dumps',
        'Delegating financial calculations or transactions to LLMs'
      ]
    },
    {
      title: '3. The Learning Progression',
      color: COLORS.SUCCESS_GREEN,
      points: [
        'Existing Clean App -> Raw Model -> Structured Output',
        'Tools & Function Calling -> Autonomous Agents',
        'Multi-Agent Coordination -> Sliding Memory -> RAG',
        'Automated Evaluations -> Telemetry -> Production Deployment'
      ]
    }
  ];

  pillars.forEach((p, idx) => {
    const xPos = 0.8 + idx * 4.0;
    s2.addShape(pres.ShapeType.roundRect, {
      x: xPos, y: 3.55, w: 3.73, h: 3.2,
      rectRadius: 0.1,
      fill: { color: COLORS.WHITE },
      line: { color: COLORS.BORDER_LIGHT, width: 1 }
    });
    s2.addShape(pres.ShapeType.roundRect, {
      x: xPos + 0.15, y: 3.7, w: 3.43, h: 0.45,
      rectRadius: 0.08,
      fill: { color: p.color === COLORS.DEMO_RED ? COLORS.DEMO_BG : p.color === COLORS.SUCCESS_GREEN ? COLORS.SUCCESS_BG : COLORS.AZURE_LIGHT }
    });
    s2.addText(p.title, {
      x: xPos + 0.2, y: 3.7, w: 3.33, h: 0.45,
      fontSize: 12, bold: true, color: p.color, fontFace: FONTS.BODY, valign: 'middle'
    });
    p.points.forEach((pt, ptIdx) => {
      s2.addText(`•  ${pt}`, {
        x: xPos + 0.2, y: 4.3 + ptIdx * 0.58, w: 3.33, h: 0.52,
        fontSize: 11, color: COLORS.TEXT_DARK, fontFace: FONTS.BODY
      });
    });
  });

  s2.addNotes(formatNotes({
    explanation: 'Set clear expectations on day one. Enterprise developers often express skepticism toward AI because tutorials online show simplistic Python chatbots that hallucinate SQL or calculate numbers incorrectly. Explain that our objective is to make AI an architectural citizen inside an enterprise .NET solution.',
    whatToEmphasize: 'The central thesis: AI understands natural language intent and coordinates workflows; normal C# software enforces rules, security, transactions, and truth.',
    discussionPrompt: 'What has been your biggest frustration when evaluating AI tutorials or demos for your enterprise codebase?',
    transition: 'Let us inspect the real business application that will serve as our case study throughout the entire curriculum.'
  }));

  // ==========================================
  // SLIDE 3: The Case Study Application (InvoiceAI)
  // ==========================================
  const s3 = pres.addSlide();
  s3.background = { color: COLORS.LIGHT_BG };
  addSlideHeader(s3, 'Case Study', 'Meet the Baseline System: InvoiceAI Enterprise Billing', 'A production-grade ASP.NET Core Clean Architecture solution managing real-world billing lifecycles.');
  addSlideFooter(s3, 3, 70);

  s3.addShape(pres.ShapeType.roundRect, {
    x: 0.8, y: 1.95, w: 5.7, h: 4.8,
    rectRadius: 0.1,
    fill: { color: COLORS.WHITE },
    line: { color: COLORS.BORDER_LIGHT, width: 1 }
  });
  s3.addText('DOMAIN RULES & BUSINESS INVARIANTS', {
    x: 1.0, y: 2.1, w: 5.3, h: 0.35,
    fontSize: 12, bold: true, color: COLORS.AZURE_BLUE, fontFace: FONTS.BODY
  });
  const domainRules = [
    'Customer Directory: Unique email, name required, financial ledger tracking.',
    'Sequential Numbering: Atomic SQL sequence generating INV-000001, INV-000002.',
    'Itemized Invoices: Multi-item line billing (Quantity > 0, UnitPrice >= 0).',
    'Calculated Totals: TotalAmount = Sum(Quantity * UnitPrice) in Nigerian Naira (₦).',
    'State Machine Transitions: Draft -> Sent -> Paid / Overdue / Cancelled.',
    'Strict Audit Rule: ONLY Draft invoices can be deleted; non-drafts must be Cancelled.'
  ];
  domainRules.forEach((rule, idx) => {
    s3.addText(`✔  ${rule}`, {
      x: 1.0, y: 2.55 + idx * 0.65, w: 5.3, h: 0.58,
      fontSize: 11, color: COLORS.TEXT_DARK, fontFace: FONTS.BODY
    });
  });

  s3.addShape(pres.ShapeType.roundRect, {
    x: 6.8, y: 1.95, w: 5.73, h: 4.8,
    rectRadius: 0.1,
    fill: { color: COLORS.WHITE },
    line: { color: COLORS.BORDER_LIGHT, width: 1 }
  });
  s3.addText('TECHNICAL ARCHITECTURE STACK', {
    x: 7.0, y: 2.1, w: 5.3, h: 0.35,
    fontSize: 12, bold: true, color: COLORS.AZURE_BLUE, fontFace: FONTS.BODY
  });
  const techStack = [
    { label: 'Target Framework', val: '.NET 10 (net10.0) with C# 13 features' },
    { label: 'Architecture', val: 'Clean Architecture with DDD & In-Process CQRS' },
    { label: 'CQRS & Dispatcher', val: 'Lightweight IDispatcher with explicit Command/Query handlers' },
    { label: 'Functional Flow', val: 'Result<T> pattern throughout; zero business exceptions' },
    { label: 'Database & Writes', val: 'SQL Server with EF Core (Microsoft.EntityFrameworkCore.SqlServer)' },
    { label: 'Database & Reads', val: 'High-performance Dapper queries (Microsoft.Data.SqlClient)' },
    { label: 'APIs & Frontend', val: 'Minimal APIs + ASP.NET Core MVC with Tailwind CSS' }
  ];
  techStack.forEach((t, idx) => {
    s3.addText(`•  ${t.label}:`, {
      x: 7.0, y: 2.55 + idx * 0.58, w: 5.3, h: 0.25,
      fontSize: 10, bold: true, color: COLORS.SKY_BLUE, fontFace: FONTS.BODY
    });
    s3.addText(t.val, {
      x: 7.25, y: 2.8 + idx * 0.58, w: 5.1, h: 0.3,
      fontSize: 11, color: COLORS.TEXT_DARK, fontFace: FONTS.BODY
    });
  });

  s3.addNotes(formatNotes({
    explanation: 'Introduce the completed reference solution (InvoiceApp) that is sitting in the repository. Show learners that it is a real application with Customers, Invoices, Line Items, and strict validation rules. Emphasize that we do not change the core domain model to accommodate AI; rather, we build an intelligent layer on top of it.',
    whatToEmphasize: 'Notice the state machine: Draft -> Sent -> Paid / Overdue / Cancelled, and the deletion invariant: only Draft invoices can be deleted. Non-drafts must be Cancelled.',
    discussionPrompt: 'Why is it dangerous to let an AI assistant write directly to SQL tables via generated SQL strings, rather than invoking domain commands?',
    transition: 'Let us look at the 14-lab progression through which you will build this entire AI capability.'
  }));

  // ==========================================
  // SLIDE 4: The 14-Lab Hands-On Curriculum Journey
  // ==========================================
  const s4 = pres.addSlide();
  s4.background = { color: COLORS.LIGHT_BG };
  addSlideHeader(s4, 'Course Roadmap', 'The Hands-On Journey: 14 Practical Milestones', 'Step-by-step modular evolution from a traditional application to a production AI system.');
  addSlideFooter(s4, 4, 70);

  const col1Labs = [
    'Lab 01: Inspect & Run Baseline InvoiceApp System',
    'Lab 02: Scaffold InvoiceApp.AI & Register IChatClient',
    'Lab 03: Build Minimal AI Chat Endpoint in Minimal APIs',
    'Lab 04: Author System Directives for InvoiceAssistantAgent',
    'Lab 05: Implement Strongly Typed Models & FluentValidation',
    'Lab 06: Build Read Query Tools in InvoiceTools.cs',
    'Lab 07: Build State Mutation Tools in InvoiceTools.cs'
  ];
  const col2Labs = [
    'Lab 08: Implement BaseAgent.cs & 4 Specialized Agents',
    'Lab 09: Build AgentCoordinator.cs & Semantic Routing',
    'Lab 10: Implement AgentMemoryService.cs (Sliding Window)',
    'Lab 11: Implement DocumentationTools.cs & Grounded RAG',
    'Lab 12: Implement AgentEvaluationService.cs (Auditing)',
    'Lab 13: Implement TelemetryService.cs (Tokens & Latency)',
    'Lab 14: Connect Web UI Chat Studio & End-to-End Test'
  ];

  s4.addShape(pres.ShapeType.roundRect, {
    x: 0.8, y: 1.95, w: 5.7, h: 4.8,
    rectRadius: 0.1,
    fill: { color: COLORS.WHITE },
    line: { color: COLORS.BORDER_LIGHT, width: 1 }
  });
  s4.addText('PHASE 1: FOUNDATIONS, TOOLS & AGENTS (LABS 1-7)', {
    x: 1.0, y: 2.1, w: 5.3, h: 0.35,
    fontSize: 11, bold: true, color: COLORS.AZURE_BLUE, fontFace: FONTS.BODY
  });
  col1Labs.forEach((l, idx) => {
    s4.addShape(pres.ShapeType.roundRect, {
      x: 1.0, y: 2.5 + idx * 0.58, w: 0.45, h: 0.45,
      rectRadius: 0.08,
      fill: { color: COLORS.SUCCESS_BG }
    });
    s4.addText(`${idx + 1}`, {
      x: 1.0, y: 2.5 + idx * 0.58, w: 0.45, h: 0.45,
      fontSize: 11, bold: true, color: COLORS.SUCCESS_GREEN, fontFace: FONTS.BODY, align: 'center', valign: 'middle'
    });
    s4.addText(l.substring(8), {
      x: 1.55, y: 2.5 + idx * 0.58, w: 4.75, h: 0.45,
      fontSize: 10, bold: true, color: COLORS.TEXT_DARK, fontFace: FONTS.BODY, valign: 'middle'
    });
  });

  s4.addShape(pres.ShapeType.roundRect, {
    x: 6.8, y: 1.95, w: 5.73, h: 4.8,
    rectRadius: 0.1,
    fill: { color: COLORS.WHITE },
    line: { color: COLORS.BORDER_LIGHT, width: 1 }
  });
  s4.addText('PHASE 2: ORCHESTRATION, MEMORY & RAG (LABS 8-14)', {
    x: 7.0, y: 2.1, w: 5.3, h: 0.35,
    fontSize: 11, bold: true, color: COLORS.AZURE_BLUE, fontFace: FONTS.BODY
  });
  col2Labs.forEach((l, idx) => {
    const labNum = idx + 8;
    s4.addShape(pres.ShapeType.roundRect, {
      x: 7.0, y: 2.5 + idx * 0.58, w: 0.45, h: 0.45,
      rectRadius: 0.08,
      fill: { color: labNum === 14 ? COLORS.SUCCESS_BG : COLORS.AZURE_LIGHT }
    });
    s4.addText(labNum === 14 ? '★' : `${labNum}`, {
      x: 7.0, y: 2.5 + idx * 0.58, w: 0.45, h: 0.45,
      fontSize: 11, bold: true, color: labNum === 14 ? COLORS.SUCCESS_GREEN : COLORS.SKY_BLUE,
      fontFace: FONTS.BODY, align: 'center', valign: 'middle'
    });
    s4.addText(l.substring(8), {
      x: 7.55, y: 2.5 + idx * 0.58, w: 4.75, h: 0.45,
      fontSize: 10, bold: true, color: COLORS.TEXT_DARK, fontFace: FONTS.BODY, valign: 'middle'
    });
  });

  s4.addNotes(formatNotes({
    explanation: 'Walk through the 14-lab roadmap. Explain that learners will not be overwhelmed with complex multi-agent swarms upfront. They start with a working legacy app in Lab 1, attach a basic chat client in Lab 2, progress to structured output and tools, and gradually assemble the coordinator, memory, and telemetry in Phase 2.',
    whatToEmphasize: 'Notice how each lab maps directly to real files in the repository. By the end of Lab 14, learners will have created the entire InvoiceApp.AI project from scratch.',
    transition: 'Before we write a single line of code, let us establish the 10 non-negotiable architectural principles of enterprise AI.'
  }));

  // ==========================================
  // SLIDE 5: 10 Golden Architecture Principles
  // ==========================================
  const s5 = pres.addSlide();
  s5.background = { color: COLORS.LIGHT_BG };
  addSlideHeader(s5, 'Architectural Manifesto', 'The 10 Golden Principles of Enterprise AI in .NET', 'Guiding commandments for integrating probabilistic intelligence into mission-critical systems.');
  addSlideFooter(s5, 5, 70);

  const principles = [
    { num: '01', title: 'AI Augments, Never Replaces', desc: 'AI assists human operators and workflows; it never supplants core business architecture.' },
    { num: '02', title: 'Domain Rules Remain Deterministic', desc: 'Calculations, tax rules, constraints, and state transitions belong exclusively in C# and SQL.' },
    { num: '03', title: 'Agents Interact via Strongly Typed Tools', desc: 'Models never directly touch tables. They request application capabilities via declared tools.' },
    { num: '04', title: 'Never Trust Model Output Blindly', desc: 'Validate all AI-generated parameters with FluentValidation and domain contracts before execution.' },
    { num: '05', title: 'Authorization Belongs to the Application', desc: 'The model is never an auth barrier. Business services verify user claims on every tool call.' },
    { num: '06', title: 'Prefer One Capable Agent Over Complex Swarms', desc: 'Start with a single well-tooled agent before prematurely adopting multi-agent orchestration.' },
    { num: '07', title: 'RAG for Knowledge, Tools for Live Data', desc: 'Vector search retrieves unstructured knowledge; function tools retrieve operational relational records.' },
    { num: '08', title: 'Evaluate AI Behavior Like Production Code', desc: 'Test for groundedness, hallucinations, and policy compliance automatically on every turn.' },
    { num: '09', title: 'Measure Tokens, Latency, & Cost Continuously', desc: 'Treat inference cost and latency as primary architectural non-functional requirements.' },
    { num: '10', title: 'Design for Graceful Degraded Failure', desc: 'When the model is unreachable or throttled, your core business software must continue functioning.' }
  ];

  principles.forEach((p, idx) => {
    const isRight = idx >= 5;
    const xPos = isRight ? 6.8 : 0.8;
    const yPos = 1.95 + (idx % 5) * 0.95;

    s5.addShape(pres.ShapeType.roundRect, {
      x: xPos, y: yPos, w: 5.7, h: 0.85,
      rectRadius: 0.08,
      fill: { color: COLORS.WHITE },
      line: { color: COLORS.BORDER_LIGHT, width: 1 }
    });
    s5.addShape(pres.ShapeType.roundRect, {
      x: xPos + 0.12, y: yPos + 0.15, w: 0.55, h: 0.55,
      rectRadius: 0.08,
      fill: { color: COLORS.AZURE_LIGHT }
    });
    s5.addText(p.num, {
      x: xPos + 0.12, y: yPos + 0.15, w: 0.55, h: 0.55,
      fontSize: 12, bold: true, color: COLORS.AZURE_BLUE, fontFace: FONTS.BODY, align: 'center', valign: 'middle'
    });
    s5.addText(p.title, {
      x: xPos + 0.78, y: yPos + 0.12, w: 4.8, h: 0.32,
      fontSize: 11, bold: true, color: COLORS.TEXT_DARK, fontFace: FONTS.BODY
    });
    s5.addText(p.desc, {
      x: xPos + 0.78, y: yPos + 0.42, w: 4.8, h: 0.38,
      fontSize: 9.5, color: COLORS.TEXT_MUTED, fontFace: FONTS.BODY
    });
  });

  s5.addNotes(formatNotes({
    explanation: 'Have students read through these 10 principles. These represent the culmination of hard-won lessons from enterprise production AI deployments.',
    whatToEmphasize: 'Pay special attention to Principle 2: "Domain Rules Remain Deterministic", Principle 5: "Authorization Belongs to the Application", and Principle 10: "Design for Graceful Degraded Failure".',
    transition: 'Let us now dive into Module 1: Understanding AI Integration in existing enterprise applications.'
  }));

  // ==========================================
  // SLIDE 6: Module 1 - Traditional vs AI Architecture
  // ==========================================
  const s6 = pres.addSlide();
  s6.background = { color: COLORS.LIGHT_BG };
  addSlideHeader(s6, 'Module 1: AI Integration', 'Architectural Evolution: Traditional vs. AI-Enabled', 'How the integration layer sits cleanly between UI intent and existing domain services.');
  addSlideFooter(s6, 6, 70);

  s6.addShape(pres.ShapeType.roundRect, {
    x: 0.8, y: 1.95, w: 5.7, h: 4.8,
    rectRadius: 0.1,
    fill: { color: COLORS.WHITE },
    line: { color: COLORS.BORDER_LIGHT, width: 1 }
  });
  s6.addText('TRADITIONAL DETERMINISTIC ARCHITECTURE', {
    x: 1.0, y: 2.15, w: 5.3, h: 0.35,
    fontSize: 12, bold: true, color: COLORS.TEXT_MUTED, fontFace: FONTS.BODY
  });
  const tradSteps = [
    { title: 'User / Web Browser', desc: 'Submits static HTTP GET/POST form payloads' },
    { title: 'ASP.NET Core Controllers & APIs', desc: 'Validates DTOs, returns ProblemDetails or ViewModels' },
    { title: 'CQRS Commands & Queries (IDispatcher)', desc: 'Executes explicit business use cases' },
    { title: 'Domain Entities & Repositories', desc: 'Enforces invariants (Draft deletion, state transitions)' },
    { title: 'SQL Server Database', desc: 'Relational persistence (EF Core writes, Dapper reads)' }
  ];
  tradSteps.forEach((st, idx) => {
    s6.addShape(pres.ShapeType.roundRect, {
      x: 1.2, y: 2.6 + idx * 0.8, w: 4.9, h: 0.58,
      rectRadius: 0.08,
      fill: { color: 'F1F5F9' },
      line: { color: COLORS.BORDER_LIGHT, width: 1 }
    });
    s6.addText(st.title, {
      x: 1.35, y: 2.65 + idx * 0.8, w: 4.6, h: 0.25,
      fontSize: 10, bold: true, color: COLORS.TEXT_DARK, fontFace: FONTS.BODY
    });
    s6.addText(st.desc, {
      x: 1.35, y: 2.9 + idx * 0.8, w: 4.6, h: 0.22,
      fontSize: 8.5, color: COLORS.TEXT_MUTED, fontFace: FONTS.BODY
    });
  });

  s6.addShape(pres.ShapeType.roundRect, {
    x: 6.8, y: 1.95, w: 5.73, h: 4.8,
    rectRadius: 0.1,
    fill: { color: COLORS.WHITE },
    line: { color: COLORS.AZURE_BLUE, width: 1.5 }
  });
  s6.addText('AI-ENABLED AGENTIC ARCHITECTURE', {
    x: 7.0, y: 2.15, w: 5.3, h: 0.35,
    fontSize: 12, bold: true, color: COLORS.AZURE_BLUE, fontFace: FONTS.BODY
  });
  const aiSteps = [
    { title: 'User Keystroke / Prompt + UI Context', desc: 'Sends natural language + active screen state (invoiceId)' },
    { title: 'AI Coordinator & Agent Swarm (MAF)', desc: 'Enriches memory, routes intent to specialized agent' },
    { title: 'AI Domain Tools (AIFunctionFactory)', desc: 'Model requests execution of strongly typed C# tools' },
    { title: 'Application Services & CQRS IDispatcher', desc: 'Same exact command handlers enforce business invariants' },
    { title: 'SQL Server + Telemetry & Evaluations', desc: 'Relational database updated; quality audited' }
  ];
  aiSteps.forEach((st, idx) => {
    s6.addShape(pres.ShapeType.roundRect, {
      x: 7.2, y: 2.6 + idx * 0.8, w: 4.9, h: 0.58,
      rectRadius: 0.08,
      fill: { color: COLORS.AZURE_LIGHT },
      line: { color: COLORS.AZURE_BORDER, width: 1 }
    });
    s6.addText(st.title, {
      x: 7.35, y: 2.65 + idx * 0.8, w: 4.6, h: 0.25,
      fontSize: 10, bold: true, color: COLORS.SKY_BLUE, fontFace: FONTS.BODY
    });
    s6.addText(st.desc, {
      x: 7.35, y: 2.9 + idx * 0.8, w: 4.6, h: 0.22,
      fontSize: 8.5, color: COLORS.TEXT_DARK, fontFace: FONTS.BODY
    });
  });

  s6.addNotes(formatNotes({
    explanation: 'Contrast traditional request-response on left with AI-enabled on right. Note that we did not bypass CQRS or domain invariants. AI sits above application services.',
    whatToEmphasize: 'The AI layer calls the exact same IDispatcher handlers that the Web UI and REST API call.',
    transition: 'Let us examine the difference between deterministic and probabilistic computation.'
  }));

  // ==========================================
  // SLIDE 7: Module 1 - Deterministic vs. Probabilistic
  // ==========================================
  const s7 = pres.addSlide();
  s7.background = { color: COLORS.LIGHT_BG };
  addSlideHeader(s7, 'Module 1: AI Integration', 'Deterministic vs. Probabilistic Systems', 'Architectural separation of concerns: matching the right computational engine to the task.');
  addSlideFooter(s7, 7, 70);

  const tableData = [
    [
      { text: 'Dimension', options: { bold: true, fill: '0F172A', color: 'FFFFFF' } },
      { text: 'Deterministic Software (C# / SQL)', options: { bold: true, fill: '0F172A', color: 'FFFFFF' } },
      { text: 'Probabilistic AI Systems (LLMs / Agents)', options: { bold: true, fill: '0078D4', color: 'FFFFFF' } }
    ],
    [
      { text: 'Core Strength', options: { bold: true } },
      { text: 'Absolute mathematical precision, repeatability, transactional consistency' },
      { text: 'Natural language comprehension, reasoning, unstructured data extraction, synthesis' }
    ],
    [
      { text: 'Handling Invariants', options: { bold: true } },
      { text: 'State machine rules (Draft deletion), unique constraints, foreign keys' },
      { text: 'Cannot guarantee consistency; prone to hallucinations if ungrounded' }
    ],
    [
      { text: 'Execution Cost', options: { bold: true } },
      { text: 'Sub-millisecond latency, negligible compute cost' },
      { text: '500ms to 3s inference latency, token-based API billing' }
    ],
    [
      { text: 'Role in InvoiceAI', options: { bold: true } },
      { text: 'Summing line totals, updating database, validating dates, permissions' },
      { text: 'Understanding intent ("mark INV-001 paid"), summarizing overdue trends, drafting notes' }
    ],
    [
      { text: 'Failure Mode', options: { bold: true } },
      { text: 'Explicit exceptions, validation errors, HTTP ProblemDetails' },
      { text: 'Confident hallucinations, off-topic drift, tool argument errors' }
    ]
  ];

  s7.addTable(tableData, {
    x: 0.8, y: 2.05, w: 11.73, h: 4.6,
    colW: [2.0, 4.86, 4.87],
    fontSize: 10.5,
    fontFace: FONTS.BODY,
    color: COLORS.TEXT_DARK,
    border: { pt: 1, color: COLORS.BORDER_LIGHT },
    valign: 'middle'
  });

  s7.addNotes(formatNotes({
    explanation: 'Walk through comparison table. Never ask an LLM to calculate math. Use C# for math and SQL for aggregations; use LLM for language comprehension.',
    whatToEmphasize: 'Temperature 0.0 reduces variance but does not turn an LLM into an arithmetic calculator.',
    transition: 'Now let us classify the different ways AI can be incorporated into an application.'
  }));

  // ==========================================
  // SLIDE 8: Module 1 - Feature vs Copilot vs Agent
  // ==========================================
  const s8 = pres.addSlide();
  s8.background = { color: COLORS.LIGHT_BG };
  addSlideHeader(s8, 'Module 1: AI Integration', 'Autonomy Spectrum: AI Feature vs. Copilot vs. Agent', 'Selecting the appropriate level of agentic autonomy for each business requirement.');
  addSlideFooter(s8, 8, 70);

  const levels = [
    {
      level: 'LEVEL 1: AI FEATURE',
      subtitle: 'One-Shot Inference',
      color: '64748B',
      desc: 'Single stateless call to an LLM. No memory, no tools, no loop.',
      example: 'Generating an invoice payment reminder email draft from an existing invoice DTO.',
      control: 'High control, predictable latency, zero agency.'
    },
    {
      level: 'LEVEL 2: COPILOT ASSISTANT',
      subtitle: 'Human-in-the-Loop Reasoning',
      color: COLORS.AZURE_BLUE,
      desc: 'Multi-turn conversation with memory and tool inspection. Human confirms critical actions.',
      example: 'User asks "Which invoices are overdue?"; assistant lists invoices and asks "Would you like me to send reminders?"',
      control: 'Balanced autonomy, human maintains final approval on state mutations.'
    },
    {
      level: 'LEVEL 3: AUTONOMOUS AGENT',
      subtitle: 'Goal-Driven Multi-Step Execution',
      color: COLORS.SUCCESS_GREEN,
      desc: 'Agent plans, executes multiple tools in sequence, checks preconditions, and self-corrects.',
      example: 'Coordinator receives "Reconcile Apex Corp billing": resolves customer, checks balance, creates draft, and returns summary.',
      control: 'High agency, requires strong guardrails, automated evaluations, and timeout controls.'
    }
  ];

  levels.forEach((lvl, idx) => {
    const xPos = 0.8 + idx * 4.0;
    s8.addShape(pres.ShapeType.roundRect, {
      x: xPos, y: 2.05, w: 3.73, h: 4.7,
      rectRadius: 0.1,
      fill: { color: COLORS.WHITE },
      line: { color: COLORS.BORDER_LIGHT, width: 1 }
    });
    s8.addShape(pres.ShapeType.roundRect, {
      x: xPos + 0.15, y: 2.2, w: 3.43, h: 0.65,
      rectRadius: 0.08,
      fill: { color: lvl.color === COLORS.SUCCESS_GREEN ? COLORS.SUCCESS_BG : lvl.color === COLORS.AZURE_BLUE ? COLORS.AZURE_LIGHT : 'F1F5F9' }
    });
    s8.addText(lvl.level, {
      x: xPos + 0.2, y: 2.25, w: 3.33, h: 0.3,
      fontSize: 11, bold: true, color: lvl.color, fontFace: FONTS.BODY
    });
    s8.addText(lvl.subtitle, {
      x: xPos + 0.2, y: 2.55, w: 3.33, h: 0.25,
      fontSize: 9.5, italic: true, color: COLORS.TEXT_MUTED, fontFace: FONTS.BODY
    });

    s8.addText('DESCRIPTION', {
      x: xPos + 0.2, y: 3.05, w: 3.33, h: 0.25,
      fontSize: 9, bold: true, color: COLORS.TEXT_MUTED, fontFace: FONTS.BODY
    });
    s8.addText(lvl.desc, {
      x: xPos + 0.2, y: 3.3, w: 3.33, h: 0.7,
      fontSize: 10.5, color: COLORS.TEXT_DARK, fontFace: FONTS.BODY
    });

    s8.addText('INVOICEAI EXAMPLE', {
      x: xPos + 0.2, y: 4.15, w: 3.33, h: 0.25,
      fontSize: 9, bold: true, color: COLORS.TEXT_MUTED, fontFace: FONTS.BODY
    });
    s8.addText(lvl.example, {
      x: xPos + 0.2, y: 4.4, w: 3.33, h: 0.9,
      fontSize: 10.5, color: COLORS.TEXT_DARK, fontFace: FONTS.BODY
    });

    s8.addText('CONTROL & GOVERNANCE', {
      x: xPos + 0.2, y: 5.45, w: 3.33, h: 0.25,
      fontSize: 9, bold: true, color: COLORS.TEXT_MUTED, fontFace: FONTS.BODY
    });
    s8.addText(lvl.control, {
      x: xPos + 0.2, y: 5.7, w: 3.33, h: 0.8,
      fontSize: 10, italic: true, color: lvl.color, fontFace: FONTS.BODY
    });
  });

  s8.addNotes(formatNotes({
    explanation: 'Avoid the hype trap: not every AI interaction needs to be an autonomous agent. Choose the lowest level of autonomy that satisfies the requirement.',
    transition: 'Let us test our architectural reasoning with our first interactive class discussion.'
  }));

  // ==========================================
  // SLIDE 9: Module 1 - [DISCUSSION] Architectural Boundaries
  // ==========================================
  const s9 = pres.addSlide();
  s9.background = { color: COLORS.LIGHT_BG };
  addSlideHeader(s9, 'Module 1: Architecture Workshop', 'Discussion: Where Does AI Belong in Enterprise Billing?', 'Reasoning through domain boundaries, financial risks, and architectural responsibilities.');
  addSlideFooter(s9, 9, 70);

  s9.addShape(pres.ShapeType.roundRect, {
    x: 0.8, y: 1.95, w: 11.73, h: 4.8,
    rectRadius: 0.1,
    fill: { color: COLORS.WHITE },
    line: { color: COLORS.AZURE_BLUE, width: 2 }
  });

  s9.addText('SCENARIO 1: CALCULATING LINE ITEM TOTALS AND VALUE ADDED TAX (VAT)', {
    x: 1.1, y: 2.2, w: 11.1, h: 0.35,
    fontSize: 13, bold: true, color: COLORS.AZURE_BLUE, fontFace: FONTS.BODY
  });
  s9.addText('• Option A: Ask the LLM to calculate Quantity * UnitPrice and compute 7.5% VAT in the prompt response.\n• Option B: Provide line items to C# domain model, compute totals deterministically, and pass totals to model.\n• Architecture Verdict: Option B. LLM arithmetic is probabilistic and violates audit compliance. C# guarantees precision.', {
    x: 1.1, y: 2.55, w: 11.1, h: 0.9,
    fontSize: 11, color: COLORS.TEXT_DARK, fontFace: FONTS.BODY
  });

  s9.addText('SCENARIO 2: DELETING AN INVOICE VIA NATURAL LANGUAGE', {
    x: 1.1, y: 3.6, w: 11.1, h: 0.35,
    fontSize: 13, bold: true, color: COLORS.AZURE_BLUE, fontFace: FONTS.BODY
  });
  s9.addText('• User Prompt: "Please delete invoice INV-000003 right now."\n• Invariant: INV-000003 is currently in "Paid" status. Domain rule says only "Draft" invoices can be deleted.\n• Who enforces this? Can the AI override it if the user insists? No! The DeleteDraftInvoice tool delegates to the C# command handler, which rejects the deletion with Result.Failure(InvoiceErrors.OnlyDraftCanBeDeleted).', {
    x: 1.1, y: 3.95, w: 11.1, h: 1.0,
    fontSize: 11, color: COLORS.TEXT_DARK, fontFace: FONTS.BODY
  });

  s9.addText('SCENARIO 3: WHO OWES THE MOST MONEY ACROSS ALL CUSTOMERS?', {
    x: 1.1, y: 5.1, w: 11.1, h: 0.35,
    fontSize: 13, bold: true, color: COLORS.AZURE_BLUE, fontFace: FONTS.BODY
  });
  s9.addText('• Option A: Feed all 50,000 raw invoices into the LLM prompt window and ask it to sort and sum them.\n• Option B: Call a Dapper SQL aggregate query tool (GetTopDebtors), return the top 5 records as JSON, and let the LLM synthesize a polite executive summary.\n• Architecture Verdict: Option B. Minimizes token consumption, eliminates context overflow, and guarantees accurate sums.', {
    x: 1.1, y: 5.45, w: 11.1, h: 1.0,
    fontSize: 11, color: COLORS.TEXT_DARK, fontFace: FONTS.BODY
  });

  s9.addNotes(formatNotes({
    explanation: 'Engage class with these 3 scenarios before opening code. Math belongs in C#, invariants belong in entities, aggregations in SQL, language in AI.',
    transition: 'Now let us transition to our very first hands-on coding lab: inspecting and running the baseline invoice app.'
  }));

  // ==========================================
  // SLIDE 10: [CODING LAB 01] Inspect Baseline App
  // ==========================================
  const s10 = pres.addSlide();
  s10.background = { color: COLORS.LIGHT_BG };
  addCodingLabHeader(s10, '01', 'Inspect & Run Baseline InvoiceApp System', 'Verify .NET 10 environment, SQL Server LocalDB, compile Clean Architecture solution, test REST APIs.');
  addSlideFooter(s10, 10, 70);

  // Left Card: Live Coding Action Plan
  s10.addShape(pres.ShapeType.roundRect, {
    x: 0.8, y: 1.95, w: 5.7, h: 4.8,
    rectRadius: 0.1,
    fill: { color: COLORS.WHITE },
    line: { color: COLORS.BORDER_LIGHT, width: 1 }
  });
  s10.addText('INSTRUCTOR LIVE CODING CHECKLIST', {
    x: 1.0, y: 2.15, w: 5.3, h: 0.35,
    fontSize: 11, bold: true, color: COLORS.SUCCESS_GREEN, fontFace: FONTS.BODY
  });
  const lab1Steps = [
    '1. Open solution in Visual Studio or VS Code: InvoiceTraining.slnx',
    '2. Inspect Directory.Packages.props: note Central Package Management',
    '3. Review Domain aggregate invariants in src/InvoiceApp.Domain/Entities/Invoice.cs',
    '4. Check appsettings.json connection string: (localdb)\\\\mssqllocaldb',
    '5. Run database migrations and seed data in Infrastructure',
    '6. Launch Web & API projects: dotnet run --project src/InvoiceApp.Web',
    '7. Open browser at https://localhost:5001 and Swagger at /swagger'
  ];
  lab1Steps.forEach((st, idx) => {
    s10.addText(st, {
      x: 1.0, y: 2.55 + idx * 0.58, w: 5.3, h: 0.52,
      fontSize: 10, color: COLORS.TEXT_DARK, fontFace: FONTS.BODY
    });
  });

  // Right Card: Terminal / Verification Blueprint
  s10.addShape(pres.ShapeType.roundRect, {
    x: 6.8, y: 1.95, w: 5.73, h: 4.8,
    rectRadius: 0.1,
    fill: { color: COLORS.CODE_BG },
    line: { color: '334155', width: 1 }
  });
  const lab1Code = `# Terminal Verification Commands (Run in Solution Root)

# 1. Verify .NET 10 SDK
dotnet --version
# Expected: 10.0.100 or higher

# 2. Build entire solution
dotnet build InvoiceTraining.slnx

# 3. Run all unit and integration tests
dotnet test

# 4. Run Web Application with AI Chat Studio
cd src/InvoiceApp.Web
dotnet run

# Observe in Browser:
# • Dashboard with KPI metrics in Naira (₦)
# • Invoices ledger with statuses (Draft, Sent, Paid)
# • Customers directory with ledger history`;
  s10.addText(lab1Code, {
    x: 7.0, y: 2.15, w: 5.3, h: 4.4,
    fontSize: 9, fontFace: FONTS.CODE, color: COLORS.CODE_TEXT
  });

  s10.addNotes(formatNotes({
    explanation: 'Start Lab 01. The goal is to ensure every student has a working baseline application before writing any AI code. Open InvoiceTraining.slnx in Visual Studio. Show students the clean solution structure.',
    whatToEmphasize: 'Show students Invoice.cs lines 40-75 where the status transitions (Draft -> Sent -> Paid / Overdue / Cancelled) are implemented. Emphasize that we will not touch or break these rules.',
    demoGuidance: 'Run `dotnet test` in front of the class. Show that all domain invariant tests pass. Then launch the web app and show the dashboard.',
    implementationCaveats: 'If students have SQL Server connection issues, show them appsettings.json and confirm LocalDB is running: `sqllocaldb info mssqllocaldb`.',
    transition: 'Now that our baseline app is running, let us examine the Microsoft AI ecosystem in Module 2.'
  }));

  // ==========================================
  // SLIDE 11: Module 2 - Modern .NET AI Ecosystem
  // ==========================================
  const s11 = pres.addSlide();
  s11.background = { color: COLORS.LIGHT_BG };
  addSlideHeader(s11, 'Module 2: Agent Framework', 'The Modern .NET AI Ecosystem: Unified Abstractions', 'How Microsoft.Extensions.AI and Microsoft Agent Framework unify AI development across .NET 10.');
  addSlideFooter(s11, 11, 70);

  const layers = [
    {
      title: 'Microsoft.Extensions.AI',
      badge: 'FOUNDATION LAYER',
      color: COLORS.AZURE_BLUE,
      items: [
        'IChatClient & IEmbeddingGenerator standard interfaces',
        'Vendor-neutral abstraction over OpenAI, Azure OpenAI, Ollama, Llama',
        'Composable middleware pipeline via ChatClientBuilder',
        'Native telemetry, OpenTelemetry activities, and token usage'
      ]
    },
    {
      title: 'Microsoft Agent Framework (MAF)',
      badge: 'AGENT & ORCHESTRATION',
      color: COLORS.SKY_BLUE,
      items: [
        'Autonomous agent encapsulation (IBaseAgent / BaseAgent)',
        'Declarative tool bindings via AITool & AIFunctionFactory',
        'Multi-agent coordinator and supervisor workflows',
        'Conversational memory and session state management'
      ]
    },
    {
      title: 'Azure.AI.OpenAI & Model Providers',
      badge: 'INFERENCE INFRASTRUCTURE',
      color: COLORS.SUCCESS_GREEN,
      items: [
        'AzureOpenAIClient managing enterprise endpoints & API keys',
        'Microsoft Entra ID (Managed Identity) token authentication',
        'Support for gpt-4o, gpt-4o-mini, and text-embedding-3-small',
        'Built-in enterprise Content Safety and rate limit management'
      ]
    }
  ];

  layers.forEach((lyr, idx) => {
    const xPos = 0.8 + idx * 4.0;
    s11.addShape(pres.ShapeType.roundRect, {
      x: xPos, y: 2.05, w: 3.73, h: 4.7,
      rectRadius: 0.1,
      fill: { color: COLORS.WHITE },
      line: { color: COLORS.BORDER_LIGHT, width: 1 }
    });
    s11.addShape(pres.ShapeType.roundRect, {
      x: xPos + 0.15, y: 2.2, w: 3.43, h: 0.7,
      rectRadius: 0.08,
      fill: { color: lyr.color === COLORS.SUCCESS_GREEN ? COLORS.SUCCESS_BG : COLORS.AZURE_LIGHT }
    });
    s11.addText(lyr.badge, {
      x: xPos + 0.2, y: 2.25, w: 3.33, h: 0.25,
      fontSize: 9, bold: true, color: lyr.color, fontFace: FONTS.BODY
    });
    s11.addText(lyr.title, {
      x: xPos + 0.2, y: 2.5, w: 3.33, h: 0.35,
      fontSize: 12, bold: true, color: COLORS.TEXT_DARK, fontFace: FONTS.BODY
    });
    lyr.items.forEach((it, itIdx) => {
      s11.addText(`•  ${it}`, {
        x: xPos + 0.2, y: 3.1 + itIdx * 0.85, w: 3.33, h: 0.75,
        fontSize: 11, color: COLORS.TEXT_DARK, fontFace: FONTS.BODY
      });
    });
  });

  s11.addNotes(formatNotes({
    explanation: 'Explain the hierarchy of Microsoft AI libraries in .NET 10. IChatClient is the BCL abstraction; MAF provides agents and coordinator.',
    transition: 'Let us inspect the core interfaces and abstractions that make up this ecosystem.'
  }));

  // ==========================================
  // SLIDE 12: Module 2 - Core Abstractions
  // ==========================================
  const s12 = pres.addSlide();
  s12.background = { color: COLORS.LIGHT_BG };
  addSlideHeader(s12, 'Module 2: Agent Framework', 'Core Abstractions & Architecture Map', 'The essential interfaces that govern agents, tools, memory, and orchestration.');
  addSlideFooter(s12, 12, 70);

  const abstractions = [
    { name: 'IChatClient', pkg: 'Microsoft.Extensions.AI', role: 'Primary inference contract. Provides GetResponseAsync(messages, options, ct).' },
    { name: 'AITool / AIFunctionFactory', pkg: 'Microsoft.Extensions.AI', role: 'Wraps C# methods into JSON Schema tool descriptions that LLMs can call.' },
    { name: 'IBaseAgent / BaseAgent', pkg: 'InvoiceApp.AI.Agents', role: 'Domain agent abstraction encapsulating instructions, bound tools, and execution.' },
    { name: 'IAgentCoordinator', pkg: 'InvoiceApp.AI.Coordinator', role: 'Supervisor orchestrator that enriches memory, routes prompts, and runs evaluations.' },
    { name: 'IAgentMemoryService', pkg: 'InvoiceApp.AI.Memory', role: 'Thread-safe conversation session storage maintaining sliding context windows.' },
    { name: 'IAgentEvaluationService', pkg: 'InvoiceApp.AI.Evaluations', role: 'Post-generation auditing for policy compliance, confidence, and hallucinations.' }
  ];

  abstractions.forEach((ab, idx) => {
    const isRight = idx >= 3;
    const xPos = isRight ? 6.8 : 0.8;
    const yPos = 2.05 + (idx % 3) * 1.55;

    s12.addShape(pres.ShapeType.roundRect, {
      x: xPos, y: yPos, w: 5.73, h: 1.4,
      rectRadius: 0.1,
      fill: { color: COLORS.WHITE },
      line: { color: COLORS.BORDER_LIGHT, width: 1 }
    });
    s12.addText(ab.name, {
      x: xPos + 0.2, y: yPos + 0.15, w: 3.5, h: 0.35,
      fontSize: 13, bold: true, color: COLORS.AZURE_BLUE, fontFace: FONTS.CODE
    });
    s12.addShape(pres.ShapeType.roundRect, {
      x: xPos + 3.8, y: yPos + 0.15, w: 1.7, h: 0.28,
      rectRadius: 0.06,
      fill: { color: 'F1F5F9' }
    });
    s12.addText(ab.pkg.replace('InvoiceApp.AI.', ''), {
      x: xPos + 3.8, y: yPos + 0.15, w: 1.7, h: 0.28,
      fontSize: 8.5, color: COLORS.TEXT_MUTED, fontFace: FONTS.BODY, align: 'center', valign: 'middle'
    });
    s12.addText(ab.role, {
      x: xPos + 0.2, y: yPos + 0.55, w: 5.3, h: 0.75,
      fontSize: 10.5, color: COLORS.TEXT_DARK, fontFace: FONTS.BODY
    });
  });

  s12.addNotes(formatNotes({
    explanation: 'Map out the core contracts. IChatClient and AITool come from Microsoft.Extensions.AI. The agent and coordinator interfaces are domain-owned.',
    transition: 'Let us see how all these components are registered in the ASP.NET Core Dependency Injection container.'
  }));

  // ==========================================
  // SLIDE 13: Module 2 - Dependency Injection
  // ==========================================
  const s13 = pres.addSlide();
  s13.background = { color: COLORS.LIGHT_BG };
  addSlideHeader(s13, 'Module 2: Agent Framework', 'Dependency Injection Configuration in InvoiceApp.AI', 'Clean registration of AI clients, specialized agents, tools, memory, and coordinators.');
  addSlideFooter(s13, 13, 70);

  s13.addShape(pres.ShapeType.roundRect, {
    x: 0.8, y: 1.95, w: 6.8, h: 4.8,
    rectRadius: 0.1,
    fill: { color: COLORS.CODE_BG },
    line: { color: '334155', width: 1 }
  });
  const diCode = `// src/InvoiceApp.AI/DependencyInjection.cs (Simplified)
public static IServiceCollection AddAgentFramework(
    this IServiceCollection services, IConfiguration config)
{
    // 1. Evaluate credentials & SimulationMode
    bool isSimulated = string.IsNullOrWhiteSpace(apiKey) ||
                       apiKey.Contains("YOUR_API_KEY");
    services.AddSingleton(new SimulationMode(isSimulated));

    // 2. Register IChatClient with Function Calling pipeline
    if (!isSimulated) {
        services.AddSingleton(new AzureOpenAIClient(endpoint, cred));
        services.AddSingleton<IChatClient>(sp => sp
            .GetRequiredService<AzureOpenAIClient>()
            .GetChatClient(deploymentName)
            .AsIChatClient()
            .AsBuilder()
            .UseFunctionInvocation() // <--- CRITICAL MIDDLEWARE
            .Build());
    }

    // 3. Register Domain Tools & Specialized Agents
    services.AddScoped<InvoiceTools>();
    services.AddScoped<IBaseAgent, InvoiceAssistantAgent>();
    services.AddScoped<IBaseAgent, AnalyticsAgent>();
    services.AddScoped<IAgentCoordinator, AgentCoordinator>();

    return services;
}`;
  s13.addText(diCode, {
    x: 1.0, y: 2.1, w: 6.4, h: 4.5,
    fontSize: 9, fontFace: FONTS.CODE, color: COLORS.CODE_TEXT
  });

  s13.addShape(pres.ShapeType.roundRect, {
    x: 7.8, y: 1.95, w: 4.73, h: 4.8,
    rectRadius: 0.1,
    fill: { color: COLORS.WHITE },
    line: { color: COLORS.BORDER_LIGHT, width: 1 }
  });
  s13.addText('REGISTRATION ARCHITECTURE HIGHLIGHTS', {
    x: 8.0, y: 2.15, w: 4.3, h: 0.35,
    fontSize: 11, bold: true, color: COLORS.AZURE_BLUE, fontFace: FONTS.BODY
  });
  const diHighlights = [
    { title: '.UseFunctionInvocation()', desc: 'Middleware that intercepts model tool calls, executes the C# method, and feeds the result back automatically.' },
    { title: 'SimulationMode Detection', desc: 'Inspects API keys. If keys are missing or placeholders, transparently activates offline mock inference.' },
    { title: 'Scoped Lifetime for Tools', desc: 'Domain tools depend on IDispatcher and DbContext, so tools and agents are registered with Scoped lifetime.' },
    { title: 'IEnumerable<IBaseAgent>', desc: 'Registering multiple agents as IBaseAgent allows the coordinator to inject all agents dynamically as a swarm.' }
  ];
  diHighlights.forEach((dh, idx) => {
    s13.addText(`•  ${dh.title}`, {
      x: 8.0, y: 2.6 + idx * 1.0, w: 4.3, h: 0.28,
      fontSize: 10.5, bold: true, color: COLORS.SKY_BLUE, fontFace: FONTS.BODY
    });
    s13.addText(dh.desc, {
      x: 8.2, y: 2.88 + idx * 1.0, w: 4.1, h: 0.65,
      fontSize: 9.5, color: COLORS.TEXT_DARK, fontFace: FONTS.BODY
    });
  });

  s13.addNotes(formatNotes({
    explanation: 'Walk through lines 43-148 of DependencyInjection.cs. Emphasize how clean and idiomatic services.AddAgentFramework(config) is in ASP.NET Core.',
    whatToEmphasize: 'Point out .UseFunctionInvocation(). Without it, function calling would return raw JSON tool calls instead of executing.',
    transition: 'Let us examine how SimulationMode enables offline development.'
  }));

  // ==========================================
  // SLIDE 14: Module 2 - Simulation Mode
  // ==========================================
  const s14 = pres.addSlide();
  s14.background = { color: COLORS.LIGHT_BG };
  addSlideHeader(s14, 'Module 2: Agent Framework', 'Simulation Mode: Deterministic Offline Testing', 'Enabling rapid UI prototyping, automated testing, and offline training without cloud dependencies.');
  addSlideFooter(s14, 14, 70);

  s14.addShape(pres.ShapeType.roundRect, {
    x: 0.8, y: 1.95, w: 5.7, h: 4.8,
    rectRadius: 0.1,
    fill: { color: COLORS.WHITE },
    line: { color: COLORS.BORDER_LIGHT, width: 1 }
  });
  s14.addText('THE DEVELOPER CHALLENGE', {
    x: 1.0, y: 2.15, w: 5.3, h: 0.35,
    fontSize: 11, bold: true, color: COLORS.WARNING_ORANGE, fontFace: FONTS.BODY
  });
  s14.addText('• Cloud API keys require active subscriptions, budgets, and enterprise approval.\n• Rate limits (HTTP 429) can stall development during fast edit-and-continue cycles.\n• Automated unit/integration tests must not incur live token billing or fail on network drops.\n• Training classrooms cannot guarantee 30 students hitting Azure OpenAI concurrently.', {
    x: 1.0, y: 2.55, w: 5.3, h: 1.3,
    fontSize: 10.5, color: COLORS.TEXT_DARK, fontFace: FONTS.BODY
  });

  s14.addText('THE SIMULATIONMODE SOLUTION', {
    x: 1.0, y: 3.95, w: 5.3, h: 0.35,
    fontSize: 11, bold: true, color: COLORS.SUCCESS_GREEN, fontFace: FONTS.BODY
  });
  s14.addText('• Automatic Detection: If API key is empty or default, SimulationMode engages.\n• Realistic Latency: Simulates 150ms inference delay for authentic UI feel.\n• Direct Tool Execution: Simulated agents invoke the real C# domain tools directly.\n• Zero External Traffic: 100% offline functionality across every course lab.', {
    x: 1.0, y: 4.35, w: 5.3, h: 1.3,
    fontSize: 10.5, color: COLORS.TEXT_DARK, fontFace: FONTS.BODY
  });

  s14.addShape(pres.ShapeType.roundRect, {
    x: 6.8, y: 1.95, w: 5.73, h: 4.8,
    rectRadius: 0.1,
    fill: { color: COLORS.CODE_BG },
    line: { color: '334155', width: 1 }
  });
  const simCode = `// src/InvoiceApp.AI/Agents/BaseAgent.cs (Snippet)
public virtual async Task<AgentResponse> ExecuteAsync(
    AgentRequest request, CancellationToken ct)
{
    var sw = Stopwatch.StartNew();

    // Offline / Simulation Fallback Check
    if (_simulationMode.IsEnabled || _chatClient is null)
    {
        await Task.Delay(150, ct); // Realistic latency
        sw.Stop();

        var simText = await GenerateSimulatedResponseAsync(
            request, ct);

        _telemetry.TrackAgentExecution(Name, sw.ElapsedMilliseconds, 0);
        return AgentResponse.Success(simText, Name, 0, sw.ElapsedMilliseconds);
    }

    // Live Azure OpenAI Inference Execution
    // ...
}`;
  s14.addText(simCode, {
    x: 7.0, y: 2.15, w: 5.3, h: 4.4,
    fontSize: 9, fontFace: FONTS.CODE, color: COLORS.CODE_TEXT
  });

  s14.addNotes(formatNotes({
    explanation: 'Explain SimulationMode. In simulation mode, agents still invoke the real domain tools against SQL Server, enabling 100% offline development.',
    transition: 'Let us now live-code Lab 02: Scaffolding InvoiceApp.AI and configuring DependencyInjection.cs.'
  }));

  // ==========================================
  // SLIDE 15: [CODING LAB 02] Scaffold InvoiceApp.AI
  // ==========================================
  const s15 = pres.addSlide();
  s15.background = { color: COLORS.LIGHT_BG };
  addCodingLabHeader(s15, '02', 'Scaffold InvoiceApp.AI & Wire DI Container', 'Create the AI class library, reference Azure & Microsoft.Extensions.AI packages, implement AddAgentFramework.');
  addSlideFooter(s15, 15, 70);

  s15.addShape(pres.ShapeType.roundRect, {
    x: 0.8, y: 1.95, w: 5.7, h: 4.8,
    rectRadius: 0.1,
    fill: { color: COLORS.WHITE },
    line: { color: COLORS.BORDER_LIGHT, width: 1 }
  });
  s15.addText('INSTRUCTOR LIVE CODING CHECKLIST', {
    x: 1.0, y: 2.15, w: 5.3, h: 0.35,
    fontSize: 11, bold: true, color: COLORS.SUCCESS_GREEN, fontFace: FONTS.BODY
  });
  const lab2Steps = [
    '1. Create class library: dotnet new classlib -n InvoiceApp.AI -o src/InvoiceApp.AI',
    '2. Add to solution: dotnet sln add src/InvoiceApp.AI/InvoiceApp.AI.csproj',
    '3. Add project reference: InvoiceApp.AI -> InvoiceApp.Application',
    '4. Add package references in Directory.Packages.props: Azure.AI.OpenAI, Microsoft.Extensions.AI, Microsoft.Extensions.AI.OpenAI',
    '5. Create src/InvoiceApp.AI/Common/SimulationMode.cs',
    '6. Create src/InvoiceApp.AI/Common/AzureOpenAISettings.cs',
    '7. Implement src/InvoiceApp.AI/DependencyInjection.cs with AddAgentFramework',
    '8. Call builder.Services.AddAgentFramework(builder.Configuration) in Api/Program.cs'
  ];
  lab2Steps.forEach((st, idx) => {
    s15.addText(st, {
      x: 1.0, y: 2.52 + idx * 0.52, w: 5.3, h: 0.48,
      fontSize: 9.5, color: COLORS.TEXT_DARK, fontFace: FONTS.BODY
    });
  });

  s15.addShape(pres.ShapeType.roundRect, {
    x: 6.8, y: 1.95, w: 5.73, h: 4.8,
    rectRadius: 0.1,
    fill: { color: COLORS.CODE_BG },
    line: { color: '334155', width: 1 }
  });
  const lab2Code = `// src/InvoiceApp.AI/Common/SimulationMode.cs
namespace InvoiceApp.AI.Common;

public sealed class SimulationMode(bool isEnabled)
{
    public bool IsEnabled { get; } = isEnabled;
}

// src/InvoiceApp.AI/DependencyInjection.cs (Core Registration)
services.AddSingleton<IChatClient>(sp =>
{
    if (isSimulated) return null!;
    
    var client = new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(apiKey));
    return client.GetChatClient(deploymentName)
        .AsIChatClient()
        .AsBuilder()
        .UseFunctionInvocation()
        .Build();
});`;
  s15.addText(lab2Code, {
    x: 7.0, y: 2.15, w: 5.3, h: 4.4,
    fontSize: 9, fontFace: FONTS.CODE, color: COLORS.CODE_TEXT
  });

  s15.addNotes(formatNotes({
    explanation: 'Perform live coding of Lab 02. Type out SimulationMode.cs and DependencyInjection.cs. Show students how AddAgentFramework is structured.',
    whatToEmphasize: 'Point out that IChatClient is created with .AsIChatClient().AsBuilder().UseFunctionInvocation().Build(). Explain each builder step.',
    demoGuidance: 'After writing DependencyInjection.cs, build the project with `dotnet build src/InvoiceApp.AI` to prove it compiles with zero warnings.',
    transition: 'Now let us examine the minimal request flow in Module 3.'
  }));

  // ==========================================
  // SLIDE 16: Module 3 - First AI Request Flow
  // ==========================================
  const s16 = pres.addSlide();
  s16.background = { color: COLORS.LIGHT_BG };
  addSlideHeader(s16, 'Module 3: First Integration', 'The Minimal AI Request Flow: Sequence Diagram', 'Tracing the simplest meaningful path from user prompt to model response in ASP.NET Core.');
  addSlideFooter(s16, 16, 70);

  s16.addShape(pres.ShapeType.roundRect, {
    x: 0.8, y: 1.95, w: 11.73, h: 4.8,
    rectRadius: 0.1,
    fill: { color: COLORS.WHITE },
    line: { color: COLORS.BORDER_LIGHT, width: 1 }
  });

  const lifelines16 = [
    { name: 'Browser / Client', x: 1.6 },
    { name: 'ASP.NET Core API', x: 4.5 },
    { name: 'IChatClient', x: 7.4 },
    { name: 'Azure OpenAI (gpt-4o)', x: 10.3 }
  ];

  lifelines16.forEach(ll => {
    s16.addShape(pres.ShapeType.roundRect, {
      x: ll.x - 1.1, y: 2.15, w: 2.2, h: 0.45,
      rectRadius: 0.08,
      fill: { color: COLORS.AZURE_LIGHT },
      line: { color: COLORS.AZURE_BORDER, width: 1 }
    });
    s16.addText(ll.name, {
      x: ll.x - 1.1, y: 2.15, w: 2.2, h: 0.45,
      fontSize: 10, bold: true, color: COLORS.SKY_BLUE, fontFace: FONTS.BODY, align: 'center', valign: 'middle'
    });
    s16.addShape(pres.ShapeType.line, {
      x: ll.x, y: 2.65, w: 0, h: 3.8,
      line: { color: 'CBD5E1', width: 1, dashType: 'dash' }
    });
  });

  const messages16 = [
    { fromX: 1.6, toX: 4.5, y: 3.0, label: '1. POST /api/ai/simple-chat { prompt: "Explain payment terms" }' },
    { fromX: 4.5, toX: 7.4, y: 3.7, label: '2. chatClient.GetResponseAsync(messages, options, ct)' },
    { fromX: 7.4, toX: 10.3, y: 4.4, label: '3. HTTPS /chat/completions (JSON Payload)' },
    { fromX: 10.3, toX: 7.4, y: 5.1, label: '4. Returns ChatCompletion (Usage, Text)' },
    { fromX: 7.4, toX: 4.5, y: 5.6, label: '5. Wraps in ChatResponse' },
    { fromX: 4.5, toX: 1.6, y: 6.1, label: '6. 200 OK { text: "...", tokens: 42, ms: 680 }' }
  ];

  messages16.forEach(m => {
    const isReverse = m.fromX > m.toX;
    s16.addShape(pres.ShapeType.line, {
      x: isReverse ? m.toX : m.fromX,
      y: m.y,
      w: Math.abs(m.toX - m.fromX),
      h: 0,
      line: { color: isReverse ? COLORS.SUCCESS_GREEN : COLORS.AZURE_BLUE, width: 1.5, endArrowType: 'triangle' }
    });
    s16.addText(m.label, {
      x: Math.min(m.fromX, m.toX), y: m.y - 0.28, w: Math.abs(m.toX - m.fromX), h: 0.25,
      fontSize: 8.5, bold: true, color: isReverse ? COLORS.SUCCESS_GREEN : COLORS.TEXT_DARK,
      fontFace: FONTS.BODY, align: 'center'
    });
  });

  s16.addNotes(formatNotes({
    explanation: 'Walk through the sequence diagram. Notice how CancellationToken flows from HttpContext into GetResponseAsync.',
    transition: 'Let us see the exact C# code required to configure and execute this minimal client.'
  }));

  // ==========================================
  // SLIDE 17: Module 3 - Configuring IChatClient
  // ==========================================
  const s17 = pres.addSlide();
  s17.background = { color: COLORS.LIGHT_BG };
  addSlideHeader(s17, 'Module 3: First Integration', 'Configuring IChatClient & Middleware Pipelines', 'Building a resilient, production-ready inference client with Microsoft.Extensions.AI.');
  addSlideFooter(s17, 17, 70);

  s17.addShape(pres.ShapeType.roundRect, {
    x: 0.8, y: 1.95, w: 6.8, h: 4.8,
    rectRadius: 0.1,
    fill: { color: COLORS.CODE_BG },
    line: { color: '334155', width: 1 }
  });
  const clientCode17 = `// Minimal Endpoint in ASP.NET Core Minimal APIs
app.MapPost("/api/ai/simple-chat", async (
    [FromBody] SimplePromptRequest request,
    IChatClient chatClient,
    CancellationToken ct) =>
{
    var messages = new List<ChatMessage>
    {
        new(ChatRole.System, "You are a professional invoice assistant."),
        new(ChatRole.User, request.Prompt)
    };

    var options = new ChatOptions
    {
        Temperature = 0.2f, // Low temperature for deterministic outputs
        MaxOutputTokens = 500
    };

    ChatResponse response = await chatClient.GetResponseAsync(
        messages, options, ct);

    return Results.Ok(new {
        text = response.Text,
        tokens = response.Usage?.TotalTokenCount
    });
});`;
  s17.addText(clientCode17, {
    x: 1.0, y: 2.1, w: 6.4, h: 4.5,
    fontSize: 9, fontFace: FONTS.CODE, color: COLORS.CODE_TEXT
  });

  s17.addShape(pres.ShapeType.roundRect, {
    x: 7.8, y: 1.95, w: 4.73, h: 4.8,
    rectRadius: 0.1,
    fill: { color: COLORS.WHITE },
    line: { color: COLORS.BORDER_LIGHT, width: 1 }
  });
  s17.addText('CORE CONFIGURATION PARAMETERS', {
    x: 8.0, y: 2.15, w: 4.3, h: 0.35,
    fontSize: 11, bold: true, color: COLORS.AZURE_BLUE, fontFace: FONTS.BODY
  });
  const configParams17 = [
    { param: 'Temperature = 0.2f', desc: 'Financial applications require factual, grounded answers. Low temperature drastically reduces creative hallucinations.' },
    { param: 'CancellationTokens', desc: 'Always pass CancellationToken to GetResponseAsync. Long-running inference must abort if client disconnects.' },
    { param: 'ChatRole.System', desc: 'Defines the behavioral contract, persona, and boundary instructions before user message is evaluated.' },
    { param: 'response.Usage', desc: 'Inspects input and output token consumption for auditing, rate limiting, and cost allocation.' }
  ];
  configParams17.forEach((cp, idx) => {
    s17.addText(`•  ${cp.param}`, {
      x: 8.0, y: 2.6 + idx * 1.0, w: 4.3, h: 0.28,
      fontSize: 10.5, bold: true, color: COLORS.SKY_BLUE, fontFace: FONTS.BODY
    });
    s17.addText(cp.desc, {
      x: 8.2, y: 2.88 + idx * 1.0, w: 4.1, h: 0.65,
      fontSize: 9.5, color: COLORS.TEXT_DARK, fontFace: FONTS.BODY
    });
  });

  s17.addNotes(formatNotes({
    explanation: 'Walk through minimal endpoint. Explain temperature and cancellation tokens.',
    transition: 'Let us now live-code Lab 03: Building our first Minimal API chat endpoint.'
  }));

  // ==========================================
  // SLIDE 18: [CODING LAB 03] First Chat Endpoint
  // ==========================================
  const s18 = pres.addSlide();
  s18.background = { color: COLORS.LIGHT_BG };
  addCodingLabHeader(s18, '03', 'Build Minimal AI Chat Endpoint in Minimal APIs', 'Expose /api/ai/simple-chat, inject IChatClient, handle cancellation tokens, test via Swagger.');
  addSlideFooter(s18, 18, 70);

  s18.addShape(pres.ShapeType.roundRect, {
    x: 0.8, y: 1.95, w: 5.7, h: 4.8,
    rectRadius: 0.1,
    fill: { color: COLORS.WHITE },
    line: { color: COLORS.BORDER_LIGHT, width: 1 }
  });
  s18.addText('INSTRUCTOR LIVE CODING CHECKLIST', {
    x: 1.0, y: 2.15, w: 5.3, h: 0.35,
    fontSize: 11, bold: true, color: COLORS.SUCCESS_GREEN, fontFace: FONTS.BODY
  });
  const lab3Steps = [
    '1. Open src/InvoiceApp.Api/Endpoints/AiEndpoints.cs',
    '2. Define request payload record: SimpleAiRequest(string Prompt)',
    '3. Map route: group.MapPost("/simple-chat", ...)',
    '4. Inject IChatClient and CancellationToken in handler delegate',
    '5. Construct List<ChatMessage> with System and User turns',
    '6. Set ChatOptions with Temperature = 0.2f',
    '7. Await chatClient.GetResponseAsync(messages, options, ct)',
    '8. Return Results.Ok with response.Text and token count',
    '9. Test endpoint in Swagger UI (/swagger) with test prompt'
  ];
  lab3Steps.forEach((st, idx) => {
    s18.addText(st, {
      x: 1.0, y: 2.52 + idx * 0.46, w: 5.3, h: 0.42,
      fontSize: 9.5, color: COLORS.TEXT_DARK, fontFace: FONTS.BODY
    });
  });

  s18.addShape(pres.ShapeType.roundRect, {
    x: 6.8, y: 1.95, w: 5.73, h: 4.8,
    rectRadius: 0.1,
    fill: { color: COLORS.CODE_BG },
    line: { color: '334155', width: 1 }
  });
  const lab3Code = `// src/InvoiceApp.Api/Endpoints/AiEndpoints.cs (Live Code)
public static class AiEndpoints
{
    public static IEndpointRouteBuilder MapAiEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/ai").WithTags("AI Assistant");

        group.MapPost("/simple-chat", async (
            [FromBody] SimpleAiRequest request,
            IChatClient chatClient,
            CancellationToken ct) =>
        {
            var messages = new List<ChatMessage>
            {
                new(ChatRole.System, "You are InvoiceAI. Help with billing inquiries."),
                new(ChatRole.User, request.Prompt)
            };

            var options = new ChatOptions { Temperature = 0.2f };
            var response = await chatClient.GetResponseAsync(messages, options, ct);

            return Results.Ok(new {
                message = response.Text,
                tokens = response.Usage?.TotalTokenCount
            });
        });

        return app;
    }
}`;
  s18.addText(lab3Code, {
    x: 7.0, y: 2.15, w: 5.3, h: 4.4,
    fontSize: 8.5, fontFace: FONTS.CODE, color: COLORS.CODE_TEXT
  });

  s18.addNotes(formatNotes({
    explanation: 'Live code this endpoint in src/InvoiceApp.Api/Endpoints/AiEndpoints.cs. Show students that by injecting IChatClient directly, minimal APIs make AI endpoints as easy to write as any CRUD endpoint.',
    whatToEmphasize: 'Show students the response in Swagger. Highlight that if SimulationMode is active, the endpoint returns a realistic simulated response without crashing.',
    demoGuidance: 'Open Swagger UI, execute a request with `{"prompt": "What are your invoice payment terms?"}`, and inspect the JSON response.',
    transition: 'Now that our basic endpoint works, let us move to Module 4: Prompt Engineering for application developers.'
  }));

  console.log('Part 1 slides (1-18) generated successfully.');
}

module.exports = { createPart1Slides };

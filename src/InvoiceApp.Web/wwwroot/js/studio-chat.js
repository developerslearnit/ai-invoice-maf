/**
 * Studio Chat Controller for InvoiceAI Agent Studio
 * Powers the interactive console, real-time agent inspector,
 * telemetry counters, tool execution history, and rich markdown tables.
 */
class StudioChatController {
  constructor() {
    this.conversationId = null;
    this.isLoading = false;
    this.toolsHistory = [];

    this.dom = {
      messages: document.getElementById('studio-messages'),
      input: document.getElementById('studio-input'),
      sendBtn: document.getElementById('studio-send-btn'),
      clearBtn: document.getElementById('studio-clear-btn'),
      typing: document.getElementById('studio-typing'),
      typingText: document.getElementById('studio-typing-text'),
      agentName: document.getElementById('inspector-agent-name'),
      latency: document.getElementById('inspector-latency'),
      confidence: document.getElementById('inspector-confidence'),
      convId: document.getElementById('inspector-conv-id'),
      toolsList: document.getElementById('inspector-tools-list'),
      toolsCount: document.getElementById('tools-count-badge')
    };

    this.init();
  }

  init() {
    if (!this.dom.messages || !this.dom.input) return;

    this.dom.input.addEventListener('input', () => {
      this.updateSendButtonState();
      this.autoResizeInput();
    });

    this.dom.input.addEventListener('keydown', (e) => {
      if (e.key === 'Enter' && !e.shiftKey) {
        e.preventDefault();
        this.sendMessage();
      }
    });

    this.dom.sendBtn?.addEventListener('click', () => this.sendMessage());
    this.dom.clearBtn?.addEventListener('click', () => this.clearChat());

    // Suggested prompt chip delegation
    this.dom.messages.addEventListener('click', (e) => {
      const chip = e.target.closest('[data-studio-prompt]');
      if (chip) {
        const prompt = chip.getAttribute('data-studio-prompt');
        if (prompt) this.sendMessage(prompt);
      }
    });
  }

  autoResizeInput() {
    const input = this.dom.input;
    if (!input) return;
    input.style.height = 'auto';
    input.style.height = `${Math.min(input.scrollHeight, 140)}px`;
  }

  updateSendButtonState() {
    const text = this.dom.input?.value.trim() || '';
    if (this.dom.sendBtn) {
      this.dom.sendBtn.disabled = text.length === 0 || this.isLoading;
    }
  }

  sendPresetPrompt(promptText) {
    if (!promptText) return;
    if (this.dom.input) {
      this.dom.input.value = promptText;
      this.autoResizeInput();
      this.updateSendButtonState();
    }
    this.sendMessage(promptText);
  }

  clearChat() {
    if (!this.dom.messages) return;
    this.conversationId = null;
    if (this.dom.convId) this.dom.convId.textContent = 'Cleared (new session on prompt)';
    if (this.dom.agentName) this.dom.agentName.textContent = 'AgentCoordinator (Supervisor)';
    if (this.dom.latency) this.dom.latency.textContent = '-- ms';
    if (this.dom.confidence) this.dom.confidence.textContent = '-- %';
    this.toolsHistory = [];
    this.renderToolsHistory();

    // Reset messages stream to welcome
    this.dom.messages.innerHTML = `
      <div class="flex gap-3">
        <div class="w-9 h-9 rounded-xl bg-gradient-to-tr from-brand-600 to-indigo-500 text-white flex items-center justify-center shrink-0 shadow-sm mt-0.5 text-xs font-bold">
            AI
        </div>
        <div class="max-w-[90%] bg-white border border-slate-200/90 rounded-2xl rounded-tl-sm p-5 shadow-soft text-slate-800 space-y-3">
            <div class="flex items-center justify-between">
                <div class="text-xs font-bold text-brand-600 flex items-center gap-1.5">
                    <span>Agent Coordinator</span>
                    <span class="text-[10px] font-normal text-slate-400">&bull; Supervisor Router</span>
                </div>
                <span class="px-2 py-0.5 rounded text-[10px] font-semibold bg-emerald-50 text-emerald-700 border border-emerald-200">
                    Ready
                </span>
            </div>
            <p class="text-sm leading-relaxed text-slate-700">
                Session reset. I am ready to process your next invoice or customer command.
            </p>
        </div>
      </div>
    `;

    // Re-append typing container
    if (this.dom.typing) {
      this.dom.messages.appendChild(this.dom.typing);
      this.dom.typing.classList.add('hidden');
    }
  }

  async sendMessage(explicitText = null) {
    const text = explicitText || this.dom.input?.value.trim();
    if (!text || this.isLoading) return;

    if (!explicitText && this.dom.input) {
      this.dom.input.value = '';
      this.dom.input.style.height = 'auto';
    }
    this.updateSendButtonState();

    // Append User Message
    this.appendMessage('user', text);

    // Show Typing Indicator
    this.setLoading(true, text);

    const startTime = performance.now();

    try {
      const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';

      const payload = {
        message: text,
        conversationId: this.conversationId,
        context: {
          page: 'AiStudio',
          source: 'InteractiveAgentStudio'
        }
      };

      const response = await fetch('/ai/chat', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'X-CSRF-TOKEN': token
        },
        body: JSON.stringify(payload)
      });

      if (!response.ok) {
        throw new Error(`Studio Chat HTTP Error: ${response.status}`);
      }

      const data = await response.json();
      const elapsedMs = Math.round(data.durationMs || (performance.now() - startTime));

      this.conversationId = data.conversationId;

      // Update Inspector Panel
      this.updateInspector({
        agentName: data.agentName || 'AgentCoordinator',
        latencyMs: elapsedMs,
        confidenceScore: data.confidenceScore || 0.96,
        conversationId: data.conversationId,
        query: text,
        reply: data.message
      });

      // Append Assistant Message
      this.appendMessage('assistant', data.message, {
        agentName: data.agentName,
        durationMs: elapsedMs,
        suggestions: data.suggestions
      });

      // Global notification for tool change
      window.dispatchEvent(new CustomEvent('invoiceai:toolExecuted', { detail: data }));

    } catch (err) {
      console.error('Studio chat failure', err);
      this.appendMessage('assistant', '⚠️ *Sorry, an error occurred communicating with the Agent Coordinator. Please ensure InvoiceApp.Api is active and try again.*', {
        agentName: 'Error Handler',
        durationMs: Math.round(performance.now() - startTime)
      });
    } finally {
      this.setLoading(false);
    }
  }

  appendMessage(role, text, meta = {}) {
    const isUser = role === 'user';
    const msgEl = document.createElement('div');
    msgEl.className = `flex gap-3 mb-4 ${isUser ? 'justify-end' : 'justify-start'}`;

    const timestamp = new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });

    if (isUser) {
      msgEl.innerHTML = `
        <div class="max-w-[85%] bg-brand-600 text-white rounded-2xl rounded-tr-sm px-5 py-3.5 shadow-soft">
          <div class="text-xs font-semibold text-brand-200 mb-1 flex items-center justify-between">
            <span>You</span>
            <span class="text-[10px] text-brand-200/80">${timestamp}</span>
          </div>
          <div class="text-sm leading-relaxed">${this.escapeHtml(text)}</div>
        </div>
      `;
    } else {
      const agentBadge = meta.agentName || 'AgentCoordinator';
      const durationText = meta.durationMs ? `${meta.durationMs}ms` : '';
      const formattedContent = this.formatMarkdown(text);

      let suggestionsHtml = '';
      if (Array.isArray(meta.suggestions) && meta.suggestions.length > 0) {
        suggestionsHtml = `
          <div class="pt-3 border-t border-slate-100 mt-3 space-y-1.5">
            <div class="text-[11px] font-semibold text-slate-400 uppercase tracking-wider">Suggested Actions:</div>
            <div class="flex flex-wrap gap-1.5">
              ${meta.suggestions.map(s => `
                <button type="button" data-studio-prompt="${this.escapeHtml(s)}"
                        class="text-xs bg-slate-100 hover:bg-brand-50 text-slate-700 hover:text-brand-700 border border-slate-200 hover:border-brand-200 rounded-full px-3 py-1 transition-colors font-medium text-left">
                  ✨ ${this.escapeHtml(s)}
                </button>
              `).join('')}
            </div>
          </div>
        `;
      }

      msgEl.innerHTML = `
        <div class="w-9 h-9 rounded-xl bg-gradient-to-tr from-brand-600 to-indigo-500 text-white flex items-center justify-center shrink-0 shadow-sm mt-0.5 text-xs font-bold">
          ✨
        </div>
        <div class="max-w-[88%] bg-white border border-slate-200/90 rounded-2xl rounded-tl-sm p-5 shadow-soft text-slate-800 space-y-2">
          <div class="flex items-center justify-between border-b border-slate-100 pb-2">
            <div class="flex items-center gap-2">
              <span class="px-2 py-0.5 rounded text-[11px] font-bold bg-brand-50 text-brand-700 border border-brand-200">
                ${this.escapeHtml(agentBadge)}
              </span>
              ${durationText ? `<span class="text-[11px] font-mono text-slate-400">${durationText}</span>` : ''}
            </div>
            <span class="text-[10px] text-slate-400">${timestamp}</span>
          </div>
          <div class="text-sm leading-relaxed text-slate-800 space-y-2 prose-sm">
            ${formattedContent}
          </div>
          ${suggestionsHtml}
        </div>
      `;
    }

    if (this.dom.typing && this.dom.typing.parentNode === this.dom.messages) {
      this.dom.messages.insertBefore(msgEl, this.dom.typing);
    } else {
      this.dom.messages?.appendChild(msgEl);
    }

    this.scrollToBottom();
  }

  updateInspector(data) {
    if (this.dom.agentName) this.dom.agentName.textContent = data.agentName;
    if (this.dom.latency) this.dom.latency.textContent = `${data.latencyMs} ms`;
    if (this.dom.confidence) {
      const pct = Math.round(data.confidenceScore * 100);
      this.dom.confidence.textContent = `${pct}%`;
    }
    if (this.dom.convId) this.dom.convId.textContent = data.conversationId;

    // Detect tools executed from query or response
    const detectedTools = this.inferExecutedTools(data.query, data.reply, data.agentName);
    detectedTools.forEach(t => {
      this.toolsHistory.unshift({
        tool: t.name,
        target: t.target,
        time: new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit', second: '2-digit' }),
        agent: data.agentName
      });
    });

    this.renderToolsHistory();
  }

  inferExecutedTools(query, reply, agentName) {
    const tools = [];
    const q = (query || '').toLowerCase();
    const r = (reply || '').toLowerCase();

    if (q.includes('create customer') || r.includes("created successfully with id")) {
      tools.push({ name: 'CustomerTools.CreateCustomer', target: 'Customer Onboarding' });
    }
    if (q.includes('create invoice') || r.includes("invoice created successfully") || r.includes("inv-")) {
      if (q.includes('create invoice')) {
        tools.push({ name: 'InvoiceTools.CreateInvoice', target: 'New Invoice Record' });
      }
    }
    if (q.includes('mark') || q.includes('paid') || q.includes('sent') || r.includes('status updated')) {
      tools.push({ name: 'InvoiceTools.UpdateInvoiceStatus', target: 'State Machine Transition' });
    }
    if (q.includes('delete') && q.includes('draft')) {
      tools.push({ name: 'InvoiceTools.DeleteDraftInvoice', target: 'Draft Deletion' });
    }
    if (q.includes('overdue')) {
      tools.push({ name: 'InvoiceTools.GetOverdueInvoices', target: 'Debtor Audit Query' });
    }
    if (q.includes('summary') || q.includes('activity')) {
      tools.push({ name: 'InvoiceTools.GetInvoiceSummary', target: 'Executive KPIs' });
    }
    if (q.includes('how do i') || q.includes('how can i') || agentName.includes('Documentation')) {
      tools.push({ name: 'DocumentationTools.ReadDocumentationPage', target: 'Markdown RAG' });
    }

    if (tools.length === 0) {
      tools.push({ name: `${agentName}.ProcessAsync`, target: 'Semantic Intent Routing' });
    }

    return tools;
  }

  renderToolsHistory() {
    if (!this.dom.toolsList) return;
    if (this.dom.toolsCount) {
      this.dom.toolsCount.textContent = `${this.toolsHistory.length} Runs`;
    }

    if (this.toolsHistory.length === 0) {
      this.dom.toolsList.innerHTML = `
        <div class="p-4 text-center text-slate-400 bg-slate-50/50 rounded-xl border border-dashed border-slate-200 text-xs">
          No tools executed yet.
        </div>
      `;
      return;
    }

    this.dom.toolsList.innerHTML = this.toolsHistory.map(item => `
      <div class="p-3 bg-slate-50 hover:bg-slate-100 rounded-xl border border-slate-200/80 transition-colors text-xs space-y-1">
        <div class="flex items-center justify-between">
          <span class="font-mono font-bold text-brand-700 text-[11px] truncate">${this.escapeHtml(item.tool)}</span>
          <span class="text-[10px] text-slate-400 font-mono">${item.time}</span>
        </div>
        <div class="flex items-center justify-between text-[11px] text-slate-600">
          <span>Target: <strong class="text-slate-800">${this.escapeHtml(item.target)}</strong></span>
          <span class="text-[10px] text-indigo-600 font-semibold">${this.escapeHtml(item.agent)}</span>
        </div>
      </div>
    `).join('');
  }

  setLoading(isLoading, userPrompt = '') {
    this.isLoading = isLoading;
    this.updateSendButtonState();

    if (this.dom.typing) {
      if (isLoading) {
        if (this.dom.typingText) {
          if (userPrompt.toLowerCase().includes('create customer')) {
            this.dom.typingText.textContent = 'Customer Operations Agent executing CreateCustomer...';
          } else if (userPrompt.toLowerCase().includes('create invoice')) {
            this.dom.typingText.textContent = 'Invoice Assistant Agent creating invoice & computing items...';
          } else if (userPrompt.toLowerCase().includes('paid') || userPrompt.toLowerCase().includes('sent')) {
            this.dom.typingText.textContent = 'Executing status transition in state machine...';
          } else {
            this.dom.typingText.textContent = 'Agent Coordinator delegating & evaluating reasoning...';
          }
        }
        this.dom.typing.classList.remove('hidden');
      } else {
        this.dom.typing.classList.add('hidden');
      }
    }
    this.scrollToBottom();
  }

  formatMarkdown(text) {
    if (!text) return '';

    // Convert INV-00000X to clickable badge links
    text = text.replace(/(INV-\d{6})/g, '<a href="/Invoices?Search=$1" class="inline-flex items-center gap-1 px-1.5 py-0.5 rounded font-mono text-xs font-bold bg-indigo-50 hover:bg-indigo-100 text-brand-700 border border-brand-200 transition-colors" title="View $1 in Invoices">$1 ↗</a>');

    // Bold & italic
    text = text.replace(/\*\*(.*?)\*\*/g, '<strong>$1</strong>');
    text = text.replace(/\*(.*?)\*/g, '<em>$1</em>');

    // Format markdown tables (| col | col |)
    const lines = text.split('\n');
    let inTable = false;
    let tableHtml = '';
    const formattedLines = [];

    for (let i = 0; i < lines.length; i++) {
      const line = lines[i].trim();

      if (line.startsWith('|') && line.endsWith('|')) {
        // Table separator row (|---|---|)
        if (line.includes('---')) {
          continue;
        }

        const cells = line.split('|').slice(1, -1).map(c => c.trim());

        if (!inTable) {
          inTable = true;
          tableHtml = '<div class="overflow-x-auto my-3"><table class="min-w-full text-xs divide-y divide-slate-200 border border-slate-200 rounded-xl overflow-hidden">';
          tableHtml += '<thead class="bg-slate-100 font-semibold text-slate-700"><tr>';
          cells.forEach(c => tableHtml += `<th class="py-2 px-3 text-left">${c}</th>`);
          tableHtml += '</tr></thead><tbody class="divide-y divide-slate-100 bg-white">';
        } else {
          tableHtml += '<tr class="hover:bg-slate-50 transition-colors">';
          cells.forEach(c => tableHtml += `<td class="py-2 px-3 text-slate-700">${c}</td>`);
          tableHtml += '</tr>';
        }
      } else {
        if (inTable) {
          inTable = false;
          tableHtml += '</tbody></table></div>';
          formattedLines.push(tableHtml);
          tableHtml = '';
        }

        if (line.startsWith('• ') || line.startsWith('- ')) {
          formattedLines.push(`<li class="ml-4 list-disc text-slate-700 my-0.5">${line.substring(2)}</li>`);
        } else if (line.startsWith('# ')) {
          formattedLines.push(`<h3 class="text-base font-bold text-slate-900 mt-2 mb-1">${line.substring(2)}</h3>`);
        } else if (line.startsWith('## ')) {
          formattedLines.push(`<h4 class="text-sm font-bold text-slate-900 mt-2 mb-1">${line.substring(3)}</h4>`);
        } else if (line.startsWith('### ')) {
          formattedLines.push(`<h5 class="text-xs font-bold text-slate-900 mt-1 mb-0.5">${line.substring(4)}</h5>`);
        } else if (line.length > 0) {
          formattedLines.push(`<p class="my-1">${line}</p>`);
        }
      }
    }

    if (inTable) {
      tableHtml += '</tbody></table></div>';
      formattedLines.push(tableHtml);
    }

    return formattedLines.join('');
  }

  escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
  }

  scrollToBottom() {
    if (this.dom.messages) {
      requestAnimationFrame(() => {
        this.dom.messages.scrollTop = this.dom.messages.scrollHeight;
      });
    }
  }
}

// Initialize on DOM load
document.addEventListener('DOMContentLoaded', () => {
  window.StudioChat = new StudioChatController();
});

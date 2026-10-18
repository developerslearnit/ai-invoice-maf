/**
 * AI Assistant Chat Experience Controller
 * Supports Floating Indicator, Compact Mode, Expanded Workspace,
 * Multi-Agent Swarm attribution, Clear Conversation, Table Rendering,
 * Entity Badges (INV-00000X), Page Context Injection, and Live Events.
 */
class AiAssistantController {
  constructor() {
    this.state = {
      isOpen: false,
      isExpanded: false,
      isMinimized: false,
      isLoading: false,
      conversationId: null,
      messages: []
    };

    this.dom = {};
    this.init();
  }

  init() {
    // Cache DOM elements
    this.dom.indicator = document.getElementById('ai-indicator');
    this.dom.chatPanel = document.getElementById('ai-chat-panel');
    this.dom.messagesContainer = document.getElementById('ai-messages');
    this.dom.input = document.getElementById('ai-input');
    this.dom.sendBtn = document.getElementById('ai-send-btn');
    this.dom.typingIndicator = document.getElementById('ai-typing');
    this.dom.expandBtn = document.getElementById('ai-expand-btn');
    this.dom.minimizeBtn = document.getElementById('ai-minimize-btn');
    this.dom.closeBtn = document.getElementById('ai-close-btn');
    this.dom.clearBtn = document.getElementById('ai-clear-btn');
    this.dom.suggestionsContainer = document.getElementById('ai-suggestions');

    if (!this.dom.indicator || !this.dom.chatPanel) return;

    this.bindEvents();
    this.loadInitialSuggestions();
  }

  bindEvents() {
    // Floating Pill Click
    this.dom.indicator.addEventListener('click', () => this.open());

    // Panel Controls
    this.dom.closeBtn?.addEventListener('click', () => this.close());
    this.dom.minimizeBtn?.addEventListener('click', () => this.minimize());
    this.dom.expandBtn?.addEventListener('click', () => this.toggleExpand());
    this.dom.clearBtn?.addEventListener('click', () => this.clearChat());

    // Input Handling
    this.dom.input?.addEventListener('input', () => {
      this.updateSendButtonState();
      this.autoResizeInput();
    });

    this.dom.input?.addEventListener('keydown', (e) => {
      if (e.key === 'Enter' && !e.shiftKey) {
        e.preventDefault();
        this.sendMessage();
      }
    });

    this.dom.sendBtn?.addEventListener('click', () => this.sendMessage());

    // Suggested Chips delegation
    document.addEventListener('click', (e) => {
      const chip = e.target.closest('[data-ai-prompt]');
      if (chip) {
        const prompt = chip.getAttribute('data-ai-prompt');
        if (prompt) {
          if (!this.state.isOpen) this.open();
          this.sendMessage(prompt);
        }
      }
    });
  }

  open() {
    this.state.isOpen = true;
    this.state.isMinimized = false;
    this.dom.indicator.classList.add('hidden');
    this.dom.chatPanel.classList.remove('hidden');

    this.applyDimensions();
    this.scrollToBottom();
    setTimeout(() => this.dom.input?.focus(), 150);
  }

  close() {
    this.state.isOpen = false;
    this.dom.chatPanel.classList.add('hidden');
    this.dom.indicator.classList.remove('hidden');
  }

  minimize() {
    this.state.isMinimized = true;
    this.state.isOpen = false;
    this.dom.chatPanel.classList.add('hidden');
    this.dom.indicator.classList.remove('hidden');
  }

  toggleExpand() {
    this.state.isExpanded = !this.state.isExpanded;
    this.applyDimensions();
    this.scrollToBottom();
  }

  applyDimensions() {
    const panel = this.dom.chatPanel;
    const expandIcon = document.getElementById('ai-expand-icon');

    if (this.state.isExpanded) {
      panel.classList.remove('w-[92vw]', 'sm:w-[420px]', 'h-[600px]', 'bottom-6', 'right-6');
      panel.classList.add(
        'w-[95vw]', 'md:w-[80vw]', 'lg:w-[75vw]',
        'h-[88vh]',
        'bottom-4', 'right-4', 'sm:bottom-6', 'sm:right-6'
      );
      if (expandIcon) {
        expandIcon.innerHTML = `<path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 14h6m0 0v6m0-6L3 21m17-7h-6m0 0v6m0-6l7 7M14 4h6m-6 0v6m0-6l7-7M4 10h6m0 0V4m0 6L3 3" />`;
      }
      this.dom.expandBtn?.setAttribute('title', 'Collapse view');
    } else {
      panel.classList.remove(
        'w-[95vw]', 'md:w-[80vw]', 'lg:w-[75vw]',
        'h-[88vh]', 'bottom-4', 'right-4'
      );
      panel.classList.add('w-[92vw]', 'sm:w-[420px]', 'h-[600px]', 'bottom-6', 'right-6');
      if (expandIcon) {
        expandIcon.innerHTML = `<path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 8V4m0 0h4M4 4l5 5m11-1V4m0 0h-4m4 0l-5 5M4 16v4m0 0h4m-4 0l5-5m11 5l-5-5m5 5v-4m0 4h-4" />`;
      }
      this.dom.expandBtn?.setAttribute('title', 'Expand to workspace');
    }
  }

  autoResizeInput() {
    const input = this.dom.input;
    if (!input) return;
    input.style.height = 'auto';
    input.style.height = `${Math.min(input.scrollHeight, 120)}px`;
  }

  updateSendButtonState() {
    const text = this.dom.input?.value.trim() || '';
    if (this.dom.sendBtn) {
      this.dom.sendBtn.disabled = text.length === 0 || this.state.isLoading;
    }
  }

  clearChat() {
    this.state.conversationId = null;
    if (!this.dom.messagesContainer) return;

    this.dom.messagesContainer.innerHTML = `
      <div class="flex gap-3 mb-2">
        <div class="w-8 h-8 rounded-full bg-gradient-to-tr from-brand-600 to-indigo-400 text-white flex items-center justify-center shrink-0 shadow-sm mt-0.5 text-xs font-bold">
            AI
        </div>
        <div class="max-w-[88%] bg-white border border-slate-200/80 rounded-2xl rounded-tl-sm p-4 shadow-sm text-slate-800">
            <div class="text-xs font-semibold text-brand-600 mb-1">Invoice AI Assistant</div>
            <p class="text-sm leading-relaxed mb-3">
                Chat cleared. How can I assist you with your invoicing or customer operations today?
            </p>
            <div id="ai-suggestions" class="flex flex-wrap gap-1.5">
            </div>
        </div>
      </div>
    `;

    this.dom.suggestionsContainer = document.getElementById('ai-suggestions');
    this.loadInitialSuggestions();

    if (this.dom.typingIndicator) {
      this.dom.messagesContainer.appendChild(this.dom.typingIndicator);
      this.dom.typingIndicator.classList.add('hidden');
    }
  }

  async sendMessage(explicitText = null) {
    const text = explicitText || this.dom.input?.value.trim();
    if (!text || this.state.isLoading) return;

    if (!explicitText && this.dom.input) {
      this.dom.input.value = '';
      this.dom.input.style.height = 'auto';
    }
    this.updateSendButtonState();

    // Append User Message to UI
    this.appendMessage('user', text);

    // Show Typing Indicator
    this.setLoading(true);

    const startTime = performance.now();

    try {
      const context = window.__AI_CONTEXT__ || null;
      const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';

      const payload = {
        message: text,
        conversationId: this.state.conversationId,
        context: context
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
        throw new Error(`API error: ${response.status}`);
      }

      const data = await response.json();
      const elapsedMs = Math.round(data.durationMs || (performance.now() - startTime));
      this.state.conversationId = data.conversationId;

      // Append AI Response
      this.appendMessage('assistant', data.message, {
        agentName: data.agentName || 'AgentCoordinator',
        durationMs: elapsedMs,
        suggestions: data.suggestions
      });

      // Render suggestions if provided
      if (Array.isArray(data.suggestions) && data.suggestions.length > 0) {
        this.renderSuggestions(data.suggestions);
      }

      // Dispatch global tool executed event
      window.dispatchEvent(new CustomEvent('invoiceai:toolExecuted', { detail: data }));

    } catch (err) {
      console.error('AI chat failed', err);
      this.appendMessage('assistant', '⚠️ *Sorry, I encountered a connection issue while reaching the AI service. Please ensure the backend application is running and try again.*');
    } finally {
      this.setLoading(false);
    }
  }

  appendMessage(role, text, meta = {}) {
    const isUser = role === 'user';
    const messageEl = document.createElement('div');
    messageEl.className = `flex gap-3 mb-4 ${isUser ? 'justify-end' : 'justify-start'}`;

    const formattedContent = this.formatMarkdown(text);
    const timestamp = new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });

    if (isUser) {
      messageEl.innerHTML = `
        <div class="max-w-[85%] bg-brand-600 text-white rounded-2xl rounded-tr-sm px-4 py-3 shadow-sm">
          <div class="text-sm leading-relaxed">${this.escapeHtml(text)}</div>
          <div class="text-[10px] text-brand-200 text-right mt-1">${timestamp}</div>
        </div>
      `;
    } else {
      const agentBadge = meta.agentName ? `<span class="px-1.5 py-0.2 rounded text-[10px] font-bold bg-brand-50 text-brand-700 border border-brand-200">${this.escapeHtml(meta.agentName)}</span>` : '';
      const durationBadge = meta.durationMs ? `<span class="text-[10px] text-slate-400 font-mono">${meta.durationMs}ms</span>` : '';

      messageEl.innerHTML = `
        <div class="w-8 h-8 rounded-full bg-gradient-to-tr from-brand-600 to-indigo-400 text-white flex items-center justify-center shrink-0 shadow-sm mt-0.5 text-xs font-bold">
          ✨
        </div>
        <div class="max-w-[85%] bg-white border border-slate-200/80 rounded-2xl rounded-tl-sm px-4 py-3 shadow-sm text-slate-800 space-y-1.5">
          <div class="text-xs font-semibold text-brand-600 mb-1 flex items-center justify-between border-b border-slate-100 pb-1.5">
            <div class="flex items-center gap-1.5">
              <span>Invoice AI</span>
              ${agentBadge}
            </div>
            <div class="flex items-center gap-1.5">
              ${durationBadge}
              <span class="text-[10px] font-normal text-slate-400">${timestamp}</span>
            </div>
          </div>
          <div class="text-sm leading-relaxed space-y-1.5 text-slate-800">${formattedContent}</div>
        </div>
      `;
    }

    if (this.dom.typingIndicator && this.dom.typingIndicator.parentNode === this.dom.messagesContainer) {
      this.dom.messagesContainer.insertBefore(messageEl, this.dom.typingIndicator);
    } else {
      this.dom.messagesContainer?.appendChild(messageEl);
    }

    this.scrollToBottom();
  }

  setLoading(isLoading) {
    this.state.isLoading = isLoading;
    this.updateSendButtonState();

    if (this.dom.typingIndicator) {
      if (isLoading) {
        this.dom.typingIndicator.classList.remove('hidden');
      } else {
        this.dom.typingIndicator.classList.add('hidden');
      }
    }
    this.scrollToBottom();
  }

  renderSuggestions(suggestions) {
    if (!this.dom.suggestionsContainer) return;
    this.dom.suggestionsContainer.innerHTML = '';

    suggestions.forEach(s => {
      const btn = document.createElement('button');
      btn.type = 'button';
      btn.className = 'text-xs bg-slate-100 hover:bg-brand-50 text-slate-700 hover:text-brand-700 border border-slate-200 hover:border-brand-200 rounded-full px-3 py-1.5 transition-colors font-medium text-left';
      btn.setAttribute('data-ai-prompt', s);
      btn.textContent = `✨ ${s}`;
      this.dom.suggestionsContainer.appendChild(btn);
    });

    this.scrollToBottom();
  }

  loadInitialSuggestions() {
    this.renderSuggestions([
      'Create customer Apex Innovations',
      'Create invoice for Apex Innovations',
      'Show overdue invoices',
      'Summarize invoice activity',
      'Which customer owes the most?'
    ]);
  }

  formatMarkdown(text) {
    if (!text) return '';

    // Convert INV-00000X to clickable badge links
    text = text.replace(/(INV-\d{6})/g, '<a href="/Invoices?Search=$1" class="inline-flex items-center gap-1 px-1.5 py-0.2 rounded font-mono text-xs font-bold bg-indigo-50 hover:bg-indigo-100 text-brand-700 border border-brand-200 transition-colors" title="View $1 in Invoices">$1 ↗</a>');

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
        if (line.includes('---')) continue;
        const cells = line.split('|').slice(1, -1).map(c => c.trim());

        if (!inTable) {
          inTable = true;
          tableHtml = '<div class="overflow-x-auto my-2.5"><table class="min-w-full text-xs divide-y divide-slate-200 border border-slate-200 rounded-lg overflow-hidden">';
          tableHtml += '<thead class="bg-slate-100 font-semibold text-slate-700"><tr>';
          cells.forEach(c => tableHtml += `<th class="py-1.5 px-2.5 text-left">${c}</th>`);
          tableHtml += '</tr></thead><tbody class="divide-y divide-slate-100 bg-white">';
        } else {
          tableHtml += '<tr class="hover:bg-slate-50 transition-colors">';
          cells.forEach(c => tableHtml += `<td class="py-1.5 px-2.5 text-slate-700">${c}</td>`);
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
          formattedLines.push(`<h4 class="text-sm font-bold text-slate-900 mt-2 mb-1">${line.substring(2)}</h4>`);
        } else if (line.startsWith('## ') || line.startsWith('### ')) {
          formattedLines.push(`<h5 class="text-xs font-bold text-slate-900 mt-1 mb-0.5">${line.replace(/^#+\s*/, '')}</h5>`);
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
    if (this.dom.messagesContainer) {
      requestAnimationFrame(() => {
        this.dom.messagesContainer.scrollTop = this.dom.messagesContainer.scrollHeight;
      });
    }
  }
}

// Instantiate on DOM load
document.addEventListener('DOMContentLoaded', () => {
  window.AiAssistant = new AiAssistantController();
});

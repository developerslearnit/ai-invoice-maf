/**
 * Invoices Page & Modal Controller
 * Dynamic Line Items, Real-time Subtotal/Total Math, Status Transitions & Draft Deletions
 */
window.InvoicesManager = (() => {
  let pendingStatusAction = null;
  let pendingDeleteAction = null;

  // Format Nigerian Naira in JavaScript
  function formatNaira(amount) {
    return '₦' + Number(amount || 0).toLocaleString('en-NG', {
      minimumFractionDigits: 2,
      maximumFractionDigits: 2
    });
  }

  // --- Create Invoice Modal ---
  function openCreateModal() {
    const form = document.getElementById('invoice-create-form');
    if (form) {
      form.reset();
      // Set default dates
      const today = new Date().toISOString().split('T')[0];
      const dueDate = new Date(Date.now() + 14 * 24 * 60 * 60 * 1000).toISOString().split('T')[0];
      const issueInput = document.getElementById('inv-issue-date');
      const dueInput = document.getElementById('inv-due-date');
      if (issueInput) issueInput.value = today;
      if (dueInput) dueInput.value = dueDate;

      // Reset items container to 1 blank row
      const itemsContainer = document.getElementById('invoice-items-container');
      if (itemsContainer) {
        itemsContainer.innerHTML = '';
        addItemRow();
      }
      recalculateTotals();
    }
    Modal.open('invoice-create-modal');
  }

  function addItemRow(desc = '', qty = 1, price = 0) {
    const container = document.getElementById('invoice-items-container');
    if (!container) return;

    const row = document.createElement('div');
    row.className = 'invoice-item-row grid grid-cols-12 gap-3 items-center bg-slate-50/70 p-3 rounded-xl border border-slate-200/80 transition-all';
    row.innerHTML = `
      <div class="col-span-12 sm:col-span-5">
        <label class="block sm:hidden text-xs font-medium text-slate-500 mb-1">Description</label>
        <input type="text" class="item-desc w-full text-sm rounded-lg border-slate-300 focus:border-brand-500 focus:ring-brand-500 py-2 px-3 border bg-white" placeholder="e.g. Software Architecture Consulting" value="${escapeHtml(desc)}" required />
      </div>
      <div class="col-span-4 sm:col-span-2">
        <label class="block sm:hidden text-xs font-medium text-slate-500 mb-1">Qty</label>
        <input type="number" step="1" min="1" class="item-qty w-full text-sm rounded-lg border-slate-300 focus:border-brand-500 focus:ring-brand-500 py-2 px-3 border bg-white text-right" value="${qty}" required />
      </div>
      <div class="col-span-5 sm:col-span-3">
        <label class="block sm:hidden text-xs font-medium text-slate-500 mb-1">Unit Price (₦)</label>
        <input type="number" step="0.01" min="0" class="item-price w-full text-sm rounded-lg border-slate-300 focus:border-brand-500 focus:ring-brand-500 py-2 px-3 border bg-white text-right" value="${price}" required />
      </div>
      <div class="col-span-3 sm:col-span-2 flex items-center justify-between sm:justify-end gap-2">
        <div class="text-right">
          <span class="item-line-total text-sm font-semibold text-slate-900">₦0.00</span>
        </div>
        <button type="button" class="remove-item-btn text-slate-400 hover:text-rose-500 p-1.5 rounded-lg hover:bg-rose-50 transition-colors" title="Remove row">
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"/></svg>
        </button>
      </div>
    `;

    container.appendChild(row);

    // Bind row recalculation
    const qtyInput = row.querySelector('.item-qty');
    const priceInput = row.querySelector('.item-price');
    const removeBtn = row.querySelector('.remove-item-btn');

    const updateRow = () => {
      const q = parseFloat(qtyInput.value) || 0;
      const p = parseFloat(priceInput.value) || 0;
      const total = q * p;
      row.querySelector('.item-line-total').textContent = formatNaira(total);
      recalculateTotals();
    };

    qtyInput.addEventListener('input', updateRow);
    priceInput.addEventListener('input', updateRow);

    removeBtn.addEventListener('click', () => {
      const rows = container.querySelectorAll('.invoice-item-row');
      if (rows.length <= 1) {
        Toast.warning('An invoice requires at least one item.');
        return;
      }
      row.remove();
      recalculateTotals();
    });

    updateRow();
  }

  function recalculateTotals() {
    const container = document.getElementById('invoice-items-container');
    if (!container) return;

    let total = 0;
    container.querySelectorAll('.invoice-item-row').forEach(row => {
      const q = parseFloat(row.querySelector('.item-qty')?.value) || 0;
      const p = parseFloat(row.querySelector('.item-price')?.value) || 0;
      total += (q * p);
    });

    const display = document.getElementById('inv-calculated-total');
    if (display) {
      display.textContent = formatNaira(total);
    }
  }

  async function handleCreateSubmit(e) {
    e.preventDefault();

    const customerId = document.getElementById('inv-customer-id')?.value;
    const issueDate = document.getElementById('inv-issue-date')?.value;
    const dueDate = document.getElementById('inv-due-date')?.value;
    const notes = document.getElementById('inv-notes')?.value || null;

    if (!customerId) {
      Toast.error('Please select a customer.');
      return;
    }

    const items = [];
    document.querySelectorAll('#invoice-items-container .invoice-item-row').forEach(row => {
      const desc = row.querySelector('.item-desc')?.value.trim();
      const qty = parseFloat(row.querySelector('.item-qty')?.value) || 0;
      const price = parseFloat(row.querySelector('.item-price')?.value) || 0;
      if (desc && qty > 0) {
        items.push({ description: desc, quantity: qty, unitPrice: price });
      }
    });

    if (items.length === 0) {
      Toast.error('Please add at least one valid line item with description and quantity.');
      return;
    }

    const payload = {
      customerId: customerId,
      issueDate: issueDate,
      dueDate: dueDate,
      notes: notes,
      items: items
    };

    const submitBtn = document.getElementById('inv-submit-btn');
    const spinner = document.getElementById('inv-submit-spinner');
    submitBtn.disabled = true;
    spinner?.classList.remove('hidden');

    try {
      const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';
      const res = await fetch('/invoices', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'X-CSRF-TOKEN': token
        },
        body: JSON.stringify(payload)
      });

      const result = await res.json();
      if (res.ok && result.success) {
        Modal.close('invoice-create-modal');
        Toast.success('Invoice created successfully.');
        setTimeout(() => {
          if (result.id) {
            window.location.href = `/invoices/${result.id}`;
          } else {
            window.location.reload();
          }
        }, 600);
      } else {
        Toast.error(result.message || 'Failed to create invoice.');
      }
    } catch (err) {
      console.error(err);
      Toast.error('Network failure while saving invoice.');
    } finally {
      submitBtn.disabled = false;
      spinner?.classList.add('hidden');
    }
  }

  // --- Status Transition Modal ---
  function promptStatusChange(invoiceId, invoiceNumber, currentStatus, targetStatus) {
    pendingStatusAction = { invoiceId, targetStatus };
    const modalText = document.getElementById('status-modal-desc');
    if (modalText) {
      modalText.textContent = `Mark invoice ${invoiceNumber} as ${targetStatus}? This will transition its state from ${currentStatus} to ${targetStatus}.`;
    }
    Modal.open('status-change-modal');
  }

  async function confirmStatusChange() {
    if (!pendingStatusAction) return;

    const { invoiceId, targetStatus } = pendingStatusAction;
    const confirmBtn = document.getElementById('status-confirm-btn');
    confirmBtn.disabled = true;

    try {
      const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';
      const res = await fetch(`/invoices/${invoiceId}/status`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'X-CSRF-TOKEN': token
        },
        body: JSON.stringify({ invoiceId, status: targetStatus })
      });

      const result = await res.json();
      if (res.ok && result.success) {
        Modal.close('status-change-modal');
        Toast.success(`Invoice transitioned to ${targetStatus}.`);
        setTimeout(() => window.location.reload(), 600);
      } else {
        Toast.error(result.message || 'Unable to transition status.');
      }
    } catch (err) {
      console.error(err);
      Toast.error('Network error updating status.');
    } finally {
      confirmBtn.disabled = false;
      pendingStatusAction = null;
    }
  }

  // --- Delete Draft Modal ---
  function promptDeleteDraft(invoiceId, invoiceNumber) {
    pendingDeleteAction = { invoiceId };
    const desc = document.getElementById('delete-modal-desc');
    if (desc) {
      desc.textContent = `Are you sure you want to delete invoice ${invoiceNumber}? This draft will be permanently removed.`;
    }
    Modal.open('delete-invoice-modal');
  }

  async function confirmDeleteDraft() {
    if (!pendingDeleteAction) return;

    const { invoiceId } = pendingDeleteAction;
    const confirmBtn = document.getElementById('delete-confirm-btn');
    confirmBtn.disabled = true;

    try {
      const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';
      const res = await fetch(`/invoices/${invoiceId}/delete`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'X-CSRF-TOKEN': token
        }
      });

      const result = await res.json();
      if (res.ok && result.success) {
        Modal.close('delete-invoice-modal');
        Toast.success('Draft invoice deleted.');
        setTimeout(() => window.location.href = '/invoices', 600);
      } else {
        Toast.error(result.message || 'Unable to delete invoice.');
      }
    } catch (err) {
      console.error(err);
      Toast.error('Network error deleting invoice.');
    } finally {
      confirmBtn.disabled = false;
      pendingDeleteAction = null;
    }
  }

  function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
  }

  // Bind forms & buttons on load
  document.addEventListener('DOMContentLoaded', () => {
    document.getElementById('invoice-create-form')?.addEventListener('submit', handleCreateSubmit);
    document.getElementById('add-item-row-btn')?.addEventListener('click', () => addItemRow());
    document.getElementById('status-confirm-btn')?.addEventListener('click', confirmStatusChange);
    document.getElementById('delete-confirm-btn')?.addEventListener('click', confirmDeleteDraft);
  });

  return {
    openCreateModal,
    addItemRow,
    promptStatusChange,
    promptDeleteDraft
  };
})();

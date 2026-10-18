/**
 * Customers Page Interactions
 * Handles Create & Edit Modals, Validation Display, and AJAX Submission
 */
window.CustomersManager = (() => {
  function openCreateModal() {
    const form = document.getElementById('customer-form');
    if (form) {
      form.reset();
      document.getElementById('customer-modal-id').value = '';
      document.getElementById('customer-modal-title').textContent = 'Add New Customer';
      document.getElementById('customer-submit-btn-text').textContent = 'Create Customer';
      clearValidationErrors(form);
    }
    Modal.open('customer-modal');
  }

  async function openEditModal(customerId) {
    const form = document.getElementById('customer-form');
    if (!form) return;
    form.reset();
    clearValidationErrors(form);

    document.getElementById('customer-modal-id').value = customerId;
    document.getElementById('customer-modal-title').textContent = 'Edit Customer';
    document.getElementById('customer-submit-btn-text').textContent = 'Save Changes';

    Modal.open('customer-modal');

    // Fetch existing customer data
    try {
      const res = await fetch(`/customers/get/${customerId}`);
      if (!res.ok) throw new Error('Failed to load customer');
      const data = await res.json();

      document.getElementById('customer-name').value = data.name || '';
      document.getElementById('customer-email').value = data.email || '';
      document.getElementById('customer-phone').value = data.phone || '';
      document.getElementById('customer-address').value = data.address || '';
    } catch (err) {
      console.error(err);
      Toast.error('Could not load customer details for editing.');
      Modal.close('customer-modal');
    }
  }

  async function handleFormSubmit(e) {
    e.preventDefault();
    const form = e.target;
    clearValidationErrors(form);

    const submitBtn = document.getElementById('customer-submit-btn');
    const submitText = document.getElementById('customer-submit-btn-text');
    const submitSpinner = document.getElementById('customer-submit-spinner');
    const customerId = document.getElementById('customer-modal-id').value;

    const isEdit = !!customerId;
    const url = isEdit ? `/customers/edit/${customerId}` : '/customers';

    const payload = {
      name: document.getElementById('customer-name').value.trim(),
      email: document.getElementById('customer-email').value.trim(),
      phone: document.getElementById('customer-phone').value.trim() || null,
      address: document.getElementById('customer-address').value.trim() || null
    };

    // Client-side quick checks
    if (!payload.name) {
      showFieldError('Name', 'Name is required.');
      return;
    }
    if (!payload.email) {
      showFieldError('Email', 'Email is required.');
      return;
    }

    // Set loading state
    submitBtn.disabled = true;
    submitSpinner?.classList.remove('hidden');

    try {
      const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';
      const response = await fetch(url, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'X-CSRF-TOKEN': token
        },
        body: JSON.stringify(payload)
      });

      const result = await response.json();

      if (response.ok && result.success) {
        Modal.close('customer-modal');
        Toast.success(result.message || (isEdit ? 'Customer updated.' : 'Customer created.'));
        setTimeout(() => window.location.reload(), 600);
      } else {
        if (result.errors) {
          for (const [key, msgs] of Object.entries(result.errors)) {
            showFieldError(key, msgs.join(' '));
          }
        }
        Toast.error(result.message || 'Operation failed. Please review the inputs.');
      }
    } catch (err) {
      console.error(err);
      Toast.error('A network error occurred while saving customer.');
    } finally {
      submitBtn.disabled = false;
      submitSpinner?.classList.add('hidden');
    }
  }

  function showFieldError(fieldName, message) {
    const normalized = fieldName.toLowerCase();
    const errorEl = document.querySelector(`[data-valmsg-for="${normalized}"]`)
      || document.querySelector(`[data-valmsg-for="${fieldName}"]`);
    if (errorEl) {
      errorEl.textContent = message;
      errorEl.classList.remove('hidden');
    }
  }

  function clearValidationErrors(form) {
    form.querySelectorAll('[data-valmsg-for]').forEach(el => {
      el.textContent = '';
      el.classList.add('hidden');
    });
  }

  document.addEventListener('DOMContentLoaded', () => {
    const form = document.getElementById('customer-form');
    form?.addEventListener('submit', handleFormSubmit);
  });

  return { openCreateModal, openEditModal };
})();

/**
 * Accessible, Responsive Vanilla JS Modal Controller
 */
window.Modal = (() => {
  let activeModal = null;

  function open(modalId) {
    const modal = document.getElementById(modalId);
    if (!modal) return;

    modal.classList.remove('hidden');
    document.body.classList.add('overflow-hidden');
    activeModal = modal;

    // Focus first interactive element or modal itself
    const focusable = modal.querySelector('button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])');
    if (focusable) {
      setTimeout(() => focusable.focus(), 50);
    }
  }

  function close(modalId) {
    const modal = modalId ? document.getElementById(modalId) : activeModal;
    if (!modal) return;

    modal.classList.add('hidden');
    document.body.classList.remove('overflow-hidden');
    if (activeModal === modal) {
      activeModal = null;
    }
  }

  // Global listeners
  document.addEventListener('keydown', (e) => {
    if (e.key === 'Escape' && activeModal) {
      close();
    }
  });

  document.addEventListener('click', (e) => {
    // Backdrop click
    if (e.target.hasAttribute('data-modal-backdrop')) {
      close(e.target.closest('[id]')?.id);
    }
    // Close button click
    if (e.target.closest('[data-modal-close]')) {
      const btn = e.target.closest('[data-modal-close]');
      const targetId = btn.getAttribute('data-modal-close');
      close(targetId);
    }
  });

  return { open, close };
})();

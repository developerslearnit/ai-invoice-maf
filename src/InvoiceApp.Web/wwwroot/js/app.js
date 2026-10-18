/**
 * Main Web App Orchestrator
 * Responsive Sidebar Drawer, Collapsible Navigation, Global Shortcuts
 */
document.addEventListener('DOMContentLoaded', () => {
  // Mobile Sidebar Toggle
  const mobileMenuBtn = document.getElementById('mobile-menu-btn');
  const sidebar = document.getElementById('app-sidebar');
  const backdrop = document.getElementById('sidebar-backdrop');

  function openSidebar() {
    sidebar?.classList.remove('-translate-x-full');
    backdrop?.classList.remove('hidden');
    document.body.classList.add('overflow-hidden');
  }

  function closeSidebar() {
    sidebar?.classList.add('-translate-x-full');
    backdrop?.classList.add('hidden');
    document.body.classList.remove('overflow-hidden');
  }

  mobileMenuBtn?.addEventListener('click', openSidebar);
  backdrop?.addEventListener('click', closeSidebar);

  // Trigger AI assistant from sidebar link
  document.getElementById('sidebar-ai-btn')?.addEventListener('click', (e) => {
    e.preventDefault();
    if (window.innerWidth < 1024) {
      closeSidebar();
    }
    window.AiAssistant?.open();
  });
});

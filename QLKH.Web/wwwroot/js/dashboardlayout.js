/* =============================================
   QLKH Dashboard Layout — dashboardlayout.js
   Vanilla JS · No dependencies · Clean
   ============================================= */

(function () {
    'use strict';

    /* ------------------------------------------------
       Elements
    ------------------------------------------------ */
    var sidebar = document.getElementById('dbSidebar');
    var overlay = document.getElementById('dbOverlay');
    var toggle = document.getElementById('dbMenuToggle');
    var closeBtn = document.getElementById('dbSidebarClose');

    /* ------------------------------------------------
       Sidebar open / close
    ------------------------------------------------ */
    function openSidebar() {
        if (!sidebar || !overlay) return;
        sidebar.classList.add('open');
        overlay.classList.add('active');
        document.body.style.overflow = 'hidden';
        if (toggle) toggle.setAttribute('aria-expanded', 'true');
    }

    function closeSidebar() {
        if (!sidebar || !overlay) return;
        sidebar.classList.remove('open');
        overlay.classList.remove('active');
        document.body.style.overflow = '';
        if (toggle) toggle.setAttribute('aria-expanded', 'false');
    }

    if (toggle) toggle.addEventListener('click', openSidebar);
    if (closeBtn) closeBtn.addEventListener('click', closeSidebar);
    if (overlay) overlay.addEventListener('click', closeSidebar);

    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') closeSidebar();
    });

    window.addEventListener('resize', function () {
        if (window.innerWidth >= 992) closeSidebar();
    });

    /* Close sidebar khi click nav link trên mobile */
    var navLinks = document.querySelectorAll('#dbNav .db-nav-link');
    navLinks.forEach(function (link) {
        link.addEventListener('click', function () {
            if (window.innerWidth < 992) closeSidebar();
        });
    });

    /* ------------------------------------------------
       Active nav link theo URL
    ------------------------------------------------ */
    var path = window.location.pathname.replace(/\/+$/, '') || '/';

    navLinks.forEach(function (link) {
        var href = link.getAttribute('href');
        if (!href || href === '#' || href.startsWith('javascript')) return;
        var normalized = href.replace(/\/+$/, '') || '/';
        var isActive = normalized === path ||
            (normalized !== '/' && path.toLowerCase().startsWith(normalized.toLowerCase()));
        if (isActive) link.classList.add('active');
    });

    /* ------------------------------------------------
       Alerts — auto-dismiss + close button
    ------------------------------------------------ */
    var AUTO_DISMISS_MS = 5000;

    function dismissAlert(el) {
        el.classList.add('hiding');
        setTimeout(function () {
            if (el.parentNode) el.parentNode.removeChild(el);
        }, 360);
    }

    document.querySelectorAll('#dbAlertZone .db-alert').forEach(function (alert) {
        /* Close button */
        var btn = alert.querySelector('.db-alert-close');
        if (btn) btn.addEventListener('click', function () { dismissAlert(alert); });
        /* Auto */
        setTimeout(function () {
            if (alert.parentNode) dismissAlert(alert);
        }, AUTO_DISMISS_MS);
    });

    /* ------------------------------------------------
       Topbar title — tự điền từ <h1> trong content
    ------------------------------------------------ */
    var titleEl = document.querySelector('.db-topbar-title');
    if (titleEl && !titleEl.textContent.trim()) {
        var h1 = document.querySelector('.db-content h1, .db-content .db-page-title');
        if (h1) titleEl.textContent = h1.textContent.trim();
    }

})();
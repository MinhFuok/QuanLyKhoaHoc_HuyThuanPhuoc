/* =============================================
   QLKH Dashboard Layout — dashboardlayout.js
   Vanilla JS · No dependencies · Clean
   ============================================= */

(function () {
    'use strict';

    function $$(sel, ctx) {
        return Array.from((ctx || document).querySelectorAll(sel));
    }

    function mulberry32(seed) {
        return function () {
            var t = seed += 0x6D2B79F5;
            t = Math.imul(t ^ t >>> 15, t | 1);
            t ^= t + Math.imul(t ^ t >>> 7, t | 61);
            return ((t ^ t >>> 14) >>> 0) / 4294967296;
        };
    }

    function buildStarShadow(count, maxX, maxY, color, seed) {
        var random = mulberry32(seed);
        var points = [];

        for (var i = 0; i < count; i += 1) {
            points.push(
                Math.round(random() * maxX) + 'px ' +
                Math.round(random() * maxY) + 'px ' +
                color
            );
        }

        return points.join(', ');
    }

    /* ------------------------------------------------
       Elements
    ------------------------------------------------ */
    var sidebar = document.getElementById('dbSidebar');
    var overlay = document.getElementById('dbOverlay');
    var toggle = document.getElementById('dbMenuToggle');
    var closeBtn = document.getElementById('dbSidebarClose');

    /* ------------------------------------------------
       Shared Admin background
    ------------------------------------------------ */
    function initAdminSpaceBackground() {
        if (!document.body.classList.contains('db-body-admin')) return;

        var host = document.querySelector('.db-main-wrap');
        if (!host || host.querySelector('.da-space-bg')) return;

        var bg = document.createElement('div');
        bg.className = 'da-space-bg';
        bg.setAttribute('aria-hidden', 'true');
        bg.innerHTML = [
            '<div class="da-space-layer da-space-layer-1">',
            '<span class="da-space-stars da-space-stars-1"></span>',
            '<span class="da-space-stars da-space-stars-1 da-space-stars-dup"></span>',
            '</div>',
            '<div class="da-space-layer da-space-layer-2">',
            '<span class="da-space-stars da-space-stars-2"></span>',
            '<span class="da-space-stars da-space-stars-2 da-space-stars-dup"></span>',
            '</div>',
            '<div class="da-space-layer da-space-layer-3">',
            '<span class="da-space-stars da-space-stars-3"></span>',
            '<span class="da-space-stars da-space-stars-3 da-space-stars-dup"></span>',
            '</div>',
        ].join('');

        host.insertBefore(bg, host.firstChild);
        host.classList.add('da-space-host');

        var span = 4200;
        var configs = [
            { selector: '.da-space-stars-1', count: 780, color: 'rgba(255,255,255,.88)', seed: 11 },
            { selector: '.da-space-stars-2', count: 220, color: 'rgba(255,255,255,.78)', seed: 29 },
            { selector: '.da-space-stars-3', count: 95, color: 'rgba(255,255,255,.62)', seed: 53 }
        ];

        host.style.setProperty('--da-space-span', span + 'px');
        host.style.setProperty('--da-space-span-negative', '-' + span + 'px');

        configs.forEach(function (config) {
            var shadow = buildStarShadow(config.count, 2000, span, config.color, config.seed);
            $$(config.selector, bg).forEach(function (el) {
                el.style.boxShadow = shadow;
            });
        });
    }

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
       Fix: không dùng startsWith để tránh StudentCertificate
       bị active luôn Student
    ------------------------------------------------ */
    var path = window.location.pathname.replace(/\/+$/, '') || '/';

    navLinks.forEach(function (link) {
        link.classList.remove('active');

        var href = link.getAttribute('href');
        if (!href || href === '#' || href.startsWith('javascript')) return;

        var normalized = href.replace(/\/+$/, '') || '/';
        var isActive = normalized.toLowerCase() === path.toLowerCase();

        if (isActive) {
            link.classList.add('active');
        }
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
        var btn = alert.querySelector('.db-alert-close');
        if (btn) {
            btn.addEventListener('click', function () {
                dismissAlert(alert);
            });
        }

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

    initAdminSpaceBackground();

})();

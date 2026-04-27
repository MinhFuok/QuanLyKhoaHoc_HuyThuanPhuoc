(function () {
    'use strict';

    function $$(selector, context) {
        return Array.from((context || document).querySelectorAll(selector));
    }

    function updateRow(row) {
        var input = row.querySelector('input[type="checkbox"], input[type="radio"]');
        if (!input) return;
        row.classList.toggle('is-checked', !!input.checked);
        row.classList.toggle('is-disabled', !!input.disabled);
    }

    function initAmbient() {
        var mainWrap = document.querySelector('.db-main-wrap');
        if (!mainWrap || mainWrap.querySelector('.ap-ambient')) return;

        var ambient = document.createElement('div');
        ambient.className = 'ap-ambient';
        ambient.setAttribute('aria-hidden', 'true');
        ambient.innerHTML = [
            '<span class="ap-blob ap-blob-violet"></span>',
            '<span class="ap-blob ap-blob-pink"></span>',
            '<span class="ap-blob ap-blob-sky"></span>',
            '<span class="ap-blob ap-blob-emerald"></span>'
        ].join('');

        mainWrap.insertBefore(ambient, mainWrap.firstChild);
    }

    function initCheckRows() {
        var rows = $$('.admin-check-row');
        if (!rows.length) return;

        rows.forEach(function (row) {
            updateRow(row);

            row.addEventListener('click', function (event) {
                if (row.tagName === 'LABEL') return;
                if (event.target.closest('input, button, a, select, textarea')) return;

                var input = row.querySelector('input[type="checkbox"], input[type="radio"]');
                if (!input || input.disabled) return;

                if (input.type === 'checkbox') input.checked = !input.checked;
                else input.checked = true;

                input.dispatchEvent(new Event('change', { bubbles: true }));
            });
        });

        document.addEventListener('change', function (event) {
            if (!event.target.matches('.admin-check-input, .admin-check-row input[type="checkbox"], .admin-check-row input[type="radio"]')) {
                return;
            }

            rows.forEach(updateRow);
        });
    }

    function initReveal() {
        var targets = $$('.admin-account-page .admin-page-header, .admin-account-page .admin-filter-card, .admin-account-page .admin-table-card, .admin-account-page .admin-form-card, .admin-account-page .admin-delete-card, .admin-account-page .admin-system-card, .admin-account-page .admin-feature-card, .admin-account-page .admin-section-box, .admin-account-page .admin-info-box, .admin-account-page .admin-warning-box, .admin-account-page .admin-danger-box, .admin-account-page .admin-current-image-box');
        if (!targets.length) return;

        targets.forEach(function (el) { el.classList.add('ap-reveal'); });

        if (!('IntersectionObserver' in window)) {
            targets.forEach(function (el) { el.classList.add('is-visible'); });
            return;
        }

        var observer = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                if (!entry.isIntersecting) return;
                entry.target.classList.add('is-visible');
                observer.unobserve(entry.target);
            });
        }, { threshold: 0.08, rootMargin: '0px 0px -40px 0px' });

        targets.forEach(function (el, index) {
            el.style.transitionDelay = Math.min(index * 0.04, 0.24) + 's';
            observer.observe(el);
        });
    }

    function initGlow() {
        if (!window.matchMedia || !window.matchMedia('(pointer: fine)').matches) return;

        var cards = $$('.admin-account-page .admin-page-header, .admin-account-page .admin-filter-card, .admin-account-page .admin-table-card, .admin-account-page .admin-form-card, .admin-account-page .admin-delete-card, .admin-account-page .admin-system-card, .admin-account-page .admin-feature-card, .admin-account-page .admin-section-box, .admin-account-page .admin-info-box, .admin-account-page .admin-warning-box, .admin-account-page .admin-danger-box, .admin-account-page .admin-current-image-box, .admin-account-page .admin-check-row');

        cards.forEach(function (el) {
            var rafId = 0;
            var nextX = 50;
            var nextY = 50;

            function commit() {
                rafId = 0;
                el.style.setProperty('--ap-glow-x', nextX.toFixed(2) + '%');
                el.style.setProperty('--ap-glow-y', nextY.toFixed(2) + '%');
            }

            function queue(x, y) {
                nextX = x;
                nextY = y;
                if (rafId) return;
                rafId = requestAnimationFrame(commit);
            }

            el.addEventListener('pointerenter', function (event) {
                if (event.pointerType && event.pointerType !== 'mouse' && event.pointerType !== 'pen') return;
                var rect = el.getBoundingClientRect();
                el.style.setProperty('--ap-glow-alpha', '0.32');
                queue(((event.clientX - rect.left) / rect.width) * 100, ((event.clientY - rect.top) / rect.height) * 100);
            });

            el.addEventListener('pointermove', function (event) {
                if (event.pointerType && event.pointerType !== 'mouse' && event.pointerType !== 'pen') return;
                var rect = el.getBoundingClientRect();
                queue(((event.clientX - rect.left) / rect.width) * 100, ((event.clientY - rect.top) / rect.height) * 100);
            });

            el.addEventListener('pointerleave', function () {
                if (rafId) cancelAnimationFrame(rafId);
                rafId = 0;
                el.style.setProperty('--ap-glow-alpha', '0');
                el.style.setProperty('--ap-glow-x', '50%');
                el.style.setProperty('--ap-glow-y', '50%');
            });
        });
    }

    function init() {
        if (!document.body.classList.contains('db-body-admin')) return;
        initAmbient();
        initCheckRows();
        initReveal();
        initGlow();
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }
})();

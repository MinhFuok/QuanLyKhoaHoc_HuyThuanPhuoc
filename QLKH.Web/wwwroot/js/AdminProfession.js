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
        var selectors = [
            '.admin-account-page .admin-page-header',
            '.admin-account-page .admin-filter-card',
            '.admin-account-page .admin-table-card',
            '.admin-account-page .admin-form-card',
            '.admin-account-page .admin-delete-card',
            '.admin-account-page .admin-system-card',
            '.admin-account-page .admin-feature-card',
            '.admin-account-page .admin-section-box',
            '.admin-account-page .admin-info-box',
            '.admin-account-page .admin-warning-box',
            '.admin-account-page .admin-danger-box',
            '.admin-account-page .admin-current-image-box'
        ];

        var targets = [];
        selectors.forEach(function (selector) {
            $$(selector).forEach(function (el) {
                el.classList.add('ap-reveal');
                targets.push(el);
            });
        });

        if (!targets.length) return;

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
        }, { threshold: 0.1, rootMargin: '0px 0px -40px 0px' });

        targets.forEach(function (el, index) {
            el.style.transitionDelay = Math.min(index * 0.04, 0.2) + 's';
            observer.observe(el);
        });
    }

    function init() {
        if (!document.body.classList.contains('db-body-admin')) return;
        initCheckRows();
        initReveal();
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }
})();

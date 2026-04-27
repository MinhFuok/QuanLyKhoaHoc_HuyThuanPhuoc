(function () {
    'use strict';

    function $$(selector, context) {
        return Array.from((context || document).querySelectorAll(selector));
    }

    function updateRow(row) {
        var input = row.querySelector('input[type="checkbox"], input[type="radio"]');
        if (!input) return;
        row.classList.toggle('is-checked', !!input.checked);
    }

    function refreshRows() {
        $$('.admin-check-row').forEach(updateRow);
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

                if (input.type === 'checkbox') {
                    input.checked = !input.checked;
                } else {
                    input.checked = true;
                }

                input.dispatchEvent(new Event('change', { bubbles: true }));
            });
        });

        document.addEventListener('change', function (event) {
            if (!event.target.matches('.admin-check-input, .admin-check-row input[type="checkbox"], .admin-check-row input[type="radio"]')) {
                return;
            }

            refreshRows();
        });
    }

    function init() {
        if (!document.body.classList.contains('db-body-admin')) return;
        if (!document.querySelector('.admin-account-page')) return;
        initCheckRows();
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }
})();

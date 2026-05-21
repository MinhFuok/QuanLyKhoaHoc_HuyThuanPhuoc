(function (window, document) {
    'use strict';

    if (window.AdminPages) {
        return;
    }

    var core = window.AdminCore;

    if (!core) {
        return;
    }

    var q = core.q;
    var qq = core.qq;

    function initCheckRows() {
        qq('.admin-check-row').forEach(function (row) {
            if (!core.once(row, 'adminPagesCheckReady')) {
                return;
            }

            function syncRowState() {
                var input = q('input[type="checkbox"], input[type="radio"]', row);

                if (!input) {
                    return;
                }

                row.classList.toggle('is-checked', !!input.checked);
                row.classList.toggle('is-disabled', !!input.disabled);
            }

            syncRowState();

            row.addEventListener('click', function (event) {
                if (event.target.closest('input, button, a, select, textarea, label')) {
                    return;
                }

                var input = q('input[type="checkbox"], input[type="radio"]', row);

                if (!input || input.disabled) {
                    return;
                }

                if (input.type === 'checkbox') {
                    input.checked = !input.checked;
                } else {
                    input.checked = true;
                }

                input.dispatchEvent(new Event('change', { bubbles: true }));
            });

            row.addEventListener('change', syncRowState);

            qq('input[type="checkbox"], input[type="radio"]', row).forEach(function (input) {
                input.addEventListener('change', syncRowState);
            });
        });
    }

    function initFormEnhancements(selector) {
        qq(selector).forEach(function (form) {
            if (!core.once(form, 'adminPagesFormReady')) {
                return;
            }

            var firstError = q('.input-validation-error', form);

            if (firstError) {
                window.setTimeout(function () {
                    firstError.focus();
                }, 150);
            }

            qq('input, select, textarea', form).forEach(function (field) {
                field.addEventListener('input', function () {
                    field.classList.remove('input-validation-error');
                });

                if (field.tagName === 'SELECT') {
                    field.addEventListener('change', function () {
                        field.classList.remove('input-validation-error');
                    });
                }
            });

            qq('.cp-textarea, .admin-textarea', form).forEach(function (textarea) {
                function resize() {
                    textarea.style.height = 'auto';
                    textarea.style.height = (textarea.scrollHeight + 2) + 'px';
                }

                textarea.addEventListener('input', resize);
                resize();
            });
        });
    }

    function initDeleteForms(selector) {
        qq(selector).forEach(function (form) {
            if (!core.once(form, 'adminPagesDeleteReady')) {
                return;
            }

            form.addEventListener('submit', function (event) {
                var label = form.getAttribute('data-delete-label') || 'mục này';
                var message = form.getAttribute('data-delete-message') || ('Xác nhận xóa ' + label + '?\n\nHành động này không thể hoàn tác.');

                if (window.confirm(message)) {
                    return;
                }

                event.preventDefault();

                var card = form.closest('.cp-delete-card, .admin-delete-card, .cp-form-card');

                if (!card) {
                    return;
                }

                var offsets = [8, -7, 5, -4, 3, 0];
                offsets.forEach(function (offset, index) {
                    window.setTimeout(function () {
                        card.style.transform = 'translateX(' + offset + 'px)';
                        card.style.transition = 'transform 70ms ease';
                    }, index * 60);
                });
            });
        });
    }

    function initInlineAlerts() {
        qq('[data-cp-alert]').forEach(function (alert) {
            if (!core.once(alert, 'adminPagesAlertReady')) {
                return;
            }

            var closeButton = q('[data-cp-alert-close]', alert);

            if (closeButton) {
                closeButton.addEventListener('click', function () {
                    core.dismissAlert(alert);
                });
            }

            var delay = Number(alert.getAttribute('data-cp-alert-delay') || 4200);

            if (delay > 0) {
                window.setTimeout(function () {
                    core.dismissAlert(alert);
                }, delay);
            }
        });
    }

    function initEnrollmentTabs() {
        qq('[data-enrollment-tabs]').forEach(function (wrapper) {
            if (!core.once(wrapper, 'adminPagesTabsReady')) {
                return;
            }

            var buttons = qq('.enrollment-tab-btn', wrapper);
            var panes = qq('.enrollment-tab-pane', wrapper);

            if (!buttons.length || !panes.length) {
                return;
            }

            function activate(button) {
                var targetId = button.getAttribute('data-tab');
                var target = targetId ? document.getElementById(targetId) : null;

                if (!target) {
                    return;
                }

                buttons.forEach(function (item) {
                    var isActive = item === button;
                    item.classList.toggle('active', isActive);
                    item.setAttribute('aria-selected', isActive ? 'true' : 'false');
                });

                panes.forEach(function (pane) {
                    var isActive = pane === target;
                    pane.classList.toggle('active', isActive);
                    pane.hidden = !isActive;
                });
            }

            buttons.forEach(function (button) {
                button.addEventListener('click', function () {
                    activate(button);
                });
            });

            activate(q('.enrollment-tab-btn.active', wrapper) || buttons[0]);
        });
    }

    function initFileInputs() {
        qq('[data-cp-file-input]').forEach(function (input) {
            if (!core.once(input, 'adminPagesFileReady')) {
                return;
            }

            var targetId = input.getAttribute('data-cp-file-target');
            var target = targetId ? document.getElementById(targetId) : null;

            if (!target) {
                return;
            }

            var originalText = target.textContent;

            function syncLabel() {
                var fileName = input.files && input.files.length ? input.files[0].name : '';
                target.textContent = fileName || originalText;
            }

            input.addEventListener('change', syncLabel);
            syncLabel();
        });
    }

    function initCodeCopy() {
        qq('.cp-code').forEach(function (chip) {
            if (!core.once(chip, 'adminPagesCodeReady')) {
                return;
            }

            chip.style.cursor = 'pointer';
            chip.title = 'Nhấn để sao chép';

            chip.addEventListener('click', function () {
                if (!navigator.clipboard) {
                    return;
                }

                var text = chip.textContent.trim();
                var original = chip.textContent;

                navigator.clipboard.writeText(text).then(function () {
                    chip.textContent = 'Đã sao chép';
                    chip.style.background = 'linear-gradient(135deg, rgba(22,163,74,.14) 0%, rgba(34,197,94,.1) 100%)';
                    chip.style.color = '#15803d';

                    window.setTimeout(function () {
                        chip.textContent = original;
                        chip.style.background = '';
                        chip.style.color = '';
                    }, 1400);
                });
            });
        });
    }

    function initOverflowTitles() {
        qq('.cp-name, .admin-name, .admin-email').forEach(function (element) {
            if (element.scrollWidth > element.clientWidth) {
                element.title = element.textContent.trim();
            }
        });
    }

    function decorateAddButtons() {
        qq('.cp-btn').forEach(function (button) {
            var text = (button.textContent || '').trim();

            if (/^[+＋]/.test(text)) {
                button.classList.add('cp-btn--add');
            }
        });
    }

    function buildEditIcon(cssClass) {
        return '' +
            '<svg class="' + cssClass + '" viewBox="0 0 512 512" aria-hidden="true">' +
            '<path d="M410.3 231l11.3-11.3-33.9-33.9-62.1-62.1L291.7 89.8l-11.3 11.3-22.6 22.6L58.6 322.9c-10.4 10.4-18 23.3-22.2 37.4L1 480.7c-2.5 8.4-.2 17.5 6.1 23.7s15.3 8.5 23.7 6.1l120.3-35.4c14.1-4.2 27-11.8 37.4-22.2L387.7 253.7 410.3 231zM160 399.4l-9.1 22.7c-4 3.1-8.5 5.4-13.3 6.9L59.4 452l23-78.1c1.4-4.9 3.8-9.4 6.9-13.3l22.7-9.1v32c0 8.8 7.2 16 16 16h32zM362.7 18.7L348.3 33.2 325.7 55.8 314.3 67.1l33.9 33.9 62.1 62.1 33.9 33.9 11.3-11.3 22.6-22.6 14.5-14.5c25-25 25-65.5 0-90.5L453.3 18.7c-25-25-65.5-25-90.5 0zm-47.4 168l-144 144c-6.2 6.2-16.4 6.2-22.6 0s-6.2-16.4 0-22.6l144-144c6.2-6.2 16.4-6.2 22.6 0s6.2 16.4 0 22.6z"></path>' +
            '</svg>';
    }

    function buildDeleteIcon(maskId, topClass, bottomClass, trashClass) {
        return '' +
            '<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 39 7" class="' + topClass + '" aria-hidden="true">' +
            '<line stroke-width="4" stroke="white" y2="5" x2="39" y1="5"></line>' +
            '<line stroke-width="3" stroke="white" y2="1.5" x2="26.0357" y1="1.5" x1="12"></line>' +
            '</svg>' +
            '<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 33 39" class="' + bottomClass + '" aria-hidden="true">' +
            '<mask fill="white" id="' + maskId + '">' +
            '<path d="M0 0H33V35C33 37.2091 31.2091 39 29 39H4C1.79086 39 0 37.2091 0 35V0Z"></path>' +
            '</mask>' +
            '<path mask="url(#' + maskId + ')" fill="white" d="M0 0H33H0ZM37 35C37 39.4183 33.4183 43 29 43H4C-0.418278 43 -4 39.4183 -4 35H4H29H37ZM4 43C-0.418278 43 -4 39.4183 -4 35V0H4V35V43ZM37 0V35C37 39.4183 33.4183 43 29 43V35V0H37Z"></path>' +
            '<path stroke-width="4" stroke="white" d="M12 6L12 29"></path>' +
            '<path stroke-width="4" stroke="white" d="M21 6V29"></path>' +
            '</svg>' +
            '<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 89 80" class="' + trashClass + '" aria-hidden="true">' +
            '<path fill="white" d="M20.5 10.5L37.5 15.5L42.5 11.5L51.5 12.5L68.75 0L72 11.5L79.5 12.5H88.5L87 22L68.75 31.5L75.5066 25L86 26L87 35.5L77.5 48L70.5 49.5L80 50L77.5 71.5L63.5 58.5L53.5 68.5L65.5 70.5L45.5 73L35.5 79.5L28 67L16 63L12 51.5L0 48L16 25L22.5 17L20.5 10.5Z"></path>' +
            '</svg>';
    }

    function decorateHocVuActionButtons() {
        var deleteCounter = 0;

        qq('.cp-actions .cp-btn').forEach(function (button) {
            if (!core.once(button, 'adminPagesActionReady')) {
                return;
            }

            var text = core.normalizeText(button.textContent);

            if (text !== 'sửa' && text !== 'xóa') {
                return;
            }

            button.classList.remove('cp-btn', 'cp-btn--primary', 'cp-btn--ghost', 'cp-btn--info', 'cp-btn--warning', 'cp-btn--danger', 'cp-btn--sm', 'cp-btn--add');
            button.classList.add('cp-icon-action', text === 'sửa' ? 'cp-action-edit' : 'cp-action-delete');
            button.setAttribute('data-label', text === 'sửa' ? 'Sửa' : 'Xóa');
            button.setAttribute('title', text === 'sửa' ? 'Sửa' : 'Xóa');
            button.setAttribute('aria-label', text === 'sửa' ? 'Sửa' : 'Xóa');

            if (text === 'sửa') {
                button.innerHTML = buildEditIcon('cp-edit-svg');
                return;
            }

            deleteCounter += 1;
            button.innerHTML = buildDeleteIcon('cp-delete-mask-' + deleteCounter, 'cp-bin-top', 'cp-bin-bottom', 'cp-garbage');
        });
    }

    function initDepartmentTree() {
        qq('.toggle-arrow[data-target]').forEach(function (button) {
            if (!core.once(button, 'adminPagesToggleReady')) {
                return;
            }

            var targetClass = button.getAttribute('data-target');
            var icon = q('.toggle-arrow-icon', button);
            var rows = targetClass ? qq('.' + targetClass) : [];

            if (!rows.length) {
                return;
            }

            rows.forEach(function (row) {
                row.hidden = false;
            });

            button.setAttribute('aria-expanded', 'true');

            button.addEventListener('click', function () {
                var shouldOpen = button.getAttribute('aria-expanded') !== 'true';

                rows.forEach(function (row) {
                    row.hidden = !shouldOpen;
                });

                button.setAttribute('aria-expanded', shouldOpen ? 'true' : 'false');

                if (icon) {
                    icon.textContent = shouldOpen ? '▾' : '▸';
                }
            });
        });
    }

    function mountAdmin() {
        if (!document.body.classList.contains('db-body-admin')) {
            return;
        }

        var targets = [];
        [
            '.admin-page-header',
            '.admin-filter-card',
            '.admin-table-card',
            '.admin-form-card',
            '.admin-delete-card',
            '.admin-system-card',
            '.admin-feature-card',
            '.admin-section-box',
            '.admin-info-box',
            '.admin-warning-box',
            '.admin-danger-box',
            '.admin-current-image-box'
        ].forEach(function (selector) {
            targets = targets.concat(qq('.admin-account-page ' + selector));
        });

        core.revealElements(targets, 'ap-reveal', 0.04);
        initCheckRows();
        initFormEnhancements('.admin-form-card form');
        initDeleteForms('form[data-delete-form]');
        initDepartmentTree();
        initOverflowTitles();
    }

    function mountHocVu() {
        if (!document.body.classList.contains('db-body-hocvu')) {
            return;
        }

        var targets = [];
        [
            '.cp-header',
            '.cp-filter-card',
            '.student-filter-card',
            '.teacher-filter-card',
            '.enrollment-filter-card',
            '.cp-table-wrap',
            '.cp-form-card',
            '.cp-detail-card',
            '.cp-delete-card',
            '.cp-switch-card',
            '.classroom-tabs',
            '.enrollment-tabs'
        ].forEach(function (selector) {
            targets = targets.concat(qq(selector));
        });

        core.revealElements(targets, 'hv-reveal', 0.04);
        initFormEnhancements('form[data-admin-form], #courseForm, #classroomForm, #studentForm, #enrollmentForm');
        initDeleteForms('form[data-delete-form], #deleteForm');
        initInlineAlerts();
        initEnrollmentTabs();
        initFileInputs();
        initCodeCopy();
        initOverflowTitles();
        decorateAddButtons();
        decorateHocVuActionButtons();
    }

    window.AdminPages = {
        mountAdmin: mountAdmin,
        mountHocVu: mountHocVu
    };
})(window, document);

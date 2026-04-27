(function () {
    'use strict';

    function $(selector, context) {
        return (context || document).querySelector(selector);
    }

    function $$(selector, context) {
        return Array.from((context || document).querySelectorAll(selector));
    }

    function initReveal() {
        var selectors = [
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
        ];

        var targets = [];
        selectors.forEach(function (selector) {
            $$(selector).forEach(function (el) {
                el.classList.add('hv-reveal');
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
            el.style.transitionDelay = Math.min(index * 0.04, 0.24) + 's';
            observer.observe(el);
        });
    }

    function initAdminForm(form) {
        if (form.dataset.cpFormReady === 'true') return;
        form.dataset.cpFormReady = 'true';

        var firstErr = form.querySelector('.input-validation-error');
        if (firstErr) {
            setTimeout(function () { firstErr.focus(); }, 200);
        }

        $$('.cp-input', form).forEach(function (input) {
            input.addEventListener('input', function () {
                input.classList.remove('input-validation-error');

                var name = input.id || input.name;
                if (!name) return;

                var span = form.querySelector('[data-valmsg-for="' + name + '"]');
                if (!span) return;

                span.textContent = '';
                span.classList.remove('field-validation-error');
            });
        });

        $$('.cp-textarea', form).forEach(function (textarea) {
            function resize() {
                textarea.style.height = 'auto';
                textarea.style.height = (textarea.scrollHeight + 2) + 'px';
            }

            textarea.addEventListener('input', resize);
            resize();
        });

        form.addEventListener('submit', function () {
            var errCount = form.querySelectorAll('.input-validation-error, .field-validation-error:not(:empty)').length;
            if (errCount) return;

            var btn = form.querySelector('[type="submit"]');
            if (!btn || btn.dataset.loading === 'true') return;

            btn.dataset.loading = 'true';
            btn.disabled = true;

            var original = btn.innerHTML;
            var loadingText = form.getAttribute('data-loading-text') || 'Đang lưu...';
            btn.innerHTML = '<span>' + loadingText + '</span>';

            setTimeout(function () {
                btn.disabled = false;
                btn.dataset.loading = 'false';
                btn.innerHTML = original;
            }, 9000);
        });
    }

    function initDeleteForm(form) {
        if (form.dataset.cpDeleteReady === 'true') return;
        form.dataset.cpDeleteReady = 'true';

        form.addEventListener('submit', function (event) {
            var label = form.getAttribute('data-delete-label') || 'mục này';
            var message = form.getAttribute('data-delete-message')
                || ('Xác nhận xóa ' + label + '?\n\nHành động này không thể hoàn tác.');

            if (window.confirm(message)) return;

            event.preventDefault();

            var card = form.closest('.cp-delete-card');
            if (!card) return;

            var steps = [8, -7, 5, -4, 3, 0];
            steps.forEach(function (px, index) {
                setTimeout(function () {
                    card.style.transform = 'translateX(' + px + 'px)';
                    card.style.transition = 'transform .07s ease';
                }, index * 60);
            });
        });
    }

    function initFileInput(input) {
        if (input.dataset.cpFileReady === 'true') return;
        input.dataset.cpFileReady = 'true';

        var targetId = input.getAttribute('data-cp-file-target');
        var target = targetId ? document.getElementById(targetId) : null;
        if (!target) return;

        if (!target.getAttribute('data-cp-file-default')) {
            target.setAttribute('data-cp-file-default', target.textContent);
        }

        function updateLabel() {
            var fileName = input.files && input.files.length ? input.files[0].name : '';
            target.textContent = fileName || target.getAttribute('data-cp-file-default') || '';
        }

        input.addEventListener('change', updateLabel);
        updateLabel();
    }

    function closeAlert(alert) {
        if (!alert || alert.dataset.cpAlertClosing === 'true') return;
        alert.dataset.cpAlertClosing = 'true';
        alert.style.opacity = '0';
        alert.style.transform = 'translateY(-6px)';
        alert.style.transition = 'opacity .2s ease, transform .2s ease';
        setTimeout(function () {
            if (alert.parentNode) {
                alert.parentNode.removeChild(alert);
            }
        }, 220);
    }

    function initInlineAlerts() {
        $$('[data-cp-alert]').forEach(function (alert) {
            var closeBtn = alert.querySelector('[data-cp-alert-close]');
            if (closeBtn) {
                closeBtn.addEventListener('click', function () { closeAlert(alert); });
            }

            var delay = Number(alert.getAttribute('data-cp-alert-delay') || 4200);
            if (delay > 0) {
                setTimeout(function () { closeAlert(alert); }, delay);
            }
        });
    }

    function initEnrollmentTabs(wrapper) {
        if (wrapper.dataset.cpTabsReady === 'true') return;
        wrapper.dataset.cpTabsReady = 'true';

        var buttons = $$('.enrollment-tab-btn', wrapper);
        var panes = $$('.enrollment-tab-pane', wrapper);
        if (!buttons.length || !panes.length) return;

        function activate(button) {
            var targetId = button.getAttribute('data-tab');
            var targetPane = targetId ? document.getElementById(targetId) : null;
            if (!targetPane) return;

            buttons.forEach(function (btn) {
                var isActive = btn === button;
                btn.classList.toggle('active', isActive);
                btn.setAttribute('aria-selected', isActive ? 'true' : 'false');
            });

            panes.forEach(function (pane) {
                var isActive = pane === targetPane;
                pane.classList.toggle('active', isActive);
                pane.hidden = !isActive;
            });
        }

        buttons.forEach(function (button) {
            button.addEventListener('click', function () { activate(button); });
        });

        activate(wrapper.querySelector('.enrollment-tab-btn.active') || buttons[0]);
    }

    function initCodeCopy() {
        $$('.cp-code').forEach(function (chip) {
            chip.style.cursor = 'pointer';
            chip.title = 'Click để sao chép';

            chip.addEventListener('click', function () {
                if (!navigator.clipboard) return;

                var text = chip.textContent.trim();
                navigator.clipboard.writeText(text).then(function () {
                    var original = chip.textContent;
                    chip.textContent = 'Đã sao chép';
                    chip.style.background = 'linear-gradient(135deg, rgba(34,197,94,.14) 0%, rgba(16,185,129,.1) 100%)';
                    chip.style.color = '#15803d';

                    setTimeout(function () {
                        chip.textContent = original;
                        chip.style.background = '';
                        chip.style.color = '';
                    }, 1400);
                });
            });
        });
    }

    function initTitles() {
        $$('.cp-name').forEach(function (el) {
            if (el.scrollWidth > el.clientWidth) {
                el.title = el.textContent.trim();
            }
        });
    }

    function normalizeText(text) {
        return (text || '').replace(/\s+/g, ' ').trim().toLowerCase();
    }

    function decorateAddButtons() {
        $$('.cp-btn').forEach(function (btn) {
            var text = (btn.textContent || '').trim();
            if (/^[+＋]/.test(text)) {
                btn.classList.add('cp-btn--add');
            }
        });
    }

    function buildEditIcon() {
        return '' +
            '<svg class="cp-edit-svg" viewBox="0 0 512 512" aria-hidden="true">' +
            '<path d="M410.3 231l11.3-11.3-33.9-33.9-62.1-62.1L291.7 89.8l-11.3 11.3-22.6 22.6L58.6 322.9c-10.4 10.4-18 23.3-22.2 37.4L1 480.7c-2.5 8.4-.2 17.5 6.1 23.7s15.3 8.5 23.7 6.1l120.3-35.4c14.1-4.2 27-11.8 37.4-22.2L387.7 253.7 410.3 231zM160 399.4l-9.1 22.7c-4 3.1-8.5 5.4-13.3 6.9L59.4 452l23-78.1c1.4-4.9 3.8-9.4 6.9-13.3l22.7-9.1v32c0 8.8 7.2 16 16 16h32zM362.7 18.7L348.3 33.2 325.7 55.8 314.3 67.1l33.9 33.9 62.1 62.1 33.9 33.9 11.3-11.3 22.6-22.6 14.5-14.5c25-25 25-65.5 0-90.5L453.3 18.7c-25-25-65.5-25-90.5 0zm-47.4 168l-144 144c-6.2 6.2-16.4 6.2-22.6 0s-6.2-16.4 0-22.6l144-144c6.2-6.2 16.4-6.2 22.6 0s6.2 16.4 0 22.6z"></path>' +
            '</svg>';
    }

    function buildDeleteIcon(maskId) {
        return '' +
            '<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 39 7" class="cp-bin-top" aria-hidden="true">' +
            '<line stroke-width="4" stroke="white" y2="5" x2="39" y1="5"></line>' +
            '<line stroke-width="3" stroke="white" y2="1.5" x2="26.0357" y1="1.5" x1="12"></line>' +
            '</svg>' +
            '<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 33 39" class="cp-bin-bottom" aria-hidden="true">' +
            '<mask fill="white" id="' + maskId + '">' +
            '<path d="M0 0H33V35C33 37.2091 31.2091 39 29 39H4C1.79086 39 0 37.2091 0 35V0Z"></path>' +
            '</mask>' +
            '<path mask="url(#' + maskId + ')" fill="white" d="M0 0H33H0ZM37 35C37 39.4183 33.4183 43 29 43H4C-0.418278 43 -4 39.4183 -4 35H4H29H37ZM4 43C-0.418278 43 -4 39.4183 -4 35V0H4V35V43ZM37 0V35C37 39.4183 33.4183 43 29 43V35V0H37Z"></path>' +
            '<path stroke-width="4" stroke="white" d="M12 6L12 29"></path>' +
            '<path stroke-width="4" stroke="white" d="M21 6V29"></path>' +
            '</svg>' +
            '<svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 89 80" class="cp-garbage" aria-hidden="true">' +
            '<path fill="white" d="M20.5 10.5L37.5 15.5L42.5 11.5L51.5 12.5L68.75 0L72 11.5L79.5 12.5H88.5L87 22L68.75 31.5L75.5066 25L86 26L87 35.5L77.5 48L70.5 49.5L80 50L77.5 71.5L63.5 58.5L53.5 68.5L65.5 70.5L45.5 73L35.5 79.5L28 67L16 63L12 51.5L0 48L16 25L22.5 17L20.5 10.5Z"></path>' +
            '</svg>';
    }

    function decorateActionButtons() {
        var deleteCounter = 0;

        $$('.cp-actions .cp-btn').forEach(function (btn) {
            if (btn.dataset.cpActionStyled === 'true') return;

            var text = normalizeText(btn.textContent);
            if (text !== 'sửa' && text !== 'xóa') return;

            btn.dataset.cpActionStyled = 'true';
            btn.classList.remove('cp-btn', 'cp-btn--primary', 'cp-btn--ghost', 'cp-btn--info', 'cp-btn--warning', 'cp-btn--danger', 'cp-btn--sm', 'cp-btn--add');
            btn.classList.add('cp-icon-action', text === 'sửa' ? 'cp-action-edit' : 'cp-action-delete');
            btn.setAttribute('data-label', text === 'sửa' ? 'Sửa' : 'Xóa');
            btn.setAttribute('title', text === 'sửa' ? 'Sửa' : 'Xóa');
            btn.setAttribute('aria-label', text === 'sửa' ? 'Sửa' : 'Xóa');

            if (text === 'sửa') {
                btn.innerHTML = buildEditIcon();
                return;
            }

            deleteCounter += 1;
            btn.innerHTML = buildDeleteIcon('cp-delete-mask-' + deleteCounter);
        });
    }

    function init() {
        if (!document.body.classList.contains('db-body-hocvu')) return;

        initReveal();
        $$('form[data-admin-form], #courseForm, #classroomForm, #studentForm, #enrollmentForm').forEach(initAdminForm);
        $$('form[data-delete-form], #deleteForm').forEach(initDeleteForm);
        $$('[data-cp-file-input]').forEach(initFileInput);
        $$('[data-enrollment-tabs]').forEach(initEnrollmentTabs);
        initInlineAlerts();
        decorateAddButtons();
        decorateActionButtons();
        initCodeCopy();
        initTitles();
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init, { once: true });
    } else {
        init();
    }
})();

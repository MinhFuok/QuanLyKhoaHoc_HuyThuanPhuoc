(function (window, document) {
    'use strict';

    if (window.AdminCore) {
        return;
    }

    function q(selector, context) {
        return (context || document).querySelector(selector);
    }

    function qq(selector, context) {
        return Array.from((context || document).querySelectorAll(selector));
    }

    function onReady(callback) {
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', callback, { once: true });
            return;
        }

        callback();
    }

    function normalizeText(text) {
        return (text || '').replace(/\s+/g, ' ').trim().toLowerCase();
    }

    function once(element, key) {
        if (!element) {
            return false;
        }

        if (element.dataset[key] === 'true') {
            return false;
        }

        element.dataset[key] = 'true';
        return true;
    }

    function createRevealObserver(options) {
        if (!('IntersectionObserver' in window)) {
            return null;
        }

        return new IntersectionObserver(function (entries, observer) {
            entries.forEach(function (entry) {
                if (!entry.isIntersecting) {
                    return;
                }

                entry.target.classList.add('is-visible');
                observer.unobserve(entry.target);
            });
        }, options || { threshold: 0.1, rootMargin: '0px 0px -40px 0px' });
    }

    function revealElements(targets, revealClass, baseDelay) {
        if (!targets || !targets.length) {
            return;
        }

        var observer = createRevealObserver();

        targets.forEach(function (element, index) {
            element.classList.add(revealClass);

            if (observer) {
                element.style.transitionDelay = Math.min(index * (baseDelay || 0.04), 0.3) + 's';
                observer.observe(element);
                return;
            }

            element.classList.add('is-visible');
        });
    }

    function parseDataset(elementId, mapping) {
        var element = document.getElementById(elementId);
        var result = {};

        if (!element) {
            return result;
        }

        Object.keys(mapping).forEach(function (key) {
            var config = mapping[key];
            var raw = element.dataset[config.name];

            if (config.type === 'number') {
                result[key] = Number(raw || 0);
                return;
            }

            result[key] = raw || '';
        });

        return result;
    }

    function initSidebar() {
        var sidebar = document.getElementById('dbSidebar');
        var overlay = document.getElementById('dbOverlay');
        var toggle = document.getElementById('dbMenuToggle');
        var closeButton = document.getElementById('dbSidebarClose');

        if (!sidebar || !overlay) {
            return;
        }

        function openSidebar() {
            sidebar.classList.add('open');
            overlay.classList.add('active');
            document.body.style.overflow = 'hidden';

            if (toggle) {
                toggle.setAttribute('aria-expanded', 'true');
            }
        }

        function closeSidebar() {
            sidebar.classList.remove('open');
            overlay.classList.remove('active');
            document.body.style.overflow = '';

            if (toggle) {
                toggle.setAttribute('aria-expanded', 'false');
            }
        }

        if (toggle && once(toggle, 'adminCoreBound')) {
            toggle.addEventListener('click', openSidebar);
        }

        if (closeButton && once(closeButton, 'adminCoreBound')) {
            closeButton.addEventListener('click', closeSidebar);
        }

        if (once(overlay, 'adminCoreBound')) {
            overlay.addEventListener('click', closeSidebar);
        }

        if (once(document.body, 'adminCoreSidebarDelegation')) {
            document.addEventListener('click', function (event) {
                var navLink = event.target.closest('#dbNav .db-nav-link');
                if (!navLink || window.innerWidth >= 992) {
                    return;
                }

                closeSidebar();
            });

            document.addEventListener('keydown', function (event) {
                if (event.key === 'Escape') {
                    closeSidebar();
                }
            });

            window.addEventListener('resize', function () {
                if (window.innerWidth >= 992) {
                    closeSidebar();
                }
            });
        }
    }

    function initActiveNav() {
        var path = window.location.pathname.replace(/\/+$/, '') || '/';

        qq('#dbNav .db-nav-link').forEach(function (link) {
            var href = link.getAttribute('href');

            if (!href || href === '#' || href.indexOf('javascript:') === 0) {
                return;
            }

            var normalizedHref = href.replace(/\/+$/, '') || '/';
            link.classList.toggle('active', normalizedHref.toLowerCase() === path.toLowerCase());
        });
    }

    function dismissAlert(alert) {
        if (!alert || alert.dataset.closing === 'true') {
            return;
        }

        alert.dataset.closing = 'true';
        alert.classList.add('hiding');

        window.setTimeout(function () {
            if (alert.parentNode) {
                alert.parentNode.removeChild(alert);
            }
        }, 220);
    }

    function initAlerts() {
        qq('#dbAlertZone .db-alert').forEach(function (alert) {
            if (!once(alert, 'adminCoreAlertReady')) {
                return;
            }

            var closeButton = q('.db-alert-close', alert);

            if (closeButton) {
                closeButton.addEventListener('click', function () {
                    dismissAlert(alert);
                });
            }

            window.setTimeout(function () {
                dismissAlert(alert);
            }, 5000);
        });
    }

    function initConfirmButtons() {
        qq('.btn, .cp-btn').forEach(function (button) {
            var label = normalizeText(button.textContent);

            if (!label.startsWith('xác nhận')) {
                return;
            }

            button.classList.add('confirm-glow');
        });
    }

    function initLayout() {
        initSidebar();
        initActiveNav();
        initAlerts();
        initConfirmButtons();
    }

    window.AdminCore = {
        q: q,
        qq: qq,
        once: once,
        onReady: onReady,
        normalizeText: normalizeText,
        revealElements: revealElements,
        parseDataset: parseDataset,
        dismissAlert: dismissAlert,
        initLayout: initLayout
    };

    onReady(initLayout);
})(window, document);

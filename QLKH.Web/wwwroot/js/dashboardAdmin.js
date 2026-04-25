/**
 * dashboardAdmin.js
 * Corporate Edu Dashboard – plain JS, no Razor, Chart.js only.
 * Data injected via window.adminDashboardData from Index.cshtml.
 */
(function () {
    'use strict';

    /* ── Palette (mirrors CSS vars) ────────────────────────────── */
    var C = {
        blue: '#1a56db',
        blueMid: '#2563eb',
        blueLight: '#3b82f6',
        bluePale: 'rgba(26,86,219,.12)',
        cyan: '#06b6d4',
        cyanLight: '#22d3ee',
        cyanPale: 'rgba(6,182,212,.12)',
        navy: '#12275a',
        navyPale: 'rgba(18,39,90,.10)',
        slate: '#334155',
        slatePale: 'rgba(51,65,85,.10)',
        success: '#10b981',
        danger: '#ef4444',
        muted: '#94a3b8',
        grid: 'rgba(0,0,0,.05)',
        white: '#ffffff',
    };

    /* ── Helpers ────────────────────────────────────────────────── */
    function $$(sel, ctx) { return Array.from((ctx || document).querySelectorAll(sel)); }
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

    /* ── 0. Space background ───────────────────────────────────── */
    function initSpaceBackground() {
        var page = document.querySelector('.admin-dashboard-page');
        var host = document.querySelector('.db-main-wrap') || page;
        if (!page || !host || host.querySelector('.da-space-bg')) return;

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
        document.body.classList.add('da-space-body');

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

    /* ── 1. Reveal on scroll ────────────────────────────────────── */
    function initReveal() {
        var selectors = [
            '.da-header',
            '.da-kpi',
            '.da-card',
            '.da-maintenance-alert',
        ];
        var all = [];
        selectors.forEach(function (sel) {
            $$('.admin-dashboard-page ' + sel).forEach(function (el) {
                el.classList.add('da-reveal');
                all.push(el);
            });
        });

        if (!('IntersectionObserver' in window)) {
            all.forEach(function (el) { el.classList.add('is-visible'); });
            return;
        }

        var io = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                if (entry.isIntersecting) {
                    entry.target.classList.add('is-visible');
                    io.unobserve(entry.target);
                }
            });
        }, { threshold: 0.06, rootMargin: '0px 0px -30px 0px' });

        all.forEach(function (el, i) {
            el.style.transitionDelay = Math.min(i * 0.05, 0.6) + 's';
            io.observe(el);
        });
    }

    /* ── 2. Count-up for .stat-value ────────────────────────────── */
    function countUp(el, target, duration) {
        var num = parseInt(target, 10);
        if (isNaN(num)) { el.textContent = target; return; }
        var startTime = null;
        function step(ts) {
            if (!startTime) startTime = ts;
            var p = Math.min((ts - startTime) / duration, 1);
            var eased = 1 - Math.pow(1 - p, 3);
            el.textContent = Math.round(eased * num).toLocaleString('vi-VN');
            if (p < 1) requestAnimationFrame(step);
        }
        el.textContent = '0';
        requestAnimationFrame(step);
    }

    function initCountUps() {
        $$('.admin-dashboard-page .stat-value').forEach(function (el) {
            var raw = el.textContent.trim().replace(/\D/g, '');
            if (!raw) return;
            setTimeout(function () { countUp(el, raw, 950); }, 300);
        });
    }

    /* ── 3. Chart.js global defaults ───────────────────────────── */
    function applyChartDefaults() {
        if (typeof Chart === 'undefined') return;
        Chart.defaults.font.family = "'Segoe UI', system-ui, sans-serif";
        Chart.defaults.font.size = 12;
        Chart.defaults.color = '#64748b';
    }

    function darkTooltip(extra) {
        return Object.assign({
            backgroundColor: '#0d1b3e',
            padding: 10,
            cornerRadius: 8,
            titleColor: '#ffffff',
            bodyColor: 'rgba(255,255,255,.82)',
            borderColor: 'rgba(255,255,255,.08)',
            borderWidth: 1,
            displayColors: true,
            boxPadding: 4,
        }, extra || {});
    }

    /* ── 3a. Overview Bar ───────────────────────────────────────── */
    function initOverviewChart(data) {
        var el = document.getElementById('overviewBarChart');
        if (!el || typeof Chart === 'undefined') return;

        var colors = [C.blue, C.cyan, C.success, C.blueLight, C.navy];
        var bgs = colors.map(function (c) { return c + 'cc'; });

        new Chart(el, {
            type: 'bar',
            data: {
                labels: ['Học viên', 'Giáo viên', 'Khóa học', 'Lớp học', 'Khoa/Ngành'],
                datasets: [{
                    label: 'Số lượng',
                    data: [
                        data.TotalStudents || 0,
                        data.TotalTeachers || 0,
                        data.TotalCourses || 0,
                        data.TotalClassRooms || 0,
                        data.TotalDepartments || 0,
                    ],
                    backgroundColor: bgs,
                    borderColor: colors,
                    borderWidth: 2,
                    borderRadius: 10,
                    borderSkipped: false,
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: true,
                animation: { duration: 900, easing: 'easeOutQuart' },
                plugins: {
                    legend: { display: false },
                    tooltip: Object.assign(darkTooltip(), {
                        callbacks: {
                            label: function (ctx) {
                                return '  ' + ctx.dataset.label + ': '
                                    + ctx.parsed.y.toLocaleString('vi-VN');
                            }
                        }
                    }),
                },
                scales: {
                    x: {
                        grid: { display: false },
                        ticks: { font: { weight: '600' } }
                    },
                    y: {
                        beginAtZero: true,
                        grid: { color: C.grid, drawBorder: false },
                        ticks: {
                            precision: 0,
                            callback: function (v) { return v.toLocaleString('vi-VN'); }
                        }
                    }
                }
            }
        });
    }

    /* ── 3b. User Status Doughnut ───────────────────────────────── */
    function initUserStatusChart(data) {
        var el = document.getElementById('userStatusChart');
        if (!el || typeof Chart === 'undefined') return;

        new Chart(el, {
            type: 'doughnut',
            data: {
                labels: ['Đang hoạt động', 'Đã khóa'],
                datasets: [{
                    data: [data.ActiveUsers || 0, data.LockedUsers || 0],
                    backgroundColor: [C.success + 'dd', C.danger + 'dd'],
                    borderColor: [C.success, C.danger],
                    borderWidth: 2,
                    hoverOffset: 8,
                }]
            },
            options: {
                responsive: true,
                cutout: '70%',
                animation: { duration: 950, easing: 'easeOutQuart' },
                plugins: {
                    legend: {
                        position: 'bottom',
                        labels: {
                            padding: 18,
                            usePointStyle: true,
                            pointStyleWidth: 10,
                            font: { weight: '600', size: 12 }
                        }
                    },
                    tooltip: Object.assign(darkTooltip(), {
                        callbacks: {
                            label: function (ctx) {
                                var total = ctx.dataset.data.reduce(function (a, b) { return a + b; }, 0);
                                var pct = total ? Math.round(ctx.parsed / total * 100) : 0;
                                return '  ' + ctx.label + ': '
                                    + ctx.parsed.toLocaleString('vi-VN')
                                    + ' (' + pct + '%)';
                            }
                        }
                    }),
                }
            }
        });
    }

    /* ── 3c. Link Status Horizontal Bar ────────────────────────── */
    function initLinkStatusChart(data) {
        var el = document.getElementById('linkStatusChart');
        if (!el || typeof Chart === 'undefined') return;

        new Chart(el, {
            type: 'bar',
            data: {
                labels: ['Học viên', 'Giáo viên'],
                datasets: [
                    {
                        label: 'Đã liên kết',
                        data: [data.LinkedStudents || 0, data.LinkedTeachers || 0],
                        backgroundColor: C.blue + 'cc',
                        borderColor: C.blue,
                        borderWidth: 2,
                        borderRadius: 8,
                        borderSkipped: false,
                    },
                    {
                        label: 'Chưa liên kết',
                        data: [data.UnlinkedStudents || 0, data.UnlinkedTeachers || 0],
                        backgroundColor: C.muted + '55',
                        borderColor: C.muted,
                        borderWidth: 2,
                        borderRadius: 8,
                        borderSkipped: false,
                    }
                ]
            },
            options: {
                indexAxis: 'y',
                responsive: true,
                animation: { duration: 900, easing: 'easeOutQuart' },
                plugins: {
                    legend: {
                        position: 'bottom',
                        labels: {
                            padding: 18,
                            usePointStyle: true,
                            pointStyleWidth: 10,
                            font: { weight: '600', size: 12 }
                        }
                    },
                    tooltip: Object.assign(darkTooltip(), {
                        callbacks: {
                            label: function (ctx) {
                                return '  ' + ctx.dataset.label + ': '
                                    + ctx.parsed.x.toLocaleString('vi-VN');
                            }
                        }
                    }),
                },
                scales: {
                    x: {
                        beginAtZero: true,
                        grid: { color: C.grid, drawBorder: false },
                        ticks: {
                            precision: 0,
                            callback: function (v) { return v.toLocaleString('vi-VN'); }
                        }
                    },
                    y: {
                        grid: { display: false },
                        ticks: { font: { weight: '600' } }
                    }
                }
            }
        });
    }

    /* ── 4. Quick action ripple ─────────────────────────────────── */
    function initRipple() {
        $$('.admin-dashboard-page .da-action-item').forEach(function (el) {
            el.addEventListener('click', function (e) {
                var rect = el.getBoundingClientRect();
                var ripple = document.createElement('span');
                ripple.style.cssText = [
                    'position:absolute',
                    'border-radius:50%',
                    'background:rgba(255,255,255,.3)',
                    'width:6px', 'height:6px',
                    'pointer-events:none',
                    'transform:scale(0)',
                    'transition:transform .55s ease,opacity .55s ease',
                    'left:' + (e.clientX - rect.left - 3) + 'px',
                    'top:' + (e.clientY - rect.top - 3) + 'px',
                ].join(';');
                el.appendChild(ripple);
                requestAnimationFrame(function () {
                    ripple.style.transform = 'scale(40)';
                    ripple.style.opacity = '0';
                });
                setTimeout(function () { ripple.remove(); }, 650);
            });
        });
    }

    /* ── Boot ───────────────────────────────────────────────────── */
    function init() {
        var data = window.adminDashboardData || {};
        initSpaceBackground();
        initReveal();
        initCountUps();
        applyChartDefaults();
        initOverviewChart(data);
        initUserStatusChart(data);
        initLinkStatusChart(data);
        initRipple();
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }

})();

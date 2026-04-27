/**
 * dashboardAdmin.js
 * Corporate Edu Dashboard – plain JS, no Razor, Chart.js only.
 * Data injected via window.adminDashboardData from Index.cshtml.
 */
(function () {
    'use strict';

    /* ── Palette (mirrors CSS vars) ────────────────────────────── */
    var C = {
        blue: '#38bdf8',
        blueMid: '#60a5fa',
        blueLight: '#7dd3fc',
        bluePale: 'rgba(56,189,248,.18)',
        cyan: '#22d3ee',
        cyanLight: '#67e8f9',
        cyanPale: 'rgba(34,211,238,.18)',
        navy: '#a78bfa',
        navyPale: 'rgba(167,139,250,.16)',
        slate: '#94a3b8',
        slatePale: 'rgba(148,163,184,.16)',
        teal: '#0ea5e9',
        amber: '#fbbf24',
        panel: '#0f1b31',
        success: '#34d399',
        danger: '#f87171',
        muted: 'rgba(203,218,235,.72)',
        grid: 'rgba(255,255,255,.08)',
        gridSoft: 'rgba(255,255,255,.04)',
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

    function createVerticalGradient(chart, stops) {
        var ctx = chart.ctx;
        var area = chart.chartArea;
        if (!area) return stops[0][1];

        var gradient = ctx.createLinearGradient(0, area.top, 0, area.bottom);
        stops.forEach(function (stop) {
            gradient.addColorStop(stop[0], stop[1]);
        });
        return gradient;
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

    /* ── 2a. Liquid glass pointer light ───────────────────────── */
    function initLiquidGlassPointer() {
        if (!window.matchMedia || !window.matchMedia('(pointer: fine)').matches) return;

        var targets = $$('.admin-dashboard-page .da-kpi, .admin-dashboard-page .da-card, .admin-dashboard-page .da-identity, .admin-dashboard-page .da-maintenance-alert, .admin-dashboard-page .da-action-item');

        targets.forEach(function (el) {
            var rafId = 0;
            var lastX = 0;
            var lastY = 0;
            var alpha = el.classList.contains('da-action-item')
                ? 0.18
                : el.classList.contains('da-maintenance-alert')
                    ? 0.16
                    : 0.24;

            function commit() {
                rafId = 0;
                var rect = el.getBoundingClientRect();
                if (!rect.width || !rect.height) return;

                var x = ((lastX - rect.left) / rect.width) * 100;
                var y = ((lastY - rect.top) / rect.height) * 100;

                el.style.setProperty('--da-light-x', x.toFixed(2) + '%');
                el.style.setProperty('--da-light-y', y.toFixed(2) + '%');
            }

            function queue(clientX, clientY) {
                lastX = clientX;
                lastY = clientY;
                if (rafId) return;
                rafId = requestAnimationFrame(commit);
            }

            el.addEventListener('pointerenter', function (e) {
                if (e.pointerType && e.pointerType !== 'mouse' && e.pointerType !== 'pen') return;
                el.style.setProperty('--da-light-alpha', String(alpha));
                queue(e.clientX, e.clientY);
            });

            el.addEventListener('pointermove', function (e) {
                if (e.pointerType && e.pointerType !== 'mouse' && e.pointerType !== 'pen') return;
                el.style.setProperty('--da-light-alpha', String(alpha));
                queue(e.clientX, e.clientY);
            });

            el.addEventListener('pointerleave', function () {
                if (rafId) {
                    cancelAnimationFrame(rafId);
                    rafId = 0;
                }

                el.style.setProperty('--da-light-alpha', '0');
                el.style.setProperty('--da-light-x', '50%');
                el.style.setProperty('--da-light-y', '50%');
            });
        });
    }

    /* ── 3. Chart.js global defaults ───────────────────────────── */
    function applyChartDefaults() {
        if (typeof Chart === 'undefined') return;
        Chart.defaults.font.family = "'Roboto', 'Segoe UI', Arial, sans-serif";
        Chart.defaults.font.size = 12;
        Chart.defaults.color = C.muted;
        Chart.defaults.borderColor = C.gridSoft;

        if (!Chart._daPluginsRegistered) {
            Chart.register({
                id: 'daNeonLineGlow',
                beforeDatasetDraw: function (chart, args) {
                    if (chart.config.type !== 'line') return;
                    var dataset = chart.data.datasets[args.index];
                    if (!dataset || dataset.hidden) return;

                    var ctx = chart.ctx;
                    ctx.save();
                    ctx.shadowBlur = dataset.shadowBlur || 18;
                    ctx.shadowColor = dataset.shadowColor || dataset.borderColor || 'rgba(255,255,255,.24)';
                    ctx.lineJoin = 'round';
                    ctx.lineCap = 'round';
                },
                afterDatasetDraw: function (chart) {
                    if (chart.config.type !== 'line') return;
                    chart.ctx.restore();
                }
            }, {
                id: 'daCenterText',
                afterDraw: function (chart, args, pluginOptions) {
                    if (chart.config.type !== 'doughnut' || !pluginOptions || !pluginOptions.lines || !pluginOptions.lines.length) return;
                    var meta = chart.getDatasetMeta(0);
                    if (!meta || !meta.data || !meta.data.length) return;

                    var ctx = chart.ctx;
                    var centerX = meta.data[0].x;
                    var centerY = meta.data[0].y;
                    var lineHeight = pluginOptions.lineHeight || 18;
                    var totalHeight = pluginOptions.lines.length * lineHeight;
                    var startY = centerY - totalHeight / 2 + lineHeight / 2;

                    ctx.save();
                    ctx.textAlign = 'center';
                    ctx.textBaseline = 'middle';

                    pluginOptions.lines.forEach(function (line, index) {
                        ctx.font = line.font || "700 24px 'Roboto', 'Segoe UI', Arial, sans-serif";
                        ctx.fillStyle = line.color || '#ffffff';
                        ctx.fillText(line.text, centerX, startY + index * lineHeight);
                    });

                    ctx.restore();
                }
            });

            Chart._daPluginsRegistered = true;
        }
    }

    function darkTooltip(extra) {
        return Object.assign({
            backgroundColor: 'rgba(10,13,29,.94)',
            padding: 12,
            cornerRadius: 12,
            titleColor: '#ffffff',
            bodyColor: 'rgba(226,232,240,.92)',
            borderColor: 'rgba(103,198,243,.22)',
            borderWidth: 1,
            displayColors: true,
            boxPadding: 4,
        }, extra || {});
    }

    /* ── 3a. Overview Bar ───────────────────────────────────────── */
    function initOverviewChart(data) {
        var el = document.getElementById('overviewBarChart');
        if (!el || typeof Chart === 'undefined') return;

        new Chart(el, {
            type: 'line',
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
                    borderColor: C.blue,
                    backgroundColor: function (context) {
                        return createVerticalGradient(context.chart, [
                            [0, 'rgba(56,189,248,.4)'],
                            [0.45, 'rgba(96,165,250,.18)'],
                            [1, 'rgba(15,23,42,0)']
                        ]);
                    },
                    borderWidth: 3,
                    fill: true,
                    tension: 0.42,
                    pointRadius: 3,
                    pointHoverRadius: 5,
                    pointBorderWidth: 0,
                    pointBackgroundColor: '#e0f2fe',
                    pointHoverBackgroundColor: '#ffffff',
                    shadowBlur: 24,
                    shadowColor: 'rgba(56,189,248,.32)',
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                animation: { duration: 1100, easing: 'easeOutQuart' },
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
                        ticks: {
                            color: 'rgba(203,218,235,.74)',
                            font: { weight: '600' }
                        },
                        border: { display: false }
                    },
                    y: {
                        beginAtZero: true,
                        grid: { color: C.grid, drawBorder: false },
                        border: { display: false },
                        ticks: {
                            precision: 0,
                            color: 'rgba(203,218,235,.58)',
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
                    backgroundColor: [C.success, C.amber],
                    borderColor: ['rgba(12,16,34,.94)', 'rgba(12,16,34,.94)'],
                    borderWidth: 4,
                    spacing: 2,
                    hoverOffset: 6,
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                cutout: '66%',
                animation: { duration: 950, easing: 'easeOutQuart' },
                plugins: {
                    legend: { display: false },
                    daCenterText: {
                        lineHeight: 20,
                        lines: [
                            {
                                text: (data.ActiveUsers || 0).toLocaleString('vi-VN'),
                                font: "800 28px 'Roboto', 'Segoe UI', Arial, sans-serif",
                                color: '#f8fafc'
                            },
                            {
                                text: 'hoạt động',
                                font: "600 12px 'Roboto', 'Segoe UI', Arial, sans-serif",
                                color: 'rgba(203,218,235,.68)'
                            }
                        ]
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
            type: 'line',
            data: {
                labels: ['Học viên', 'Giáo viên'],
                datasets: [
                    {
                        label: 'Đã liên kết',
                        data: [data.LinkedStudents || 0, data.LinkedTeachers || 0],
                        borderColor: C.cyan,
                        backgroundColor: function (context) {
                            return createVerticalGradient(context.chart, [
                                [0, 'rgba(34,211,238,.28)'],
                                [1, 'rgba(34,211,238,0)']
                            ]);
                        },
                        borderWidth: 3,
                        fill: true,
                        tension: 0.45,
                        pointRadius: 4,
                        pointHoverRadius: 6,
                        pointBorderWidth: 0,
                        pointBackgroundColor: '#cffafe',
                        shadowBlur: 22,
                        shadowColor: 'rgba(34,211,238,.28)',
                    },
                    {
                        label: 'Chưa liên kết',
                        data: [data.UnlinkedStudents || 0, data.UnlinkedTeachers || 0],
                        borderColor: C.navy,
                        backgroundColor: function (context) {
                            return createVerticalGradient(context.chart, [
                                [0, 'rgba(167,139,250,.18)'],
                                [1, 'rgba(167,139,250,0)']
                            ]);
                        },
                        borderWidth: 3,
                        fill: true,
                        tension: 0.45,
                        pointRadius: 4,
                        pointHoverRadius: 6,
                        pointBorderWidth: 0,
                        pointBackgroundColor: '#ede9fe',
                        shadowBlur: 20,
                        shadowColor: 'rgba(167,139,250,.22)',
                    }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                animation: { duration: 950, easing: 'easeOutQuart' },
                plugins: {
                    legend: {
                        position: 'bottom',
                        labels: {
                            color: 'rgba(203,218,235,.76)',
                            padding: 14,
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
                        grid: { display: false },
                        border: { display: false },
                        ticks: {
                            color: 'rgba(203,218,235,.74)',
                            font: { weight: '600' }
                        }
                    },
                    y: {
                        beginAtZero: true,
                        grid: { color: C.grid, drawBorder: false },
                        border: { display: false },
                        ticks: {
                            precision: 0,
                            color: 'rgba(203,218,235,.58)',
                            callback: function (v) { return v.toLocaleString('vi-VN'); }
                        }
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
        initLiquidGlassPointer();
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

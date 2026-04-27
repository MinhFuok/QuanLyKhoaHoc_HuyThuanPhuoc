(function () {
    'use strict';

    var C = {
        purple: '#5b45f2',
        blue: '#2388ff',
        cyan: '#18b8f2',
        teal: '#10b981',
        orange: '#fb923c',
        pink: '#f43f5e',
        text: '#0f172a',
        muted: '#64748b',
        grid: 'rgba(148,163,184,0.18)',
        white: '#ffffff'
    };

    function $$(selector, context) {
        return Array.from((context || document).querySelectorAll(selector));
    }

    function createVerticalGradient(chart, stops) {
        var area = chart.chartArea;
        if (!area) return stops[0][1];

        var gradient = chart.ctx.createLinearGradient(0, area.top, 0, area.bottom);
        stops.forEach(function (stop) {
            gradient.addColorStop(stop[0], stop[1]);
        });

        return gradient;
    }

    function initReveal() {
        var targets = [];
        ['.da-header', '.da-kpi', '.da-card', '.da-maintenance-alert'].forEach(function (selector) {
            $$('.admin-dashboard-page ' + selector).forEach(function (el) {
                el.classList.add('da-reveal');
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
            el.style.transitionDelay = Math.min(index * 0.05, 0.32) + 's';
            observer.observe(el);
        });
    }

    function countUp(el, target, duration) {
        var num = parseInt(target, 10);
        if (isNaN(num)) {
            el.textContent = target;
            return;
        }

        var startTime = null;

        function step(timestamp) {
            if (!startTime) startTime = timestamp;
            var progress = Math.min((timestamp - startTime) / duration, 1);
            var eased = 1 - Math.pow(1 - progress, 3);
            el.textContent = Math.round(eased * num).toLocaleString('vi-VN');
            if (progress < 1) requestAnimationFrame(step);
        }

        el.textContent = '0';
        requestAnimationFrame(step);
    }

    function initCountUps() {
        $$('.admin-dashboard-page .stat-value').forEach(function (el) {
            var raw = el.textContent.trim().replace(/\D/g, '');
            if (!raw) return;
            setTimeout(function () { countUp(el, raw, 850); }, 180);
        });
    }

    function applyChartDefaults() {
        if (typeof Chart === 'undefined') return;

        Chart.defaults.font.family = "'Roboto', 'Segoe UI', Arial, sans-serif";
        Chart.defaults.font.size = 12;
        Chart.defaults.color = C.muted;
        Chart.defaults.borderColor = C.grid;

        if (Chart._daPluginsRegistered) return;

        Chart.register({
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
                    ctx.font = line.font || "700 22px 'Roboto', sans-serif";
                    ctx.fillStyle = line.color || C.text;
                    ctx.fillText(line.text, centerX, startY + index * lineHeight);
                });

                ctx.restore();
            }
        });

        Chart._daPluginsRegistered = true;
    }

    function tooltip(extra) {
        return Object.assign({
            backgroundColor: 'rgba(15,23,42,0.94)',
            padding: 12,
            cornerRadius: 14,
            titleColor: C.white,
            bodyColor: 'rgba(255,255,255,0.88)',
            displayColors: true,
            boxPadding: 4
        }, extra || {});
    }

    function baseScales() {
        return {
            x: {
                grid: { display: false },
                ticks: { color: C.muted, font: { weight: '700' } },
                border: { display: false }
            },
            y: {
                beginAtZero: true,
                grid: { color: C.grid, drawBorder: false },
                border: { display: false },
                ticks: {
                    precision: 0,
                    color: C.muted,
                    callback: function (value) { return value.toLocaleString('vi-VN'); }
                }
            }
        };
    }

    function initOverviewChart(data) {
        var el = document.getElementById('overviewBarChart');
        if (!el || typeof Chart === 'undefined') return;

        new Chart(el, {
            type: 'bar',
            data: {
                labels: ['H\u1ecdc vi\u00ean', 'Gi\u00e1o vi\u00ean', 'Kh\u00f3a h\u1ecdc', 'L\u1edbp h\u1ecdc', 'Ch\u01b0\u01a1ng tr\u00ecnh'],
                datasets: [{
                    label: 'S\u1ed1 l\u01b0\u1ee3ng',
                    data: [data.TotalStudents || 0, data.TotalTeachers || 0, data.TotalCourses || 0, data.TotalClassRooms || 0, data.TotalDepartments || 0],
                    backgroundColor: function (context) {
                        return createVerticalGradient(context.chart, [
                            [0, '#8b5cf6'],
                            [0.55, '#5b45f2'],
                            [1, '#2388ff']
                        ]);
                    },
                    borderRadius: 16,
                    borderSkipped: false,
                    maxBarThickness: 46
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                animation: { duration: 900, easing: 'easeOutQuart' },
                plugins: {
                    legend: { display: false },
                    tooltip: Object.assign(tooltip(), {
                        callbacks: {
                            label: function (ctx) { return '  ' + ctx.dataset.label + ': ' + ctx.parsed.y.toLocaleString('vi-VN'); }
                        }
                    })
                },
                scales: baseScales()
            }
        });
    }

    function initUserStatusChart(data) {
        var el = document.getElementById('userStatusChart');
        if (!el || typeof Chart === 'undefined') return;

        new Chart(el, {
            type: 'doughnut',
            data: {
                labels: ['\u0110ang ho\u1ea1t \u0111\u1ed9ng', '\u0110\u00e3 kh\u00f3a'],
                datasets: [{
                    data: [data.ActiveUsers || 0, data.LockedUsers || 0],
                    backgroundColor: [C.teal, C.orange],
                    borderColor: [C.white, C.white],
                    borderWidth: 8,
                    spacing: 3,
                    hoverOffset: 5
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                cutout: '70%',
                animation: { duration: 850, easing: 'easeOutQuart' },
                plugins: {
                    legend: {
                        position: 'bottom',
                        labels: {
                            color: C.muted,
                            padding: 18,
                            usePointStyle: true,
                            pointStyle: 'circle',
                            font: { weight: '700', size: 12 }
                        }
                    },
                    daCenterText: {
                        lineHeight: 22,
                        lines: [
                            { text: (data.ActiveUsers || 0).toLocaleString('vi-VN'), font: "900 28px 'Roboto', sans-serif", color: C.text },
                            { text: 'ho\u1ea1t \u0111\u1ed9ng', font: "700 12px 'Roboto', sans-serif", color: C.muted }
                        ]
                    },
                    tooltip: Object.assign(tooltip(), {
                        callbacks: {
                            label: function (ctx) {
                                var total = ctx.dataset.data.reduce(function (a, b) { return a + b; }, 0);
                                var pct = total ? Math.round(ctx.parsed / total * 100) : 0;
                                return '  ' + ctx.label + ': ' + ctx.parsed.toLocaleString('vi-VN') + ' (' + pct + '%)';
                            }
                        }
                    })
                }
            }
        });
    }

    function initLinkStatusChart(data) {
        var el = document.getElementById('linkStatusChart');
        if (!el || typeof Chart === 'undefined') return;

        new Chart(el, {
            type: 'bar',
            data: {
                labels: ['H\u1ecdc vi\u00ean', 'Gi\u00e1o vi\u00ean'],
                datasets: [{
                    label: '\u0110\u00e3 li\u00ean k\u1ebft',
                    data: [data.LinkedStudents || 0, data.LinkedTeachers || 0],
                    backgroundColor: C.blue,
                    borderRadius: 14,
                    borderSkipped: false,
                    maxBarThickness: 42
                }, {
                    label: 'Ch\u01b0a li\u00ean k\u1ebft',
                    data: [data.UnlinkedStudents || 0, data.UnlinkedTeachers || 0],
                    backgroundColor: C.pink,
                    borderRadius: 14,
                    borderSkipped: false,
                    maxBarThickness: 42
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                animation: { duration: 900, easing: 'easeOutQuart' },
                plugins: {
                    legend: {
                        position: 'bottom',
                        labels: {
                            color: C.muted,
                            padding: 16,
                            usePointStyle: true,
                            pointStyle: 'circle',
                            font: { weight: '700', size: 12 }
                        }
                    },
                    tooltip: Object.assign(tooltip(), {
                        callbacks: {
                            label: function (ctx) { return '  ' + ctx.dataset.label + ': ' + ctx.parsed.y.toLocaleString('vi-VN'); }
                        }
                    })
                },
                scales: baseScales()
            }
        });
    }

    function init() {
        var data = window.adminDashboardData || {};
        initReveal();
        initCountUps();
        applyChartDefaults();
        initOverviewChart(data);
        initUserStatusChart(data);
        initLinkStatusChart(data);
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }
})();

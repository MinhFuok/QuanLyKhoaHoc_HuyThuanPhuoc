(function () {
    'use strict';

    var C = {
        violet: '#7c3aed',
        violetSoft: 'rgba(124,58,237,.18)',
        pink: '#db2777',
        pinkSoft: 'rgba(219,39,119,.16)',
        sky: '#0ea5e9',
        skySoft: 'rgba(14,165,233,.18)',
        emerald: '#10b981',
        amber: '#f59e0b',
        danger: '#ef4444',
        text: '#332f3a',
        muted: '#635f69',
        grid: 'rgba(124,58,237,.08)',
        gridSoft: 'rgba(14,165,233,.06)',
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
        }, { threshold: 0.08, rootMargin: '0px 0px -40px 0px' });

        targets.forEach(function (el, index) {
            el.style.transitionDelay = Math.min(index * 0.05, 0.35) + 's';
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
        function step(ts) {
            if (!startTime) startTime = ts;
            var progress = Math.min((ts - startTime) / duration, 1);
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
            setTimeout(function () { countUp(el, raw, 900); }, 250);
        });
    }

    function initGlow() {
        if (!window.matchMedia || !window.matchMedia('(pointer: fine)').matches) return;

        var targets = $$('.admin-dashboard-page .da-header, .admin-dashboard-page .da-kpi, .admin-dashboard-page .da-card, .admin-dashboard-page .da-maintenance-alert, .admin-dashboard-page .da-action-item');
        targets.forEach(function (el) {
            var rafId = 0;
            var nextX = 50;
            var nextY = 50;
            var alpha = el.classList.contains('da-action-item') ? 0.22 : 0.34;

            function commit() {
                rafId = 0;
                el.style.setProperty('--da-light-x', nextX.toFixed(2) + '%');
                el.style.setProperty('--da-light-y', nextY.toFixed(2) + '%');
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
                el.style.setProperty('--da-light-alpha', String(alpha));
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
                el.style.setProperty('--da-light-alpha', '0');
                el.style.setProperty('--da-light-x', '50%');
                el.style.setProperty('--da-light-y', '50%');
            });
        });
    }

    function applyChartDefaults() {
        if (typeof Chart === 'undefined') return;

        Chart.defaults.font.family = "'DM Sans', 'Segoe UI', Arial, sans-serif";
        Chart.defaults.font.size = 12;
        Chart.defaults.color = C.muted;
        Chart.defaults.borderColor = C.gridSoft;

        if (Chart._daPluginsRegistered) return;

        Chart.register({
            id: 'daLineGlow',
            beforeDatasetDraw: function (chart, args) {
                if (chart.config.type !== 'line') return;
                var dataset = chart.data.datasets[args.index];
                if (!dataset || dataset.hidden) return;

                var ctx = chart.ctx;
                ctx.save();
                ctx.shadowBlur = dataset.shadowBlur || 18;
                ctx.shadowColor = dataset.shadowColor || dataset.borderColor || 'rgba(124,58,237,.18)';
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
                    ctx.font = line.font || "700 24px 'Nunito', 'DM Sans', sans-serif";
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
            backgroundColor: 'rgba(255,255,255,.96)',
            padding: 12,
            cornerRadius: 16,
            titleColor: C.text,
            bodyColor: C.muted,
            borderColor: 'rgba(124,58,237,.14)',
            borderWidth: 1,
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
            type: 'line',
            data: {
                labels: ['Học viên', 'Giáo viên', 'Khóa học', 'Lớp học', 'Khoa/Ngành'],
                datasets: [{
                    label: 'Số lượng',
                    data: [data.TotalStudents || 0, data.TotalTeachers || 0, data.TotalCourses || 0, data.TotalClassRooms || 0, data.TotalDepartments || 0],
                    borderColor: C.violet,
                    backgroundColor: function (context) {
                        return createVerticalGradient(context.chart, [
                            [0, 'rgba(124,58,237,.28)'],
                            [0.45, 'rgba(219,39,119,.14)'],
                            [1, 'rgba(255,255,255,0)']
                        ]);
                    },
                    borderWidth: 3,
                    fill: true,
                    tension: 0.42,
                    pointRadius: 4,
                    pointHoverRadius: 6,
                    pointBorderWidth: 0,
                    pointBackgroundColor: '#ffffff',
                    pointHoverBackgroundColor: '#f3e8ff',
                    shadowBlur: 20,
                    shadowColor: 'rgba(124,58,237,.22)'
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                animation: { duration: 1000, easing: 'easeOutQuart' },
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
                labels: ['Đang hoạt động', 'Đã khóa'],
                datasets: [{
                    data: [data.ActiveUsers || 0, data.LockedUsers || 0],
                    backgroundColor: [C.emerald, C.amber],
                    borderColor: ['rgba(255,255,255,.96)', 'rgba(255,255,255,.96)'],
                    borderWidth: 6,
                    spacing: 3,
                    hoverOffset: 6
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                cutout: '68%',
                animation: { duration: 900, easing: 'easeOutQuart' },
                plugins: {
                    legend: { display: false },
                    daCenterText: {
                        lineHeight: 20,
                        lines: [
                            { text: (data.ActiveUsers || 0).toLocaleString('vi-VN'), font: "900 28px 'Nunito', 'DM Sans', sans-serif", color: C.text },
                            { text: 'hoạt động', font: "700 12px 'DM Sans', sans-serif", color: C.muted }
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
            type: 'line',
            data: {
                labels: ['Học viên', 'Giáo viên'],
                datasets: [{
                    label: 'Đã liên kết',
                    data: [data.LinkedStudents || 0, data.LinkedTeachers || 0],
                    borderColor: C.sky,
                    backgroundColor: function (context) {
                        return createVerticalGradient(context.chart, [
                            [0, 'rgba(14,165,233,.22)'],
                            [1, 'rgba(255,255,255,0)']
                        ]);
                    },
                    borderWidth: 3,
                    fill: true,
                    tension: 0.45,
                    pointRadius: 4,
                    pointHoverRadius: 6,
                    pointBorderWidth: 0,
                    pointBackgroundColor: '#ffffff',
                    shadowBlur: 20,
                    shadowColor: 'rgba(14,165,233,.18)'
                }, {
                    label: 'Chưa liên kết',
                    data: [data.UnlinkedStudents || 0, data.UnlinkedTeachers || 0],
                    borderColor: C.pink,
                    backgroundColor: function (context) {
                        return createVerticalGradient(context.chart, [
                            [0, 'rgba(219,39,119,.18)'],
                            [1, 'rgba(255,255,255,0)']
                        ]);
                    },
                    borderWidth: 3,
                    fill: true,
                    tension: 0.45,
                    pointRadius: 4,
                    pointHoverRadius: 6,
                    pointBorderWidth: 0,
                    pointBackgroundColor: '#ffffff',
                    shadowBlur: 20,
                    shadowColor: 'rgba(219,39,119,.16)'
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                animation: { duration: 950, easing: 'easeOutQuart' },
                plugins: {
                    legend: {
                        position: 'bottom',
                        labels: {
                            color: C.muted,
                            padding: 14,
                            usePointStyle: true,
                            pointStyleWidth: 10,
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

    function initRipple() {
        $$('.admin-dashboard-page .da-action-item').forEach(function (el) {
            el.addEventListener('click', function (event) {
                var rect = el.getBoundingClientRect();
                var ripple = document.createElement('span');
                ripple.style.cssText = [
                    'position:absolute',
                    'border-radius:50%',
                    'background:rgba(255,255,255,.52)',
                    'width:8px',
                    'height:8px',
                    'pointer-events:none',
                    'transform:scale(0)',
                    'transition:transform .55s ease,opacity .55s ease',
                    'left:' + (event.clientX - rect.left - 4) + 'px',
                    'top:' + (event.clientY - rect.top - 4) + 'px'
                ].join(';');
                el.appendChild(ripple);

                requestAnimationFrame(function () {
                    ripple.style.transform = 'scale(38)';
                    ripple.style.opacity = '0';
                });

                setTimeout(function () { ripple.remove(); }, 650);
            });
        });
    }

    function init() {
        var data = window.adminDashboardData || {};
        initGlow();
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

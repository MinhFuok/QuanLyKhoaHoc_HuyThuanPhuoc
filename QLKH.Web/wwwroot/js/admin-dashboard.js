(function (window, document) {
    'use strict';

    if (window.AdminDashboardLib || !window.AdminCore) {
        return;
    }

    var core = window.AdminCore;
    var q = core.q;
    var qq = core.qq;

    function countUp(element, value, duration) {
        var target = parseInt(value, 10);

        if (Number.isNaN(target)) {
            element.textContent = value;
            return;
        }

        var start = null;

        function step(timestamp) {
            if (!start) {
                start = timestamp;
            }

            var progress = Math.min((timestamp - start) / duration, 1);
            var eased = 1 - Math.pow(1 - progress, 3);
            element.textContent = Math.round(eased * target).toLocaleString('vi-VN');

            if (progress < 1) {
                window.requestAnimationFrame(step);
            }
        }

        element.textContent = '0';
        window.requestAnimationFrame(step);
    }

    function initCountUps(root) {
        qq('.stat-value', root).forEach(function (element) {
            var raw = element.textContent.trim().replace(/\D/g, '');

            if (!raw) {
                return;
            }

            window.setTimeout(function () {
                countUp(element, raw, 820);
            }, 160);
        });
    }

    function initReveal(root) {
        if (!root) {
            return;
        }

        var targets = [];

        ['.da-header', '.da-kpi', '.da-card', '.da-maintenance-alert'].forEach(function (selector) {
            targets = targets.concat(qq(selector, root));
        });

        core.revealElements(targets, 'da-reveal', 0.05);
    }

    function getTheme(root) {
        var styles = window.getComputedStyle(root || document.documentElement);

        return {
            primary: styles.getPropertyValue('--dashboard-primary').trim() || '#2563eb',
            secondary: styles.getPropertyValue('--dashboard-secondary').trim() || '#0ea5e9',
            tertiary: styles.getPropertyValue('--dashboard-tertiary').trim() || '#22d3ee',
            text: styles.getPropertyValue('--dashboard-text').trim() || '#0f172a',
            muted: styles.getPropertyValue('--dashboard-muted').trim() || '#64748b',
            grid: 'rgba(148,163,184,0.18)',
            white: '#ffffff'
        };
    }

    function createVerticalGradient(chart, stops) {
        var area = chart.chartArea;

        if (!area) {
            return stops[0][1];
        }

        var gradient = chart.ctx.createLinearGradient(0, area.top, 0, area.bottom);

        stops.forEach(function (stop) {
            gradient.addColorStop(stop[0], stop[1]);
        });

        return gradient;
    }

    function applyChartDefaults(theme) {
        if (typeof Chart === 'undefined') {
            return false;
        }

        Chart.defaults.font.family = "'Roboto', 'Segoe UI', Arial, sans-serif";
        Chart.defaults.font.size = 12;
        Chart.defaults.color = theme.muted;
        Chart.defaults.borderColor = theme.grid;

        if (!Chart._adminDashboardPlugin) {
            Chart.register({
                id: 'adminDashboardCenterText',
                afterDraw: function (chart, args, options) {
                    if (chart.config.type !== 'doughnut' || !options || !options.lines || !options.lines.length) {
                        return;
                    }

                    var meta = chart.getDatasetMeta(0);

                    if (!meta || !meta.data || !meta.data.length) {
                        return;
                    }

                    var ctx = chart.ctx;
                    var centerX = meta.data[0].x;
                    var centerY = meta.data[0].y;
                    var lineHeight = options.lineHeight || 18;
                    var totalHeight = options.lines.length * lineHeight;
                    var startY = centerY - totalHeight / 2 + lineHeight / 2;

                    ctx.save();
                    ctx.textAlign = 'center';
                    ctx.textBaseline = 'middle';

                    options.lines.forEach(function (line, index) {
                        ctx.font = line.font || "700 22px 'Roboto', sans-serif";
                        ctx.fillStyle = line.color || theme.text;
                        ctx.fillText(line.text, centerX, startY + index * lineHeight);
                    });

                    ctx.restore();
                }
            });

            Chart._adminDashboardPlugin = true;
        }

        return true;
    }

    function tooltip(theme, overrides) {
        return Object.assign({
            backgroundColor: 'rgba(15,23,42,0.94)',
            padding: 12,
            cornerRadius: 14,
            titleColor: theme.white,
            bodyColor: 'rgba(255,255,255,0.88)',
            displayColors: true,
            boxPadding: 4
        }, overrides || {});
    }

    function baseScales(theme) {
        return {
            x: {
                grid: { display: false },
                border: { display: false },
                ticks: {
                    color: theme.muted,
                    font: { weight: '700' }
                }
            },
            y: {
                beginAtZero: true,
                grid: { color: theme.grid, drawBorder: false },
                border: { display: false },
                ticks: {
                    precision: 0,
                    color: theme.muted,
                    callback: function (value) {
                        return value.toLocaleString('vi-VN');
                    }
                }
            }
        };
    }

    function createBarChart(canvasId, theme, config) {
        var canvas = document.getElementById(canvasId);

        if (!canvas || typeof Chart === 'undefined') {
            return null;
        }

        return new Chart(canvas, {
            type: 'bar',
            data: {
                labels: config.labels,
                datasets: [{
                    label: config.label || 'Số lượng',
                    data: config.data,
                    backgroundColor: function (context) {
                        return createVerticalGradient(context.chart, config.gradient || [
                            [0, theme.secondary],
                            [1, theme.primary]
                        ]);
                    },
                    borderRadius: 16,
                    borderSkipped: false,
                    maxBarThickness: config.maxBarThickness || 46
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                animation: { duration: 850, easing: 'easeOutQuart' },
                plugins: {
                    legend: { display: false },
                    tooltip: tooltip(theme, {
                        callbacks: {
                            label: function (context) {
                                return '  ' + context.dataset.label + ': ' + context.parsed.y.toLocaleString('vi-VN');
                            }
                        }
                    })
                },
                scales: baseScales(theme)
            }
        });
    }

    function createStackedBarChart(canvasId, theme, config) {
        var canvas = document.getElementById(canvasId);

        if (!canvas || typeof Chart === 'undefined') {
            return null;
        }

        return new Chart(canvas, {
            type: 'bar',
            data: config.data,
            options: {
                responsive: true,
                maintainAspectRatio: false,
                animation: { duration: 850, easing: 'easeOutQuart' },
                plugins: {
                    legend: {
                        position: 'bottom',
                        labels: {
                            color: theme.muted,
                            padding: 16,
                            usePointStyle: true,
                            pointStyle: 'circle',
                            font: { weight: '700', size: 12 }
                        }
                    },
                    tooltip: tooltip(theme, {
                        callbacks: {
                            label: function (context) {
                                return '  ' + context.dataset.label + ': ' + context.parsed.y.toLocaleString('vi-VN');
                            }
                        }
                    })
                },
                scales: baseScales(theme)
            }
        });
    }

    function createDoughnutChart(canvasId, theme, config) {
        var canvas = document.getElementById(canvasId);

        if (!canvas || typeof Chart === 'undefined') {
            return null;
        }

        return new Chart(canvas, {
            type: 'doughnut',
            data: {
                labels: config.labels,
                datasets: [{
                    data: config.data,
                    backgroundColor: config.colors,
                    borderColor: [theme.white, theme.white, theme.white],
                    borderWidth: 8,
                    spacing: 3,
                    hoverOffset: 5
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                cutout: '70%',
                animation: { duration: 820, easing: 'easeOutQuart' },
                plugins: {
                    legend: {
                        position: 'bottom',
                        labels: {
                            color: theme.muted,
                            padding: 18,
                            usePointStyle: true,
                            pointStyle: 'circle',
                            font: { weight: '700', size: 12 }
                        }
                    },
                    adminDashboardCenterText: config.centerText,
                    tooltip: tooltip(theme, {
                        callbacks: {
                            label: function (context) {
                                var total = context.dataset.data.reduce(function (sum, value) {
                                    return sum + value;
                                }, 0);
                                var percentage = total ? Math.round(context.parsed / total * 100) : 0;
                                return '  ' + context.label + ': ' + context.parsed.toLocaleString('vi-VN') + ' (' + percentage + '%)';
                            }
                        }
                    })
                }
            }
        });
    }

    function setup(rootSelector) {
        var root = q(rootSelector);

        if (!root) {
            return null;
        }

        initReveal(root);
        initCountUps(root);

        var theme = getTheme(root);

        if (!applyChartDefaults(theme)) {
            return { root: root, theme: theme, ready: false };
        }

        return { root: root, theme: theme, ready: true };
    }

    window.AdminDashboardLib = {
        setup: setup,
        createBarChart: createBarChart,
        createStackedBarChart: createStackedBarChart,
        createDoughnutChart: createDoughnutChart
    };
})(window, document);

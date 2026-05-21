(function (window) {
    'use strict';

    if (!window.AdminCore || !window.AdminDashboardLib) {
        return;
    }

    window.AdminCore.onReady(function () {
        var setup = window.AdminDashboardLib.setup('.admin-dashboard-page');

        if (!setup || !setup.ready) {
            return;
        }

        var data = window.AdminCore.parseDataset('adminDashboardData', {
            TotalStudents: { name: 'totalStudents', type: 'number' },
            TotalTeachers: { name: 'totalTeachers', type: 'number' },
            TotalCourses: { name: 'totalCourses', type: 'number' },
            TotalClassRooms: { name: 'totalClassRooms', type: 'number' },
            TotalDepartments: { name: 'totalDepartments', type: 'number' },
            ActiveUsers: { name: 'activeUsers', type: 'number' },
            LockedUsers: { name: 'lockedUsers', type: 'number' },
            LinkedStudents: { name: 'linkedStudents', type: 'number' },
            UnlinkedStudents: { name: 'unlinkedStudents', type: 'number' },
            LinkedTeachers: { name: 'linkedTeachers', type: 'number' },
            UnlinkedTeachers: { name: 'unlinkedTeachers', type: 'number' }
        });

        window.AdminDashboardLib.createBarChart('overviewBarChart', setup.theme, {
            labels: ['Học viên', 'Giáo viên', 'Khóa học', 'Lớp học', 'Chương trình'],
            data: [data.TotalStudents, data.TotalTeachers, data.TotalCourses, data.TotalClassRooms, data.TotalDepartments],
            gradient: [
                [0, '#8b5cf6'],
                [0.55, '#5b45f2'],
                [1, '#2388ff']
            ]
        });

        window.AdminDashboardLib.createDoughnutChart('userStatusChart', setup.theme, {
            labels: ['Đang hoạt động', 'Đã khóa'],
            data: [data.ActiveUsers, data.LockedUsers],
            colors: ['#10b981', '#fb923c'],
            centerText: {
                lineHeight: 22,
                lines: [
                    { text: data.ActiveUsers.toLocaleString('vi-VN'), font: "900 28px 'Roboto', sans-serif", color: setup.theme.text },
                    { text: 'hoạt động', font: "700 12px 'Roboto', sans-serif", color: setup.theme.muted }
                ]
            }
        });

        window.AdminDashboardLib.createStackedBarChart('linkStatusChart', setup.theme, {
            data: {
                labels: ['Học viên', 'Giáo viên'],
                datasets: [{
                    label: 'Đã liên kết',
                    data: [data.LinkedStudents, data.LinkedTeachers],
                    backgroundColor: '#2388ff',
                    borderRadius: 14,
                    borderSkipped: false,
                    maxBarThickness: 42
                }, {
                    label: 'Chưa liên kết',
                    data: [data.UnlinkedStudents, data.UnlinkedTeachers],
                    backgroundColor: '#f43f5e',
                    borderRadius: 14,
                    borderSkipped: false,
                    maxBarThickness: 42
                }]
            }
        });
    });
})(window);

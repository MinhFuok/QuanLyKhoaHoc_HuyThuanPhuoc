(function (window) {
    'use strict';

    if (!window.AdminCore || !window.AdminDashboardLib) {
        return;
    }

    window.AdminCore.onReady(function () {
        var setup = window.AdminDashboardLib.setup('.hocvu-dashboard-page');

        if (!setup || !setup.ready) {
            return;
        }

        var data = window.AdminCore.parseDataset('hocVuDashboardData', {
            TotalCourses: { name: 'totalCourses', type: 'number' },
            TotalClassRooms: { name: 'totalClassRooms', type: 'number' },
            TotalStudents: { name: 'totalStudents', type: 'number' },
            TotalTeachers: { name: 'totalTeachers', type: 'number' },
            TotalEnrollments: { name: 'totalEnrollments', type: 'number' },
            PendingEnrollments: { name: 'pendingEnrollments', type: 'number' },
            ConfirmedEnrollments: { name: 'confirmedEnrollments', type: 'number' },
            CancelledEnrollments: { name: 'cancelledEnrollments', type: 'number' },
            TotalCertificates: { name: 'totalCertificates', type: 'number' }
        });

        window.AdminDashboardLib.createBarChart('hocVuOverviewChart', setup.theme, {
            labels: ['Khóa học', 'Lớp học', 'Học viên', 'Giảng viên', 'Chứng chỉ'],
            data: [data.TotalCourses, data.TotalClassRooms, data.TotalStudents, data.TotalTeachers, data.TotalCertificates],
            gradient: [
                [0, '#5b45f2'],
                [0.55, '#0f77f0'],
                [1, '#18b8f2']
            ]
        });

        window.AdminDashboardLib.createDoughnutChart('hocVuEnrollmentChart', setup.theme, {
            labels: ['Chờ duyệt', 'Đã duyệt', 'Đã hủy'],
            data: [data.PendingEnrollments, data.ConfirmedEnrollments, data.CancelledEnrollments],
            colors: ['#fb923c', '#22c55e', '#8b5cf6'],
            centerText: {
                lineHeight: 22,
                lines: [
                    { text: data.TotalEnrollments.toLocaleString('vi-VN'), font: "900 28px 'Roboto', sans-serif", color: setup.theme.text },
                    { text: 'ghi danh', font: "700 12px 'Roboto', sans-serif", color: setup.theme.muted }
                ]
            }
        });
    });
})(window);

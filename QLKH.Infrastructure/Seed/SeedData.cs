using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLKH.Domain.Entities;
using QLKH.Domain.Enums;
using QLKH.Infrastructure.Data;

namespace QLKH.Infrastructure.Seed
{
    public static class SeedData
    {
        public static async Task InitializeAsync(AppDbContext context)
        {
            var testStudentEmail = "nsmp2022@gmail.com";

            if (!context.Courses.Any())
            {
                var courses = new List<Course>
                {
                    new Course
                    {
                        CourseCode = "C001",
                        CourseName = "Lập trình C# cơ bản",
                        Description = "Khóa học nhập môn lập trình C#",
                        DurationInMonths = 3,
                        Fee = 1500000,
                        Status = CourseStatus.Active
                    },
                    new Course
                    {
                        CourseCode = "C002",
                        CourseName = "ASP.NET Core MVC",
                        Description = "Khóa học xây dựng web với ASP.NET Core MVC",
                        DurationInMonths = 4,
                        Fee = 2500000,
                        Status = CourseStatus.Active
                    }
                };

                await context.Courses.AddRangeAsync(courses);
                await context.SaveChangesAsync();
            }

            if (!context.Teachers.Any())
            {
                var teacher = new Teacher
                {
                    TeacherCode = "GV001",
                    FullName = "Nguyen Van Teacher",
                    Email = "giaovien@qlkh.com",
                    PhoneNumber = "0900000001",
                    Specialization = "English"
                };

                await context.Teachers.AddAsync(teacher);
                await context.SaveChangesAsync();
            }

            if (!context.Students.Any())
            {
                var student = new Student
                {
                    StudentCode = "HV001",
                    FullName = "Nguyen Van Student",
                    Email = "hocvien@qlkh.com",
                    PhoneNumber = "0900000002",
                    DateOfBirth = new DateTime(2005, 1, 1),
                    Address = "Ho Chi Minh City"
                };

                await context.Students.AddAsync(student);
                await context.SaveChangesAsync();
            }

            if (!context.Students.Any(x => x.Email == testStudentEmail))
            {
                var testStudentUser = context.Users.FirstOrDefault(x => x.Email == testStudentEmail);

                if (testStudentUser != null)
                {
                    var testStudent = new Student
                    {
                        StudentCode = "HV999",
                        FullName = "Hoc Vien Test Gmail",
                        Email = testStudentEmail,
                        PhoneNumber = "0999999999",
                        DateOfBirth = new DateTime(2000, 1, 1),
                        Address = "Ho Chi Minh City",
                        ApplicationUserId = testStudentUser.Id
                    };

                    await context.Students.AddAsync(testStudent);
                    await context.SaveChangesAsync();
                }
            }

            if (!context.ClassRooms.Any())
            {
                var firstCourse = context.Courses.FirstOrDefault();
                var firstTeacher = context.Teachers.FirstOrDefault();

                if (firstCourse != null && firstTeacher != null)
                {
                    var classRooms = new List<ClassRoom>
                    {
                        new ClassRoom
                        {
                            ClassCode = "CL001",
                            ClassName = "Lớp C# K1",
                            CourseId = firstCourse.Id,
                            TeacherId = firstTeacher.Id,
                            StartDate = DateTime.Today,
                            EndDate = DateTime.Today.AddMonths(3),
                            MaxStudents = 30
                        }
                    };

                    await context.ClassRooms.AddRangeAsync(classRooms);
                    await context.SaveChangesAsync();
                }
            }

            if (!context.Enrollments.Any())
            {
                var firstStudent = context.Students.FirstOrDefault();
                var firstClassRoom = context.ClassRooms.FirstOrDefault();

                if (firstStudent != null && firstClassRoom != null)
                {
                    var enrollments = new List<Enrollment>
                    {
                        new Enrollment
                        {
                            StudentId = firstStudent.Id,
                            ClassRoomId = firstClassRoom.Id,
                            EnrolledAt = DateTime.Now,
                            Status = EnrollmentStatus.Confirmed
                        }
                    };

                    await context.Enrollments.AddRangeAsync(enrollments);
                    await context.SaveChangesAsync();
                }
            }

            var testStudentEntity = context.Students.FirstOrDefault(x => x.Email == testStudentEmail);
            var firstClassRoomForTest = context.ClassRooms.FirstOrDefault();

            if (testStudentEntity != null && firstClassRoomForTest != null &&
                !context.Enrollments.Any(x => x.StudentId == testStudentEntity.Id && x.ClassRoomId == firstClassRoomForTest.Id))
            {
                var testEnrollment = new Enrollment
                {
                    StudentId = testStudentEntity.Id,
                    ClassRoomId = firstClassRoomForTest.Id,
                    EnrolledAt = DateTime.Now,
                    Status = EnrollmentStatus.Confirmed
                };

                await context.Enrollments.AddAsync(testEnrollment);
                await context.SaveChangesAsync();
            }

            if (!context.ClassSchedules.Any())
            {
                var firstClassRoom = context.ClassRooms.FirstOrDefault();

                if (firstClassRoom != null)
                {
                    var schedules = new List<ClassSchedule>
                    {
                        new ClassSchedule
                        {
                            ClassRoomId = firstClassRoom.Id,
                            LessonTitle = "Buổi 1 - Giới thiệu khóa học",
                            StudyDate = DateTime.Today.AddDays(1),
                            StartTime = new TimeSpan(18, 0, 0),
                            EndTime = new TimeSpan(20, 0, 0),
                            RoomName = "Phòng A101",
                            Note = "Ổn định lớp và giới thiệu nội dung khóa học"
                        },
                        new ClassSchedule
                        {
                            ClassRoomId = firstClassRoom.Id,
                            LessonTitle = "Buổi 2 - Kiến thức nền tảng",
                            StudyDate = DateTime.Today.AddDays(3),
                            StartTime = new TimeSpan(18, 0, 0),
                            EndTime = new TimeSpan(20, 0, 0),
                            RoomName = "Phòng A101",
                            Note = "Ôn tập kiến thức cơ bản"
                        }
                    };

                    await context.ClassSchedules.AddRangeAsync(schedules);
                    await context.SaveChangesAsync();
                }
            }

            if (!context.ClassMaterials.Any())
            {
                var firstClassRoom = context.ClassRooms.FirstOrDefault();

                if (firstClassRoom != null)
                {
                    var materials = new List<ClassMaterial>
                    {
                        new ClassMaterial
                        {
                            ClassRoomId = firstClassRoom.Id,
                            Title = "Slide buổi 1",
                            Description = "Tài liệu giới thiệu khóa học và nội dung buổi học đầu tiên",
                            FilePath = "/materials/cl001/slide-buoi-1.pdf",
                            CreatedAt = DateTime.Now
                        },
                        new ClassMaterial
                        {
                            ClassRoomId = firstClassRoom.Id,
                            Title = "Tài liệu tham khảo chương 1",
                            Description = "Tài liệu đọc thêm cho học viên",
                            FilePath = "/materials/cl001/tai-lieu-tham-khao-chuong-1.pdf",
                            CreatedAt = DateTime.Now
                        }
                    };

                    await context.ClassMaterials.AddRangeAsync(materials);
                    await context.SaveChangesAsync();
                }
            }

            if (!context.Assignments.Any())
            {
                var firstClassRoom = context.ClassRooms.FirstOrDefault();

                if (firstClassRoom != null)
                {
                    var assignments = new List<Assignment>
                    {
                        new Assignment
                        {
                            ClassRoomId = firstClassRoom.Id,
                            Title = "Bài tập 1 - Ôn tập kiến thức cơ bản",
                            Description = "Học viên làm bài tập ôn lại các nội dung đã học ở buổi đầu tiên.",
                            DueDate = DateTime.Today.AddDays(7),
                            CreatedAt = DateTime.Now
                        },
                        new Assignment
                        {
                            ClassRoomId = firstClassRoom.Id,
                            Title = "Bài tập 2 - Thực hành chương 1",
                            Description = "Hoàn thành các câu hỏi và bài thực hành trong chương 1.",
                            DueDate = DateTime.Today.AddDays(10),
                            CreatedAt = DateTime.Now
                        }
                    };

                    await context.Assignments.AddRangeAsync(assignments);
                    await context.SaveChangesAsync();
                }
            }
            if (!context.SystemSettings.Any())
            {
                var setting = new SystemSetting
                {
                    SiteName = "QLKH",
                    HomeBannerTitle = "Hệ thống quản lý khóa học online",
                    HomeBannerSubtitle = "Học mọi lúc, mọi nơi với các khóa học trực tuyến chất lượng",
                    HomeBannerImageUrl = "/images/banner-default.jpg",

                    FooterText = "© 2026 QLKH - Nền tảng quản lý khóa học online",
                    ContactEmail = "support@qlkh.com",
                    ContactPhone = "0123456789",
                    ContactAddress = "Thủ Dầu Một, Bình Dương",
                    FacebookUrl = "https://facebook.com/",
                    YoutubeUrl = "https://youtube.com/",

                    IsWebsiteEnabled = true,
                    MaintenanceMessage = "Hệ thống đang bảo trì. Vui lòng quay lại sau.",

                    EnableEmail = true,
                    SmtpServer = "smtp.gmail.com",
                    SmtpPort = 587,
                    SenderName = "QLKH",
                    SenderEmail = "example@gmail.com",
                    SmtpUsername = "example@gmail.com",
                    SmtpPassword = "app-password-demo",

                    EnableVnPay = false,
                    EnableMomo = false
                };

                await context.SystemSettings.AddAsync(setting);
                await context.SaveChangesAsync();
            }
            if (!context.Departments.Any())
            {
                var khoaCongNghe = new Department
                {
                    DepartmentCode = "KCN",
                    DepartmentName = "Khoa Công nghệ",
                    Description = "Nhóm ngành công nghệ và kỹ thuật",
                    IsActive = true
                };

                var khoaNgoaiNgu = new Department
                {
                    DepartmentCode = "KNN",
                    DepartmentName = "Khoa Ngoại ngữ",
                    Description = "Nhóm ngành ngoại ngữ và luyện thi chứng chỉ",
                    IsActive = true
                };

                await context.Departments.AddRangeAsync(khoaCongNghe, khoaNgoaiNgu);
                await context.SaveChangesAsync();

                var childDepartments = new List<Department>
                {
                    new Department
                    {
                        DepartmentCode = "CNTT",
                        DepartmentName = "Công nghệ thông tin",
                        Description = "Lập trình, phát triển phần mềm, web",
                        IsActive = true,
                        ParentDepartmentId = khoaCongNghe.Id
                    },
                    new Department
                    {
                        DepartmentCode = "KTPM",
                        DepartmentName = "Kỹ thuật phần mềm",
                        Description = "Phân tích, thiết kế và phát triển phần mềm",
                        IsActive = true,
                        ParentDepartmentId = khoaCongNghe.Id
                    },
                    new Department
                    {
                        DepartmentCode = "KHDL",
                        DepartmentName = "Khoa học dữ liệu",
                        Description = "Dữ liệu, phân tích, AI cơ bản",
                        IsActive = true,
                        ParentDepartmentId = khoaCongNghe.Id
                    },
                    new Department
                    {
                        DepartmentCode = "IELTS",
                        DepartmentName = "Luyện thi IELTS",
                        Description = "Các khóa luyện thi IELTS online",
                        IsActive = true,
                        ParentDepartmentId = khoaNgoaiNgu.Id
                    },
                    new Department
                    {
                        DepartmentCode = "TOEIC",
                        DepartmentName = "Luyện thi TOEIC",
                        Description = "Các khóa luyện thi TOEIC online",
                        IsActive = true,
                        ParentDepartmentId = khoaNgoaiNgu.Id
                    },
                    new Department
                    {
                        DepartmentCode = "TAGT",
                        DepartmentName = "Tiếng Anh giao tiếp",
                        Description = "Các khóa tiếng Anh giao tiếp online",
                        IsActive = true,
                        ParentDepartmentId = khoaNgoaiNgu.Id
                    }
                };

                await context.Departments.AddRangeAsync(childDepartments);
                await context.SaveChangesAsync();
            }
        }
    }
}
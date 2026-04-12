using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QLKH.Domain.Entities;
using QLKH.Infrastructure.Identity;

namespace QLKH.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<ClassRoom> ClassRooms { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<ClassSchedule> ClassSchedules { get; set; }
        public DbSet<ClassMaterial> ClassMaterials { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Submission> Submissions { get; set; }
        public DbSet<TeacherReview> TeacherReviews { get; set; }
        public DbSet<SystemSetting> SystemSettings { get; set; }
        public DbSet<Department> Departments { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.CourseCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(x => x.CourseName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(x => x.Description)
                    .HasMaxLength(1000);

                entity.Property(x => x.Fee)
                    .HasColumnType("decimal(18,2)");

                entity.HasIndex(x => x.CourseCode)
                    .IsUnique();
            });

            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.TeacherCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(x => x.FullName)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(x => x.Email)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(x => x.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(x => x.Specialization)
                    .HasMaxLength(200);

                entity.Property(x => x.ApplicationUserId)
                    .HasMaxLength(450);

                entity.HasIndex(x => x.TeacherCode)
                    .IsUnique();

                entity.HasIndex(x => x.ApplicationUserId)
                    .IsUnique()
                    .HasFilter("[ApplicationUserId] IS NOT NULL");

                entity.HasOne<ApplicationUser>()
                    .WithOne()
                    .HasForeignKey<Teacher>(x => x.ApplicationUserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.StudentCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(x => x.FullName)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(x => x.Email)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(x => x.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(x => x.Address)
                    .HasMaxLength(300);

                entity.Property(x => x.ApplicationUserId)
                    .HasMaxLength(450);

                entity.HasIndex(x => x.StudentCode)
                    .IsUnique();

                entity.HasIndex(x => x.ApplicationUserId)
                    .IsUnique()
                    .HasFilter("[ApplicationUserId] IS NOT NULL");

                entity.HasOne<ApplicationUser>()
                    .WithOne()
                    .HasForeignKey<Student>(x => x.ApplicationUserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<ClassRoom>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.ClassCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(x => x.ClassName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.HasIndex(x => x.ClassCode)
                    .IsUnique();

                entity.HasOne(x => x.Course)
                    .WithMany(x => x.ClassRooms)
                    .HasForeignKey(x => x.CourseId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Teacher)
                    .WithMany(x => x.ClassRooms)
                    .HasForeignKey(x => x.TeacherId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasOne(x => x.Student)
                    .WithMany(x => x.Enrollments)
                    .HasForeignKey(x => x.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.ClassRoom)
                    .WithMany(x => x.Enrollments)
                    .HasForeignKey(x => x.ClassRoomId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => new { x.StudentId, x.ClassRoomId })
                    .IsUnique();
            });
            modelBuilder.Entity<ClassSchedule>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.LessonTitle)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(x => x.RoomName)
                    .HasMaxLength(100);

                entity.Property(x => x.Note)
                    .HasMaxLength(500);

                entity.HasOne(x => x.ClassRoom)
                    .WithMany(x => x.ClassSchedules)
                    .HasForeignKey(x => x.ClassRoomId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<ClassMaterial>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(x => x.Description)
                    .HasMaxLength(1000);

                entity.Property(x => x.FilePath)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(x => x.ClassRoom)
                    .WithMany(x => x.ClassMaterials)
                    .HasForeignKey(x => x.ClassRoomId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Assignment>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(x => x.Description)
                    .HasMaxLength(2000);

                entity.HasOne(x => x.ClassRoom)
                    .WithMany(x => x.Assignments)
                    .HasForeignKey(x => x.ClassRoomId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Submission>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.SubmissionText)
                    .HasMaxLength(4000);

                entity.Property(x => x.FilePath)
                    .HasMaxLength(500);

                entity.Property(x => x.Score)
                    .HasColumnType("decimal(5,2)");

                entity.Property(x => x.Feedback)
                    .HasMaxLength(2000);

                entity.HasOne(x => x.Assignment)
                    .WithMany(x => x.Submissions)
                    .HasForeignKey(x => x.AssignmentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.Student)
                    .WithMany(x => x.Submissions)
                    .HasForeignKey(x => x.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => new { x.AssignmentId, x.StudentId })
                    .IsUnique();
            });
            modelBuilder.Entity<TeacherReview>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Rating)
                    .IsRequired();

                entity.Property(x => x.Comment)
                    .HasMaxLength(2000);

                entity.HasOne(x => x.Teacher)
                    .WithMany(x => x.TeacherReviews)
                    .HasForeignKey(x => x.TeacherId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Student)
                    .WithMany(x => x.TeacherReviews)
                    .HasForeignKey(x => x.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.ClassRoom)
                    .WithMany(x => x.TeacherReviews)
                    .HasForeignKey(x => x.ClassRoomId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(x => new { x.StudentId, x.ClassRoomId })
                    .IsUnique();
            });
            modelBuilder.Entity<Department>()
                .HasOne(d => d.ParentDepartment)
                .WithMany(d => d.Children)
                .HasForeignKey(d => d.ParentDepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
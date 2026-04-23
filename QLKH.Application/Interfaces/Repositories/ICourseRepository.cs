using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLKH.Application.ViewModels;
using QLKH.Domain.Entities;

namespace QLKH.Application.Interfaces.Repositories
{
    public interface ICourseRepository
    {
        Task<IEnumerable<Course>> GetAllAsync();
        Task<Course?> GetByIdAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> ExistsByCodeAsync(string courseCode);

        Task<CourseDeleteImpactViewModel?> GetDeleteImpactAsync(int id);
        Task DeleteCascadeAsync(int id);

        Task AddAsync(Course course);
        void Update(Course course);
        void Delete(Course course);
        Task SaveChangesAsync();
    }
}
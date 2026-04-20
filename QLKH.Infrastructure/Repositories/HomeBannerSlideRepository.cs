using Microsoft.EntityFrameworkCore;
using QLKH.Application.Interfaces.Repositories;
using QLKH.Domain.Entities;
using QLKH.Infrastructure.Data;

namespace QLKH.Infrastructure.Repositories
{
    public class HomeBannerSlideRepository : IHomeBannerSlideRepository
    {
        private readonly AppDbContext _context;

        public HomeBannerSlideRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<HomeBannerSlide>> GetAllAsync()
        {
            return await _context.HomeBannerSlides
                .OrderBy(x => x.DisplayOrder)
                .ThenBy(x => x.Id)
                .ToListAsync();
        }

        public async Task<IEnumerable<HomeBannerSlide>> GetActiveSlidesAsync()
        {
            return await _context.HomeBannerSlides
                .Where(x => x.IsActive)
                .OrderBy(x => x.DisplayOrder)
                .ThenBy(x => x.Id)
                .ToListAsync();
        }

        public async Task<HomeBannerSlide?> GetByIdAsync(int id)
        {
            return await _context.HomeBannerSlides.FindAsync(id);
        }

        public async Task<int> GetNextDisplayOrderAsync()
        {
            var maxOrder = await _context.HomeBannerSlides
                .Select(x => (int?)x.DisplayOrder)
                .MaxAsync();

            return (maxOrder ?? 0) + 1;
        }

        public async Task AddAsync(HomeBannerSlide slide)
        {
            await _context.HomeBannerSlides.AddAsync(slide);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(HomeBannerSlide slide)
        {
            _context.HomeBannerSlides.Update(slide);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var slide = await _context.HomeBannerSlides.FindAsync(id);
            if (slide != null)
            {
                _context.HomeBannerSlides.Remove(slide);
                await _context.SaveChangesAsync();
            }
        }

        public async Task MoveUpAsync(int id)
        {
            var current = await _context.HomeBannerSlides.FindAsync(id);
            if (current == null) return;

            var previous = await _context.HomeBannerSlides
                .Where(x => x.DisplayOrder < current.DisplayOrder)
                .OrderByDescending(x => x.DisplayOrder)
                .FirstOrDefaultAsync();

            if (previous == null) return;

            var temp = current.DisplayOrder;
            current.DisplayOrder = previous.DisplayOrder;
            previous.DisplayOrder = temp;

            await _context.SaveChangesAsync();
        }

        public async Task MoveDownAsync(int id)
        {
            var current = await _context.HomeBannerSlides.FindAsync(id);
            if (current == null) return;

            var next = await _context.HomeBannerSlides
                .Where(x => x.DisplayOrder > current.DisplayOrder)
                .OrderBy(x => x.DisplayOrder)
                .FirstOrDefaultAsync();

            if (next == null) return;

            var temp = current.DisplayOrder;
            current.DisplayOrder = next.DisplayOrder;
            next.DisplayOrder = temp;

            await _context.SaveChangesAsync();
        }
    }
}
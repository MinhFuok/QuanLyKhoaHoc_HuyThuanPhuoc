using QLKH.Domain.Entities;

namespace QLKH.Application.Interfaces.Repositories
{
    public interface IHomeBannerSlideRepository
    {
        Task<IEnumerable<HomeBannerSlide>> GetAllAsync();
        Task<IEnumerable<HomeBannerSlide>> GetActiveSlidesAsync();
        Task<HomeBannerSlide?> GetByIdAsync(int id);
        Task<int> GetNextDisplayOrderAsync();
        Task AddAsync(HomeBannerSlide slide);
        Task UpdateAsync(HomeBannerSlide slide);
        Task DeleteAsync(int id);
        Task MoveUpAsync(int id);
        Task MoveDownAsync(int id);
    }
}
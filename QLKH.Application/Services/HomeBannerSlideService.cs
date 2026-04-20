using QLKH.Application.Interfaces.Repositories;
using QLKH.Application.Interfaces.Services;
using QLKH.Domain.Entities;

namespace QLKH.Application.Services
{
    public class HomeBannerSlideService : IHomeBannerSlideService
    {
        private readonly IHomeBannerSlideRepository _repository;

        public HomeBannerSlideService(IHomeBannerSlideRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<HomeBannerSlide>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<IEnumerable<HomeBannerSlide>> GetActiveSlidesAsync()
        {
            return await _repository.GetActiveSlidesAsync();
        }

        public async Task<HomeBannerSlide?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<int> GetNextDisplayOrderAsync()
        {
            return await _repository.GetNextDisplayOrderAsync();
        }

        public async Task AddAsync(HomeBannerSlide slide)
        {
            await _repository.AddAsync(slide);
        }

        public async Task UpdateAsync(HomeBannerSlide slide)
        {
            await _repository.UpdateAsync(slide);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task MoveUpAsync(int id)
        {
            await _repository.MoveUpAsync(id);
        }

        public async Task MoveDownAsync(int id)
        {
            await _repository.MoveDownAsync(id);
        }
    }
}
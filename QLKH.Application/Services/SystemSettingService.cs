using QLKH.Application.Interfaces.Repositories;
using QLKH.Application.Interfaces.Services;
using QLKH.Domain.Entities;

namespace QLKH.Application.Services
{
    public class SystemSettingService : ISystemSettingService
    {
        private readonly ISystemSettingRepository _repository;

        public SystemSettingService(ISystemSettingRepository repository)
        {
            _repository = repository;
        }

        public async Task<SystemSetting?> GetAsync()
        {
            return await _repository.GetFirstAsync();
        }

        public async Task UpdateAsync(SystemSetting setting)
        {
            await _repository.UpdateAsync(setting);
        }
    }
}
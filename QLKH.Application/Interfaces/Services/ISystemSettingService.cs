using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLKH.Domain.Entities;

namespace QLKH.Application.Interfaces.Services
{
    public interface ISystemSettingService
    {
        Task<SystemSetting?> GetAsync();
        Task UpdateAsync(SystemSetting setting);
    }
}
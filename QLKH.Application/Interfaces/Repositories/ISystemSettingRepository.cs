using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using QLKH.Domain.Entities;

namespace QLKH.Application.Interfaces.Repositories
{
    public interface ISystemSettingRepository
    {
        Task<SystemSetting?> GetFirstAsync();
        Task UpdateAsync(SystemSetting setting);
    }
}
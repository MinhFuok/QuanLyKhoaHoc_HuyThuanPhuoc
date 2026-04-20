using QLKH.Domain.Entities;

namespace QLKH.Web.Areas.Admin.Models
{
    public class SystemSettingsPageViewModel
    {
        public SystemSettingViewModel SystemSetting { get; set; } = new SystemSettingViewModel();

        public List<HomeBannerSlide> BannerSlides { get; set; } = new List<HomeBannerSlide>();
    }
}
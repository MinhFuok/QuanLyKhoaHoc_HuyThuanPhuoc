using QLKH.Domain.Entities;

namespace QLKH.Web.Models
{
    public class HomePageViewModel
    {
        public SystemSetting? SystemSetting { get; set; }
        public IEnumerable<HomeBannerSlide> BannerSlides { get; set; } = new List<HomeBannerSlide>();
    }
}
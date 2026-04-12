using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace QLKH.Web.Areas.HocVien.Controllers
{
    [Area("HocVien")]
    [Authorize(Roles = "Admin,HocVien")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
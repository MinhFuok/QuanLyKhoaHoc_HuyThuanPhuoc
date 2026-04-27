using Microsoft.AspNetCore.Mvc;
using QLKH.Application.Interfaces.Services;
using QLKH.Application.ViewModels.AiSupport;

namespace QLKH.Web.Controllers
{
    public class AiSupportController : Controller
    {
        private readonly IAiSupportService _aiSupportService;

        public AiSupportController(IAiSupportService aiSupportService)
        {
            _aiSupportService = aiSupportService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewData["Title"] = "AI Tư vấn hỗ trợ";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ask([FromBody] AiChatRequest request)
        {
            if (!ModelState.IsValid)
            {
                return Json(new AiChatResponse
                {
                    Success = false,
                    ErrorMessage = "Câu hỏi không hợp lệ hoặc quá dài."
                });
            }

            var result = await _aiSupportService.AskAsync(request);
            return Json(result);
        }
    }
}
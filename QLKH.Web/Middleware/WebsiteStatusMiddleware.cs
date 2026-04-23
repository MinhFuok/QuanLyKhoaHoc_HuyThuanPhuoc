using QLKH.Application.Interfaces.Services;

namespace QLKH.Web.Middleware
{
    public class WebsiteStatusMiddleware
    {
        private readonly RequestDelegate _next;

        public WebsiteStatusMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ISystemSettingService systemSettingService)
        {
            var path = context.Request.Path.Value?.ToLowerInvariant() ?? string.Empty;

            // Bỏ qua static files, login, admin, error...
            if (IsExcludedPath(path))
            {
                await _next(context);
                return;
            }

            var setting = await systemSettingService.GetAsync();

            if (setting != null && !setting.IsWebsiteEnabled)
            {
                context.Response.Redirect("/Home/Maintenance");
                return;
            }

            await _next(context);
        }

        private static bool IsExcludedPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            return path.StartsWith("/admin")
                   || path.StartsWith("/account/login")
                   || path.StartsWith("/account/logout")
                   || path.StartsWith("/account/accessdenied")
                   || path.StartsWith("/home/maintenance")
                   || path.StartsWith("/css")
                   || path.StartsWith("/js")
                   || path.StartsWith("/lib")
                   || path.StartsWith("/images")
                   || path.StartsWith("/uploads")
                   || path.StartsWith("/favicon")
                   || path.StartsWith("/error");
        }
    }
}
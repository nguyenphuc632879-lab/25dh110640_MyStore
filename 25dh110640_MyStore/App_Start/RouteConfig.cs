using System.Web.Mvc;
using System.Web.Routing;

namespace MyStore
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Route mặc định của phần Customer.
            // Thêm namespaces để tránh lỗi trùng tên HomeController với Area Admin (BE#1 - slide 7)
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "MyStore.Controllers" }
            );
        }
    }
}

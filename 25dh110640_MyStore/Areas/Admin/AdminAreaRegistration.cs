using System.Web.Mvc;

namespace MyStore.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get { return "Admin"; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            // Thêm namespaces để tránh lỗi trùng HomeController với phần Customer (BE#1)
            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "MyStore.Areas.Admin.Controllers" }
            );
        }
    }
}

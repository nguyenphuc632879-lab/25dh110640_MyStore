using System;
using System.Web.Mvc;
using MyStore.Models;
using MyStore.Models.ViewModel;

namespace MyStore.Controllers
{
    /// <summary>
    /// Controller cơ sở: khởi tạo DbContext + cung cấp sẵn dữ liệu cho
    /// menu cá nhân (PersonalMenu) dùng chung cho mọi trang.
    /// </summary>
    public class BaseController : Controller
    {
        protected MyStoreEntities db = new MyStoreEntities();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Số lượng mặt hàng trong giỏ -> hiển thị trên biểu tượng giỏ hàng
            var cart = Session["Cart"] as Cart;
            var pm = new PersonalMenuVM
            {
                IsLoggedIn = User != null && User.Identity != null && User.Identity.IsAuthenticated,
                Username = (User != null && User.Identity != null) ? User.Identity.Name : "",
                CartCount = cart != null ? cart.Count : 0
            };
            ViewBag.pmmodel = pm;

            base.OnActionExecuting(filterContext);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}

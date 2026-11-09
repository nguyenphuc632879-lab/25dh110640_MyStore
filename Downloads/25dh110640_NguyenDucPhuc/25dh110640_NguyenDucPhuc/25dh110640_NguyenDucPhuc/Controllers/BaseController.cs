using System;
using System.Web.Mvc;
using MyStore.Models;
using MyStore.Models.ViewModel;

namespace MyStore.Controllers
{
    public class BaseController : Controller
    {
        protected MyStoreEntities db = new MyStoreEntities();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
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

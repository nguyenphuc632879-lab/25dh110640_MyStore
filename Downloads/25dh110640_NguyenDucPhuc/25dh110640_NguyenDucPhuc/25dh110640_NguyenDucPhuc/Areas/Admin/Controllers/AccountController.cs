using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MyStore.Models;

namespace MyStore.Areas.Admin.Controllers
{
    public class AccountController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string Username, string Password, bool RememberMe = false, string returnUrl = "")
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                ViewBag.Error = "Vui lòng nhập tên đăng nhập và mật khẩu";
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }

            var user = db.Users.FirstOrDefault(u =>
                u.Username == Username &&
                u.Password.Trim() == Password.Trim() &&
                u.UserRole.Trim() == UserRoles.Admin);

            if (user == null)
            {
                ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng, hoặc tài khoản không có quyền Admin";
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }

            var ticket = new FormsAuthenticationTicket(
                1, user.Username, DateTime.Now,
                DateTime.Now.AddMinutes(FormsAuthentication.Timeout.TotalMinutes),
                RememberMe, UserRoles.Admin, FormsAuthentication.FormsCookiePath);

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket))
            {
                HttpOnly = true
            };
            if (RememberMe) cookie.Expires = ticket.Expiration;
            Response.Cookies.Add(cookie);

            Session["Username"] = user.Username;
            Session["UserRole"] = UserRoles.Admin;

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home", new { area = "Admin" });
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Login");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}

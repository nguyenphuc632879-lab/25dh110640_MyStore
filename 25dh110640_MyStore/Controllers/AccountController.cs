using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MyStore.Models;
using MyStore.Models.ViewModel;

namespace MyStore.Controllers
{
    public class AccountController : BaseController
    {
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterVM model)
        {
            if (!ModelState.IsValid) return View(model);

            // Kiểm tra tên đăng nhập đã tồn tại chưa
            if (db.Users.Any(u => u.Username == model.Username))
            {
                ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại");
                return View(model);
            }

            var user = new User
            {
                Username = model.Username,
                Password = model.Password,
                UserRole = UserRoles.Customer   // NCHAR(1): 'C'
            };
            db.Users.Add(user);

            var customer = new Customer
            {
                Username = model.Username,
                CustomerName = model.CustomerName,
                CustomerPhone = model.CustomerPhone,
                CustomerEmail = model.CustomerEmail,
                CustomerAddress = model.CustomerAddress
            };
            db.Customers.Add(customer);

            db.SaveChanges();
            SetAuthCookie(model.Username, UserRoles.Customer, false);

            return RedirectToAction("Index", "Home");
        }
        public JsonResult IsUsernameAvailable(string Username)
        {
            bool exists = db.Users.Any(u => u.Username == Username);
            return Json(!exists, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginVM model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }

            // Password là NCHAR(50) -> DB tự thêm khoảng trắng ở cuối, nên phải Trim khi so sánh
            var user = db.Users.FirstOrDefault(u =>
                u.Username == model.Username && u.Password.Trim() == model.Password.Trim() &&
                (u.UserRole.Trim() == UserRoles.Customer || u.UserRole.Trim() == UserRoles.Admin));

            if (user == null)
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng");
                ViewBag.ReturnUrl = returnUrl;
                return View(model);
            }
            SetAuthCookie(user.Username, user.UserRole.Trim(), model.RememberMe);

            Session["Username"] = user.Username;
            Session["UserRole"] = user.UserRole.Trim();

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Thông tin tài khoản

        [Authorize]
        [HttpGet]
        public ActionResult ProfileInfo()
        {
            var username = User.Identity.Name;
            var customer = db.Customers.FirstOrDefault(c => c.Username == username);
            if (customer == null) return HttpNotFound();

            var model = new ProfileVM
            {
                Username = customer.Username,
                CustomerName = customer.CustomerName,
                CustomerPhone = customer.CustomerPhone,
                CustomerEmail = customer.CustomerEmail,
                CustomerAddress = customer.CustomerAddress
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProfile(ProfileVM model)
        {
            var username = User.Identity.Name;
            var customer = db.Customers.FirstOrDefault(c => c.Username == username);
            if (customer == null) return HttpNotFound();

            // Đổi mật khẩu thì không cần validate các trường thông tin khác
            if (model.NewPassword == null)
            {
                if (!ModelState.IsValid) return View("ProfileInfo", model);
            }

            customer.CustomerName = model.CustomerName;
            customer.CustomerPhone = model.CustomerPhone;
            customer.CustomerEmail = model.CustomerEmail;
            customer.CustomerAddress = model.CustomerAddress;

            db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            TempData["Message"] = "Cập nhật thông tin thành công";
            return RedirectToAction("ProfileInfo");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ProfileVM model)
        {
            var username = User.Identity.Name;
            var user = db.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return HttpNotFound();

            if (string.IsNullOrEmpty(model.OldPassword) ||
                model.OldPassword.Trim() != user.Password.Trim())
            {
                TempData["Error"] = "Mật khẩu hiện tại không đúng";
                return RedirectToAction("ProfileInfo");
            }

            if (string.IsNullOrEmpty(model.NewPassword) || model.NewPassword.Length < 6)
            {
                TempData["Error"] = "Mật khẩu mới phải có ít nhất 6 ký tự";
                return RedirectToAction("ProfileInfo");
            }
            user.Password = model.NewPassword;
            db.Entry(user).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            TempData["Message"] = "Đổi mật khẩu thành công";
            return RedirectToAction("ProfileInfo");
        }
        private void SetAuthCookie(string username, string role, bool rememberMe)
        {
            var ticket = new FormsAuthenticationTicket(
                1,
                username,
                DateTime.Now,
                DateTime.Now.AddMinutes(FormsAuthentication.Timeout.TotalMinutes),
                rememberMe,
                role,                       
                FormsAuthentication.FormsCookiePath);

            var encrypted = FormsAuthentication.Encrypt(ticket);
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted)
            {
                HttpOnly = true,
                Expires = rememberMe ? ticket.Expiration : DateTime.MinValue
            };
            Response.Cookies.Add(cookie);
        }
    }
}

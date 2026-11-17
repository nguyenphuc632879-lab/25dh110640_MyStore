using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using _25dh110640_MyStore.Models;
using _25dh110640_MyStore.Models.ViewModel;

namespace _25dh110640_MyStore.Areas.Admin.Controllers
{
    public class UsersController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        public ActionResult Index(string searchTerm, int? page)
        {
            var users = db.Users.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                users = users.Where(u => u.Username.Contains(searchTerm) || u.UserRole.Contains(searchTerm));
            }

            users = users.OrderBy(u => u.Username);

            var model = new UserSearchVM();
            model.SearchTerm = searchTerm;
            model.PageSize = 5;
            model.PageNumber = page.HasValue && page.Value > 0 ? page.Value : 1;
            model.TotalItems = users.Count();
            model.Users = users.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();
            return View(model);
        }

        public ActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        public ActionResult Create()
        {
            return View(new User());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Username,Password,UserRole")] User user)
        {
            ValidateUser(user);
            if (!string.IsNullOrWhiteSpace(user.Username) && db.Users.Find(user.Username) != null)
            {
                ModelState.AddModelError("Username", "Tên tài khoản đã tồn tại");
            }

            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        public ActionResult Edit(string id)
        {
            return Details(id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Username,Password,UserRole")] User user)
        {
            ValidateUser(user);

            if (ModelState.IsValid)
            {
                db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        public ActionResult Delete(string id)
        {
            return Details(id);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            if (db.Customers.Any(c => c.Username == id))
            {
                ModelState.AddModelError("", "Không thể xóa tài khoản này vì đang gắn với một khách hàng.");
                return View(user);
            }
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Login()
        {
            return View(new LoginVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginVM model)
        {
            if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError("", "Vui lòng nhập tên tài khoản và mật khẩu");
                return View(model);
            }

            User user = db.Users.Find(model.Username);
            if (user == null || user.Password != model.Password ||
                !string.Equals(user.UserRole, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("", "Tên tài khoản hoặc mật khẩu không đúng");
                return View(model);
            }

            Session["AdminUser"] = user.Username;
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            Session.Remove("AdminUser");
            return RedirectToAction("Login");
        }

        private void ValidateUser(User user)
        {
            if (string.IsNullOrWhiteSpace(user.Username))
            {
                ModelState.AddModelError("Username", "Vui lòng nhập tên tài khoản");
            }
            if (string.IsNullOrWhiteSpace(user.Password))
            {
                ModelState.AddModelError("Password", "Vui lòng nhập mật khẩu");
            }
            if (string.IsNullOrWhiteSpace(user.UserRole))
            {
                ModelState.AddModelError("UserRole", "Vui lòng chọn vai trò");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

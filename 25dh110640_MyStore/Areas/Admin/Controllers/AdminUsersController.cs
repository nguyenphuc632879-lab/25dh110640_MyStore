using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using MyStore.Models;
using MyStore.Models.ViewModel;
using PagedList;

namespace MyStore.Areas.Admin.Controllers
{
    public class AdminUsersController : AdminBaseController
    {
        public ActionResult Index(AdminUserSearchVM model, int? page)
        {
            var users = db.Users.Where(u => u.UserRole == UserRoles.Admin).AsQueryable();

            if (!string.IsNullOrEmpty(model.SearchTerm))
                users = users.Where(u => u.Username.Contains(model.SearchTerm));

            users = model.SortOrder == "name_desc"
                ? users.OrderByDescending(u => u.Username)
                : users.OrderBy(u => u.Username);

            int pageNumber = page ?? (model.PageNumber <= 0 ? 1 : model.PageNumber);
            int pageSize = model.PageSize <= 0 ? 10 : model.PageSize;

            model.PageNumber = pageNumber;
            model.PageSize = pageSize;
            model.AdminUsers = users.ToPagedList(pageNumber, pageSize);

            return View(model);
        }
        public ActionResult Details(string id)
        {
            return GetUser(id);
        }

        private ActionResult GetUser(string id)
        {
            if (string.IsNullOrEmpty(id)) return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            User user = db.Users.Find(id);
            if (user == null) return HttpNotFound();
            return View(user);
        }
        public ActionResult Create()
        {
            return View(new User { UserRole = UserRoles.Admin });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Username,Password,UserRole")] User user)
        {
            if (db.Users.Any(u => u.Username == user.Username))
                ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại");

            if (ModelState.IsValid)
            {
                user.UserRole = UserRoles.Admin;
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Admin/AdminUsers/Edit/admin
        public ActionResult Edit(string id)
        {
            return GetUser(id);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Username,Password,UserRole")] User user)
        {
            if (ModelState.IsValid)
            {
                user.UserRole = UserRoles.Admin;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }
        public ActionResult Delete(string id)
        {
            return GetUser(id);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            var user = db.Users.Find(id);
            if (user == null) return HttpNotFound();
            if (user.Username == User.Identity.Name)
            {
                TempData["Error"] = "Không thể xoá tài khoản đang đăng nhập";
                return RedirectToAction("Index");
            }

            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}

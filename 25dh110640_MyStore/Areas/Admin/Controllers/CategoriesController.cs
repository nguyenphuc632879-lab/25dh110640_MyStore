using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using MyStore.Models;

namespace MyStore.Areas.Admin.Controllers
{
    /// <summary>
    /// BE#2 - Quản lý danh mục sản phẩm (CRUD)
    /// </summary>
    public class CategoriesController : AdminBaseController
    {
        // GET: Admin/Categories
        public ActionResult Index()
        {
            return View(db.Categories.ToList());
        }

        /// <summary>
        /// Hàm con dùng chung cho 3 action GET: Details, Edit, Delete (BE#2 - tối ưu hóa code)
        /// </summary>
        private ActionResult GetCategory(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            Category category = db.Categories.Find(id);
            if (category == null) return HttpNotFound();
            return View(category);
        }

        // GET: Admin/Categories/Details/5
        public ActionResult Details(int? id)
        {
            return GetCategory(id);
        }

        // GET: Admin/Categories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CategoryID,CategoryName")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // GET: Admin/Categories/Edit/5
        public ActionResult Edit(int? id)
        {
            // Tối ưu: dùng lại hàm con GetCategory (BE#2 - slide 13)
            return GetCategory(id);
        }

        // POST: Admin/Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CategoryID,CategoryName")] Category category)
        {
            if (ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // GET: Admin/Categories/Delete/5
        public ActionResult Delete(int? id)
        {
            // Tối ưu: dùng lại hàm con GetCategory
            return GetCategory(id);
        }

        // POST: Admin/Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = db.Categories.Find(id);
            if (category == null) return HttpNotFound();

            if (category.Products != null && category.Products.Any())
            {
                TempData["Error"] = "Không thể xoá danh mục đang có sản phẩm";
                return RedirectToAction("Index");
            }

            db.Categories.Remove(category);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}

using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyStore.Models;
using MyStore.Models.ViewModel;
using PagedList;

namespace MyStore.Areas.Admin.Controllers
{
    public class ProductsController : AdminBaseController
    {
        #region Index: tìm kiếm + sắp xếp + phân trang

        public ActionResult Index(ProductSearchVM model, int? page)
        {
            var products = db.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrEmpty(model.SearchTerm))
            {
                products = products.Where(p =>
                    p.ProductName.Contains(model.SearchTerm) ||
                    (p.ProductDescription != null && p.ProductDescription.Contains(model.SearchTerm)) ||
                    (p.Category != null && p.Category.CategoryName.Contains(model.SearchTerm)));
            }

            if (model.MinPrice.HasValue)
                products = products.Where(p => p.ProductPrice >= model.MinPrice.Value);

            if (model.MaxPrice.HasValue)
                products = products.Where(p => p.ProductPrice <= model.MaxPrice.Value);

            switch (model.SortOrder)
            {
                case "name_desc": products = products.OrderByDescending(p => p.ProductName); break;
                case "price": products = products.OrderBy(p => p.ProductPrice); break;
                case "price_desc": products = products.OrderByDescending(p => p.ProductPrice); break;
                default: products = products.OrderBy(p => p.ProductName); break;
            }

            int pageNumber = Math.Max(page ?? (model.PageNumber <= 0 ? 1 : model.PageNumber), 1);
            int pageSize = model.PageSize <= 0 ? 6 : model.PageSize;

            model.PageNumber = pageNumber;
            model.PageSize = pageSize;
            model.Products = products.ToPagedList(pageNumber, pageSize);

            return View(model);
        }

        #endregion

        #region Details

        public ActionResult Details(int? id)
        {
            return GetProduct(id);
        }

        private ActionResult GetProduct(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            Product product = db.Products.Include(p => p.Category).FirstOrDefault(p => p.ProductID == id);
            if (product == null) return HttpNotFound();
            return View(product);
        }

        #endregion

        #region Create

        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,CategoryID,ProductName,ProductDescription,ProductPrice,ProductImage,UploadImg")] Product product)
        {
            if (ModelState.IsValid)
            {
                SaveUploadedImage(product);

                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        #endregion

        #region Edit

        public ActionResult Edit(int? id)
        {
            var result = GetProduct(id);
            if (result is ViewResult)
                ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", ((Product)((ViewResult)result).Model).CategoryID);
            return result;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,CategoryID,ProductName,ProductDescription,ProductPrice,ProductImage,UploadImg")] Product product)
        {
            if (ModelState.IsValid)
            {
                SaveUploadedImage(product);

                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        #endregion

        #region Delete

        public ActionResult Delete(int? id)
        {
            return GetProduct(id);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            if (product == null) return HttpNotFound();

            if (db.OrderDetails.Any(d => d.ProductID == id))
            {
                TempData["Error"] = "Sản phẩm đã có trong đơn hàng, không thể xoá";
                return RedirectToAction("Index");
            }

            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        #endregion

        #region Upload ảnh

        private void SaveUploadedImage(Product product)
        {
            if (product.UploadImg != null && product.UploadImg.ContentLength > 0)
            {
                string filename = Path.GetFileName(product.UploadImg.FileName);
                string savePath = "~/Content/images/";
                product.ProductImage = savePath + filename;
                product.UploadImg.SaveAs(Path.Combine(Server.MapPath(savePath), filename));
            }
            else if (string.IsNullOrEmpty(product.ProductImage))
            {
                product.ProductImage = "~/Content/images/default.png";
            }
        }

        #endregion
    }
}

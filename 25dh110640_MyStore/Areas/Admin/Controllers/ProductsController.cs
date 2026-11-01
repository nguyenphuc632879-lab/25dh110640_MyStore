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
    /// <summary>
    /// BE#3 - Quản lý sản phẩm (CRUD) + BE#4 - Tìm kiếm, sắp xếp, phân trang
    /// </summary>
    public class ProductsController : AdminBaseController
    {
        #region Index: tìm kiếm + sắp xếp + phân trang

        // GET: Admin/Products
        public ActionResult Index(ProductSearchVM model, int? page)
        {
            var products = db.Products.Include(p => p.Category).AsQueryable();

            // 4.1 Tìm kiếm theo từ khoá (tên, mô tả hoặc tên danh mục)
            if (!string.IsNullOrEmpty(model.SearchTerm))
            {
                products = products.Where(p =>
                    p.ProductName.Contains(model.SearchTerm) ||
                    (p.ProductDescription != null && p.ProductDescription.Contains(model.SearchTerm)) ||
                    (p.Category != null && p.Category.CategoryName.Contains(model.SearchTerm)));
            }

            // 4.2 Tìm kiếm nâng cao theo khoảng giá
            if (model.MinPrice.HasValue)
                products = products.Where(p => p.ProductPrice >= model.MinPrice.Value);

            if (model.MaxPrice.HasValue)
                products = products.Where(p => p.ProductPrice <= model.MaxPrice.Value);

            // 4.3 Sắp xếp kết quả
            switch (model.SortOrder)
            {
                case "name_desc": products = products.OrderByDescending(p => p.ProductName); break;
                case "price": products = products.OrderBy(p => p.ProductPrice); break;
                case "price_desc": products = products.OrderByDescending(p => p.ProductPrice); break;
                default: products = products.OrderBy(p => p.ProductName); break;   // name
            }

            // 4.4 Phân trang
            int pageNumber = page ?? (model.PageNumber <= 0 ? 1 : model.PageNumber);
            int pageSize = model.PageSize <= 0 ? 6 : model.PageSize;

            model.PageNumber = pageNumber;
            model.PageSize = pageSize;
            model.Products = products.ToPagedList(pageNumber, pageSize);

            return View(model);
        }

        #endregion

        #region Details

        // GET: Admin/Products/Details/5
        public ActionResult Details(int? id)
        {
            return GetProduct(id);
        }

        /// <summary>
        /// Hàm con dùng chung cho Details, Edit, Delete (tối ưu hóa code)
        /// </summary>
        private ActionResult GetProduct(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            Product product = db.Products.Include(p => p.Category).FirstOrDefault(p => p.ProductID == id);
            if (product == null) return HttpNotFound();
            return View(product);
        }

        #endregion

        #region Create

        // GET: Admin/Products/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            return View();
        }

        // POST: Admin/Products/Create
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

        // GET: Admin/Products/Edit/5
        public ActionResult Edit(int? id)
        {
            var result = GetProduct(id);
            if (result is ViewResult)
                ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", ((Product)((ViewResult)result).Model).CategoryID);
            return result;
        }

        // POST: Admin/Products/Edit/5
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

        // GET: Admin/Products/Delete/5
        public ActionResult Delete(int? id)
        {
            return GetProduct(id);
        }

        // POST: Admin/Products/Delete/5
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

        #region Upload ảnh (BE#3 - 3.4)

        /// <summary>
        /// Lưu ảnh upload vào thư mục ~/Content/images
        /// </summary>
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

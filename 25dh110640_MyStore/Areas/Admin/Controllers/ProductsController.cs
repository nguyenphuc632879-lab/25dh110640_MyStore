using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using _25dh110640_MyStore.Models;
using _25dh110640_MyStore.Models.ViewModel;

namespace _25dh110640_MyStore.Areas.Admin.Controllers
{
    public class ProductsController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Admin/Products
        public ActionResult Index(string searchTerm, decimal? minPrice, decimal? maxPrice, string sortOrder, int? page)
        {
            var model = new ProductSearchVM();
            var products = db.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                products = products.Where(p =>
                    p.ProductName.Contains(searchTerm) ||
                    p.ProductDecription.Contains(searchTerm) ||
                    p.Category.CategoryName.Contains(searchTerm));
            }

            if (minPrice.HasValue)
            {
                products = products.Where(p => p.ProductPrice >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                products = products.Where(p => p.ProductPrice <= maxPrice.Value);
            }

            switch (sortOrder)
            {
                case "name_asc":
                    products = products.OrderBy(p => p.ProductName);
                    break;
                case "name_desc":
                    products = products.OrderByDescending(p => p.ProductName);
                    break;
                case "price_asc":
                    products = products.OrderBy(p => p.ProductPrice);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(p => p.ProductPrice);
                    break;
                default:
                    products = products.OrderBy(p => p.ProductName);
                    break;
            }

            model.SearchTerm = searchTerm;
            model.MinPrice = minPrice;
            model.MaxPrice = maxPrice;
            model.SortOrder = sortOrder;
            model.PageSize = 5;
            model.PageNumber = page.HasValue && page.Value > 0 ? page.Value : 1;
            model.TotalItems = products.Count();
            model.Products = products.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();
            return View(model);
        }

        // GET: Admin/Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Admin/Products/Create
        public ActionResult Create()
        {
            LoadCategories();
            return View();
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,CategoryID,ProductName,ProductDecription,ProductPrice,ProductImage,UploadImg")] Product product)
        {
            ValidateProduct(product);

            if (ModelState.IsValid)
            {
                SaveProductImage(product);
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            LoadCategories(product.CategoryID);
            return View(product);
        }

        // GET: Admin/Products/Edit/5
        public ActionResult Edit(int? id)
        {
            ActionResult result = Details(id);
            ViewResult viewResult = result as ViewResult;
            if (viewResult != null)
            {
                LoadCategories(((Product)viewResult.Model).CategoryID);
            }
            return result;
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,CategoryID,ProductName,ProductDecription,ProductPrice,ProductImage,UploadImg")] Product product)
        {
            ValidateProduct(product);

            if (ModelState.IsValid)
            {
                SaveProductImage(product);
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            LoadCategories(product.CategoryID);
            return View(product);
        }

        // GET: Admin/Products/Delete/5
        public ActionResult Delete(int? id)
        {
            return Details(id);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            if (db.OrderDetails.Any(o => o.ProductID == id))
            {
                ModelState.AddModelError("", "Không thể xóa sản phẩm này vì đã có trong đơn hàng.");
                return View(product);
            }
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private void LoadCategories(int? selectedCategoryID = null)
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", selectedCategoryID);
        }

        private void ValidateProduct(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.ProductName))
            {
                ModelState.AddModelError("ProductName", "Vui lòng nhập tên sản phẩm");
            }
            if (string.IsNullOrWhiteSpace(product.ProductDecription))
            {
                ModelState.AddModelError("ProductDecription", "Vui lòng nhập mô tả");
            }
        }

        private void SaveProductImage(Product product)
        {
            if (product.UploadImg != null && product.UploadImg.ContentLength > 0)
            {
                string fileName = Path.GetFileName(product.UploadImg.FileName);
                string savePath = "~/Content/images/";
                string folder = Server.MapPath(savePath);
                Directory.CreateDirectory(folder);
                product.UploadImg.SaveAs(Path.Combine(folder, fileName));
                product.ProductImage = savePath + fileName;
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

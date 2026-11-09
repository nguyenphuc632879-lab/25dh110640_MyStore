using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using MyStore.Models;
using MyStore.Models.ViewModel;
using PagedList;

namespace MyStore.Controllers
{
    public class HomeController : BaseController
    {
        private Dictionary<int, int> GetSoldCounts()
        {
            return db.OrderDetails
                     .Where(d => d.Order.PaymentStatus == null || d.Order.PaymentStatus != "Đã huỷ")
                     .GroupBy(d => d.ProductID)
                     .Select(g => new { ProductID = g.Key, Sold = g.Sum(d => d.Quantity) })
                     .ToDictionary(x => x.ProductID, x => x.Sold);
        }

        private static int SoldOf(Dictionary<int, int> sold, int productId)
        {
            int n;
            return sold.TryGetValue(productId, out n) ? n : 0;
        }

        public ActionResult Index(string searchTerm, int? page)
        {
            var model = new HomeProductVM();
            model.SearchTerm = searchTerm;

            var products = db.Products.Include(x => x.Category).AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                products = products.Where(p =>
                    p.ProductName.Contains(searchTerm) ||
                    (p.ProductDescription != null && p.ProductDescription.Contains(searchTerm)) ||
                    (p.Category != null && p.Category.CategoryName.Contains(searchTerm)));
            }

            var matched = products.OrderBy(p => p.ProductID).ToList();
            var sold = GetSoldCounts();

            model.FeaturedProducts = matched
                .OrderByDescending(p => SoldOf(sold, p.ProductID))
                .ThenBy(p => p.ProductID)
                .Take(10)
                .ToList();

            int pageNumber = Math.Max(page ?? 1, 1);
            model.NewProducts = matched
                .OrderBy(p => SoldOf(sold, p.ProductID))
                .ThenBy(p => p.ProductID)
                .Take(20)
                .ToPagedList(pageNumber, 6);

            return View(model);
        }

        public ActionResult ProductList(int? id, string searchTerm, decimal? minPrice, decimal? maxPrice, string sortOrder, int? page)
        {
            var model = new ProductSearchVM
            {
                CategoryID = id,
                SearchTerm = searchTerm,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                SortOrder = sortOrder,
                PageSize = 6
            };

            var products = db.Products.Include(p => p.Category).AsQueryable();

            if (id.HasValue)
                products = products.Where(p => p.CategoryID == id.Value);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                products = products.Where(p =>
                    p.ProductName.Contains(searchTerm) ||
                    (p.ProductDescription != null && p.ProductDescription.Contains(searchTerm)));
            }

            if (minPrice.HasValue) products = products.Where(p => p.ProductPrice >= minPrice.Value);
            if (maxPrice.HasValue) products = products.Where(p => p.ProductPrice <= maxPrice.Value);

            switch (sortOrder)
            {
                case "name": products = products.OrderBy(p => p.ProductName); break;
                case "name_desc": products = products.OrderByDescending(p => p.ProductName); break;
                case "price": products = products.OrderBy(p => p.ProductPrice); break;
                case "price_desc": products = products.OrderByDescending(p => p.ProductPrice); break;
                default: products = products.OrderBy(p => p.ProductID); break;
            }

            ViewBag.CategoryName = "Tất cả sản phẩm";
            if (id.HasValue)
            {
                var cat = db.Categories.Find(id.Value);
                if (cat != null) ViewBag.CategoryName = cat.CategoryName;
            }

            int pageNumber = Math.Max(page ?? 1, 1);
            model.Products = products.ToPagedList(pageNumber, model.PageSize);

            return View(model);
        }

        [HttpGet]
        public ActionResult ProductDetail(int id, int quantity = 1, int? page = 1)
        {
            var product = db.Products.Include(p => p.Category).FirstOrDefault(p => p.ProductID == id);
            if (product == null) return HttpNotFound();

            var sold = GetSoldCounts();

            var model = new ProductDetailVM
            {
                product = product,
                quantity = quantity < 1 ? 1 : quantity,
                SoldCount = SoldOf(sold, product.ProductID)
            };

            model.RelatedProducts = db.Products
                .Where(p => p.CategoryID == product.CategoryID && p.ProductID != product.ProductID)
                .OrderBy(p => p.ProductID)
                .Take(8).ToList();

            var top = db.Products
                .Where(p => p.CategoryID == product.CategoryID)
                .ToList()
                .OrderByDescending(p => SoldOf(sold, p.ProductID))
                .ThenBy(p => p.ProductID)
                .Take(8)
                .ToList();

            model.TopProducts = top.ToPagedList(Math.Max(page ?? 1, 1), 3);

            return View(model);
        }

        [HttpGet]
        public ActionResult Search(string searchTerm)
        {
            return RedirectToAction("Index", new { searchTerm = searchTerm });
        }

        [ChildActionOnly]
        public ActionResult CategoryMenuPV()
        {
            var categories = db.Categories.ToList();
            return PartialView("_CategoriesPV", categories);
        }
    }
}

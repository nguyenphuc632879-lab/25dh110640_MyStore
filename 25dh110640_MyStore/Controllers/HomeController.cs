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
        /// <summary>
        /// Số lượng đã bán của từng sản phẩm (không tính đơn đã huỷ): ProductID -> tổng số lượng
        /// </summary>
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

        /// <summary>
        /// Trang chủ: banner + tìm kiếm + sản phẩm nổi bật (top 10 bán chạy nhất, dạng slider)
        /// + danh sách 20 sản phẩm ít người mua nhất (phân trang 6 sản phẩm/trang).
        /// Khi tìm kiếm (tên, mô tả hoặc danh mục), kết quả hiển thị đồng thời ở CẢ HAI phần.
        /// </summary>
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

            // Sản phẩm nổi bật: top 10 có nhiều người mua nhất (trong kết quả tìm kiếm)
            model.FeaturedProducts = matched
                .OrderByDescending(p => SoldOf(sold, p.ProductID))
                .ThenBy(p => p.ProductID)
                .Take(10)
                .ToList();

            // Danh sách sản phẩm: 20 sản phẩm có ít người mua nhất, phân trang 6 sản phẩm / trang
            int pageNumber = page ?? 1;
            model.NewProducts = matched
                .OrderBy(p => SoldOf(sold, p.ProductID))
                .ThenBy(p => p.ProductID)
                .Take(20)
                .ToPagedList(pageNumber, 6);

            return View(model);
        }

        /// <summary>
        /// Trang danh sách sản phẩm theo danh mục (có tìm kiếm, lọc khoảng giá, sắp xếp, phân trang)
        /// </summary>
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

            int pageNumber = page ?? 1;
            model.Products = products.ToPagedList(pageNumber, model.PageSize);

            return View(model);
        }

        /// <summary>
        /// Trang chi tiết sản phẩm: thông tin sản phẩm (kèm số lượng đã bán) + sản phẩm tương tự
        /// + Top Deals (bán chạy nhất cùng danh mục, phân trang 3 sản phẩm/trang)
        /// </summary>
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

            // 8 sản phẩm cùng danh mục
            model.RelatedProducts = db.Products
                .Where(p => p.CategoryID == product.CategoryID && p.ProductID != product.ProductID)
                .OrderBy(p => p.ProductID)
                .Take(8).ToList();

            // 8 sản phẩm bán chạy nhất cùng danh mục, phân trang 3 sản phẩm / trang
            var top = db.Products
                .Where(p => p.CategoryID == product.CategoryID)
                .ToList()
                .OrderByDescending(p => SoldOf(sold, p.ProductID))
                .ThenBy(p => p.ProductID)
                .Take(8)
                .ToList();

            model.TopProducts = top.ToPagedList(page ?? 1, 3);

            return View(model);
        }

        /// <summary>
        /// Tìm kiếm phía Customer (kết quả hiển thị lại trên trang Home)
        /// </summary>
        [HttpGet]
        public ActionResult Search(string searchTerm)
        {
            return RedirectToAction("Index", new { searchTerm = searchTerm });
        }

        /// <summary>
        /// Partial View: menu danh mục sản phẩm trên Header (_CategoriesPV.cshtml)
        /// </summary>
        [ChildActionOnly]
        public ActionResult CategoryMenuPV()
        {
            var categories = db.Categories.ToList();
            return PartialView("_CategoriesPV", categories);
        }
    }
}

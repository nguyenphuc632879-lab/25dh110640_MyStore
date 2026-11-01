using System;
using System.Linq;
using System.Web.Mvc;

namespace MyStore.Areas.Admin.Controllers
{
    /// <summary>
    /// Trang Home của Admin: thống kê (dùng Google Chart)
    /// </summary>
    public class HomeController : AdminBaseController
    {
        public ActionResult Index()
        {
            ViewBag.TotalCategories = db.Categories.Count();
            ViewBag.TotalProducts = db.Products.Count();
            ViewBag.TotalCustomers = db.Customers.Count();
            ViewBag.TotalOrders = db.Orders.Count();
            ViewBag.TotalRevenue = db.Orders.Any() ? db.Orders.Sum(o => (decimal?)o.TotalAmount) ?? 0 : 0;
            ViewBag.PendingOrders = db.Orders.Count(o => o.PaymentStatus == "Chờ thanh toán");

            return View();
        }

        /// <summary>
        /// Dữ liệu JSON cho Google Chart: doanh thu theo 6 tháng gần nhất
        /// </summary>
        public JsonResult GetRevenueChartData()
        {
            var data = db.Orders
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Revenue = g.Sum(o => (decimal?)o.TotalAmount) ?? 0
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToList()
                .Select(x => new { label = x.Month + "/" + x.Year, value = x.Revenue })
                .ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Dữ liệu JSON: số lượng sản phẩm theo danh mục (PieChart)
        /// </summary>
        public JsonResult GetCategoryChartData()
        {
            var data = db.Categories
                .Select(c => new { label = c.CategoryName, value = c.Products.Count })
                .ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Dữ liệu JSON: 5 sản phẩm bán chạy nhất
        /// </summary>
        public JsonResult GetTopProductChartData()
        {
            var data = db.OrderDetails
                .GroupBy(d => d.Product.ProductName)
                .Select(g => new { label = g.Key, value = g.Sum(d => (int?)d.Quantity) ?? 0 })
                .OrderByDescending(x => x.value)
                .Take(5)
                .ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}

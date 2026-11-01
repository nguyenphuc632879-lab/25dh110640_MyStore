using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using MyStore.Models;
using PagedList;

namespace MyStore.Areas.Admin.Controllers
{
    /// <summary>
    /// Quản lý đơn hàng: xem danh sách, chi tiết, cập nhật trạng thái
    /// </summary>
    public class OrdersController : AdminBaseController
    {
        // GET: Admin/Orders
        public ActionResult Index(string searchTerm, string status, int? page)
        {
            var orders = db.Orders.Include(o => o.Customer).AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                int orderId;
                bool isNumber = int.TryParse(searchTerm, out orderId);
                orders = orders.Where(o =>
                    (isNumber && o.OrderID == orderId) ||
                    (o.Customer != null && o.Customer.CustomerName.Contains(searchTerm)));
            }

            if (!string.IsNullOrEmpty(status) && status != "Tất cả")
                orders = orders.Where(o => o.PaymentStatus == status);

            ViewBag.SearchTerm = searchTerm;
            ViewBag.Status = string.IsNullOrEmpty(status) ? "Tất cả" : status;
            ViewBag.StatusList = new[] { "Tất cả", "Chờ thanh toán", "Đang xử lý", "Đang vận chuyển", "Đã giao", "Đã huỷ" };

            var model = orders.OrderByDescending(o => o.OrderDate).ToPagedList(page ?? 1, 10);
            return View(model);
        }

        // GET: Admin/Orders/Details/5
        public ActionResult Details(int id)
        {
            var order = db.Orders.Include("OrderDetails.Product")
                                 .Include("Customer")
                                 .FirstOrDefault(o => o.OrderID == id);
            if (order == null) return HttpNotFound();

            return View(order);
        }

        // POST: Admin/Orders/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateStatus(int orderId, string status)
        {
            var order = db.Orders.Find(orderId);
            if (order == null) return HttpNotFound();

            order.PaymentStatus = status;
            db.Entry(order).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Details", new { id = orderId });
        }

        // POST: Admin/Orders/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var order = db.Orders.Find(id);
            if (order == null) return HttpNotFound();

            var details = db.OrderDetails.Where(d => d.OrderID == id).ToList();
            foreach (var d in details) db.OrderDetails.Remove(d);

            db.Orders.Remove(order);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}

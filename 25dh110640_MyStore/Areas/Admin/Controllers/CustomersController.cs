using System;
using System.Linq;
using System.Web.Mvc;
using MyStore.Models;
using MyStore.Models.ViewModel;
using PagedList;

namespace MyStore.Areas.Admin.Controllers
{
    /// <summary>
    /// BE#5 - Quản lý tài khoản khách hàng và đơn hàng
    /// Xem, tìm kiếm, sắp xếp, phân trang tài khoản
    /// </summary>
    public class CustomersController : AdminBaseController
    {
        // GET: Admin/Customers
        public ActionResult Index(CustomerSearchVM model, int? page)
        {
            var customers = db.Customers.Include(c => c.User).AsQueryable();

            if (!string.IsNullOrEmpty(model.SearchTerm))
            {
                customers = customers.Where(c =>
                    c.CustomerName.Contains(model.SearchTerm) ||
                    c.Username.Contains(model.SearchTerm) ||
                    (c.CustomerPhone != null && c.CustomerPhone.Contains(model.SearchTerm)) ||
                    (c.CustomerEmail != null && c.CustomerEmail.Contains(model.SearchTerm)));
            }

            switch (model.SortOrder)
            {
                case "name_desc": customers = customers.OrderByDescending(c => c.CustomerName); break;
                // CSDL không có cột ngày sinh -> sắp theo mã khách hàng (mới nhất / cũ nhất)
                case "date": customers = customers.OrderBy(c => c.CustomerID); break;
                case "date_desc": customers = customers.OrderByDescending(c => c.CustomerID); break;
                default: customers = customers.OrderBy(c => c.CustomerName); break;
            }

            int pageNumber = page ?? (model.PageNumber <= 0 ? 1 : model.PageNumber);
            int pageSize = model.PageSize <= 0 ? 10 : model.PageSize;

            model.PageNumber = pageNumber;
            model.PageSize = pageSize;
            model.Customers = customers.ToPagedList(pageNumber, pageSize);

            return View(model);
        }

        // GET: Admin/Customers/Details/5
        public ActionResult Details(int id)
        {
            var customer = db.Customers.Include(c => c.User).FirstOrDefault(c => c.CustomerID == id);
            if (customer == null) return HttpNotFound();

            ViewBag.Orders = db.Orders.Where(o => o.CustomerID == id)
                                      .OrderByDescending(o => o.OrderDate).ToList();

            return View(customer);
        }

        // GET: Admin/Customers/Orders/5 - danh sách đơn hàng của 1 khách hàng
        public ActionResult Orders(int id, int? page)
        {
            var customer = db.Customers.Find(id);
            if (customer == null) return HttpNotFound();

            ViewBag.Customer = customer;

            var orders = db.Orders.Where(o => o.CustomerID == id)
                                  .OrderByDescending(o => o.OrderDate)
                                  .ToPagedList(page ?? 1, 10);

            return View(orders);
        }

        // GET: Admin/Customers/OrderDetails/5 - chi tiết 1 đơn hàng
        public ActionResult OrderDetails(int id)
        {
            var order = db.Orders.Include("OrderDetails.Product")
                                 .Include("Customer")
                                 .FirstOrDefault(o => o.OrderID == id);
            if (order == null) return HttpNotFound();

            return View(order);
        }

        // POST: Admin/Customers/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var customer = db.Customers.Find(id);
            if (customer == null) return HttpNotFound();

            if (db.Orders.Any(o => o.CustomerID == id))
            {
                TempData["Error"] = "Khách hàng đã có đơn hàng, không thể xoá";
                return RedirectToAction("Index");
            }

            // Xoá cả tài khoản đăng nhập tương ứng
            var user = db.Users.FirstOrDefault(u => u.Username == customer.Username);
            db.Customers.Remove(customer);
            if (user != null) db.Users.Remove(user);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}

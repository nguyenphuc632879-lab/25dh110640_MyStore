using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using _25dh110640_MyStore.Models;
using _25dh110640_MyStore.Models.ViewModel;

namespace _25dh110640_MyStore.Areas.Admin.Controllers
{
    public class OrdersController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        public ActionResult Index(int? customerId, string searchTerm, int? page)
        {
            var orders = db.Orders.Include(o => o.Customer).AsQueryable();
            var model = new OrderListVM();

            if (customerId.HasValue)
            {
                orders = orders.Where(o => o.CustomerID == customerId.Value);
                Customer customer = db.Customers.Find(customerId.Value);
                model.CustomerID = customerId;
                model.CustomerName = customer != null ? customer.CustomerName : null;
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                orders = orders.Where(o =>
                    o.Customer.CustomerName.Contains(searchTerm) ||
                    o.PaymentStatus.Contains(searchTerm) ||
                    o.AddressDelivery.Contains(searchTerm));
            }

            orders = orders.OrderByDescending(o => o.OrderDate).ThenByDescending(o => o.OrderID);

            model.SearchTerm = searchTerm;
            model.PageSize = 5;
            model.PageNumber = page.HasValue && page.Value > 0 ? page.Value : 1;
            model.TotalItems = orders.Count();
            model.Orders = orders.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();
            return View(model);
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails.Select(d => d.Product))
                .SingleOrDefault(o => o.OrderID == id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
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

using System.Linq;
using System.Net;
using System.Web.Mvc;
using _25dh110640_MyStore.Models;
using _25dh110640_MyStore.Models.ViewModel;

namespace _25dh110640_MyStore.Areas.Admin.Controllers
{
    public class CustomersController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        public ActionResult Index(string searchTerm, string sortOrder, int? page)
        {
            var customers = db.Customers.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                customers = customers.Where(c =>
                    c.CustomerName.Contains(searchTerm) ||
                    c.CustomerEmail.Contains(searchTerm) ||
                    c.CustomerPhone.Contains(searchTerm) ||
                    c.Username.Contains(searchTerm));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    customers = customers.OrderByDescending(c => c.CustomerName);
                    break;
                case "id_asc":
                    customers = customers.OrderBy(c => c.CustomerID);
                    break;
                case "id_desc":
                    customers = customers.OrderByDescending(c => c.CustomerID);
                    break;
                default:
                    customers = customers.OrderBy(c => c.CustomerName);
                    break;
            }

            var model = new CustomerSearchVM();
            model.SearchTerm = searchTerm;
            model.SortOrder = sortOrder;
            model.PageSize = 5;
            model.PageNumber = page.HasValue && page.Value > 0 ? page.Value : 1;
            model.TotalItems = customers.Count();
            model.Customers = customers.Skip((model.PageNumber - 1) * model.PageSize).Take(model.PageSize).ToList();
            return View(model);
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
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

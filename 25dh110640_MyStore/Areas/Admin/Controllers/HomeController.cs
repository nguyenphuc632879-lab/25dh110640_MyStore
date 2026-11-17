using System.Linq;
using System.Web.Mvc;
using _25dh110640_MyStore.Models;
using _25dh110640_MyStore.Models.ViewModel;

namespace _25dh110640_MyStore.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        public ActionResult Index()
        {
            var stats = db.Categories.Select(c => new CategoryStatVM
            {
                CategoryName = c.CategoryName,
                Quantity = c.Products.Count(),
                MaxPrice = c.Products.Max(p => (decimal?)p.ProductPrice) ?? 0,
                MinPrice = c.Products.Min(p => (decimal?)p.ProductPrice) ?? 0,
                AvgPrice = c.Products.Average(p => (decimal?)p.ProductPrice) ?? 0
            }).ToList();

            return View(stats);
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

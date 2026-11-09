using System;
using System.Web.Mvc;
using MyStore.Filters;
using MyStore.Models;

namespace MyStore.Areas.Admin.Controllers
{
    [AuthorizeRole(UserRoles.Admin)]
    public class AdminBaseController : Controller
    {
        protected MyStoreEntities db = new MyStoreEntities();

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}

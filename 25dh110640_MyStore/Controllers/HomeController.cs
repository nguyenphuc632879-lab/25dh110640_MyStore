using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _25dh110640_MyStore.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        // Các đường dẫn được giao diện hiện tại sử dụng.
        // Khi project chưa có View riêng cho danh mục/chi tiết, chuyển về trang chủ an toàn.
        public ActionResult PageDienThoai()
        {
            return RedirectToAction("Index");
        }

        public ActionResult PagePhuKien()
        {
            return RedirectToAction("Index");
        }

        public ActionResult PageThietBi()
        {
            return RedirectToAction("Index");
        }

        public ActionResult ProductDetails(int id)
        {
            return RedirectToAction("Index");
        }
    }
}
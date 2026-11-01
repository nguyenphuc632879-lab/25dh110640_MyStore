using System;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using MyStore.Models;
using MyStore.Models.ViewModel;

namespace MyStore.Controllers
{
    /// <summary>
    /// BE#8 - Thanh toán bằng PayPal (OPTIONAL).
    ///
    /// Để chạy thật: vào Tools > NuGet Package Manager > Package Manager Console chạy
    ///     Install-Package PayPal
    /// rồi thay phần demo bên dưới bằng SDK của PayPal và điền ClientId/ClientSecret trong Web.config.
    ///
    /// Phiên bản này mô phỏng luồng thanh toán: tạo đơn hàng với trạng thái "Đã thanh toán"
    /// sau khi người dùng xác nhận trên trang PayPal (giả lập).
    /// </summary>
    public class PaypalController : BaseController
    {
        [Authorize]
        public ActionResult PaymentWithPaypal()
        {
            var model = Session["PendingOrder"] as CheckoutVM;
            if (model == null) return RedirectToAction("Index", "Home");

            ViewBag.PayPalClientId = ConfigurationManager.AppSettings["PayPalClientId"];
            ViewBag.PayPalMode = ConfigurationManager.AppSettings["PayPalMode"];

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PaymentSuccess()
        {
            var model = Session["PendingOrder"] as CheckoutVM;
            var service = new CartService(Session);
            var cart = service.GetCart();

            if (model == null || cart == null || !cart.Items.Any() || model.CartItems == null || !model.CartItems.Any())
                return RedirectToAction("Index", "Home");

            var user = db.Users.SingleOrDefault(u => u.Username == User.Identity.Name);
            var customer = user != null ? db.Customers.SingleOrDefault(c => c.Username == user.Username) : null;
            if (customer == null) return RedirectToAction("Login", "Account");

            var order = new Order
            {
                CustomerID = customer.CustomerID,
                OrderDate = DateTime.Now,
                TotalAmount = model.CartItems.Sum(i => i.TotalPrice),
                PaymentStatus = "Đang xử lý",        // đã thanh toán qua PayPal
                ShippingAddress = model.ShippingAddress,
                DeliveryMethod = model.DeliveryMethod,
                PaymentMethod = "PayPal"
            };

            foreach (var item in model.CartItems)
            {
                order.OrderDetails.Add(new OrderDetail
                {
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    UnitPrice = item.ProductPrice
                });
            }

            db.Orders.Add(order);
            db.SaveChanges();

            Session["PendingOrder"] = null;
            cart.RemoveItems(model.CartItems.Select(i => i.ProductID).ToList());
            service.SetCart(cart);

            return RedirectToAction("OrderSuccess", "Order", new { id = order.OrderID });
        }

        public ActionResult PaymentCancel()
        {
            Session["PendingOrder"] = null;
            return RedirectToAction("Checkout", "Order");
        }
    }
}

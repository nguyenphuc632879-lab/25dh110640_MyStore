using _25dh110640_MyStore.Models;
using _25dh110640_MyStore.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace _25dh110640_MyStore.Controllers
{
    public class OrderController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        public ActionResult Checkout()
        {
            if (Session["Username"] == null)
                return RedirectToAction("Login", "Account");

            var cart = new CartService(Session).GetCart();
            if (cart.Items == null || !cart.Items.Any())
                return RedirectToAction("Index", "Cart");

            string username = Session["Username"].ToString();
            var customer = db.Customers.SingleOrDefault(c => c.Username == username);
            if (customer == null)
                return RedirectToAction("ProfileInfo", "Account");

            var cartItems = cart.Items.Select(i => new CartItem
            {
                ProductID = i.ProductID,
                ProductName = i.ProductName,
                ProductImage = i.ProductImage,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity,
                Category = i.Category
            }).ToList();

            var model = new CheckoutVM
            {
                CartItems = cartItems,
                TotalAmount = cartItems.Sum(item => item.TotalPrice),
                OrderDate = DateTime.Now,
                ShippingAddress = customer.CustomerAddress,
                CustomerID = customer.CustomerID,
                Username = customer.Username
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(CheckoutVM model)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Login", "Account");

            var cart = new CartService(Session).GetCart();
            if (cart.Items == null || !cart.Items.Any())
                return RedirectToAction("Index", "Cart");

            string username = Session["Username"].ToString();
            var customer = db.Customers.SingleOrDefault(c => c.Username == username);
            if (customer == null)
                return RedirectToAction("ProfileInfo", "Account");

            // Không lấy giá/tổng tiền từ trình duyệt; dùng dữ liệu giỏ hàng trong Session.
            var cartItems = cart.Items.Select(i => new CartItem
            {
                ProductID = i.ProductID,
                ProductName = i.ProductName,
                ProductImage = i.ProductImage,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity,
                Category = i.Category
            }).ToList();

            if (string.IsNullOrWhiteSpace(model.ShippingAddress))
                model.ShippingAddress = customer.CustomerAddress;

            if (string.IsNullOrWhiteSpace(model.ShippingAddress))
            {
                ModelState.AddModelError("ShippingAddress", "Vui lòng nhập địa chỉ giao hàng.");
                model.CartItems = cartItems;
                model.TotalAmount = cartItems.Sum(x => x.TotalPrice);
                return View(model);
            }

            model.TotalAmount = cartItems.Sum(x => x.TotalPrice);

            // PayPal hiện chỉ là lựa chọn giao diện, chưa thực hiện giao dịch thật.
            string paymentLabel = model.PaymentMethod == "Paypal"
                ? "PayPal (chưa tích hợp thanh toán)"
                : "Tiền mặt";

            return SaveOrderAndRedirect(model, customer, cartItems, paymentLabel);
        }

        private ActionResult SaveOrderAndRedirect(CheckoutVM model, Customer customer, List<CartItem> cartItems, string paymentStatus)
        {
            var order = new Order
            {
                CustomerID = customer.CustomerID,
                OrderDate = DateTime.Today,
                TotalAmount = cartItems.Sum(x => x.TotalPrice),
                PaymentStatus = paymentStatus,
                AddressDelivery = string.IsNullOrWhiteSpace(model.ShippingAddress)
                    ? customer.CustomerAddress
                    : model.ShippingAddress,
                OrderDetails = cartItems.Select(item => new OrderDetail
                {
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                }).ToList()
            };

            db.Orders.Add(order);
            db.SaveChanges();

            // Hai lựa chọn này chỉ tồn tại trong phiên hiện tại, không ghi thêm cột database.
            TempData["OrderPaymentMethod"] = model.PaymentMethod == "Paypal" ? "PayPal (chưa tích hợp)" : "Tiền mặt";
            TempData["OrderShippingMethod"] = model.ShippingMethod;
            Session["Cart"] = null;

            return RedirectToAction("OrderSuccess", new { id = order.OrderID });
        }

        public ActionResult OrderSuccess(int id)
        {
            var order = db.Orders.Include("OrderDetails.Product")
                .SingleOrDefault(o => o.OrderID == id);

            if (order == null)
                return HttpNotFound();

            // Không cho người dùng xem đơn hàng của tài khoản khác.
            if (Session["Username"] == null)
                return RedirectToAction("Login", "Account");

            string username = Session["Username"].ToString();
            var customer = db.Customers.SingleOrDefault(c => c.Username == username);
            if (customer == null || order.CustomerID != customer.CustomerID)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);

            ViewBag.PaymentMethod = TempData["OrderPaymentMethod"] as string ?? order.PaymentStatus;
            ViewBag.ShippingMethod = TempData["OrderShippingMethod"] as string;
            return View(order);
        }

        public ActionResult MyOrder()
        {
            if (Session["Username"] == null)
                return RedirectToAction("Login", "Account");

            string username = Session["Username"].ToString();
            var customer = db.Customers.FirstOrDefault(c => c.Username == username);

            if (customer == null)
                return View(new List<Order>());

            var orders = db.Orders.Include("OrderDetails.Product")
                .Where(o => o.CustomerID == customer.CustomerID)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(orders);
        }

        public ActionResult BuyAgain(int id)
        {
            if (Session["Username"] == null)
                return RedirectToAction("Login", "Account");

            string username = Session["Username"].ToString();
            var customer = db.Customers.SingleOrDefault(c => c.Username == username);
            if (customer == null)
                return RedirectToAction("MyOrder");

            var order = db.Orders.Include("OrderDetails.Product")
                .SingleOrDefault(o => o.OrderID == id && o.CustomerID == customer.CustomerID);

            if (order == null)
                return HttpNotFound();

            var cart = new CartService(Session).GetCart();
            foreach (var detail in order.OrderDetails)
            {
                if (detail.Product != null)
                {
                    cart.AddItem(
                        detail.Product.ProductID,
                        detail.Product.ProductImage,
                        detail.Product.ProductName,
                        detail.UnitPrice,
                        detail.Quantity,
                        detail.Product.Category != null ? detail.Product.Category.CategoryName : string.Empty
                    );
                }
            }

            return RedirectToAction("Index", "Cart");
        }
    }
}

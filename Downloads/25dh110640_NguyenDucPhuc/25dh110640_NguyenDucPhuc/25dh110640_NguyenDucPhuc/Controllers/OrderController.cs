using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using MyStore.Models;
using MyStore.Models.ViewModel;
using PagedList;

namespace MyStore.Controllers
{
    public class OrderController : BaseController
    {
        private CartService GetCartService()
        {
            return new CartService(Session);
        }

        #region Hàm dùng chung

        private Customer GetCurrentCustomer()
        {
            var user = db.Users.SingleOrDefault(u => u.Username == User.Identity.Name);
            if (user == null) return null;
            return db.Customers.SingleOrDefault(c => c.Username == user.Username);
        }

        private static System.Collections.Generic.List<CartItem> GetSelectedItems(Cart cart, System.Collections.Generic.ICollection<int> selectedIds)
        {
            var items = cart.Items.ToList();
            if (selectedIds != null && selectedIds.Count > 0)
                items = items.Where(i => selectedIds.Contains(i.ProductID)).ToList();
            return items;
        }

        #endregion

        #region Trang Đặt hàng (Checkout)

        [Authorize]
        [HttpGet]
        public ActionResult Checkout(int[] selectedIds)
        {
            var cart = GetCartService().GetCart();
            if (cart == null || cart.Items == null || !cart.Items.Any())
            {
                TempData["Error"] = "Giỏ hàng của bạn đang trống";
                return RedirectToAction("Index", "Home");
            }

            var customer = GetCurrentCustomer();
            if (customer == null) return RedirectToAction("Login", "Account");

            var items = GetSelectedItems(cart, selectedIds);
            if (!items.Any())
            {
                TempData["Error"] = "Vui lòng chọn ít nhất một sản phẩm để mua";
                return RedirectToAction("Index", "Cart");
            }

            var model = new CheckoutVM
            {
                ShippingAddress = customer.CustomerAddress,
                CustomerName = customer.CustomerName,
                CustomerPhone = customer.CustomerPhone,
                CartItems = items,
                SelectedIds = items.Select(i => i.ProductID).ToList(),
                TotalAmount = items.Sum(i => i.TotalPrice),
                OrderDate = DateTime.Now
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(CheckoutVM model, string newShippingAddress)
        {
            var cart = GetCartService().GetCart();
            if (cart == null || !cart.Items.Any())
            {
                TempData["Error"] = "Giỏ hàng của bạn đang trống";
                return RedirectToAction("Index", "Home");
            }

            if (!string.IsNullOrWhiteSpace(newShippingAddress))
            {
                model.ShippingAddress = newShippingAddress;
                ModelState.Remove("ShippingAddress");
            }

            var items = GetSelectedItems(cart, model.SelectedIds);
            model.CartItems = items;
            model.SelectedIds = items.Select(i => i.ProductID).ToList();
            model.TotalAmount = items.Sum(i => i.TotalPrice);

            if (!items.Any())
            {
                TempData["Error"] = "Vui lòng chọn ít nhất một sản phẩm để mua";
                return RedirectToAction("Index", "Cart");
            }

            if (!ModelState.IsValid) return View(model);

            var customer = GetCurrentCustomer();
            if (customer == null) return RedirectToAction("Login", "Account");

            string paymentStatus = "Chờ thanh toán";
            if (model.PaymentMethod == "PayPal")
            {
                Session["PendingOrder"] = model;
                return RedirectToAction("PaymentWithPaypal", "Paypal");
            }

            var order = new Order
            {
                CustomerID = customer.CustomerID,
                OrderDate = DateTime.Now,
                TotalAmount = model.TotalAmount,
                PaymentStatus = paymentStatus,
                ShippingAddress = model.ShippingAddress,
                DeliveryMethod = model.DeliveryMethod,
                PaymentMethod = model.PaymentMethod
            };

            foreach (var item in items)
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

            TempData["OrderDeliveryMethod"] = model.DeliveryMethod;
            TempData["OrderPaymentMethod"] = model.PaymentMethod;

            cart.RemoveItems(model.SelectedIds);
            GetCartService().SetCart(cart);

            return RedirectToAction("OrderSuccess", new { id = order.OrderID });
        }

        #endregion

        #region Xác nhận đơn hàng

        [Authorize]
        public ActionResult OrderSuccess(int id)
        {
            var customer = GetCurrentCustomer();
            if (customer == null) return RedirectToAction("Login", "Account");

            var order = db.Orders
                          .Include("OrderDetails")
                          .Include("OrderDetails.Product")
                          .FirstOrDefault(o => o.OrderID == id && o.CustomerID == customer.CustomerID);

            if (order == null) return HttpNotFound();

            order.DeliveryMethod = TempData["OrderDeliveryMethod"] as string;
            order.PaymentMethod = TempData["OrderPaymentMethod"] as string;

            return View(order);
        }

        #endregion

        #region Đơn hàng của tôi (Lịch sử mua hàng)

        [Authorize]
        public ActionResult MyOrder(string searchTerm, string status, int? page)
        {
            var customer = GetCurrentCustomer();
            if (customer == null) return RedirectToAction("Login", "Account");

            var orders = db.Orders.Include("OrderDetails.Product")
                                  .Where(o => o.CustomerID == customer.CustomerID)
                                  .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                int orderId;
                bool isNumber = int.TryParse(searchTerm, out orderId);
                orders = orders.Where(o =>
                    (isNumber && o.OrderID == orderId) ||
                    o.OrderDetails.Any(d => d.Product.ProductName.Contains(searchTerm)));
            }

            if (!string.IsNullOrEmpty(status) && status != "Tất cả")
                orders = orders.Where(o => o.PaymentStatus == status);

            var model = new MyOrderVM
            {
                SearchTerm = searchTerm,
                Status = string.IsNullOrEmpty(status) ? "Tất cả" : status,
                PageSize = 5
            };

            int pageNumber = Math.Max(page ?? 1, 1);
            model.Orders = orders.OrderByDescending(o => o.OrderDate).ToPagedList(pageNumber, model.PageSize);

            return View(model);
        }

        [Authorize]
        public ActionResult OrderDetail(int id)
        {
            var customer = GetCurrentCustomer();
            if (customer == null) return RedirectToAction("Login", "Account");

            var order = db.Orders.Include("OrderDetails.Product")
                          .FirstOrDefault(o => o.OrderID == id && o.CustomerID == customer.CustomerID);
            if (order == null) return HttpNotFound();

            return View(order);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelOrder(int id)
        {
            var customer = GetCurrentCustomer();
            if (customer == null) return RedirectToAction("Login", "Account");

            var order = db.Orders.FirstOrDefault(o => o.OrderID == id && o.CustomerID == customer.CustomerID);
            if (order == null) return HttpNotFound();

            if (order.PaymentStatus == "Đã giao" || order.PaymentStatus == "Đang vận chuyển")
            {
                TempData["Error"] = "Không thể huỷ đơn hàng đã được vận chuyển hoặc đã giao";
                return RedirectToAction("MyOrder");
            }

            order.PaymentStatus = "Đã huỷ";
            db.Entry(order).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("MyOrder");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reorder(int id)
        {
            var customer = GetCurrentCustomer();
            if (customer == null) return RedirectToAction("Login", "Account");

            var order = db.Orders.Include("OrderDetails.Product.Category")
                          .FirstOrDefault(o => o.OrderID == id && o.CustomerID == customer.CustomerID);
            if (order == null) return HttpNotFound();

            var cart = GetCartService().GetCart();
            foreach (var d in order.OrderDetails)
            {
                if (d.Product == null) continue;
                cart.AddItem(new CartItem
                {
                    ProductID = d.Product.ProductID,
                    ProductName = d.Product.ProductName,
                    ProductImage = d.Product.ProductImage,
                    ProductPrice = d.Product.ProductPrice,
                    Quantity = d.Quantity,
                    Category = d.Product.Category != null ? d.Product.Category.CategoryName : "Khác"
                });
            }
            GetCartService().SetCart(cart);

            return RedirectToAction("Index", "Cart");
        }

        #endregion
    }
}

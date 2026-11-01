using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MyStore.Models;
using MyStore.Models.ViewModel;
using PagedList;

namespace MyStore.Controllers
{
    public class CartController : BaseController
    {
        /// <summary>
        /// Lấy dịch vụ giỏ hàng (thao tác với Session)
        /// </summary>
        public CartService GetCartService()
        {
            return new CartService(Session);
        }

        /// <summary>
        /// Trang giỏ hàng: hiển thị các mặt hàng, gom nhóm theo danh mục (nâng cao)
        /// </summary>
        public ActionResult Index(int? page)
        {
            var cart = GetCartService().GetCart();

            // Giỏ hàng nâng cao: hiển thị thêm danh sách sản phẩm cùng danh mục với sản phẩm trong giỏ
            var categories = cart.Items.Select(i => i.Category).Distinct().ToList();
            if (categories.Count > 0)
            {
                var similar = db.Products
                    .Where(p => p.Category != null && categories.Contains(p.Category.CategoryName))
                    .OrderBy(p => p.ProductID);
                cart.SimilarProducts = similar.ToPagedList(page ?? 1, 4);
            }

            return View(cart);
        }

        /// <summary>
        /// Thêm sản phẩm vào giỏ hàng
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddToCart(int productId, int quantity = 1)
        {
            var product = db.Products.Include(p => p.Category).FirstOrDefault(p => p.ProductID == productId);
            if (product == null) return HttpNotFound();

            var cart = GetCartService().GetCart();
            cart.AddItem(new CartItem
            {
                ProductID = product.ProductID,
                ProductName = product.ProductName,
                ProductImage = product.ProductImage,
                ProductPrice = product.ProductPrice,
                Quantity = quantity < 1 ? 1 : quantity,
                Category = product.Category != null ? product.Category.CategoryName : "Khác"
            });
            GetCartService().SetCart(cart);

            if (Request.IsAjaxRequest())
                return Json(new { success = true, count = cart.Count });

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Xoá 1 sản phẩm khỏi giỏ hàng
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveFromCart(int productId)
        {
            var cart = GetCartService().GetCart();
            cart.RemoveItem(productId);
            GetCartService().SetCart(cart);

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Cập nhật số lượng của 1 sản phẩm trong giỏ
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateQuantity(int productId, int quantity)
        {
            var cart = GetCartService().GetCart();
            cart.UpdateQuantity(productId, quantity);
            GetCartService().SetCart(cart);

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Xoá toàn bộ giỏ hàng
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClearCart()
        {
            GetCartService().ClearCart();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Partial view: số lượng mặt hàng trong giỏ (hiển thị trên menu)
        /// </summary>
        [ChildActionOnly]
        public ActionResult CartSummary()
        {
            var cart = GetCartService().GetCart();
            return PartialView("_CartSummaryPV", cart.Count);
        }
    }
}

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
        public CartService GetCartService()
        {
            return new CartService(Session);
        }
        public ActionResult Index(int? page)
        {
            var cart = GetCartService().GetCart();
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveFromCart(int productId)
        {
            var cart = GetCartService().GetCart();
            cart.RemoveItem(productId);
            GetCartService().SetCart(cart);

            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateQuantity(int productId, int quantity)
        {
            var cart = GetCartService().GetCart();
            cart.UpdateQuantity(productId, quantity);
            GetCartService().SetCart(cart);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ClearCart()
        {
            GetCartService().ClearCart();
            return RedirectToAction("Index");
        }
        [ChildActionOnly]
        public ActionResult CartSummary()
        {
            var cart = GetCartService().GetCart();
            return PartialView("_CartSummaryPV", cart.Count);
        }
    }
}

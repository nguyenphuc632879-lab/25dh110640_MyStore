using System;
using System.Web;

namespace MyStore.Models.ViewModel
{
    /// <summary>
    /// Dịch vụ thao tác với giỏ hàng trong Session (BE#7)
    /// </summary>
    public class CartService
    {
        private readonly HttpSessionStateBase session;

        public CartService(HttpSessionStateBase session)
        {
            this.session = session;
        }

        public Cart GetCart()
        {
            var cart = session["Cart"] as Cart;
            if (cart == null)
            {
                cart = new Cart();
                session["Cart"] = cart;
            }
            return cart;
        }

        public void SetCart(Cart cart)
        {
            session["Cart"] = cart;
        }

        public void ClearCart()
        {
            session["Cart"] = null;
        }
    }
}

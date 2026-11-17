using System.Web;
using _25dh110640_MyStore.Models.ViewModel;

namespace _25dh110640_MyStore.Models
{
    public class CartService
    {
        private const string CartSessionKey = "Cart";
        private readonly HttpSessionStateBase session;

        public CartService(HttpSessionStateBase sessionState)
        {
            session = sessionState;
        }

        public Cart GetCart()
        {
            var cart = session[CartSessionKey] as Cart;
            if (cart == null)
            {
                cart = new Cart();
                session[CartSessionKey] = cart;
            }
            return cart;
        }

        public void ClearCart()
        {
            session[CartSessionKey] = null;
        }
    }
}

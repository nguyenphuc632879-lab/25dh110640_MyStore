using System;
using System.Collections.Generic;
using System.Linq;
using PagedList;
using _25dh110640_MyStore.Models;

namespace _25dh110640_MyStore.Models.ViewModel
{
    [Serializable]
    public class CartItem
    {
        public int ProductID { get; set; }
        public string ProductImage { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string Category { get; set; }

        public decimal TotalPrice
        {
            get { return UnitPrice * Quantity; }
        }
    }

    [Serializable]
    public class Cart
    {
        public Cart()
        {
            Items = new List<CartItem>();
            PageSize = 8;
        }

        public List<CartItem> Items { get; set; }
        public IPagedList<Product> SimilarProducts { get; set; }
        public int PageSize { get; set; }

        public void AddItem(int productId, string image, string name, decimal unitPrice, int quantity, string category)
        {
            if (quantity < 1) quantity = 1;

            var existing = Items.FirstOrDefault(x => x.ProductID == productId);
            if (existing == null)
            {
                Items.Add(new CartItem
                {
                    ProductID = productId,
                    ProductImage = image,
                    ProductName = name,
                    UnitPrice = unitPrice,
                    Quantity = quantity,
                    Category = category
                });
            }
            else
            {
                existing.Quantity += quantity;
            }
        }

        public void RemoveItem(int productId)
        {
            Items.RemoveAll(x => x.ProductID == productId);
        }

        public void UpdateQuantity(int productId, int quantity)
        {
            var item = Items.FirstOrDefault(x => x.ProductID == productId);
            if (item == null) return;

            if (quantity <= 0)
                Items.Remove(item);
            else
                item.Quantity = quantity;
        }

        public void Clear()
        {
            Items.Clear();
        }
    }

    public class CheckoutVM
    {
        public CheckoutVM()
        {
            CartItems = new List<CartItem>();
            PaymentMethod = "Tiền mặt";
            ShippingMethod = "Giao hàng nhanh";
        }

        public List<CartItem> CartItems { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public string ShippingAddress { get; set; }
        public string PaymentMethod { get; set; }
        public string ShippingMethod { get; set; }
        public int CustomerID { get; set; }
        public string Username { get; set; }
    }
}

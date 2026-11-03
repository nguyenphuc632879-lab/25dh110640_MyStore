using System;
using System.Collections.Generic;
using System.Linq;
using MyStore.Models;
using PagedList;

namespace MyStore.Models.ViewModel
{
    public class Cart
    {
        private List<CartItem> items = new List<CartItem>();

        public IEnumerable<CartItem> Items
        {
            get { return items; }
        }

        // Giỏ hàng nâng cao: gom nhóm các mặt hàng theo Danh mục
        public List<IGrouping<string, CartItem>> GroupedItems
        {
            get { return items.GroupBy(i => i.Category).ToList(); }
        }

        public IPagedList<Product> SimilarProducts { get; set; }

        public int Count
        {
            get { return items.Sum(i => i.Quantity); }
        }

        public decimal Total
        {
            get { return items.Sum(i => i.TotalPrice); }
        }

        public void AddItem(CartItem item)
        {
            var exist = items.FirstOrDefault(i => i.ProductID == item.ProductID);
            if (exist != null)
                exist.Quantity += item.Quantity;
            else
                items.Add(item);
        }

        public void RemoveItem(int productId)
        {
            var exist = items.FirstOrDefault(i => i.ProductID == productId);
            if (exist != null)
                items.Remove(exist);
        }

        public void UpdateQuantity(int productId, int quantity)
        {
            var exist = items.FirstOrDefault(i => i.ProductID == productId);
            if (exist != null)
            {
                if (quantity <= 0) items.Remove(exist);
                else exist.Quantity = quantity;
            }
        }


        public void RemoveItems(IEnumerable<int> productIds)
        {
            items.RemoveAll(i => productIds.Contains(i.ProductID));
        }

        public void Clear()
        {
            items.Clear();
        }
    }
}

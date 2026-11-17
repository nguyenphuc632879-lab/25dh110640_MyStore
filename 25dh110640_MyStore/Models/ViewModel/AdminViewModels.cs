using System.Collections.Generic;

namespace _25dh110640_MyStore.Models.ViewModel
{
    public class CustomerSearchVM : PagerVM
    {
        public string SearchTerm { get; set; }
        public string SortOrder { get; set; }
        public List<Customer> Customers { get; set; }
    }

    public class OrderListVM : PagerVM
    {
        public int? CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string SearchTerm { get; set; }
        public List<Order> Orders { get; set; }
    }

    public class UserSearchVM : PagerVM
    {
        public string SearchTerm { get; set; }
        public List<User> Users { get; set; }
    }

    public class CategoryStatVM
    {
        public string CategoryName { get; set; }
        public int Quantity { get; set; }
        public decimal MaxPrice { get; set; }
        public decimal MinPrice { get; set; }
        public decimal AvgPrice { get; set; }
    }

    public class LoginVM
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}

using CarService.Models;

namespace CarService.ViewModels
{
    public class OrdersViewModel
    {
        public List<Order> orders { get; set; }
        public Dictionary<string,string> masters { get; set; }
    }
}

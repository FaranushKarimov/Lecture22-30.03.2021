using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iBook.tj.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string Username { get; set; }
        public bool IsCanceled { get; set; }
        public bool IsDelivered { get; set; }
        public int AmountOfBooks { get; set; }
        public string PhoneNumber { get; set; }
        public string ReceiverName { get; set; }
        public double AmountOfCost { get; set; }
        public DateTime OrderCreatedTime { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iBook.tj.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public DateTime TimeAddded { get; set; }

        public virtual ICollection<CartItemDetail> CartItemDetails { get; set; }
    }
}

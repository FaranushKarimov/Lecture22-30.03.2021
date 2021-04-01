using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iBook.tj.ViewModels
{
    public class CartItemViewModel
    {
        public int Amount { get; set; }
        public int BookId { get; set; }
        public int CartItemId { get; set; }
        public double BookCost { get; set; }
        public string BookName { get; set; }
        public string ImageLink { get; set; }
        public string BookAuthor { get; set; }
        public string CategoryName { get; set; }
        
    }
}

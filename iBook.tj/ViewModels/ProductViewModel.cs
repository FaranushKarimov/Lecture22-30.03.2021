using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iBook.tj.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public string Author { get; set; }

        public int YearOfPublishing { get; set; }
        public int Amount { get; set; }
        public string CategoryName { get; set; }
        public string ImageLink { get; set; }
        public double BookCost { get; set; }
    }
}

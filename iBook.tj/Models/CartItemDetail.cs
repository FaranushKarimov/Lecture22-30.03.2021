using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace iBook.tj.Models
{
    public class CartItemDetail
    {
        public int Id { get; set; }
        public int Amount { get; set; }
       
        [Required]
        public int BookId { get; set; }

        [Required]
        public int CartItemId { get; set; }
        
        [Required]
        public virtual CartItem CartItem { get; set; }

        [Required]
        public virtual Book Book { get; set; }
    }
}

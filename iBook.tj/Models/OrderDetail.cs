using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace iBook.tj.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        
        [Required]
        public int BookId { get; set; }
        
        [Required]
        public int OrderId { get; set; }
        
        public int Amount { get; set; }
        
        [Required]
        public virtual Book Book { get; set; }
        
        [Required]
        public virtual Order Order { get; set; }

    }
}

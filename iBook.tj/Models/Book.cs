using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace iBook.tj.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Пожалуйста введите название")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Пожалуйста введите описание")]
        [Column(TypeName ="nvarchar(20000)")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Пожалуйста введите имя автора")]
        public string Author { get; set; }

        [Required(ErrorMessage = "Пожалуйста введите год публикации")]
        public int YearOfPublishing { get; set; }

        [Required(ErrorMessage = "Пожалуйста выберите категорию")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Пожалуйста выберите фотографию")]
        public string ImageLink { get; set; }

        [Required(ErrorMessage = "Пожалуйста введите стоимость")]
        public double BookCost { get; set; }
        
        [Required(ErrorMessage ="Пожалуйста введите количество")]
        public int Amount { get; set; }

        public virtual Category Category { get; set; }

        public ICollection<CartItemDetail> CartItemDetails { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace iBook.tj.ViewModels
{
    public class BookViewModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Пожалуйста введите название")]
        [Display(Name = "Название")]
        public string Title { get; set; }
        
        [Required(ErrorMessage = "Пожалуйста введите описание")]
        [Display(Name = "Описание")]
        [Column(TypeName = "nvarchar(20000)")]
        public string Description { get; set; }
       
        [Required(ErrorMessage = "Пожалуйста введите имя автора")]
        [Display(Name = "Автор")]
        public string Author { get; set; }

        [Required(ErrorMessage = "Пожалуйста введите год публикации")]
        [Display(Name = "Год публикации")]
        public string YearOfPublishing { get; set; }
        
        [Required(ErrorMessage = "Пожалуйста выберите категорию")]
        [Display(Name = "Категория")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Пожалуйста введите стоимость")]
        [Display(Name = "Цена")]
        public string BookCost { get; set; }

        [Required(ErrorMessage = "Пожалуйста введите количество")]
        [Display(Name = "Количество")]
        public string Amount { get; set; }
        
        [Required(ErrorMessage = "Пожалуйста выберите фотографию")]
        [Display(Name = "Фотография")]
        public IFormFile ImageLink { get; set; }
    }
}

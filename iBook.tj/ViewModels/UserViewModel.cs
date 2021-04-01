using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace iBook.tj.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Пожалуйста введите имя")]
        [Display(Name = "Имя")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Пожалуйста введите фамилию")]
        [Display(Name = "Фамилие")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Пожалуйста введите email")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Пожалуйста введите пароль")]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Пожалуйста введите подтверждение пароля")]
        [Display(Name = "Подтверждение пароля")]
        public string ConfirmPassword { get; set; }

    }
}

using Data_Access_Layer.Entities;
using System.ComponentModel.DataAnnotations;

namespace Hotel_Management_System.Models.DTOs
{
    public class CustomerCreateDTO
    {
        [Required]
        [Display(Name = "الاسم الأول")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "اسم العائلة")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "النوع")]
        public Gender Gender { get; set; }

        [Display(Name = "تاريخ الميلاد")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Required]
        [Display(Name = "البريد الإلكتروني")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "رقم الهاتف")]
        public string PhoneNumber { get; set; }

        [Display(Name = "العنوان")]
        public string? Address { get; set; }

        [Required]
        [Display(Name = "الهوية")]
        public string Role { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "كلمة المرور")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "كلمة المرور وتأكيدها غير متطابقين.")]
        [Display(Name = "تأكيد كلمة المرور")]
        public string ConfirmPassword { get; set; }
    }
}

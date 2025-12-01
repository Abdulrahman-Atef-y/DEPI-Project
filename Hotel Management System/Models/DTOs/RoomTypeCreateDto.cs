using System.ComponentModel.DataAnnotations;

namespace Hotel_Management_System.Models.DTOs
{
    public class RoomTypeCreateDto
    {
        [Required(ErrorMessage = "الاسم مطلوب.")]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "السعر مطلوب.")]
        [DataType(DataType.Currency)]
        public double Price { get; set; }

        [Required(ErrorMessage = "عدد الأشخاص مطلوب.")]
        public int Occupancy { get; set; }
        [Required(ErrorMessage = "يجب إضافة صورة واحدة على الأقل.")]
        public IFormFile[] RoomImages { get; set; }
        public int Area { get; set; }
    }
}

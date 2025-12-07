using Hotel_Management_System.Models.DTOs;
namespace Hotel_Management_System.Models.DTOs
{
    public class CustomerDTO
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Role { get; set; }
    }
}

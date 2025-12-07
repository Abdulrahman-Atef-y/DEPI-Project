namespace Hotel_Management_System.Models.DTOs
{
    public class RoomReviewDTO
    {
        public string GuestName { get; set; }
        public string? Comment { get; set; }
        public int Rating { get; set; }
        public DateTime Date { get; set; }
    }
}

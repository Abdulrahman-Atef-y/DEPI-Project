using Data_Access_Layer.Entities;

namespace Hotel_Management_System.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalRooms { get; set; }
        public int TotalBookings { get; set; }
        public int TotalUsers { get; set; }
        public decimal TotalRevenue { get; set; }
        public int[] MonthlyBookings { get; set; } = new int[12];
        public string[] RoomTypeLabels { get; set; } = Array.Empty<string>();
        public int[] RoomTypeCounts { get; set; } = Array.Empty<int>();
        public IEnumerable<Booking> RecentBookings { get; set; } = new List<Booking>();
    }
}

namespace TableReservations.Models
{
    public class Revenue
    {
        public List<RevenueData> WeeklyRevenue { get; set; } = new();
        public List<RevenueData> MonthlyRevenue { get; set; } = new();
        public List<RevenueData> YearlyRevenue { get; set; } = new();
        public decimal TotalRevenue { get; set; }
    }
}
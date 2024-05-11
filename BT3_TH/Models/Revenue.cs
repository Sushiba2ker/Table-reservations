namespace BT3_TH.Models
{
    public class Revenue
    {
        public List<RevenueData> WeeklyRevenue { get; set; }
        public List<RevenueData> MonthlyRevenue { get; set; }
        public List<RevenueData> YearlyRevenue { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
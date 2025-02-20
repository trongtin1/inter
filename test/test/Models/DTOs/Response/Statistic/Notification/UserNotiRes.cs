namespace test.Models.DTOs.Response.Statistic.Notification
{
    public class UserNotiRes
    {
        public string Email { get; set; }
        public List<MonthlyNotiStatistic> MonthlyStats { get; set; } = new();
        public int YearlyTotal { get; set; }
    }
}
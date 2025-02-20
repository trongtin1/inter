namespace test.Models.DTOs.Response.Statistic.Email
{
    public class UserEmailRes
    {
        public string Email { get; set; }
        public List<MonthlyEmailStatistic> MonthlyStats { get; set; } = new();
        public int YearlyTotal { get; set; }
    }
}
namespace test.Models.DTOs.Response.Statistic
{
    public class UserEmailRes
    {
        public string Email { get; set; }
        public List<MonthlyStatistic> MonthlyStats { get; set; } = new();
        public int YearlyTotal { get; set; }
    }
}
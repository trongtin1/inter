using System;

namespace test.Models
{
    public class UserAccessInfo
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime ConnectedTime { get; set; }
        public TimeSpan TotalConnectedTime { get; set; } = TimeSpan.Zero;
        public bool IsOnline { get; set; }
    }
} 
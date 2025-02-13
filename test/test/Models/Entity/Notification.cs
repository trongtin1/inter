using System;
using System.Collections.Generic;

namespace test.Models.Entity;

public partial class Notification
{
    public long Id { get; set; }

    public DateTime CreatedTime { get; set; }

    public string? Type { get; set; }

    public string? Content { get; set; }

    public bool IsRead { get; set; }

    public string Email { get; set; } = null!;

    public DateTime? LastModified { get; set; }

    public string? From { get; set; }

    public string? Url { get; set; }

    public bool? IsSeen { get; set; }

    public string? ContentEn { get; set; }

    public string? UrlMobile { get; set; }

    public string? ReqBody { get; set; }

    public string? DetailId { get; set; }

    public DateTime? SeenTime { get; set; }

    public string? HostName { get; set; }

    public string? FunctionName { get; set; }
}

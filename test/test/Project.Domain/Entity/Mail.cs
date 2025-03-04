using System;
using System.Collections.Generic;

namespace test.Project.Domain.Entity;
public partial class Mail
{
    public long Id { get; set; }

    public string? Email { get; set; }

    public string? EmailContent { get; set; }

    public string? FileAttach { get; set; }

    public string? CreateBy { get; set; }

    public DateTime CreateTime { get; set; }

    public bool IsSend { get; set; }

    public DateTime? SendTime { get; set; }

    public string? Subject { get; set; }

    public string? SentStatus { get; set; }

    public string? EmailCc { get; set; }

    public string? EmailBcc { get; set; }

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public string? Location { get; set; }

    public int? MailType { get; set; }

    public string? Organizer { get; set; }

    public string? OrganizerMail { get; set; }

    public string? Uid { get; set; }
}

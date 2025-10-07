using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class Comment
{
    public ulong Id { get; set; }

    public ulong ExamId { get; set; }

    public ulong UserId { get; set; }

    public ulong? ReplyTo { get; set; }

    public string? Content { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Exam Exam { get; set; } = null!;

    public virtual User? ReplyToNavigation { get; set; }

    public virtual User User { get; set; } = null!;
}

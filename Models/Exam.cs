using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class Exam
{
    public ulong Id { get; set; }

    public byte ExamType { get; set; }

    public string DisplayType { get; set; } = null!;

    public string Title { get; set; } = null!;

    public byte Grade { get; set; }

    public ushort TimeLimit { get; set; }

    public ushort? EarlyTurnIn { get; set; }

    public ICollection<string>? PartTitle { get; set; } = [];

    public bool? AllowShowScore { get; set; }

    public bool? AllowPartSwap { get; set; }

    public bool? AllowQuestionSwap { get; set; }

    public bool? AllowAnswerSwap { get; set; }

    public ulong? TeacherId { get; set; }

    public string? SubjectId { get; set; }

    public ulong? ApprovedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public ushort State { get; set; } = 1;

    public virtual Teacher? ApprovedByNavigation { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = [];

    public virtual ICollection<ExamAttempt> ExamAttempts { get; set; } = [];

    public virtual ICollection<PdfExamAttempt> PdfExamAttempts { get; set; } = [];

    public virtual ICollection<PdfExamCode> PdfExamCodes { get; set; } = [];

    public virtual Subject? Subject { get; set; }

    public virtual Teacher? Teacher { get; set; }
}

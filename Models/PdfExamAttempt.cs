using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class PdfExamAttempt
{
    public ulong Id { get; set; }

    public ulong ExamId { get; set; }

    public ulong StudentId { get; set; }

    public decimal? Score { get; set; }

    public int? Duration { get; set; }

    public DateTime? SubmittedAt { get; set; }

    public string? Comment { get; set; }

    public ulong CodeId { get; set; }

    public string StudentAnswer { get; set; } = null!;

    public ICollection<bool> CorrectBoard { get; set; } = null!;

    public virtual PdfExamCode Code { get; set; } = null!;

    public virtual Exam Exam { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}

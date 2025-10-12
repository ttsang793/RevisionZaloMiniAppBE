using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class ExamAttempt
{
    public ulong Id { get; set; }

    public ulong ExamId { get; set; }

    public ulong StudentId { get; set; }

    public decimal? Score { get; set; }

    public uint? Duration { get; set; }

    public DateTime? SubmittedAt { get; set; }

    public string? Comment { get; set; }

    public virtual Exam Exam { get; set; } = null!;

    public virtual ICollection<ExamAttemptAnswer> ExamAttemptAnswers { get; set; } = new List<ExamAttemptAnswer>();

    public virtual Student Student { get; set; } = null!;
}

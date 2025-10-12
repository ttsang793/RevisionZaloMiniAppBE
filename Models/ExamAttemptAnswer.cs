using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class ExamAttemptAnswer
{
    public ulong Id { get; set; }

    public ulong AttemptId { get; set; }

    public ulong ExamQuestionId { get; set; }

    public string? StudentAnswer { get; set; }

    public bool? IsCorrect { get; set; }

    public virtual ExamAttempt Attempt { get; set; } = null!;

    public virtual ExamQuestion ExamQuestion { get; set; } = null!;
}

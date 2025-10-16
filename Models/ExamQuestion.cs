using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class ExamQuestion
{
    public ulong Id { get; set; }

    public ulong ExamPartId { get; set; }

    public ulong QuestionId { get; set; }

    public byte OrderIndex { get; set; }

    public decimal Point { get; set; }

    public virtual ICollection<ExamAttemptAnswer> ExamAttemptAnswers { get; set; } = [];

    public virtual ExamPart ExamPart { get; set; } = null!;

    public virtual Question Question { get; set; } = null!;
}

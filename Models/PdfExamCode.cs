using System;
using System.Collections.Generic;

namespace backend.Models;

public partial class PdfExamCode
{
    public ulong Id { get; set; }

    public ulong ExamId { get; set; }

    public string Code { get; set; } = null!;

    public string AnswerKey { get; set; } = null!;

    public virtual Exam Exam { get; set; } = null!;

    public virtual ICollection<PdfExamAttempt> PdfExamAttempts { get; set; } = new List<PdfExamAttempt>();
}

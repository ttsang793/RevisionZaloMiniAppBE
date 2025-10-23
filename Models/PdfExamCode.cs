namespace backend.Models;

public partial class PdfExamCode
{
    public ulong Id { get; set; }

    public ulong ExamId { get; set; }

    public string Code { get; set; } = null!;

    public string? TaskPdf { get; set; }

    public string? AnswerPdf { get; set; }

    public byte NumPart { get; set; }

    public virtual Exam Exam { get; set; } = null!;

    public virtual ICollection<PdfExamAttempt> PdfExamAttempts { get; set; } = new List<PdfExamAttempt>();

    public virtual ICollection<PdfExamCodeQuestion> PdfExamCodeQuestions { get; set; } = new List<PdfExamCodeQuestion>();
}

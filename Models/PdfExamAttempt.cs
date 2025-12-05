namespace backend.Models;

public partial class PdfExamAttempt
{
    public ulong Id { get; set; }

    public ulong PdfExamCodeId { get; set; }

    public ICollection<string> StudentAnswer { get; set; } = [];

    public ICollection<bool[]> CorrectBoard { get; set; } = [];

    public ICollection<decimal> PointBoard { get; set; } = [];

    public virtual ExamAttempt IdNavigation { get; set; } = null!;

    public virtual PdfExamCode PdfExamCode { get; set; } = null!;
}
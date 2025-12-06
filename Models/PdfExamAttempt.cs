namespace backend.Models;

public partial class PdfExamAttempt
{
    public ulong Id { get; set; }

    public ulong PdfExamCodeId { get; set; }

    public List<string> StudentAnswer { get; set; } = [];

    public List<bool[]> CorrectBoard { get; set; } = [];

    public List<decimal> PointBoard { get; set; } = [];

    public virtual ExamAttempt IdNavigation { get; set; } = null!;

    public virtual PdfExamCode PdfExamCode { get; set; } = null!;

    public override string ToString()
    {
        return $"{Id} {PdfExamCodeId} {StudentAnswer[0]} {PointBoard[0]}";
    }
}
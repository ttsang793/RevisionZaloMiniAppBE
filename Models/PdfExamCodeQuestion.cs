namespace backend.Models;

public partial class PdfExamCodeQuestion
{
    public ulong Id { get; set; }

    public string Type { get; set; } = null!;

    public ushort PartIndex { get; set; }

    public ushort QuestionIndex { get; set; }

    public string? AnswerKey { get; set; }

    public decimal Point { get; set; }

    public ulong? PdfExamCodeId { get; set; }

    public virtual PdfExamCode? PdfExamCode { get; set; }
}

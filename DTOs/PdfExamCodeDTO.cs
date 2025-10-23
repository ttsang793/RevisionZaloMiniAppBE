namespace backend.DTOs;

public partial class PdfExamCodeDTO
{
    public ulong? Id { get; set; }

    public ulong ExamId { get; set; }

    public string Code { get; set; } = null!;

    public string? TaskPdf { get; set; }

    public string? AnswerPdf { get; set; }

    public byte NumPart { get; set; }

    public ICollection<PdfExamCodeQuestionDTO> Questions { get; set; } = [];
}

public partial class PdfExamCodeQuestionDTO
{
    public ulong? Id { get; set; }

    public string Type { get; set; } = null!;

    public ushort PartIndex { get; set; }

    public ushort QuestionIndex { get; set; }

    public string? AnswerKey { get; set; }

    public decimal Point { get; set; }

    public ulong? PdfExamCodeId { get; set; }
}
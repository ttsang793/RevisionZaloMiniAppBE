namespace backend.Models;

public partial class ExamPart
{
    public ulong Id { get; set; }

    public ulong ExamId { get; set; }

    public string PartTitle { get; set; } = null!;

    public virtual ICollection<ExamQuestion> ExamQuestions { get; set; } = [];
}

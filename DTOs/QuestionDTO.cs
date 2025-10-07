namespace backend.DTOs;

public abstract partial class QuestionDTO
{
    public ulong Id { get; set; }

    public string Title { get; set; } = null!;

    public byte? Grade { get; set; }

    public byte? Type { get; set; }

    public byte? Difficulty { get; set; }

    public string? TopicId { get; set; }

    public string? SubjectId { get; set; }

    public string? Explaination { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}

public partial class MultipleChoiceQuestionDTO : QuestionDTO
{
    public string AnswerA { get; set; } = null!;

    public string AnswerB { get; set; } = null!;

    public string AnswerC { get; set; } = null!;

    public string AnswerD { get; set; } = null!;

    public string AnswerKey { get; set; } = null!;
}

public partial class TrueFalseQuestionDTO : QuestionDTO
{
    public bool? AnswerKey { get; set; }
}

public partial class ShortAnswerQuestionDTO : QuestionDTO
{
    public decimal AnswerKey { get; set; }
}

public partial class FillInTheBlankQuestionDTO : QuestionDTO
{
    public ICollection<string> AnswerKeys { get; set; } = [];

    public bool? MarkAsWrong { get; set; }
}

public partial class ConstructedResponseQuestionDTO : QuestionDTO
{
    public ICollection<string> AnswerKeys { get; set; } = [];

    public bool? AllowTakePhoto { get; set; }

    public bool? AllowEnter { get; set; }

    public bool? MarkAsWrong { get; set; }
}


public partial class SortingQuestionDTO : QuestionDTO
{
    public ICollection<string> CorrectOrder { get; set; } = [];
}
using backend.Models;

namespace backend.DTOs;

public partial class QuestionReadDTO
{
    public ulong? Id { get; set; }

    public string? Title { get; set; }

    public byte? Grade { get; set; }

    public string Type { get; set; } = null!;

    public byte? Difficulty { get; set; }

    public string? TopicId { get; set; }

    public string? SubjectName { get; set; }

    public string? Explanation { get; set; }
}

public abstract partial class QuestionDTO
{
    public ulong? Id { get; set; }

    public string? Title { get; set; }

    public string? ImageUrl { get; set; }

    public byte? Grade { get; set; }

    public string Type { get; set; } = null!;

    public byte? Difficulty { get; set; }

    public string? TopicId { get; set; }

    public string? Explanation { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}

public partial class MultipleChoiceQuestionDTO : QuestionDTO
{
    public string CorrectAnswer { get; set; } = null!;

    public ICollection<string> WrongAnswer { get; set; } = [];
}

public partial class TrueFalseQuestionDTO : QuestionDTO
{
    public bool? AnswerKey { get; set; }
}

public partial class ShortAnswerQuestionDTO : QuestionDTO
{
    public string AnswerKey { get; set; } = null!;
}

public partial class ManualResponseQuestionDTO : QuestionDTO
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

public partial class GroupQuestionGetDTO : QuestionDTO
{
    public string? PassageTitle { get; set; }

    public string? PassageContent { get; set; }

    public string? PassageAuthor { get; set; }

    public ICollection<Question> Questions { get; set; } = [];
}

public partial class GroupQuestionPostDTO : QuestionDTO
{
    public string? PassageTitle { get; set; }

    public string? PassageContent { get; set; }

    public string? PassageAuthor { get; set; }

    public ICollection<ulong> Questions { get; set; } = [];
}

public partial class TrueFalseTHPTQuestionDTO : QuestionDTO
{
    public string? PassageTitle { get; set; }

    public string? PassageContent { get; set; }

    public string? PassageAuthor { get; set; }

    public ICollection<string> Statements { get; set; } = [];

    public ICollection<bool> AnswerKeys { get; set; } = [];
}
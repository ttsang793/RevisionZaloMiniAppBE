using backend.Models;
using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;
[ApiController]
[Route("/api/question")]
public class QuestionController : Controller
{
    private readonly ILogger<QuestionController> _logger;
    private readonly QuestionDb _questionDb;

    public QuestionController(ILogger<QuestionController> logger, ZaloRevisionAppDbContext dbContext)
    {
        _logger = logger;
        _questionDb = new QuestionDb(dbContext);
    }

    // GET
    [HttpGet("teacher/{teacherId}")]
    public async Task<List<QuestionReadDTO>> GetAllQuestionsByTeacher(ulong teacherId, string? title)
    {
        return await _questionDb.GetAllQuestionsByTeacher(teacherId, title);
    }

    [HttpGet("teacher/filter/{teacherId}")]
    public async Task<List<QuestionReadDTO>> GetFilterQuestionsByTeacher(ulong teacherId, string type, string title = "")
    {        
        return await _questionDb.GetFilterQuestionsByTeacher(teacherId, type, title);
    }

    [HttpGet("{id}")]
    public async Task<QuestionDTO> GetQuestionById(ulong id)
    {
        var question = await _questionDb.GetQuestionById(id);

        switch (question.Type)
        {
            case "multiple-choice": return await GetMultipleChoiceQuestionById(question);
            case "true-false": return await GetTrueFalseQuestionById(question);
            case "short-answer": return await GetShortAnswerQuestionById(question);
            case "gap-fill": case "constructed-response": return await GetManualResponseQuestionById(question);
            case "sorting": return await GetSortingQuestionById(question);
            // case "group": return await GetGroupQuestionById(question);
            case "true-false-thpt": return await GetTrueFalseTHPTQuestionById(question);
            default: break;
        }
        return null;
    }

    private async Task<MultipleChoiceQuestionDTO> GetMultipleChoiceQuestionById(Question q)
    {
        var answer = await _questionDb.GetMultipleChoiceQuestionById(q.Id);
        return new MultipleChoiceQuestionDTO
        {
            Id = q.Id,
            Title = q.Title,
            Grade = q.Grade,
            Type = q.Type,
            Difficulty = q.Difficulty,
            TopicId = q.TopicId,
            Explanation = q.Explanation,
            CorrectAnswer = answer.CorrectAnswer,
            WrongAnswer = answer.WrongAnswer,
        };
    }

    private async Task<TrueFalseQuestionDTO> GetTrueFalseQuestionById(Question q)
    {
        var answer = await _questionDb.GetTrueFalseQuestionById(q.Id);
        return new TrueFalseQuestionDTO
        {
            Id = q.Id,
            Title = q.Title,
            Grade = q.Grade,
            Type = q.Type,
            Difficulty = q.Difficulty,
            TopicId = q.TopicId,
            Explanation = q.Explanation,
            AnswerKey = answer.AnswerKey
        };
    }

    private async Task<ShortAnswerQuestionDTO> GetShortAnswerQuestionById(Question q)
    {
        var answer = await _questionDb.GetShortAnswerQuestionById(q.Id);
        return new ShortAnswerQuestionDTO
        {
            Id = q.Id,
            Title = q.Title,
            Grade = q.Grade,
            Type = q.Type,
            Difficulty = q.Difficulty,
            TopicId = q.TopicId,
            Explanation = q.Explanation,
            AnswerKey = answer.AnswerKey
        };
    }

    private async Task<ManualResponseQuestionDTO> GetManualResponseQuestionById(Question q)
    {
        var answer = await _questionDb.GetManualResponseQuestionById(q.Id);
        return new ManualResponseQuestionDTO
        {
            Id = q.Id,
            Title = q.Title,
            Grade = q.Grade,
            Type = q.Type,
            Difficulty = q.Difficulty,
            TopicId = q.TopicId,
            Explanation = q.Explanation,
            AnswerKeys = answer.AnswerKeys,
            MarkAsWrong = answer.MarkAsWrong
        };
    }

    private async Task<SortingQuestionDTO> GetSortingQuestionById(Question q)
    {
        var answer = await _questionDb.GetSortingQuestionById(q.Id);
        return new SortingQuestionDTO
        {
            Id = q.Id,
            Title = q.Title,
            Grade = q.Grade,
            Type = q.Type,
            Difficulty = q.Difficulty,
            TopicId = q.TopicId,
            Explanation = q.Explanation,
            CorrectOrder = answer.CorrectOrder
        };
    }

    /*
    private async Task<GroupQuestionGetDTO> GetGroupQuestionById(Question q)
    {
        var answer = await _questionDb.GetGroupQuestionById(q.Id);

        return new GroupQuestionGetDTO
        {
            Id = q.Id,
            Title = q.Title,
            Grade = q.Grade,
            Type = q.Type,
            PassageTitle = answer.PassageTitle,
            PassageContent = answer.PassageContent,
            PassageAuthor = answer.PassageAuthor,
            Questions = await _questionDb.GetQuestionByMultipleIds(answer.Questions)
        };
    }
    */

    private async Task<TrueFalseTHPTQuestionDTO> GetTrueFalseTHPTQuestionById(Question q)
    {
        var answer = await _questionDb.GetTrueFalseTHPTQuestionById(q.Id);
        return new TrueFalseTHPTQuestionDTO
        {
            Id = q.Id,
            Title = q.Title,
            Grade = q.Grade,
            Type = q.Type,
            Difficulty = q.Difficulty,
            TopicId = q.TopicId,
            Explanation = q.Explanation,
            Statements = answer.Statements,
            AnswerKeys = answer.AnswerKeys
        };
    }

    // POST
    [HttpPost("multiple-choice")]
    public async Task<IActionResult> AddMultipleChoiceQuestion(MultipleChoiceQuestionDTO questionDTO)
    {
        Question q = new Question
        {
            Title = questionDTO.Title,
            Grade = questionDTO.Grade,
            Type = questionDTO.Type,
            Difficulty = questionDTO.Difficulty,
            TopicId = questionDTO.TopicId,
            TeacherId = questionDTO.TeacherId,
            Explanation = questionDTO.Explanation
        };

        MultipleChoiceQuestion mcq = new MultipleChoiceQuestion
        {
            CorrectAnswer = questionDTO.CorrectAnswer,
            WrongAnswer = questionDTO.WrongAnswer,
        };

        return await _questionDb.AddMultipleChoiceQuestion(q, mcq) ? StatusCode(201, new { q.Id }) : StatusCode(400);
    }

    [HttpPost("true-false")]
    public async Task<IActionResult> AddTrueFalseQuestion(TrueFalseQuestionDTO questionDTO)
    {
        Question q = new Question
        {
            Title = questionDTO.Title,
            Grade = questionDTO.Grade,
            Type = questionDTO.Type,
            Difficulty = questionDTO.Difficulty,
            TopicId = questionDTO.TopicId,
            TeacherId = questionDTO.TeacherId,
            Explanation = questionDTO.Explanation
        };

        TrueFalseQuestion tfq = new TrueFalseQuestion
        {
            AnswerKey = questionDTO.AnswerKey
        };

        return await _questionDb.AddTrueFalseQuestion(q, tfq) ? StatusCode(201, new { q.Id }) : StatusCode(400);
    }

    [HttpPost("short-answer")]
    public async Task<IActionResult> AddShortAnswerQuestion(ShortAnswerQuestionDTO questionDTO)
    {
        Question q = new Question
        {
            Title = questionDTO.Title,
            Grade = questionDTO.Grade,
            Type = questionDTO.Type,
            Difficulty = questionDTO.Difficulty,
            TopicId = questionDTO.TopicId,
            TeacherId = questionDTO.TeacherId,
            Explanation = questionDTO.Explanation
        };

        ShortAnswerQuestion saq = new ShortAnswerQuestion
        {
            AnswerKey = questionDTO.AnswerKey
        };

        return await _questionDb.AddShortAnswerQuestion(q, saq) ? StatusCode(201, new { q.Id }) : StatusCode(400);
    }

    [HttpPost("manual-response")]
    public async Task<IActionResult> AddManualResponseQuestion(ManualResponseQuestionDTO questionDTO)
    {
        Question q = new Question
        {
            Title = questionDTO.Title,
            Grade = questionDTO.Grade,
            Type = questionDTO.Type,
            Difficulty = questionDTO.Difficulty,
            TopicId = questionDTO.TopicId,
            TeacherId = questionDTO.TeacherId,
            Explanation = questionDTO.Explanation
        };

        ManualResponseQuestion crq = new ManualResponseQuestion
        {
            AnswerKeys = questionDTO.AnswerKeys,
            AllowTakePhoto = questionDTO.AllowTakePhoto,
            AllowEnter = questionDTO.AllowEnter,
            MarkAsWrong = questionDTO.MarkAsWrong
        };

        return await _questionDb.AddManualResponseQuestion(q, crq) ? StatusCode(201, new { q.Id }) : StatusCode(400);
    }

    [HttpPost("sorting")]
    public async Task<IActionResult> AddSortingQuestion(SortingQuestionDTO questionDTO)
    {
        Question q = new Question
        {
            Title = questionDTO.Title,
            Grade = questionDTO.Grade,
            Type = questionDTO.Type,
            Difficulty = questionDTO.Difficulty,
            TopicId = questionDTO.TopicId,
            TeacherId = questionDTO.TeacherId,
            Explanation = questionDTO.Explanation
        };

        SortingQuestion sq = new SortingQuestion
        {
            CorrectOrder = questionDTO.CorrectOrder
        };

        return await _questionDb.AddSortingQuestion(q, sq) ? StatusCode(201, new { q.Id }) : StatusCode(400);
    }

    /*
    [HttpPost("group")]
    public async Task<IActionResult> AddGroupQuestion(GroupQuestionPostDTO questionDTO)
    {
        Question q = new Question
        {
            Title = questionDTO.Title,
            Grade = questionDTO.Grade,
            Type = questionDTO.Type
        };

        GroupQuestion gq = new GroupQuestion
        {
            PassageTitle = questionDTO.PassageTitle,
            PassageContent = questionDTO.PassageContent,
            PassageAuthor = questionDTO.PassageAuthor,
            Questions = questionDTO.Questions
        };

        return await _questionDb.AddGroupQuestion(q, gq) ? StatusCode(201, new { q.Id }) : StatusCode(400);
    }
    */

    [HttpPost("true-false-THPT")]
    public async Task<IActionResult> AddTrueFalseTHPTQuestion(TrueFalseTHPTQuestionDTO questionDTO)
    {
        Question q = new Question
        {
            Title = questionDTO.Title,
            Grade = questionDTO.Grade,
            Type = questionDTO.Type,
            Difficulty = questionDTO.Difficulty,
            TopicId = questionDTO.TopicId,
            TeacherId = questionDTO.TeacherId,
            Explanation = questionDTO.Explanation
        };

        TrueFalseTHPTQuestion tfq = new TrueFalseTHPTQuestion
        {
            Statements = questionDTO.Statements,
            AnswerKeys = questionDTO.AnswerKeys
        };

        return await _questionDb.AddTrueFalseTHPTQuestion(q, tfq) ? StatusCode(201, new { q.Id }) : StatusCode(400);
    }

    // PUT
    [HttpPut("multiple-choice/{id}")]
    public async Task<IActionResult> UpdateMultipleChoiceQuestion(MultipleChoiceQuestionDTO questionDTO, ulong id)
    {
        Question q = new Question
        {
            Id = id,
            Title = questionDTO.Title,
            Grade = questionDTO.Grade,
            Type = questionDTO.Type,
            Difficulty = questionDTO.Difficulty,
            TopicId = questionDTO.TopicId,
            TeacherId = questionDTO.TeacherId,
            Explanation = questionDTO.Explanation
        };

        MultipleChoiceQuestion mcq = new MultipleChoiceQuestion
        {
            Id = id,
            CorrectAnswer = questionDTO.CorrectAnswer,
            WrongAnswer = questionDTO.WrongAnswer
        };

        return await _questionDb.UpdateQuestion(q, mcq) ? StatusCode(201) : StatusCode(400);
    }

    [HttpPut("true-false/{id}")]
    public async Task<IActionResult> UpdateTrueFalseQuestion(TrueFalseQuestionDTO questionDTO, ulong id)
    {
        Question q = new Question
        {
            Id = id,
            Title = questionDTO.Title,
            Grade = questionDTO.Grade,
            Type = questionDTO.Type,
            Difficulty = questionDTO.Difficulty,
            TopicId = questionDTO.TopicId,
            TeacherId = questionDTO.TeacherId,
            Explanation = questionDTO.Explanation
        };

        TrueFalseQuestion tfq = new TrueFalseQuestion
        {
            Id = id,
            AnswerKey = questionDTO.AnswerKey
        };

        return await _questionDb.UpdateQuestion(q, tfq) ? StatusCode(201) : StatusCode(400);
    }

    [HttpPut("short-answer/{id}")]
    public async Task<IActionResult> UpdateShortAnswerQuestion(ShortAnswerQuestionDTO questionDTO, ulong id)
    {
        Question q = new Question
        {
            Id = id,
            Title = questionDTO.Title,
            Grade = questionDTO.Grade,
            Type = questionDTO.Type,
            Difficulty = questionDTO.Difficulty,
            TopicId = questionDTO.TopicId,
            TeacherId = questionDTO.TeacherId,
            Explanation = questionDTO.Explanation
        };

        ShortAnswerQuestion saq = new ShortAnswerQuestion
        {
            Id = id,
            AnswerKey = questionDTO.AnswerKey
        };

        return await _questionDb.UpdateQuestion(q, saq) ? StatusCode(201) : StatusCode(400);
    }

    [HttpPut("manual-response/{id}")]
    public async Task<IActionResult> UpdateManualResponseQuestion(ManualResponseQuestionDTO questionDTO, ulong id)
    {
        Question q = new Question
        {
            Id = id,
            Title = questionDTO.Title,
            Grade = questionDTO.Grade,
            Type = questionDTO.Type,
            Difficulty = questionDTO.Difficulty,
            TopicId = questionDTO.TopicId,
            TeacherId = questionDTO.TeacherId,
            Explanation = questionDTO.Explanation
        };

        ManualResponseQuestion crq = new ManualResponseQuestion
        {
            Id = id,
            AnswerKeys = questionDTO.AnswerKeys,
            AllowTakePhoto = questionDTO.AllowTakePhoto,
            AllowEnter = questionDTO.AllowEnter,
            MarkAsWrong = questionDTO.MarkAsWrong
        };

        return await _questionDb.UpdateQuestion(q, crq) ? StatusCode(201) : StatusCode(400);
    }

    [HttpPut("sorting/{id}")]
    public async Task<IActionResult> UpdateSortingQuestion(SortingQuestionDTO questionDTO, ulong id)
    {
        Question q = new Question
        {
            Id = id,
            Title = questionDTO.Title,
            Grade = questionDTO.Grade,
            Type = questionDTO.Type,
            Difficulty = questionDTO.Difficulty,
            TopicId = questionDTO.TopicId,
            TeacherId = questionDTO.TeacherId,
            Explanation = questionDTO.Explanation
        };

        SortingQuestion sq = new SortingQuestion
        {
            Id = id,
            CorrectOrder = questionDTO.CorrectOrder
        };

        return await _questionDb.UpdateQuestion(q, sq) ? StatusCode(201) : StatusCode(400);
    }

    [HttpPut("group/{id}")]
    public async Task<IActionResult> UpdateGroupQuestion(GroupQuestionPostDTO questionDTO, ulong id)
    {
        Question q = new Question
        {
            Id = id,
            Title = questionDTO.Title,
            Grade = questionDTO.Grade,
            Type = questionDTO.Type
        };

        GroupQuestion gq = new GroupQuestion
        {
            Id = id,
            PassageTitle = questionDTO.PassageTitle,
            PassageContent = questionDTO.PassageContent,
            PassageAuthor = questionDTO.PassageAuthor,
            Questions = questionDTO.Questions
        };

        return await _questionDb.UpdateQuestion(q, gq) ? StatusCode(201) : StatusCode(400);
    }

    [HttpPut("true-false-THPT/{id}")]
    public async Task<IActionResult> UpdateTrueFalseTHPTQuestion(TrueFalseTHPTQuestionDTO questionDTO, ulong id)
    {
        Question q = new Question
        {
            Id = id,
            Title = questionDTO.Title,
            Grade = questionDTO.Grade,
            Type = questionDTO.Type,
            Difficulty = questionDTO.Difficulty,
            TopicId = questionDTO.TopicId,
            TeacherId = questionDTO.TeacherId,
            Explanation = questionDTO.Explanation
        };

        TrueFalseTHPTQuestion tfq = new TrueFalseTHPTQuestion
        {
            Id = id,
            Statements = questionDTO.Statements,
            AnswerKeys = questionDTO.AnswerKeys
        };

        return await _questionDb.UpdateQuestion(q, tfq) ? StatusCode(201) : StatusCode(400);
    }

    // DELETE
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteQuestion(ulong id)
    {
        var question = await _questionDb.GetQuestionById(id);
        return await _questionDb.DeleteQuestion(question) ? StatusCode(201) : StatusCode(400);
    }
}
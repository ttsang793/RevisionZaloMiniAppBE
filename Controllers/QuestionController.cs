using backend.Models;
using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;
[ApiController]
[Route("/api/[controller]")]
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
    [HttpGet]
    public async Task<List<Question>> GetAllQuestionsByTeacher()
    {
        return await _questionDb.GetAllQuestionsByTeacher();
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
            case "fill-in-the-blank": case "constructed-response": return await GetManualResponseQuestionById(question);
            case "sorting": return await GetSortingQuestionById(question);
            //case "group": return await GetSortingQuestionById(question);
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
            SubjectId = q.SubjectId,
            Explanation = q.Explanation,
            CorrectAnswer = answer.CorrectAnswer,
            WrongAnswer1 = answer.WrongAnswer1,
            WrongAnswer2 = answer.WrongAnswer2,
            WrongAnswer3 = answer.WrongAnswer3,
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
            SubjectId = q.SubjectId,
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
            SubjectId = q.SubjectId,
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
            SubjectId = q.SubjectId,
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
            SubjectId = q.SubjectId,
            Explanation = q.Explanation,
            CorrectOrder = answer.CorrectOrder
        };
    }

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
            SubjectId = q.SubjectId,
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
            Id = questionDTO.Id,
            Title = questionDTO.Title,
            Grade = questionDTO.Grade,
            Type = questionDTO.Type,
            Difficulty = questionDTO.Difficulty,
            TopicId = questionDTO.TopicId,
            SubjectId = questionDTO.SubjectId,
            Explanation = questionDTO.Explanation
        };

        MultipleChoiceQuestion mcq = new MultipleChoiceQuestion
        {
            Id = questionDTO.Id,
            CorrectAnswer = questionDTO.CorrectAnswer,
            WrongAnswer1 = questionDTO.WrongAnswer1,
            WrongAnswer2 = questionDTO.WrongAnswer2,
            WrongAnswer3 = questionDTO.WrongAnswer3
        };

        return await _questionDb.AddMultipleChoiceQuestion(q, mcq) ? StatusCode(201) : StatusCode(400);
    }

    [HttpPost("true-false")]
    public async Task<IActionResult> AddTrueFalseQuestion(TrueFalseQuestionDTO questionDTO)
    {
        Question q = new Question
        {
            Id = questionDTO.Id,
            Title = questionDTO.Title,
            Grade = questionDTO.Grade,
            Type = questionDTO.Type,
            Difficulty = questionDTO.Difficulty,
            TopicId = questionDTO.TopicId,
            SubjectId = questionDTO.SubjectId,
            Explanation = questionDTO.Explanation
        };

        TrueFalseQuestion tfq = new TrueFalseQuestion
        {
            Id = questionDTO.Id,
            AnswerKey = questionDTO.AnswerKey
        };

        return await _questionDb.AddTrueFalseQuestion(q, tfq) ? StatusCode(201) : StatusCode(400);
    }

    [HttpPost("short-answer")]
    public async Task<IActionResult> AddShortAnswerQuestion(ShortAnswerQuestionDTO questionDTO)
    {
        Question q = new Question
        {
            Id = questionDTO.Id,
            Title = questionDTO.Title,
            Grade = questionDTO.Grade,
            Type = questionDTO.Type,
            Difficulty = questionDTO.Difficulty,
            TopicId = questionDTO.TopicId,
            SubjectId = questionDTO.SubjectId,
            Explanation = questionDTO.Explanation
        };

        ShortAnswerQuestion saq = new ShortAnswerQuestion
        {
            Id = questionDTO.Id,
            AnswerKey = questionDTO.AnswerKey
        };

        return await _questionDb.AddShortAnswerQuestion(q, saq) ? StatusCode(201) : StatusCode(400);
    }

    [HttpPost("manual-response")]
    public async Task<IActionResult> AddManualResponseQuestion(ManualResponseQuestionDTO questionDTO)
    {
        Question q = new Question
        {
            Id = questionDTO.Id,
            Title = questionDTO.Title,
            Grade = questionDTO.Grade,
            Type = questionDTO.Type,
            Difficulty = questionDTO.Difficulty,
            TopicId = questionDTO.TopicId,
            SubjectId = questionDTO.SubjectId,
            Explanation = questionDTO.Explanation
        };

        ManualResponseQuestion crq = new ManualResponseQuestion
        {
            Id = questionDTO.Id,
            AnswerKeys = questionDTO.AnswerKeys,
            AllowTakePhoto = questionDTO.AllowTakePhoto,
            AllowEnter = questionDTO.AllowEnter,
            MarkAsWrong = questionDTO.MarkAsWrong
        };

        return await _questionDb.AddManualResponseQuestion(q, crq) ? StatusCode(201) : StatusCode(400);
    }

    [HttpPost("sorting")]
    public async Task<IActionResult> AddSortingQuestion(SortingQuestionDTO questionDTO)
    {
        Question q = new Question
        {
            Id = questionDTO.Id,
            Title = questionDTO.Title,
            Grade = questionDTO.Grade,
            Type = questionDTO.Type,
            Difficulty = questionDTO.Difficulty,
            TopicId = questionDTO.TopicId,
            SubjectId = questionDTO.SubjectId,
            Explanation = questionDTO.Explanation
        };

        SortingQuestion sq = new SortingQuestion
        {
            Id = questionDTO.Id,
            CorrectOrder = questionDTO.CorrectOrder
        };

        return await _questionDb.AddSortingQuestion(q, sq) ? StatusCode(201) : StatusCode(400);
    }

    [HttpPost("true-false-THPT")]
    public async Task<IActionResult> AddTrueFalseTHPTQuestion(TrueFalseTHPTQuestionDTO questionDTO)
    {
        Question q = new Question
        {
            Id = questionDTO.Id,
            Title = questionDTO.Title,
            Grade = questionDTO.Grade,
            Type = questionDTO.Type,
            Difficulty = questionDTO.Difficulty,
            TopicId = questionDTO.TopicId,
            SubjectId = questionDTO.SubjectId,
            Explanation = questionDTO.Explanation,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        TrueFalseTHPTQuestion tfq = new TrueFalseTHPTQuestion
        {
            Id = questionDTO.Id,
            Statements = questionDTO.Statements,
            AnswerKeys = questionDTO.AnswerKeys
        };

        return await _questionDb.AddTrueFalseTHPTQuestion(q, tfq) ? StatusCode(201) : StatusCode(400);
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
            SubjectId = questionDTO.SubjectId,
            Explanation = questionDTO.Explanation,
            UpdatedAt = DateTime.Now
        };

        MultipleChoiceQuestion mcq = new MultipleChoiceQuestion
        {
            Id = id,
            CorrectAnswer = questionDTO.CorrectAnswer,
            WrongAnswer1 = questionDTO.WrongAnswer1,
            WrongAnswer2 = questionDTO.WrongAnswer2,
            WrongAnswer3 = questionDTO.WrongAnswer3,
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
            SubjectId = questionDTO.SubjectId,
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
            SubjectId = questionDTO.SubjectId,
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
            SubjectId = questionDTO.SubjectId,
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
            SubjectId = questionDTO.SubjectId,
            Explanation = questionDTO.Explanation
        };

        SortingQuestion sq = new SortingQuestion
        {
            Id = questionDTO.Id,
            CorrectOrder = questionDTO.CorrectOrder
        };

        return await _questionDb.UpdateQuestion(q, sq) ? StatusCode(201) : StatusCode(400);
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
            SubjectId = questionDTO.SubjectId,
            Explanation = questionDTO.Explanation
        };

        TrueFalseTHPTQuestion tfq = new TrueFalseTHPTQuestion
        {
            Id = questionDTO.Id,
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
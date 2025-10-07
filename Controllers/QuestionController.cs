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
            case 1: return await GetMultipleChoiceQuestionById(question);
            case 2: return await GetTrueFalseQuestionById(question);
            case 3: return await GetShortAnswerQuestionById(question);
            case 4: return await GetFillInTheBlankQuestionById(question);
            case 5: return await GetConstructedResponseQuestionById(question);
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
            Explaination = q.Explaination,
            AnswerA = answer.AnswerA,
            AnswerB = answer.AnswerB,
            AnswerC = answer.AnswerC,
            AnswerD = answer.AnswerD,
            AnswerKey = answer.AnswerKey
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
            Explaination = q.Explaination,
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
            Explaination = q.Explaination,
            AnswerKey = answer.AnswerKey
        };
    }

    private async Task<FillInTheBlankQuestionDTO> GetFillInTheBlankQuestionById(Question q)
    {
        var answer = await _questionDb.GetFillInTheBlankQuestionById(q.Id);
        return new FillInTheBlankQuestionDTO
        {
            Id = q.Id,
            Title = q.Title,
            Grade = q.Grade,
            Type = q.Type,
            Difficulty = q.Difficulty,
            TopicId = q.TopicId,
            SubjectId = q.SubjectId,
            Explaination = q.Explaination,
            AnswerKeys = answer.AnswerKeys,
            MarkAsWrong = answer.MarkAsWrong
        };
    }

    private async Task<ConstructedResponseQuestionDTO> GetConstructedResponseQuestionById(Question q)
    {
        var answer = await _questionDb.GetConstructedResponseQuestionById(q.Id);
        return new ConstructedResponseQuestionDTO
        {
            Id = q.Id,
            Title = q.Title,
            Grade = q.Grade,
            Type = q.Type,
            Difficulty = q.Difficulty,
            TopicId = q.TopicId,
            SubjectId = q.SubjectId,
            Explaination = q.Explaination,
            AnswerKeys = answer.AnswerKeys,
            AllowTakePhoto = answer.AllowTakePhoto,
            AllowEnter = answer.AllowEnter,
            MarkAsWrong = answer.MarkAsWrong
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
            Explaination = questionDTO.Explaination,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        MultipleChoiceQuestion mcq = new MultipleChoiceQuestion
        {
            Id = questionDTO.Id,
            AnswerA = questionDTO.AnswerA,
            AnswerB = questionDTO.AnswerB,
            AnswerC = questionDTO.AnswerC,
            AnswerD = questionDTO.AnswerD,
            AnswerKey = questionDTO.AnswerKey
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
            Explaination = questionDTO.Explaination,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
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
            Explaination = questionDTO.Explaination,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        ShortAnswerQuestion saq = new ShortAnswerQuestion
        {
            Id = questionDTO.Id,
            AnswerKey = questionDTO.AnswerKey
        };

        return await _questionDb.AddShortAnswerQuestion(q, saq) ? StatusCode(201) : StatusCode(400);
    }

    [HttpPost("fill-in-the-blank")]
    public async Task<IActionResult> AddFillInTheBlankQuestion(FillInTheBlankQuestionDTO questionDTO)
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
            Explaination = questionDTO.Explaination,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        FillInTheBlankQuestion fitbq = new FillInTheBlankQuestion
        {
            Id = questionDTO.Id,
            AnswerKeys = questionDTO.AnswerKeys,
            MarkAsWrong = questionDTO.MarkAsWrong
        };

        return await _questionDb.AddFillInTheBlankQuestion(q, fitbq) ? StatusCode(201) : StatusCode(400);
    }

    [HttpPost("constructed-response")]
    public async Task<IActionResult> AddConstructedResponseQuestion(ConstructedResponseQuestionDTO questionDTO)
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
            Explaination = questionDTO.Explaination,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        ConstructedResponseQuestion crq = new ConstructedResponseQuestion
        {
            Id = questionDTO.Id,
            AnswerKeys = questionDTO.AnswerKeys,
            AllowTakePhoto = questionDTO.AllowTakePhoto,
            AllowEnter = questionDTO.AllowEnter,
            MarkAsWrong = questionDTO.MarkAsWrong
        };

        return await _questionDb.AddConstructedResponseQuestion(q, crq) ? StatusCode(201) : StatusCode(400);
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
            Explaination = questionDTO.Explaination,
            UpdatedAt = DateTime.Now
        };

        MultipleChoiceQuestion mcq = new MultipleChoiceQuestion
        {
            Id = id,
            AnswerA = questionDTO.AnswerA,
            AnswerB = questionDTO.AnswerB,
            AnswerC = questionDTO.AnswerC,
            AnswerD = questionDTO.AnswerD,
            AnswerKey = questionDTO.AnswerKey
        };

        return await _questionDb.UpdateMultipleChoiceQuestion(q, mcq) ? StatusCode(201) : StatusCode(400);
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
            Explaination = questionDTO.Explaination,
            UpdatedAt = DateTime.Now
        };

        TrueFalseQuestion tfq = new TrueFalseQuestion
        {
            Id = id,
            AnswerKey = questionDTO.AnswerKey
        };

        return await _questionDb.UpdateTrueFalseQuestion(q, tfq) ? StatusCode(201) : StatusCode(400);
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
            Explaination = questionDTO.Explaination,
            UpdatedAt = DateTime.Now
        };

        ShortAnswerQuestion saq = new ShortAnswerQuestion
        {
            Id = id,
            AnswerKey = questionDTO.AnswerKey
        };

        return await _questionDb.UpdateShortAnswerQuestion(q, saq) ? StatusCode(201) : StatusCode(400);
    }

    [HttpPut("fill-in-the-blank/{id}")]
    public async Task<IActionResult> UpdateFillInTheBlankQuestion(FillInTheBlankQuestionDTO questionDTO, ulong id)
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
            Explaination = questionDTO.Explaination,
            UpdatedAt = DateTime.Now
        };

        FillInTheBlankQuestion fitbq = new FillInTheBlankQuestion
        {
            Id = id,
            AnswerKeys = questionDTO.AnswerKeys,
            MarkAsWrong = questionDTO.MarkAsWrong
        };

        return await _questionDb.UpdateFillInTheBlankQuestion(q, fitbq) ? StatusCode(201) : StatusCode(400);
    }

    [HttpPut("constructed-response/{id}")]
    public async Task<IActionResult> UpdateConstructedResponseQuestion(ConstructedResponseQuestionDTO questionDTO, ulong id)
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
            Explaination = questionDTO.Explaination,
            UpdatedAt = DateTime.Now
        };

        ConstructedResponseQuestion crq = new ConstructedResponseQuestion
        {
            Id = id,
            AnswerKeys = questionDTO.AnswerKeys,
            AllowTakePhoto = questionDTO.AllowTakePhoto,
            AllowEnter = questionDTO.AllowEnter,
            MarkAsWrong = questionDTO.MarkAsWrong
        };

        return await _questionDb.UpdateConstructedResponseQuestion(q, crq) ? StatusCode(201) : StatusCode(400);
    }

    // DELETE
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteQuestion(ulong id)
    {
        var question = await _questionDb.GetQuestionById(id);
        return await _questionDb.DeleteQuestion(question) ? StatusCode(201) : StatusCode(400);
    }
}
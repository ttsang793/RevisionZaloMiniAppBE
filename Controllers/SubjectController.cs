using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class SubjectController : Controller
{
    private readonly ILogger<SubjectController> _logger;
    private readonly SubjectDb _subjectDb;

    public SubjectController(ILogger<SubjectController> logger, ZaloRevisionAppDbContext dbContext)
    {
        _logger = logger;
        _subjectDb = new SubjectDb(dbContext);
    }

    [HttpGet("")]
    public async Task<List<Subject>> Get()
    {
        return await _subjectDb.GetAsync();
    }

    [HttpGet("{id}")]
    public async Task<Subject> GetById(string id)
    {
        return await _subjectDb.GetByIdAsync(id);
    }

    [HttpPost("")]
    public async Task<IActionResult> Add([Bind("Id", "Name", "Classes", "QuestionTN", "QuestionDS", "QuestionTLN", "QuestionDVCT", "QuestionTL", "QuestionSX", "IsVisible")] Subject subject)
    {
        return await _subjectDb.Add(subject) ? StatusCode(201) : StatusCode(400);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([Bind("Id", "Name", "Classes", "QuestionTN", "QuestionDS", "QuestionTLN", "QuestionDVCT", "QuestionTL", "QuestionSX", "IsVisible")] Subject subject)
    {
        return await _subjectDb.Update(subject) ? StatusCode(200) : StatusCode(400);
    }

    [HttpDelete("visible/{id}")]
    public async Task<IActionResult> ChangeVisible(string id)
    {
        return await _subjectDb.ChangeVisible(id) ? StatusCode(200) : StatusCode(400);
    }
}

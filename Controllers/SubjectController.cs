using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("/api/subject")]
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

    [HttpGet("active")]
    public async Task<List<Subject>> GetActive()
    {
        return await _subjectDb.GetActiveAsync();
    }

    [HttpGet("{id}")]
    public async Task<Subject> GetById(string id)
    {
        return await _subjectDb.GetByIdAsync(id);
    }

    [HttpGet("{id}/grade")]
    public async Task<ICollection<byte>> GetGradesById(string id)
    {
        return await _subjectDb.GetGradesByIdAsync(id);
    }

    [HttpGet("grade/{grade}")]
    public async Task<List<Subject>> GetbyGrade(byte grade)
    {
        return await _subjectDb.GetByGradeAsync(grade);
    }

    [HttpPost("")]
    public async Task<IActionResult> Add(Subject subject)
    {
        sbyte response = await _subjectDb.Add(subject);
        switch (response)
        {
            case -1: return StatusCode(409, new { NameError = "Tên môn học đã tồn tại!" } );
            case 1: return StatusCode(201);
            default: return StatusCode(400);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Subject subject)
    {
        sbyte response = await _subjectDb.Update(subject);
        switch (response)
        {
            case -1: return StatusCode(409, new { NameError = "Tên môn học đã tồn tại!" });
            case 1: return StatusCode(201);
            default: return StatusCode(400);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> ChangeVisible(string id)
    {
        return await _subjectDb.ChangeVisible(id) ? StatusCode(200) : StatusCode(400);
    }
}

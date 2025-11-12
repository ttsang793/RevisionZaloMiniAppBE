using backend.Models;
using backend.Services;
using backend.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("/api/topic")]
public class TopicController : Controller
{
    private readonly ILogger<TopicController> _logger;
    private readonly TopicDb _topicDb;

    public TopicController(ILogger<TopicController> logger, ZaloRevisionAppDbContext dbContext)
    {
        _logger = logger;
        _topicDb = new TopicDb(dbContext);
    }

    [HttpGet]
    public async Task<List<Topic>> Get([FromQuery] string? name)
    {
        if (!string.IsNullOrEmpty(name)) return await _topicDb.GetByNameAsync(name);
        return await _topicDb.GetAsync();
    }

    [HttpGet("active")]
    public async Task<List<Topic>> GetActive()
    {
        return await _topicDb.GetActiveAsync();
    }

    [HttpGet("{id}")]
    public async Task<Topic> GetById(string id)
    {
        return await _topicDb.GetByIdAsync(id);
    }

    [HttpPost]
    public async Task<IActionResult> Add([Bind("Name", "Classes", "SubjectId")] TopicDTO topicDTO)
    {
        Topic topic = new Topic { Name = topicDTO.Name, Grades = topicDTO.Grades, SubjectId = topicDTO.SubjectId };
        return await _topicDb.Add(topic) ? StatusCode(201) : StatusCode(400);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([Bind("Id", "Name", "Classes", "SubjectId")] TopicDTO topicDTO, string id)
    {
        Topic topic = new Topic { Id = id, Name = topicDTO.Name, Grades = topicDTO.Grades, SubjectId = topicDTO.SubjectId };
        return await _topicDb.Update(topic) ? StatusCode(200) : StatusCode(400);
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> ChangeVisible(string id)
    {
        return await _topicDb.ChangeVisible(id) ? StatusCode(200) : StatusCode(400);
    }
}
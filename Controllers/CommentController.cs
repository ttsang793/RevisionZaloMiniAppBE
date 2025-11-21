using backend.DTOs;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/api/comment")]
public class CommentController : Controller
{
    private readonly ILogger<CommentController> _logger;
    private readonly CommentDb _commentDb;
    private readonly AchievementDb _achievementDb;

    public CommentController(ILogger<CommentController> logger, ZaloRevisionAppDbContext dbContext)
    {
        _logger = logger;
        _commentDb = new CommentDb(dbContext);
        _achievementDb = new AchievementDb(dbContext);
    }

    [HttpGet("{examId}")]
    public async Task<List<CommentReadDTO>> GetCommentsByExamIdAsync(ulong examId)
    {
        return await _commentDb.GetCommentsAsync(examId);
    }

    [HttpPost]
    public async Task<IActionResult> AddComment(CommentInsertDTO c)
    {
        Comment comment = new Comment
        {
            ExamId = c.ExamId,
            UserId = c.UserId,
            ReplyTo = c.ReplyTo,
            Content = c.Content
        };

        bool result = await _commentDb.AddComment(comment);
        _achievementDb.DetechAchievement(7, c.UserId);

        return result ? StatusCode(201) : StatusCode(400);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteComment(ulong id)
    {
        return await _commentDb.DeleteComment(id) ? StatusCode(200) : StatusCode(400);
    }
}
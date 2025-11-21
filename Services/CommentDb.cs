using backend.DTOs;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class CommentDb
{
    private ZaloRevisionAppDbContext _dbContext;

    public CommentDb(ZaloRevisionAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<CommentReadDTO>> GetCommentsAsync(ulong examId)
    {
        var allComments = await (from c in _dbContext.Comments
                            join u in _dbContext.Users
                            on c.UserId equals u.Id
                            where c.ExamId == examId
                            orderby c.Id descending
                            select new CommentReadDTO
                            {
                                Id = c.Id,
                                UserId = u.Id,
                                UserName = u.Name,
                                UserAvatar = u.Avatar,
                                ExamId = c.ExamId,
                                ReplyTo = c.ReplyTo,
                                Content = c.Content,
                                CreatedAt = c.CreatedAt
                            }).ToListAsync();

        var repliesLookup = allComments
            .Where(c => c.ReplyTo != null)
            .OrderBy(c => c.CreatedAt)
            .GroupBy(c => c.ReplyTo.Value)
            .ToDictionary(g => g.Key, g => g.ToList());

        var result = allComments.Where(c => c.ReplyTo == null).OrderByDescending(c => c.Id).ToList();

        foreach (var comment in result)
            if (repliesLookup.TryGetValue(comment.Id, out var replies)) comment.Replies = replies;

        return result;
    }

    public async Task<Comment> GetCommentById(ulong id)
    {
        return await _dbContext.Comments.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<bool> AddComment(Comment comment)
    {
        _dbContext.Comments.Add(comment);
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteComment(ulong id)
    {
        var deleteComment = await GetCommentById(id);
        _dbContext.Comments.Remove(deleteComment);
        return await _dbContext.SaveChangesAsync() > 0;
    }
}

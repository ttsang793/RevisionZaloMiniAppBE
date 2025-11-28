using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class TopicDb
{
    private ZaloRevisionAppDbContext _dbContext;

    public TopicDb(ZaloRevisionAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Topic>> GetAsync()
    {
        var result = await (from t in _dbContext.Topics
                           join s in _dbContext.Subjects
                           on t.SubjectId equals s.Id
                           select new Topic
                           {
                               Id = t.Id,
                               Name = t.Name,
                               Grades = t.Grades,
                               SubjectId = t.SubjectId,
                               Subject = t.Subject,
                               IsVisible = t.IsVisible && s.IsVisible
                           }).ToListAsync();

        return result;
    }

    public async Task<List<Topic>> GetActiveAsync()
    {
        return await _dbContext.Topics.Where(t => t.IsVisible == true).ToListAsync();
    }

    public async Task<List<Topic>> GetBySubjectIdAsync(string subjectId)
    {
        return await _dbContext.Topics.Where(t => t.SubjectId == subjectId).ToListAsync();
    }

    public async Task<Topic> GetByIdAsync(string id)
    {
        return await _dbContext.Topics.Where(t => t.Id.Equals(id)).FirstAsync();
    }

    public async Task<List<Topic>> GetByNameAsync(string name)
    {
        var result = await (from t in _dbContext.Topics
                            join s in _dbContext.Subjects
                            on t.SubjectId equals s.Id
                            where t.Name.ToLower().Contains(name.ToLower())
                            select new Topic
                            {
                                Id = t.Id,
                                Name = t.Name,
                                Grades = t.Grades,
                                SubjectId = t.SubjectId,
                                Subject = t.Subject,
                                IsVisible = t.IsVisible
                            }).ToListAsync();

        return result;
    }

    public async Task<bool> Add(Topic t)
    {
        int idNumber = await _dbContext.Topics.CountAsync(topic => topic.Id.StartsWith(t.SubjectId));
        t.Id = t.SubjectId + (++idNumber);

        _dbContext.Topics.Add(t);
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> Update(Topic t)
    {
        var oldTopic = await GetByIdAsync(t.Id);
        if (oldTopic == null) return false;

        oldTopic.TakeValuesFrom(t);
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<sbyte> ChangeVisible(string id)
    {
        var topic = await GetByIdAsync(id);
        if (topic == null) return 0;

        var subjectVisible = (await _dbContext.Subjects.FirstAsync(s => s.Id == topic.SubjectId)).IsVisible;
        if (!subjectVisible) return -1;
        
        var oldIsVisible = topic.IsVisible;
        topic.IsVisible = !oldIsVisible;

        return await _dbContext.SaveChangesAsync() > 0 ? (sbyte)1 : (sbyte)0;
    }
}

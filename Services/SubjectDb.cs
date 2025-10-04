using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class SubjectDb
{
    private ZaloRevisionAppDbContext _dbContext;

    public SubjectDb(ZaloRevisionAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Subject>> GetAsync()
    {
        return await _dbContext.Subjects.ToListAsync();
    }

    public async Task<Subject> GetByIdAsync(string id)
    {
        return await _dbContext.Subjects.FindAsync(id);
    }

    public async Task<bool> Add(Subject s)
    {
        _dbContext.Subjects.Add(s);
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> Update(Subject s)
    {
        var oldSubject = await GetByIdAsync(s.Id);
        if (oldSubject == null) return false;

        oldSubject.TakeValuesFrom(s);
        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> ChangeVisible(string id)
    {
        var subject = await GetByIdAsync(id);

        if (subject == null) return false;
        var oldIsVisible = subject.IsVisible;
        subject.IsVisible = !oldIsVisible;

        return await _dbContext.SaveChangesAsync() > 0;
    }
}

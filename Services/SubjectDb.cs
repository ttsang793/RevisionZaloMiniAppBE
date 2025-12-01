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

    public async Task<List<Subject>> GetActiveAsync()
    {
        return await _dbContext.Subjects.Where(s => s.IsVisible == true).ToListAsync();
    }

    public async Task<Subject> GetByIdAsync(string id)
    {
        return await _dbContext.Subjects.FirstAsync(s => s.Id == id);
    }

    public async Task<sbyte> Add(Subject s)
    {
        if (_dbContext.Subjects.Any(sj => sj.Name == s.Name)) return -1;
        _dbContext.Subjects.Add(s);
        return (sbyte)(await _dbContext.SaveChangesAsync() > 0 ? 1 : 0);
    }

    public async Task<sbyte> Update(Subject s)
    {
        var oldSubject = await GetByIdAsync(s.Id);
        if (oldSubject == null) return 0;

        if (_dbContext.Subjects.Any(sj => sj.Name == s.Name && sj.Id != s.Id)) return -1;
        oldSubject.TakeValuesFrom(s);
        return (sbyte)(await _dbContext.SaveChangesAsync() > 0 ? 1 : 0);
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

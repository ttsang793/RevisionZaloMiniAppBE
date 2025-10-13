using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class ExamPartDb
{
    private ZaloRevisionAppDbContext _dbContext;

    public ExamPartDb(ZaloRevisionAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ExamPart>> GetExamPartsAsyncByExamId(ulong id)
    {
        return await _dbContext.ExamParts.Where(e => e.ExamId == id).ToListAsync();
    }

    public async Task<ExamPart> GetExamPartById(ulong id)
    {
        return await _dbContext.ExamParts.FirstOrDefaultAsync(e => e.ExamId == id);
    }

    public async Task<bool> AddExamPart(ExamPart ep)
    {
        _dbContext.ExamParts.Add(ep);

        return await _dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteExamPart(ulong id)
    {
        _dbContext.ExamParts.Remove(await GetExamPartById(id));

        return await _dbContext.SaveChangesAsync() > 0;
    }
}

using backend.Models;

namespace backend.Services;

public class ExamDb
{
    private ZaloRevisionAppDbContext _dbContext;

    public ExamDb(ZaloRevisionAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> AddExam(Exam exam)
    {
        _dbContext.Exams.Add(exam);
        return await _dbContext.SaveChangesAsync() > 0;
    }
}

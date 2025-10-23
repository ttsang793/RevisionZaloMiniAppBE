using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class PdfExamCodeDb
{
    public ZaloRevisionAppDbContext DbContext { get; }

    public PdfExamCodeDb(ZaloRevisionAppDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public async Task<List<PdfExamCode>> GetExamCodeByExamId(ulong examId)
    {
        List<string> examCodes = await DbContext.PdfExamCodes.Where(ec => ec.ExamId == examId).Select(ec => ec.Code).ToListAsync();
        Console.WriteLine(examCodes.Count);
        string currentCode = examCodes[new Random().Next(1, examCodes.Count) - 1];
        Console.WriteLine(currentCode);

        return await DbContext.PdfExamCodes.Where(ec => ec.ExamId == examId && ec.Code == currentCode).ToListAsync();
    }

    public async Task<PdfExamCode?> AddExamCode(PdfExamCode pdfExamCode)
    {
        await DbContext.PdfExamCodes.AddAsync(pdfExamCode);
        bool saved = await DbContext.SaveChangesAsync() > 0;
        return saved ? pdfExamCode : null;
    }

    public async Task<bool> AddExamCodeQuestion(PdfExamCodeQuestion pdfExamCodeQuestion)
    {
        await DbContext.PdfExamCodeQuestions.AddAsync(pdfExamCodeQuestion);
        return await DbContext.SaveChangesAsync() > 0;
    }
}

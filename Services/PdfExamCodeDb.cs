using backend.Models;
using backend.DTOs;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class PdfExamCodeDb
{
    public ZaloRevisionAppDbContext DbContext { get; }

    public PdfExamCodeDb(ZaloRevisionAppDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public async Task<List<PdfExamCodeDTO>> GetAllExamCodesByExamId(ulong examId)
    {
        var result = await (from ec in DbContext.PdfExamCodes
                            where ec.ExamId == examId
                            select new PdfExamCodeDTO
                            {
                                Id = ec.Id,
                                ExamId = ec.ExamId,
                                Code = ec.Code,
                                TaskPdf = ec.TaskPdf,
                                AnswerPdf = ec.AnswerPdf,
                                NumPart = ec.NumPart
                            }
                           ).ToListAsync();

        foreach (var row in result)
        {
            row.Questions = await (from q in DbContext.PdfExamCodeQuestions
                                   where q.PdfExamCodeId == row.Id
                                   select new PdfExamCodeQuestionDTO
                                   {
                                       Type = q.Type,
                                       PartIndex = q.PartIndex,
                                       QuestionIndex = q.QuestionIndex,
                                       AnswerKey = q.AnswerKey,
                                       Point = q.Point
                                   }).ToListAsync();
        }

        return result;
    }

    public async Task<PdfExamCodeDTO> GetExamCodeByExamId(ulong examId, ulong? pdfExamCodeId)
    {
        PdfExamCode currentCode;
        if (pdfExamCodeId == null)
        {
            List<PdfExamCode> examCodes = await DbContext.PdfExamCodes.Where(ec => ec.ExamId == examId).ToListAsync();
            currentCode = examCodes[new Random().Next(1, examCodes.Count + 1) - 1];
            pdfExamCodeId = currentCode.Id;
        }
        else currentCode = await DbContext.PdfExamCodes.Where(ec => ec.Id == pdfExamCodeId).FirstAsync();

        return new PdfExamCodeDTO
        {
            Id = pdfExamCodeId,
            ExamId = currentCode.ExamId,
            Code = currentCode.Code,
            TaskPdf = currentCode.TaskPdf,
            AnswerPdf = currentCode.AnswerPdf,
            NumPart = currentCode.NumPart,
            AllowShowScore = (bool)(await DbContext.Exams.Where(e => e.Id == examId).Select(e => e.AllowShowScore).FirstAsync()),
            Questions = await (from q in DbContext.PdfExamCodeQuestions
                                where q.PdfExamCodeId == currentCode.Id
                                select new PdfExamCodeQuestionDTO
                                {
                                    Type = q.Type,
                                    PartIndex = q.PartIndex,
                                    QuestionIndex = q.QuestionIndex,
                                    AnswerKey = q.AnswerKey,
                                    Point = q.Point
                                }).ToListAsync()
        };
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

    public async Task<bool> UpdatePdfExamUrl(ulong id, List<string?> files)
    {
        var result = await DbContext.PdfExamCodes.Where(pea => pea.Id == id).FirstAsync();
        result.TaskPdf = files[0];
        result.AnswerPdf = files[1];

        DbContext.PdfExamCodes.Update(result);
        return await DbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteExamCodes(ulong examId)
    {
        var deleted = await DbContext.PdfExamCodes.Where(pea => pea.ExamId == examId).ToListAsync();
        if (deleted.Count == 0) return true;        
        DbContext.RemoveRange(deleted);
        return await DbContext.SaveChangesAsync() > 0;
    }
}

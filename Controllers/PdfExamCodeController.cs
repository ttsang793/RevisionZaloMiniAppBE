using backend.Models;
using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;
[ApiController]
[Route("/api/pdf-exam-code")]
public class PdfExamCodeController : Controller
{
    private readonly ILogger<PdfExamCodeController> _logger;
    private readonly PdfExamCodeDb _pdfExamCodeDb;
    private readonly ExamDb _examDb;

    public PdfExamCodeController(ILogger<PdfExamCodeController> logger, ZaloRevisionAppDbContext dbContext)
    {
        _logger = logger;
        _pdfExamCodeDb = new PdfExamCodeDb(dbContext);
        _examDb = new ExamDb(dbContext);
    }

    [HttpGet("{examId}/all")]
    public async Task<List<PdfExamCodeDTO>> GetAllExamCodesByExamId(ulong examId)
    {
        return await _pdfExamCodeDb.GetAllExamCodesByExamId(examId);
    }

    [HttpGet("{examId}/{pdfExamCodeId?}")]
    public async Task<PdfExamCodeDTO> GetExamCodeByExamId(ulong examId, ulong? pdfExamCodeId)
    {
        return await _pdfExamCodeDb.GetExamCodeByExamId(examId, pdfExamCodeId);
    }

    [HttpPost]
    public async Task<IActionResult> AddPdfExamCodes(List<PdfExamCodeDTO> pdfExamCodeDTOList)
    {
        try
        {
            List<ulong> newIds = [];
            ulong examId = pdfExamCodeDTOList.ElementAt(0).ExamId;
            byte status = 1;
            await _pdfExamCodeDb.DeleteExamCodes(examId);

            await using var transaction = await _pdfExamCodeDb.DbContext.Database.BeginTransactionAsync();

            foreach (var pdfExamCodeDTO in pdfExamCodeDTOList)
            {
                if (pdfExamCodeDTO.Status == 0) status = 0;

                var pdfExamCode = new PdfExamCode
                {
                    ExamId = pdfExamCodeDTO.ExamId,
                    Code = pdfExamCodeDTO.Code,
                    TaskPdf = pdfExamCodeDTO.TaskPdf,
                    AnswerPdf = pdfExamCodeDTO.AnswerPdf,
                    NumPart = pdfExamCodeDTO.NumPart
                };

                var pdfExamCodeAdded = await _pdfExamCodeDb.AddExamCode(pdfExamCode);
                if (pdfExamCodeAdded == null) return StatusCode(400, "Failed to add PDF exam code!");

                var newId = pdfExamCodeAdded.Id;
                newIds.Add(newId);
                foreach (var q in pdfExamCodeDTO.Questions)
                {
                    var pdfExamCodeQuestion = new PdfExamCodeQuestion
                    {
                        AnswerKey = q.AnswerKey,
                        PartIndex = q.PartIndex,
                        QuestionIndex = q.QuestionIndex,
                        PdfExamCodeId = newId,
                        Point = q.Point,
                        Type = q.Type
                    };

                    bool questionId = await _pdfExamCodeDb.AddExamCodeQuestion(pdfExamCodeQuestion);
                    if (!questionId)
                    {
                        await transaction.RollbackAsync();
                        return StatusCode(400, "Failed to add one of the PDF exam questions.");
                    }
                }
            }

            await _examDb.UpdateExam(examId, status);
            await transaction.CommitAsync();
            return StatusCode(201, new { Id = newIds });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error adding PDF exam code(s) on {DateTime.Now}");
            return StatusCode(500);
        }
    }
}
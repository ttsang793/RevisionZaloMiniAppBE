using backend.Services;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/upload")]
public class UploadController : Controller
{
    private readonly ILogger<UploadController> _logger;
    private readonly UploadService _uploadService;
    private readonly QuestionDb _questionDb;
    private readonly PdfExamCodeDb _pdfExamCodeDb;

    public UploadController(ILogger<UploadController> logger, Cloudinary cloudinary, ZaloRevisionAppDbContext dbContext)
    {
        _logger = logger;
        _uploadService = new UploadService(cloudinary);
        _questionDb = new QuestionDb(dbContext);
        _pdfExamCodeDb = new PdfExamCodeDb(dbContext);
    }

    [HttpPost("image/{id}/{type}")]
    public async Task<IActionResult> UploadImage(ulong id, string type, IFormFile file)
    {
        try
        {
            var result = await _uploadService.UploadImageAsync(file);

            if (result.Error != null)
            {
                _logger.LogError($"Lỗi tải file: {result.Error.Message}");
                return StatusCode(404, new { message = result.Error.Message });
            }

            string url = result.SecureUrl.ToString();
            var response = (type == "question") ? await _questionDb.UpdateQuestionImage(id, url) : true;
            return response ? StatusCode(200) : StatusCode(404);
        }
        
        catch (ArgumentException ae)
        {
            return StatusCode(404, new { Error = ae.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Upload lỗi");
            return StatusCode(500, new { Error = "Lỗi hệ thống! Vui lòng thử lại sau!" });
        }
    }

    [HttpPost("pdf/{id}")]
    public async Task<IActionResult> UploadPDF(ulong id, List<IFormFile> files)
    {
        List<string?> fileList = [];

        foreach (IFormFile file in files)
        {
            try
            {
                var result = await _uploadService.UploadPDFAsync(file);

                if (result.Error != null) fileList.Add(null);
                else fileList.Add(result.SecureUrl.ToString());
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Upload lỗi");
                fileList.Add(null);
            }
        }

        return await _pdfExamCodeDb.UpdatePdfExamUrl(id, fileList) ? StatusCode(200) : StatusCode(404);
    }
}
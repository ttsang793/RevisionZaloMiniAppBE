using backend.DTOs;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/admin")]
public class AdminController : Controller
{
    private readonly ILogger<QuestionController> _logger;
    private readonly AdminDb _adminDb;

    public AdminController(ILogger<QuestionController> logger, ZaloRevisionAppDbContext dbContext)
    {
        _logger = logger;
        _adminDb = new AdminDb(dbContext);
    }

    [HttpPost]
    public async Task<IActionResult> Login(AdminDTO adminDTO)
    {
        var result = await _adminDb.VerifyAdmin(adminDTO);
        return (result is UserDTO) ? StatusCode(200, result) : StatusCode(400, result);
    }
    
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(AdminResetPassDTO adminDTO)
    {
        switch (await _adminDb.ResetPassword(adminDTO))
        {
            case -1: return StatusCode(400, new { Old = "Mật khẩu cũ sai!" });
            case 0: return StatusCode(404);
            case 1: return StatusCode(200);
            default: return StatusCode(500);
        }
    }
}

using backend.DTOs;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Identity.Data;
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

    /*
    [HttpPost]
    public async Task<IActionResult> ResetPassword(AdminDTO adminDTO)
    {
        return await _adminDb.ResetPassword(adminDTO) ? StatusCode(200) : StatusCode(400);
    }*/
}

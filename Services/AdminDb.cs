using backend.Models;
using backend.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class AdminDb
{
    private ZaloRevisionAppDbContext _dbContext;
    private readonly PasswordHasher<Admin> _passwordHasher = new PasswordHasher<Admin>();

    public AdminDb(ZaloRevisionAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<dynamic> VerifyAdmin(AdminDTO adminDTO)
    {
        var admin = await _dbContext.Admins.FirstOrDefaultAsync(a => a.Id == adminDTO.Id);
        if (admin == null) return new AdminErrorDTO { IdError = "Vui lòng kiểm tra ID tài khoản!" };

        var verifyResult = _passwordHasher.VerifyHashedPassword(admin, admin.Password, adminDTO.Password);
        if (verifyResult == PasswordVerificationResult.Success)
        {
            var user = await _dbContext.Users.FirstAsync(u => u.Id == adminDTO.Id);
            return new UserDTO { Name = user.Name, Avatar = user.Avatar };
        }
        return new AdminErrorDTO { PasswordError = "Nhập sai mật khẩu!" };
    }

    public async Task<bool> ResetPassword(AdminDTO adminDTO)
    {
        var admin = await _dbContext.Admins.FirstOrDefaultAsync(a => a.Id == adminDTO.Id);
        if (admin == null) return false;

        admin.Password = _passwordHasher.HashPassword(admin, adminDTO.Password);
        return await _dbContext.SaveChangesAsync() > 0;
    }
}

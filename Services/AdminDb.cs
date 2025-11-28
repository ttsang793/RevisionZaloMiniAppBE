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
            try
            {
                return await (from u in _dbContext.Users
                              where u.Id == admin.Id
                              select new UserDTO { Name = u.Name, Avatar = u.Avatar }
                             ).FirstAsync();
            }
            catch { }
        }
        return new AdminErrorDTO { PasswordError = "Nhập sai mật khẩu!" };
    }

    public async Task<sbyte> ResetPassword(AdminResetPassDTO adminDTO)
    {
        var admin = await _dbContext.Admins.FirstOrDefaultAsync(a => a.Id == adminDTO.Id);
        if (admin == null) return -2;

        var verifyResult = _passwordHasher.VerifyHashedPassword(admin, admin.Password, adminDTO.OldPassword);
        if (verifyResult == PasswordVerificationResult.Success)
        {
            admin.Password = _passwordHasher.HashPassword(admin, adminDTO.NewPassword);
            return (sbyte)(await _dbContext.SaveChangesAsync() > 0 ? 1 : 0);
        }
        return -1;
    }
}

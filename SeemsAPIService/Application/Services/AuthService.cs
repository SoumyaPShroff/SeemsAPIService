using Microsoft.EntityFrameworkCore;
using SeemsAPIService.Application.DTOs;
using SeemsAPIService.Application.Interfaces;
using System.Text;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _repo;

    public AuthService(IAuthRepository repo)
    {
        _repo = repo;
    }
 
    public async Task<bool> VerifyLoginAsync(LoginRequestDto dto)
    {
        var user = await _repo.GetUserAsync(dto.LoginID);
        if (user == null) return false;

        // return BCrypt.Net.BCrypt.Verify(dto.Password, user.Password);
        string encodedPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(dto.Password));
        return user.Password == encodedPassword;
    }

    public async Task ResetPasswordAsync(string loginId, string newPassword)
    {
        //string hash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        // await _repo.UpdatePasswordAsync(loginId, hash);
        string encodedPassword = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(newPassword));  
        await _repo.UpdatePasswordAsync(loginId, encodedPassword);
    }

    public Task<string?> GetEmailByLoginIdAsync(string loginId)
        => _repo.GetEmailByLoginIdAsync(loginId);
}

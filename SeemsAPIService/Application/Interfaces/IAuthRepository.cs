using SeemsAPIService.Domain.Entities;

public interface IAuthRepository
{
    Task<Login?> GetUserAsync(string loginId);
    Task UpdatePasswordAsync(string loginId, string hashedPassword);
    Task<string?> GetEmailByLoginIdAsync(string loginId);
}


using SeemsAPIService.Application.DTOs;

// Auth only
public interface IAuthService
{
    Task<bool> VerifyLoginAsync(LoginRequestDto dto);
    Task ResetPasswordAsync(string loginId, string newPassword);
    Task<string?> GetEmailByLoginIdAsync(string loginId);
}

//// User data only
//public interface IUserQueryService
//{
//    Task<string?> GetUserNameAsync(string loginId);
//    Task<List<object>> GetSalesManagersAsync();
//    Task<List<object>> GetDesignManagersAsync();
//    Task<List<object>> GetAnalysisManagersAsync();
//}
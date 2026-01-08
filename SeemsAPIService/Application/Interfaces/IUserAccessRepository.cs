using SeemsAPIService.Application.DTOs;

namespace SeemsAPIService.Application.Interfaces
{
    public interface IUserAccessRepository
    {
        Task<bool> HasRoleAccessAsync(string role, string pageName);
        Task<string?> GetUserDesignationAsync(string loginId);
        Task<List<EmployeeBasicDto>> GetAllActiveEmployeesAsync();
        Task<List<ManagerDto>> GetAnalysisManagersAsync();
        Task<List<ManagerDto>> GetDesignManagersAsync();
    }
}

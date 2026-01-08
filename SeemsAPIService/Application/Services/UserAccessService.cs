using SeemsAPIService.Application.DTOs;
using SeemsAPIService.Application.Interfaces;

namespace SeemsAPIService.Application.Services
{
    public class UserAccessService : IUserAccessService
    {
        private readonly IUserAccessRepository _repo;

        public UserAccessService(IUserAccessRepository repo)
        {
            _repo = repo;
        }

        public Task<bool> HasRoleAccessAsync(string role, string pageName)
            => _repo.HasRoleAccessAsync(role, pageName);

        public Task<string?> GetUserDesignationAsync(string loginId)
            => _repo.GetUserDesignationAsync(loginId);

        public Task<List<EmployeeBasicDto>> GetAllActiveEmployeesAsync()
            => _repo.GetAllActiveEmployeesAsync();

        public Task<List<ManagerDto>> GetAnalysisManagersAsync()
            => _repo.GetAnalysisManagersAsync();

        public Task<List<ManagerDto>> GetDesignManagersAsync()
            => _repo.GetDesignManagersAsync();
    }
}

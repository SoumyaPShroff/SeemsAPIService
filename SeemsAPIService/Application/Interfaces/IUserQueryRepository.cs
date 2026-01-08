using SeemsAPIService.Application.DTOs;
using SeemsAPIService.Domain.Entities;

namespace SeemsAPIService.Application.Interfaces
{
    public interface IUserQueryRepository
    {
        Task<List<HOPCManagerList>> GetHOPCManagerListAsync();
        Task<List<ManagerCostCenterDto>> GetManagerCostcenterInfoAsync(string loginId);
        Task<List<string?>> GetEmailIDsAsync(List<string> loginIds);
    }
}

using SeemsAPIService.Application.DTOs;
using SeemsAPIService.Domain.Entities;

namespace SeemsAPIService.Application.Interfaces
{
    public interface IUserQueryService
    {
        Task<List<HOPCManagerList>> GetHOPCManagerListAsync();
        Task<List<ManagerCostCenterDto>> GetManagerCostcenterInfoAsync(string loginId);
        Task<object> GetEmailIDsAsync(string loginIds);
    }

}

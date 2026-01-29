using SeemsAPIService.Domain.Entities;

namespace SeemsAPIService.Application.Interfaces
{
    public interface IReusableService
    {
        Task<string> GetUserNameAsync(string loginId);
        Task<string> GetUserEmaiIdAsync(string loginId);
        Task<List<se_stages_tools>> GetStageToolsAsync(long? toolId);
        Task<List<setting_employee>> GetHOPCTasksAsync(long? taskId);
    }
}

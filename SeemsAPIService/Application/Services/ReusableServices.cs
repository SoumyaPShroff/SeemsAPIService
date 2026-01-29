using SeemsAPIService.Application.Interfaces;
using SeemsAPIService.Domain.Entities;
namespace SeemsAPIService.Application.Services
{
    public class ReusableService : IReusableService
    {
        private readonly IReusableRepository _repository;

        public ReusableService(IReusableRepository repository)
        {
            _repository = repository;
        }

        public async Task<string> GetUserNameAsync(string loginId)
        {
            if (string.IsNullOrWhiteSpace(loginId))
                return string.Empty;

            return await _repository.GetUserNameAsync(loginId);
        }

        public async Task<string> GetUserEmaiIdAsync(string loginId)
        {
            if (string.IsNullOrWhiteSpace(loginId))
                return string.Empty;

            return await _repository.GetUserEmaiIdAsync(loginId);
        }

        public Task<List<se_stages_tools>> GetStageToolsAsync(long? toolId)
            => _repository.GetStageToolsAsync(toolId);

        public Task<List<setting_employee>> GetHOPCTasksAsync(long? taskId)
            => _repository.GetHOPCTasksAsync(taskId);
    }
}

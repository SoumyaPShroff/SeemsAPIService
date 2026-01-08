using SeemsAPIService.Application.Interfaces;
using SeemsAPIService.Application.DTOs;
using SeemsAPIService.Domain.Entities;

namespace SeemsAPIService.Application.Services
{
    public class CommonQueryService : ICommonQueryService
    {
        private readonly ICommonQueryRepository _repo;

        public CommonQueryService(ICommonQueryRepository repo)
        {
            _repo = repo;
        }

        public Task<List<SalesManagerDto>> GetSalesManagersAsync()
            => _repo.GetSalesManagersAsync();

        public Task<List<SalesNpiUserDto>> GetSalesNpiUsersAsync()
            => _repo.GetSalesNpiUsersAsync();

        public Task<List<SalesEnqRecipientDto>> GetSalesEnqRecipientsAsync()
            => _repo.GetSalesEnqRecipientsAsync();

        public Task<List<SidebarAccessMenus>> GetSideBarAccessMenusAsync(int designationId)
            => _repo.GetSideBarAccessMenusAsync(designationId);

        public Task<long> GetRoleDesignIdAsync(string designationName)
            => _repo.GetRoleDesignIdAsync(designationName);
    }
}

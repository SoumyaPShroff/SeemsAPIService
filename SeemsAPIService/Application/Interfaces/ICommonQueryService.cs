using SeemsAPIService.Application.DTOs;
using SeemsAPIService.Domain.Entities;
namespace SeemsAPIService.Application.Interfaces

{
public interface ICommonQueryService
{
    Task<List<SalesManagerDto>> GetSalesManagersAsync();
    Task<List<SalesNpiUserDto>> GetSalesNpiUsersAsync();
    Task<List<SalesEnqRecipientDto>> GetSalesEnqRecipientsAsync();
    Task<List<SidebarAccessMenus>> GetSideBarAccessMenusAsync(int designationId);
    Task<long> GetRoleDesignIdAsync(string designationName);
}
}

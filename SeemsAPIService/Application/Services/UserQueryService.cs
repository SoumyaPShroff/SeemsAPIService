using SeemsAPIService.Application.DTOs;
using SeemsAPIService.Application.Interfaces;
using SeemsAPIService.Domain.Entities;

public class UserQueryService : IUserQueryService
{
    private readonly IUserQueryRepository _repo;

    public UserQueryService(IUserQueryRepository repo)
    {
        _repo = repo;
    }

    // -------- HOPCManagerList --------
    public async Task<List<HOPCManagerList>> GetHOPCManagerListAsync()
    {
        return await _repo.GetHOPCManagerListAsync();
    }

    // -------- ManagerCostcenterInfo --------
    public async Task<List<ManagerCostCenterDto>> GetManagerCostcenterInfoAsync(string loginId)
    {
        return await _repo.GetManagerCostcenterInfoAsync(loginId);
    }

    // -------- GetEmailIDs --------
    public async Task<object> GetEmailIDsAsync(string loginIds)
    {
        var ids = loginIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                          .Select(id => id.Trim())
                          .ToList();

        var emails = await _repo.GetEmailIDsAsync(ids);

        if (emails == null || emails.Count == 0)
            return new List<string>();

        // match old behaviour
        return emails.Count == 1 ? emails.First()! : emails;
    }
}

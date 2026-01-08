using SeemsAPIService.Application.Interfaces;
using SeemsAPIService.Domain.Entities;

public class JobService : IJobService
{
    private readonly IJobRepository _repo;

    public JobService(IJobRepository repo)
    {
        _repo = repo;
    }

    public Task<Job?> GetJobByNumberAsync(string jobNumber)
        => _repo.GetJobByNumberAsync(jobNumber);

    public Task<string?> GetJobStatusAsync(string jobNumber)
        => _repo.GetJobStatusAsync(jobNumber);

    public Task<List<BillingPlannerRpt>> GetBillingPlannerAsync(string start, string end, string? costcenter)
        => _repo.GetBillingPlannerAsync(start, end, costcenter);

    public Task<List<Invoicedictionary>> GetInvoiceDictionaryAsync(string start, string end)
        => _repo.GetInvoiceDictionaryAsync(start, end);

    public async Task<List<string>> GetPCBToolsAsync()
    {
        var tools = await _repo.GetPCBToolsAsync();

        if (!tools.Contains("-"))
            tools.Insert(0, "-");

        return tools;
    }
}

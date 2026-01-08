using SeemsAPIService.Domain.Entities;

public interface IJobRepository
{
    Task<Job?> GetJobByNumberAsync(string jobNumber);
    Task<string?> GetJobStatusAsync(string jobNumber);
    Task<List<BillingPlannerRpt>> GetBillingPlannerAsync(string start, string end, string? costcenter);
    Task<List<Invoicedictionary>> GetInvoiceDictionaryAsync(string start, string end);
    Task<List<string>> GetPCBToolsAsync();
}

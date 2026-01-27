using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using SeemsAPIService.Application.Interfaces;
using SeemsAPIService.Domain.Entities;
using SeemsAPIService.Infrastructure.Persistence;

public class JobRepository : IJobRepository
{
    private readonly AppDbContext _context;

    public JobRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Job?> GetJobByNumberAsync(string jobNumber)
    {
        // return  _context.job.FirstOrDefaultAsync(j => j.JobNumber == jobNumber);
        return await _context.job.AsNoTracking().FirstOrDefaultAsync(j => j.JobNumber == jobNumber); // use of AsNoTrcking() - declaring as readonly and helps improve performance
    }

    public async Task<string?> GetJobStatusAsync(string jobNumber)
    {
        return await _context.job
            .Where(j => j.JobNumber == jobNumber)
            .Select(j => j.Status)
            .FirstOrDefaultAsync();
    }

    public async Task<List<BillingPlannerRpt>> GetBillingPlannerAsync(string start, string end, string? costcenter)
    {
        //avoid SQL injection in SP calls
        //string sql = $"CALL sp_BillingPlanner('{start}','{end}','{costcenter}')";
        //return await _context.BillingPlannerRpt
        //    .FromSqlRaw(sql)
        //    .ToListAsync();

        string sql = "CALL sp_BillingPlanner(@start,@end,@costcenter)";
        return await _context.BillingPlannerRpt
            .FromSqlRaw(sql,
                new MySqlParameter("@start", start),
                new MySqlParameter("@end", end),
                new MySqlParameter("@costcenter", costcenter))
            .ToListAsync();

    }

    public async Task<List<Invoicedictionary>> GetInvoiceDictionaryAsync(
        string start, string end)
    {
        //string sql = $"CALL sp_InvoiceDictionary('{start}','{end}')";
        //return await _context.Invoicedictionary
        //    .FromSqlRaw(sql)
        //    .ToListAsync();
        string sql = "CALL sp_InvoiceDictionary(@start,@end)";
        return await _context.Invoicedictionary
            .FromSqlRaw(sql,
             new MySqlParameter("@start", start),
                new MySqlParameter("@end", end))
            .ToListAsync();
    }

    public async Task<List<string>> GetPCBToolsAsync()
    {
        return await _context.tool
            .Where(t => t.Pcbtool != null)
            .Select(t => t.Pcbtool!)
            .Distinct()
            .ToListAsync();
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeemsAPIService.Domain.Entities;
using SeemsAPIService.Infrastructure.Persistence;

namespace SeemsAPIService.API.Controllers
{
    public class JobController : ControllerBase
    {

        private readonly AppDbContext _context;

        public JobController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet("JobDataByNumber/{pJobnumber}")]
        public IActionResult GetJobDataByNumber(string pJobnumber)
        {
            var job = _context.job.FirstOrDefault(j => j.JobNumber == pJobnumber);
            if (job == null)
                return NotFound($"No job found with jobnumber {pJobnumber}");
            return Ok(job);
        }

        [HttpGet("JobStatus/{pJobnumber}")]
        public IActionResult GetJobStatus(string pJobnumber)
        {
            var jobstatus = _context.job.Where(j => j.JobNumber == pJobnumber).Select(j => j.Status).FirstOrDefault();
            return Ok(jobstatus);
        }

        [HttpGet("BillingPlanner")]
        public async Task<List<BillingPlannerRpt>> BillingPlanner(string startdate, string enddate, string? costcenter)

        {
            {
                List<BillingPlannerRpt> list;
                string sql = $"CALL sp_BillingPlanner ('{startdate}','{enddate}','{costcenter}')";
                list = await _context.BillingPlannerRpt.FromSqlRaw<BillingPlannerRpt>(sql).ToListAsync();
                return list;
            }
        }
        //[HttpGet("BillingPlanner/{startdate}/{enddate}/{costcenter?}")]
        //public async Task<List<BillingPlannerRpt>> BillingPlanner(string startdate, string enddate, string? costcenter = null)
        //{
        //    var safeCostcenter = string.IsNullOrEmpty(costcenter) || costcenter == "undefined" ? "" : costcenter;

        //    var sql = "CALL sp_BillingPlanner ({0}, {1}, {2})";
        //    var list = await _context.BillingPlannerRpt
        //        .FromSqlRaw(sql, startdate, enddate, safeCostcenter)
        //        .ToListAsync();

        //    return list;
        //}


        [HttpGet("InvoiceDictionary/{startdate}/{enddate}")]
        public async Task<List<Invoicedictionary>> InvoiceDictionary(string startdate, string enddate)

        {
            {
                List<Invoicedictionary> list;
                string sql = $"CALL sp_InvoiceDictionary('{startdate}','{enddate}')";
                list = await _context.Invoicedictionary.FromSqlRaw<Invoicedictionary>(sql).ToListAsync();
                return list;
            }
        }

    }
}

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

        [HttpGet("PCBTools")]
        public async Task<IActionResult> PCBTools()
        {
            try
            {
                var PCBTools = await _context.tool.Where(t => t.Pcbtool != null).Select(t =>  t.Pcbtool).ToListAsync();

                if (PCBTools == null || !PCBTools.Any())
                    return NotFound("No PCBTools found.");

                return Ok(PCBTools);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while fetching PCBTools.", error = ex.Message });
            }
        }

    }
}

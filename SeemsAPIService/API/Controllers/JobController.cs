using Microsoft.AspNetCore.Mvc;
using SeemsAPIService.Application.Interfaces;

namespace SeemsAPIService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobController : ControllerBase
    {
        private readonly IJobService _jobService;

        public JobController(IJobService jobService)
        {
            _jobService = jobService;
        }

        [HttpGet("JobDataByNumber/{jobNumber}")]
        public async Task<IActionResult> GetJobDataByNumber(string jobNumber)
        {
            var job = await _jobService.GetJobByNumberAsync(jobNumber);

            if (job == null)
                return NotFound($"No job found with jobnumber {jobNumber}");

            return Ok(job);
        }

        [HttpGet("JobStatus/{jobNumber}")]
        public async Task<IActionResult> GetJobStatus(string jobNumber)
        {
            var status = await _jobService.GetJobStatusAsync(jobNumber);
            return Ok(status);
        }

        [HttpGet("BillingPlanner")]
        public async Task<IActionResult> BillingPlanner(
            string startdate,
            string enddate,
            string? costcenter)
        {
            var result = await _jobService
                .GetBillingPlannerAsync(startdate, enddate, costcenter);

            return Ok(result);
        }

        [HttpGet("InvoiceDictionary/{startdate}/{enddate}")]
        public async Task<IActionResult> InvoiceDictionary(
            string startdate,
            string enddate)
        {
            var result = await _jobService
                .GetInvoiceDictionaryAsync(startdate, enddate);

            return Ok(result);
        }

        [HttpGet("PCBTools")]
        public async Task<IActionResult> PCBTools()
        {
            var tools = await _jobService.GetPCBToolsAsync();

            if (!tools.Any())
                return NotFound("No PCBTools found.");

            return Ok(tools);
        }
    }
}

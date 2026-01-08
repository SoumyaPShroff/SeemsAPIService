using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeemsAPIService.Application.DTOs;
using SeemsAPIService.Application.Interfaces;
using SeemsAPIService.Application.Services;
using SeemsAPIService.Domain.Entities;
using SeemsAPIService.Infrastructure.Persistence;
using System.Text;

namespace SeemsAPIService.API.Controllers
{
    public class HomeController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IReusableService _reusableService;
        private readonly IEmailService _emailService;
        private readonly IUserQueryService _userQueryService;
        private readonly IUserAccessService _accessService;
        private readonly ICommonQueryService _commonService;

        public HomeController(IAuthService authService,IReusableService reusableService,IEmailService emailService, IUserQueryService userQueryService, IUserAccessService accessService, ICommonQueryService commonService)
        {
            _authService = authService;
            _reusableService = reusableService;
            _emailService = emailService;
            _userQueryService = userQueryService;
            _accessService = accessService;
            _commonService = commonService;
        }

        [HttpPost("VerifyLoginUser")]
        public async Task<IActionResult> VerifyLoginUser([FromBody] LoginRequestDto request)
        {
            bool isValid = await _authService.VerifyLoginAsync(request);

            if (!isValid)
                return Unauthorized(new { message = "Invalid username or password." });

            return Ok(new { loginId = request.LoginID });
        }

        [HttpPost("ResetPassword/{loginId}/{newPassword}")]
        public async Task<IActionResult> ResetPassword(string loginId, string newPassword)
        {
            await _authService.ResetPasswordAsync(loginId, newPassword);
            return Ok(new { message = "Password updated successfully." });
        }

        [HttpGet("ForgotPassword/{loginId}")]
        public async Task<IActionResult> ForgotPassword(string loginId)
        {
            var email = await _authService.GetEmailByLoginIdAsync(loginId);

            if (string.IsNullOrEmpty(email))
                return NotFound("Email not found.");

            string subject = "Forgot Password - Sienna ECAD System";
            string body = $@"
                <p>Dear {loginId},</p>
                <p>You requested password recovery.</p>
                <p>Please contact your administrator.</p>";

            await _emailService.SendEmailAsync(email, subject, body);

            return Ok(new { message = $"Email sent to {email}" });
        }

        // ---------------- COMMON ----------------

        [HttpGet("UserName/{loginId}")]
        public async Task<IActionResult> UserName(string loginId)
        {
            var name = await _reusableService.GetUserNameAsync(loginId);
            return Ok(name ?? "");
        }

        [HttpGet("StageTools")]
        public async Task<IActionResult> StageTools(long? toolId)
        {
            var tools = await _reusableService.GetStageToolsAsync(toolId);

            if (!tools.Any())
                return NotFound("No stage tools found.");

            return Ok(tools);
        }

        [HttpGet("HOPCTasks")]
        public async Task<IActionResult> HOPCTasks(long? taskId)
        {
            var tasks = await _reusableService.GetHOPCTasksAsync(taskId);

            if (!tasks.Any())
                return NotFound("No hopc tasks found.");

            return Ok(tasks);
        }
        // ---------------- HOPCManagerList ----------------
        [HttpGet("HOPCManagerList")]
        public async Task<IActionResult> HOPCManagerList()
        {
            var list = await _userQueryService.GetHOPCManagerListAsync();

            if (list == null || !list.Any())
                return NotFound("No HOPC managers found.");

            return Ok(list);
        }

        // ---------------- ManagerCostcenterInfo ----------------
        [HttpGet("ManagerCostcenterInfo/{loginId}")]
        public async Task<IActionResult> ManagerCostcenterInfo(string loginId)
        {
            var result = await _userQueryService.GetManagerCostcenterInfoAsync(loginId);

            if (result == null || !result.Any())
                return NotFound($"No manager found with HOPC1ID '{loginId}'");

            return Ok(result);
        }

        // ---------------- GetEmailIDs ----------------
        [HttpGet("EmailId/{loginIds}")]
        public async Task<IActionResult> GetEmailIDs(string loginIds)
        {
            var result = await _userQueryService.GetEmailIDsAsync(loginIds);

            if (result is List<string> list && list.Count == 0)
                return NotFound($"No email IDs found for LoginID(s): {loginIds}");

            return Ok(result);
        }
        // -------- UserRoleInternalRights --------
        [HttpGet("UserRoleInternalRights/{role}/{pageName}")]
        public async Task<IActionResult> UserRoleInternalRights(string role, string pageName)
        {
            bool hasAccess = await _accessService.HasRoleAccessAsync(role, pageName);
            return Ok(hasAccess);
        }

        // -------- UserDesignation --------
        [HttpGet("UserDesignation/{loginId}")]
        public async Task<IActionResult> UserDesignation(string loginId)
        {
            var designation = await _accessService.GetUserDesignationAsync(loginId);

            if (designation == null)
                return NotFound($"No designation found with loginid '{loginId}'");

            return Ok(designation);
        }

        // -------- AllActiveEmployees --------
        [HttpGet("AllActiveEmployees")]
        public async Task<IActionResult> AllActiveEmployees()
        {
            var emps = await _accessService.GetAllActiveEmployeesAsync();

            if (!emps.Any())
                return NotFound("No active employees found.");

            return Ok(emps);
        }

        // -------- AnalysisManagers --------
        [HttpGet("AnalysisManagers")]
        public async Task<IActionResult> AnalysisManagers()
        {
            var list = await _accessService.GetAnalysisManagersAsync();

            if (!list.Any())
                return NotFound("No analysis managers found.");

            return Ok(list);
        }

        // -------- DesignManagers --------
        [HttpGet("DesignManagers")]
        public async Task<IActionResult> DesignManagers()
        {
            var list = await _accessService.GetDesignManagersAsync();

            if (!list.Any())
                return NotFound("No design managers found.");

            return Ok(list);
        }
        [HttpGet("SalesManagers")]
        public async Task<IActionResult> SalesManagers()
        {
            var result = await _commonService.GetSalesManagersAsync();
            if (!result.Any()) return NotFound("No sales managers found.");
            return Ok(result);
        }

        [HttpGet("salesnpiusers")]
        public async Task<IActionResult> SalesNpiUsers()
        {
            var result = await _commonService.GetSalesNpiUsersAsync();
            if (!result.Any()) return NotFound("No sales NPI users found.");
            return Ok(result);
        }

        [HttpGet("SalesEnq_Email_Recipients")]
        public async Task<IActionResult> SalesEnqEmailRecipients()
        {
            var result = await _commonService.GetSalesEnqRecipientsAsync();
            return Ok(result);
        }

        [HttpGet("SideBarAccessMenus/{designationId}")]
        public async Task<IActionResult> SideBarAccessMenus(int designationId)
        {
            var result = await _commonService.GetSideBarAccessMenusAsync(designationId);
            return Ok(result);
        }

        [HttpGet("RoleDesignID/{designationName}")]
        public async Task<IActionResult> RoleDesignID(string designationName)
        {
            var id = await _commonService.GetRoleDesignIdAsync(designationName);
            return Ok(id);
        }
    }
}
 
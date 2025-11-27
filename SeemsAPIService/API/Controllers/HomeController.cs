
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using SeemsAPIService.Application.Services;
using SeemsAPIService.Domain.Entities;
using SeemsAPIService.Infrastructure.Persistence;
using System.Linq;
using System.Text;

namespace SeemsAPIService.API.Controllers
{
    public class HomeController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly EmailTriggerService _emailService;
        public HomeController(AppDbContext context, EmailTriggerService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpGet("VerifyLoginUser/{ploginid}/{ppassword}")]
        public async Task<ActionResult> VerifyLoginUser(string ploginid, string ppassword)

        {
            try
            {
                // Convert plain text password to Base64
                string encodedPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(ppassword));
                //  string encodedPassword = AesEncryptionHelper.EncryptToBase64(ppassword);

                var VerifyLoginUser = await _context.Login
                                             .Where(l => l.LoginID == ploginid && l.Password == encodedPassword)
                                             .FirstOrDefaultAsync();
                if (VerifyLoginUser == null)
                {
                    // Return a 401 Unauthorized status code with a meaningful message
                    return Unauthorized(new { message = "Invalid username or password." });
                }

                // Return success response with the user's login ID
                return Ok(new { loginId = VerifyLoginUser.LoginID });
            }
            catch (Exception ex)
            {
                // Return a 500 Internal Server Error with the exception details
                // Avoid exposing sensitive details in production
                return StatusCode(500, new { message = "An error occurred.", details = ex.Message });
            }
        }

        [HttpPost("ResetPassword/{ploginid}/{pNewpassword}")]
        public async Task<IActionResult> ResetPassword(string ploginid, string pNewpassword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ploginid) || string.IsNullOrWhiteSpace(pNewpassword))
                    return BadRequest(new { message = "Login ID and new password are required." });

                // 🔍 Step 1: Find user by login ID
                var user = await _context.Login
                    .Where(l => l.LoginID == ploginid)
                    .FirstOrDefaultAsync();

                if (user == null)
                    return NotFound(new { message = "User not found." });

                // ✅ Encode the password in Base64
                string encodedPassword = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(pNewpassword));

                user.Password = encodedPassword;
                int affected = await _context.SaveChangesAsync();

                return Ok(new { message = "Password updated successfully." });
            }
            catch (Exception ex)
            {
                // Log the error (don’t expose in production)
                return StatusCode(500, new { message = "An error occurred while resetting the password.", details = ex.Message });
            }
        }

        [HttpGet("ForgotPassword/{ploginid}")]
        public async Task<IActionResult> ForgotPassword(string ploginid)
        {
            if (string.IsNullOrEmpty(ploginid))
                return BadRequest(new { message = "Login ID is required." });

            // 1️⃣ Fetch the Email ID from SQL (Login Table)
            var user = await _context.Login
                .Where(l => l.LoginID == ploginid)
                .Select(l => new { l.LoginID, l.EmailID })
                .FirstOrDefaultAsync();

            if (user == null || string.IsNullOrEmpty(user.EmailID))
                return NotFound(new { message = "Email not found for the given Login ID." });

            // 2️⃣ Build email content
            string toEmail = user.EmailID;
            string subject = "Forgot Password - Sienna ECAD System";
            string body = $@"
                <p>Dear {user.LoginID},</p>
                <p>You have requested to retrieve your password or reset it.</p>
                <p>Please contact your administrator or use your login credentials if remembered.</p>
                <p>– Sienna ECAD Team</p>";

            try
            {
                // 3️⃣ Call your independent Email Service
                await _emailService.SendEmailAsync(user.EmailID, subject, body);
                return Ok(new { message = $"Email successfully sent to {toEmail}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Failed to send email: {ex.Message}" });
            }
        }


        //[HttpGet("UserName/{pLoginId}")]
        //public IActionResult getUserName(string pLoginId)

        //{
        //    try
        //    {
        //        var userName = _context.general_employee.Where(l => l.IDno == pLoginId).Select(l => l.Name).FirstOrDefault();

        //        if (userName == null)
        //            return NotFound($"No user found with LoginID '{pLoginId}'");

        //        return Ok(userName);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        [HttpGet("EmailId/{loginIds}")]
        public IActionResult GetEmailIDs(string loginIds)
        {
            try
            {
                var ids = loginIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                  .Select(id => id.Trim())
                                  .ToList();

                var emailList = (
                    from login in _context.Login
                    join emp in _context.general_employee
                        on login.LoginID equals emp.IDno
                    where ids.Contains(login.LoginID)
                        && emp.EmpStatus == "Active"
                    select login.EmailID
                ).Distinct().ToList();

                if (emailList == null || emailList.Count == 0)
                    return NotFound($"No email IDs found for LoginID(s): {loginIds}");

                return emailList.Count == 1
                    ? Ok(emailList.First())  // return string for single
                    : Ok(emailList);         // return list for multiple
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("ManagerCostcenterInfo/{pLoginId}")]
        public IActionResult ManagerCostcenterInfo(string pLoginId)
        {
            try
            {
                var managerInfo = _context.setting_employee
                    .Where(l => l.HOPC1ID == pLoginId)
                    .Select(l => new
                    {
                        hopc1id = l.HOPC1ID,
                        hopc1name = l.HOPC1NAME,
                        costcenter = l.costcenter
                    })
                    .ToList();

                if (managerInfo == null || managerInfo.Count == 0)
                    return NotFound($"No manager found with HOPC1ID '{pLoginId}'");

                return Ok(managerInfo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("HOPCManagerList")]
        public async Task<List<HOPCManagerList>> HOPCManagerList()

        {
            {
                List<HOPCManagerList> list;
                string sql = $"CALL sp_HOPCManagers()";
                list = await _context.HOPCManagerList.FromSqlRaw<HOPCManagerList>(sql).ToListAsync();
                return list;

            }
        }

        [HttpGet("UserRoleInternalRights/{pRole}/{pPageName}")]
        public async Task<ActionResult<bool>> UserRoleInternalRights(string pRole, string pPageName)
        {
            try
            {
                // Get the record for that role
                var roleRecord = await _context.employeeroles
                    .FirstOrDefaultAsync(e => e.Roles == pRole.Trim());

                if (roleRecord == null)
                    //return NotFound("Role not found");
                    return false;

                // Use reflection to dynamically check the column value
                var property = roleRecord.GetType().GetProperty(pPageName,
                    System.Reflection.BindingFlags.IgnoreCase |
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.Instance);

                if (property == null)
                    // return BadRequest("Invalid page name");
                    return false;

                var value = property.GetValue(roleRecord);
                //if value is null then returns 0
                bool hasAccess = Convert.ToInt32(value ?? 0) == 1;

                return Ok(hasAccess);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("UserDesignation/{pLoginId}")]
        public IActionResult UserDesignation(string pLoginId)

        {
            try
            {
                var userjobtitle = _context.general_employee.Where(g => g.IDno == pLoginId).Select(l => l.JobTitle).FirstOrDefault();

                if (userjobtitle == null)
                    return NotFound($"No designation found with loginid '{pLoginId}'");

                return Ok(userjobtitle);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("AllActiveEmployees")]
        public async Task<IActionResult> AllActiveEmployees()
        {
            try
            {
                var activeEmps = await _context.general_employee.Where(e => e.EmpStatus == "Active").Select(e => new { e.IDno, e.Name, e.EmailId }).ToListAsync();

                if (activeEmps == null || !activeEmps.Any())
                    return NotFound("No active employees found.");

                return Ok(activeEmps);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while fetching active employees.", error = ex.Message });
            }
        }

        [HttpGet("AnalysisManagers")]
        public async Task<IActionResult> AnalysisManagers()
        {
            try
            {
                   var analyMngrs = await (
                     from s in _context.setting_employee
                     join l in _context.Login
                         on s.HOPC1ID equals l.LoginID
                     where s.costcenter_analysis == "YES"
                        && s.costcenter_status == "Active"
                     select new
                     {
                         s.HOPC1ID,
                         s.HOPC1NAME,
                         l.EmailID
                     }
                 ).ToListAsync();
                if (analyMngrs == null || !analyMngrs.Any())
                    return NotFound("No analyMngrs found.");

                return Ok(analyMngrs);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while fetching analyMngrs.", error = ex.Message });
            }
        }
        [HttpGet("DesignManagers")]
        public async Task<IActionResult> DesignManagers()
        {
            try
            {
                var designMngrs = await (
                    from s in _context.setting_employee
                    join l in _context.Login
                        on s.HOPC1ID equals l.LoginID
                    where s.design == "YES"
                       && s.costcenter_status == "Active"
                    select new
                    {
                        s.HOPC1ID,
                        s.HOPC1NAME,
                        l.EmailID
                    }
                ).ToListAsync();
                if (designMngrs == null || !designMngrs.Any())
                    return NotFound("No designMngrs found.");

                return Ok(designMngrs);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while fetching designMngrs.", error = ex.Message });
            }
        }
        public class SalesManagerDto
        {
            public string ID { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;

            public string EmailID { get; set; } = string.Empty;
        }
        [HttpGet("SalesManagers")]
        public async Task<IActionResult> SalesManagers()
        {
            try
            {
                var settingList = await (
                    from s in _context.setting_employee
                    join l in _context.Login
                        on s.HOPC1ID equals l.LoginID
                    where s.costcenter_sales == "YES"
                       && s.costcenter_status == "Active"
                    select new SalesManagerDto
                    {
                        ID = s.HOPC1ID,
                        Name = s.HOPC1NAME,
                        EmailID = l.EmailID
                    }
                ).ToListAsync();
                var generalList = await (
                    from g in _context.general_employee
                    join l in _context.Login
                        on g.IDno equals l.LoginID
                    where g.Functional == "Selling"
                       && g.JobTitle.Contains("sales")
                       && g.EmpStatus == "Active"
                    select new SalesManagerDto
                    {
                        ID = g.IDno,
                        Name = g.Name,
                        EmailID = l.EmailID
                    }
                ).ToListAsync();

                // Combine and de-duplicate by ID (keep the first occurrence)
                var combined = settingList
                    .Concat(generalList)
                    .GroupBy(x => x.ID)
                    .Select(g => g.First())
                    .OrderBy(x => x.Name)   // optional: order by name
                    .ToList();

                if (combined == null || !combined.Any())
                    return NotFound("No sales managers found.");

                return Ok(combined);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while fetching sales managers.", error = ex.Message });
            }
        }
        [HttpGet("salesnpiusers")]
        public async Task<IActionResult> salesnpiusers()

        {
            try
            {
                var salesnpiMngrs = await (
                      from s in _context.view_salesnpiusers
                      join l in _context.Login
                          on s.IDno equals l.LoginID
                      select new
                      {
                          s.IDno,
                          s.Name,
                          l.EmailID
                      }
                  ).ToListAsync();
                if (salesnpiMngrs == null || !salesnpiMngrs.Any())
                    return NotFound("No salesnpiMngrs found.");

                return Ok(salesnpiMngrs);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while fetching salesnpiMngrs.", error = ex.Message });
            }
        }

        [HttpGet("SalesEnq_Email_Recipients")]
        public async Task<List<object>> SalesEnq_Email_Recipients()
        {
            var defaultMails =
                from a in _context.Email_Recipients
                join b in _context.Login on a.LoginId equals b.LoginID into g
                from b in g.DefaultIfEmpty()
                where a.Design == "1"
                select new
                {
                    a.LoginId,
                    a.EnqCreated_PositionInEmail,
                    EmailID = b != null ? b.EmailID : null
                };

            // Project anonymous type to object
            var result = await defaultMails
                .Select(x => (object)x)
                .ToListAsync();

            return result;
        }
    }
}

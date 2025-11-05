
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeemsAPIService.Application.Services;
using SeemsAPIService.Domain.Entities;
using SeemsAPIService.Infrastructure.Persistence;
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

        //  [HttpPost("resetpassword/{ploginid}/{pNewpassword}")]
        [HttpPost("ResetPassword/{ploginid}/{pNewpassword}")]
        public async Task<IActionResult> ResetPassword(string ploginid, string pNewpassword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ploginid) || string.IsNullOrWhiteSpace(pNewpassword))
                    return BadRequest(new { message = "Login ID and new password are required." });

                //  var user = await _context.Login.FirstOrDefaultAsync(l => l.LoginID == ploginid);
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


        [HttpGet("UserName/{pLoginId}")]
        public IActionResult getUserName(string pLoginId)

        {
            try
            {
                var userName = _context.general_employee.Where(l => l.IDno == pLoginId).Select(l => l.Name).FirstOrDefault();

                if (userName == null)
                    return NotFound($"No user found with LoginID '{pLoginId}'");

                return Ok(userName);
            }
            catch (Exception ex)
            {
                throw ex;
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

    }
}

using System.Threading.Tasks;

namespace SeemsAPIService.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(
            string toEmail,
            string subject,
            string body,
            string? ccEmail = null
        );
    }
}

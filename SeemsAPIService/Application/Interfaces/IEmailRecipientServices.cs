using System.Collections.Generic;
namespace SeemsAPIService.Application.Interfaces
{
    public interface IEmailRecipientService
    {
        Task<(List<string> To, List<string> Cc)> GetRecipientsAsync(string type);
    }
}

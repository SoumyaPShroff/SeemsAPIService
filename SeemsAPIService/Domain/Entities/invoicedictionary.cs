
using Microsoft.EntityFrameworkCore;
#nullable disable
namespace SeemsAPIService.Domain.Entities
{
    [Keyless]
    public class Invoicedictionary
    {
        public string jobnumber { get; set; }
        public int month { get; set; }
        public int year { get; set; }

    }
}

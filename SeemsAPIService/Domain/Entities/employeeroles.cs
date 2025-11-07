using Microsoft.EntityFrameworkCore;
#nullable disable
namespace SeemsAPIService.Domain.Entities
{
    [Keyless]
    public class employeeroles
    {
        public Int32 Id { get; set; }
        public string Roles { get; set; }
        public Int32  billingplanner { get; set; }
        public Int32 viewallenquiries { get; set; }
        public Int32 salesmgmtdashboard { get; set; }

    }
}

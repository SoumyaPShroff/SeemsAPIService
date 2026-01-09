using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
#nullable disable
namespace SeemsAPIService.Domain.Entities
{
    [Keyless]
    public class HOPCManagerList
    {
        public string hopc1id { get; set; }
        public string hopc1name { get; set; }
        public string costcenter { get; set; }

        public string EmailID { get; set; }
    }
}
    
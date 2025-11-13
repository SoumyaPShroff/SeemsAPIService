using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
#nullable disable
namespace SeemsAPIService.Domain.Entities
{
    public class se_customer_locations
    {
        [Key]
        public Int64 location_id { get; set; }
        public Int64 customer_id { get; set; }
        public string location { get; set; }
        public string address { get; set; }
        public string phoneno1 { get; set; }
        public string phoneno2 { get; set; }

    }
}

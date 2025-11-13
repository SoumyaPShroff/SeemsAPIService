using System.ComponentModel.DataAnnotations;
#nullable disable
namespace SeemsAPIService.Domain.Entities
{
    public class se_customer_contacts
    {
        [Key]
        public Int64 contact_id { get; set; }
        public Int64 location_id { get; set; }
        public Int64 customer_id { get; set; }
        public string ContactTitle { get; set; }
        public string ContactName { get; set; }
        public string email11 { get; set; }
        public string mobile1 { get; set; }
        public string mobile2 { get; set; }
    }
}

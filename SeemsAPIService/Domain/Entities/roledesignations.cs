using System.ComponentModel.DataAnnotations;
#nullable disable
namespace SeemsAPIService.Domain.Entities
{
    public class roledesignations
    {
        [Key]
        public long designationid { get; set; }
        public string designation { get; set; }
    }
}

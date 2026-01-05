using System.ComponentModel.DataAnnotations;
#nullable disable
namespace SeemsAPIService.Domain.Entities
{
    public class se_quotation
    {
        [Key]
        public string enquiryno { get; set; }
        public string board_ref { get; set; }
        public string quoteNo { get; set; }
        public string createdBy { get; set; }
        public string versionNo { get; set; }
        public string tandc { get; set; }
      
    }
}

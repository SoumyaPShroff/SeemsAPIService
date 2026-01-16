using System.ComponentModel.DataAnnotations;
#nullable disable
namespace SeemsAPIService.Domain.Entities
{
    public class se_quotation
    {
        [Key]
        public string quoteNo { get; set; }
        public string enquiryno { get; set; }
        public string board_ref { get; set; }
        public string createdBy { get; set; }
        public int versionNo { get; set; }
        public string tandc { get; set; }

        // Navigation
        public ICollection<se_quotation_items> Items { get; set; }

    }
}

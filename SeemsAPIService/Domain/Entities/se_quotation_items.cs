using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeemsAPIService.Domain.Entities
{
  
    public class se_quotation_items
    {
        [Key]   // ✅ REQUIRED
       // [DatabaseGenerated(DatabaseGeneratedOption.Identity)] - auto increment
        public long slNo { get; set; }
        public string quoteNo { get; set; } //FK

        [ForeignKey(nameof(quoteNo))]
        public se_quotation Quotation { get; set; } // navigation
        public string layout { get; set; }
        public string quantity { get; set; }
        public string unit_rate { get; set; }
        public string currency_id { get; set; }
        public DateTime created_on { get; set; }
        public string updatedbyid { get; set; }
        public string versionNo { get; set; }
        public string durationtype { get; set; }

        public long location_id { get; set; }
    }
}

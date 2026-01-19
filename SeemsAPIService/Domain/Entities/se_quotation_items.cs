using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeemsAPIService.Domain.Entities
{
    public class se_quotation_items
    {
        [Key]   // ✅ REQUIRED
       // [DatabaseGenerated(DatabaseGeneratedOption.Identity)] - auto increment
        public int slNo { get; set; }
        public string quoteNo { get; set; }  // 🔴 FK (NOT NULL)

        [ForeignKey(nameof(quoteNo))]
        public se_quotation Quotation { get; set; } // navigation
        public string layout { get; set; }
        public int quantity { get; set; }
        public decimal unit_rate { get; set; }
        public int currency_id { get; set; }
        public DateTime created_on { get; set; }
        public string updatedbyid { get; set; }
        public int versionNo { get; set; }
        public string durationtype { get; set; }

        public int location_id { get; set; }
    }
}

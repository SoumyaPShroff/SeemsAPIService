using System.ComponentModel.DataAnnotations;

namespace SeemsAPIService.Domain.Entities
{
    public class se_quotation_items
    {
        [Key]
        public long slNo { get; set; }
        public string quoteNo { get; set; }
        public string layout { get; set; }
        public string quantity { get; set; }
        public string unit_rate { get; set; }
        public string currency_id { get; set; }
        public string created_on { get; set; }
        public string updatedbyid { get; set; }
        public string versionNo { get; set; }

        public string durationtype { get; set; }

    }
}

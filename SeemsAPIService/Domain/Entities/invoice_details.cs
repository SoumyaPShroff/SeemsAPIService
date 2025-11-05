using Microsoft.EntityFrameworkCore;
#nullable disable
namespace SeemsAPIService.Domain.Entities
{
    [Keyless]
    public class invoice_details
    {
        public uint JobID { get; set; }
        public string JobNumber { get; set; }
        public string po_amount { get; set; }
        public string invoicedate { get; set; }
        public string layoutname { get; set; }
        public string Boardref { get; set; }
        public string rateperhr { get; set; }
        public string customeraddrs { get; set; }
        public string invoiceno { get; set; }
        public uint invoicecount { get; set; }
        public string quoteNo { get; set; }
        public string Remarks { get; set; }
        public string datemn { get; set; }
        public string invoice_currency { get; set; }
        public string billingdate { get; set; }
        public string PoHrs { get; set; }
        public string iponumber { get; set; }   
    }


}

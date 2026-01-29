using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
#nullable disable
namespace SeemsAPIService.Domain.Entities
{
    [Keyless]
    public class ViewQuoteDetails
    {
        public string  enquiryno { get; set; }
        public string quoteNo { get; set; }
        public string  customer { get; set; }
        public string  createdon { get; set; }
        public string name { get; set; }  //created by sales
        public Decimal  totalquoteAmt { get; set; }
        public int  versionno { get; set; }
    }
}

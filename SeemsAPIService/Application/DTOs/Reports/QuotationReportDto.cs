using System.Collections.Generic;

namespace SeemsAPIService.Application.DTOs.Reports
{
    public class QuotationReportDto
    {
        public string QuoteNo { get; set; }
        public string EnquiryNo { get; set; }
        public string Customer { get; set; }
        public string BoardRef { get; set; }
        public int VersionNo { get; set; }
        public string CreatedBy { get; set; }

        public List<RptQuotationLineDto> Items { get; set; } = new();

        public decimal GrandTotal { get; set; }
        public string TermsAndConditions { get; set; }
    }
}

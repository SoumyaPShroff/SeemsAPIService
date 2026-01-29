namespace SeemsAPIService.Application.DTOs.Reports
{
    public class QuotationReportDto
    {
        public RptQuotationHeaderDto Header { get; set; }
        public List<RptQuotationLineDto> Items { get; set; } = new();
        public decimal SubTotal { get; set; }
        public decimal GrandTotal { get; set; }
        public string TermsAndConditions { get; set; }
    }
}

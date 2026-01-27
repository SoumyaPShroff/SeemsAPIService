namespace SeemsAPIService.Application.DTOs.Reports
{
    public class RptQuotationLineDto
    {
        public int SlNo { get; set; }
        public string Layout { get; set; }
        public int Quantity { get; set; }
        public decimal UnitRate { get; set; }
        public string CurrencySymbol { get; set; }
        public string DurationType { get; set; }

        public decimal LineTotal => Quantity * UnitRate;
    }
}

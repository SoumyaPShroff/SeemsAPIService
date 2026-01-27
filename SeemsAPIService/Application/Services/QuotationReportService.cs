using SeemsAPIService.Application.Services;
using SeemsAPIService.Application.DTOs.Reports;
using SeemsAPIService.Application.Interfaces;

public class QuotationReportService 
    //: ISalesRepository
{
    private readonly ISalesRepository _repo;

    public QuotationReportService(ISalesRepository repo)
    {
        _repo = repo;
    }

    //public QuotationReportDto GetQuotationReport(string quoteNo)
    //{
    //    // Existing DTO (write-side structure)
    //    SalesService quotation = _repo.GetQuotationDetailsAsync(quoteNo);

    //    var reportItems = quotation.Items.Select((item, index) =>
    //        new RptQuotationLineDto
    //        {
    //            SerialNo = index + 1,
    //            Layout = item.layout,
    //            Quantity = item.quantity,
    //            UnitRate = item.unit_rate,
    //            DurationType = item.durationtype,
    //            CurrencySymbol = GetCurrencySymbol(item.currency_id)
    //        }).ToList();

    //    return new QuotationReportDto
    //    {
    //        Header = new RptQuotationHeaderDto
    //        {
    //            QuoteNo = quotation.quoteNo,
    //            EnquiryNo = quotation.enquiryno,
    //            CreatedBy = quotation.createdBy,
    //            VersionNo = quotation.versionNo,
    //            BoardRef = quotation.board_ref
    //        },
    //        Items = reportItems,
    //        TermsAndConditions = quotation.tandc,
    //        Sales = _repo.GetSalesInfo(quotation.createdBy)
    //    };
    //}

    //private string GetCurrencySymbol(int currencyId)
    //{
    //    return currencyId switch
    //    {
    //        1 => "₹",
    //        2 => "$",
    //        3 => "€",
    //        _ => ""
    //    };
    //}
}

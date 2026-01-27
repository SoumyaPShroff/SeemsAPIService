using QuestPDF.Infrastructure;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using SeemsAPIService.Application.DTOs.Reports;

namespace SeemsAPIService.Infrastructure.Documents
{
    public class QuotationPDFDocument : IDocument
 
    {
        private readonly QuotationReportDto _data;

        public QuotationPDFDocument(QuotationReportDto data)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));
        }
        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(20);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Content().Column(col =>
                {
                    col.Item().Element(Header);
                    col.Item().PaddingVertical(10).Element(ItemsTable);
                    col.Item().PaddingTop(10).Element(Terms);
                    col.Item().PaddingTop(20).Element(Signature);
                });
            });
        }

        private void Header(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().Text("QUOTATION").Bold().FontSize(14);
                col.Item().Text($"Quote No: {_data.QuoteNo}");
                col.Item().Text($"Enquiry No: {_data.EnquiryNo}");
                col.Item().Text($"Version: {_data.VersionNo}");
            });
        }

        private void ItemsTable(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(30);
                    columns.RelativeColumn();
                    columns.ConstantColumn(50);
                    columns.ConstantColumn(60);
                    columns.ConstantColumn(70);
                });

                table.Header(header =>
                {
                    header.Cell().Text("Sl");
                    header.Cell().Text("Description");
                    header.Cell().Text("Qty");
                    header.Cell().Text("Rate");
                    header.Cell().Text("Total");
                });

                foreach (var item in _data.Items)
                {
                    table.Cell().Text(item.SlNo.ToString());
                    table.Cell().Text(item.Layout);
                    table.Cell().Text(item.Quantity.ToString());
                    table.Cell().Text(item.UnitRate.ToString("N2"));
                    table.Cell().Text(item.LineTotal.ToString("N2"));
                }
            });
        }

        private void Terms(IContainer container)
        {
            container.Text(text =>
            {
                text.Span("Terms & Conditions\n").Bold();
                text.Span(_data.TermsAndConditions);
            });
        }

        private void Signature(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().Text("For SEEMS TECHNOLOGIES").Bold();
                col.Item().Text(_data.CreatedBy);
            });
        }
    }
}
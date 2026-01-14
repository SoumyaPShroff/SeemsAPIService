using SeemsAPIService.Application.DTOs;
using SeemsAPIService.Application.Mapper;
using SeemsAPIService.Domain.Entities;

public class QuotationMapper : IEntityMapper<QuotationDto, se_quotation, string?>
{
    public se_quotation MapForAdd(QuotationDto dto, string? extra)
    {
        return new se_quotation
        {
            quoteNo = dto.quoteNo,
            enquiryno = dto.enquiryno,
            board_ref = dto.board_ref,
            createdBy = dto.createdBy,
            versionNo = dto.versionNo,
            tandc = dto.tandc,

            Items = dto.Items.Select(i => new se_quotation_items
            {
                slNo = i.slNo,
                quoteNo = dto.quoteNo,
                layout = i.layout,
                quantity = i.quantity,
                unit_rate = i.unit_rate,
                currency_id = i.currency_id,
                durationtype = i.durationtype,
                updatedbyid = i.updatedbyid,
                versionNo = dto.versionNo,
                created_on = DateTime.UtcNow,
                location_id = i.location_id,

            }).ToList()
        };
    }
 
    // =========================
    // EDIT
    // =========================
    public void MapForEdit(QuotationDto dto, se_quotation existing, string? extra)
    {
        // --- header ---
        existing.board_ref = dto.board_ref;
        existing.createdBy = dto.createdBy;
        existing.versionNo = dto.versionNo;
        existing.tandc = dto.tandc;

        // --- items ---
        // simple strategy: remove & re-add
        existing.Items.Clear();

        foreach (var i in dto.Items)
        {
            existing.Items.Add(new se_quotation_items
            {
                slNo = i.slNo,          // keep PK if editing existing lines
                quoteNo = existing.quoteNo,
                layout = i.layout,
                quantity = i.quantity,
                unit_rate = i.unit_rate,
                currency_id = i.currency_id,
                durationtype = i.durationtype
            });
        }
    }
}

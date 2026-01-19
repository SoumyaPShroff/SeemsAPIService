using SeemsAPIService.Application.DTOs;
using SeemsAPIService.Application.Mapper.SeemsAPIService.Application.Mapper;
using SeemsAPIService.Domain.Entities;

public class QuotationMapper : IEntityMapper<QuotationDto, se_quotation, string?>
{
    // =========================
    // ADD
    // =========================
    //public se_quotation MapForAdd(QuotationDto dto, string? extra)
    //{
    //    return new se_quotation
    //    {
    //        quoteNo = dto.quoteNo,         //required to map even though generated at service level
    //        enquiryno = dto.enquiryno,
    //        board_ref = dto.board_ref,
    //        createdBy = dto.createdBy,
    //        versionNo = dto.versionNo,
    //        tandc = dto.tandc,
    //        createdOn = DateTime.UtcNow,

    //        Items = dto.Items.Select(i => new se_quotation_items
    //        {
    //            slNo = 0, // ✅ DB generates
    //            quoteNo = dto.quoteNo,  // ✅ REQUIRED FK
    //            layout = i.layout,
    //            quantity = i.quantity,
    //            unit_rate = i.unit_rate,
    //            currency_id = i.currency_id,
    //            durationtype = i.durationtype,
    //            updatedbyid = i.updatedbyid,
    //            versionNo = dto.versionNo,
    //            created_on = DateTime.UtcNow,
    //            location_id = i.location_id
    //        }).ToList()
    //    };
    //}
    public se_quotation MapForAdd(QuotationDto dto, string? extra) //manually generate sino
    {
        int nextSlNo = 1; // start slNo from 1 for this new quotation

        return new se_quotation
        {
            quoteNo = dto.quoteNo,         // required to map even though generated at service level
            enquiryno = dto.enquiryno,
            board_ref = dto.board_ref,
            createdBy = dto.createdBy,
            versionNo = dto.versionNo,
            tandc = dto.tandc,
            createdOn = DateTime.UtcNow,
 
            Items = dto.Items.Select(i => new se_quotation_items
            {
                slNo = nextSlNo++,            // ✅ manually assign unique slNo
                quoteNo = dto.quoteNo,        // ✅ FK
                layout = i.layout,
                quantity = i.quantity,
                unit_rate = i.unit_rate,
                currency_id = i.currency_id,
                durationtype = i.durationtype,
                updatedbyid = i.updatedbyid,
                versionNo = dto.versionNo,
                created_on = DateTime.UtcNow,
                location_id = i.location_id
            }).ToList()
        };
    }

    // =========================
    // EDIT
    // =========================
    //public void MapForEdit(QuotationDto dto, se_quotation existing, string? extra)
    //{
    //    existing.board_ref = dto.board_ref;
    //    existing.tandc = dto.tandc;

    //    // DELETE
    //    if (dto.deletedSlNos?.Any() == true)
    //    {
    //        var toDelete = existing.Items
    //            .Where(i => dto.deletedSlNos.Contains(i.slNo))
    //            .ToList();

    //        foreach (var item in toDelete)
    //            existing.Items.Remove(item);
    //    }

    //    // 🔑 Determine next slNo ONCE
    //    int nextSlNo = existing.Items.Any()
    //        ? existing.Items.Max(i => i.slNo) + 1
    //        : 1;

    //    foreach (var itemDto in dto.Items)
    //    {
    //        // UPDATE
    //        if (itemDto.slNo > 0)
    //        {
    //            var existingItem = existing.Items
    //                .FirstOrDefault(i => i.slNo == itemDto.slNo);

    //            if (existingItem != null)
    //            {
    //                existingItem.layout = itemDto.layout;
    //                existingItem.quantity = itemDto.quantity;
    //                existingItem.unit_rate = itemDto.unit_rate;
    //                existingItem.currency_id = itemDto.currency_id;
    //                existingItem.durationtype = itemDto.durationtype;
    //                existingItem.updatedbyid = itemDto.updatedbyid;
    //                existingItem.location_id = itemDto.location_id;

    //                continue; // 🔴 THIS WAS MISSING
    //            }
    //        }

    //        // INSERT new line
    //        existing.Items.Add(new se_quotation_items
    //        {
    //            slNo = nextSlNo++,          // ✅ always unique
    //            quoteNo = existing.quoteNo,
    //            layout = itemDto.layout,
    //            quantity = itemDto.quantity,
    //            unit_rate = itemDto.unit_rate,
    //            currency_id = itemDto.currency_id,
    //            durationtype = itemDto.durationtype,
    //            updatedbyid = itemDto.updatedbyid,
    //            versionNo = existing.versionNo,
    //            created_on = DateTime.UtcNow,
    //            location_id = itemDto.location_id
    //        });
    //    }
    //}
    public void MapForEdit(QuotationDto dto, se_quotation existing, string? extra)
    {
        existing.board_ref = dto.board_ref;
        existing.tandc = dto.tandc;

        // -------------------------
        // DELETE
        // -------------------------
        if (dto.deletedSlNos?.Any() == true)
        {
            var toDelete = existing.Items
                .Where(i => dto.deletedSlNos.Contains(i.slNo))
                .ToList();

            foreach (var item in toDelete)
                existing.Items.Remove(item);
        }

        // -------------------------
        // FIND NEXT SLNO
        // -------------------------
        int nextSlNo = existing.Items.Any()
            ? existing.Items.Max(i => i.slNo) + 1
            : 1;

        // -------------------------
        // UPDATE / ADD
        // -------------------------
        foreach (var itemDto in dto.Items)
        {
            // 🔵 UPDATE EXISTING
            if (itemDto.slNo > 0)
            {
                var existingItem = existing.Items
                    .FirstOrDefault(i => i.slNo == itemDto.slNo);

                if (existingItem == null)
                    continue;

                existingItem.layout = itemDto.layout;
                existingItem.quantity = itemDto.quantity;
                existingItem.unit_rate = itemDto.unit_rate;
                existingItem.currency_id = itemDto.currency_id;
                existingItem.durationtype = itemDto.durationtype;
                existingItem.updatedbyid = itemDto.updatedbyid;
                existingItem.location_id = itemDto.location_id;

                continue; // 🔑 VERY IMPORTANT
            }

            // 🟢 ADD NEW
            existing.Items.Add(new se_quotation_items
            {
                slNo = nextSlNo++,
                quoteNo = existing.quoteNo,
                layout = itemDto.layout,
                quantity = itemDto.quantity,
                unit_rate = itemDto.unit_rate,
                currency_id = itemDto.currency_id,
                durationtype = itemDto.durationtype,
                updatedbyid = itemDto.updatedbyid,
                versionNo = existing.versionNo,
                created_on = DateTime.UtcNow,
                location_id = itemDto.location_id
            });
        }
    }

}

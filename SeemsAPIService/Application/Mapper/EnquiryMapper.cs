using SeemsAPIService.Application.DTOs;
using SeemsAPIService.Application.Mapper.SeemsAPIService.Application.Mapper;
using SeemsAPIService.Domain.Entities;

namespace SeemsAPIService.Application.Mapper
{

    public class EnquiryMapper : IEntityMapper<EnquiryDto, se_enquiry, string?>
    {
        public se_enquiry MapForAdd(EnquiryDto dto, string? savedFilePath)
        {
            return new se_enquiry
            {
                enquiryno = dto.enquiryno,   // already generated in service
                customer_id = dto.customer_id,
                contact_id = dto.contact_id,
                type = dto.type,
                salesresponsibilityid = dto.salesresponsibilityid,
                completeresponsibilityid = dto.completeresponsibilityid,
                createdBy = dto.createdBy,
                createdOn = DateTime.Now,
                uploadedfilename = savedFilePath ?? "",
                status = "Open",
                statename = dto.statename ?? "-",
                currency_id = dto.currency_id,
                inputreceivedthru = dto.inputreceivedthru,
                quotation_request_lastdate = dto.quotation_request_lastdate,
                location_id = dto.location_id,

                // layout
                design = dto.design,
                library = dto.library,
                qacam = dto.qacam,
                dfm = dto.dfm ?? "",
                layout_fab = dto.layout_fab,
                layout_testing = dto.layout_testing,
                layout_others = dto.layout_others ?? "",
                layoutbyid = dto.layoutbyid ?? "",
                dfa = dto.dfa,

                // analysis
                si = dto.si,
                pi = dto.pi,
                emi_net_level = dto.emi_net_level,
                emi_system_level = dto.emi_system_level,
                thermal_board_level = dto.thermal_board_level,
                thermal_system_level = dto.thermal_system_level,
                analysis_others = dto.analysis_others ?? "",
                analysisbyid = dto.analysisbyid ?? "",

                // va / npi
                npi_fab = dto.npi_fab,
                asmb = dto.asmb,
                npi_testing = dto.npi_testing,
                npi_others = dto.npi_others ?? "",
                hardware = dto.hardware,
                software = dto.software,
                fpg = dto.fpg,
                VA_Assembly = dto.VA_Assembly,
                DesignOutSource = dto.DesignOutSource,
                npibyid = dto.npibyid ?? "",

                NPINew_BOMProc = dto.NPINew_BOMProc,
                NPINew_Fab = dto.NPINew_Fab,
                NPINew_Assbly = dto.NPINew_Assbly,
                NPINew_Testing = dto.NPINew_Testing,
                NPINewbyid = dto.NPINewbyid ?? "",
                npinew_jobwork = dto.npinew_jobwork,

                Remarks = dto.Remarks ?? "",
                enquirytype = dto.enquirytype ?? "",
                tool = dto.tool ?? "",
                govt_tender = dto.govt_tender,
                jobnames = dto.jobnames ?? "",
                appendreq = dto.appendreq ?? "",
                ReferenceBy = dto.ReferenceBy ?? "",
                tm = dto.tm,
                vaMech = dto.vaMech,

                // onsite
                toolLicense = dto.toolLicense,
                toolId = dto.toolId,
                taskId = dto.taskId,
                expFrom = dto.expFrom,
                expTo = dto.expTo,
                noOfResources = dto.noOfResources,
                tentStartDate = dto.tentStartDate,
                logistics = dto.logistics,
                onsiteDurationType = dto.onsiteDurationType,
                hourlyRateType = dto.hourlyRateType,
                hourlyReate = dto.hourlyReate,
                profReqLastDate = dto.profReqLastDate,
                onsiteDuration = dto.onsiteDuration,
            };
        }
        public void MapForEdit(EnquiryDto dto, se_enquiry existing, string? savedFilePath)
        {
            existing.customer_id = dto.customer_id;
            existing.contact_id = dto.contact_id;
            existing.type = dto.type;
            existing.statename = dto.statename ?? "-";
            existing.currency_id = dto.currency_id;
            existing.inputreceivedthru = dto.inputreceivedthru;
            existing.salesresponsibilityid = dto.salesresponsibilityid;
            existing.completeresponsibilityid = dto.completeresponsibilityid;
            existing.quotation_request_lastdate = dto.quotation_request_lastdate;
            existing.location_id = dto.location_id;

            // layout
            existing.design = dto.design;
            existing.library = dto.library;
            existing.qacam = dto.qacam;
            existing.dfm = dto.dfm ?? "";
            existing.layout_fab = dto.layout_fab;
            existing.layout_testing = dto.layout_testing;
            existing.layout_others = dto.layout_others ?? "";
            existing.layoutbyid = dto.layoutbyid ?? "";
            existing.dfa = dto.dfa;

            // analysis
            existing.si = dto.si;
            existing.pi = dto.pi;
            existing.emi_net_level = dto.emi_net_level;
            existing.emi_system_level = dto.emi_system_level;
            existing.thermal_board_level = dto.thermal_board_level;
            existing.thermal_system_level = dto.thermal_system_level;
            existing.analysis_others = dto.analysis_others ?? "";
            existing.analysisbyid = dto.analysisbyid ?? "";

            // va / npi
            existing.npi_fab = dto.npi_fab;
            existing.asmb = dto.asmb;
            existing.npi_testing = dto.npi_testing;
            existing.npi_others = dto.npi_others ?? "";
            existing.hardware = dto.hardware;
            existing.software = dto.software;
            existing.fpg = dto.fpg;
            existing.VA_Assembly = dto.VA_Assembly;
            existing.DesignOutSource = dto.DesignOutSource;
            existing.npibyid = dto.npibyid ?? "";

            existing.NPINew_BOMProc = dto.NPINew_BOMProc;
            existing.NPINew_Fab = dto.NPINew_Fab;
            existing.NPINew_Assbly = dto.NPINew_Assbly;
            existing.NPINew_Testing = dto.NPINew_Testing;
            existing.NPINewbyid = dto.NPINewbyid ?? "";
            existing.npinew_jobwork = dto.npinew_jobwork;

            existing.Remarks = dto.Remarks ?? "";
            existing.enquirytype = dto.enquirytype ?? "";
            existing.tool = dto.tool ?? "";
            existing.govt_tender = dto.govt_tender;
            existing.jobnames = dto.jobnames ?? "";
            existing.appendreq = dto.appendreq ?? "";
            existing.ReferenceBy = dto.ReferenceBy ?? "";
            existing.tm = dto.tm;
            existing.vaMech = dto.vaMech;

            // onsite
            existing.toolLicense = dto.toolLicense;
            existing.toolId = dto.toolId;
            existing.taskId = dto.taskId;
            existing.expFrom = dto.expFrom;
            existing.expTo = dto.expTo;
            existing.noOfResources = dto.noOfResources;
            existing.tentStartDate = dto.tentStartDate;
            existing.logistics = dto.logistics;
            existing.onsiteDurationType = dto.onsiteDurationType;
            existing.hourlyRateType = dto.hourlyRateType;
            existing.hourlyReate = dto.hourlyReate;
            existing.profReqLastDate = dto.profReqLastDate;
            existing.onsiteDuration = dto.onsiteDuration;

            // file (only if new uploaded)
            if (!string.IsNullOrEmpty(savedFilePath))
                existing.uploadedfilename = savedFilePath;

            existing.createdBy = dto.createdBy;
            existing.createdOn = DateTime.Now;
        }
    }
}
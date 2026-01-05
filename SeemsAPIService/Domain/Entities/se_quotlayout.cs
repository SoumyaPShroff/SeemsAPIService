
using System;
using System.ComponentModel.DataAnnotations;

namespace SeemsAPIService.Domain.Entities
{
    public class se_quotlayout
    {
        [Key]
        public long idNo { get; set; }
        public string layout { get; set; }
        public string Taxname { get; set; }
        public double? tax_INR { get; set; }
        public double? tax_USD { get; set; }
        public double?  tax_EURO { get; set; }

    }
    }

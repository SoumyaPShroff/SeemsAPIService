using System.ComponentModel.DataAnnotations;

namespace SeemsAPIService.Domain.Entities
#nullable disable
{
    public class se_stages_tools
    {
        [Key]
        public Int64 Idno { get; set; }
        public string Tools { get; set; }

        public Int64 condition_tool { get; set; }
    }
}

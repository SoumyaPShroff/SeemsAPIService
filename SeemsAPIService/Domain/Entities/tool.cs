using System.ComponentModel.DataAnnotations;
#nullable disable
namespace SeemsAPIService.Domain.Entities
{
    public class tool
    {
        [Key] public Int64 toolid { get; set; }
        public string toolname { get; set; }
        public string Pcbtool { get; set; }
        public string Schtool { get; set; }
        public string sitool { get; set; }
        public string thermaltool { get; set; }
        public string mechanicaltool { get; set; }
        public string dfxtool { get; set; }
        public string camtool { get; set; }
        public string tools { get; set; }
        public string pitool { get; set; }
        public string EMItool { get; set; }
        public string Othertool { get; set; }
        public string PackageDesign { get; set; }
        public Boolean status { get; set; }
    }
}

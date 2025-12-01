using System.ComponentModel.DataAnnotations;

namespace SeemsAPIService.Domain.Entities
#nullable disable
{
    public class SidebarAccessMenus
    {
        [Key]
        public long accessid { get; set; }
        public string mainmenu { get; set; }
        public string submenu { get; set; }
        public string pagename { get; set; } 
        public string route { get; set; }


    }
}

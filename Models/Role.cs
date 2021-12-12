using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace prodotnet.Models
{
    public class Role
    {
        public int RoleID {get;set;}

        [Required]
        [Display(Name ="Tên quyền")]
        public string RoleName {get;set;}

        public ICollection<User> Users { get; set; }
    }
}
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace prodotnet.Models
{
    public class User
    {
        [Key]
        public int IdUser {get;set;}

        [Required(ErrorMessage ="Phải nhập tên đăng nhập")]
        [StringLength(20,MinimumLength = 3)]
        [Display(Name ="Tên đăng nhập")]
        public string Username {get;set;}

        [Required(ErrorMessage ="Phải nhập email")]
        [Display(Name ="Email")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}",ErrorMessage ="Phả nhập email đúng chuẩn!!")]
        public string Email { get; set; }

        [Required(ErrorMessage ="Phải nhập mật khẩu")]
        [Display(Name="Mật khẩu")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "{0} dài {2} đến {1}")]
        public string Password { get; set; }

        [NotMapped]
        [Display(Name ="Xác nhận mật khẩu")]
        [Required(ErrorMessage ="Phải nhập xác nhận mật khẩu")]
        [System.ComponentModel.DataAnnotations.Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Tên quyền")]
        public Nullable<int>  RoleID { get; set; }
        [ForeignKey("RoleID")]
        [Display(Name = "Tên quyền")]
        public Role Role {get;set;}
    }
}
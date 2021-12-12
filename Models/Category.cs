using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace prodotnet.Models
{
    public class Category{

        [Key]
        public int Id {get;set;}

        [Required(ErrorMessage ="Phải có tên danh mục")]
        [Display(Name ="Tên danh mục")]
        [StringLength(100,MinimumLength =3,ErrorMessage ="{0} dài {1} đến {2}")]
        public string Title {get;set;}

        [Display(Name ="Mô tả danh mục")]
        [DataType(DataType.Text)]
        public string Content {get;set;}

        [Required(ErrorMessage = "Phải tạo url")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "{0} dài {1} đến {2}")]
        [RegularExpression(@"^[a-z0-9-]*$", ErrorMessage = "Chỉ dùng các ký tự [a-z0-9-]")]
        [Display(Name = "Url hiện thị")]
        public string Slug { set; get; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace prodotnet.Models
{
    public class CreatePostModel : Post{
        
        [Display(Name ="Chuyên mục")]
        public int[] CategoryIDs{get;set;}
    }
}
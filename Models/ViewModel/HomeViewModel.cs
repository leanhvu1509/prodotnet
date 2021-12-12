using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace prodotnet.Models.ViewModel
{
    public class HomeViewModel
    {
        public List<Post> ListPosts {get;set;}

        public List<Category> ListCategories {get;set;}
    
    }
}
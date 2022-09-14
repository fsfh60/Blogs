using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace BlogRUs.ViewModels
{
    public class BlogViewModel
    {
        //data data annontations to do the validation 
        [Required, MaxLength(100), DisplayName("Subject")]
        public string Title { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }
    }
}

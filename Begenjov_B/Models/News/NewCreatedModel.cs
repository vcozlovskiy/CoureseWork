using System.ComponentModel.DataAnnotations;

namespace Begenjov_B.Models.News
{
    public class NewCreatedModel
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Text { get; set; }
    }
}

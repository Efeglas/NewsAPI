using System.ComponentModel.DataAnnotations;

namespace NewsService.DTOs
{
    public class CreateNewsDTO
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public string ImageUrl { get; set; }   
    }
}

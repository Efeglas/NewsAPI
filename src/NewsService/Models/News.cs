namespace NewsService.Models
{
    public class News
    {
        public News(string title, string description, string imageUrl, string author)
        {
            Id = Guid.NewGuid();
            Title = title;
            ImageUrl = imageUrl;
            CreatedAt = DateTime.Now.ToUniversalTime();
            UpdatedAt = DateTime.Now.ToUniversalTime();
            Description = description;
            Author = author;
        }
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
    }
}

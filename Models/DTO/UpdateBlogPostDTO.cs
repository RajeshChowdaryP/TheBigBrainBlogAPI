namespace TheBigBrainBlog.API.Models.DTO
{
    public class UpdateBlogPostDTO
    {
        public required string Title { get; set; }
        public string ShortDescription { get; set; }
        public string Content { get; set; }
        public string FeaturedImgUrl { get; set; }
        public string UrlHandle { get; set; }
        public DateTime PublishedDate { get; set; }
        public string Author { get; set; }
        public bool IsVisible { get; set; } = true;

        public List<Guid> Categories { get; set; } = new List<Guid>();
    }
}

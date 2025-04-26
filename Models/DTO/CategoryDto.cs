namespace TheBigBrainBlog.API.Models.DTO
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string UrlHandle { get; set; }

    }
}

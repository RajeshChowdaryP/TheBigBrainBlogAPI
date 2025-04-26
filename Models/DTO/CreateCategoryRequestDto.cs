namespace TheBigBrainBlog.API.Models.DTO
{
    public class CreateCategoryRequestDto
    {
        public required string Name { get; set; }
        public string UrlHandle { get; set; }
    }
}

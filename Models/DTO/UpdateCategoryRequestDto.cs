namespace TheBigBrainBlog.API.Models.DTO
{
    public class UpdateCategoryRequestDto
    {
        public required string Name { get; set; }
        public string UrlHandle { get; set; }
    }
}

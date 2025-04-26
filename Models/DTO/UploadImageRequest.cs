namespace TheBigBrainBlog.API.Models.DTO
{
    public class UploadImageRequest
    {
        // Swashbuckle has trouble understanding how to document IFormFile directly as a parameter. Wrapping it in a model marked with [FromForm] gives it the necessary context.
        public IFormFile file { get; set; }
    }
}

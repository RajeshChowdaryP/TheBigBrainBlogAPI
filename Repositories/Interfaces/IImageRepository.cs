using TheBigBrainBlog.API.Models.Domain;

namespace TheBigBrainBlog.API.Repositories.Interfaces
{
    public interface IImageRepository
    {
        Task<IEnumerable<BlogImage?>> GetAllImages();
        Task<BlogImage?> UploadImage(IFormFile file, BlogImage blogImage);
    }
}

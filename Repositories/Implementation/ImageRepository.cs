using Microsoft.EntityFrameworkCore;
using TheBigBrainBlog.API.Data;
using TheBigBrainBlog.API.Models.Domain;
using TheBigBrainBlog.API.Repositories.Interfaces;

namespace TheBigBrainBlog.API.Repositories.Implementation
{
    public class ImageRepository : IImageRepository
    {
        private readonly IWebHostEnvironment _environment; // IWebHostEnvironment is used to get the path of the root folder of the project
        private readonly IHttpContextAccessor httpContextAccessor; // IHttpContextAccessor is used to access the current HTTP context
        private readonly ApplicationDbContext _dbContext; // ApplicationDbContext is used to interact with the database

        public ImageRepository(IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor, ApplicationDbContext dbContext) 
        { 
            _environment = environment;
            this.httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }
        public async Task<BlogImage?> UploadImage(IFormFile file, BlogImage blogImage)
        {
            // 1. Generate a unique file name  
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff"); // Millisecond precision  
            var randomSuffix = Guid.NewGuid().ToString().Substring(0, 8); // Short, random  
            var uniqueFileName = $"{blogImage.FileName}_{timestamp}_{randomSuffix}{blogImage.FileExtension}";

            // 2. Upload the image to API/Images folder  
            var localImagePath = Path.Combine(_environment.ContentRootPath, "Images", uniqueFileName);

            using var stream = new FileStream(localImagePath, FileMode.Create);
            await file.CopyToAsync(stream);

            // 3. Save the image information to the database  
            var httpRequest = httpContextAccessor.HttpContext.Request;
            var url = $"{httpRequest.Scheme}://{httpRequest.Host}/Images/{uniqueFileName}"; // Generate URL for the image  

            blogImage.Url = url;

            await _dbContext.BlogImages.AddAsync(blogImage); // Add the image information to the database  
            await _dbContext.SaveChangesAsync(); // Save the changes to the database  

            // 4. Return the image information  
            return blogImage;
        }

        public async Task<IEnumerable<BlogImage?>> GetAllImages()
        {
            //var images = await _dbContext.BlogImages.ToListAsync();

            //// Ensure URLs are correctly formatted when retrieving  
            //var httpRequest = httpContextAccessor.HttpContext.Request;
            //foreach (var image in images)
            //{
            //    if (image != null && !image.Url.StartsWith("http"))
            //    {
            //        image.Url = $"{httpRequest.Scheme}://{httpRequest.Host}/Images/{Path.GetFileName(image.Url)}";
            //    }
            //}

            //return images;

            return await _dbContext.BlogImages.ToListAsync(); // Get all images from the database

        }
    }
}

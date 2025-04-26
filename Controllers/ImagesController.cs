using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TheBigBrainBlog.API.Models.Domain;
using TheBigBrainBlog.API.Models.DTO;
using TheBigBrainBlog.API.Repositories.Interfaces;

namespace TheBigBrainBlog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageRepository _imageRepository;
        public ImagesController(IImageRepository imageRepository)
        {
            _imageRepository = imageRepository;
        }

        // This method handles the HTTP POST request to upload an image file.  
        // It accepts three parameters from the form:  
        // - file: The uploaded image file. This is of type IFormFile, which represents the file sent with the HTTP request.  
        // - fileName: The desired name for the file. This is a string provided by the user to name the uploaded file.  
        // - title: A title or description for the image. This is a string that can be used to describe the image.  
        [HttpPost("UploadImage")]
        public async Task<IActionResult> UploadImage([FromForm] UploadImageRequest request, [FromForm] string fileName, [FromForm] string title)
        {
            if (request.file == null)
            {
                return BadRequest();
            }
            ValidateFileUpload(request.file);
            if (ModelState.IsValid)
            {
                BlogImage? imageData = new BlogImage
                {
                    FileExtension = Path.GetExtension(request.file.FileName).ToLower(),
                    FileName = fileName,
                    Title = title,
                    CreatedDate = DateTime.Now,
                };

                imageData = await _imageRepository.UploadImage(request.file, imageData);

                if (imageData != null)
                {
                    return Ok(new BlogImageDTO
                    {
                        Id = imageData.Id,
                        FileName = imageData.FileName,
                        FileExtension = imageData.FileExtension,
                        Title = imageData.Title,
                        Url = imageData.Url,
                        CreatedDate = imageData.CreatedDate
                    });
                }
                return BadRequest("Image upload failed.");
            }

            return BadRequest(ModelState); // Return the model state errors if validation fails.
        }

        private void ValidateFileUpload(IFormFile file)
        {
            var allowedFileTypes = new[] { ".jpeg", ".png", ".jpg" };
            var fileExtension = Path.GetExtension(file.FileName);

            if (!allowedFileTypes.Contains(fileExtension.ToLower()))
            {
                /*ModelState is a property of the ControllerBase class in ASP.NET Core.
                 represents the state of model binding and validation for the current request.
                ModelState is used to store validation errors and provides a way to check if the data submitted by the client is valid.
                You can use ModelState to add custom validation errors (e.g., using ModelState.AddModelError)  
                 or to check if the model is valid(e.g., using ModelState.IsValid).  
                 It is commonly used in scenarios where you need to validate user input or file uploads
                 and provide meaningful error messages back to the client.
                 Adds a model state error for the "File" key with a specific error message.  
                 This is used to indicate that the uploaded file has an invalid type,
                 and the error message will be included in the response to the client.*/
                ModelState.AddModelError("File", "Invalid file type. Only JPEG, PNG, and JPG are allowed.");
            }

            //Validate file size(e.g., max 5 MB)
            const long maxFileSize = 5 * 1024 * 1024; // 5 MB  
            if (file.Length > maxFileSize)
            {
                ModelState.AddModelError("File", "File size exceeds the maximum allowed size of 5 MB.");
            }
        }

        [HttpGet("GetAllImages")]
        public async Task<IActionResult> GetAllImages()
        {
            var images = await _imageRepository.GetAllImages();
            if (images == null) // Fix: Check if images is null before dereferencing  
            {
                return NotFound();
            }

            var imagesList = new List<BlogImageDTO>();
            foreach (var image in images)
            {
                imagesList.Add(new BlogImageDTO
                {
                    Id = image.Id,
                    FileName = image.FileName,
                    FileExtension = image.FileExtension,
                    Title = image.Title,
                    Url = image.Url,
                    CreatedDate = image.CreatedDate
                });
            }
            return Ok(imagesList);
        }
    }
}

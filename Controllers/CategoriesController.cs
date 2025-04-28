using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TheBigBrainBlog.API.Data;
using TheBigBrainBlog.API.Models.Domain;
using TheBigBrainBlog.API.Models.DTO;
using TheBigBrainBlog.API.Repositories.Interfaces;

namespace TheBigBrainBlog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoriesController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpPost("CreateCategory")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequestDto request)
        {
            try
            {// map Dto to Domain Model
                var category = new Category
                {
                    Name = request.Name,
                    UrlHandle = request.UrlHandle
                };

                // get the implementation through repository layer
                await _categoryRepository.CreateCategory(category);

                // model Domain to Dto
                var response = new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    UrlHandle = request.UrlHandle
                };

                // return the added value
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("GetAllCategories")]
        //[Authorize(Roles = "Writer,Reader")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryRepository.GetCategoriesAsync();

            // map domain model to Dto
            var response = new List<CategoryDto>();
            foreach(var category in categories)
            {
                response.Add(new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    UrlHandle = category.UrlHandle

                });
            }
            return Ok(response);
        }

        [HttpGet("GetCategoryById={categoryId}")]
        public async Task<IActionResult> GetCategoryById([FromRoute] Guid categoryId)
        {
            var category = await _categoryRepository.GetCategoryById(categoryId);

            if(category == null)
            {
                return NotFound();
            }

            var response = new CategoryDto()
            {
                Id = category.Id,
                Name = category.Name,
                UrlHandle = category.UrlHandle
            };
            return Ok(response);
        }

        [HttpPut("UpdateCategory/{id}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> UpdateCategory([FromRoute] Guid id, [FromBody] UpdateCategoryRequestDto category) 
        {
            Category updateRequiredItemData = new Category
            {
                Id = id,
                Name = category.Name,
                UrlHandle = category.UrlHandle
            };

            var result = await _categoryRepository.UpdateCategory(updateRequiredItemData);
            if(result == null)
            {
                return NotFound();
            }

            var response = new CategoryDto
            {
                Id = result.Id,
                Name = result.Name,
                UrlHandle = result.UrlHandle
            };

            return Ok(response);

        }

        [HttpDelete("DeleteCategory/{id}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> DeleteCategory([FromRoute] Guid id)
        {
            var response = await _categoryRepository.DeleteCategory(id);
            if(response == null)
            {
                return NotFound();
            }
            var deletedcategory = new CategoryDto
            {
                Id= response.Id,
                Name= response.Name,
                UrlHandle = response.UrlHandle
            };
            return Ok(deletedcategory);
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TheBigBrainBlog.API.Models.Domain;
using TheBigBrainBlog.API.Models.DTO;
using TheBigBrainBlog.API.Repositories.Implementation;
using TheBigBrainBlog.API.Repositories.Interfaces;

namespace TheBigBrainBlog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostsController : ControllerBase
    {
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly ICategoryRepository _categoryRepository;
        public BlogPostsController(IBlogPostRepository blogPostRepository, ICategoryRepository categoryRepository)
        {
            _blogPostRepository = blogPostRepository;
            _categoryRepository = categoryRepository;
        }

        [HttpPost("CreateBlogPost")]
        public async Task<IActionResult> CreateBlogPost([FromBody] CreateBlogPostRequestDTO post)
        {
            try
            {
                BlogPost blogPost = new BlogPost()
                {
                    Title = post.Title,
                    ShortDescription = post.ShortDescription,
                    Content = post.Content,
                    FeaturedImgUrl = post.FeaturedImgUrl,
                    UrlHandle = post.UrlHandle,
                    PublishedDate = post.PublishedDate,
                    Author = post.Author,
                    IsVisible = post.IsVisible,
                    Categories = new List<Category>()
                };

                foreach (var categoryId in post.Categories)
                {
                    var category = await _categoryRepository.GetCategoryById(categoryId);
                    if (category != null)
                    {
                        blogPost.Categories.Add(category);
                    }
                }

                blogPost = await _blogPostRepository.CreateBlogPost(blogPost);

                BlogPostDTO response = new BlogPostDTO()
                {
                    Id = blogPost.Id,
                    Title = blogPost.Title,
                    ShortDescription = blogPost.ShortDescription,
                    Content = blogPost.Content,
                    FeaturedImgUrl = blogPost.FeaturedImgUrl,
                    UrlHandle = blogPost.UrlHandle,
                    PublishedDate = blogPost.PublishedDate,
                    Author = blogPost.Author,
                    IsVisible = blogPost.IsVisible,
                    Categories = blogPost.Categories.Select(c => new CategoryDto()
                    {
                        Id = c.Id,
                        Name = c.Name,
                        UrlHandle = c.UrlHandle
                    }).ToList()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("GetAllBlogPosts")]
        public async Task<IActionResult> GetAllBlogposts()
        {
            try
            {
                var blogPosts = await _blogPostRepository.GetAllBlogPostsAsync();

                var response = new List<BlogPostDTO>();

                foreach (var post in blogPosts)
                {
                    response.Add(new BlogPostDTO()
                    {
                        Id = post.Id,
                        Title = post.Title,
                        ShortDescription = post.ShortDescription,
                        Content = post.Content,
                        FeaturedImgUrl = post.FeaturedImgUrl,
                        UrlHandle = post.UrlHandle,
                        PublishedDate = post.PublishedDate,
                        Author = post.Author,
                        IsVisible = post.IsVisible,
                        Categories = post.Categories.Select(c => new CategoryDto()
                        {
                            Id = c.Id,
                            Name = c.Name,
                            UrlHandle = c.UrlHandle
                        }).ToList()
                    });
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("GetBlogpostById/{id:Guid}")]
        public async Task<IActionResult> GetBlogpostById([FromRoute] Guid id)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(id.ToString()))
                {
                    var blogPost = await _blogPostRepository.GetBlogpostById(id);
                    if (blogPost != null)
                    {
                        BlogPostDTO requestedBlogPost = new BlogPostDTO()
                        {
                            Id = blogPost.Id,
                            Title = blogPost.Title,
                            ShortDescription = blogPost.ShortDescription,
                            Content = blogPost.Content,
                            FeaturedImgUrl = blogPost.FeaturedImgUrl,
                            UrlHandle = blogPost.UrlHandle,
                            PublishedDate = blogPost.PublishedDate,
                            Author = blogPost.Author,
                            IsVisible = blogPost.IsVisible,
                            Categories = blogPost.Categories.Select(c => new CategoryDto()
                            {
                                Id = c.Id,
                                Name = c.Name,
                                UrlHandle = c.UrlHandle
                            }).ToList()
                        };
                        return Ok(requestedBlogPost);
                    }
                    return NotFound("Request Not found with the id: " + id);
                }
                return BadRequest("Invalid Id");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut("UpdateBlogpostById/{id:Guid}")]
        public async Task<IActionResult> UpdateBlogpostById([FromRoute] Guid id, [FromBody] UpdateBlogPostDTO post)
        {
            try
            {
                //DTO to Domain model
                BlogPost blogPost = new BlogPost()
                {
                    Id = id,
                    Title = post.Title,
                    ShortDescription = post.ShortDescription,
                    Content = post.Content,
                    FeaturedImgUrl = post.FeaturedImgUrl,
                    UrlHandle = post.UrlHandle,
                    PublishedDate = post.PublishedDate,
                    Author = post.Author,
                    IsVisible = post.IsVisible,
                    Categories = new List<Category>()
                };

                foreach (var categoryGuid in post.Categories)
                {
                    var existingCategory = await _categoryRepository.GetCategoryById(categoryGuid);
                    if (existingCategory != null)
                    {
                        blogPost.Categories.Add(existingCategory);
                    }
                }

                BlogPost? response = await _blogPostRepository.UpdateBlogPost(blogPost);

                if (response == null)
                {
                    return NotFound();
                }

                // Domain to Dto
                BlogPostDTO requestedBlogPost = new BlogPostDTO()
                {
                    Id = blogPost.Id,
                    Title = blogPost.Title,
                    ShortDescription = blogPost.ShortDescription,
                    Content = blogPost.Content,
                    FeaturedImgUrl = blogPost.FeaturedImgUrl,
                    UrlHandle = blogPost.UrlHandle,
                    PublishedDate = blogPost.PublishedDate,
                    Author = blogPost.Author,
                    IsVisible = blogPost.IsVisible,
                    Categories = blogPost.Categories.Select(c => new CategoryDto()
                    {
                        Id = c.Id,
                        Name = c.Name,
                        UrlHandle = c.UrlHandle
                    }).ToList()
                };
                return Ok(requestedBlogPost);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        [HttpDelete("DeleteBlogPost/{id:Guid}")]
        public async Task<IActionResult> DeleteBlogPost([FromRoute] Guid id)
        {
            BlogPost? response = await _blogPostRepository.DeleteBlogPostAsync(id);
            if(response == null)
            {
                return NotFound();
            }

            BlogPostDTO requestedBlogPost = new BlogPostDTO()
            {
                Id = response.Id,
                Title = response.Title,
                ShortDescription = response.ShortDescription,
                Content = response.Content,
                FeaturedImgUrl = response.FeaturedImgUrl,
                UrlHandle = response.UrlHandle,
                PublishedDate = response.PublishedDate,
                Author = response.Author,
                IsVisible = response.IsVisible,
                Categories = response.Categories.Select(c => new CategoryDto()
                {
                    Id = c.Id,
                    Name = c.Name,
                    UrlHandle = c.UrlHandle
                }).ToList()
            };

            return Ok(requestedBlogPost);
        }
    }
}

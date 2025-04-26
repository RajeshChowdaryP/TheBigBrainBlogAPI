using Microsoft.AspNetCore.Mvc;
using TheBigBrainBlog.API.Models.Domain;

namespace TheBigBrainBlog.API.Repositories.Interfaces
{
    public interface IBlogPostRepository
    {
        Task<BlogPost> CreateBlogPost(BlogPost blogPost);
        Task<IEnumerable<BlogPost>> GetAllBlogPostsAsync();
        Task<BlogPost?> GetBlogpostById(Guid Id);
        Task<BlogPost?> UpdateBlogPost(BlogPost request);
        Task<BlogPost?> DeleteBlogPostAsync(Guid id);
        Task<BlogPost?> GetBlogPostByUrlHandleAsync(string url);
    }
}

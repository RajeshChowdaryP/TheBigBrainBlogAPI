using Microsoft.EntityFrameworkCore;
using System.Collections;
using TheBigBrainBlog.API.Data;
using TheBigBrainBlog.API.Models.Domain;
using TheBigBrainBlog.API.Repositories.Interfaces;

namespace TheBigBrainBlog.API.Repositories.Implementation
{
    public class BlogPostRepository: IBlogPostRepository
    {
        private readonly ApplicationDbContext _context;
        public BlogPostRepository(ApplicationDbContext dbContext) { 
            _context = dbContext;
        }
        public async Task<BlogPost> CreateBlogPost(BlogPost blogPost)
        {
            await _context.BlogPosts.AddAsync(blogPost);
            _context.SaveChanges();

            return blogPost;
        }

        public async Task<IEnumerable<BlogPost>> GetAllBlogPostsAsync()
        {
            return await _context.BlogPosts.Include(c => c.Categories).ToListAsync();
        }

        public async Task<BlogPost?> GetBlogpostById(Guid id)
        {
            return await _context.BlogPosts.Include(c => c.Categories).FirstOrDefaultAsync(post => post.Id == id);
        }

        public async Task<BlogPost?> UpdateBlogPost(BlogPost request)
        {
            BlogPost? postInfo = await _context.BlogPosts.Include(c => c.Categories).FirstOrDefaultAsync(post => post.Id == request.Id);
            if(postInfo != null)
            {
                // Update blogpost
                _context.Entry(postInfo).CurrentValues.SetValues(request);

                // Update categories
                postInfo.Categories = request.Categories;

                _context.SaveChanges();
                return request;
            }
            return null;
        }

        public async Task<BlogPost?> DeleteBlogPostAsync(Guid id)
        {
            BlogPost? post = await _context.BlogPosts.Include(b => b.Categories).FirstOrDefaultAsync(b => b.Id == id);
            if(post == null)
            {
                return null;
            }

            _context.BlogPosts.Remove(post);
            _context.SaveChanges();
            return post;
        }

        public async Task<BlogPost?> GetBlogPostByUrlHandleAsync(string url)
        {
            return await _context.BlogPosts.Include(url => url.Categories).FirstOrDefaultAsync(post => post.UrlHandle == url);
        }
    }
}

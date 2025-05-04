using TheBigBrainBlog.API.Models.Domain;

namespace TheBigBrainBlog.API.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<Category> CreateCategory(Category category);

        Task<IEnumerable<Category>> GetCategoriesAsync(string? query = null);
        Task<Category?> GetCategoryById(Guid Id);

        Task<Category?> UpdateCategory(Category category);
        Task<Category?> DeleteCategory(Guid id);
    }
}

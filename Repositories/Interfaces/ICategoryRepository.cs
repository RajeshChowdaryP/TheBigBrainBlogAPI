using TheBigBrainBlog.API.Models.Domain;

namespace TheBigBrainBlog.API.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<Category> CreateCategory(Category category);

        Task<IEnumerable<Category>> GetCategoriesAsync(string? query = null, string? sortBy = null, string? sortByDirection = null, int? pageNumber = 1, int? pageSize = 100);
        Task<Category?> GetCategoryById(Guid Id);

        Task<Category?> UpdateCategory(Category category);
        Task<Category?> DeleteCategory(Guid id);
    }
}

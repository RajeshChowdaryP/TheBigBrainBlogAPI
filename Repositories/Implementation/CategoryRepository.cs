using Microsoft.EntityFrameworkCore;
using TheBigBrainBlog.API.Data;
using TheBigBrainBlog.API.Models.Domain;
using TheBigBrainBlog.API.Repositories.Interfaces;

namespace TheBigBrainBlog.API.Repositories.Implementation
{
    public class CategoryRepository: ICategoryRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public CategoryRepository(ApplicationDbContext dbContext) 
        {
            _dbContext = dbContext;
        }

        public async Task<Category> CreateCategory(Category category) 
        {

            // add the object to Categories dataSet property
            await _dbContext.Categories.AddAsync(category);

            // tell dbcontext to save the changes
            _dbContext.SaveChanges();

            return category;
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync(string? query = null, string? sortBy = null, string? sortByDirection = null)
        {
            // Query  
            var categories = _dbContext.Categories.AsQueryable<Category>();

            // Filtering  
            if (!string.IsNullOrWhiteSpace(query))
            {
                categories = categories.Where(c => c.Name.Contains(query)); // c.Name.Contains(query, StringComparison.OrdinalIgnoreCase()) ordinalIgnoreCase helps us to ignore the case matching and only compare the text but EF Core handle it bydefault so no need to add StringComparison.OrdinalIgnoreCase() 
            }

            // Sorting  
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                if(string.Equals(sortBy, "Name", StringComparison.OrdinalIgnoreCase)) // Here name is the column name that we given in the category domain model
                {
                    var isAsc = string.Equals(sortByDirection, "asc", StringComparison.OrdinalIgnoreCase) ? true : false;

                    categories = isAsc ? categories.OrderBy(c => c.Name) : categories.OrderByDescending(c => c.Name);
                }

                // We can add multiple sorts for different columns
            }

            // Pagination  

            return await categories.ToListAsync();
        }

        public async Task<Category?> UpdateCategory(Category categoryData)
        {
            Category? item =  await _dbContext.Categories.FirstOrDefaultAsync(el => el.Id == categoryData.Id);
            if(item != null)
            {
                //item.Name = categoryData.Name;
                //item.UrlHandle = categoryData.UrlHandle;
                _dbContext.Entry(item).CurrentValues.SetValues(categoryData);
                _dbContext.SaveChanges();
                return categoryData;
            }

            return null;

        }
        
        public async Task<Category?> GetCategoryById(Guid categoryId)
        {
            return await _dbContext.Categories.FirstOrDefaultAsync(category => category.Id == categoryId);
        }

        public async Task<Category?> DeleteCategory(Guid id)
        {
            var existingCategory = await _dbContext.Categories.FirstOrDefaultAsync(category => category.Id == id);
            if(existingCategory is null)
            {
                return null;
            }
            _dbContext.Categories.Remove(existingCategory);
            _dbContext.SaveChanges();
            return existingCategory;
        }
    }

}

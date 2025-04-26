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

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await _dbContext.Categories.ToListAsync();

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

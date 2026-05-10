using Microsoft.EntityFrameworkCore;
using UrbanBites.Application.Interfaces.Repositories;
using UrbanBites.Domain.Entities;
using UrbanBites.Infrastructure.Data;

namespace UrbanBites.Infrastructure.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context) { }

        public override async Task<IEnumerable<Product>> GetAllAsync()
        {
            Console.WriteLine("----> ProductRepository.GetAllAsync() INVOKED!");
            return await _dbSet
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .ToListAsync();
        }

        public override async Task<Product?> GetByIdAsync(Guid id)
            => await _dbSet
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(Guid categoryId)
            => await _dbSet
                .Where(p => p.CategoryId == categoryId)
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .ToListAsync();

        public async Task<IEnumerable<Product>> GetProductsByBrandAsync(Guid brandId)
            => await _dbSet
                .Where(p => p.BrandId == brandId)
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .ToListAsync();

        public async Task<IEnumerable<Product>> SearchAsync(string query)
            => await _dbSet
        .Include(p => p.Brand)
                .Include(p => p.Category)
                .Where(p => p.Name.Contains(query) ||
                            p.Description.Contains(query))
                .ToListAsync();


    }

}
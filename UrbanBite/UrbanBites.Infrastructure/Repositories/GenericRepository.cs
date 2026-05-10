using Microsoft.EntityFrameworkCore;
using UrbanBites.Application.Interfaces.Repositories;
using UrbanBites.Infrastructure.Data;

namespace UrbanBites.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(Guid id)
            => await _dbSet.FindAsync(id);

        public virtual async Task<IEnumerable<T>> GetAllAsync()
            => await _dbSet.ToListAsync();

        public async Task AddAsync(T entity)
            => await _dbSet.AddAsync(entity);

        public void Update(T entity)
            => _dbSet.Update(entity);

        public void Delete(T entity)
            => _dbSet.Remove(entity);

        public async Task<bool> SaveChangesAsync()
        {
            try
            {
                return await _context.SaveChangesAsync() > 0;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                foreach (var entry in ex.Entries)
                {
                    Console.WriteLine($"CONCURRENCY ERROR: Entity={entry.Entity.GetType().Name}, State={entry.State}");
                    var dbValues = await entry.GetDatabaseValuesAsync();
                    Console.WriteLine($"DB Values null? {dbValues is null}");
                }
                throw;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DB UPDATE ERROR: {ex.InnerException?.Message ?? ex.Message}");
                foreach (var entry in ex.Entries)
                {
                    Console.WriteLine($"Entity={entry.Entity.GetType().Name}, State={entry.State}");
                }
                throw;
            }
        }
    }
}
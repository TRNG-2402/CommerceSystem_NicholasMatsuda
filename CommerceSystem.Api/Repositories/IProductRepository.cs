using CommerceSystem.Api.Models;

namespace CommerceSystem.Api.Repositories;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id);
    Task<List<Product>> GetAllAsync();
    Task AddAsync(Product product);
    Task<bool> SkuExistsAsync(string sku);
    Task SaveChangesAsync();
    Task DeleteByIdAsync(int id);
}
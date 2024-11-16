using WNC.G06.Models.Repository;
using WNC.G06.Models;
using Microsoft.EntityFrameworkCore;

public class ProductRepository
{
    private readonly DataContext _dataContext;

    public ProductRepository(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task AddProductAsync(ProductModel product)
    {
        await _dataContext.Products.AddAsync(product);
        await _dataContext.SaveChangesAsync();
    }
    public IEnumerable<ProductModel> GetCafes()
    {
        return _dataContext.Products.Include(u => u.Cafe).ToList();
    }
}

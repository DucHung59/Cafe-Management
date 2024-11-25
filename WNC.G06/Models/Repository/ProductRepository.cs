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
  
    public IEnumerable<ProductModel> GetAllProducts()
    {
        return _dataContext.Products
               .Include(p => p.Cafe)  
               .ToList();
    }

    public void DeleteProduct(int productId)
    {
        var product = _dataContext.Products.Find(productId);
        if (product != null)
        {
            _dataContext.Products.Remove(product);
            _dataContext.SaveChanges();
        }
    }

}

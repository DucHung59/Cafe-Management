using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WNC.G06.Models;
using WNC.G06.Models.Repository;

public class ManagerController : Controller
{
    private readonly DataContext _dataContext;
    private readonly string _managerPermission = "manager";
    private readonly UserRepository _userRepository;
    private readonly ProductRepository _productRepository;
    private readonly CafeRepository _cafeRepository;

    public ManagerController(DataContext dataContext, UserRepository userRepository, ProductRepository productRepository, CafeRepository cafeRepository)
    {
        _dataContext = dataContext;
        _userRepository = userRepository;
        _productRepository = productRepository;
        _cafeRepository = cafeRepository;
    }

    public IActionResult Index()
    {
        if (!CheckAccess(_managerPermission))
        {
            return RedirectToAction("AccessDenied", "Home");
        }
        return View();
    }

    [HttpGet]
    public IActionResult AddCafe()
    {
        if (!CheckAccess(_managerPermission))
        {
            return RedirectToAction("AccessDenied", "Home");
        }
        var username = User.Identity?.Name ?? "Guest";
        ViewData["UserName"] = username;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddCafe(CafeModel cafe)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("AccessDenied", "Home");
        }

        if (_cafeRepository.IsCafenameExist(cafe.CafeName))
        {
            ModelState.AddModelError("CafeName", "Tên cửa hàng đã tồn tại. Vui lòng chọn tên khác.");

            var username = User.Identity?.Name ?? "Guest";
            ViewData["UserName"] = username;

            return View(); 
        }

        var newCafe = new CafeModel
        {
            CafeName = cafe.CafeName,
            Address = cafe.Address,
            Phone = cafe.Phone,
            Description = cafe.Description,
            Status = cafe.Status,
            UserID = int.Parse(userId)
        };

        await _cafeRepository.AddCafeAsync(newCafe);
        return RedirectToAction("Index");
    }



    [HttpGet]
    public IActionResult AddProduct()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 
        var cafes = _dataContext.Cafes.Where(c => c.UserID.ToString() == userId).ToList();
        ViewBag.Cafes = new SelectList(cafes, "CafeID", "CafeName");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddProduct(ProductModel product)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("AccessDenied", "Home");
        }

        var cafes = _dataContext.Cafes.Where(c => c.UserID.ToString() == userId).ToList();

        if (!cafes.Any())
        {
            ModelState.AddModelError("CafeID", "Bạn không có cửa hàng nào để thêm sản phẩm.");
            ViewBag.Cafes = new SelectList(cafes, "CafeID", "CafeName");
            return View(product);
        }

        bool isProductExistsInSelectedCafe = _dataContext.Products.Any(p => p.ProductName == product.ProductName && p.CafeID == product.CafeID);

        if (isProductExistsInSelectedCafe)
        {
            ModelState.AddModelError("ProductName", "Sản phẩm này đã tồn tại trong cửa hàng đã chọn. Vui lòng chọn tên khác.");

            ViewBag.Cafes = new SelectList(cafes, "CafeID", "CafeName", product.CafeID);
            return View(product);
        }

        var newProduct = new ProductModel
        {
            ProductName = product.ProductName,
            Price = product.Price,
            imgUrl = product.imgUrl,
            Description = product.Description,
            Status = product.Status,
            CafeID = product.CafeID 
        };

        await _productRepository.AddProductAsync(newProduct);

        return RedirectToAction("Index"); 
    }




    private bool CheckAccess(string permission)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return false;
        }

        if (User.FindFirst(ClaimTypes.Role)?.Value != permission)
        {
            return false;
        }

        return true;
    }
    private int GetLoggedInUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out int userId) ? userId : 0;
    }
}

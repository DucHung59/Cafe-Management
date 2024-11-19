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
    private readonly ProductRepository _productRepository;
    private readonly CafeRepository _cafeRepository;

    public ManagerController(DataContext dataContext, ProductRepository productRepository, CafeRepository cafeRepository)
    {
        _dataContext = dataContext;
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
            Status = true,
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

    /* [HttpPost]
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
              Status = true,
              CafeID = product.CafeID 
          };

          await _productRepository.AddProductAsync(newProduct);

          return RedirectToAction("Index"); 
      }*/

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

        // Kiểm tra nếu sản phẩm đã tồn tại trong cửa hàng
        bool isProductExistsInSelectedCafe = _dataContext.Products
            .Any(p => p.ProductName == product.ProductName && p.CafeID == product.CafeID);

        if (isProductExistsInSelectedCafe)
        {
            ModelState.AddModelError("ProductName", "Sản phẩm này đã tồn tại trong cửa hàng đã chọn. Vui lòng chọn tên khác.");
            ViewBag.Cafes = new SelectList(cafes, "CafeID", "CafeName", product.CafeID);
            return View(product);
        }

        // Kiểm tra giá sản phẩm
        if (product.Price < 0)
        {
            ModelState.AddModelError("Price", "Giá sản phẩm không được nhỏ hơn 0.");
            ViewBag.Cafes = new SelectList(cafes, "CafeID", "CafeName", product.CafeID);
            return View(product);
        }

        // Kiểm tra định dạng URL hình ảnh

        if (string.IsNullOrEmpty(product.imgUrl) || !product.imgUrl.Trim().EndsWith(".png", StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError("imgUrl", "URL của hình ảnh phải có đuôi .png và không được để trống.");
            ViewBag.Cafes = new SelectList(cafes, "CafeID", "CafeName", product.CafeID);
            return View(product);
        }
        // Tạo sản phẩm mới
        var newProduct = new ProductModel
        {
            ProductName = product.ProductName,
            Price = product.Price,
            imgUrl = product.imgUrl,
            Description = product.Description,
            Status = true,
            CafeID = product.CafeID
        };

        await _productRepository.AddProductAsync(newProduct);

        return RedirectToAction("Index", "Manager");
    }
    [HttpGet]
    public IActionResult IndexProduct()
    {
        // Lấy UserID của người dùng đăng nhập
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("AccessDenied", "Home");
        }

        // Lấy danh sách các CafeID mà người dùng sở hữu
        var cafeIds = _dataContext.Cafes
            .Where(c => c.UserID.ToString() == userId)
            .Select(c => c.CafeID)
            .ToList();

        // Lấy tất cả sản phẩm của các cửa hàng mà người dùng sở hữu (bao gồm cả sản phẩm có Status = False)
        var products = _dataContext.Products
            .Where(p => cafeIds.Contains(p.CafeID)) // Lọc theo cửa hàng
            .Include(p => p.Cafe)  // Nạp thông tin cửa hàng (Cafe)
            .ToList();

        // Xử lý ảnh sản phẩm
        var imageDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "css", "Images");
        var imagePaths = products.Select(product =>
        {
            var imagePath = Path.Combine(imageDirectory, product.imgUrl);

            return System.IO.File.Exists(imagePath)
                ? $"/css/Images/{product.imgUrl}"
                : "/css/Images/default.png";
        }).ToList();

        ViewBag.ImagePaths = imagePaths;
        return View(products);
    }


    [HttpPost]
    public IActionResult DeleteProduct(int productId)
    {
        var product = _dataContext.Products.FirstOrDefault(p => p.ProductID == productId);
        if (product != null)
        {
            product.Status = false;  // Chỉ thay đổi trạng thái thành False
            _dataContext.SaveChanges();  // Lưu thay đổi vào cơ sở dữ liệu
        }

        return Json(new { success = true, productId = productId });  // Trả về kết quả JSON báo thành công
    }







    [HttpGet]
    public IActionResult IndexCafe()
    {
        // Lấy UserID của người dùng từ thông tin đăng nhập
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return RedirectToAction("AccessDenied", "Home");
        }

        var userId = int.Parse(userIdClaim.Value);

        // Gọi repository để lấy danh sách cửa hàng
        var cafes = _cafeRepository.GetCafesByUserId(userId);

        return View(cafes);
    }




    // AJAX Delete
    [HttpDelete]
    public IActionResult DeleteProductAjax(int productId)
    {
        var product = _productRepository.GetAllProducts().FirstOrDefault(p => p.ProductID == productId);

        if (product != null)
        {
            _productRepository.DeleteProduct(productId);
            return Ok();
        }

        return NotFound();
    }

    [HttpGet]
    public IActionResult UpdateCafe(int id)
    {
        var cafe = _dataContext.Cafes.FirstOrDefault(c => c.CafeID == id);
        if (cafe == null)
        {
            return NotFound();
        }
        return View(cafe); // Trả về view chứa thông tin cafe để chỉnh sửa
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
    /*private int GetLoggedInUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out int userId) ? userId : 0;
    }*/
}

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
    private readonly PaymentRepository _paymentRepository;

    public ManagerController(DataContext dataContext, ProductRepository productRepository, CafeRepository cafeRepository, PaymentRepository paymentRepository)
    {
        _dataContext = dataContext;
        _productRepository = productRepository;
        _cafeRepository = cafeRepository;
        _paymentRepository = paymentRepository;
    }

    public IActionResult Index()
    {
        if (!CheckAccess(_managerPermission))
        {
            return RedirectToAction("AccessDenied", "Home");
        }
        return RedirectToAction("IndexCafe", "Manager");
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
        return RedirectToAction("IndexCafe", "Manager");
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
    public async Task<IActionResult> AddProduct(ProductModel product, IFormFile uploadedImage)
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

        if (_dataContext.Products.Any(p => p.ProductName == product.ProductName && p.CafeID == product.CafeID))
        {
            ModelState.AddModelError("ProductName", "Sản phẩm này đã tồn tại trong cửa hàng đã chọn. Vui lòng chọn tên khác.");
            ViewBag.Cafes = new SelectList(cafes, "CafeID", "CafeName", product.CafeID);
            return View(product);
        }

        if (product.Price < 0)
        {
            ModelState.AddModelError("Price", "Giá sản phẩm không được nhỏ hơn 0.");
            ViewBag.Cafes = new SelectList(cafes, "CafeID", "CafeName", product.CafeID);
            return View(product);
        }

        if (uploadedImage == null || uploadedImage.Length == 0)
        {
            ModelState.AddModelError("imgUrl", "Bạn phải tải lên một hình ảnh.");
            ViewBag.Cafes = new SelectList(cafes, "CafeID", "CafeName", product.CafeID);
            return View(product);
        }

        string imageName = Guid.NewGuid().ToString() + Path.GetExtension(uploadedImage.FileName);
        string savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/css/Images", imageName);

        using (var stream = new FileStream(savePath, FileMode.Create))
        {
            await uploadedImage.CopyToAsync(stream);
        }

        var newProduct = new ProductModel
        {
            ProductName = product.ProductName,
            Price = product.Price,
            imgUrl = imageName, 
            Description = product.Description,
            Status = true,
            CafeID = product.CafeID
        };

        await _productRepository.AddProductAsync(newProduct);

        return RedirectToAction("IndexProduct", "Manager");
    }



    [HttpGet]
    public IActionResult IndexProduct()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("AccessDenied", "Home");
        }
        var cafeIds = _dataContext.Cafes
            .Where(c => c.UserID.ToString() == userId)
            .Select(c => c.CafeID)
            .ToList();
        var products = _dataContext.Products
            .Where(p => cafeIds.Contains(p.CafeID)) 
            .Include(p => p.Cafe)  
            .ToList();

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

    [HttpGet]
    public IActionResult UpdateProduct(int productId)
    {
        var product = _dataContext.Products.FirstOrDefault(p => p.ProductID == productId);

        if (product == null)
        {
            return NotFound("Sản phẩm không tồn tại.");
        }

        return View(product); // Trả về view với thông tin sản phẩm
    }
    [HttpPost]
    public async Task<IActionResult> UpdateProduct(ProductModel product, IFormFile uploadedImage)
    {
        var existingProduct = _dataContext.Products.FirstOrDefault(p => p.ProductID == product.ProductID);

        if (existingProduct == null)
        {
            return NotFound("Sản phẩm không tồn tại.");
        }

        // Cập nhật thông tin cơ bản của sản phẩm
        existingProduct.ProductName = product.ProductName;
        existingProduct.Price = product.Price;
        existingProduct.Description = product.Description;

        // Nếu có ảnh mới được tải lên
        if (uploadedImage != null && uploadedImage.Length > 0)
        {
            string imageName = Guid.NewGuid().ToString() + Path.GetExtension(uploadedImage.FileName);
            string savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/css/Images", imageName);

            // Lưu ảnh mới
            using (var stream = new FileStream(savePath, FileMode.Create))
            {
                await uploadedImage.CopyToAsync(stream);
            }

            // Xóa ảnh cũ (nếu cần)
            if (!string.IsNullOrEmpty(existingProduct.imgUrl))
            {
                string oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/css/Images", existingProduct.imgUrl);
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            // Cập nhật ảnh trong database
            existingProduct.imgUrl = imageName;
        }

        // Lưu thay đổi vào database
        await _dataContext.SaveChangesAsync();

        return RedirectToAction("IndexProduct", "Manager");
    }

    [HttpGet]
    public IActionResult IndexCafe()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return RedirectToAction("AccessDenied", "Home");
        }

        var userId = int.Parse(userIdClaim.Value);

        var cafes = _cafeRepository.GetCafesByUserId(userId);

        return View(cafes);
    }
    // GET: UpdateCafe
    [HttpGet]
    public IActionResult UpdateCafe(int cafeId)
    {
        var cafe = _dataContext.Cafes.FirstOrDefault(c => c.CafeID == cafeId);

        if (cafe == null)
        {
            return NotFound("Cửa hàng không tồn tại.");
        }

        return View(cafe); // Trả về view với thông tin cửa hàng
    }

    // POST: UpdateCafe
    [HttpPost]
    public async Task<IActionResult> UpdateCafe(CafeModel cafe)
    {
        var existingCafe = _dataContext.Cafes.FirstOrDefault(c => c.CafeID == cafe.CafeID);

        if (existingCafe == null)
        {
            return NotFound("Cửa hàng không tồn tại.");
        }

        // Cập nhật thông tin cửa hàng
        existingCafe.CafeName = cafe.CafeName;
        existingCafe.Address = cafe.Address;
        existingCafe.Phone = cafe.Phone;
        existingCafe.Description = cafe.Description;

        // Lưu thay đổi vào database
        await _dataContext.SaveChangesAsync();

        return RedirectToAction("Index", "Manager"); // Chuyển hướng về danh sách cửa hàng
    }


    [HttpGet]
    public IActionResult Payment()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("AccessDenied", "Home");
        }
        var username = User.Identity?.Name ?? "Guest";

        var userCafeCount = _cafeRepository.GetCafesByUserId(int.Parse(userId)).Where(x => x.Status != false).Count();

        float totalAmount = userCafeCount * 10000000;

        var paymentModel = new PaymentModel
        {
            UserID = int.Parse(userId),
            Amount = totalAmount,
            Date = DateTime.Now
        };

        ViewData["UserName"] = username;
        ViewData["Count"] = userCafeCount;
        ViewData["Amount"] = totalAmount;
        ViewData["Date"] = paymentModel.Date;

        return View(paymentModel);
    }

    [HttpPost]
    public async Task<IActionResult> Payment(PaymentModel payment)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

       if (string.IsNullOrEmpty(userId))
        {
            ModelState.AddModelError("", "Không thể xác định người dùng.");
            AddPaymentViewData(null, 0, payment.Amount); // Đảm bảo dữ liệu được gán
            return View(payment);
        }
        var userCafeCount = _cafeRepository.GetCafesByUserId(int.Parse(userId)).Count();

        if (userCafeCount == 0)
        {
            ModelState.AddModelError("", "Bạn chưa có cửa hàng nào để thanh toán.");
            AddPaymentViewData(User.Identity?.Name, userCafeCount, payment.Amount);
            return View(payment);
        }

        float totalAmount = userCafeCount * 10000000;

        var currentMonth = DateTime.Now.Month;
        var currentYear = DateTime.Now.Year;
        var hasPaid = await _paymentRepository.HasPaymentForMonthAsync(int.Parse(userId), currentMonth, currentYear);

        if (hasPaid)
        {
            ModelState.AddModelError("", "Bạn đã thanh toán hóa đơn cho tháng này.");
            AddPaymentViewData(User.Identity?.Name, userCafeCount, totalAmount);
            return View(payment);
        }

        payment.UserID = int.Parse(userId);
        payment.Amount = totalAmount;
        payment.Date = DateTime.Now;

        try
        {
            await _paymentRepository.AddPaymentAsync(payment);
            ViewBag.PaymentSuccess = "Thanh toán thành công!";
            AddPaymentViewData(User.Identity?.Name, userCafeCount, totalAmount);
            return View(payment); 
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Có lỗi xảy ra khi xử lý thanh toán: {ex.Message}");
            AddPaymentViewData(User.Identity?.Name, userCafeCount, totalAmount);
            return View(payment);
        }
    }

    private void AddPaymentViewData(string? userName, int cafeCount, float totalAmount)
    {
        ViewData["UserName"] = userName ?? "Không xác định"; 
        ViewData["Count"] = cafeCount >= 0 ? cafeCount : 0;  
        ViewData["Amount"] = totalAmount >= 0 ? totalAmount : 0; 
    }

    [HttpPost]
    public async Task<IActionResult> DeleteProduct([FromBody] int productId)
    {
        if (productId <= 0)
        {
            return Json(new { success = false, message = "ID sản phẩm không hợp lệ" });
        }

        var product = await _dataContext.Products.FindAsync(productId);
        if (product == null)
        {
            return Json(new { success = false, message = "Không tìm thấy sản phẩm" });
        }

        product.Status = false;
        await _dataContext.SaveChangesAsync();

        return Json(new { success = true, message = "Sản phẩm đã bị xóa" });
    }

    [HttpPost]
    public async Task<IActionResult> RestoreProduct([FromBody] int productId)
    {
        if (productId <= 0)
        {
            return Json(new { success = false, message = "ID sản phẩm không hợp lệ" });
        }

        var product = await _dataContext.Products.FindAsync(productId);
        if (product == null)
        {
            return Json(new { success = false, message = "Không tìm thấy sản phẩm" });
        }

        product.Status = true; 
        await _dataContext.SaveChangesAsync();

        return Json(new { success = true, message = "Sản phẩm đã được khôi phục" });
    }



    [HttpPost]
    public async Task<IActionResult> DeleteCafe([FromBody] int cafeId)
    {
        if (cafeId <= 0)
        {
            return Json(new { success = false, message = "CafeID không hợp lệ" });
        }

        var cafe = await _dataContext.Cafes.FindAsync(cafeId);
        if (cafe == null)
        {
            return Json(new { success = false, message = "Không tìm thấy cửa hàng" });
        }

        cafe.Status = false;
        await _dataContext.SaveChangesAsync();

        return Json(new { success = true, message = "Cửa hàng đã bị xóa" });
    }

    [HttpPost]
    public async Task<IActionResult> RestoreCafe([FromBody] int cafeId)
    {
        if (cafeId <= 0)
        {
            return Json(new { success = false, message = "CafeID không hợp lệ" });
        }

        var cafe = await _dataContext.Cafes.FindAsync(cafeId);
        if (cafe == null)
        {
            return Json(new { success = false, message = "Không tìm thấy cửa hàng" });
        }

        cafe.Status = true; 
        await _dataContext.SaveChangesAsync();

        return Json(new { success = true, message = "Cửa hàng đã được khôi phục." });
    }

    /* [HttpGet]
     public IActionResult NV()
     {
         var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

         if (string.IsNullOrEmpty(userId))
         {
             return RedirectToAction("AccessDenied", "Home");
         }

         // Lấy danh sách các cửa hàng của người dùng
         var cafeIds = _dataContext.Cafes
             .Where(c => c.UserID.ToString() == userId)
             .Select(c => c.CafeID)
             .ToList();

         // Truyền danh sách cửa hàng vào ViewBag để hiển thị dropdown
         var cafes = _dataContext.Cafes.Where(c => cafeIds.Contains(c.CafeID)).ToList();
         ViewBag.StoreList = cafes;

         // Lấy danh sách sản phẩm của các cửa hàng đã chọn
         var products = _dataContext.Products
             .Where(p => cafeIds.Contains(p.CafeID))
             .Include(p => p.Cafe)
             .ToList();

         var imageDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "css", "Images");
         var imagePaths = products.Select(product =>
         {
             var imagePath = Path.Combine(imageDirectory, product.imgUrl);

             return System.IO.File.Exists(imagePath)
                 ? $"/css/Images/{product.imgUrl}"
                 : "/css/Images/default.png";
         }).ToList();

         // Truyền dữ liệu vào View
         ViewBag.ImagePaths = imagePaths;
         return View(products); // Truyền danh sách sản phẩm
     }*/
    [HttpGet]
    public IActionResult NV()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("AccessDenied", "Home");
        }

        // Lấy danh sách các cửa hàng của người dùng
        var cafeIds = _dataContext.Cafes
            .Where(c => c.UserID.ToString() == userId)
            .Select(c => c.CafeID)
            .ToList();

        var cafes = _dataContext.Cafes.Where(c => cafeIds.Contains(c.CafeID)).ToList();
        ViewBag.StoreList = cafes;

        // Lấy danh sách sản phẩm của các cửa hàng đã chọn
        var products = _dataContext.Products
            .Where(p => cafeIds.Contains(p.CafeID))
            .Include(p => p.Cafe)
            .ToList();

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
    public IActionResult ThanhToan([FromBody] ThanhToanRequest request)
    {
        // Kiểm tra dữ liệu đầu vào
        if (request == null || request.ProductIds == null || request.Quantities == null || request.CafeId <= 0)
        {
            return Json(new { success = false, message = "Dữ liệu không hợp lệ!" });
        }

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Json(new { success = false, message = "Người dùng chưa đăng nhập." });
        }

        if (request.ProductIds.Count == 0 || request.Quantities.Count == 0)
        {
            return Json(new { success = false, message = "Giỏ hàng trống hoặc có lỗi trong dữ liệu sản phẩm!" });
        }

        try
        {
            // Tạo hóa đơn
            var order = new OrderModel
            {
                UserID = int.Parse(userId),
                CafeID = request.CafeId,
                TotalAmount = 0, // Khởi tạo là decimal
                OrderDate = DateTime.Now
            };

            // Lưu hóa đơn vào cơ sở dữ liệu
            _dataContext.Orders.Add(order);
            _dataContext.SaveChanges();

            float totalAmount = 0;

            // Thêm chi tiết hóa đơn
            for (int i = 0; i < request.ProductIds.Count; i++)
            {
                var product = _dataContext.Products.FirstOrDefault(p => p.ProductID == request.ProductIds[i]);
                if (product != null)
                {
                    var orderDetail = new OrderDetailModel
                    {
                        OrderID = order.OrderID,
                        ProductID = request.ProductIds[i],
                        Quantity = request.Quantities[i],
                        Price = product.Price
                    };

                    _dataContext.OrderDetails.Add(orderDetail);
                    totalAmount += product.Price * request.Quantities[i]; // Tính tổng tiền
                }
                else
                {
                    return Json(new { success = false, message = $"Sản phẩm với ID {request.ProductIds[i]} không tồn tại!" });
                }
            }

            // Cập nhật tổng tiền
            order.TotalAmount = totalAmount;
            _dataContext.SaveChanges();

            return Json(new { success = true, message = "Thanh toán thành công!" });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during payment: {ex.Message}");
            return Json(new { success = false, message = "Có lỗi xảy ra khi thanh toán. Vui lòng thử lại." });
        }
    }

    // Tạo lớp để nhận dữ liệu JSON từ frontend
    public class ThanhToanRequest
    {
        public List<int> ProductIds { get; set; }
        public List<int> Quantities { get; set; }
        public int CafeId { get; set; }
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

    [HttpGet]
    public IActionResult Report()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var reports = (from o in _dataContext.Orders
                       join c in _dataContext.Cafes
                       on o.CafeID equals c.CafeID
                       join u in _dataContext.Users
                       on o.UserID equals u.UserID
                       select new
                       {
                           o.OrderID,
                           o.TotalAmount,
                           o.OrderDate,
                           c.CafeID,
                           c.CafeName,
                           u.UserName,
                       }).ToList();

        var cafeIds = _dataContext.Cafes
            .Where(c => c.UserID.ToString() == userId)
            .Select(c => c.CafeID)
            .ToList();

        var cafes = _dataContext.Cafes.Where(c => cafeIds.Contains(c.CafeID)).ToList();
        ViewBag.StoreList = cafes;

        return View(reports);
    }

    [Route("Manager/ReportDetail/{id}")]
    public IActionResult ReportDetail(int id)
    {
        var reportdetail = (from od in _dataContext.OrderDetails
                            join o in _dataContext.Orders
                            on od.OrderID equals o.OrderID
                            join p in _dataContext.Products
                            on od.ProductID equals p.ProductID
                            join c in _dataContext.Cafes
                            on o.CafeID equals c.CafeID
                            select new
                            {
                                c.CafeName,
                                o.OrderID,
                                od.Quantity,
                                p.ProductName,
                                p.Price,
                                o.TotalAmount,
                                o.OrderDate,
                            })
                            .Where(x => x.OrderID == id)
                            .GroupBy(x => new { x.CafeName, x.OrderDate })
                            .Select(group => new
                            {
                                group.Key.CafeName,
                                group.Key.OrderDate,
                                Orders = group.Select(order => new
                                {
                                    order.OrderID,
                                    order.ProductName,
                                    order.Quantity,
                                    order.Price,
                                    order.TotalAmount
                                }).ToList()
                            }).ToList();
        return View(reportdetail);
    }
}

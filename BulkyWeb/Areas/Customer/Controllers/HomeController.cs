
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger , IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
       
        public IActionResult Index()
        {
            IEnumerable<Product> productlist=_unitOfWork.product.GetAll(includeproperties:"Category");
            return View(productlist);
        }
        
        public IActionResult Details(int id)
        {
            Carts cart = new() {
                Product = _unitOfWork.product.Get(u => u.id == id, includeproperties: "Category"),
                Count = 1,
                ProductId = id
            };
           
            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(Carts cart)
        {
            cart.Id = 0;
            var claimsidentity = (ClaimsIdentity)User.Identity;
            var userId = claimsidentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            cart.ApplicationUserId = userId;

            Carts cartfromDb = _unitOfWork.carts.Get(u => u.ApplicationUserId == userId && u.ProductId == cart.ProductId);
            if (cartfromDb != null)
            {
                cartfromDb.Count += cart.Count;
                _unitOfWork.carts.Update(cartfromDb);
            }
            else
            {
                _unitOfWork.carts.Add(cart);
            }
            TempData["success"] = "Cart Added Successfully";
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CartVM CartVM { get; set; }

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsidentity = (ClaimsIdentity)User.Identity;
            var userId = claimsidentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            CartVM = new()
            {
                  Listcarts=_unitOfWork.carts.GetAll(u=>u.ApplicationUserId == userId, includeproperties:"Product"),
            };
            foreach(var cart in CartVM.Listcarts)
            {
                cart.price = GetPriceBasedOnQuantity(cart);
                CartVM.ordertotal += (cart.price * cart.Count);
            }
            return View(CartVM);
        }

        public IActionResult plus(int cartId) 
        { 
            var cartfromdb= _unitOfWork.carts.Get(u=>u.Id== cartId);
            cartfromdb.Count++;
            _unitOfWork.carts.Update(cartfromdb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult minus(int cartId)
        {
            var cartfromdb = _unitOfWork.carts.Get(u => u.Id == cartId);
            if(cartfromdb.Count <= 1)
            {
                _unitOfWork.carts.Remove(cartfromdb);
            }
            else
            {
                cartfromdb.Count--;
                _unitOfWork.carts.Update(cartfromdb);

            }
            
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult remove(int cartId)
        {
            var cartfromdb = _unitOfWork.carts.Get(u => u.Id == cartId);
            
                _unitOfWork.carts.Remove(cartfromdb);
            

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Summary()
        {
            return View();
        }
        private double GetPriceBasedOnQuantity(Carts carts)
        {
            if (carts.Count <= 50)
            {
                return carts.Product.price;
            }
            else
            {
                if(carts.Count <= 100) 
                {
                 return carts.Product.price50;
                }
                else
                {
                    return carts.Product.price100;
                }
            }
        }
    }
}

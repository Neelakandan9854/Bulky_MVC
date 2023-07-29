using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
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
                OrderHead = new()
            };
            foreach(var cart in CartVM.Listcarts)
            {
                cart.price = GetPriceBasedOnQuantity(cart);
                CartVM.OrderHead.OrderTotal += (cart.price * cart.Count);
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
            var claimsidentity = (ClaimsIdentity)User.Identity;
            var userId = claimsidentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            CartVM = new()
            {
                Listcarts = _unitOfWork.carts.GetAll(u => u.ApplicationUserId == userId, includeproperties: "Product"),
                OrderHead = new()
            };
            CartVM.OrderHead.ApplicationUser = _unitOfWork.applicationUser.Get(u => u.Id == userId);
            CartVM.OrderHead.Name= CartVM.OrderHead.ApplicationUser.Name;
            CartVM.OrderHead.StreetAdress = CartVM.OrderHead.ApplicationUser.StreetAdress;
            CartVM.OrderHead.State = CartVM.OrderHead.ApplicationUser.State;
            CartVM.OrderHead.Postalcode = CartVM.OrderHead.ApplicationUser.Postalcode;
            CartVM.OrderHead.PhoneNumber = CartVM.OrderHead.ApplicationUser.PhoneNumber;
            CartVM.OrderHead.City = CartVM.OrderHead.ApplicationUser.City;
            foreach (var cart in CartVM.Listcarts)
            {
                cart.price = GetPriceBasedOnQuantity(cart);
                CartVM.OrderHead.OrderTotal += (cart.price * cart.Count);
            }
            return View(CartVM);
        }
        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPost()
        {
            var claimsidentity = (ClaimsIdentity)User.Identity;
            var userId = claimsidentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            CartVM.Listcarts = _unitOfWork.carts.GetAll(u => u.ApplicationUserId == userId, includeproperties: "Product");

            CartVM.OrderHead.OrderDate = DateTime.Now;
            CartVM.OrderHead.ApplicationUserId = userId;

            ApplicationUser applicationUser= _unitOfWork.applicationUser.Get(u => u.Id == userId);
            
            foreach (var cart in CartVM.Listcarts)
            {
                cart.price = GetPriceBasedOnQuantity(cart);
                CartVM.OrderHead.OrderTotal += (cart.price * cart.Count);
            }
            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                CartVM.OrderHead.PaymentStatus = SD.PaymentStatusPending;
                CartVM.OrderHead.OrderStatus = SD.StatusPending;
            }
            else
            {

                CartVM.OrderHead.PaymentStatus = SD.PaymentStatusDelayedPayment;
                CartVM.OrderHead.OrderStatus = SD.StatusApproved;
            }

            _unitOfWork.OrderHeader.Add(CartVM.OrderHead);
            _unitOfWork.Save();

            foreach(var cart in CartVM.Listcarts)
            {
                OrderDetail orderdetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = CartVM.OrderHead.Id,
                    Price = cart.price,
                    Count = cart.Count
                };
                _unitOfWork.OrderDetail.Add(orderdetail);
                _unitOfWork.Save();
            }

            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                var domain = "http://localhost:5067/";
                var options = new SessionCreateOptions
                {
                  

                  SuccessUrl =  domain + $"customer/cart/OrderConformation?id={CartVM.OrderHead.Id}",
                  CancelUrl = domain +"customer/cart/index",
                  LineItems = new List<SessionLineItemOptions>(),
                  Mode = "payment",
                };
                foreach(var item in CartVM.Listcarts)
                {
                    var sessionLineitem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions()
                        {
                            UnitAmount = (long)(item.price*100),
                            Currency = "inr",
                            ProductData = new SessionLineItemPriceDataProductDataOptions()
                            {
                                Name = item.Product.Tittle
                            }
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionLineitem);
                }
                var service = new SessionService();
                Session session=service.Create(options);
                _unitOfWork.OrderHeader.UpdateStripePaymentID(CartVM.OrderHead.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.Save();
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }
            return RedirectToAction(nameof(OrderConformation) ,new { id= CartVM.OrderHead.Id});
        }


        public IActionResult OrderConformation(int id)
        {
            OrderHead orderHead = _unitOfWork.OrderHeader.Get(u => u.Id == id, includeproperties: "ApplicationUser");
            if(orderHead.PaymentStatus!= SD.PaymentStatusDelayedPayment)
            {
                var service= new SessionService();
                Session session = service.Get(orderHead.SessionId);
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdateStripePaymentID(id, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
            }
            List<Carts> cart = _unitOfWork.carts
                .GetAll(u => u.ApplicationUserId == orderHead.ApplicationUserId).ToList();
            _unitOfWork.carts.RemoveRange(cart);
            _unitOfWork.Save();
            return View(id);
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

using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderVM OrderVM { get; set; }

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Details(int orderId)
        {
            OrderVM = new()
            {
                OrderHead = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeproperties: "ApplicationUser"),
                OrderDetail = _unitOfWork.OrderDetail.GetAll(u=>u.OrderHeaderId == orderId, includeproperties:"Product")
            };
            return View(OrderVM);
        }
        [HttpPost]
        [Authorize (Roles = SD.Role_User_Admin+","+ SD.Role_User_Emp) ]
        public IActionResult UpdateOrderDatail()
        {
            var orderHeaderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHead.Id);

            orderHeaderFromDb.Name = OrderVM.OrderHead.Name;
            orderHeaderFromDb.PhoneNumber = OrderVM.OrderHead.PhoneNumber;
            orderHeaderFromDb.StreetAdress = OrderVM.OrderHead.StreetAdress;
            orderHeaderFromDb.City = OrderVM.OrderHead.City;
            orderHeaderFromDb.State = OrderVM.OrderHead.State;
            orderHeaderFromDb.Postalcode = OrderVM.OrderHead.Postalcode;
            if (!string.IsNullOrEmpty(OrderVM.OrderHead.Carrier))
            {
                orderHeaderFromDb.Carrier = OrderVM.OrderHead.Carrier;
            }
            if (!string.IsNullOrEmpty(OrderVM.OrderHead.TrackingNumber))
            {
                orderHeaderFromDb.Carrier = OrderVM.OrderHead.TrackingNumber;
            }
            _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
            _unitOfWork.Save();
            TempData["success"] = "Order details Updated Successfully";
            return RedirectToAction(nameof(Details), new {orderId=orderHeaderFromDb.Id});
        }
        #region APICALL
        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHead> Objorderheader = _unitOfWork.OrderHeader.GetAll(includeproperties: "ApplicationUser").ToList();

            switch (status)
            {
                case "pending":
                    Objorderheader = Objorderheader.Where(u => u.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;

                case "approved":
                    Objorderheader = Objorderheader.Where(u => u.OrderStatus == SD.StatusApproved);
                    break;
                case "completed":
                    Objorderheader = Objorderheader.Where(u => u.OrderStatus == SD.StatusShipped);
                    break;
                case "inprocess":
                    Objorderheader = Objorderheader.Where(u => u.OrderStatus == SD.StatusInProcess);
                    break;
                default:
                   
                    break;
            }

            return Json(new { data = Objorderheader });
        }
       
        #endregion
    }
}

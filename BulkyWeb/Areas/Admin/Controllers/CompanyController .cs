using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing.Constraints;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_User_Admin)]
    public class CompanyController:Controller
    {
        private readonly IUnitOfWork _unitofwork;

        public CompanyController(IUnitOfWork Unitofwork )
        {
            _unitofwork = Unitofwork;
            
        }
        public IActionResult Index()
        {
            List<Company> Cat = _unitofwork.company.GetAll().ToList();
           
            return View(Cat);
        }
        public IActionResult Upsert(int? id)
        {

            if(id== null || id == 0)
            {
                return View(new Company());
            }
            else
            {
                Company companyobj = _unitofwork.company.Get(u =>u.Id == id);
                return View(companyobj);
            }
            
        }

        [HttpPost]
        public IActionResult Upsert(Company companyobj)
        {
            
            if (ModelState.IsValid)
            {
               
                if(companyobj.Id == 0) 
                {
                    _unitofwork.company.Add(companyobj);
                }
                else
                {
                    _unitofwork.company.Update(companyobj);
                }

               
                _unitofwork.Save();
                TempData["success"] = "Company created sucessfully";
                return RedirectToAction("Index");
            }
            else
            {
               
                   
                return View(companyobj);
            }
            
        }

        

        #region APICALL
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> Cat = _unitofwork.company.GetAll().ToList();

            return Json(new {data= Cat});
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var CompanyToBeDeleted= _unitofwork.company.Get(u=> u.Id==id);

            if(CompanyToBeDeleted == null)
            {
                return Json(new { success = false , message = "Error while deleting"});
            }
            
            _unitofwork.company.Remove(CompanyToBeDeleted);
            _unitofwork.Save();
            return Json(new { success = true, message = " Succesfully deleted" });
        }
        #endregion

    }
}

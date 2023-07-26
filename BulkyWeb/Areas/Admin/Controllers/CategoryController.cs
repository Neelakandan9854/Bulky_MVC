
using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_User_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitofwork;
        public CategoryController(IUnitOfWork Unitofwork)
        {
            _unitofwork = Unitofwork;
        }
        public IActionResult Index()
        {
            List<Category> Cat = _unitofwork.category.GetAll().ToList();
            return View(Cat);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "DisplayName Cannot Exactly Match the name");
            }
            if (ModelState.IsValid)
            {
                _unitofwork.category.Add(obj);
                _unitofwork.Save();
                TempData["success"] = "Category created sucessfully";
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult Edit(int id)
        {
            Category categoryfromDb = _unitofwork.category.Get(u => u.Id == id);

            if (categoryfromDb == null)
            {
                return NotFound();
            }


            return View(categoryfromDb);
        }
        [HttpPost]
        public IActionResult Edit(Category cat)
        {


            if (ModelState.IsValid)
            {
                _unitofwork.category.Update(cat);
                _unitofwork.Save();
                TempData["success"] = "Category Updated sucessfully";
                return RedirectToAction("Index");
            }


            return View();
        }
        public IActionResult Delete(int id)
        {

            Category categoryfromDb = _unitofwork.category.Get(u => u.Id == id);

            if (categoryfromDb == null)
            {
                return NotFound();
            }
            _unitofwork.category.Remove(categoryfromDb);
            TempData["success"] = "Category Deleted sucessfully";
            _unitofwork.Save();

            return RedirectToAction("Index");

        }
    }
}

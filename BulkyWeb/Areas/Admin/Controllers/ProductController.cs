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
    public class ProductController:Controller
    {
        private readonly IUnitOfWork _unitofwork;

        private readonly IWebHostEnvironment _webhostenvironment;
        public ProductController(IUnitOfWork Unitofwork , IWebHostEnvironment webhostenvironment)
        {
            _unitofwork = Unitofwork;
            _webhostenvironment = webhostenvironment;
        }
        public IActionResult Index()
        {
            List<Product> Cat = _unitofwork.product.GetAll(includeproperties:"Category").ToList();
           
            return View(Cat);
        }
        public IActionResult Upsert(int? id)
        {



            ProductVM productVM = new()
            {
                CategoryList =_unitofwork.category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
            Product = new Product()
            };

            if(id== null || id == 0)
            {
                return View(productVM);
            }
            else
            {
                productVM.Product = _unitofwork.product.Get(u =>u.id == id);
                return View(productVM);
            }
            
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM obj, IFormFile? file)
        {
            
            if (ModelState.IsValid)
            {
                string Rootpath = _webhostenvironment.WebRootPath;
                if(file != null )
                {
                   string  filename = Guid.NewGuid().ToString()+Path.GetExtension(file.FileName) ;
                    string productpath = Path.Combine(Rootpath, @"Images\Products");
                    if(!string.IsNullOrEmpty(obj.Product.ImageUrl))
                    {
                        var oldimage = Path.Combine(Rootpath, obj.Product.ImageUrl.TrimStart('\\'));

                        if(System.IO.File.Exists(oldimage))
                        {
                            System.IO.File.Delete(oldimage);
                        }
                    }

                    using( var fileStream=  new FileStream(Path.Combine(productpath, filename), FileMode.Create))
                    {
                        file.CopyTo(fileStream);

                        obj.Product.ImageUrl = @"\Images\Products\"+filename;
                    }

                }
                if( obj.Product.id == 0) 
                {
                    _unitofwork.product.Add(obj.Product);
                }
                else
                {
                    _unitofwork.product.Update(obj.Product);
                }

               
                _unitofwork.Save();
                TempData["success"] = "Product created sucessfully";
                return RedirectToAction("Index");
            }
            else
            {
                obj.CategoryList = _unitofwork.category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                   
                return View(obj);
            }
            
        }

        

        #region APICALL
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> Cat = _unitofwork.product.GetAll(includeproperties: "Category").ToList();

            return Json(new {data= Cat});
        }
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted= _unitofwork.product.Get(u=> u.id==id);

            if(productToBeDeleted == null)
            {
                return Json(new { success = false , message = "Error while deleting"});
            }
            var oldimage = Path.Combine(_webhostenvironment.WebRootPath,productToBeDeleted.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(oldimage))
            {
                System.IO.File.Delete(oldimage);
            }
            _unitofwork.product.Remove(productToBeDeleted);
            _unitofwork.Save();
            return Json(new { success = true, message = " Succesfully deleted" });
        }
        #endregion

    }
}

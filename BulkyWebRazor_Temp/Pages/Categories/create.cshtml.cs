using BulkyWebRazor_Temp.Data;
using BulkyWebRazor_Temp.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazor_Temp.Pages.Categories
{
    [BindProperties]
    public class createModel : PageModel
    {
        
        private readonly ApplicationDbContext _db;

        public Category Category { get; set; }

        public createModel(ApplicationDbContext db)
        {
            _db = db;
        }
        public void OnGet()
        {
        }

        public IActionResult OnPost() 
        {
           _db.Categories.Add(Category);
           _db.SaveChanges();
           return RedirectToPage("Index");
        }
    }
}

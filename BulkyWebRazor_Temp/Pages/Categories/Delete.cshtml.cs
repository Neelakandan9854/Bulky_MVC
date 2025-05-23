using BulkyWebRazor_Temp.Data;
using BulkyWebRazor_Temp.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazor_Temp.Pages.Categories
{
    [BindProperties]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public Category Category { get; set; }

        public DeleteModel(ApplicationDbContext db)
        {
            _db= db;
        }


        public void OnGet(int? id)
        {
            Category = _db.Categories.Find(id);
        }

        public IActionResult OnPost()
        {
            _db.Categories.Remove(Category);
            _db.SaveChanges();
            return RedirectToPage("Index");
        }
    }
}

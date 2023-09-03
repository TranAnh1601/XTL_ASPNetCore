using Microsoft.AspNetCore.Mvc;
using XTL_ASPNetCore.Models;

namespace XTL_ASPNetCore.Areas.Database.Controllers
{
    [Area("Database")]
    [Route("/database-manage/[action]")]
    public class DbManageController : Controller
    {
        private readonly AppDbContext _context;
        public DbManageController(AppDbContext context)
        {
            _context = context;
        }      
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Delete()
        {
            return View();
        }
        [TempData]
        public string StatusMessage { set; get; }
        [HttpPost]
        public async Task<IActionResult> DeleteAsync()
        {
            var success = await _context.Database.EnsureCreatedAsync();
            //StatusMessage = success ? "detete success" : "cant delete";
            //return RedirectToAction(nameof(Index));
            return View(Delete);
        }
    }
}

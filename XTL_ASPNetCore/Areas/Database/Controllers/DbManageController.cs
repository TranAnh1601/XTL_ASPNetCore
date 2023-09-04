using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using XTL_ASPNetCore.Data;
using XTL_ASPNetCore.Models;

namespace XTL_ASPNetCore.Areas.Database.Controllers
{
    [Area("Database")]
    [Route("/database-manage/[action]")]
    public class DbManageController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;



        public DbManageController(AppDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
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
        public async Task<IActionResult> SeedDataAsync()
        {
            var rolenames = typeof(RoleName).GetFields().ToList();
            foreach(var r in rolenames)
            {
                var rolename = (string)r.GetRawConstantValue();
                var rfound = await _roleManager.FindByIdAsync(rolename);
                if(rfound == null)
                {
                    await _roleManager.CreateAsync(new IdentityRole(rolename));
                }
            }
            //add admin
            var useradmin = await _userManager.FindByEmailAsync("admin");
            if(useradmin==null)
            {
                useradmin = new AppUser()
                {
                    UserName = "admin",
                    Email = "admin@gmail.com",
                    EmailConfirmed = true
                };
                await _userManager.CreateAsync(useradmin , "123456");
                await _userManager.AddToRoleAsync(useradmin, RoleName.Administrator);
            }
            StatusMessage = "seed database";
            return RedirectToAction("Index");
        }
    }
}

using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using XTL_ASPNetCore.Data;
using XTL_ASPNetCore.Models;
using XTL_ASPNetCore.Models.Blog;

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
            var useradmin = await _userManager.FindByEmailAsync("admin@gmail.com");
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
            SeedPostcategory();

            StatusMessage = "seed database";
            return RedirectToAction("Index");
        }

        private void SeedPostcategory()
        {
            _context.categories.RemoveRange(_context.categories.Where(c => c.Description.Contains("[fakeData]")));
            _context.Posts.RemoveRange(_context.Posts.Where(p => p.Content.Contains("[fakeData]")));


            var fakerCategory = new Faker<Category>();
            int cm = 1;
            fakerCategory.RuleFor(c => c.Title, fk => $"CM{cm++} " + fk.Lorem.Sentence(1, 2).Trim('.'));
            fakerCategory.RuleFor(c => c.Description, fk => fk.Lorem.Sentences(5) + "[fakeData]");
            fakerCategory.RuleFor(c => c.Slug, fk => fk.Lorem.Slug());

            //sinh ra code
            var cate1 = fakerCategory.Generate();
            var cate11 = fakerCategory.Generate();
            var cate12 = fakerCategory.Generate();
            var cate2 = fakerCategory.Generate();
            var cate21 = fakerCategory.Generate();
            var cate211 = fakerCategory.Generate();
            //quan he cha con
            cate11.ParentCategory = cate1;
            cate12.ParentCategory = cate1;
            cate21.ParentCategory = cate2;
            cate211.ParentCategory = cate21;
            //add vao category
            var categories = new Category[] { cate1, cate2, cate12, cate11, cate21, cate211 };
            _context.categories.AddRange(categories);



            // POST
            var rCateIndex = new Random();
            int bv = 1;

            var user = _userManager.GetUserAsync(this.User).Result;
            var fakerPost = new Faker<Post>();
            fakerPost.RuleFor(p => p.AuthorId, f => user.Id);
            fakerPost.RuleFor(p => p.Content, f => f.Lorem.Paragraphs(7) + "[fakeData]");
            fakerPost.RuleFor(p => p.DateCreated, f => f.Date.Between(new DateTime(2021, 1, 1), new DateTime(2021, 7, 1)));
            fakerPost.RuleFor(p => p.Description, f => f.Lorem.Sentences(3));
            fakerPost.RuleFor(p => p.Published, f => true);
            fakerPost.RuleFor(p => p.Slug, f => f.Lorem.Slug());
            fakerPost.RuleFor(p => p.Title, f => $"Bài {bv++} " + f.Lorem.Sentence(3, 4).Trim('.'));

            List<Post> posts = new List<Post>();
            List<PostCategory> post_categories = new List<PostCategory>();


            for (int i = 0; i < 40; i++)
            {
                var post = fakerPost.Generate();
                post.DateUpdated = post.DateCreated;
                posts.Add(post);
                post_categories.Add(new PostCategory()
                {
                    Post = post,
                    Category = categories[rCateIndex.Next(5)] // truy cap ngau nhin tu 1 den 5 category
                });
            }

            _context.AddRange(posts);
            _context.AddRange(post_categories);
           // END POST
            _context.SaveChanges();
        }
    }
}

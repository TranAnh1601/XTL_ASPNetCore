using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using XTL_ASPNetCore.Areas.Identity.Models.UserViewModels;
using XTL_ASPNetCore.Areas.Product.Models;
using XTL_ASPNetCore.Data;
using XTL_ASPNetCore.Models;
using XTL_ASPNetCore.Models.Blog;
using XTL_ASPNetCore.Models.Product;
using XTL_ASPNetCore.Utilities;

namespace XTL_ASPNetCore.Areas.Product.Controllers
{
    [Area("Product")]
    [Route("admin/productmanager/[action]/{id?}")]
    [Authorize(Roles = RoleName.Editor +"," + RoleName.Administrator)]
    public class ManageProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        public ManageProductController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Blog/Posts
        // lay du lieu = query name = p
        [TempData]
        public string StatusMessage { get; set; }
        public async Task<IActionResult> Index([FromQuery(Name ="p")]int currentPage, int pagesize)
        {
            var posts =  _context.Products.Include(p => p.Author).OrderByDescending(p => p.DateCreated).AsQueryable();
            //return View(posts);
            //-----***----phan trang
            var totalPosts = await posts.CountAsync(); // dem phan tu post
            if (pagesize <= 0) pagesize = 10; // 10 bai viet tren 1 trang
            int countPages = (int)Math.Ceiling((double)totalPosts / pagesize); //tinh toan co bao nhiu trang

            if (currentPage > countPages) currentPage = countPages;
            if (currentPage < 1) currentPage = 1;

#pragma warning disable CS8603 // Possible null reference return.
            var pagingModel = new PagingModel()
            {
                countpages = countPages,
                currentpage = countPages,
                generateUrl = (pageNumber) => Url.Action("Index", new
                {
                    p = pageNumber,
                    // nhan 1 so nguyen page va tra ve 1 chuoi url
                    pagesize = pagesize
                }), 
            };
#pragma warning restore CS8603 // Possible null reference return.

            ViewBag.pagingModel = pagingModel;
            ViewBag.totalPosts = totalPosts;

            var postInPage = await posts.Skip((currentPage - 1) * pagesize)
                        .Take(pagesize)
                        .Include(p => p.ProductCategoryProducts)
                        .ThenInclude(pc => pc.Category).ToListAsync();

           
            return View(postInPage);
        }

        // GET: Blog/Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var post = await _context.Products
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Blog/Posts/Create
        public async Task<IActionResult> Create()
        {
            //ViewData["AuthorId"] = new SelectList(_context.Users,"Id","Id");
            var categories = await _context.CategoryProducts.ToListAsync();
            ViewData["categories"] = new MultiSelectList(categories, "Id", "Title"); // taọ multi (data, key, hiển thị)
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Slug,Content,Published,CategoryIDs,Price")] CreateProductModel product)
        {
            var categories = await _context.CategoryProducts.ToListAsync();
            ViewData["categories"] = new MultiSelectList(categories, "Id", "Title");
            if (product.Slug == null)
            {
                product.Slug = AppUtilities.GenerateSlug(product.Title);
            }
            if (await _context.Products.AnyAsync(p => p.Slug == product.Slug))
            {
                ModelState.AddModelError("Slug", "Nhập chuỗi Url khác");
                return View(product);
            }
            if (!ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(this.User);
                product.DateCreated = product.DateUpdated = DateTime.Now;
                product.AuthorId = user.Id;
             
                _context.Add(product);

                if (product.CategoryIDs != null)
                {
                    foreach (var CateId in product.CategoryIDs)
                    {
                        _context.Add(new ProductCategoryProduct()
                        {
                            CategoryID = CateId,
                            Product = product
                        });
                    }
                }


                await _context.SaveChangesAsync();
                StatusMessage = "Vừa tạo bài viết mới";
                return RedirectToAction(nameof(Index));
            }


            return View(product);
        }
        // GET: Blog/Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // var post = await _context.Posts.FindAsync(id);
            var product = await _context.Products.Include(p => p.ProductCategoryProducts).FirstOrDefaultAsync(p => p.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

            var postEdit = new CreateProductModel()
            {
                ProductID = product.ProductID,
                Title = product.Title,
                Content = product.Content,
                Description = product.Description,
                Slug = product.Slug,
                Published = product.Published,
                CategoryIDs = product.ProductCategoryProducts.Select(pc => pc.CategoryID).ToArray(),
                Price = product.Price
            };

            var categories = await _context.CategoryProducts.ToListAsync();
            ViewData["categories"] = new MultiSelectList(categories, "Id", "Title");

            return View(postEdit);
        }


        // POST: Blog/Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductID,Title,Description,Slug,Content,Published,CategoryIDs,Price")] CreateProductModel product)
        {
            if (id != product.ProductID)
            {
                return NotFound();
            }
            var categories = await _context.CategoryProducts.ToListAsync();
            ViewData["categories"] = new MultiSelectList(categories, "Id", "Title");


            if (product.Slug == null)
            {
                product.Slug = AppUtilities.GenerateSlug(product.Title);
            }

            if (await _context.Products.AnyAsync(p => p.Slug == product.Slug && p.ProductID != id))
            {
                ModelState.AddModelError("Slug", "Nhập chuỗi Url khác");
                return View(product);
            }


            if (ModelState.IsValid)
            {
                try
                {

                    var productUpdate = await _context.Products.Include(p => p.ProductCategoryProducts).FirstOrDefaultAsync(p => p.ProductID == id);
                    if (productUpdate == null)
                    {
                        return NotFound();
                    }
                    
                    productUpdate.Title = product.Title;
                    productUpdate.Description = product.Description;
                    productUpdate.Content = product.Content;
                    productUpdate.Published = product.Published;
                    productUpdate.Slug = product.Slug;
                    productUpdate.DateUpdated = DateTime.Now;
                    productUpdate.Price = product.Price;

                    // Update PostCategory
                    if (product.CategoryIDs == null) product.CategoryIDs = new int[] { };

                    var oldCateIds = productUpdate.ProductCategoryProducts.Select(c => c.CategoryID).ToArray();
                    var newCateIds = product.CategoryIDs;

                    var removeCatePosts = from productCate in productUpdate.ProductCategoryProducts
                                          where (!newCateIds.Contains(productCate.CategoryID))
                                          select productCate;
                    _context.ProductCategoryProducts.RemoveRange(removeCatePosts);

                    var addCateIds = from CateId in newCateIds
                                     where !oldCateIds.Contains(CateId)
                                     select CateId;

                    foreach (var CateId in addCateIds)
                    {
                        _context.ProductCategoryProducts.Add(new ProductCategoryProduct()
                        {
                            ProductID = id,
                            CategoryID = CateId
                        });
                    }

                    _context.Update(productUpdate);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                StatusMessage = "Vừa cập nhật sp";
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Users, "Id", "Id", product.AuthorId);
            return View(product);
        }

        // GET: Blog/Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Blog/Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'AppDbContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
          return (_context.Products?.Any(e => e.ProductID== id)).GetValueOrDefault();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using XTL_ASPNetCore.Models.Product;

namespace XTL_ASPNetCore.Views.Shared.Components.CategorySidebar
{
    [ViewComponent]
    public class CategoryProductSidebar : ViewComponent
    {

        public class CategorySidebarData
        {
            public List<CategoryProduct> Categories { get; set; }
            public int level { get; set; }

            public string categoryslug { get; set; }

        }

        public IViewComponentResult Invoke(CategorySidebarData data)
        {
            return View(data);
        }

    }
}

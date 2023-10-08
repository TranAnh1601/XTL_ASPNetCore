using Bogus.DataSets;
using System.ComponentModel.DataAnnotations;
using XTL_ASPNetCore.Models.Blog;
using XTL_ASPNetCore.Models.Product;

namespace XTL_ASPNetCore.Areas.Product.Models
{
    public class CreateProductModel : ProductModel
    {
        [Display(Name = "Chuyên mục")]
        public int[] CategoryIDs { get; set; } // post thuoc category nao
    }
}

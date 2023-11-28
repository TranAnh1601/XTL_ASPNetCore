using XTL_ASPNetCore.Models.Product;

namespace XTL_ASPNetCore.Areas.Product.Models
{
    public class CartItem
    {
        public int quantity { set; get; }
        public ProductModel product { set; get; }
    }
}

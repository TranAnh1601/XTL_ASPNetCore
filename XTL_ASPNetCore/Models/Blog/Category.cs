using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace XTL_ASPNetCore.Models.Blog
{
    [Table("Category")]
    public class Category
    {

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Phải có tên danh mục")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "{0} dài {1} đến {2}")]
        [Display(Name = "Tên danh mục")]
        public string Title { get; set; } // Tiều đề Category

       
        [DataType(DataType.Text)]
        [Display(Name = "Nội dung danh mục")]
        public string Description { get; set; }  // Nội dung, thông tin chi tiết về Category

      
        [Required(ErrorMessage = "Phải tạo url")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "{0} dài {1} đến {2}")]
        [RegularExpression(@"^[a-z0-9-]*$", ErrorMessage = "Chỉ dùng các ký tự [a-z0-9-]")]
        [Display(Name = "Url hiện thị")]
        public string Slug { set; get; }   //chuỗi Url 
        public ICollection<Category> CategoryChildren { get; set; }   // Các Category con
      
        [Display(Name = "Danh mục cha")]
        public int? ParentCategoryId { get; set; }  // Category cha (FKey)

        [ForeignKey("ParentCategoryId")]
        [Display(Name = "Danh mục cha")]
        public Category ParentCategory { set; get; }


        //public void ChildCategoryIDs(ICollection<Category> childcates, List<int> lists)
        //{
        //    if (childcates == null)
        //        childcates = this.CategoryChildren;

        //    foreach (Category category in childcates)
        //    {
        //        lists.Add(category.Id);
        //        ChildCategoryIDs(category.CategoryChildren, lists);

        //    }
        //}

        //public List<Category> ListParents()
        //{
        //    List<Category> li = new List<Category>();
        //    var parent = this.ParentCategory;
        //    while (parent != null)
        //    {
        //        li.Add(parent);
        //        parent = parent.ParentCategory;

        //    }
        //    li.Reverse();
        //    return li;
        //}
    }
}

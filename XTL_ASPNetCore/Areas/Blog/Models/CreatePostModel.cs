﻿using Bogus.DataSets;
using System.ComponentModel.DataAnnotations;
using XTL_ASPNetCore.Models.Blog;

namespace XTL_ASPNetCore.Areas.Blog.Models
{
    public class CreatepostModel : Post
    {
        [Display(Name = "Chuyên mục")]
        public int[] CategoryIDs { get; set; } // post thuoc category nao
    }
}

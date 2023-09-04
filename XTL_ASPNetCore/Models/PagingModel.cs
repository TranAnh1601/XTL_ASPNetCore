using System;

namespace XTL_ASPNetCore.Models
{
    public class PagingModel
    {
        public int currentpage { get; set; }
        public int countpages { get; set; }
        public Func<int?, string> generateUrl { get; set; }
        
    }
   
}
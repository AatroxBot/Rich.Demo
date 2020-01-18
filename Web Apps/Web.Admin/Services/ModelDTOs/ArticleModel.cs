using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Admin.Services.ModelDTOs
{
    public class ArticleModel
    {
        
        public int Id { get; set; }
       
        public string Pid { get; set; }

        public string Title { get; set; }

        public string Tag { get; set; }
       
        public string Content { get; set; }

        public string ImagePath { get; set; }

        public bool Checked { get; set; }
      
        public string CreateUser { get; set; }
       
        public DateTime CreateTime { get; set; }
        public string UpdateUser { get; set; }
        public DateTime UpdateTime { get; set; }
        public bool IsDelete { get; set; }
    }
}

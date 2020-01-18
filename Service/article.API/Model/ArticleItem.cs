using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace article.API.Model
{
    public class ArticleItem
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Pid { get; set; }

        public string Title { get; set; }

        public string Tag { get; set; }
        [StringLength(255)]
        public string Content { get; set; }

        public string ImagePath { get; set; }

        public bool Checked { get; set; }
      
        public string CreateUser { get; set; }
       
        public DateTime CreateTime { get; set; }
        public string UpdateUser { get; set; }
        public DateTime UpdateTime { get; set; }
       
        public bool IsDelete { get; set; }

        public string ZipPath { get; set; }
        public int DownCount { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace article.API.Model
{
    public class MenuItem
    {
        [Required]
        public string Id { get; set; }
        public string Cid { get; set; }

        public string Name { get; set; }

        public string Tag { get; set; }

        public bool IsDelete { get; set; }

        public int? SortCode { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateTime { get; set; }
        public string UpdateUser { get; set; }
        public DateTime UpdateTime { get; set; }

       
    }
}

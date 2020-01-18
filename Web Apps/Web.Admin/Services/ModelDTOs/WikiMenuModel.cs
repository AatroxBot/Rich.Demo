using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Admin.Services.ModelDTOs
{
    public class WikiMenuModel
    {
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace article.API.Data
{
    public class ArticleSettings
    {
        public string PicBaseUrl { get; set; }

        public string EventBusConnection { get; set; }

        public bool UseCustomizationData { get; set; }
        public bool AzureStorageEnabled { get; set; }
    }
}

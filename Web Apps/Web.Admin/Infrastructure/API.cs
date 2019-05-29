using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Admin.Infrastructure
{
    public static class API
    {
        public static class Article
        {
            public static string GetAllArticles(string baseUri, int page, int take, string title="")
            {
                var filterQs = "";

                if (!string.IsNullOrEmpty(title))
                {
                    filterQs = $"/title/{title}";

                }
                else
                {
                    filterQs = string.Empty;
                }

                return $"{baseUri}/items{filterQs}?pageIndex={page}&pageSize={take}";
            }

            public static string GetArticleById(string baseUri, int id)
            {
                return $"{baseUri}/items/{id}";
            }

            public static string AddItem(string baseUri)
            {
                return $"{baseUri}/items";
            }

            public static string UpdateItem(string baseUri)
            {
                return $"{baseUri}/items";
            }

            public static string DeleteItem(string baseUri,int id)
            {
                return $"{baseUri}/{id}";
            }
        }
    }
}

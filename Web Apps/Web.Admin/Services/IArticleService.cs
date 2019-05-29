using Ace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Admin.ViewModels;

namespace Web.Admin.Services
{
    public interface IArticleService
    {
        Task<PagedData<ArticleModel>> GetAllArticles(int pageSize, int pageIndex);

        Task<ArticleModel> ItemByIdAsync(int id);

        Task<string> Add(ArticleModel input);

        Task Update(ArticleModel input);


        Task Delete(int id);
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ace;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Admin.Common;
using Web.Admin.Services;
using Web.Admin.Services.ModelDTOs;
using Web.Admin.ViewModels;

namespace Web.Admin.Areas.WikiManage.Controllers
{
    [Area("WikiManage")]
    [Authorize]
    public class WikiDocumentController : WebController
    {
        private readonly IArticleService _articleService;

        public WikiDocumentController(IArticleService articleService)
        {
            _articleService = articleService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetModels(Pagination pagination, string keyword)
        {
            PagedData<ArticleModel> pagedData =await _articleService.GetAllArticles(pagination.PageSize, pagination.Page);
            return this.SuccessData(pagedData);
        }

        //public ActionResult GetDocument(string id)
        //{
        //    IWikiDocumentAppService service = this.CreateService<IWikiDocumentAppService>();
        //    WikiDocumentDetail doc = service.GetDocumentDetail(id);
        //    return this.SuccessData(doc);
        //}

        public async Task<ActionResult> Document(int id)
        {
            ArticleModel doc = new ArticleModel();

            if (id != 0)
            {
                ArticleModel detail = await _articleService.ItemByIdAsync(id);

                if (detail == null)
                {
                    /* 404 */
                }

                doc.Id = detail.Id;
                doc.Title = detail.Title;
                doc.Tag = detail.Tag;
                doc.Content = detail.Content;
            }

            this.ViewBag.Doc = doc;

            return View();
        }


        [HttpPost]
        //[System.Web.Mvc.ValidateInput(false)]
        public async Task<ActionResult> Add(ArticleModel input)
        {
            input.CreateTime = DateTime.Now;
            input.CreateUser = this.CurrentSession.UserName;
            string id = await _articleService.Add(input);
            return this.SuccessData(id);
        }

        [HttpPost]
        //[System.Web.Mvc.ValidateInput(false)]
        public async Task<ActionResult> Update(ArticleModel input)
        {
            input.UpdateTime = DateTime.Now;
            input.UpdateUser = this.CurrentSession.UserName;
            await _articleService.Update(input);
            return this.SuccessMsg("更新成功");
        }
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            await _articleService.Delete(id);
            return this.SuccessMsg("删除成功");
        }



    }
}

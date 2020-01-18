using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Admin.Common;
using Web.Admin.Common.Tree;
using Web.Admin.Services;
using Web.Admin.Services.ModelDTOs;

namespace Web.Admin.Areas.WikiManage.Controllers
{
    [Area("WikiManage")]
    [Authorize]
    public class WikiMenuController : WebController
    {
        private readonly IArticleService _articleService;

        public WikiMenuController(IArticleService articleService)
        {
            _articleService = articleService;
        }


        public async Task<IActionResult> Index()
        {
            List<WikiMenuModel> rootMenuItems = await _articleService.GetRootWikiMenuItems();
            this.ViewBag.RootMenuItems = rootMenuItems;

            return View();
        }

        public async Task<IActionResult> GetModels(string keyword)
        {
            var data =await _articleService.GetWikiMenuItems();
            if (!string.IsNullOrEmpty(keyword))
            {
                data = TreeHelper.TreeWhere(data, a => a.Name.Contains(keyword), a => a.Id, a => a.Cid);
            }

            List<DataTableTree> ret = new List<DataTableTree>();
            DataTableTree.AppendChildren(data, ref ret, null, 0, a => a.Id, a => a.Cid, a => a.SortCode);

            return this.SuccessData(ret);
        }

        [HttpPost]
        public async Task<ActionResult> Add(WikiMenuModel input)
        {
            input.CreateTime = DateTime.Now;
            input.CreateUser = this.CurrentSession.UserName;
            WikiMenuModel entity = await _articleService.Add(input);
            return this.AddSuccessData(entity);
        }

        [HttpPost]
        public async Task<ActionResult> Update(WikiMenuModel input)
        {
            input.UpdateTime = DateTime.Now;
            input.UpdateUser = this.CurrentSession.UserName;
            await _articleService.Update(input);
            return this.UpdateSuccessMsg();
        }
        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            await _articleService.Delete(id);
            return this.DeleteSuccessMsg();
        }


    }
}
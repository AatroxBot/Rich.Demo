using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Ace;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Admin.Common;
using Web.Admin.Services;
using Web.Admin.ViewModels;

namespace Web.Admin.Controllers
{
    [Authorize]
    public class HomeController : WebController
    {
        private readonly IIdentityParser<ApplicationUser> _appUserParser;

        public HomeController(IIdentityParser<ApplicationUser> appUserParser)
        {
            _appUserParser = appUserParser;
        }


        public IActionResult Index()
        {
            var user = _appUserParser.Parse(HttpContext.User);
            this.ViewBag.CurrentSession = this.CurrentSession;
            return View();
        }

        public ActionResult Default()
        {
            this.ViewBag.CurrentSession = this.CurrentSession;
            return View();
        }

        [HttpGet]
        public ActionResult GetClientsDataJson()
        {
            var data = new
            {
                //dataItems = this.GetDataItemList(),
                //organize = this.GetOrganizeList(),
                //role = this.GetRoleList(),
                //duty = this.GetDutyList(),
                user = "",
                authorizeMenu = this.GetMenuList(),
                //authorizeButton = this.GetMenuButtonList(),
            };
            return this.SuccessData(data);
        }

        public ActionResult Error()
        {
            return this.View();
        }

        object GetMenuList()
        {
           //暂时不做权限控制,写死
            var roleId = this.CurrentSession.RoleId;
            #region menu
            string menu = "[{'Id':'e7e1cfb2856d492faeaadc8e2962ac76','ParentId':null,'EnCode':null,'Name':'Wiki管理','Icon':'fa fa-gears','UrlAddress':null,'OpenTarget':'expand','IsMenu':false,'IsExpand':false,'IsPublic':false,'SortCode':2,'Description':null,'CreationTime':'2016-11-29T23:05:58.513','CreateUserId':'9f2ec079-7d0f-4fe2-90ab-8b09a8302aba','LastModifyTime':'2016-11-29T23:14:03.633','LastModifyUserId':'9f2ec079-7d0f-4fe2-90ab-8b09a8302aba','IsEnabled':false,'IsDeleted':false,'DeleteTime':null,'DeleteUserId':null,'ChildNodes':[{'Id':'6e5b779e849e459f957f3abef2a277e6','ParentId':'e7e1cfb2856d492faeaadc8e2962ac76','EnCode':null,'Name':'文档管理','Icon':null,'UrlAddress':'/WikiManage/WikiDocument/Index','OpenTarget':'iframe','IsMenu':true,'IsExpand':false,'IsPublic':false,'SortCode':1,'Description':null,'CreationTime':'2016-11-29T23:08:11.833','CreateUserId':'9f2ec079-7d0f-4fe2-90ab-8b09a8302aba','LastModifyTime':'2016-11-29T23:08:28.57','LastModifyUserId':'9f2ec079-7d0f-4fe2-90ab-8b09a8302aba','IsEnabled':true,'IsDeleted':false,'DeleteTime':null,'DeleteUserId':null,'ChildNodes':[]},{'Id':'a7f1f2f73ac74b5ba8421ed9b3840439','ParentId':'e7e1cfb2856d492faeaadc8e2962ac76','EnCode':null,'Name':'菜单管理','Icon':null,'UrlAddress':'/WikiManage/WikiMenu/Index','OpenTarget':'iframe','IsMenu':true,'IsExpand':false,'IsPublic':false,'SortCode':2,'Description':null,'CreationTime':'2016-11-29T23:09:14.787','CreateUserId':'9f2ec079-7d0f-4fe2-90ab-8b09a8302aba','LastModifyTime':null,'LastModifyUserId':null,'IsEnabled':true,'IsDeleted':false,'DeleteTime':null,'DeleteUserId':null,'ChildNodes':[]}]},{'Id':'73FD1267-79BA-4E23-A152-744AF73117E9','ParentId':null,'EnCode':null,'Name':'系统安全','Icon':'fa fa-desktop','UrlAddress':null,'OpenTarget':'expand','IsMenu':false,'IsExpand':true,'IsPublic':false,'SortCode':3,'Description':null,'CreationTime':'2016-10-20T12:34:58.943','CreateUserId':null,'LastModifyTime':'2016-07-22T15:46:56.033','LastModifyUserId':'9f2ec079-7d0f-4fe2-90ab-8b09a8302aba','IsEnabled':true,'IsDeleted':false,'DeleteTime':null,'DeleteUserId':null,'ChildNodes':[]}]";
            #endregion
            return menu;
            //return this.ToMenuJson(menuList, null);
        }


        //string ToMenuJson(List<T> data, string parentId)
        //{
        //    StringBuilder sbJson = new StringBuilder();
        //    sbJson.Append("[");
        //    List<T> entities = data.FindAll(t => t.ParentId == parentId);
        //    if (entities.Count > 0)
        //    {
        //        foreach (var item in entities)
        //        {
        //            string strJson = JsonHelper.Serialize(item);
        //            strJson = strJson.Insert(strJson.Length - 1, ",\"ChildNodes\":" + ToMenuJson(data, item.Id) + "");
        //            sbJson.Append(strJson + ",");
        //        }
        //        sbJson = sbJson.Remove(sbJson.Length - 1, 1);
        //    }
        //    sbJson.Append("]");
        //    return sbJson.ToString();
        //}





    }
}

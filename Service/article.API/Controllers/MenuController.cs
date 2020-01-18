using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using article.API.Data;
using article.API.Model;
using article.API.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace article.API.Controllers
{
    [Authorize]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly ArticleContext _articleContext;
        public MenuController(ArticleContext context)
        {
            _articleContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet]
        [Route("api/articles/menus/all")]
        [ProducesResponseType(typeof(List<MenuItem>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<List<MenuItem>>> GetAllMenusAsync()
        {
            var campaignList = await _articleContext.MenuItems.ToListAsync();

            if (campaignList is null)
            {
                return Ok();
            }
            return campaignList;
        }



        [Route("api/articles/menus/{menuIds:string}")]
        private async Task<List<MenuItem>> GetItemsByIdsAsync(string menuIds)
        {
            var numIds = menuIds.Split(',');

            if (numIds.Length<=0)
            {
                return new List<MenuItem>();
            }

            var items = await _articleContext.MenuItems.Where(ci => menuIds.Contains(ci.Id)).ToListAsync();

            return items;
        }


        [HttpGet]
        [Route("api/articles/menu/{menuId:string}")]
        public async Task<ActionResult<MenuItem>> ItemByIdAsync(string menuId)
        {
            if (string.IsNullOrEmpty(menuId))
            {
                return BadRequest();
            }

            var item = await _articleContext.MenuItems.SingleOrDefaultAsync(ci => ci.Id == menuId&&ci.IsDelete==false);

            if (item != null)
            {
                return item;
            }

            return NotFound();
        }

        [HttpGet]
        [Route("api/articles/classMenus/{Cid:string}")]
        public async Task<ActionResult<List<MenuItem>>> ItemByCidAsync(string Cid)
        {
            var item = await _articleContext.MenuItems.Where(ci => ci.Cid == Cid&&ci.IsDelete==false).OrderBy(a=>a.SortCode).ToListAsync();

            if (item == null)
            {
                return NotFound();
            }
            return item;
        }

        [Route("api/articles/menus")]
        [HttpPost]
        public async Task<ActionResult<MenuItem>> CreateProductAsync([FromBody]MenuItem menu)
        {
            if (menu is null)
            {
                return BadRequest();
            }

            var item = new MenuItem
            {
                Id = menu.Id,
                Cid = menu.Cid,
                Tag = menu.Tag,
                Name = menu.Name,
                IsDelete = menu.IsDelete,
                SortCode = menu.SortCode,
                CreateUser = menu.CreateUser,
                CreateTime = DateTime.Now
            };

            await _articleContext.MenuItems.AddAsync(item);

            await _articleContext.SaveChangesAsync();

            //return CreatedAtAction(nameof(ItemByIdAsync), new { id = item.Id }, null);
            return item;
        }


        [Route("api/articles/menus")]
        [HttpPut]
        public async Task<ActionResult> UpdateProductAsync([FromBody]MenuItem productToUpdate)
        {
            var catalogItem = await _articleContext.MenuItems.SingleOrDefaultAsync(i => i.Id == productToUpdate.Id);

            if (catalogItem == null)
            {
                return NotFound(new { Message = $"Menu with id {productToUpdate.Id} not found." });
            }
            // Update current product
            catalogItem = productToUpdate;
            _articleContext.MenuItems.Update(catalogItem);

            await _articleContext.SaveChangesAsync();

            return CreatedAtAction(nameof(ItemByIdAsync), new { id = productToUpdate.Id }, null);
        }



        [Route("api/articles/menus/{id}")]
        [HttpDelete]
        public async Task<ActionResult> DeleteProductAsync(string id)
        {
            var product = _articleContext.MenuItems.SingleOrDefault(x => x.Id == id);

            if (product == null)
            {
                return NotFound(new { Message = $"Menu with id {product.Id} not found." });
            }

            //_articleContext.ArticleItems.Remove(product);
            product.IsDelete = true;
            _articleContext.MenuItems.Update(product);

            await _articleContext.SaveChangesAsync();

            return NoContent();
        }

    }
}
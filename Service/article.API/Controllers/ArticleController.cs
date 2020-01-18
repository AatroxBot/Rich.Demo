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
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly ArticleContext _articleContext;
        public ArticleController(ArticleContext context)
        {
            _articleContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet]
        [Route("items")]
        public async Task<IActionResult> ItemsAsync([FromQuery]int pageSize = 10, [FromQuery]int pageIndex = 0, string ids = null)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                var items = await GetItemsByIdsAsync(ids);
                if (!items.Any())
                {
                    return BadRequest("ids value invalid. Must be comma-separated list of numbers");
                }

                return Ok(items);
            }

          //  var totalItems = await _articleContext.ArticleItems
          //.Where(p => p.Checked == true && p.IsDelete == false)
          //.ToListAsync();

            var itemsOnPage = await _articleContext.ArticleItems
                .OrderBy(c => c.Id)
                .Skip(pageSize * (pageIndex - 1))
                .Take(pageSize)
                .ToListAsync();


            ApiReturnModel.PageInfo pageResult = new ApiReturnModel.PageInfo();
            pageResult.page = pageIndex;
            pageResult.pageSize = pageSize;
            //pageResult.pageCount = totalItems.Count;
            return Ok(new ApiReturnModel()
            {
                result = 1,
                data = itemsOnPage,
                message = "查询成功",
                pageInfo = pageResult
            });
        }

        private async Task<List<ArticleItem>> GetItemsByIdsAsync(string ids)
        {
            var numIds = ids.Split(',').Select(id => (Ok: int.TryParse(id, out int x), Value: x));

            if (!numIds.All(nid => nid.Ok))
            {
                return new List<ArticleItem>();
            }

            var idsToSelect = numIds
                .Select(id => id.Value);



            var items = await _articleContext.ArticleItems.Where(ci => idsToSelect.Contains(ci.Id)).ToListAsync();

            return items;
        }


        [HttpGet]
        [Route("items/{id:int}")]
        public async Task<ActionResult<ArticleItem>> ItemByIdAsync(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }


            var item = await _articleContext.ArticleItems.SingleOrDefaultAsync(ci => ci.Id == id);

            if (item != null)
            {
                return item;
            }

            return NotFound();
        }

        [HttpGet]
        [Route("items/withtitle/{title:minlength(10)}")]
        public async Task<ActionResult<PaginatedItemsViewModel<ArticleItem>>> ItemsWithTitleAsync(string title, [FromQuery]int pageSize = 10, [FromQuery]int pageIndex = 0)
        {
            var totalItems = await _articleContext.ArticleItems
                .Where(c => c.Title.StartsWith(title))
                .LongCountAsync();

            var itemsOnPage = await _articleContext.ArticleItems
                .Where(c => c.Title.StartsWith(title))
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();


            return new PaginatedItemsViewModel<ArticleItem>(pageIndex, pageSize, totalItems, itemsOnPage);
        }

        [Route("items")]
        [HttpPut]
        public async Task<ActionResult> UpdateProductAsync([FromBody]ArticleItem productToUpdate)
        {
            var catalogItem = await _articleContext.ArticleItems.SingleOrDefaultAsync(i => i.Id == productToUpdate.Id);

            if (catalogItem == null)
            {
                return NotFound(new { Message = $"Item with id {productToUpdate.Id} not found." });
            }

            var oldContent = catalogItem.Content;
            var raiseProductPriceChangedEvent = oldContent != productToUpdate.Content;

            // Update current product
            //catalogItem = productToUpdate;
            catalogItem.Title = productToUpdate.Title;
            catalogItem.Tag = productToUpdate.Tag;
            catalogItem.Content = productToUpdate.Content;
            catalogItem.UpdateUser = productToUpdate.UpdateUser;
            catalogItem.UpdateTime = productToUpdate.UpdateTime;

            _articleContext.ArticleItems.Update(catalogItem);

            await _articleContext.SaveChangesAsync();

            if (raiseProductPriceChangedEvent) // Save product's data and publish integration event through the Event Bus if price has changed
            {
                //Create Integration Event to be published through the Event Bus
                //var priceChangedEvent = new ProductPriceChangedIntegrationEvent(catalogItem.Id, productToUpdate.Price, oldPrice);

                // Achieving atomicity between original Catalog database operation and the IntegrationEventLog thanks to a local transaction
                //await _catalogIntegrationEventService.SaveEventAndCatalogContextChangesAsync(priceChangedEvent);

                // Publish through the Event Bus and mark the saved event as published
                //await _catalogIntegrationEventService.PublishThroughEventBusAsync(priceChangedEvent);
            }
            else // Just save the updated product because the Product's Price hasn't changed.
            {

            }


            return CreatedAtAction(nameof(ItemByIdAsync), new { id = productToUpdate.Id }, null);
        }

        //POST api/v1/[controller]/items
        [Route("items")]
        [HttpPost]
        public async Task<ActionResult<int>> CreateProductAsync([FromBody]ArticleItem product)
        {
            var item = new ArticleItem
            {
                Pid = product.Pid,
                Title = product.Title,
                Tag = product.Tag,
                Content = product.Content,
                ImagePath = product.ImagePath,
                Checked = product.Checked,
                CreateUser = product.CreateUser,
                CreateTime = DateTime.Now
            };

            await _articleContext.ArticleItems.AddAsync(item);

            await _articleContext.SaveChangesAsync();

            //return CreatedAtAction(nameof(ItemByIdAsync), new { id = item.Id }, null);
            return item.Id;
        }

        //DELETE api/v1/[controller]/id
        [Route("{id}")]
        [HttpDelete]
        public async Task<ActionResult> DeleteProductAsync(int id)
        {
            var product = _articleContext.ArticleItems.SingleOrDefault(x => x.Id == id);

            if (product == null)
            {
                return NotFound(new { Message = $"Item with id {product.Id} not found." });
            }

            //_articleContext.ArticleItems.Remove(product);
            product.IsDelete = true;
            _articleContext.ArticleItems.Update(product);

            await _articleContext.SaveChangesAsync();

            return NoContent();
        }


        [HttpGet]
        [Route("items/title/{name:minlength(1)}")]
        [ProducesResponseType(typeof(PaginatedItemsViewModel<ArticleItem>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PaginatedItemsViewModel<ArticleItem>>> ItemsWithNameAsync(string name, [FromQuery]int pageSize = 10, [FromQuery]int pageIndex = 0)
        {
            var totalItems = await _articleContext.ArticleItems
                .Where(c => c.Title.StartsWith(name))
                .LongCountAsync();

            var itemsOnPage = await _articleContext.ArticleItems
                .Where(c => c.Title.StartsWith(name))
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedItemsViewModel<ArticleItem>(pageIndex, pageSize, totalItems, itemsOnPage);
        }
    }
}
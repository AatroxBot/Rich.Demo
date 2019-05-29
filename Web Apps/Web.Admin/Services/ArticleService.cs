using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Ace;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Web.Admin.Common;
using Web.Admin.Infrastructure;
using Web.Admin.ViewModels;

namespace Web.Admin.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IOptions<AppSettings> _settings;
        private readonly HttpClient _httpClient;
        private readonly ILogger<ArticleService> _logger;
        private readonly string _remoteServiceBaseUrl;
        public ArticleService(IOptions<AppSettings> settings, HttpClient httpClient, ILogger<ArticleService> logger)
        {
            _settings = settings;
            _httpClient = httpClient;
            _logger = logger;

            _remoteServiceBaseUrl = $"{_settings.Value.ArticleUrl}/api/article";
        }

        public async Task<PagedData<ArticleModel>> GetAllArticles(int pageSize, int pageIndex)
        {
            var uri = API.Article.GetAllArticles(_remoteServiceBaseUrl, pageIndex, pageSize);

            var responseString = await _httpClient.GetStringAsync(uri);

            var response = JsonConvert.DeserializeObject<ApiReturnModel>(responseString);

            List<ArticleModel> actionPageList = JsonConvert.DeserializeObject<List<ArticleModel>>(response.data.ToString());
            //返回前端数据源
            PagedData<ArticleModel> pagedData = new PagedData<ArticleModel>(actionPageList, response.pageInfo.pageCount,
                response.pageInfo.page, response.pageInfo.pageSize);
            return pagedData;
        }

        public async Task<ArticleModel> ItemByIdAsync(int id)
        {

            var uri = API.Article.GetArticleById(_remoteServiceBaseUrl, id);

            var responseString = await _httpClient.GetStringAsync(uri);

            var response = JsonConvert.DeserializeObject<ArticleModel>(responseString);

            return response;
        }

        public async Task<string> Add(ArticleModel input)
        {
            var uri = API.Article.AddItem(_remoteServiceBaseUrl);

            var Content = new StringContent(JsonConvert.SerializeObject(input), System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(uri, Content);

            if (response.IsSuccessStatusCode)
            {
                var str = await response.Content.ReadAsStringAsync();

                return str;
            }
            return "0";
        }

        public async Task Delete(int id)
        {
            var uri = API.Article.DeleteItem(_remoteServiceBaseUrl,id);
            var response = await _httpClient.DeleteAsync(uri);

        }

        public async Task Update(ArticleModel input)
        {
            var uri = API.Article.UpdateItem(_remoteServiceBaseUrl);

            var Content = new StringContent(JsonConvert.SerializeObject(input), System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(uri, Content);

            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
        }
    }
}

using article.API.Extensions;
using article.API.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace article.API.Data
{
    public class ArticleContextSeed
    {
        public async Task SeedAsync(ArticleContext context, IHostingEnvironment env, IOptions<ArticleSettings> settings, ILogger<ArticleContextSeed> logger)
        {
            var policy = CreatePolicy(logger, nameof(ArticleContextSeed));

            await policy.ExecuteAsync(async () =>
            {
                var useCustomizationData = settings.Value.UseCustomizationData;
                var contentRootPath = env.ContentRootPath;
                var picturePath = env.WebRootPath;
                

                if (!context.ArticleItems.Any())
                {
                    await context.ArticleItems.AddRangeAsync(useCustomizationData
                        ? GetCatalogItemsFromFile(contentRootPath, context, logger)
                        : GetPreconfiguredItems());

                    await context.SaveChangesAsync();

                    GetCatalogItemPictures(contentRootPath, picturePath);
                }
            });
        }

        private Policy CreatePolicy(ILogger<ArticleContextSeed> logger, string prefix, int retries = 3)
        {
            return Policy.Handle<SqlException>().
                WaitAndRetryAsync(
                    retryCount: retries,
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        logger.LogWarning(exception, "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}", prefix, exception.GetType().Name, exception.Message, retry, retries);
                    }
                );
        }


        private IEnumerable<ArticleItem> GetCatalogItemsFromFile(string contentRootPath, ArticleContext context, ILogger<ArticleContextSeed> logger)
        {
            string csvFileCatalogItems = Path.Combine(contentRootPath, "Setup", "ArticleItems.csv");

            if (!File.Exists(csvFileCatalogItems))
            {
                return GetPreconfiguredItems();
            }

            string[] csvheaders;
            try
            {
                string[] requiredHeaders = { "Id", "Pid", "Title", "Content", "ImagePath", "Checked" };
                string[] optionalheaders = { "availablestock" };
                csvheaders = GetHeaders(csvFileCatalogItems, requiredHeaders, optionalheaders);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message);
                return GetPreconfiguredItems();
            }


            return File.ReadAllLines(csvFileCatalogItems)
                        .Skip(1) // skip header row
                        .Select(row => Regex.Split(row, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)"))
                        .SelectTry(column => CreateCatalogItem(column, csvheaders))
                        .OnCaughtException(ex => { logger.LogError(ex, "EXCEPTION ERROR: {Message}", ex.Message); return null; })
                        .Where(x => x != null);
        }

        private IEnumerable<ArticleItem> GetPreconfiguredItems()
        {
            return new List<ArticleItem>()
            {
                 new ArticleItem { Id = 1, Pid = 0, Title = "First Catelog", Content = ".NET Black & White Mug", ImagePath = "1.png",Checked=true },
                 new ArticleItem { Id = 2, Pid = 1, Title = "Next Catelog", Content = ".NET Bot Black Hoodie",  ImagePath = "2.png",Checked=true  },
            };
        }

        private ArticleItem CreateCatalogItem(string[] column, string[] headers)
        {
            if (column.Count() != headers.Count())
            {
                throw new Exception($"column count '{column.Count()}' not the same as headers count'{headers.Count()}'");
            }

            string catalogTypeName = column[Array.IndexOf(headers, "catalogtypename")].Trim('"').Trim();

            string priceString = column[Array.IndexOf(headers, "price")].Trim('"').Trim();
            if (!Decimal.TryParse(priceString, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out Decimal price))
            {
                throw new Exception($"price={priceString}is not a valid decimal number");
            }

            var articleItem = new ArticleItem()
            {
                Id = Convert.ToInt32(column[Array.IndexOf(headers, "Id")].Trim('"').Trim()),
                Pid = Convert.ToInt32(column[Array.IndexOf(headers, "Pid")].Trim('"').Trim()),
                Title = column[Array.IndexOf(headers, "Title")].Trim('"').Trim(),
                Content = column[Array.IndexOf(headers, "Content")].Trim('"').Trim(),
                ImagePath = column[Array.IndexOf(headers, "ImagePath")].Trim('"').Trim(),
                Checked = Convert.ToBoolean(column[Array.IndexOf(headers, "Checked")].Trim('"').Trim()),
            };

            return articleItem;
        }

        private string[] GetHeaders(string csvfile, string[] requiredHeaders, string[] optionalHeaders = null)
        {
            string[] csvheaders = File.ReadLines(csvfile).First().ToLowerInvariant().Split(',');

            if (csvheaders.Count() < requiredHeaders.Count())
            {
                throw new Exception($"requiredHeader count '{ requiredHeaders.Count()}' is bigger then csv header count '{csvheaders.Count()}' ");
            }

            if (optionalHeaders != null)
            {
                if (csvheaders.Count() > (requiredHeaders.Count() + optionalHeaders.Count()))
                {
                    throw new Exception($"csv header count '{csvheaders.Count()}'  is larger then required '{requiredHeaders.Count()}' and optional '{optionalHeaders.Count()}' headers count");
                }
            }

            foreach (var requiredHeader in requiredHeaders)
            {
                if (!csvheaders.Contains(requiredHeader))
                {
                    throw new Exception($"does not contain required header '{requiredHeader}'");
                }
            }

            return csvheaders;
        }


        private void GetCatalogItemPictures(string contentRootPath, string picturePath)
        {
            if (picturePath != null)
            {
                DirectoryInfo directory = new DirectoryInfo(picturePath);
                foreach (FileInfo file in directory.GetFiles())
                {
                    file.Delete();
                }

                string zipFileCatalogItemPictures = Path.Combine(contentRootPath, "Setup", "ArticleItems.zip");
                ZipFile.ExtractToDirectory(zipFileCatalogItemPictures, picturePath);
            }
        }
    }
}

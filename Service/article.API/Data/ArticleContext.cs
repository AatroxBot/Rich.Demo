using article.API.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace article.API.Data
{
    public class ArticleContext : DbContext
    {
        public ArticleContext(DbContextOptions<ArticleContext> options) : base(options)
        {
            
        }
        public DbSet<ArticleItem> ArticleItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new ArticleItemEntityTypeConfiguration());
        }

        public class ArticleContextDesignFactory : IDesignTimeDbContextFactory<ArticleContext>
        {
            public ArticleContext CreateDbContext(string[] args)
            {
                var optionsBuilder = new DbContextOptionsBuilder<ArticleContext>()
                    .UseSqlServer("Server=wms.test.db.richtj.com;Database=ArticleDb;User Id=sa;Password=Rich@123;");

                return new ArticleContext(optionsBuilder.Options);
            }
        }


    }
}

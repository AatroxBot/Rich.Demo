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

        public DbSet<MenuItem> MenuItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new ArticleItemEntityTypeConfiguration());
        }

        public class ArticleContextDesignFactory : IDesignTimeDbContextFactory<ArticleContext>
        {
            public ArticleContext CreateDbContext(string[] args)
            {
                var optionsBuilder = new DbContextOptionsBuilder<ArticleContext>()
                    .UseSqlServer("Server=localhost; Database=ArticleDb; Trusted_Connection=True;");

                return new ArticleContext(optionsBuilder.Options);
            }
        }


    }
}

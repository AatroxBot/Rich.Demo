using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using article.API.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace article.API.Data
{
    public class ArticleItemEntityTypeConfiguration : IEntityTypeConfiguration<ArticleItem>
    {
        public void Configure(EntityTypeBuilder<ArticleItem> builder)
        {
            builder.ToTable("Article");

            builder.HasKey(ci => ci.Id);

            builder.Property(ci => ci.Pid)
                .IsRequired(true)
                .HasMaxLength(50);

            builder.Property(ci => ci.Title)
                .IsRequired(true);

            builder.Property(ci => ci.Tag)
              .IsRequired(false);

            builder.Property(ci => ci.Content)
                .IsRequired(false)
                .HasMaxLength(500);

            builder.Property(ci => ci.ImagePath)
                .IsRequired(false);

            builder.Property(ci => ci.Checked)
                .IsRequired(true);

            builder.Property(ci => ci.CreateUser)
           .IsRequired(true);

            builder.Property(ci => ci.CreateTime)
         .IsRequired(true);

            builder.Property(ci => ci.UpdateUser);

            builder.Property(ci => ci.UpdateTime);



            builder.Property(ci => ci.IsDelete).IsRequired(true);
        }
    }
}

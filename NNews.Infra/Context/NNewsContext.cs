using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace NNews.Infra.Context;

public partial class NNewsContext : DbContext
{
    public NNewsContext()
    {
    }

    public NNewsContext(DbContextOptions<NNewsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Article> Articles { get; set; }

    public virtual DbSet<ArticleRole> ArticleRoles { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasKey(e => e.ArticleId).HasName("articles_pkey");

            entity.ToTable("articles");

            entity.Property(e => e.ArticleId)
                .HasDefaultValueSql("nextval('article_id_seq'::regclass)")
                .HasColumnName("article_id");
            entity.Property(e => e.AuthorId).HasColumnName("author_id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DateAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date_at");
            entity.Property(e => e.Status)
                .HasDefaultValue(0)
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Category).WithMany(p => p.Articles)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_article_category");

            entity.HasMany(d => d.Tags).WithMany(p => p.Articles)
                .UsingEntity<Dictionary<string, object>>(
                    "ArticleTag",
                    r => r.HasOne<Tag>().WithMany()
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_article_tag_tag"),
                    l => l.HasOne<Article>().WithMany()
                        .HasForeignKey("ArticleId")
                        .HasConstraintName("fk_article_tag_article"),
                    j =>
                    {
                        j.HasKey("ArticleId", "TagId").HasName("article_tags_pkey");
                        j.ToTable("article_tags");
                        j.IndexerProperty<long>("ArticleId").HasColumnName("article_id");
                        j.IndexerProperty<long>("TagId").HasColumnName("tag_id");
                    });
        });

        modelBuilder.Entity<ArticleRole>(entity =>
        {
            entity.HasKey(e => new { e.ArticleId, e.Slug }).HasName("article_roles_pkey");

            entity.ToTable("article_roles");

            entity.Property(e => e.ArticleId).HasColumnName("article_id");
            entity.Property(e => e.Slug)
                .HasMaxLength(80)
                .HasColumnName("slug");
            entity.Property(e => e.Name)
                .HasMaxLength(80)
                .HasColumnName("name");

            entity.HasOne(d => d.Article).WithMany(p => p.ArticleRoles)
                .HasForeignKey(d => d.ArticleId)
                .HasConstraintName("fk_article_role_article");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("categories_pkey");

            entity.ToTable("categories");

            entity.Property(e => e.CategoryId)
                .HasDefaultValueSql("nextval('category_id_seq'::regclass)")
                .HasColumnName("category_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.ParentId).HasColumnName("parent_id");
            entity.Property(e => e.Title)
                .HasMaxLength(240)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("fk_category_parent");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.TagId).HasName("tags_pkey");

            entity.ToTable("tags");

            entity.Property(e => e.TagId)
                .HasDefaultValueSql("nextval('tag_id_seq'::regclass)")
                .HasColumnName("tag_id");
            entity.Property(e => e.Slug)
                .HasMaxLength(120)
                .HasColumnName("slug");
            entity.Property(e => e.Title)
                .HasMaxLength(120)
                .HasColumnName("title");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

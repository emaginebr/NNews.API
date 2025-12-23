using System;
using System.Collections.Generic;

namespace NNews.Infra.Context;

public partial class Article
{
    public long ArticleId { get; set; }

    public long CategoryId { get; set; }

    public long? AuthorId { get; set; }

    public DateTime DateAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public int Status { get; set; }

    public virtual ICollection<ArticleRole> ArticleRoles { get; set; } = new List<ArticleRole>();

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}

using System;
using System.Collections.Generic;

namespace NNews.Infra.Context;

public partial class Category
{
    public long CategoryId { get; set; }

    public long? ParentId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();

    public virtual ICollection<Category> InverseParent { get; set; } = new List<Category>();

    public virtual Category? Parent { get; set; }
}

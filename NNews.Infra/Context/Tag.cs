using System;
using System.Collections.Generic;

namespace NNews.Infra.Context;

public partial class Tag
{
    public long TagId { get; set; }

    public string Slug { get; set; } = null!;

    public string Title { get; set; } = null!;

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();
}

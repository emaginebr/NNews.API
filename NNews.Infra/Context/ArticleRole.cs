using System;
using System.Collections.Generic;

namespace NNews.Infra.Context;

public partial class ArticleRole
{
    public long ArticleId { get; set; }

    public string Slug { get; set; } = null!;

    public string Name { get; set; } = null!;

    public virtual Article Article { get; set; } = null!;
}

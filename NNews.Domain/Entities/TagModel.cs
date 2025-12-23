using NNews.Domain.Entities.Interfaces;
using System.Text.RegularExpressions;

namespace NNews.Domain.Entities
{
    public class TagModel : ITagModel
    {

        public long TagId { get; private set; }
        public string Slug { get; private set; }
        public string Title { get; private set; }
        public int ArticleCount { get; private set; }

        private TagModel()
        {
            Slug = string.Empty;
            Title = string.Empty;
            ArticleCount = 0;
        }

        public TagModel(string title, string? slug = null) : this()
        {
            SetTitle(title);
            SetSlug(slug ?? GenerateSlug(title));
        }

        public static TagModel Create(string title, string? slug = null)
        {
            return new TagModel(title, slug);
        }

        public static TagModel Reconstruct(long tagId, string title, string slug)
        {
            var tag = new TagModel
            {
                TagId = tagId,
                Title = title,
                Slug = slug
            };

            return tag;
        }

        public static TagModel Reconstruct(long tagId, string title, string slug, int articleCount)
        {
            var tag = new TagModel
            {
                TagId = tagId,
                Title = title,
                Slug = slug,
                ArticleCount = articleCount
            };

            return tag;
        }

        public void UpdateTitle(string title)
        {
            SetTitle(title);
        }

        public void UpdateSlug(string slug)
        {
            SetSlug(slug);
        }

        public void Update(string title, string slug)
        {
            SetTitle(title);
            SetSlug(slug);
        }

        private void SetTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be null or empty.", nameof(title));

            if (title.Length > 120)
                throw new ArgumentException("Title cannot exceed 120 characters.", nameof(title));

            Title = title.Trim();
        }

        private void SetSlug(string slug)
        {
            if (slug == null) { 
                slug = string.Empty;
            }

            if (slug.Length > 120)
                throw new ArgumentException("Slug cannot exceed 120 characters.", nameof(slug));

            if (!string.IsNullOrWhiteSpace(slug) && !IsValidSlug(slug))
                throw new ArgumentException("Slug must contain only lowercase letters, numbers, and hyphens.", nameof(slug));

            Slug = slug.Trim().ToLowerInvariant();
        }

        private bool IsValidSlug(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
                return false;

            return Regex.IsMatch(slug, @"^[a-z0-9]+(?:-[a-z0-9]+)*$");
        }

        private static string GenerateSlug(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Cannot generate slug from empty title.", nameof(title));

            var slug = title.ToLowerInvariant().Trim();

            slug = Regex.Replace(slug, @"[áàâãä]", "a");
            slug = Regex.Replace(slug, @"[éèêë]", "e");
            slug = Regex.Replace(slug, @"[íìîï]", "i");
            slug = Regex.Replace(slug, @"[óòôõö]", "o");
            slug = Regex.Replace(slug, @"[úùûü]", "u");
            slug = Regex.Replace(slug, @"[ç]", "c");
            slug = Regex.Replace(slug, @"[ñ]", "n");

            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
            slug = Regex.Replace(slug, @"\s+", "-");
            slug = Regex.Replace(slug, @"-+", "-");
            slug = slug.Trim('-');

            if (slug.Length > 120)
                slug = slug.Substring(0, 120).TrimEnd('-');

            return slug;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not TagModel other)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (TagId == 0 || other.TagId == 0)
                return false;

            return TagId == other.TagId;
        }

        public override int GetHashCode()
        {
            return TagId.GetHashCode();
        }

        public override string ToString()
        {
            return $"Tag: {Title} ({Slug})";
        }
    }
}

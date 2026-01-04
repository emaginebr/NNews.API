using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NNews.Domain.Entities;
using NNews.Domain.Entities.Interfaces;
using NNews.Infra.Context;
using NNews.Infra.Interfaces.Repository;

namespace NNews.Infra.Repository
{
    public class TagRepository : ITagRepository<ITagModel>
    {
        private readonly NNewsContext _context;
        private readonly IMapper _mapper;

        public TagRepository(NNewsContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public IEnumerable<ITagModel> ListAll()
        {
            var tags = _context.Tags
                .Include(t => t.Articles)
                .AsNoTracking()
                .OrderBy(t => t.Title)
                .ToList();

            return _mapper.Map<IEnumerable<TagModel>>(tags);
        }

        public IEnumerable<ITagModel> ListByRoles(IList<string>? roles)
        {
            var query = _context.Tags
                .Include(t => t.Articles)
                    .ThenInclude(a => a.ArticleRoles)
                .AsNoTracking()
                .Where(t => t.Articles.Any(a => a.Status == 1));

            if (roles != null && roles.Any())
            {
                query = query.Where(t => t.Articles.Any(a => 
                    a.Status == 1 && 
                    (a.ArticleRoles.Any(ar => roles.Contains(ar.Slug)) || !a.ArticleRoles.Any())
                ));
            }
            else
            {
                query = query.Where(t => t.Articles.Any(a => 
                    a.Status == 1 && !a.ArticleRoles.Any()
                ));
            }

            var tags = query
                .Select(t => new 
                {
                    Tag = t,
                    ArticleCount = t.Articles.Count(a => 
                        a.Status == 1 && 
                        (roles == null || !roles.Any() ? 
                            !a.ArticleRoles.Any() : 
                            (a.ArticleRoles.Any(ar => roles.Contains(ar.Slug)) || !a.ArticleRoles.Any())
                        )
                    )
                })
                .OrderBy(x => x.Tag.Title)
                .ToList();

            return tags.Select(x => TagModel.Reconstruct(x.Tag.TagId, x.Tag.Title, x.Tag.Slug, x.ArticleCount));
        }

        public ITagModel GetById(int id)
        {
            var tag = _context.Tags
                .Include(t => t.Articles)
                .AsNoTracking()
                .FirstOrDefault(t => t.TagId == id);

            if (tag == null)
                throw new KeyNotFoundException($"Tag with ID {id} not found.");

            return _mapper.Map<TagModel>(tag);
        }

        public ITagModel? GetBySlug(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug)) {
                return null;
            }

            var tag = _context.Tags
                .Include(t => t.Articles)
                .AsNoTracking()
                .FirstOrDefault(t => t.Slug == slug);

            if (tag == null)
                return null;

            return _mapper.Map<TagModel>(tag);
        }

        public bool ExistSlug(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
                return false;

            return _context.Tags
                .AsNoTracking()
                .Any(t => t.Slug == slug);
        }

        public bool ExistsByTitle(string title, long? excludeTagId = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                return false;

            var query = _context.Tags.AsNoTracking();

            if (excludeTagId.HasValue)
                query = query.Where(t => t.TagId != excludeTagId.Value);

            return query.Any(t => t.Title.ToLower() == title.ToLower());
        }

        public ITagModel Insert(ITagModel tagModel)
        {
            if (tagModel == null)
                throw new ArgumentNullException(nameof(tagModel));

            var tag = _mapper.Map<Tag>(tagModel);

            _context.Tags.Add(tag);
            _context.SaveChanges();

            return _mapper.Map<TagModel>(tag);
        }

        public ITagModel Update(ITagModel tagModel)
        {
            if (tagModel == null)
                throw new ArgumentNullException(nameof(tagModel));

            var existingTag = _context.Tags
                .FirstOrDefault(t => t.TagId == tagModel.TagId);

            if (existingTag == null)
                throw new KeyNotFoundException($"Tag with ID {tagModel.TagId} not found.");

            existingTag.Title = tagModel.Title;
            existingTag.Slug = tagModel.Slug;

            _context.Tags.Update(existingTag);
            _context.SaveChanges();

            return _mapper.Map<TagModel>(existingTag);
        }

        public void Delete(int id)
        {
            var tag = _context.Tags
                .FirstOrDefault(t => t.TagId == id);

            if (tag == null)
                throw new KeyNotFoundException($"Tag with ID {id} not found.");

            _context.Tags.Remove(tag);
            _context.SaveChanges();
        }

        public void MergeTags(long sourceTagId, long targetTagId)
        {
            if (sourceTagId == targetTagId)
                throw new ArgumentException("Source and target tags cannot be the same.");

            var sourceTag = _context.Tags
                .Include(t => t.Articles)
                .FirstOrDefault(t => t.TagId == sourceTagId);

            if (sourceTag == null)
                throw new KeyNotFoundException($"Source tag with ID {sourceTagId} not found.");

            var targetTag = _context.Tags
                .Include(t => t.Articles)
                .FirstOrDefault(t => t.TagId == targetTagId);

            if (targetTag == null)
                throw new KeyNotFoundException($"Target tag with ID {targetTagId} not found.");

            foreach (var article in sourceTag.Articles.ToList())
            {
                if (!targetTag.Articles.Any(a => a.ArticleId == article.ArticleId))
                {
                    targetTag.Articles.Add(article);
                }
                sourceTag.Articles.Remove(article);
            }

            _context.Tags.Remove(sourceTag);
            _context.SaveChanges();
        }
    }
}

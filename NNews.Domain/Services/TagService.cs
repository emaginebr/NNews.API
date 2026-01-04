using AutoMapper;
using NNews.Domain.Entities;
using NNews.Domain.Entities.Interfaces;
using NNews.Domain.Services.Interfaces;
using NNews.DTO;
using NNews.Infra.Interfaces.Repository;
using NTools.ACL.Interfaces;

namespace NNews.Domain.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository<ITagModel> _tagRepository;
        private readonly IMapper _mapper;
        private readonly IStringClient _stringClient;

        public TagService(ITagRepository<ITagModel> tagRepository, IMapper mapper, IStringClient stringClient)
        {
            _tagRepository = tagRepository ?? throw new ArgumentNullException(nameof(tagRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _stringClient = stringClient ?? throw new ArgumentNullException(nameof(stringClient));
        }

        private async Task<string> GenerateSlug(ITagModel md)
        {
            ITagModel? newTag = null;
            string newSlug;
            int c = 0;
            do
            {
                newSlug = await _stringClient.GenerateSlugAsync(md.Title);
                if (c > 0)
                {
                    newSlug += c.ToString();
                }
                c++;
                newTag = _tagRepository.GetBySlug(newSlug);
            } while (newTag != null && newTag.TagId != md.TagId);
            return newSlug;
        }

        public IList<TagInfo> ListAll()
        {
            var tags = _tagRepository.ListAll();
            return _mapper.Map<IList<TagInfo>>(tags);
        }

        public IList<TagInfo> ListByRoles(IList<string>? roles)
        {
            var tags = _tagRepository.ListByRoles(roles);
            return _mapper.Map<IList<TagInfo>>(tags);
        }

        public TagInfo GetById(int tagId)
        {
            var tag = _tagRepository.GetById(tagId);
            return _mapper.Map<TagInfo>(tag);
        }

        public async Task<TagInfo> InsertAsync(TagInfo tag)
        {
            if (tag == null)
                throw new ArgumentNullException(nameof(tag));

            if (string.IsNullOrWhiteSpace(tag.Title))
                throw new ArgumentException("Tag title cannot be empty.", nameof(tag));

            if (_tagRepository.ExistsByTitle(tag.Title))
                throw new InvalidOperationException($"A tag with the title '{tag.Title}' already exists.");

            var tagModel = _mapper.Map<TagModel>(tag);

            tagModel.UpdateSlug(await GenerateSlug(tagModel));

            var insertedTag = _tagRepository.Insert(tagModel);
            return _mapper.Map<TagInfo>(insertedTag);
        }

        public async Task<TagInfo> UpdateAsync(TagInfo tag)
        {
            if (tag == null)
                throw new ArgumentNullException(nameof(tag));

            if (string.IsNullOrWhiteSpace(tag.Title))
                throw new ArgumentException("Tag title cannot be empty.", nameof(tag));

            if (_tagRepository.ExistsByTitle(tag.Title, tag.TagId))
                throw new InvalidOperationException($"A tag with the title '{tag.Title}' already exists.");

            var tagModel = _mapper.Map<TagModel>(tag);

            tagModel.UpdateSlug(await GenerateSlug(tagModel));

            var updatedTag = _tagRepository.Update(tagModel);
            return _mapper.Map<TagInfo>(updatedTag);
        }

        public void Delete(int tagId)
        {
            _tagRepository.Delete(tagId);
        }

        public void MergeTags(long sourceTagId, long targetTagId)
        {
            if (sourceTagId <= 0)
                throw new ArgumentException("Source tag ID must be greater than zero.", nameof(sourceTagId));

            if (targetTagId <= 0)
                throw new ArgumentException("Target tag ID must be greater than zero.", nameof(targetTagId));

            if (sourceTagId == targetTagId)
                throw new InvalidOperationException("Source and target tags cannot be the same.");

            _tagRepository.MergeTags(sourceTagId, targetTagId);
        }
    }
}

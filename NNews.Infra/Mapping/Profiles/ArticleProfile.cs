using AutoMapper;
using NNews.Domain.Entities;
using NNews.Domain.Entities.Interfaces;
using NNews.Domain.Enums;
using NNews.Infra.Context;

namespace NNews.Infra.Mapping.Profiles
{
    public class ArticleProfile : Profile
    {
        public ArticleProfile()
        {
            CreateMap<Article, IArticleModel>()
                .ConstructUsing((src, ctx) =>
                {
                    var article = ArticleModel.Reconstruct(
                        src.ArticleId,
                        src.Title,
                        src.Content,
                        src.CategoryId,
                        src.AuthorId,
                        (ArticleStatus)src.Status,
                        src.CreatedAt,
                        src.UpdatedAt
                    );

                    if (src.Category != null)
                    {
                        var categoryModel = ctx.Mapper.Map<CategoryModel>(src.Category);
                        article.SetCategory(categoryModel);
                    }

                    if (src.Tags != null && src.Tags.Any())
                    {
                        foreach (var tag in src.Tags)
                        {
                            var tagModel = ctx.Mapper.Map<TagModel>(tag);
                            article.AddTag(tagModel);
                        }
                    }

                    if (src.ArticleRoles != null && src.ArticleRoles.Any())
                    {
                        foreach (var role in src.ArticleRoles)
                        {
                            var roleModel = new RoleModel(role.Slug, role.Name);
                            article.AddRole(roleModel);
                        }
                    }

                    return article;
                })
                .ForAllMembers(opt => opt.Ignore());

            CreateMap<IArticleModel, Article>()
                .ForMember(dest => dest.ArticleId, opt => opt.MapFrom(src => src.ArticleId))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => src.AuthorId))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.Tags, opt => opt.Ignore())
                .ForMember(dest => dest.ArticleRoles, opt => opt.Ignore());
        }
    }
}

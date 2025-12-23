using AutoMapper;
using NNews.Domain.Entities;
using NNews.Domain.Entities.Interfaces;
using NNews.Domain.Enums;
using NNews.Dtos;

namespace NNews.Infra.Mapping.Profiles
{
    public class ArticleDtoProfile : Profile
    {
        public ArticleDtoProfile()
        {
            CreateMap<ArticleModel, ArticleInfo>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags))
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles));

            CreateMap<IArticleModel, ArticleInfo>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags))
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles));

            CreateMap<ArticleInfo, ArticleModel>()
                .ConstructUsing((src, ctx) =>
                {
                    ArticleModel article;

                    if (src.ArticleId > 0)
                    {
                        article = ArticleModel.Reconstruct(
                            src.ArticleId,
                            src.Title,
                            src.Content,
                            src.CategoryId,
                            src.AuthorId,
                            (ArticleStatus)src.Status,
                            src.CreatedAt,
                            src.UpdatedAt
                        );
                    }
                    else
                    {
                        article = ArticleModel.Create(
                            src.Title,
                            src.Content,
                            src.CategoryId,
                            src.AuthorId,
                            (ArticleStatus)src.Status
                        );
                    }

                    if (src.Tags != null && src.Tags.Any())
                    {
                        foreach (var tagInfo in src.Tags)
                        {
                            var tagModel = ctx.Mapper.Map<TagModel>(tagInfo);
                            article.AddTag(tagModel);
                        }
                    }

                    if (src.Roles != null && src.Roles.Any())
                    {
                        foreach (var roleInfo in src.Roles)
                        {
                            var roleModel = ctx.Mapper.Map<RoleModel>(roleInfo);
                            article.AddRole(roleModel);
                        }
                    }

                    return article;
                })
                .ForAllMembers(opt => opt.Ignore());
        }
    }
}

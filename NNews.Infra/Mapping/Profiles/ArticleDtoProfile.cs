using AutoMapper;
using NNews.Domain.Entities;
using NNews.Domain.Entities.Interfaces;
using NNews.Domain.Enums;
using NNews.DTO;

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
                            src.DateAt,
                            src.CreatedAt,
                            src.UpdatedAt,
                            src.ImageName
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
                        
                        if (!string.IsNullOrEmpty(src.ImageName))
                        {
                            article.UpdateImageName(src.ImageName);
                        }
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

            CreateMap<ArticleInsertedInfo, ArticleModel>()
                .ConstructUsing((src, ctx) =>
                {
                    var article = ArticleModel.Create(
                        src.Title,
                        src.Content,
                        src.CategoryId,
                        src.AuthorId,
                        (ArticleStatus)src.Status
                    );
                    
                    if (!string.IsNullOrEmpty(src.ImageName))
                    {
                        article.UpdateImageName(src.ImageName);
                    }

                    return article;
                })
                .ForAllMembers(opt => opt.Ignore());

            CreateMap<ArticleUpdatedInfo, ArticleModel>()
                .ConstructUsing((src, ctx) =>
                {
                    var article = ArticleModel.Reconstruct(
                        src.ArticleId,
                        src.Title,
                        src.Content,
                        src.CategoryId,
                        src.AuthorId,
                        (ArticleStatus)src.Status,
                        src.DateAt,
                        DateTime.UtcNow,
                        DateTime.UtcNow,
                        src.ImageName
                    );

                    return article;
                })
                .ForAllMembers(opt => opt.Ignore());
        }
    }
}

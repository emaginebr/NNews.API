using AutoMapper;
using NNews.Domain.Entities;
using NNews.Domain.Entities.Interfaces;
using NNews.Dtos;

namespace NNews.Infra.Mapping.Profiles
{
    public class CategoryDtoProfile : Profile
    {
        public CategoryDtoProfile()
        {
            CreateMap<CategoryModel, CategoryInfo>();
            
            CreateMap<ICategoryModel, CategoryInfo>();

            CreateMap<CategoryInfo, CategoryModel>()
                .ConstructUsing(src => src.CategoryId > 0
                    ? CategoryModel.Reconstruct(
                        src.CategoryId,
                        src.Title,
                        src.ParentId,
                        src.CreatedAt,
                        src.UpdatedAt,
                        src.ArticleCount)
                    : CategoryModel.Create(src.Title, src.ParentId));
        }
    }
}

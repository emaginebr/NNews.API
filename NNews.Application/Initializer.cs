using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NAuth.ACL;
using NAuth.ACL.Interfaces;
using NAuth.DTO.Settings;
using NNews.Domain.Entities.Interfaces;
using NNews.Domain.Services;
using NNews.Domain.Services.Interfaces;
using NNews.DTO.Settings;
using NNews.Infra.Context;
using NNews.Infra.Interfaces.Repository;
using NNews.Infra.Mapping.Profiles;
using NNews.Infra.Repository;
using NTools.ACL;
using NTools.ACL.Interfaces;
using NTools.DTO.Settings;

namespace NNews.Application
{
    public static class Initializer
    {
        private static void injectDependency(Type serviceType, Type implementationType, IServiceCollection services, bool scoped = true)
        {
            if (scoped)
                services.AddScoped(serviceType, implementationType);
            else
                services.AddTransient(serviceType, implementationType);
        }

        public static void Configure(IServiceCollection services, string? connection, IConfiguration configuration, bool scoped = true)
        {
            if (scoped)
            {
                services.AddDbContext<NNewsContext>(x =>
                {
                    x.UseLazyLoadingProxies()
                     .UseNpgsql(connection)
                     .EnableSensitiveDataLogging()
                     .EnableDetailedErrors();
                });
            }
            else
            {
                services.AddDbContextFactory<NNewsContext>(x =>
                {
                    x.UseLazyLoadingProxies()
                     .UseNpgsql(connection)
                     .EnableSensitiveDataLogging()
                     .EnableDetailedErrors();
                });
            }

            services.AddLogging();
            services.AddHttpClient();

            services.Configure<NAuthSetting>(configuration.GetSection("NAuth"));
            services.Configure<NToolSetting>(configuration.GetSection("NTools"));
            services.Configure<NNewsSetting>(configuration.GetSection("NNews"));

            injectDependency(typeof(IStringClient), typeof(StringClient), services, scoped);
            injectDependency(typeof(IFileClient), typeof(FileClient), services, scoped);
            injectDependency(typeof(IChatGPTClient), typeof(ChatGPTClient), services, scoped);
            injectDependency(typeof(IUserClient), typeof(UserClient), services, scoped);

            #region Infra
            injectDependency(typeof(NNewsContext), typeof(NNewsContext), services, scoped);
            #endregion

            #region Repository
            injectDependency(typeof(ICategoryRepository<ICategoryModel>), typeof(CategoryRepository), services, scoped);
            injectDependency(typeof(ITagRepository<ITagModel>), typeof(TagRepository), services, scoped);
            injectDependency(typeof(IArticleRepository<IArticleModel>), typeof(ArticleRepository), services, scoped);
            #endregion

            #region AutoMapper
            services.AddAutoMapper(cfg => { }, typeof(CategoryDtoProfile).Assembly);
            services.AddAutoMapper(cfg => { }, typeof(CategoryProfile).Assembly);
            services.AddAutoMapper(cfg => { }, typeof(TagDtoProfile).Assembly);
            services.AddAutoMapper(cfg => { }, typeof(TagProfile).Assembly);
            services.AddAutoMapper(cfg => { }, typeof(ArticleProfile).Assembly);
            services.AddAutoMapper(cfg => { }, typeof(ArticleDtoProfile).Assembly);
            services.AddAutoMapper(cfg => { }, typeof(RoleDtoProfile).Assembly);
            #endregion

            #region Service
            injectDependency(typeof(ICategoryService), typeof(CategoryService), services, scoped);
            injectDependency(typeof(ITagService), typeof(TagService), services, scoped);
            injectDependency(typeof(IArticleService), typeof(ArticleService), services, scoped);
            injectDependency(typeof(IArticleAIService), typeof(ArticleAIService), services, scoped);
            #endregion

            services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, NAuthHandler>("BasicAuthentication", null);
        }
    }
}

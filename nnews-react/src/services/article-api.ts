import type { AxiosInstance } from 'axios';
import type { Article, ArticleInput, ArticleUpdate, PagedResult } from '../types/news';

const NEWS_API_ENDPOINTS = {
  ARTICLES: '/api/article',
  ARTICLES_FILTER: '/api/article/filter',
  ARTICLE_BY_ID: (id: number) => `/api/article/${id}`,
};

export class ArticleAPI {
  constructor(private client: AxiosInstance) {}

  /**
   * List articles with optional filtering by category
   */
  async listArticles(
    categoryId?: number,
    page: number = 1,
    pageSize: number = 10
  ): Promise<PagedResult<Article>> {
    const params: Record<string, any> = { page, pageSize };
    if (categoryId) {
      params.categoryId = categoryId;
    }

    const response = await this.client.get<PagedResult<Article>>(
      NEWS_API_ENDPOINTS.ARTICLES,
      { params }
    );

    return this.transformArticleDates(response.data);
  }

  /**
   * Filter articles by roles and parent category
   */
  async filterArticles(
    roles?: string[],
    parentId?: number,
    page: number = 1,
    pageSize: number = 10
  ): Promise<PagedResult<Article>> {
    const params: Record<string, any> = { page, pageSize };
    
    if (roles && roles.length > 0) {
      params.roles = roles.join(',');
    }
    
    if (parentId !== undefined) {
      params.parentId = parentId;
    }

    const response = await this.client.get<PagedResult<Article>>(
      NEWS_API_ENDPOINTS.ARTICLES_FILTER,
      { params }
    );

    return this.transformArticleDates(response.data);
  }

  /**
   * Get a single article by ID
   */
  async getArticleById(id: number): Promise<Article> {
    const response = await this.client.get<Article>(
      NEWS_API_ENDPOINTS.ARTICLE_BY_ID(id)
    );

    return this.transformArticleDate(response.data);
  }

  /**
   * Create a new article
   */
  async createArticle(article: ArticleInput): Promise<Article> {
    const response = await this.client.post<Article>(
      NEWS_API_ENDPOINTS.ARTICLES,
      article
    );

    return this.transformArticleDate(response.data);
  }

  /**
   * Update an existing article
   */
  async updateArticle(article: ArticleUpdate): Promise<Article> {
    const response = await this.client.put<Article>(
      NEWS_API_ENDPOINTS.ARTICLES,
      article
    );

    return this.transformArticleDate(response.data);
  }

  /**
   * Delete an article
   */
  async deleteArticle(id: number): Promise<void> {
    await this.client.delete(NEWS_API_ENDPOINTS.ARTICLE_BY_ID(id));
  }

  /**
   * Transform date strings to Date objects for a single article
   */
  private transformArticleDate(article: Article): Article {
    return {
      ...article,
      createdAt: article.createdAt ? new Date(article.createdAt) : undefined,
      updatedAt: article.updatedAt ? new Date(article.updatedAt) : undefined,
      category: article.category
        ? {
            ...article.category,
            createdAt: article.category.createdAt ? new Date(article.category.createdAt) : undefined,
            updatedAt: article.category.updatedAt ? new Date(article.category.updatedAt) : undefined,
          }
        : undefined,
    };
  }

  /**
   * Transform date strings to Date objects for paged results
   */
  private transformArticleDates(result: PagedResult<Article>): PagedResult<Article> {
    return {
      ...result,
      items: result.items.map(article => this.transformArticleDate(article)),
    };
  }
}

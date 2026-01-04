import type { AxiosInstance } from 'axios';
import type { Article, ArticleInput, ArticleUpdate, PagedResult, AIArticleRequest } from '../types/news';

const NEWS_API_ENDPOINTS = {
  ARTICLES: '/api/article',
  ARTICLES_FILTER: '/api/article/filter',
  ARTICLE_BY_ID: (id: number) => `/api/article/${id}`,
  IMAGE_UPLOAD: '/api/Image/uploadImage',
  INSERT_WITH_AI: '/api/article/insertWithAI',
  UPDATE_WITH_AI: '/api/article/updateWithAI',
};

export class ArticleAPI {
  constructor(private client: AxiosInstance) {}

  /**
   * Upload an image file
   */
  async uploadImage(file: File): Promise<string> {
    const formData = new FormData();
    formData.append('file', file);

    console.log('[ArticleAPI] uploadImage - Request:', { fileName: file.name, fileSize: file.size });
    
    const response = await this.client.post<string>(
      NEWS_API_ENDPOINTS.IMAGE_UPLOAD,
      formData,
      {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
      }
    );
    
    console.log('[ArticleAPI] uploadImage - Response:', response.data);
    return response.data;
  }

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
    console.log('[ArticleAPI] listArticles - Request:', { url: NEWS_API_ENDPOINTS.ARTICLES, params });

    const response = await this.client.get<PagedResult<Article>>(
      NEWS_API_ENDPOINTS.ARTICLES,
      { params }
    );
    console.log('[ArticleAPI] listArticles - Response:', response.data);

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
    console.log('[ArticleAPI] createArticle - Request:', { url: NEWS_API_ENDPOINTS.ARTICLES, data: article });
    const response = await this.client.post<Article>(
      NEWS_API_ENDPOINTS.ARTICLES,
      article
    );
    console.log('[ArticleAPI] createArticle - Response:', response.data);

    return this.transformArticleDate(response.data);
  }

  /**
   * Update an existing article
   */
  async updateArticle(article: ArticleUpdate): Promise<Article> {
    console.log('[ArticleAPI] updateArticle - Request:', { url: NEWS_API_ENDPOINTS.ARTICLES, data: article });
    const response = await this.client.put<Article>(
      NEWS_API_ENDPOINTS.ARTICLES,
      article
    );
    console.log('[ArticleAPI] updateArticle - Response:', response.data);

    return this.transformArticleDate(response.data);
  }

  /**
   * Delete an article
   */
  async deleteArticle(id: number): Promise<void> {
    console.log('[ArticleAPI] deleteArticle - Request:', { id, url: NEWS_API_ENDPOINTS.ARTICLE_BY_ID(id) });
    await this.client.delete(NEWS_API_ENDPOINTS.ARTICLE_BY_ID(id));
    console.log('[ArticleAPI] deleteArticle - Success');
  }

  /**
   * Create a new article using AI
   * @param request - AI article request with prompt and options
   * @returns Created article
   */
  async createArticleWithAI(request: AIArticleRequest): Promise<Article> {
    console.log('[ArticleAPI] createArticleWithAI - Request:', { url: NEWS_API_ENDPOINTS.INSERT_WITH_AI, data: request });
    const response = await this.client.post<Article>(
      NEWS_API_ENDPOINTS.INSERT_WITH_AI,
      request
    );
    console.log('[ArticleAPI] createArticleWithAI - Response:', response.data);
    return this.transformArticleDate(response.data);
  }

  /**
   * Update an existing article using AI
   * @param request - AI article request with articleId, prompt and options
   * @returns Updated article
   */
  async updateArticleWithAI(request: AIArticleRequest): Promise<Article> {
    if (!request.articleId) {
      throw new Error('articleId is required for updateWithAI');
    }
    
    console.log('[ArticleAPI] updateArticleWithAI - Request:', { url: NEWS_API_ENDPOINTS.UPDATE_WITH_AI, data: request });
    const response = await this.client.put<Article>(
      NEWS_API_ENDPOINTS.UPDATE_WITH_AI,
      request
    );
    console.log('[ArticleAPI] updateArticleWithAI - Response:', response.data);
    return this.transformArticleDate(response.data);
  }

  /**
   * Transform date strings to Date objects for a single article
   */
  private transformArticleDate(article: Article): Article {
    return {
      ...article,
      dateAt: article.dateAt ? new Date(article.dateAt) : undefined,
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

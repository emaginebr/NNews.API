import type { AxiosInstance } from 'axios';
import type { Category, CategoryInput, CategoryUpdate } from '../types/news';

const NEWS_API_ENDPOINTS = {
  CATEGORIES: '/api/category',
  CATEGORIES_FILTER: '/api/category/filter',
  CATEGORY_BY_ID: (id: number) => `/api/category/${id}`,
};

export class CategoryAPI {
  constructor(private client: AxiosInstance) {}

  /**
   * List all categories
   */
  async listCategories(): Promise<Category[]> {
    const response = await this.client.get<Category[]>(NEWS_API_ENDPOINTS.CATEGORIES);
    return this.transformCategoryDates(response.data);
  }

  /**
   * Filter categories by roles and parent category
   */
  async filterCategories(roles?: string[], parentId?: number): Promise<Category[]> {
    const params: Record<string, any> = {};
    
    if (roles && roles.length > 0) {
      params.roles = roles.join(',');
    }
    
    if (parentId !== undefined) {
      params.parentId = parentId;
    }

    const response = await this.client.get<Category[]>(
      NEWS_API_ENDPOINTS.CATEGORIES_FILTER,
      { params }
    );

    return this.transformCategoryDates(response.data);
  }

  /**
   * Get a single category by ID
   */
  async getCategoryById(id: number): Promise<Category> {
    const response = await this.client.get<Category>(
      NEWS_API_ENDPOINTS.CATEGORY_BY_ID(id)
    );

    return this.transformCategoryDate(response.data);
  }

  /**
   * Create a new category
   */
  async createCategory(category: CategoryInput): Promise<Category> {
    const response = await this.client.post<Category>(
      NEWS_API_ENDPOINTS.CATEGORIES,
      category
    );

    return this.transformCategoryDate(response.data);
  }

  /**
   * Update an existing category
   */
  async updateCategory(category: CategoryUpdate): Promise<Category> {
    const response = await this.client.put<Category>(
      NEWS_API_ENDPOINTS.CATEGORIES,
      category
    );

    return this.transformCategoryDate(response.data);
  }

  /**
   * Delete a category
   */
  async deleteCategory(id: number): Promise<void> {
    await this.client.delete(NEWS_API_ENDPOINTS.CATEGORY_BY_ID(id));
  }

  /**
   * Transform date strings to Date objects for a single category
   */
  private transformCategoryDate(category: Category): Category {
    return {
      ...category,
      createdAt: category.createdAt ? new Date(category.createdAt) : undefined,
      updatedAt: category.updatedAt ? new Date(category.updatedAt) : undefined,
    };
  }

  /**
   * Transform date strings to Date objects for category arrays
   */
  private transformCategoryDates(categories: Category[]): Category[] {
    return categories.map(category => this.transformCategoryDate(category));
  }
}

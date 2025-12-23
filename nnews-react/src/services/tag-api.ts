import type { AxiosInstance } from 'axios';
import type { Tag, TagInput, TagUpdate } from '../types/news';

const NEWS_API_ENDPOINTS = {
  TAGS: '/api/tag',
  TAG_BY_ID: (id: number) => `/api/tag/${id}`,
  TAG_MERGE: (sourceId: number, targetId: number) => `/api/Tag/merge/${sourceId}/${targetId}`,
};

export class TagAPI {
  constructor(private client: AxiosInstance) {}

  /**
   * List all tags
   */
  async listTags(): Promise<Tag[]> {
    console.log('[TagAPI] listTags - Request:', { url: NEWS_API_ENDPOINTS.TAGS });
    const response = await this.client.get<Tag[]>(NEWS_API_ENDPOINTS.TAGS);
    console.log('[TagAPI] listTags - Response:', response.data);
    return response.data;
  }

  /**
   * Get a single tag by ID
   */
  async getTagById(id: number): Promise<Tag> {
    console.log('[TagAPI] getTagById - Request:', { id, url: NEWS_API_ENDPOINTS.TAG_BY_ID(id) });
    const response = await this.client.get<Tag>(NEWS_API_ENDPOINTS.TAG_BY_ID(id));
    console.log('[TagAPI] getTagById - Response:', response.data);
    return response.data;
  }

  /**
   * Create a new tag
   */
  async createTag(tag: TagInput): Promise<Tag> {
    console.log('[TagAPI] createTag - Request:', { url: NEWS_API_ENDPOINTS.TAGS, data: tag });
    const response = await this.client.post<Tag>(NEWS_API_ENDPOINTS.TAGS, tag);
    console.log('[TagAPI] createTag - Response:', response.data);
    return response.data;
  }

  /**
   * Update an existing tag
   */
  async updateTag(tag: TagUpdate): Promise<Tag> {
    console.log('[TagAPI] updateTag - Request:', { url: NEWS_API_ENDPOINTS.TAGS, data: tag });
    const response = await this.client.put<Tag>(NEWS_API_ENDPOINTS.TAGS, tag);
    console.log('[TagAPI] updateTag - Response:', response.data);
    return response.data;
  }

  /**
   * Delete a tag
   */
  async deleteTag(id: number): Promise<void> {
    console.log('[TagAPI] deleteTag - Request:', { id, url: NEWS_API_ENDPOINTS.TAG_BY_ID(id) });
    await this.client.delete(NEWS_API_ENDPOINTS.TAG_BY_ID(id));
    console.log('[TagAPI] deleteTag - Success');
  }

  /**
   * Merge two tags (move all articles from source to target, then delete source)
   */
  async mergeTags(sourceTagId: number, targetTagId: number): Promise<void> {
    const url = NEWS_API_ENDPOINTS.TAG_MERGE(sourceTagId, targetTagId);
    console.log('[TagAPI] mergeTags - Request:', { url, sourceTagId, targetTagId });
    await this.client.post(url);
    console.log('[TagAPI] mergeTags - Success');
  }
}

import type { AxiosInstance } from 'axios';
import type { Tag, TagInput, TagUpdate } from '../types/news';

const NEWS_API_ENDPOINTS = {
  TAGS: '/api/tag',
  TAG_BY_ID: (id: number) => `/api/tag/${id}`,
  TAG_MERGE: '/api/tag/merge',
};

export class TagAPI {
  constructor(private client: AxiosInstance) {}

  /**
   * List all tags
   */
  async listTags(): Promise<Tag[]> {
    const response = await this.client.get<Tag[]>(NEWS_API_ENDPOINTS.TAGS);
    return response.data;
  }

  /**
   * Get a single tag by ID
   */
  async getTagById(id: number): Promise<Tag> {
    const response = await this.client.get<Tag>(NEWS_API_ENDPOINTS.TAG_BY_ID(id));
    return response.data;
  }

  /**
   * Create a new tag
   */
  async createTag(tag: TagInput): Promise<Tag> {
    const response = await this.client.post<Tag>(NEWS_API_ENDPOINTS.TAGS, tag);
    return response.data;
  }

  /**
   * Update an existing tag
   */
  async updateTag(tag: TagUpdate): Promise<Tag> {
    const response = await this.client.put<Tag>(NEWS_API_ENDPOINTS.TAGS, tag);
    return response.data;
  }

  /**
   * Delete a tag
   */
  async deleteTag(id: number): Promise<void> {
    await this.client.delete(NEWS_API_ENDPOINTS.TAG_BY_ID(id));
  }

  /**
   * Merge two tags (move all articles from source to target, then delete source)
   */
  async mergeTags(sourceTagId: number, targetTagId: number): Promise<void> {
    await this.client.post(NEWS_API_ENDPOINTS.TAG_MERGE, {
      sourceTagId,
      targetTagId,
    });
  }
}

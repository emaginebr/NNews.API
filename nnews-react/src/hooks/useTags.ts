import { useState, useCallback } from 'react';
import { useNNews } from '../contexts/NNewsContext';
import type { Tag, TagInput, TagUpdate, TagSearchParams } from '../types/news';

export interface UseTagsResult {
  tags: Tag[];
  loading: boolean;
  error: Error | null;
  fetchTags: (params?: TagSearchParams) => Promise<void>;
  getTagById: (id: number) => Promise<Tag>;
  createTag: (tag: TagInput) => Promise<Tag>;
  updateTag: (tag: TagUpdate) => Promise<Tag>;
  deleteTag: (id: number) => Promise<void>;
  mergeTags: (sourceId: number, targetId: number) => Promise<void>;
  refresh: () => Promise<void>;
}

export function useTags(): UseTagsResult {
  const { tagApi } = useNNews();
  const [tags, setTags] = useState<Tag[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<Error | null>(null);
  const [lastParams, setLastParams] = useState<TagSearchParams | undefined>();

  const fetchTags = useCallback(
    async (params?: TagSearchParams) => {
      setLoading(true);
      setError(null);
      setLastParams(params);

      try {
        let result = await tagApi.listTags();

        if (params?.searchTerm) {
          const searchLower = params.searchTerm.toLowerCase();
          result = result.filter(
            tag =>
              tag.title.toLowerCase().includes(searchLower) ||
              (tag.slug && tag.slug.toLowerCase().includes(searchLower))
          );
        }

        setTags(result);
      } catch (err) {
        const error = err instanceof Error ? err : new Error('Failed to fetch tags');
        setError(error);
        throw error;
      } finally {
        setLoading(false);
      }
    },
    [tagApi]
  );

  const getTagById = useCallback(
    async (id: number): Promise<Tag> => {
      setLoading(true);
      setError(null);

      try {
        const tag = await tagApi.getTagById(id);
        return tag;
      } catch (err) {
        const error = err instanceof Error ? err : new Error('Failed to fetch tag');
        setError(error);
        throw error;
      } finally {
        setLoading(false);
      }
    },
    [tagApi]
  );

  const createTag = useCallback(
    async (tag: TagInput): Promise<Tag> => {
      setLoading(true);
      setError(null);

      try {
        const newTag = await tagApi.createTag(tag);
        
        if (lastParams !== undefined) {
          await fetchTags(lastParams);
        } else {
          await fetchTags();
        }
        
        return newTag;
      } catch (err) {
        const error = err instanceof Error ? err : new Error('Failed to create tag');
        setError(error);
        throw error;
      } finally {
        setLoading(false);
      }
    },
    [tagApi, lastParams, fetchTags]
  );

  const updateTag = useCallback(
    async (tag: TagUpdate): Promise<Tag> => {
      setLoading(true);
      setError(null);

      try {
        const updatedTag = await tagApi.updateTag(tag);
        
        if (lastParams !== undefined) {
          await fetchTags(lastParams);
        } else {
          await fetchTags();
        }
        
        return updatedTag;
      } catch (err) {
        const error = err instanceof Error ? err : new Error('Failed to update tag');
        setError(error);
        throw error;
      } finally {
        setLoading(false);
      }
    },
    [tagApi, lastParams, fetchTags]
  );

  const deleteTag = useCallback(
    async (id: number): Promise<void> => {
      setLoading(true);
      setError(null);

      try {
        await tagApi.deleteTag(id);
        
        if (lastParams !== undefined) {
          await fetchTags(lastParams);
        } else {
          await fetchTags();
        }
      } catch (err) {
        const error = err instanceof Error ? err : new Error('Failed to delete tag');
        setError(error);
        throw error;
      } finally {
        setLoading(false);
      }
    },
    [tagApi, lastParams, fetchTags]
  );

  const mergeTags = useCallback(
    async (sourceId: number, targetId: number): Promise<void> => {
      setLoading(true);
      setError(null);

      try {
        await tagApi.mergeTags(sourceId, targetId);
        
        if (lastParams !== undefined) {
          await fetchTags(lastParams);
        } else {
          await fetchTags();
        }
      } catch (err) {
        const error = err instanceof Error ? err : new Error('Failed to merge tags');
        setError(error);
        throw error;
      } finally {
        setLoading(false);
      }
    },
    [tagApi, lastParams, fetchTags]
  );

  const refresh = useCallback(async () => {
    if (lastParams !== undefined) {
      await fetchTags(lastParams);
    } else {
      await fetchTags();
    }
  }, [lastParams, fetchTags]);

  return {
    tags,
    loading,
    error,
    fetchTags,
    getTagById,
    createTag,
    updateTag,
    deleteTag,
    mergeTags,
    refresh,
  };
}

import { useState, useCallback } from 'react';
import { useNNews } from '../contexts/NNewsContext';
import type {
  Article,
  ArticleInput,
  ArticleUpdate,
  PagedResult,
  ArticleSearchParams,
} from '../types/news';

export interface UseArticlesResult {
  articles: PagedResult<Article> | null;
  loading: boolean;
  error: Error | null;
  fetchArticles: (params?: ArticleSearchParams) => Promise<void>;
  getArticleById: (id: number) => Promise<Article>;
  createArticle: (article: ArticleInput) => Promise<Article>;
  updateArticle: (article: ArticleUpdate) => Promise<Article>;
  deleteArticle: (id: number) => Promise<void>;
  refresh: () => Promise<void>;
}

export function useArticles(): UseArticlesResult {
  const { articleApi } = useNNews();
  const [articles, setArticles] = useState<PagedResult<Article> | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<Error | null>(null);
  const [lastParams, setLastParams] = useState<ArticleSearchParams | undefined>();

  const fetchArticles = useCallback(
    async (params?: ArticleSearchParams) => {
      setLoading(true);
      setError(null);
      setLastParams(params);

      try {
        let result: PagedResult<Article>;

        if (params?.roles && params.roles.length > 0) {
          result = await articleApi.filterArticles(
            params.roles,
            params.categoryId,
            params.page,
            params.pageSize
          );
        } else {
          result = await articleApi.listArticles(
            params?.categoryId,
            params?.page,
            params?.pageSize
          );
        }

        setArticles(result);
      } catch (err) {
        const error = err instanceof Error ? err : new Error('Failed to fetch articles');
        setError(error);
        throw error;
      } finally {
        setLoading(false);
      }
    },
    [articleApi]
  );

  const getArticleById = useCallback(
    async (id: number): Promise<Article> => {
      setLoading(true);
      setError(null);

      try {
        const article = await articleApi.getArticleById(id);
        return article;
      } catch (err) {
        const error = err instanceof Error ? err : new Error('Failed to fetch article');
        setError(error);
        throw error;
      } finally {
        setLoading(false);
      }
    },
    [articleApi]
  );

  const createArticle = useCallback(
    async (article: ArticleInput): Promise<Article> => {
      setLoading(true);
      setError(null);

      try {
        const newArticle = await articleApi.createArticle(article);
        
        if (lastParams) {
          await fetchArticles(lastParams);
        }
        
        return newArticle;
      } catch (err) {
        const error = err instanceof Error ? err : new Error('Failed to create article');
        setError(error);
        throw error;
      } finally {
        setLoading(false);
      }
    },
    [articleApi, lastParams, fetchArticles]
  );

  const updateArticle = useCallback(
    async (article: ArticleUpdate): Promise<Article> => {
      setLoading(true);
      setError(null);

      try {
        const updatedArticle = await articleApi.updateArticle(article);
        
        if (lastParams) {
          await fetchArticles(lastParams);
        }
        
        return updatedArticle;
      } catch (err) {
        const error = err instanceof Error ? err : new Error('Failed to update article');
        setError(error);
        throw error;
      } finally {
        setLoading(false);
      }
    },
    [articleApi, lastParams, fetchArticles]
  );

  const deleteArticle = useCallback(
    async (id: number): Promise<void> => {
      setLoading(true);
      setError(null);

      try {
        await articleApi.deleteArticle(id);
        
        if (lastParams) {
          await fetchArticles(lastParams);
        }
      } catch (err) {
        const error = err instanceof Error ? err : new Error('Failed to delete article');
        setError(error);
        throw error;
      } finally {
        setLoading(false);
      }
    },
    [articleApi, lastParams, fetchArticles]
  );

  const refresh = useCallback(async () => {
    if (lastParams) {
      await fetchArticles(lastParams);
    }
  }, [lastParams, fetchArticles]);

  return {
    articles,
    loading,
    error,
    fetchArticles,
    getArticleById,
    createArticle,
    updateArticle,
    deleteArticle,
    refresh,
  };
}

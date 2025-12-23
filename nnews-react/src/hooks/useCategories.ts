import { useState, useCallback } from 'react';
import { useNNews } from '../contexts/NNewsContext';
import type {
  Category,
  CategoryInput,
  CategoryUpdate,
  CategoryFilterParams,
} from '../types/news';

export interface UseCategoriesResult {
  categories: Category[];
  loading: boolean;
  error: Error | null;
  fetchCategories: (params?: CategoryFilterParams) => Promise<void>;
  getCategoryById: (id: number) => Promise<Category>;
  createCategory: (category: CategoryInput) => Promise<Category>;
  updateCategory: (category: CategoryUpdate) => Promise<Category>;
  deleteCategory: (id: number) => Promise<void>;
  refresh: () => Promise<void>;
}

export function useCategories(): UseCategoriesResult {
  const { categoryApi } = useNNews();
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<Error | null>(null);
  const [lastParams, setLastParams] = useState<CategoryFilterParams | undefined>();

  const fetchCategories = useCallback(
    async (params?: CategoryFilterParams) => {
      setLoading(true);
      setError(null);
      setLastParams(params);

      try {
        let result: Category[];

        if (params?.roles && params.roles.length > 0) {
          result = await categoryApi.filterCategories(
            params.roles,
            params.parentId
          );
        } else if (params?.parentId !== undefined) {
          result = await categoryApi.filterCategories(
            undefined,
            params.parentId
          );
        } else {
          result = await categoryApi.listCategories();
        }

        if (params?.searchTerm) {
          const searchLower = params.searchTerm.toLowerCase();
          result = result.filter(cat =>
            cat.title.toLowerCase().includes(searchLower)
          );
        }

        setCategories(result);
      } catch (err) {
        const error = err instanceof Error ? err : new Error('Failed to fetch categories');
        setError(error);
        throw error;
      } finally {
        setLoading(false);
      }
    },
    [categoryApi]
  );

  const getCategoryById = useCallback(
    async (id: number): Promise<Category> => {
      setLoading(true);
      setError(null);

      try {
        const category = await categoryApi.getCategoryById(id);
        return category;
      } catch (err) {
        const error = err instanceof Error ? err : new Error('Failed to fetch category');
        setError(error);
        throw error;
      } finally {
        setLoading(false);
      }
    },
    [categoryApi]
  );

  const createCategory = useCallback(
    async (category: CategoryInput): Promise<Category> => {
      setLoading(true);
      setError(null);

      try {
        const newCategory = await categoryApi.createCategory(category);
        
        if (lastParams !== undefined) {
          await fetchCategories(lastParams);
        } else {
          await fetchCategories();
        }
        
        return newCategory;
      } catch (err) {
        const error = err instanceof Error ? err : new Error('Failed to create category');
        setError(error);
        throw error;
      } finally {
        setLoading(false);
      }
    },
    [categoryApi, lastParams, fetchCategories]
  );

  const updateCategory = useCallback(
    async (category: CategoryUpdate): Promise<Category> => {
      setLoading(true);
      setError(null);

      try {
        const updatedCategory = await categoryApi.updateCategory(category);
        
        if (lastParams !== undefined) {
          await fetchCategories(lastParams);
        } else {
          await fetchCategories();
        }
        
        return updatedCategory;
      } catch (err) {
        const error = err instanceof Error ? err : new Error('Failed to update category');
        setError(error);
        throw error;
      } finally {
        setLoading(false);
      }
    },
    [categoryApi, lastParams, fetchCategories]
  );

  const deleteCategory = useCallback(
    async (id: number): Promise<void> => {
      setLoading(true);
      setError(null);

      try {
        await categoryApi.deleteCategory(id);
        
        if (lastParams !== undefined) {
          await fetchCategories(lastParams);
        } else {
          await fetchCategories();
        }
      } catch (err) {
        const error = err instanceof Error ? err : new Error('Failed to delete category');
        setError(error);
        throw error;
      } finally {
        setLoading(false);
      }
    },
    [categoryApi, lastParams, fetchCategories]
  );

  const refresh = useCallback(async () => {
    if (lastParams !== undefined) {
      await fetchCategories(lastParams);
    } else {
      await fetchCategories();
    }
  }, [lastParams, fetchCategories]);

  return {
    categories,
    loading,
    error,
    fetchCategories,
    getCategoryById,
    createCategory,
    updateCategory,
    deleteCategory,
    refresh,
  };
}

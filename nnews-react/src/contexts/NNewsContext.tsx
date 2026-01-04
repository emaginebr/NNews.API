import React, { createContext, useContext, useMemo } from 'react';
import axios, { type AxiosInstance } from 'axios';
import { ArticleAPI } from '../services/article-api';
import { CategoryAPI } from '../services/category-api';
import { TagAPI } from '../services/tag-api';

export interface NNewsConfig {
  apiUrl: string;
  apiClient?: AxiosInstance;
  headers?: Record<string, string>;
}

export interface NNewsContextValue {
  config: NNewsConfig;
  apiClient: AxiosInstance;
  articleApi: ArticleAPI;
  categoryApi: CategoryAPI;
  tagApi: TagAPI;
}

const NNewsContext = createContext<NNewsContextValue | undefined>(undefined);

export interface NNewsProviderProps {
  config: NNewsConfig;
  children: React.ReactNode;
}

export function NNewsProvider({ config, children }: NNewsProviderProps) {
  const contextValue = useMemo(() => {
    const apiClient =
      config.apiClient ||
      axios.create({
        baseURL: config.apiUrl,
        headers: {
          'Content-Type': 'application/json',
          ...config.headers,
        },
      });

    const articleApi = new ArticleAPI(apiClient);
    const categoryApi = new CategoryAPI(apiClient);
    const tagApi = new TagAPI(apiClient);

    return {
      config,
      apiClient,
      articleApi,
      categoryApi,
      tagApi,
    };
  }, [config]);

  return (
    <NNewsContext.Provider value={contextValue}>
      {children}
    </NNewsContext.Provider>
  );
}

export function useNNews(): NNewsContextValue {
  const context = useContext(NNewsContext);
  
  if (!context) {
    throw new Error('useNNews must be used within a NNewsProvider');
  }
  
  return context;
}

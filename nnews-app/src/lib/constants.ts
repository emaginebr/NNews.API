export const APP_NAME = 'NNews';
export const APP_DESCRIPTION = 'Content management system with articles, categories, and tags powered by nnews-react';

export const ROUTES = {
  HOME: '/',
  LOGIN: '/login',
  DASHBOARD: '/dashboard',
  PROFILE: '/profile',
  TAGS: '/tags',
  CATEGORIES: '/categories',
  ARTICLES: '/articles',
  ARTICLES_NEW: '/articles/new',
  ARTICLES_EDIT: (id: number) => `/articles/edit/${id}`,
  ARTICLES_VIEW: (id: number) => `/articles/${id}`,
  ARTICLES_AI: '/articles/ai',
  ARTICLES_AI_EDIT: (id: number) => `/articles/ai/${id}`,
} as const;

export const EXTERNAL_LINKS = {
  TERMS: '/terms',
  PRIVACY: '/privacy',
  DOCS: 'https://github.com/landim32/News',
} as const;

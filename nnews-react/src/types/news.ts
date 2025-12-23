// ============================================================================
// Core Enums
// ============================================================================

export enum ArticleStatus {
  Draft = 0,
  Published = 1,
  Archived = 2,
}

// ============================================================================
// Core Interfaces
// ============================================================================

export interface Role {
  slug: string;
  name: string;
}

export interface Tag {
  id: number;
  tagId?: number;
  title: string;
  slug?: string;
  articleCount?: number;
}

export interface Category {
  id: number;
  categoryId?: number;
  parentId?: number;
  title: string;
  slug?: string;
  description?: string;
  visibleToRoles?: string[];
  createdAt?: Date;
  updatedAt?: Date;
  articleCount?: number;
}

export interface Article {
  id: number;
  articleId?: number;
  categoryId?: number;
  authorId?: number;
  title: string;
  subtitle?: string;
  excerpt?: string;
  content: string;
  status: ArticleStatus;
  author?: string;
  category?: Category;
  tags?: Tag[];
  visibleToRoles?: string[];
  createdAt?: string | Date;
  updatedAt?: string | Date;
}

// ============================================================================
// Input/Update Interfaces
// ============================================================================

export interface TagInput {
  title: string;
  slug?: string;
}

export interface TagUpdate extends TagInput {
  id: number;
  tagId?: number;
}

export interface CategoryInput {
  title: string;
  slug?: string;
  description?: string;
  parentId?: number;
  visibleToRoles?: string[];
}

export interface CategoryUpdate extends CategoryInput {
  id: number;
  categoryId?: number;
}

export interface ArticleInput {
  title: string;
  subtitle?: string;
  excerpt?: string;
  content: string;
  status: ArticleStatus;
  author?: string;
  categoryId?: number;
  tagIds?: number[];
  visibleToRoles?: string[];
}

export interface ArticleUpdate extends ArticleInput {
  id: number;
  articleId?: number;
}

// ============================================================================
// Pagination
// ============================================================================

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPrevious: boolean;
  hasNext: boolean;
}

// ============================================================================
// API Configuration
// ============================================================================

export interface NNewsConfig {
  apiUrl: string;
  timeout?: number;
  headers?: Record<string, string>;
  onError?: (error: Error) => void;
  enableCache?: boolean;
  cacheTimeout?: number;
}

export interface NewsApiError {
  message: string;
  status: number;
  errors?: Record<string, string[]>;
}

// ============================================================================
// API Endpoints
// ============================================================================

export const NEWS_API_ENDPOINTS = {
  // Articles
  ARTICLES: '/api/article',
  ARTICLES_FILTER: '/api/article/filter',
  ARTICLE_BY_ID: (id: number) => `/api/article/${id}`,

  // Categories
  CATEGORIES: '/api/category',
  CATEGORIES_FILTER: '/api/category/filter',
  CATEGORY_BY_ID: (id: number) => `/api/category/${id}`,

  // Tags
  TAGS: '/api/tag',
  TAG_BY_ID: (id: number) => `/api/tag/${id}`,
  TAG_MERGE: '/api/tag/merge',
} as const;

// ============================================================================
// Context Types
// ============================================================================

export interface NNewsContextValue {
  config: NNewsConfig;
  isLoading: boolean;
  error: Error | null;
  setError: (error: Error | null) => void;
}

// ============================================================================
// Component Props Types
// ============================================================================

export interface ArticleListProps {
  categoryId?: number;
  status?: ArticleStatus;
  page?: number;
  pageSize?: number;
  onEdit?: (article: Article) => void;
  onDelete?: (articleId: number) => void;
  onView?: (article: Article) => void;
  showCreateButton?: boolean;
  className?: string;
}

export interface ArticleEditorProps {
  articleId?: number;
  initialData?: Partial<ArticleInput>;
  onSave?: (article: Article) => void;
  onCancel?: () => void;
  showPreview?: boolean;
  enableAutoSave?: boolean;
  className?: string;
}

export interface ArticleViewerProps {
  articleId?: number;
  article?: Article;
  onEdit?: (article: Article) => void;
  onBack?: () => void;
  showMetadata?: boolean;
  className?: string;
}

export interface CategoryListProps {
  onEdit?: (category: Category) => void;
  onDelete?: (categoryId: number) => void;
  showCreateButton?: boolean;
  className?: string;
}

export interface CategoryModalProps {
  isOpen: boolean;
  onClose: () => void;
  category?: Category;
  onSave?: (category: Category) => void;
}

export interface TagListProps {
  onEdit?: (tag: Tag) => void;
  onDelete?: (tagId: number) => void;
  onMerge?: (sourceId: number, targetId: number) => void;
  showCreateButton?: boolean;
  className?: string;
}

export interface TagModalProps {
  isOpen: boolean;
  onClose: () => void;
  tag?: Tag;
  onSave?: (tag: Tag) => void;
}

// ============================================================================
// Search and Filter Types
// ============================================================================

export interface ArticleSearchParams {
  categoryId?: number;
  status?: ArticleStatus;
  tags?: number[];
  roles?: string[];
  searchTerm?: string;
  page?: number;
  pageSize?: number;
}

export interface CategoryFilterParams {
  roles?: string[];
  parentId?: number;
  searchTerm?: string;
}

export interface TagSearchParams {
  searchTerm?: string;
  page?: number;
  pageSize?: number;
}

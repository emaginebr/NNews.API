// ============================================================================
// Core Enums
// ============================================================================

export enum ArticleStatus {
  Draft = 0,
  Published = 1,
  Archived = 2,
  Scheduled = 3,
  Review = 4,
}

// ============================================================================
// Core Interfaces
// ============================================================================

export interface Role {
  slug: string;
  name: string;
}

export interface Tag {
  tagId?: number;
  title: string;
  slug?: string;
  articleCount?: number;
}

export interface Category {
  categoryId: number;
  parentId?: number;
  title: string;
  createdAt?: Date;
  updatedAt?: Date;
  articleCount?: number;
}

export interface Article {
  articleId: number;
  categoryId?: number;
  authorId?: number;
  title: string;
  content: string;
  imageName?: string;
  imageUrl?: string;
  status: ArticleStatus;
  dateAt?: string | Date;
  createdAt?: string | Date;
  updatedAt?: string | Date;
  category?: Category;
  tags?: Tag[];
  roles?: Role[];
}

// ============================================================================
// Input/Update Interfaces
// ============================================================================

export interface TagInput {
  title: string;
  slug?: string;
}

export interface TagUpdate extends TagInput {
  tagId: number;
  id?: number;
}

export interface CategoryInput {
  title: string;
  parentId?: number;
}

export interface CategoryUpdate extends CategoryInput {
  categoryId: number;
}

export interface ArticleInput {
  title: string;
  content: string;
  imageName?: string;
  status: ArticleStatus;
  categoryId?: number;
  dateAt?: string | Date;
  tagList?: string;
  roleIds?: string[];
}

export interface ArticleUpdate extends ArticleInput {
  articleId: number;
}

export interface AIArticleRequest {
  articleId?: number;
  prompt: string;
  generateImage?: boolean;
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

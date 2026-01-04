// Main entry point for @ntools/nnews package

// Styles - Import first
import './styles/index.css';

// News Components - Articles
export { ArticleList } from './components/ArticleList';
export { ArticleViewer } from './components/ArticleViewer';
export { ArticleEditor } from './components/ArticleEditor';
export { AIArticleGenerator } from './components/AIArticleGenerator';
export { MarkdownEditor } from './components/MarkdownEditor';
export { RichTextEditor } from './components/RichTextEditor';

// News Components - Categories
export { CategoryList } from './components/CategoryList';
export { CategoryModal } from './components/CategoryModal';

// News Components - Tags
export { TagList } from './components/TagList';
export { TagModal } from './components/TagModal';
export { TagMerge } from './components/TagMerge';

// Context Providers & Hooks
export { NNewsProvider, useNNews } from './contexts/NNewsContext';
export type { NNewsConfig, NNewsContextValue, NNewsProviderProps } from './contexts/NNewsContext';

// Custom Hooks - News
export { useArticles } from './hooks/useArticles';
export type { UseArticlesResult } from './hooks/useArticles';
export { useCategories } from './hooks/useCategories';
export type { UseCategoriesResult } from './hooks/useCategories';
export { useTags } from './hooks/useTags';
export type { UseTagsResult } from './hooks/useTags';

// API Client
export { ArticleAPI } from './services/article-api';
export { CategoryAPI } from './services/category-api';
export { TagAPI } from './services/tag-api';

// UI Components
export { Button } from './components/ui/button';
export { Input } from './components/ui/input';
export { Label } from './components/ui/label';
export { Avatar, AvatarImage, AvatarFallback } from './components/ui/avatar';

// News Types
export type {
  Article,
  Category,
  Tag,
  ArticleInput,
  ArticleUpdate,
  AIArticleRequest,
  CategoryInput,
  CategoryUpdate,
  TagInput,
  TagUpdate,
  ArticleSearchParams,
  CategoryFilterParams,
  TagSearchParams,
  ArticleListProps,
  ArticleViewerProps,
  ArticleEditorProps,
  CategoryListProps,
  CategoryModalProps,
  TagListProps,
  TagModalProps,
} from './types/news';

export type { TagMergeProps } from './components/TagMerge';
export type { AIArticleGeneratorProps } from './components/AIArticleGenerator';

export { ArticleStatus } from './types/news';

// Utility Functions
export {
  validateCPF,
  validateCNPJ,
  validateEmail,
  validatePhone,
  formatPhone,
  formatDocument,
  formatZipCode,
  validatePasswordStrength,
  debounce,
  throttle,
  tagsToString,
  stringToTagsPreview,
  validatePrompt,
  validateTagList,
} from './utils/validators';

export type { PasswordStrength } from './utils/validators';

export { cn } from './utils/cn';

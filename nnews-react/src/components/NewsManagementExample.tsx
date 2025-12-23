import { useState, useEffect } from 'react';
import {
  ArticleList,
  ArticleViewer,
  ArticleEditor,
  CategoryList,
  CategoryModal,
  TagList,
  TagModal,
  useArticles,
  useCategories,
  useTags,
  type Article,
  type Category,
  type Tag,
} from '..';

type ViewMode = 'list' | 'view' | 'edit';
type ManagementTab = 'articles' | 'categories' | 'tags';

export function NewsManagementExample() {
  // State
  const [viewMode, setViewMode] = useState<ViewMode>('list');
  const [activeTab, setActiveTab] = useState<ManagementTab>('articles');
  const [selectedArticle, setSelectedArticle] = useState<Article | null>(null);
  const [selectedCategory, setSelectedCategory] = useState<Category | null>(null);
  const [selectedTag, setSelectedTag] = useState<Tag | null>(null);
  const [categoryModalOpen, setCategoryModalOpen] = useState(false);
  const [tagModalOpen, setTagModalOpen] = useState(false);

  // Hooks
  const {
    articles,
    loading: articlesLoading,
    error: articlesError,
    fetchArticles,
    createArticle,
    updateArticle,
    deleteArticle,
  } = useArticles();

  const {
    categories,
    loading: categoriesLoading,
    error: categoriesError,
    fetchCategories,
    createCategory,
    updateCategory,
    deleteCategory,
  } = useCategories();

  const {
    tags,
    loading: tagsLoading,
    error: tagsError,
    fetchTags,
    createTag,
    updateTag,
    deleteTag,
  } = useTags();

  // Effects
  useEffect(() => {
    fetchArticles({ page: 1, pageSize: 20 });
    fetchCategories();
    fetchTags();
  }, []);

  // Article Handlers
  const handleArticleClick = (article: Article) => {
    setSelectedArticle(article);
    setViewMode('view');
  };

  const handleArticleEdit = (article?: Article) => {
    setSelectedArticle(article || null);
    setViewMode('edit');
  };

  const handleArticleSave = async (articleData: any) => {
    try {
      if (selectedArticle) {
        await updateArticle({ ...articleData, id: selectedArticle.id });
      } else {
        await createArticle(articleData);
      }
      setViewMode('list');
      setSelectedArticle(null);
    } catch (error) {
      console.error('Failed to save article:', error);
      alert('Failed to save article');
    }
  };

  const handleArticleDelete = async (article: Article) => {
    if (window.confirm(`Are you sure you want to delete "${article.title}"?`)) {
      try {
        await deleteArticle(article.id);
      } catch (error) {
        console.error('Failed to delete article:', error);
        alert('Failed to delete article');
      }
    }
  };

  // Category Handlers
  const handleCategoryEdit = (category?: Category) => {
    setSelectedCategory(category || null);
    setCategoryModalOpen(true);
  };

  const handleCategorySave = async (categoryData: any) => {
    try {
      if (selectedCategory) {
        await updateCategory({ ...categoryData, id: selectedCategory.id });
      } else {
        await createCategory(categoryData);
      }
      setSelectedCategory(null);
    } catch (error) {
      console.error('Failed to save category:', error);
      alert('Failed to save category');
    }
  };

  const handleCategoryDelete = async (category: Category) => {
    if (window.confirm(`Are you sure you want to delete "${category.title}"?`)) {
      try {
        await deleteCategory(category.id);
      } catch (error) {
        console.error('Failed to delete category:', error);
        alert('Failed to delete category');
      }
    }
  };

  // Tag Handlers
  const handleTagEdit = (tag?: Tag) => {
    setSelectedTag(tag || null);
    setTagModalOpen(true);
  };

  const handleTagSave = async (tagData: any) => {
    try {
      if (selectedTag) {
        await updateTag({ ...tagData, id: selectedTag.id });
      } else {
        await createTag(tagData);
      }
      setSelectedTag(null);
    } catch (error) {
      console.error('Failed to save tag:', error);
      alert('Failed to save tag');
    }
  };

  const handleTagDelete = async (tag: Tag) => {
    if (window.confirm(`Are you sure you want to delete "#${tag.title}"?`)) {
      try {
        await deleteTag(tag.id);
      } catch (error) {
        console.error('Failed to delete tag:', error);
        alert('Failed to delete tag');
      }
    }
  };

  // Render
  return (
    <div className="min-h-screen bg-gray-50">
      <div className="container mx-auto px-4 py-8">
        {/* Header */}
        <div className="mb-8">
          <h1 className="text-4xl font-bold text-gray-900">
            News Management System
          </h1>
          <p className="mt-2 text-gray-600">
            Manage your articles, categories, and tags
          </p>
        </div>

        {/* Article View/Edit */}
        {viewMode === 'view' && selectedArticle && (
          <ArticleViewer
            article={selectedArticle}
            onBack={() => {
              setViewMode('list');
              setSelectedArticle(null);
            }}
            onEdit={handleArticleEdit}
            showActions={true}
          />
        )}

        {viewMode === 'edit' && (
          <ArticleEditor
            article={selectedArticle}
            categories={categories}
            tags={tags}
            onSave={handleArticleSave}
            onCancel={() => {
              setViewMode('list');
              setSelectedArticle(null);
            }}
            loading={articlesLoading}
          />
        )}

        {/* List View */}
        {viewMode === 'list' && (
          <>
            {/* Tabs */}
            <div className="mb-6 flex gap-4 border-b border-gray-200">
              <button
                onClick={() => setActiveTab('articles')}
                className={`pb-4 text-lg font-medium transition-colors ${
                  activeTab === 'articles'
                    ? 'border-b-2 border-blue-600 text-blue-600'
                    : 'text-gray-600 hover:text-gray-900'
                }`}
              >
                Articles
              </button>
              <button
                onClick={() => setActiveTab('categories')}
                className={`pb-4 text-lg font-medium transition-colors ${
                  activeTab === 'categories'
                    ? 'border-b-2 border-blue-600 text-blue-600'
                    : 'text-gray-600 hover:text-gray-900'
                }`}
              >
                Categories
              </button>
              <button
                onClick={() => setActiveTab('tags')}
                className={`pb-4 text-lg font-medium transition-colors ${
                  activeTab === 'tags'
                    ? 'border-b-2 border-blue-600 text-blue-600'
                    : 'text-gray-600 hover:text-gray-900'
                }`}
              >
                Tags
              </button>
            </div>

            {/* Articles Tab */}
            {activeTab === 'articles' && (
              <div className="space-y-6">
                <div className="flex items-center justify-between">
                  <h2 className="text-2xl font-semibold text-gray-900">
                    Articles
                  </h2>
                  <button
                    onClick={() => handleArticleEdit()}
                    className="rounded-md bg-blue-600 px-4 py-2 text-white hover:bg-blue-700"
                  >
                    New Article
                  </button>
                </div>
                <ArticleList
                  articles={articles}
                  loading={articlesLoading}
                  error={articlesError}
                  onArticleClick={handleArticleClick}
                  onEditClick={handleArticleEdit}
                  onDeleteClick={handleArticleDelete}
                  showActions={true}
                />
              </div>
            )}

            {/* Categories Tab */}
            {activeTab === 'categories' && (
              <div className="space-y-6">
                <div className="flex items-center justify-between">
                  <h2 className="text-2xl font-semibold text-gray-900">
                    Categories
                  </h2>
                  <button
                    onClick={() => handleCategoryEdit()}
                    className="rounded-md bg-blue-600 px-4 py-2 text-white hover:bg-blue-700"
                  >
                    New Category
                  </button>
                </div>
                <CategoryList
                  categories={categories}
                  loading={categoriesLoading}
                  error={categoriesError}
                  onEditClick={handleCategoryEdit}
                  onDeleteClick={handleCategoryDelete}
                  showActions={true}
                />
              </div>
            )}

            {/* Tags Tab */}
            {activeTab === 'tags' && (
              <div className="space-y-6">
                <div className="flex items-center justify-between">
                  <h2 className="text-2xl font-semibold text-gray-900">
                    Tags
                  </h2>
                  <button
                    onClick={() => handleTagEdit()}
                    className="rounded-md bg-blue-600 px-4 py-2 text-white hover:bg-blue-700"
                  >
                    New Tag
                  </button>
                </div>
                <TagList
                  tags={tags}
                  loading={tagsLoading}
                  error={tagsError}
                  onEditClick={handleTagEdit}
                  onDeleteClick={handleTagDelete}
                  showActions={true}
                />
              </div>
            )}
          </>
        )}

        {/* Modals */}
        <CategoryModal
          category={selectedCategory}
          categories={categories}
          isOpen={categoryModalOpen}
          onClose={() => {
            setCategoryModalOpen(false);
            setSelectedCategory(null);
          }}
          onSave={handleCategorySave}
          loading={categoriesLoading}
        />

        <TagModal
          tag={selectedTag}
          isOpen={tagModalOpen}
          onClose={() => {
            setTagModalOpen(false);
            setSelectedTag(null);
          }}
          onSave={handleTagSave}
          loading={tagsLoading}
        />
      </div>
    </div>
  );
}

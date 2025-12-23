import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useArticles, ArticleList, type Article } from 'nnews-react';
import { Plus, ChevronLeft, ChevronRight } from 'lucide-react';
import { toast } from 'sonner';

export function ArticleListPage() {
  const navigate = useNavigate();
  const [currentPage, setCurrentPage] = useState(1);
  const ITEMS_PER_PAGE = 10;

  const {
    articles,
    loading,
    error,
    fetchArticles,
    deleteArticle,
  } = useArticles();

  // Fetch articles when page changes
  useEffect(() => {
    fetchArticles({ page: currentPage, pageSize: ITEMS_PER_PAGE });
  }, [currentPage]);

  const handleEditClick = (article: Article) => {
    navigate(`/articles/edit/${article.articleId}`);
  };

  const handleDeleteClick = async (article: Article) => {
    if (window.confirm(`Are you sure you want to delete "${article.title}"?`)) {
      try {
        await deleteArticle(article.articleId);
        toast.success('Article deleted successfully');
        fetchArticles({ page: currentPage, pageSize: ITEMS_PER_PAGE });
      } catch (error) {
        toast.error('Failed to delete article');
        console.error('Delete error:', error);
      }
    }
  };

  const handleArticleClick = (article: Article) => {
    navigate(`/articles/edit/${article.articleId}`);
  };

  const handleNewArticle = () => {
    navigate('/articles/new');
  };

  const handlePageChange = (newPage: number) => {
    setCurrentPage(newPage);
    fetchArticles({ page: newPage, pageSize: ITEMS_PER_PAGE });
  };

  const totalPages = articles?.totalPages || 0;

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="mb-6 flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900 dark:text-gray-100">Articles</h1>
          <p className="mt-2 text-gray-600 dark:text-gray-400">
            Manage your articles
          </p>
        </div>
        <button
          onClick={handleNewArticle}
          className="flex items-center gap-2 rounded-lg bg-blue-600 px-4 py-2 text-white hover:bg-blue-700 transition-colors"
        >
          <Plus className="h-5 w-5" />
          New Article
        </button>
      </div>

      {error && (
        <div className="mb-4 rounded-lg bg-red-50 dark:bg-red-900/20 p-4">
          <p className="text-sm text-red-800 dark:text-red-200">
            Error: {error.message}
          </p>
        </div>
      )}

      <div className="rounded-lg bg-white dark:bg-gray-800 shadow">
        <ArticleList
          articles={articles}
          loading={loading}
          error={error}
          onArticleClick={handleArticleClick}
          onEditClick={handleEditClick}
          onDeleteClick={handleDeleteClick}
          showActions={true}
          emptyMessage="No articles found. Create your first article!"
        />

        {articles && articles.totalCount > 0 && (
          <div className="border-t border-gray-200 dark:border-gray-700 px-6 py-4">
            <div className="flex items-center justify-between">
              <div className="text-sm text-gray-700 dark:text-gray-300">
                Showing page {currentPage} of {totalPages} ({articles.totalCount} total articles)
              </div>
              <div className="flex gap-2">
                <button
                  onClick={() => handlePageChange(currentPage - 1)}
                  disabled={!articles.hasPrevious}
                  className="flex items-center gap-1 rounded-lg border border-gray-300 dark:border-gray-600 px-3 py-2 text-sm font-medium text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
                >
                  <ChevronLeft className="h-4 w-4" />
                  Previous
                </button>
                <button
                  onClick={() => handlePageChange(currentPage + 1)}
                  disabled={!articles.hasNext}
                  className="flex items-center gap-1 rounded-lg border border-gray-300 dark:border-gray-600 px-3 py-2 text-sm font-medium text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
                >
                  Next
                  <ChevronRight className="h-4 w-4" />
                </button>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}

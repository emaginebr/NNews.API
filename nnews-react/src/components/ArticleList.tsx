import { ArticleStatus, type Article, type PagedResult } from '../types/news';

export interface ArticleListProps {
  articles: PagedResult<Article> | null;
  loading?: boolean;
  error?: Error | null;
  onArticleClick?: (article: Article) => void;
  onEditClick?: (article: Article) => void;
  onDeleteClick?: (article: Article) => void;
  showActions?: boolean;
  emptyMessage?: string;
}

export function ArticleList({
  articles,
  loading = false,
  error = null,
  onArticleClick,
  onEditClick,
  onDeleteClick,
  showActions = false,
  emptyMessage = 'No articles found',
}: ArticleListProps) {
  if (loading) {
    return (
      <div className="flex items-center justify-center p-8">
        <div className="text-gray-500">Loading articles...</div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="rounded-md bg-red-50 p-4">
        <div className="text-sm text-red-800">
          Error loading articles: {error.message}
        </div>
      </div>
    );
  }

  if (!articles || articles.items.length === 0) {
    return (
      <div className="flex items-center justify-center p-8">
        <div className="text-gray-500">{emptyMessage}</div>
      </div>
    );
  }

  return (
    <div className="space-y-4">
      {articles.items.map((article) => (
        <div
          key={article.articleId}
          className="rounded-lg border border-gray-200 bg-white p-6 shadow-sm hover:shadow-md transition-shadow"
        >
          <div className="flex items-start justify-between">
            <div className="flex-1">
              <h3
                className={`text-xl font-semibold text-gray-900 ${
                  onArticleClick ? 'cursor-pointer hover:text-blue-600' : ''
                }`}
                onClick={() => onArticleClick?.(article)}
              >
                {article.title}
              </h3>

              <div className="mt-4 flex flex-wrap items-center gap-4 text-sm text-gray-500">
                {article.dateAt && (
                  <span className="flex items-center gap-1">
                    <svg
                      className="h-4 w-4"
                      fill="none"
                      stroke="currentColor"
                      viewBox="0 0 24 24"
                    >
                      <path
                        strokeLinecap="round"
                        strokeLinejoin="round"
                        strokeWidth={2}
                        d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"
                      />
                    </svg>
                    {new Date(article.dateAt).toLocaleDateString()}
                  </span>
                )}

                {article.category && (
                  <span className="rounded-full bg-blue-100 px-3 py-1 text-xs font-medium text-blue-800">
                    {article.category.title}
                  </span>
                )}

                <span
                  className={`rounded-full px-3 py-1 text-xs font-medium ${
                    article.status === ArticleStatus.Published
                      ? 'bg-green-100 text-green-800'
                      : article.status === ArticleStatus.Draft
                      ? 'bg-gray-100 text-gray-800'
                      : article.status === ArticleStatus.Review
                      ? 'bg-yellow-100 text-yellow-800'
                      : article.status === ArticleStatus.Scheduled
                      ? 'bg-purple-100 text-purple-800'
                      : 'bg-orange-100 text-orange-800'
                  }`}
                >
                  {ArticleStatus[article.status]}
                </span>
              </div>

              {article.tags && article.tags.length > 0 && (
                <div className="mt-3 flex flex-wrap gap-2">
                  {article.tags.map((tag) => (
                    <span
                      key={tag.tagId}
                      className="rounded-md bg-gray-100 px-2 py-1 text-xs text-gray-700"
                    >
                      #{tag.title}
                    </span>
                  ))}
                </div>
              )}

              {article.roles && article.roles.length > 0 && (
                <div className="mt-3 flex flex-wrap gap-2">
                  {article.roles.map((role) => (
                    <span
                      key={role.slug}
                      className="rounded-md bg-purple-100 px-2 py-1 text-xs text-purple-700"
                    >
                      {role.name}
                    </span>
                  ))}
                </div>
              )}
            </div>

            {showActions && (onEditClick || onDeleteClick) && (
              <div className="ml-4 flex gap-2">
                {onEditClick && (
                  <button
                    onClick={() => onEditClick(article)}
                    className="rounded p-2 text-blue-600 hover:bg-blue-50"
                    title="Edit"
                  >
                    <svg
                      className="h-5 w-5"
                      fill="none"
                      stroke="currentColor"
                      viewBox="0 0 24 24"
                    >
                      <path
                        strokeLinecap="round"
                        strokeLinejoin="round"
                        strokeWidth={2}
                        d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"
                      />
                    </svg>
                  </button>
                )}
                {onDeleteClick && (
                  <button
                    onClick={() => onDeleteClick(article)}
                    className="rounded p-2 text-red-600 hover:bg-red-50"
                    title="Delete"
                  >
                    <svg
                      className="h-5 w-5"
                      fill="none"
                      stroke="currentColor"
                      viewBox="0 0 24 24"
                    >
                      <path
                        strokeLinecap="round"
                        strokeLinejoin="round"
                        strokeWidth={2}
                        d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"
                      />
                    </svg>
                  </button>
                )}
              </div>
            )}
          </div>
        </div>
      ))}

      {articles.totalPages > 1 && (
        <div className="mt-6 flex items-center justify-between border-t border-gray-200 pt-4">
          <div className="text-sm text-gray-700">
            Showing {articles.items.length} of {articles.totalCount} articles
            (Page {articles.page} of {articles.totalPages})
          </div>
        </div>
      )}
    </div>
  );
}

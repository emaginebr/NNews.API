import ReactMarkdown from 'react-markdown';
import remarkGfm from 'remark-gfm';
import rehypeHighlight from 'rehype-highlight';
import { ArticleStatus, type Article } from '../types/news';
import 'highlight.js/styles/github.css';

export interface ArticleViewerProps {
  article: Article;
  onBack?: () => void;
  onEdit?: (article: Article) => void;
  showActions?: boolean;
}

export function ArticleViewer({
  article,
  onBack,
  onEdit,
  showActions = false,
}: ArticleViewerProps) {
  return (
    <div className="mx-auto max-w-4xl">
      {/* Header Actions */}
      {(onBack || (showActions && onEdit)) && (
        <div className="mb-6 flex items-center justify-between">
          {onBack && (
            <button
              onClick={onBack}
              className="flex items-center gap-2 text-gray-600 hover:text-gray-900"
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
                  d="M15 19l-7-7 7-7"
                />
              </svg>
              Back
            </button>
          )}

          {showActions && onEdit && (
            <button
              onClick={() => onEdit(article)}
              className="flex items-center gap-2 rounded-md bg-blue-600 px-4 py-2 text-white hover:bg-blue-700"
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
              Edit
            </button>
          )}
        </div>
      )}

      {/* Article Header */}
      <article className="rounded-lg bg-white p-8 shadow-md">
        <header className="border-b border-gray-200 pb-6">
          <h1 className="text-4xl font-bold text-gray-900">{article.title}</h1>

          {/* Meta Information */}
          <div className="mt-6 flex flex-wrap items-center gap-4 text-sm text-gray-500">
            {article.dateAt && (
              <span className="flex items-center gap-2">
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
                    d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"
                  />
                </svg>
                {new Date(article.dateAt).toLocaleDateString('en-US', {
                  year: 'numeric',
                  month: 'long',
                  day: 'numeric',
                })}
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

          {/* Tags */}
          {article.tags && article.tags.length > 0 && (
            <div className="mt-4 flex flex-wrap gap-2">
              {article.tags.map((tag) => (
                <span
                  key={tag.tagId}
                  className="rounded-md bg-gray-100 px-3 py-1 text-sm text-gray-700"
                >
                  #{tag.title}
                </span>
              ))}
            </div>
          )}

          {/* Roles */}
          {article.roles && article.roles.length > 0 && (
            <div className="mt-4 flex flex-wrap gap-2">
              {article.roles.map((role) => (
                <span
                  key={role.slug}
                  className="rounded-md bg-purple-100 px-3 py-1 text-sm text-purple-700"
                >
                  {role.name}
                </span>
              ))}
            </div>
          )}
        </header>

        {/* Article Content */}
        <div className="prose prose-lg mt-8 max-w-none">
          <ReactMarkdown
            remarkPlugins={[remarkGfm]}
            rehypePlugins={[rehypeHighlight]}
          >
            {article.content}
          </ReactMarkdown>
        </div>
      </article>
    </div>
  );
}

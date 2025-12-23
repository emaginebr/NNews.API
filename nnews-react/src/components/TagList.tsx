import type { Tag } from '../types/news';

export interface TagListProps {
  tags: Tag[];
  loading?: boolean;
  error?: Error | null;
  onTagClick?: (tag: Tag) => void;
  onEditClick?: (tag: Tag) => void;
  onDeleteClick?: (tag: Tag) => void;
  showActions?: boolean;
  emptyMessage?: string;
}

export function TagList({
  tags,
  loading = false,
  error = null,
  onTagClick,
  onEditClick,
  onDeleteClick,
  showActions = false,
  emptyMessage = 'No tags found',
}: TagListProps) {
  if (loading) {
    return (
      <div className="flex items-center justify-center p-8">
        <div className="text-gray-500">Loading tags...</div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="rounded-md bg-red-50 p-4">
        <div className="text-sm text-red-800">
          Error loading tags: {error.message}
        </div>
      </div>
    );
  }

  if (tags.length === 0) {
    return (
      <div className="flex items-center justify-center p-8">
        <div className="text-gray-500">{emptyMessage}</div>
      </div>
    );
  }

  return (
    <div className="flex flex-wrap gap-3">
      {tags.map((tag) => (
        <div
          key={tag.id}
          className="group relative flex items-center gap-2 rounded-lg border border-gray-200 bg-white px-4 py-2 shadow-sm hover:shadow-md transition-shadow"
        >
          <span
            className={`text-sm font-medium text-gray-900 ${
              onTagClick ? 'cursor-pointer hover:text-blue-600' : ''
            }`}
            onClick={() => onTagClick?.(tag)}
          >
            #{tag.title}
          </span>

          {tag.slug && (
            <span className="text-xs font-mono text-gray-400">({tag.slug})</span>
          )}

          {showActions && (onEditClick || onDeleteClick) && (
            <div className="flex gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
              {onEditClick && (
                <button
                  onClick={() => onEditClick(tag)}
                  className="rounded p-1 text-blue-600 hover:bg-blue-50"
                  title="Edit"
                >
                  <svg
                    className="h-3.5 w-3.5"
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
                  onClick={() => onDeleteClick(tag)}
                  className="rounded p-1 text-red-600 hover:bg-red-50"
                  title="Delete"
                >
                  <svg
                    className="h-3.5 w-3.5"
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
      ))}
    </div>
  );
}

import type { Tag } from '../types/news';
import { Edit2, Trash2, GitMerge } from 'lucide-react';

export interface TagListProps {
  tags: Tag[];
  loading?: boolean;
  error?: Error | null;
  onTagClick?: (tag: Tag) => void;
  onEditClick?: (tag: Tag) => void;
  onDeleteClick?: (tag: Tag) => void;
  onMergeClick?: (tag: Tag) => void;
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
  onMergeClick,
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
          key={tag.tagId}
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

          {tag.articleCount !== undefined && (
            <span className="text-xs text-gray-500 bg-gray-100 px-2 py-0.5 rounded-full">
              {tag.articleCount}
            </span>
          )}

          {showActions && (onEditClick || onDeleteClick || onMergeClick) && (
            <div className="flex gap-1 ml-2">
              {onEditClick && (
                <button
                  onClick={() => onEditClick(tag)}
                  className="rounded p-1 text-blue-600 hover:bg-blue-50 transition-colors"
                  title="Edit"
                >
                  <Edit2 className="h-4 w-4" />
                </button>
              )}
              {onMergeClick && (
                <button
                  onClick={() => onMergeClick(tag)}
                  className="rounded p-1 text-purple-600 hover:bg-purple-50 transition-colors"
                  title="Merge"
                >
                  <GitMerge className="h-4 w-4" />
                </button>
              )}
              {onDeleteClick && (
                <button
                  onClick={() => onDeleteClick(tag)}
                  className="rounded p-1 text-red-600 hover:bg-red-50 transition-colors"
                  title="Delete"
                >
                  <Trash2 className="h-4 w-4" />
                </button>
              )}
            </div>
          )}
        </div>
      ))}
    </div>
  );
}

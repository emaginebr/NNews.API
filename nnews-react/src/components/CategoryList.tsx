import type { Category } from '../types/news';

export interface CategoryListProps {
  categories: Category[];
  loading?: boolean;
  error?: Error | null;
  onCategoryClick?: (category: Category) => void;
  onEditClick?: (category: Category) => void;
  onDeleteClick?: (category: Category) => void;
  showActions?: boolean;
  emptyMessage?: string;
}

export function CategoryList({
  categories,
  loading = false,
  error = null,
  onCategoryClick,
  onEditClick,
  onDeleteClick,
  showActions = false,
  emptyMessage = 'No categories found',
}: CategoryListProps) {
  if (loading) {
    return (
      <div className="flex items-center justify-center p-8">
        <div className="text-gray-500">Loading categories...</div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="rounded-md bg-red-50 p-4">
        <div className="text-sm text-red-800">
          Error loading categories: {error.message}
        </div>
      </div>
    );
  }

  if (categories.length === 0) {
    return (
      <div className="flex items-center justify-center p-8">
        <div className="text-gray-500">{emptyMessage}</div>
      </div>
    );
  }

  return (
    <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
      {categories.map((category) => (
        <div
          key={category.id}
          className="rounded-lg border border-gray-200 bg-white p-4 shadow-sm hover:shadow-md transition-shadow"
        >
          <div className="flex items-start justify-between">
            <div className="flex-1">
              <h3
                className={`text-lg font-semibold text-gray-900 ${
                  onCategoryClick ? 'cursor-pointer hover:text-blue-600' : ''
                }`}
                onClick={() => onCategoryClick?.(category)}
              >
                {category.title}
              </h3>

              {category.description && (
                <p className="mt-1 text-sm text-gray-600">{category.description}</p>
              )}

              {category.slug && (
                <p className="mt-2 text-xs font-mono text-gray-400">{category.slug}</p>
              )}

              {category.visibleToRoles && category.visibleToRoles.length > 0 && (
                <div className="mt-2 flex flex-wrap gap-1">
                  {category.visibleToRoles.map((role, index) => (
                    <span
                      key={index}
                      className="rounded-full bg-purple-100 px-2 py-0.5 text-xs font-medium text-purple-800"
                    >
                      {role}
                    </span>
                  ))}
                </div>
              )}
            </div>

            {showActions && (onEditClick || onDeleteClick) && (
              <div className="ml-2 flex gap-1">
                {onEditClick && (
                  <button
                    onClick={() => onEditClick(category)}
                    className="rounded p-1.5 text-blue-600 hover:bg-blue-50"
                    title="Edit"
                  >
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
                        d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"
                      />
                    </svg>
                  </button>
                )}
                {onDeleteClick && (
                  <button
                    onClick={() => onDeleteClick(category)}
                    className="rounded p-1.5 text-red-600 hover:bg-red-50"
                    title="Delete"
                  >
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
    </div>
  );
}

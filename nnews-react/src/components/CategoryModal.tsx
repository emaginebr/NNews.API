import React, { useState, useEffect } from 'react';
import type { Category, CategoryInput, CategoryUpdate } from '../types/news';

export interface CategoryModalProps {
  category?: Category | null;
  categories?: Category[];
  isOpen: boolean;
  onClose: () => void;
  onSave: (category: CategoryInput | CategoryUpdate) => Promise<void>;
  loading?: boolean;
}

export function CategoryModal({
  category,
  categories = [],
  isOpen,
  onClose,
  onSave,
  loading = false,
}: CategoryModalProps) {
  const [title, setTitle] = useState('');
  const [parentId, setParentId] = useState<number | null>(null);
  const [errors, setErrors] = useState<Record<string, string>>({});

  useEffect(() => {
    if (category) {
      setTitle(category.title);
      setParentId(category.parentId || null);
    } else {
      resetForm();
    }
  }, [category, isOpen]);

  const resetForm = () => {
    setTitle('');
    setParentId(null);
    setErrors({});
  };

  const validateForm = (): boolean => {
    const newErrors: Record<string, string> = {};

    if (!title.trim()) {
      newErrors.title = 'Title is required';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!validateForm()) {
      return;
    }

    const categoryData = {
      title: title.trim(),
      parentId: parentId || undefined,
    };

    try {
      if (category) {
        await onSave({ categoryId: category.categoryId, ...categoryData } as CategoryUpdate);
      } else {
        await onSave(categoryData as CategoryInput);
      }
      onClose();
    } catch (error) {
      console.error('Failed to save category:', error);
    }
  };

  if (!isOpen) return null;

  // Filter out the current category from parent options to prevent self-referencing
  const availableParents = categories.filter((c) => c.categoryId !== category?.categoryId);

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-50">
      <div className="w-full max-w-lg rounded-lg bg-white p-6 shadow-xl">
        <h2 className="mb-4 text-2xl font-bold text-gray-900">
          {category ? 'Edit Category' : 'Create Category'}
        </h2>

        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="space-y-2">
            <label htmlFor="title" className="block text-sm font-medium text-gray-700">
              Title *
            </label>
            <input
              id="title"
              type="text"
              value={title}
              onChange={(e) => setTitle(e.target.value)}
              className={`w-full rounded-md border ${
                errors.title ? 'border-red-300' : 'border-gray-300'
              } px-4 py-2 focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500`}
              placeholder="Enter category title"
            />
            {errors.title && <p className="text-sm text-red-600">{errors.title}</p>}
          </div>

          {availableParents.length > 0 && (
            <div className="space-y-2">
              <label htmlFor="parent" className="block text-sm font-medium text-gray-700">
                Parent Category
              </label>
              <select
                id="parent"
                value={parentId || ''}
                onChange={(e) => setParentId(e.target.value ? Number(e.target.value) : null)}
                className="w-full rounded-md border border-gray-300 px-4 py-2 focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
              >
                <option value="">None (Top Level)</option>
                {availableParents.map((cat) => (
                  <option key={cat.categoryId} value={cat.categoryId}>
                    {cat.title}
                  </option>
                ))}
              </select>
            </div>
          )}

          <div className="flex justify-end gap-3 pt-4">
            <button
              type="button"
              onClick={onClose}
              disabled={loading}
              className="rounded-md border border-gray-300 px-6 py-2 text-gray-700 hover:bg-gray-50 disabled:opacity-50"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={loading}
              className="rounded-md bg-blue-600 px-6 py-2 text-white hover:bg-blue-700 disabled:opacity-50"
            >
              {loading ? 'Saving...' : category ? 'Update' : 'Create'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

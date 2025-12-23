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
  const [slug, setSlug] = useState('');
  const [description, setDescription] = useState('');
  const [parentId, setParentId] = useState<number | null>(null);
  const [visibleToRoles, setVisibleToRoles] = useState<string>('');
  const [errors, setErrors] = useState<Record<string, string>>({});

  useEffect(() => {
    if (category) {
      setTitle(category.title);
      setSlug(category.slug || '');
      setDescription(category.description || '');
      setParentId(category.parentId || null);
      setVisibleToRoles(category.visibleToRoles?.join(', ') || '');
    } else {
      resetForm();
    }
  }, [category, isOpen]);

  const resetForm = () => {
    setTitle('');
    setSlug('');
    setDescription('');
    setParentId(null);
    setVisibleToRoles('');
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

    const roles = visibleToRoles
      .split(',')
      .map((r) => r.trim())
      .filter((r) => r.length > 0);

    const categoryData = {
      title: title.trim(),
      slug: slug.trim() || undefined,
      description: description.trim() || undefined,
      parentId: parentId || undefined,
      visibleToRoles: roles.length > 0 ? roles : undefined,
    };

    try {
      if (category) {
        await onSave({ id: category.id, ...categoryData } as CategoryUpdate);
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
  const availableParents = categories.filter((c) => c.id !== category?.id);

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-50">
      <div className="w-full max-w-lg rounded-lg bg-white p-6 shadow-xl">
        <h2 className="mb-4 text-2xl font-bold text-gray-900">
          {category ? 'Edit Category' : 'Create Category'}
        </h2>

        <form onSubmit={handleSubmit} className="space-y-4">
          {/* Title */}
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

          {/* Slug */}
          <div className="space-y-2">
            <label htmlFor="slug" className="block text-sm font-medium text-gray-700">
              Slug
            </label>
            <input
              id="slug"
              type="text"
              value={slug}
              onChange={(e) => setSlug(e.target.value)}
              className="w-full rounded-md border border-gray-300 px-4 py-2 focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
              placeholder="category-slug (optional)"
            />
            <p className="text-xs text-gray-500">
              URL-friendly version of the title. Leave empty to auto-generate.
            </p>
          </div>

          {/* Description */}
          <div className="space-y-2">
            <label htmlFor="description" className="block text-sm font-medium text-gray-700">
              Description
            </label>
            <textarea
              id="description"
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              rows={3}
              className="w-full rounded-md border border-gray-300 px-4 py-2 focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
              placeholder="Brief description (optional)"
            />
          </div>

          {/* Parent Category */}
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
                  <option key={cat.id} value={cat.id}>
                    {cat.title}
                  </option>
                ))}
              </select>
            </div>
          )}

          {/* Visible to Roles */}
          <div className="space-y-2">
            <label htmlFor="roles" className="block text-sm font-medium text-gray-700">
              Visible to Roles
            </label>
            <input
              id="roles"
              type="text"
              value={visibleToRoles}
              onChange={(e) => setVisibleToRoles(e.target.value)}
              className="w-full rounded-md border border-gray-300 px-4 py-2 focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
              placeholder="Admin, Editor (comma-separated)"
            />
            <p className="text-xs text-gray-500">
              Leave empty for public access. Comma-separated list of roles.
            </p>
          </div>

          {/* Action Buttons */}
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

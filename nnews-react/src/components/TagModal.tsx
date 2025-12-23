import React, { useState, useEffect } from 'react';
import type { Tag, TagInput, TagUpdate } from '../types/news';

export interface TagModalProps {
  tag?: Tag | null;
  isOpen: boolean;
  onClose: () => void;
  onSave: (tag: TagInput | TagUpdate) => Promise<void>;
  loading?: boolean;
}

export function TagModal({
  tag,
  isOpen,
  onClose,
  onSave,
  loading = false,
}: TagModalProps) {
  const [title, setTitle] = useState('');
  const [slug, setSlug] = useState('');
  const [errors, setErrors] = useState<Record<string, string>>({});

  useEffect(() => {
    if (tag) {
      setTitle(tag.title);
      setSlug(tag.slug || '');
    } else {
      resetForm();
    }
  }, [tag, isOpen]);

  const resetForm = () => {
    setTitle('');
    setSlug('');
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

    const tagData = {
      title: title.trim(),
      slug: slug.trim() || undefined,
    };

    try {
      if (tag) {
        await onSave({ tagId: tag.tagId, ...tagData } as TagUpdate);
      } else {
        await onSave(tagData as TagInput);
      }
      onClose();
    } catch (error) {
      console.error('Failed to save tag:', error);
    }
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-50">
      <div className="w-full max-w-md rounded-lg bg-white p-6 shadow-xl">
        <h2 className="mb-4 text-2xl font-bold text-gray-900">
          {tag ? 'Edit Tag' : 'Create Tag'}
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
              placeholder="Enter tag title"
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
              placeholder="tag-slug (optional)"
            />
            <p className="text-xs text-gray-500">
              URL-friendly version of the title. Leave empty to auto-generate.
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
              {loading ? 'Saving...' : tag ? 'Update' : 'Create'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

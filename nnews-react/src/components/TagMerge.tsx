import React, { useState, useEffect } from 'react';
import type { Tag } from '../types/news';

export interface TagMergeProps {
  sourceTag: Tag;
  availableTags: Tag[];
  isOpen: boolean;
  onClose: () => void;
  onMerge: (sourceId: number, targetId: number) => Promise<void>;
  loading?: boolean;
}

export function TagMerge({
  sourceTag,
  availableTags,
  isOpen,
  onClose,
  onMerge,
  loading = false,
}: TagMergeProps) {
  const [targetTagId, setTargetTagId] = useState<number | null>(null);
  const [errors, setErrors] = useState<Record<string, string>>({});

  // Filter out the source tag from available tags
  const selectableTags = availableTags.filter(
    (tag) => tag.tagId !== sourceTag.tagId
  );

  useEffect(() => {
    if (!isOpen) {
      setTargetTagId(null);
      setErrors({});
    }
  }, [isOpen]);

  const validateForm = (): boolean => {
    const newErrors: Record<string, string> = {};

    if (!targetTagId) {
      newErrors.targetTag = 'Please select a target tag';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!validateForm() || !targetTagId) {
      return;
    }

    try {
      await onMerge(sourceTag.tagId || 0, targetTagId);
      onClose();
    } catch (error) {
      console.error('Failed to merge tags:', error);
    }
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-50">
      <div className="w-full max-w-md rounded-lg bg-white p-6 shadow-xl">
        <h2 className="mb-4 text-2xl font-bold text-gray-900">Merge Tags</h2>

        <div className="mb-6 rounded-md bg-blue-50 p-4">
          <p className="text-sm text-blue-800">
            All articles tagged with{' '}
            <strong className="font-semibold">#{sourceTag.title}</strong> will be
            moved to the selected target tag, and the source tag will be deleted.
          </p>
        </div>

        <form onSubmit={handleSubmit} className="space-y-4">
          {/* Source Tag (Read-only) */}
          <div className="space-y-2">
            <label className="block text-sm font-medium text-gray-700">
              Source Tag
            </label>
            <div className="rounded-md border border-gray-300 bg-gray-50 px-4 py-2">
              <span className="font-medium text-gray-900">#{sourceTag.title}</span>
              {sourceTag.slug && (
                <span className="ml-2 text-xs font-mono text-gray-500">
                  ({sourceTag.slug})
                </span>
              )}
            </div>
          </div>

          {/* Target Tag Selection */}
          <div className="space-y-2">
            <label htmlFor="targetTag" className="block text-sm font-medium text-gray-700">
              Target Tag *
            </label>
            <select
              id="targetTag"
              value={targetTagId || ''}
              onChange={(e) => setTargetTagId(Number(e.target.value))}
              className={`w-full rounded-md border ${
                errors.targetTag ? 'border-red-300' : 'border-gray-300'
              } px-4 py-2 focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500`}
            >
              <option value="">Select target tag</option>
              {selectableTags.map((tag) => (
                <option key={tag.tagId} value={tag.tagId}>
                  #{tag.title} {tag.slug ? `(${tag.slug})` : ''}
                </option>
              ))}
            </select>
            {errors.targetTag && (
              <p className="text-sm text-red-600">{errors.targetTag}</p>
            )}
            <p className="text-xs text-gray-500">
              Select the tag that will receive all articles from the source tag
            </p>
          </div>

          {selectableTags.length === 0 && (
            <div className="rounded-md bg-yellow-50 p-4">
              <p className="text-sm text-yellow-800">
                No other tags available. You need at least 2 tags to perform a merge.
              </p>
            </div>
          )}

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
              disabled={loading || selectableTags.length === 0}
              className="rounded-md bg-blue-600 px-6 py-2 text-white hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {loading ? 'Merging...' : 'Merge Tags'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

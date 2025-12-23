import React, { useState, useEffect } from 'react';
import { MarkdownEditor } from './MarkdownEditor';
import { ArticleStatus } from '../types/news';
import type { Article, ArticleInput, ArticleUpdate, Category, Tag } from '../types/news';

export interface ArticleEditorProps {
  article?: Article | null;
  categories?: Category[];
  tags?: Tag[];
  onSave: (article: ArticleInput | ArticleUpdate) => Promise<void>;
  onCancel: () => void;
  loading?: boolean;
}

export function ArticleEditor({
  article,
  categories = [],
  tags = [],
  onSave,
  onCancel,
  loading = false,
}: ArticleEditorProps) {
  const [title, setTitle] = useState('');
  const [subtitle, setSubtitle] = useState('');
  const [excerpt, setExcerpt] = useState('');
  const [content, setContent] = useState('');
  const [status, setStatus] = useState<ArticleStatus>(ArticleStatus.Draft);
  const [author, setAuthor] = useState('');
  const [categoryId, setCategoryId] = useState<number | null>(null);
  const [selectedTagIds, setSelectedTagIds] = useState<number[]>([]);
  const [visibleToRoles, setVisibleToRoles] = useState<string>('');
  const [errors, setErrors] = useState<Record<string, string>>({});

  useEffect(() => {
    if (article) {
      setTitle(article.title);
      setSubtitle(article.subtitle || '');
      setExcerpt(article.excerpt || '');
      setContent(article.content);
      setStatus(article.status);
      setAuthor(article.author || '');
      setCategoryId(article.category?.id || null);
      setSelectedTagIds(article.tags?.map((t) => t.id) || []);
      setVisibleToRoles(article.visibleToRoles?.join(', ') || '');
    }
  }, [article]);

  const validateForm = (): boolean => {
    const newErrors: Record<string, string> = {};

    if (!title.trim()) {
      newErrors.title = 'Title is required';
    }

    if (!content.trim()) {
      newErrors.content = 'Content is required';
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

    const articleData = {
      title: title.trim(),
      subtitle: subtitle.trim() || undefined,
      excerpt: excerpt.trim() || undefined,
      content: content.trim(),
      status,
      author: author.trim() || undefined,
      categoryId: categoryId || undefined,
      tagIds: selectedTagIds.length > 0 ? selectedTagIds : undefined,
      visibleToRoles: roles.length > 0 ? roles : undefined,
    };

    if (article) {
      await onSave({ id: article.id, ...articleData } as ArticleUpdate);
    } else {
      await onSave(articleData as ArticleInput);
    }
  };

  const handleTagToggle = (tagId: number) => {
    setSelectedTagIds((prev) =>
      prev.includes(tagId)
        ? prev.filter((id) => id !== tagId)
        : [...prev, tagId]
    );
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-6">
      <div className="rounded-lg bg-white p-6 shadow-md">
        <h2 className="mb-6 text-2xl font-bold text-gray-900">
          {article ? 'Edit Article' : 'Create New Article'}
        </h2>

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
            placeholder="Enter article title"
          />
          {errors.title && <p className="text-sm text-red-600">{errors.title}</p>}
        </div>

        {/* Subtitle */}
        <div className="mt-4 space-y-2">
          <label htmlFor="subtitle" className="block text-sm font-medium text-gray-700">
            Subtitle
          </label>
          <input
            id="subtitle"
            type="text"
            value={subtitle}
            onChange={(e) => setSubtitle(e.target.value)}
            className="w-full rounded-md border border-gray-300 px-4 py-2 focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
            placeholder="Enter article subtitle (optional)"
          />
        </div>

        {/* Excerpt */}
        <div className="mt-4 space-y-2">
          <label htmlFor="excerpt" className="block text-sm font-medium text-gray-700">
            Excerpt
          </label>
          <textarea
            id="excerpt"
            value={excerpt}
            onChange={(e) => setExcerpt(e.target.value)}
            rows={3}
            className="w-full rounded-md border border-gray-300 px-4 py-2 focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
            placeholder="Brief summary of the article (optional)"
          />
        </div>

        {/* Content */}
        <div className="mt-4">
          <MarkdownEditor
            label="Content *"
            value={content}
            onChange={setContent}
            error={errors.content}
            placeholder="Write your article content in Markdown..."
          />
        </div>

        {/* Author */}
        <div className="mt-4 space-y-2">
          <label htmlFor="author" className="block text-sm font-medium text-gray-700">
            Author
          </label>
          <input
            id="author"
            type="text"
            value={author}
            onChange={(e) => setAuthor(e.target.value)}
            className="w-full rounded-md border border-gray-300 px-4 py-2 focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
            placeholder="Enter author name (optional)"
          />
        </div>

        {/* Status and Category Row */}
        <div className="mt-4 grid grid-cols-1 gap-4 md:grid-cols-2">
          {/* Status */}
          <div className="space-y-2">
            <label htmlFor="status" className="block text-sm font-medium text-gray-700">
              Status
            </label>
            <select
              id="status"
              value={status}
              onChange={(e) => setStatus(Number(e.target.value) as ArticleStatus)}
              className="w-full rounded-md border border-gray-300 px-4 py-2 focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
            >
              <option value={ArticleStatus.Draft}>Draft</option>
              <option value={ArticleStatus.Published}>Published</option>
              <option value={ArticleStatus.Archived}>Archived</option>
            </select>
          </div>

          {/* Category */}
          <div className="space-y-2">
            <label htmlFor="category" className="block text-sm font-medium text-gray-700">
              Category
            </label>
            <select
              id="category"
              value={categoryId || ''}
              onChange={(e) => setCategoryId(e.target.value ? Number(e.target.value) : null)}
              className="w-full rounded-md border border-gray-300 px-4 py-2 focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
            >
              <option value="">No Category</option>
              {categories.map((cat) => (
                <option key={cat.id} value={cat.id}>
                  {cat.title}
                </option>
              ))}
            </select>
          </div>
        </div>

        {/* Tags */}
        {tags.length > 0 && (
          <div className="mt-4 space-y-2">
            <label className="block text-sm font-medium text-gray-700">Tags</label>
            <div className="flex flex-wrap gap-2">
              {tags.map((tag) => (
                <button
                  key={tag.id}
                  type="button"
                  onClick={() => handleTagToggle(tag.id)}
                  className={`rounded-md px-3 py-1 text-sm font-medium transition-colors ${
                    selectedTagIds.includes(tag.id)
                      ? 'bg-blue-600 text-white'
                      : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
                  }`}
                >
                  #{tag.title}
                </button>
              ))}
            </div>
          </div>
        )}

        {/* Visible to Roles */}
        <div className="mt-4 space-y-2">
          <label htmlFor="roles" className="block text-sm font-medium text-gray-700">
            Visible to Roles
          </label>
          <input
            id="roles"
            type="text"
            value={visibleToRoles}
            onChange={(e) => setVisibleToRoles(e.target.value)}
            className="w-full rounded-md border border-gray-300 px-4 py-2 focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
            placeholder="Enter roles separated by commas (e.g., Admin, Editor)"
          />
          <p className="text-xs text-gray-500">
            Leave empty for public access. Comma-separated list of roles.
          </p>
        </div>

        {/* Action Buttons */}
        <div className="mt-6 flex justify-end gap-3">
          <button
            type="button"
            onClick={onCancel}
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
            {loading ? 'Saving...' : article ? 'Update Article' : 'Create Article'}
          </button>
        </div>
      </div>
    </form>
  );
}

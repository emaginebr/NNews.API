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
    const [content, setContent] = useState('');
    const [status, setStatus] = useState<ArticleStatus>(ArticleStatus.Draft);
    const [categoryId, setCategoryId] = useState<number | null>(null);
    const [dateAt, setDateAt] = useState<string>('');
    const [selectedTagIds, setSelectedTagIds] = useState<number[]>([]);
    const [roleIds, setRoleIds] = useState<string>('');
    const [errors, setErrors] = useState<Record<string, string>>({});

    useEffect(() => {
        if (article) {
            setTitle(article.title);
            setContent(article.content);
            setStatus(article.status);
            setCategoryId(article.categoryId || null);
            setDateAt(article.dateAt ? new Date(article.dateAt).toISOString().slice(0, 16) : '');
            setSelectedTagIds(article.tags?.map((t) => t.tagId).filter((id): id is number => id !== undefined) || []);
            setRoleIds(article.roles?.map((r) => r.slug).join(', ') || '');
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

        const roles = roleIds
            .split(',')
            .map((r) => r.trim())
            .filter((r) => r.length > 0);

        const articleData = {
            title: title.trim(),
            content: content.trim(),
            status,
            categoryId: categoryId || undefined,
            dateAt: dateAt || undefined,
            tagIds: selectedTagIds.length > 0 ? selectedTagIds : undefined,
            roleIds: roles.length > 0 ? roles : undefined,
        };

        if (article) {
            await onSave({ articleId: article.articleId, ...articleData } as ArticleUpdate);
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
            {/* Title */}
            <div className="space-y-2">
                <label htmlFor="title" className="block text-sm font-medium text-gray-700 dark:text-gray-300">
                    Title *
                </label>
                <input
                    id="title"
                    type="text"
                    value={title}
                    onChange={(e) => setTitle(e.target.value)}
                    className={`w-full rounded-md border ${errors.title ? 'border-red-300 dark:border-red-600' : 'border-gray-300 dark:border-gray-600'
                        } px-4 py-2 bg-white dark:bg-gray-900 text-gray-900 dark:text-gray-100 focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500`}
                    placeholder="Enter article title"
                />
                {errors.title && <p className="text-sm text-red-600 dark:text-red-400">{errors.title}</p>}
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

            {/* Date */}
            <div className="mt-4 space-y-2">
                <label htmlFor="dateAt" className="block text-sm font-medium text-gray-700 dark:text-gray-300">
                    Publication Date
                </label>
                <input
                    id="dateAt"
                    type="datetime-local"
                    value={dateAt}
                    onChange={(e) => setDateAt(e.target.value)}
                    className="w-full rounded-md border border-gray-300 dark:border-gray-600 px-4 py-2 bg-white dark:bg-gray-900 text-gray-900 dark:text-gray-100 focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
                />
            </div>

            {/* Status and Category Row */}
            <div className="mt-4 grid grid-cols-1 gap-4 md:grid-cols-2">
                {/* Status */}
                <div className="space-y-2">
                    <label htmlFor="status" className="block text-sm font-medium text-gray-700 dark:text-gray-300">
                        Status
                    </label>
                    <select
                        id="status"
                        value={status}
                        onChange={(e) => setStatus(Number(e.target.value) as ArticleStatus)}
                        className="w-full rounded-md border border-gray-300 dark:border-gray-600 px-4 py-2 bg-white dark:bg-gray-900 text-gray-900 dark:text-gray-100 focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
                    >
                        <option value={ArticleStatus.Draft}>Draft</option>
                        <option value={ArticleStatus.Published}>Published</option>
                        <option value={ArticleStatus.Archived}>Archived</option>
                        <option value={ArticleStatus.Scheduled}>Scheduled</option>
                        <option value={ArticleStatus.Review}>Review</option>
                    </select>
                </div>

                {/* Category */}
                <div className="space-y-2">
                    <label htmlFor="category" className="block text-sm font-medium text-gray-700 dark:text-gray-300">
                        Category
                    </label>
                    <select
                        id="category"
                        value={categoryId || ''}
                        onChange={(e) => setCategoryId(e.target.value ? Number(e.target.value) : null)}
                        className="w-full rounded-md border border-gray-300 dark:border-gray-600 px-4 py-2 bg-white dark:bg-gray-900 text-gray-900 dark:text-gray-100 focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
                    >
                        <option value="">No Category</option>
                        {categories.map((cat) => (
                            <option key={cat.categoryId} value={cat.categoryId}>
                                {cat.title}
                            </option>
                        ))}
                    </select>
                </div>
            </div>

            {/* Tags */}
            {tags.length > 0 && (
                <div className="mt-4 space-y-2">
                    <label className="block text-sm font-medium text-gray-700 dark:text-gray-300">Tags</label>
                    <div className="flex flex-wrap gap-2">
                        {tags.map((tag) => (
                            <button
                                key={tag.tagId}
                                type="button"
                                onClick={() => handleTagToggle(tag.tagId || 0)}
                                className={`rounded-md px-3 py-1 text-sm font-medium transition-colors ${selectedTagIds.includes(tag.tagId || 0)
                                        ? 'bg-blue-600 text-white'
                                        : 'bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-300 hover:bg-gray-200 dark:hover:bg-gray-600'
                                    }`}
                            >
                                #{tag.title}
                            </button>
                        ))}
                    </div>
                </div>
            )}

            {/* Roles */}
            <div className="mt-4 space-y-2">
                <label htmlFor="roles" className="block text-sm font-medium text-gray-700 dark:text-gray-300">
                    Roles
                </label>
                <input
                    id="roles"
                    type="text"
                    value={roleIds}
                    onChange={(e) => setRoleIds(e.target.value)}
                    className="w-full rounded-md border border-gray-300 dark:border-gray-600 px-4 py-2 bg-white dark:bg-gray-900 text-gray-900 dark:text-gray-100 focus:border-blue-500 focus:outline-none focus:ring-1 focus:ring-blue-500"
                    placeholder="Enter role slugs separated by commas (e.g., admin, editor)"
                />
                <p className="text-xs text-gray-500 dark:text-gray-400">
                    Comma-separated list of role slugs.
                </p>
            </div>

            {/* Action Buttons */}
            <div className="mt-6 flex justify-end gap-3">
                <button
                    type="button"
                    onClick={onCancel}
                    disabled={loading}
                    className="rounded-md border border-gray-300 dark:border-gray-600 px-6 py-2 text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-700 disabled:opacity-50"
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
        </form>
    );
}

import { useState } from 'react';
import ReactMarkdown from 'react-markdown';
import remarkGfm from 'remark-gfm';
import rehypeHighlight from 'rehype-highlight';
import 'highlight.js/styles/github.css';

export interface MarkdownEditorProps {
  value: string;
  onChange: (value: string) => void;
  placeholder?: string;
  label?: string;
  error?: string;
  minHeight?: string;
}

export function MarkdownEditor({
  value,
  onChange,
  placeholder = 'Write your content in Markdown...',
  label,
  error,
  minHeight = '400px',
}: MarkdownEditorProps) {
  const [activeTab, setActiveTab] = useState<'edit' | 'preview'>('edit');

  return (
    <div className="space-y-2">
      {label && (
        <label className="block text-sm font-medium text-gray-700">
          {label}
        </label>
      )}

      <div className="overflow-hidden rounded-lg border border-gray-300 bg-white">
        {/* Tab Headers */}
        <div className="flex border-b border-gray-300 bg-gray-50">
          <button
            type="button"
            onClick={() => setActiveTab('edit')}
            className={`flex-1 px-4 py-2 text-sm font-medium ${
              activeTab === 'edit'
                ? 'border-b-2 border-blue-600 bg-white text-blue-600'
                : 'text-gray-600 hover:text-gray-900'
            }`}
          >
            Edit
          </button>
          <button
            type="button"
            onClick={() => setActiveTab('preview')}
            className={`flex-1 px-4 py-2 text-sm font-medium ${
              activeTab === 'preview'
                ? 'border-b-2 border-blue-600 bg-white text-blue-600'
                : 'text-gray-600 hover:text-gray-900'
            }`}
          >
            Preview
          </button>
        </div>

        {/* Content Area */}
        <div style={{ minHeight }}>
          {activeTab === 'edit' ? (
            <textarea
              value={value}
              onChange={(e) => onChange(e.target.value)}
              placeholder={placeholder}
              className="h-full w-full resize-none border-0 p-4 font-mono text-sm focus:outline-none focus:ring-0"
              style={{ minHeight }}
            />
          ) : (
            <div className="prose max-w-none p-4">
              {value ? (
                <ReactMarkdown
                  remarkPlugins={[remarkGfm]}
                  rehypePlugins={[rehypeHighlight]}
                >
                  {value}
                </ReactMarkdown>
              ) : (
                <div className="text-gray-400">Nothing to preview</div>
              )}
            </div>
          )}
        </div>
      </div>

      {error && <p className="text-sm text-red-600">{error}</p>}

      {/* Markdown Help */}
      <div className="rounded-md bg-gray-50 p-3 text-xs text-gray-600">
        <p className="font-medium">Markdown Syntax Help:</p>
        <div className="mt-1 grid grid-cols-2 gap-2">
          <div>
            <strong># Heading 1</strong> - Main heading
          </div>
          <div>
            <strong>## Heading 2</strong> - Subheading
          </div>
          <div>
            <strong>**bold text**</strong> - Bold
          </div>
          <div>
            <strong>*italic text*</strong> - Italic
          </div>
          <div>
            <strong>[link](url)</strong> - Link
          </div>
          <div>
            <strong>![alt](image.jpg)</strong> - Image
          </div>
          <div>
            <strong>- item</strong> - Bullet list
          </div>
          <div>
            <strong>1. item</strong> - Numbered list
          </div>
          <div>
            <strong>`code`</strong> - Inline code
          </div>
          <div>
            <strong>```language```</strong> - Code block
          </div>
        </div>
      </div>
    </div>
  );
}

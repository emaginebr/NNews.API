# AI Features Migration Guide - nnews-react v2.0.0

## Quick Start

### 1. Update Package
```bash
npm install nnews-react@2.0.0
# or
yarn add nnews-react@2.0.0
```

### 2. Update Imports
```typescript
import {
  ArticleEditor,
  AIArticleGenerator,  // NEW
  AIArticleRequest,    // NEW
  tagsToString,        // NEW
  validatePrompt,      // NEW
} from 'nnews-react';
```

---

## Breaking Changes

### Tags: Array → String

**Before (v1.x):**
```typescript
const article: ArticleInput = {
  title: "My Article",
  content: "<p>Content</p>",
  categoryId: 1,
  tagIds: [1, 2, 3],  // ❌ REMOVED
  status: ArticleStatus.Draft,
};
```

**After (v2.0):**
```typescript
const article: ArticleInput = {
  title: "My Article",
  content: "<p>Content</p>",
  categoryId: 1,
  tagList: "AI, Technology, Innovation",  // ✅ NEW
  status: ArticleStatus.Draft,
};
```

---

## New Features

### 1. Create Article with AI

```typescript
import { useNNews } from 'nnews-react';

function CreateWithAI() {
  const { articleApi } = useNNews();
  
  const handleCreate = async () => {
    const article = await articleApi.createArticleWithAI({
      prompt: "Write a comprehensive article about React Server Components with practical examples and best practices",
      generateImage: true,
      categoryId: 1,
      roles: ["public"],
      status: ArticleStatus.Draft
    });
    
    console.log('Created:', article);
    // Article includes:
    // - AI-generated title
    // - HTML content
    // - Suggested tags
    // - DALL-E 3 image (if requested)
  };
  
  return <button onClick={handleCreate}>Create with AI</button>;
}
```

### 2. Update Article with AI

```typescript
const handleUpdate = async (articleId: number) => {
  const updated = await articleApi.updateArticleWithAI({
    articleId: articleId,
    prompt: "Add a troubleshooting section at the end and update all code examples to use TypeScript",
    generateImage: false  // Keep existing image
  });
  
  console.log('Updated:', updated);
  // AI receives current article as context
  // and applies requested changes
};
```

### 3. AIArticleGenerator Component

```typescript
import { AIArticleGenerator } from 'nnews-react';

function ArticleManager() {
  const [mode, setMode] = useState<'manual' | 'ai'>('manual');
  
  if (mode === 'ai') {
    return (
      <AIArticleGenerator
        mode="create"
        categories={categories}
        onSuccess={(article) => {
          console.log('AI created:', article);
          navigate(`/articles/${article.articleId}`);
        }}
        onCancel={() => setMode('manual')}
      />
    );
  }
  
  return <ArticleEditor {...props} />;
}
```

---

## Common Use Cases

### Use Case 1: Dual Mode Editor (Manual + AI)

```typescript
function DualModeEditor() {
  const [editorMode, setEditorMode] = useState<'manual' | 'ai'>('manual');
  const [article, setArticle] = useState<Article | null>(null);
  
  return (
    <div>
      <div className="mode-toggle">
        <button onClick={() => setEditorMode('manual')}>
          Manual Editor
        </button>
        <button onClick={() => setEditorMode('ai')}>
          AI Generator
        </button>
      </div>
      
      {editorMode === 'manual' ? (
        <ArticleEditor
          article={article}
          categories={categories}
          onSave={handleSave}
          onCancel={handleCancel}
        />
      ) : (
        <AIArticleGenerator
          mode={article ? 'update' : 'create'}
          articleId={article?.articleId}
          categories={categories}
          onSuccess={(result) => setArticle(result)}
        />
      )}
    </div>
  );
}
```

### Use Case 2: Convert Existing Article Tags

```typescript
import { tagsToString } from 'nnews-react';

function EditArticle({ articleId }: { articleId: number }) {
  const { articleApi } = useNNews();
  const [tagList, setTagList] = useState('');
  
  useEffect(() => {
    const loadArticle = async () => {
      const article = await articleApi.getArticleById(articleId);
      
      // Convert tags array to string
      setTagList(tagsToString(article.tags || []));
      // Result: "AI, Technology, Innovation"
    };
    
    loadArticle();
  }, [articleId]);
  
  return (
    <input
      value={tagList}
      onChange={(e) => setTagList(e.target.value)}
      placeholder="AI, Technology, Innovation"
    />
  );
}
```

### Use Case 3: Validate Before Submit

```typescript
import { validatePrompt, validateTagList } from 'nnews-react';

function AIForm() {
  const [prompt, setPrompt] = useState('');
  const [tagList, setTagList] = useState('');
  const [errors, setErrors] = useState<Record<string, string>>({});
  
  const handleSubmit = async () => {
    const newErrors: Record<string, string> = {};
    
    // Validate prompt
    const promptResult = validatePrompt(prompt);
    if (!promptResult.valid) {
      newErrors.prompt = promptResult.error!;
    }
    
    // Validate tags
    const tagsResult = validateTagList(tagList);
    if (!tagsResult.valid) {
      newErrors.tags = tagsResult.error!;
    }
    
    if (Object.keys(newErrors).length > 0) {
      setErrors(newErrors);
      return;
    }
    
    // Submit...
    await articleApi.createArticleWithAI({
      prompt,
      generateImage: true
    });
  };
  
  return (
    <form onSubmit={handleSubmit}>
      <textarea
        value={prompt}
        onChange={(e) => setPrompt(e.target.value)}
        placeholder="Describe your article..."
      />
      {errors.prompt && <span>{errors.prompt}</span>}
      
      <input
        value={tagList}
        onChange={(e) => setTagList(e.target.value)}
        placeholder="AI, Tech, Innovation"
      />
      {errors.tags && <span>{errors.tags}</span>}
      
      <button type="submit">Generate</button>
    </form>
  );
}
```

---

## Utility Functions

### tagsToString
```typescript
import { tagsToString } from 'nnews-react';

const tags: Tag[] = [
  { tagId: 1, title: "React", slug: "react" },
  { tagId: 2, title: "TypeScript", slug: "typescript" }
];

const tagString = tagsToString(tags);
// "React, TypeScript"
```

### stringToTagsPreview
```typescript
import { stringToTagsPreview } from 'nnews-react';

const preview = stringToTagsPreview("React, TypeScript, Hooks");
// ["React", "TypeScript", "Hooks"]
```

### validatePrompt
```typescript
import { validatePrompt } from 'nnews-react';

const result = validatePrompt("Short");
// { valid: false, error: "Prompt must be at least 10 characters long" }

const result2 = validatePrompt("This is a valid prompt for AI");
// { valid: true }
```

### validateTagList
```typescript
import { validateTagList } from 'nnews-react';

const result = validateTagList("AI, Technology, ");
// {
//   valid: false,
//   error: "Tags cannot be empty. Remove extra commas."
// }

const result2 = validateTagList("AI, Technology, Innovation");
// {
//   valid: true,
//   tags: ["AI", "Technology", "Innovation"]
// }
```

---

## API Reference

### createArticleWithAI(request: AIArticleRequest): Promise<Article>

Creates a new article using AI.

**Parameters:**
```typescript
interface AIArticleRequest {
  prompt: string;           // 10-2000 characters (required)
  generateImage?: boolean;  // Generate DALL-E 3 image
  categoryId?: number;      // Category (AI selects if omitted)
  roles?: string[];         // Role slugs
  status?: ArticleStatus;   // Initial status (default: Draft)
}
```

**Example:**
```typescript
const article = await articleApi.createArticleWithAI({
  prompt: "Write about Next.js 14 App Router",
  generateImage: true,
  categoryId: 2,
  roles: ["public"],
  status: ArticleStatus.Published
});
```

---

### updateArticleWithAI(request: AIArticleRequest): Promise<Article>

Updates an existing article using AI.

**Parameters:**
```typescript
interface AIArticleRequest {
  articleId: number;        // Required for update
  prompt: string;           // Instructions (10-2000 chars)
  generateImage?: boolean;  // Generate new image
  // categoryId, roles, status optional
}
```

**Example:**
```typescript
const updated = await articleApi.updateArticleWithAI({
  articleId: 123,
  prompt: "Expand the introduction and add a conclusion section",
  generateImage: false
});
```

---

## Error Handling

```typescript
try {
  const article = await articleApi.createArticleWithAI(request);
} catch (error: any) {
  if (error.response?.status === 400) {
    console.error('Invalid prompt or data:', error.response.data);
  } else if (error.response?.status === 401) {
    console.error('Authentication required');
  } else if (error.response?.status === 500) {
    console.error('AI generation failed:', error.message);
  }
}
```

**Status Codes:**
- `201` - Created successfully
- `200` - Updated successfully  
- `400` - Invalid data/prompt
- `401` - Unauthorized
- `404` - Article not found (update)
- `500` - Server/AI error

---

## Best Practices

1. **Loading States:** AI generation takes 3-10 seconds. Show progress indicators.

```typescript
const [loading, setLoading] = useState(false);

const handleGenerate = async () => {
  setLoading(true);
  try {
    const article = await articleApi.createArticleWithAI(request);
  } finally {
    setLoading(false);
  }
};
```

2. **Validate Before Submitting:**

```typescript
const { valid, error } = validatePrompt(prompt);
if (!valid) {
  alert(error);
  return;
}
```

3. **Image Generation:** Only enable when needed (adds 5-7 seconds).

```typescript
<label>
  <input
    type="checkbox"
    checked={generateImage}
    onChange={(e) => setGenerateImage(e.target.checked)}
  />
  Generate image (adds ~5-7 seconds)
</label>
```

4. **Error Recovery:**

```typescript
const handleGenerateWithRetry = async () => {
  let attempts = 0;
  while (attempts < 3) {
    try {
      return await articleApi.createArticleWithAI(request);
    } catch (error) {
      attempts++;
      if (attempts === 3) throw error;
      await new Promise(r => setTimeout(r, 1000 * attempts));
    }
  }
};
```

---

## Testing

```typescript
import { validatePrompt, validateTagList } from 'nnews-react';

describe('AI Utilities', () => {
  it('validates prompt length', () => {
    expect(validatePrompt('short').valid).toBe(false);
    expect(validatePrompt('This is a valid prompt').valid).toBe(true);
  });
  
  it('validates tag list format', () => {
    const result = validateTagList('AI, Tech, ');
    expect(result.valid).toBe(false);
    
    const result2 = validateTagList('AI, Tech');
    expect(result2.valid).toBe(true);
    expect(result2.tags).toEqual(['AI', 'Tech']);
  });
});
```

---

## TypeScript Support

Full TypeScript definitions included:

```typescript
import type {
  AIArticleRequest,
  AIArticleGeneratorProps,
  Article,
  ArticleInput,
  ArticleUpdate
} from 'nnews-react';
```

---

## Need Help?

- 📖 [Full Changelog](./CHANGELOG.md)
- 🐛 [Report Issues](https://github.com/landim32/NNews/issues)
- 📚 [API Documentation](https://your-api-url/swagger)

---

**Version:** 2.0.0  
**Updated:** January 3, 2026  
**License:** MIT

# Changelog - nnews-react

## Version 2.0.0 (January 3, 2026)

### 🚨 BREAKING CHANGES

#### ArticleInput and ArticleUpdate Interfaces
- **REMOVED:** `tagIds?: number[]` field
- **ADDED:** `tagList?: string` field
  - Tags are now provided as a comma-separated string (e.g., "AI, Technology, Innovation")
  - Tags will be automatically created if they don't exist
  - Existing tags will be reused based on title matching

**Migration Example:**
```typescript
// OLD (v1.x)
const article: ArticleInput = {
  title: "My Article",
  content: "<p>Content</p>",
  categoryId: 1,
  tagIds: [1, 2, 3],  // ❌ No longer supported
  status: ArticleStatus.Draft,
};

// NEW (v2.0)
const article: ArticleInput = {
  title: "My Article",
  content: "<p>Content</p>",
  categoryId: 1,
  tagList: "AI, Technology, Innovation",  // ✅ Use comma-separated string
  status: ArticleStatus.Draft,
};
```

#### ArticleEditor Component
- **REMOVED:** `tags?: Tag[]` prop - no longer needed
- **CHANGED:** Tags UI changed from checkbox selection to text input field
- Component now automatically converts tag array to string when editing existing articles

---

### ✨ NEW FEATURES

#### 1. AI Article Generation (`createArticleWithAI`)
Create complete articles using ChatGPT with automatic content generation.

**Features:**
- Generates HTML content from natural language prompts
- Automatically suggests and creates tags
- Optionally generates images using DALL-E 3
- Auto-selects appropriate category (or use specified one)

**API Method:**
```typescript
const article = await articleApi.createArticleWithAI({
  prompt: "Write a comprehensive article about AI trends in 2024",
  generateImage: true,
  categoryId: 1,  // Optional
  roles: ["public"],
  status: ArticleStatus.Draft
});
```

#### 2. AI Article Update (`updateArticleWithAI`)
Modify existing articles using AI with contextual awareness.

**Features:**
- AI receives current article content as context
- Apply specific changes based on natural language instructions
- Optionally generate new image
- Maintains article history and metadata

**API Method:**
```typescript
const updated = await articleApi.updateArticleWithAI({
  articleId: 123,
  prompt: "Add a section about ChatGPT-4 updates and improve the introduction",
  generateImage: false
});
```

#### 3. AIArticleGenerator Component
New React component for AI-powered article creation and editing.

**Props:**
```typescript
interface AIArticleGeneratorProps {
  mode: 'create' | 'update';
  articleId?: number;  // Required for update mode
  categories?: Category[];
  onSuccess: (article: Article) => void;
  onCancel?: () => void;
  className?: string;
}
```

**Usage Example:**
```typescript
import { AIArticleGenerator } from '@ntools/nnews';

<AIArticleGenerator
  mode="create"
  categories={categories}
  onSuccess={(article) => {
    console.log('Article created:', article);
    navigate(`/articles/${article.articleId}`);
  }}
  onCancel={() => setShowAIGenerator(false)}
/>
```

#### 4. New Utility Functions
Added helper functions for tag management and AI validation:

```typescript
import {
  tagsToString,
  stringToTagsPreview,
  validatePrompt,
  validateTagList
} from '@ntools/nnews';

// Convert Tag array to string
const tagString = tagsToString(article.tags); // "AI, Technology"

// Preview tags from string
const tags = stringToTagsPreview("AI, Tech, ML"); // ["AI", "Tech", "ML"]

// Validate AI prompt
const { valid, error } = validatePrompt(prompt);

// Validate tag list format
const result = validateTagList("AI, Technology, ");
// { valid: false, error: "Tags cannot be empty..." }
```

---

### 🔧 ENHANCEMENTS

#### Article Interface
- **ADDED:** `imageUrl?: string` field for direct image URL access
- Backend now returns full image URLs alongside `imageName`

#### Tag System
- Tags are automatically created when saving articles with `tagList`
- Duplicate detection by title (case-insensitive)
- Response includes full `Tag` objects with `tagId`, `title`, and `slug`

#### ArticleEditor
- Simplified tag input with text field
- Auto-converts existing article tags to comma-separated string
- Better UX with placeholder examples and validation hints

---

### 📖 NEW TYPES

```typescript
// New AI request interface
export interface AIArticleRequest {
  articleId?: number;      // Required for update, optional for create
  prompt: string;          // 10-2000 characters
  generateImage?: boolean; // Generate image with DALL-E 3
  categoryId?: number;     // Optional category
  roles?: string[];        // Optional role slugs
  status?: ArticleStatus;  // Optional initial status
}

// Enhanced Article interface
export interface Article {
  // ... existing fields
  imageUrl?: string;       // NEW: Full image URL
  tags?: Tag[];           // Returns full Tag objects
}
```

---

### 🔄 MIGRATION GUIDE

#### Step 1: Update Article Forms
Replace `tagIds` with `tagList` in all article creation/editing forms:

```typescript
// OLD
const [selectedTagIds, setSelectedTagIds] = useState<number[]>([]);

const payload = {
  ...articleData,
  tagIds: selectedTagIds  // ❌
};

// NEW
const [tagList, setTagList] = useState<string>('');

const payload = {
  ...articleData,
  tagList: tagList  // ✅
};
```

#### Step 2: Update Article Loading
Convert tags array to string when loading existing articles:

```typescript
import { tagsToString } from '@ntools/nnews';

useEffect(() => {
  if (article) {
    setTagList(tagsToString(article.tags || []));
  }
}, [article]);
```

#### Step 3: Update ArticleEditor Usage
Remove `tags` prop from ArticleEditor component:

```typescript
// OLD
<ArticleEditor
  article={article}
  categories={categories}
  tags={tags}  // ❌ No longer needed
  onSave={handleSave}
  onCancel={handleCancel}
/>

// NEW
<ArticleEditor
  article={article}
  categories={categories}
  onSave={handleSave}
  onCancel={handleCancel}
/>
```

#### Step 4: (Optional) Add AI Features
Implement AI article generation in your dashboard:

```typescript
import { AIArticleGenerator } from '@ntools/nnews';

const [showAIMode, setShowAIMode] = useState(false);

{showAIMode ? (
  <AIArticleGenerator
    mode="create"
    categories={categories}
    onSuccess={handleArticleCreated}
    onCancel={() => setShowAIMode(false)}
  />
) : (
  <ArticleEditor
    article={article}
    categories={categories}
    onSave={handleSave}
    onCancel={handleCancel}
  />
)}
```

---

### ⚠️ IMPORTANT NOTES

1. **TagList Format:** Tags must be separated by commas. Leading/trailing spaces are automatically trimmed.
   - ✅ Valid: `"AI, Technology, Innovation"`
   - ✅ Valid: `"AI,Technology,Innovation"`
   - ❌ Invalid: `"AI; Technology; Innovation"` (wrong separator)

2. **AI Image Generation:** When `generateImage: true`, operations may take 5-10 seconds. Implement proper loading states.

3. **Prompt Requirements:**
   - Minimum: 10 characters
   - Maximum: 2000 characters
   - Use `validatePrompt()` for validation

4. **Authentication:** All endpoints (including AI) require Bearer token authentication.

5. **Error Handling:** Handle these status codes:
   - `400 Bad Request`: Invalid data or prompt
   - `401 Unauthorized`: Missing or invalid authentication
   - `404 Not Found`: Article not found (update only)
   - `500 Internal Server Error`: AI or server error

---

### 📚 EXAMPLES

#### Example 1: Create Article with Tags
```typescript
const article = await articleApi.createArticle({
  title: "Getting Started with React Hooks",
  content: "<h2>Introduction</h2><p>...</p>",
  categoryId: 1,
  tagList: "React, JavaScript, Hooks, Tutorial",
  status: ArticleStatus.Published,
  dateAt: new Date().toISOString()
});

// Response includes created tags
console.log(article.tags);
// [
//   { tagId: 1, title: "React", slug: "react" },
//   { tagId: 2, title: "JavaScript", slug: "javascript" },
//   ...
// ]
```

#### Example 2: Update Article Tags
```typescript
import { tagsToString } from '@ntools/nnews';

const article = await articleApi.getArticleById(123);
const currentTags = tagsToString(article.tags || []);
// "React, JavaScript"

const updated = await articleApi.updateArticle({
  ...article,
  tagList: currentTags + ", TypeScript, Advanced",
  dateAt: article.dateAt || new Date().toISOString()
});
```

#### Example 3: Create with AI
```typescript
const aiArticle = await articleApi.createArticleWithAI({
  prompt: "Write a tutorial about React Server Components with practical examples",
  generateImage: true,
  categoryId: 2,
  roles: ["public"],
  status: ArticleStatus.Draft
});

console.log(aiArticle.title);
console.log(aiArticle.content);  // HTML content
console.log(aiArticle.imageUrl); // DALL-E generated image
console.log(aiArticle.tags);     // AI-suggested tags
```

#### Example 4: Update with AI
```typescript
const updated = await articleApi.updateArticleWithAI({
  articleId: 456,
  prompt: "Add a troubleshooting section and update code examples to React 18 syntax",
  generateImage: false
});
```

---

### 🐛 BUG FIXES

- Fixed issue with tags not being properly associated with articles
- Improved error handling for image uploads
- Enhanced type safety for article dates

---

### 📦 Dependencies

No new dependencies added. Compatible with existing setup:
- React 18+
- TypeScript 4.5+
- Axios 1.x

---

### 🔗 API Endpoints Summary

| Method | Endpoint | Description | Status |
|--------|----------|-------------|--------|
| POST | `/api/article` | Create article | 🔄 Modified (+tagList) |
| PUT | `/api/article` | Update article | 🔄 Modified (+tagList) |
| POST | `/api/article/insertWithAI` | Create with AI | ✨ New |
| PUT | `/api/article/updateWithAI` | Update with AI | ✨ New |

---

### 📝 Documentation

For detailed API documentation, see:
- Backend Swagger: `/swagger`
- Package README: `README.md`
- TypeScript definitions: `src/types/news.ts`

---

### 👥 Support

For issues or questions:
- GitHub Issues: https://github.com/landim32/NNews/issues
- Documentation: https://github.com/landim32/NNews

---

**Published:** January 3, 2026  
**Version:** 2.0.0  
**Compatibility:** Breaking changes from v1.x  
**Recommended Update:** Review migration guide before updating

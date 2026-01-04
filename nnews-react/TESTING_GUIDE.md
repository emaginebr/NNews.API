# Testing Guide - nnews-react v2.0.0

## Quick Test Checklist

### 1. Build Test
```bash
cd c:\repos\NNews\nnews-react
npm run build
```

Expected: Build completes without errors, dist/ folder created.

---

### 2. TypeScript Compilation Test
```bash
npx tsc --noEmit
```

Expected: No TypeScript errors.

---

### 3. Import Test
Create a test file to verify all exports work:

```typescript
// test-imports.ts
import {
  // Components
  ArticleEditor,
  AIArticleGenerator,
  ArticleList,
  ArticleViewer,
  
  // Types
  AIArticleRequest,
  ArticleInput,
  ArticleUpdate,
  Article,
  
  // Utilities
  tagsToString,
  stringToTagsPreview,
  validatePrompt,
  validateTagList,
  
  // Context
  useNNews,
  
  // API
  ArticleAPI,
} from 'nnews-react';

console.log('All imports successful!');
```

---

## Integration Tests

### Test 1: Manual Article Creation with Tags

```typescript
import { useNNews, ArticleStatus } from 'nnews-react';

const TestManualCreate = () => {
  const { articleApi } = useNNews();
  
  const testCreate = async () => {
    const article = await articleApi.createArticle({
      title: "Test Article",
      content: "<h2>Test</h2><p>Content</p>",
      categoryId: 1,
      tagList: "Test, Example, Demo",
      status: ArticleStatus.Draft,
      dateAt: new Date().toISOString()
    });
    
    console.assert(article.articleId > 0, 'Article should have ID');
    console.assert(article.tags?.length === 3, 'Should have 3 tags');
    console.log('✅ Manual create test passed');
  };
  
  return <button onClick={testCreate}>Test Manual Create</button>;
};
```

---

### Test 2: AI Article Creation

```typescript
const TestAICreate = () => {
  const { articleApi } = useNNews();
  
  const testAICreate = async () => {
    const article = await articleApi.createArticleWithAI({
      prompt: "Write a short article about React testing best practices",
      generateImage: false,
      categoryId: 1,
      status: ArticleStatus.Draft
    });
    
    console.assert(article.articleId > 0, 'Article should have ID');
    console.assert(article.title.length > 0, 'Should have title');
    console.assert(article.content.includes('<'), 'Should have HTML content');
    console.log('✅ AI create test passed');
  };
  
  return <button onClick={testAICreate}>Test AI Create</button>;
};
```

---

### Test 3: Tag Utilities

```typescript
import { tagsToString, stringToTagsPreview, validateTagList } from 'nnews-react';

const TestUtilities = () => {
  const runTests = () => {
    // Test 1: tagsToString
    const tags = [
      { tagId: 1, title: 'React', slug: 'react' },
      { tagId: 2, title: 'TypeScript', slug: 'typescript' }
    ];
    const str = tagsToString(tags);
    console.assert(str === 'React, TypeScript', 'Tags to string failed');
    
    // Test 2: stringToTagsPreview
    const preview = stringToTagsPreview('AI, ML, DL');
    console.assert(preview.length === 3, 'String to tags preview failed');
    console.assert(preview[0] === 'AI', 'First tag should be AI');
    
    // Test 3: validateTagList
    const valid = validateTagList('AI, Technology');
    console.assert(valid.valid === true, 'Valid tags should pass');
    
    const invalid = validateTagList('AI, , Technology');
    console.assert(invalid.valid === false, 'Empty tags should fail');
    
    console.log('✅ All utility tests passed');
  };
  
  return <button onClick={runTests}>Test Utilities</button>;
};
```

---

### Test 4: ArticleEditor Component

```typescript
import { ArticleEditor, ArticleStatus } from 'nnews-react';

const TestArticleEditor = () => {
  const [result, setResult] = useState(null);
  
  const handleSave = async (data) => {
    console.log('Article data:', data);
    console.assert(typeof data.tagList === 'string', 'tagList should be string');
    console.assert(!data.hasOwnProperty('tagIds'), 'tagIds should not exist');
    setResult(data);
    console.log('✅ ArticleEditor test passed');
  };
  
  return (
    <div>
      <ArticleEditor
        article={null}
        categories={[{ categoryId: 1, title: 'Test' }]}
        onSave={handleSave}
        onCancel={() => console.log('Cancelled')}
      />
      {result && <pre>{JSON.stringify(result, null, 2)}</pre>}
    </div>
  );
};
```

---

### Test 5: AIArticleGenerator Component

```typescript
import { AIArticleGenerator } from 'nnews-react';

const TestAIGenerator = () => {
  const handleSuccess = (article) => {
    console.log('AI generated article:', article);
    console.assert(article.articleId > 0, 'Should have article ID');
    console.assert(article.title.length > 0, 'Should have title');
    console.log('✅ AIArticleGenerator test passed');
  };
  
  return (
    <AIArticleGenerator
      mode="create"
      categories={[{ categoryId: 1, title: 'Test' }]}
      onSuccess={handleSuccess}
    />
  );
};
```

---

## Manual Testing Steps

### Step 1: Test Build
```bash
cd c:\repos\NNews\nnews-react
npm run build
ls dist/  # Verify files exist
```

### Step 2: Test in Consumer App
```bash
# In nnews-react
npm pack

# In consumer app (nnews-app)
cd c:\repos\NNews\nnews-app
npm install ../nnews-react/nnews-react-2.0.0.tgz
```

### Step 3: Test Imports
In consumer app, create test file:
```typescript
// src/test-nnews.tsx
import {
  ArticleEditor,
  AIArticleGenerator,
  tagsToString,
  validatePrompt,
} from 'nnews-react';

console.log('Imports successful');
```

### Step 4: Test ArticleEditor
Replace existing ArticleEditor usage in ArticleEditPage.tsx:
- Remove `tags` prop
- Verify tag input appears
- Submit form and check payload has `tagList`

### Step 5: Test AI Features
Add AIArticleGenerator to a test page:
```tsx
<AIArticleGenerator
  mode="create"
  categories={categories}
  onSuccess={(article) => {
    console.log('Success:', article);
  }}
/>
```

---

## Backend Compatibility Test

### Prerequisites
- NNews API running with AI endpoints
- Valid authentication token

### Test Endpoints

#### 1. Test POST /api/Article with tagList
```bash
curl -X POST http://localhost:5000/api/Article \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Test Article",
    "content": "<p>Test</p>",
    "categoryId": 1,
    "tagList": "Test, Demo, Example",
    "status": 0,
    "dateAt": "2024-01-15T10:00:00Z"
  }'
```

Expected: 201 Created with article data including tags array.

#### 2. Test POST /api/Article/insertWithAI
```bash
curl -X POST http://localhost:5000/api/Article/insertWithAI \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "prompt": "Write a short article about TypeScript benefits",
    "generateImage": false,
    "categoryId": 1,
    "status": 0
  }'
```

Expected: 201 Created with AI-generated article.

#### 3. Test PUT /api/Article/updateWithAI
```bash
curl -X PUT http://localhost:5000/api/Article/updateWithAI \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "articleId": 1,
    "prompt": "Add a conclusion section",
    "generateImage": false
  }'
```

Expected: 200 OK with updated article.

---

## Error Handling Tests

### Test 1: Invalid Prompt
```typescript
try {
  await articleApi.createArticleWithAI({
    prompt: 'short',  // Too short
    generateImage: false
  });
} catch (error) {
  console.assert(error.response?.status === 400, 'Should return 400');
  console.log('✅ Invalid prompt test passed');
}
```

### Test 2: Missing ArticleId in Update
```typescript
try {
  await articleApi.updateArticleWithAI({
    prompt: 'Update content',
    generateImage: false
    // Missing articleId
  });
} catch (error) {
  console.assert(error.message.includes('articleId'), 'Should require articleId');
  console.log('✅ Missing articleId test passed');
}
```

### Test 3: Invalid Tag Format
```typescript
const result = validateTagList('AI, , Technology');
console.assert(!result.valid, 'Empty tags should be invalid');
console.assert(result.error?.includes('empty'), 'Should mention empty tags');
console.log('✅ Invalid tag format test passed');
```

---

## Performance Tests

### Test 1: AI Generation Time
```typescript
const start = Date.now();
const article = await articleApi.createArticleWithAI({
  prompt: 'Write about React',
  generateImage: false
});
const duration = Date.now() - start;
console.log(`AI generation took ${duration}ms`);
console.assert(duration < 10000, 'Should complete in under 10s');
```

### Test 2: Image Generation Time
```typescript
const start = Date.now();
const article = await articleApi.createArticleWithAI({
  prompt: 'Write about React',
  generateImage: true
});
const duration = Date.now() - start;
console.log(`AI + Image generation took ${duration}ms`);
// With image: typically 5-15 seconds
```

---

## Regression Tests

### Test 1: Existing Features Still Work
```typescript
// Test that non-AI features still work
const categories = await categoryApi.listCategories();
console.assert(categories.items.length > 0, 'Categories should load');

const tags = await tagApi.listTags();
console.assert(tags.items.length > 0, 'Tags should load');

console.log('✅ Existing features work');
```

### Test 2: Backwards Compatibility
```typescript
// Articles created in v1.x should still load
const article = await articleApi.getArticleById(1);
console.assert(article.articleId === 1, 'Old article should load');
console.log('✅ Backwards compatibility maintained');
```

---

## Test Report Template

```markdown
# Test Report - nnews-react v2.0.0

## Date: [Date]
## Tester: [Name]

### Build Test
- [ ] Build successful
- [ ] No TypeScript errors
- [ ] dist/ folder created

### Component Tests
- [ ] ArticleEditor loads
- [ ] ArticleEditor submits with tagList
- [ ] AIArticleGenerator renders
- [ ] AIArticleGenerator creates article

### API Tests
- [ ] createArticle with tagList works
- [ ] createArticleWithAI works
- [ ] updateArticleWithAI works
- [ ] Error handling works

### Utility Tests
- [ ] tagsToString works
- [ ] stringToTagsPreview works
- [ ] validatePrompt works
- [ ] validateTagList works

### Integration Tests
- [ ] Package installs in consumer app
- [ ] All imports resolve
- [ ] Components render in app
- [ ] API calls work with backend

### Issues Found
[List any issues]

### Overall Status
- [ ] Ready for production
- [ ] Needs fixes
```

---

## Automated Test Script

Save as `test-nnews.sh`:

```bash
#!/bin/bash

echo "Testing nnews-react v2.0.0..."

# Test 1: Build
echo "Test 1: Building..."
npm run build
if [ $? -eq 0 ]; then
  echo "✅ Build successful"
else
  echo "❌ Build failed"
  exit 1
fi

# Test 2: TypeScript
echo "Test 2: TypeScript check..."
npx tsc --noEmit
if [ $? -eq 0 ]; then
  echo "✅ TypeScript check passed"
else
  echo "❌ TypeScript errors found"
  exit 1
fi

# Test 3: Pack
echo "Test 3: Creating package..."
npm pack
if [ $? -eq 0 ]; then
  echo "✅ Package created"
else
  echo "❌ Pack failed"
  exit 1
fi

echo ""
echo "All tests passed! ✅"
echo "Package is ready for publishing."
```

Run with:
```bash
bash test-nnews.sh
```

---

## Final Checklist

Before publishing:
- [ ] All automated tests pass
- [ ] Manual testing completed
- [ ] Documentation reviewed
- [ ] CHANGELOG.md updated
- [ ] package.json version updated
- [ ] No console errors
- [ ] Backend compatibility confirmed
- [ ] Example code tested
- [ ] Breaking changes documented
- [ ] Migration guide clear

**Status:** Ready for v2.0.0 release ✅

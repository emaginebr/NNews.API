# nnews-react

Complete React component library for NAuth authentication and NNews content management system. Built with TypeScript, Tailwind CSS, and designed as a distributable NPM package.

[![npm version](https://img.shields.io/npm/v/nnews-react.svg)](https://www.npmjs.com/package/nnews-react)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## Features

### Authentication (NAuth)
✨ **Complete Auth Suite** - Login, Register, Password Recovery, Change Password  
👤 **User Management** - Full CRUD with profile editing  
🎭 **Role Management** - Role-based access control  
🎯 **Security** - Device fingerprinting with FingerprintJS  
🔒 **Type-Safe** - Full TypeScript support  

### Content Management (NNews)
📝 **Article Management** - Full CRUD with Markdown editor  
📂 **Category System** - Hierarchical categories with parent-child relationships  
🏷️ **Tag Management** - Flexible tagging with merge functionality  
✍️ **Markdown Editor** - Live preview with syntax highlighting  
👁️ **Article Viewer** - Beautiful article display with GitHub-flavored Markdown  
🔐 **Access Control** - Role-based content visibility  

### General
🎨 **Theme Support** - Light/Dark mode with system detection  
📦 **Tree-shakeable** - Import only what you need  
♿ **Accessible** - WCAG compliant  
📱 **Responsive** - Mobile-first design  

## Installation

```bash
npm install nnews-react react react-dom react-router-dom

# If you don't have Tailwind CSS
npm install -D tailwindcss postcss autoprefixer tailwindcss-animate
npx tailwindcss init -p
```

### Configure Tailwind

```javascript
// tailwind.config.js
export default {
  darkMode: ['class'],
  content: [
    './src/**/*.{js,ts,jsx,tsx}',
    './node_modules/nnews-react/dist/**/*.{js,ts,jsx,tsx}',
  ],
  theme: {
    extend: {
      typography: {
        DEFAULT: {
          css: {
            maxWidth: 'none',
          },
        },
      },
    },
  },
  plugins: [
    require('tailwindcss-animate'),
    require('@tailwindcss/typography'), // For Markdown rendering
  ],
};
```

```css
/* src/index.css */
@tailwind base;
@tailwind components;
@tailwind utilities;
```

## Quick Start

### 1. Setup Providers

```tsx
import { BrowserRouter } from 'react-router-dom';
import { NAuthProvider, NNewsProvider, ThemeProvider } from 'nnews-react';
import 'nnews-react/styles';
import './index.css';

function App() {
  return (
    <BrowserRouter>
      <ThemeProvider defaultTheme="system">
        <NAuthProvider
          config={{
            apiUrl: import.meta.env.VITE_API_URL,
            enableFingerprinting: true,
          }}
        >
          <NNewsProvider
            config={{
              apiUrl: import.meta.env.VITE_API_URL,
            }}
          >
            <YourApp />
          </NNewsProvider>
        </NAuthProvider>
      </ThemeProvider>
    </BrowserRouter>
  );
}
```

```env
# .env
VITE_API_URL=https://your-nauth-api.com
```

### 2. Use Authentication Components

```tsx
import {
  LoginForm,
  RegisterForm,
  UserEditForm,
  RoleList,
  SearchForm,
  useAuth,
  useProtectedRoute,
} from 'nnews-react';
import { useNavigate } from 'react-router-dom';

// Login Page
function LoginPage() {
  const navigate = useNavigate();
  return (
    <div className="min-h-screen flex items-center justify-center">
      <LoginForm
        onSuccess={() => navigate('/dashboard')}
        showRememberMe
        showForgotPassword
      />
    </div>
  );
}

// Protected Dashboard
function Dashboard() {
  const { user, logout } = useAuth();
  useProtectedRoute({ redirectTo: '/login' });

  return (
    <div>
      <h1>Welcome, {user?.name}</h1>
      <button onClick={logout}>Logout</button>
    </div>
  );
}

// User Management
function CreateUserPage() {
  const navigate = useNavigate();
  return (
    <UserEditForm
      onSuccess={(user) => {
        console.log('User created:', user);
        navigate('/users');
      }}
      onCancel={() => navigate('/users')}
    />
  );
}
```

### 3. Use Content Management Components

```tsx
import {
  ArticleList,
  ArticleViewer,
  ArticleEditor,
  CategoryList,
  CategoryModal,
  TagList,
  TagModal,
  useArticles,
  useCategories,
  useTags,
} from 'nnews-react';

// Articles List Page
function ArticlesPage() {
  const { articles, loading, fetchArticles, deleteArticle } = useArticles();

  useEffect(() => {
    fetchArticles({ page: 1, pageSize: 20 });
  }, []);

  return (
    <div className="container mx-auto p-6">
      <h1 className="text-3xl font-bold mb-6">Articles</h1>
      <ArticleList
        articles={articles}
        loading={loading}
        onArticleClick={(article) => navigate(`/articles/${article.id}`)}
        onEditClick={(article) => navigate(`/articles/${article.id}/edit`)}
        onDeleteClick={deleteArticle}
        showActions
      />
    </div>
  );
}

// Article Viewer Page
function ArticleViewPage({ articleId }: { articleId: number }) {
  const { getArticleById } = useArticles();
  const [article, setArticle] = useState(null);

  useEffect(() => {
    getArticleById(articleId).then(setArticle);
  }, [articleId]);

  if (!article) return <div>Loading...</div>;

  return (
    <ArticleViewer
      article={article}
      onBack={() => navigate('/articles')}
      onEdit={(article) => navigate(`/articles/${article.id}/edit`)}
      showActions
    />
  );
}

// Article Editor Page
function ArticleEditorPage({ articleId }: { articleId?: number }) {
  const { createArticle, updateArticle, getArticleById } = useArticles();
  const { categories } = useCategories();
  const { tags } = useTags();
  const [article, setArticle] = useState(null);

  useEffect(() => {
    if (articleId) {
      getArticleById(articleId).then(setArticle);
    }
    fetchCategories();
    fetchTags();
  }, [articleId]);

  const handleSave = async (data) => {
    if (article) {
      await updateArticle({ ...data, id: article.id });
    } else {
      await createArticle(data);
    }
    navigate('/articles');
  };

  return (
    <ArticleEditor
      article={article}
      categories={categories}
      tags={tags}
      onSave={handleSave}
      onCancel={() => navigate('/articles')}
    />
  );
}

// Category Management
function CategoriesPage() {
  const { categories, createCategory, updateCategory, deleteCategory } = useCategories();
  const [modalOpen, setModalOpen] = useState(false);
  const [selectedCategory, setSelectedCategory] = useState(null);

  return (
    <div className="container mx-auto p-6">
      <div className="flex justify-between mb-6">
        <h1 className="text-3xl font-bold">Categories</h1>
        <button onClick={() => setModalOpen(true)}>New Category</button>
      </div>
      <CategoryList
        categories={categories}
        onEditClick={(cat) => {
          setSelectedCategory(cat);
          setModalOpen(true);
        }}
        onDeleteClick={deleteCategory}
        showActions
      />
      <CategoryModal
        isOpen={modalOpen}
        onClose={() => {
          setModalOpen(false);
          setSelectedCategory(null);
        }}
        category={selectedCategory}
        categories={categories}
        onSave={selectedCategory ? updateCategory : createCategory}
      />
    </div>
  );
}
```

## Components

### Authentication Components
- `LoginForm` - Email/password login with validation
- `RegisterForm` - Multi-step registration with password strength
- `ForgotPasswordForm` - Password recovery request
- `ResetPasswordForm` - Password reset with token
- `ChangePasswordForm` - Change password for authenticated users

### User Management
- `UserEditForm` - Create and edit users with full profile management (dual mode)
- `SearchForm` - Search and browse users with pagination

### Role Management
- `RoleList` - List and manage roles with CRUD operations
- `RoleForm` - Create and edit roles

### Content Management (News)
- `ArticleList` - Display paginated list of articles with actions
- `ArticleViewer` - Beautiful article display with Markdown rendering
- `ArticleEditor` - Full-featured article editor with Markdown support
- `MarkdownEditor` - Standalone Markdown editor with live preview
- `CategoryList` - Display and manage categories
- `CategoryModal` - Modal for creating/editing categories
- `TagList` - Display and manage tags
- `TagModal` - Modal for creating/editing tags

### UI Components
`Button`, `Input`, `Label`, `Card`, `Avatar`, `DropdownMenu`, `Toaster`

## Hooks

### Authentication Hooks
```tsx
// Authentication state
const { user, isAuthenticated, login, logout, isLoading } = useAuth();

// User management
const {
  user,
  updateUser,
  createUser,
  getUserById,
  changePassword,
  uploadImage,
  searchUsers,
} = useUser();

// Role management
const { fetchRoles, getRoleById, createRole, updateRole, deleteRole } = useNAuth();

// Route protection
useProtectedRoute({ redirectTo: '/login', requireAdmin: false });

// Theme management
const { theme, setTheme } = useTheme();
```

### Content Management Hooks
```tsx
// Article management
const {
  articles,
  loading,
  error,
  fetchArticles,
  getArticleById,
  createArticle,
  updateArticle,
  deleteArticle,
  refresh,
} = useArticles();

// Category management
const {
  categories,
  loading,
  error,
  fetchCategories,
  getCategoryById,
  createCategory,
  updateCategory,
  deleteCategory,
  refresh,
} = useCategories();

// Tag management
const {
  tags,
  loading,
  error,
  fetchTags,
  getTagById,
  createTag,
  updateTag,
  deleteTag,
  mergeTags,
  refresh,
} = useTags();
```

## Configuration

### NAuth Configuration
```tsx
<NAuthProvider
  config={{
    apiUrl: 'https://your-api.com',           // Required
    timeout: 30000,                            // Optional
    enableFingerprinting: true,                // Optional
    storageType: 'localStorage',               // Optional
    redirectOnUnauthorized: '/login',          // Optional
    onAuthChange: (user) => {},                // Optional
    onLogin: (user) => {},                     // Optional
    onLogout: () => {},                        // Optional
  }}
>
  <App />
</NAuthProvider>
```

### NNews Configuration
```tsx
<NNewsProvider
  config={{
    apiUrl: 'https://your-api.com',           // Required
    timeout: 30000,                            // Optional
  }}
>
  <App />
</NNewsProvider>
```

## API Clients

### NAuth API Client
```tsx
import { createNAuthClient } from 'nnews-react';

const authApi = createNAuthClient({ apiUrl: 'https://your-api.com' });

await authApi.login({ email, password });
await authApi.getMe();
await authApi.updateUser({ name: 'New Name' });
await authApi.uploadImage(file);
```

### NNews API Services
```tsx
import { ArticleAPI, CategoryAPI, TagAPI } from 'nnews-react';
import axios from 'axios';

const apiClient = axios.create({ baseURL: 'https://your-api.com' });

// Article API
const articleApi = new ArticleAPI(apiClient);
await articleApi.listArticles();
await articleApi.getArticleById(id);
await articleApi.createArticle(data);
await articleApi.updateArticle(data);
await articleApi.deleteArticle(id);

// Category API
const categoryApi = new CategoryAPI(apiClient);
await categoryApi.listCategories();
await categoryApi.createCategory(data);

// Tag API
const tagApi = new TagAPI(apiClient);
await tagApi.listTags();
await tagApi.mergeTags(sourceId, targetId);
```

## Customization

```tsx
<LoginForm
  className="shadow-2xl"
  styles={{
    container: 'bg-white',
    button: 'bg-purple-600',
  }}
/>
```

## Utilities

```tsx
import {
  validateCPF,
  validateCNPJ,
  validateEmail,
  formatPhone,
  validatePasswordStrength,
  cn, // Utility for merging Tailwind classes
} from 'nnews-react';
```

## Markdown Support

The Markdown editor supports:

- **Basic Formatting**: Bold, italic, strikethrough
- **Headers**: H1-H6
- **Lists**: Ordered and unordered
- **Links**: Inline and reference-style
- **Images**: Inline images
- **Code**: Inline code and fenced code blocks with syntax highlighting
- **Tables**: GitHub-flavored tables
- **Task Lists**: GitHub-style checkboxes
- **Blockquotes**: Quote blocks

## TypeScript

```tsx
import type {
  // Auth types
  UserInfo,
  LoginCredentials,
  NAuthConfig,
  Theme,
  // News types
  Article,
  Category,
  Tag,
  ArticleStatus,
  ArticleInput,
  ArticleUpdate,
  CategoryInput,
  CategoryUpdate,
  TagInput,
  TagUpdate,
} from 'nnews-react';
```

## Project Structure

```
src/
├── components/       # Auth forms + News components + UI components
├── contexts/         # NAuthContext, NNewsContext, ThemeContext
├── hooks/            # useAuth, useUser, useArticles, useCategories, useTags, etc.
├── services/         # NAuth API client + News API services
├── types/            # TypeScript definitions (auth + news)
├── utils/            # Validators, formatters, utilities
└── styles/           # Tailwind CSS
```

## Development

```bash
npm install        # Install dependencies
npm run dev        # Development mode
npm run build      # Build library
npm test           # Run tests
npm run lint       # Run ESLint
```

## Publishing

```bash
npm run build
npm publish --access public
```

## License

MIT © [Rodrigo Landim](https://github.com/landim32)

## Links

- [GitHub](https://github.com/landim32/NNews/tree/main/nnews-react)
- [NPM](https://www.npmjs.com/package/nnews-react)
- [Documentation](https://github.com/landim32/NNews/tree/main/nnews-react#readme)

import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { NAuthProvider, useNAuth } from 'nauth-react';
import { NNewsProvider } from 'nnews-react';
import { Toaster } from 'sonner';
import { useMemo } from 'react';
//import 'nauth-react/styles';
import { Layout } from './components/Layout';
import { ProtectedRoute } from './components/ProtectedRoute';
import { HomePage } from './pages/HomePage';
import { LoginPage } from './pages/LoginPage';
import { DashboardPage } from './pages/DashboardPage';
import { ProfilePage } from './pages/ProfilePage';
import TagsPage from './pages/TagsPage';
import CategoryPage from './pages/CategoryPage';
import { ArticleListPage } from './pages/ArticleListPage';
import { ArticleEditPage } from './pages/ArticleEditPage';
import { ROUTES } from './lib/constants';

function AppContent() {
  const { token } = useNAuth();

  const nNewsConfig = useMemo(() => ({
    apiUrl: import.meta.env.VITE_NNEWS_API_URL,
    ...(token && {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    }),
  }), [token]);

  return (
    <NNewsProvider config={nNewsConfig}>
          <Toaster position="bottom-right" richColors />
          <Routes>
          <Route element={<Layout />}>
            {/* Public Routes */}
            <Route path={ROUTES.HOME} element={<HomePage />} />
            <Route path={ROUTES.LOGIN} element={<LoginPage />} />

            {/* Protected Routes */}
            <Route
              path={ROUTES.DASHBOARD}
              element={
                <ProtectedRoute>
                  <DashboardPage />
                </ProtectedRoute>
              }
            />
            <Route
              path={ROUTES.PROFILE}
              element={
                <ProtectedRoute>
                  <ProfilePage />
                </ProtectedRoute>
              }
            />
            <Route
              path={ROUTES.TAGS}
              element={
                <ProtectedRoute>
                  <TagsPage />
                </ProtectedRoute>
              }
            />
            <Route
              path={ROUTES.CATEGORIES}
              element={
                <ProtectedRoute>
                  <CategoryPage />
                </ProtectedRoute>
              }
            />
            <Route
              path={ROUTES.ARTICLES}
              element={
                <ProtectedRoute>
                  <ArticleListPage />
                </ProtectedRoute>
              }
            />
            <Route
              path="/articles/new"
              element={
                <ProtectedRoute>
                  <ArticleEditPage />
                </ProtectedRoute>
              }
            />
            <Route
              path="/articles/edit/:id"
              element={
                <ProtectedRoute>
                  <ArticleEditPage />
                </ProtectedRoute>
              }
            />

            {/* Fallback */}
            <Route path="*" element={<Navigate to={ROUTES.HOME} replace />} />
          </Route>
        </Routes>
      </NNewsProvider>
  );
}

function App() {
  return (
    <BrowserRouter>
      <NAuthProvider
        config={{
          apiUrl: import.meta.env.VITE_API_URL,
          enableFingerprinting: true,
          redirectOnUnauthorized: ROUTES.LOGIN,
          onAuthChange: (user) => {
            console.log('Auth state changed:', user);
          },
        }}
      >
        <AppContent />
      </NAuthProvider>
    </BrowserRouter>
  );
}

export default App;

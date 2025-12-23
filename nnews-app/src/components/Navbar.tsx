import { Link } from 'react-router-dom';
import { useAuth } from 'nauth-react';
import { APP_NAME, ROUTES } from '../lib/constants';
import { UserMenu } from './UserMenu';
import { Shield, LayoutDashboard, Tag, FolderTree, Newspaper, ChevronDown, FileText } from 'lucide-react';
import { useState, useRef, useEffect } from 'react';

export function Navbar() {
  const { isAuthenticated } = useAuth();
  const [newsMenuOpen, setNewsMenuOpen] = useState(false);
  const newsMenuRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    function handleClickOutside(event: MouseEvent) {
      if (newsMenuRef.current && !newsMenuRef.current.contains(event.target as Node)) {
        setNewsMenuOpen(false);
      }
    }

    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  return (
    <nav className="bg-white dark:bg-gray-900 border-b border-gray-200 dark:border-gray-800 sticky top-0 z-40">
      <div className="container mx-auto px-4">
        <div className="flex items-center justify-between h-16">
          <div className="flex items-center gap-6">
            <Link to={ROUTES.HOME} className="flex items-center gap-2 text-xl font-bold">
              <Shield className="w-8 h-8 text-blue-600 dark:text-blue-400" />
              <span className="bg-gradient-to-r from-blue-600 to-purple-600 bg-clip-text text-transparent">
                {APP_NAME}
              </span>
            </Link>

            {isAuthenticated && (
              <>
                <Link
                  to={ROUTES.DASHBOARD}
                  className="flex items-center gap-2 px-4 py-2 text-sm font-medium text-gray-700 dark:text-gray-300 hover:text-blue-600 dark:hover:text-blue-400 transition-colors"
                >
                  <LayoutDashboard className="w-4 h-4" />
                  Dashboard
                </Link>
                
                <div className="relative" ref={newsMenuRef}>
                  <button
                    onClick={() => setNewsMenuOpen(!newsMenuOpen)}
                    className="flex items-center gap-2 px-4 py-2 text-sm font-medium text-gray-700 dark:text-gray-300 hover:text-blue-600 dark:hover:text-blue-400 transition-colors"
                  >
                    <Newspaper className="w-4 h-4" />
                    News
                    <ChevronDown className={`w-4 h-4 transition-transform ${newsMenuOpen ? 'rotate-180' : ''}`} />
                  </button>
                  
                  {newsMenuOpen && (
                    <div className="absolute top-full left-0 mt-1 w-48 bg-white dark:bg-gray-800 border border-gray-200 dark:border-gray-700 rounded-lg shadow-lg py-2 z-50">
                      <Link
                        to={ROUTES.ARTICLES}
                        className="flex items-center gap-2 px-4 py-2 text-sm text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors"
                        onClick={() => setNewsMenuOpen(false)}
                      >
                        <FileText className="w-4 h-4" />
                        Articles
                      </Link>
                      <Link
                        to={ROUTES.CATEGORIES}
                        className="flex items-center gap-2 px-4 py-2 text-sm text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors"
                        onClick={() => setNewsMenuOpen(false)}
                      >
                        <FolderTree className="w-4 h-4" />
                        Categories
                      </Link>
                      <Link
                        to={ROUTES.TAGS}
                        className="flex items-center gap-2 px-4 py-2 text-sm text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors"
                        onClick={() => setNewsMenuOpen(false)}
                      >
                        <Tag className="w-4 h-4" />
                        Tags
                      </Link>
                    </div>
                  )}
                </div>
              </>
            )}
          </div>

          <div className="flex items-center gap-4">
            {isAuthenticated ? (
              <UserMenu />
            ) : (
              <div className="flex items-center gap-2">
                <Link
                  to={ROUTES.LOGIN}
                  className="px-4 py-2 text-sm font-medium text-gray-700 dark:text-gray-300 hover:text-blue-600 dark:hover:text-blue-400 transition-colors"
                >
                  Login
                </Link>
                <button
                  disabled
                  className="px-4 py-2 text-sm font-medium bg-gray-400 text-gray-200 rounded-lg cursor-not-allowed opacity-60"
                >
                  Sign Up
                </button>
              </div>
            )}
          </div>
        </div>
      </div>
    </nav>
  );
}

import { useState, useEffect, useMemo } from 'react';
import { CategoryList, CategoryModal, useCategories } from 'nnews-react';
import type { Category, CategoryInput, CategoryUpdate } from 'nnews-react';
import { X, AlertTriangle, Search, ChevronLeft, ChevronRight } from 'lucide-react';
import { toast } from 'sonner';
import { Card, CardContent } from '../components/ui/Card';

type ModalMode = 'create' | 'edit' | null;

const ITEMS_PER_PAGE = 10;

export default function CategoryPage() {
  const [modalMode, setModalMode] = useState<ModalMode>(null);
  const [selectedCategory, setSelectedCategory] = useState<Category | null>(null);
  const [modalOpen, setModalOpen] = useState(false);
  const [deleteConfirmOpen, setDeleteConfirmOpen] = useState(false);
  const [categoryToDelete, setCategoryToDelete] = useState<Category | null>(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [currentPage, setCurrentPage] = useState(1);

  const {
    categories,
    loading,
    error,
    fetchCategories,
    createCategory,
    updateCategory,
    deleteCategory,
  } = useCategories();

  // Fetch categories on mount
  useEffect(() => {
    fetchCategories();
  }, []);

  // Filter categories by search term
  const filteredCategories = useMemo(() => {
    if (!searchTerm.trim()) return categories;
    
    const searchLower = searchTerm.toLowerCase();
    return categories.filter((category) =>
      category.title.toLowerCase().includes(searchLower)
    );
  }, [categories, searchTerm]);

  // Paginate filtered categories
  const paginatedCategories = useMemo(() => {
    const startIndex = (currentPage - 1) * ITEMS_PER_PAGE;
    const endIndex = startIndex + ITEMS_PER_PAGE;
    return filteredCategories.slice(startIndex, endIndex);
  }, [filteredCategories, currentPage]);

  // Calculate total pages
  const totalPages = Math.ceil(filteredCategories.length / ITEMS_PER_PAGE);

  // Reset to page 1 when search term changes
  useEffect(() => {
    setCurrentPage(1);
  }, [searchTerm]);

  const handleEdit = (category: Category) => {
    setSelectedCategory(category);
    setModalMode('edit');
    setModalOpen(true);
  };

  const handleCreate = () => {
    setSelectedCategory(null);
    setModalMode('create');
    setModalOpen(true);
  };

  const handleSave = async (categoryData: CategoryInput | CategoryUpdate) => {
    try {
      if (modalMode === 'create') {
        await createCategory(categoryData as CategoryInput);
        toast.success('Category created successfully!');
      } else if (modalMode === 'edit' && selectedCategory) {
        await updateCategory({
          ...categoryData,
          categoryId: selectedCategory.categoryId,
        } as CategoryUpdate);
        toast.success('Category updated successfully!');
      }
      handleCloseModal();
      await fetchCategories();
    } catch (err) {
      toast.error(err instanceof Error ? err.message : 'Failed to save category');
      throw err;
    }
  };

  const handleDelete = (category: Category) => {
    setCategoryToDelete(category);
    setDeleteConfirmOpen(true);
  };

  const confirmDelete = async () => {
    if (!categoryToDelete) return;

    try {
      await deleteCategory(categoryToDelete.categoryId || 0);
      toast.success('Category deleted successfully!');
      await fetchCategories();
    } catch (err) {
      toast.error(err instanceof Error ? err.message : 'Failed to delete category');
    } finally {
      setDeleteConfirmOpen(false);
      setCategoryToDelete(null);
    }
  };

  const cancelDelete = () => {
    setDeleteConfirmOpen(false);
    setCategoryToDelete(null);
  };

  const handleCloseModal = () => {
    setModalOpen(false);
    setModalMode(null);
    setSelectedCategory(null);
  };

  const handlePreviousPage = () => {
    setCurrentPage((prev) => Math.max(prev - 1, 1));
  };

  const handleNextPage = () => {
    setCurrentPage((prev) => Math.min(prev + 1, totalPages));
  };

  return (
    <div className="container mx-auto px-4">
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-gray-900 dark:text-gray-100">
          Manage Categories
        </h1>
        <p className="mt-2 text-gray-600 dark:text-gray-400">
          Create and manage categories for organizing articles
        </p>
      </div>

      <Card className="pt-5">
        <CardContent className="space-y-4">
          {/* Toolbar: Search and Create Button */}
          <div className="flex flex-col sm:flex-row gap-4 justify-between items-start sm:items-center">
            <div className="relative flex-1 max-w-md">
              <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 w-5 h-5" />
              <input
                type="text"
                placeholder="Search categories by title..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="w-full pl-10 pr-4 py-2 border border-gray-300 dark:border-gray-600 rounded-md bg-white dark:bg-gray-800 text-gray-900 dark:text-gray-100 placeholder-gray-500 dark:placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>
            <button
              onClick={handleCreate}
              className="rounded-md bg-blue-600 px-4 py-2 text-white hover:bg-blue-700 transition-colors whitespace-nowrap"
            >
              New Category
            </button>
          </div>

          {/* Results info */}
          {searchTerm && (
            <div className="text-sm text-gray-600 dark:text-gray-400">
              Found {filteredCategories.length} {filteredCategories.length === 1 ? 'category' : 'categories'}
            </div>
          )}

          {/* Category List Table */}
          <CategoryList
            categories={paginatedCategories}
            loading={loading}
            error={error}
            onEditClick={handleEdit}
            onDeleteClick={handleDelete}
            showActions={true}
            emptyMessage={searchTerm ? 'No categories match your search' : 'No categories found. Create your first category!'}
          />

          {/* Pagination */}
          {totalPages > 1 && (
            <div className="flex items-center justify-between border-t border-gray-200 dark:border-gray-700 pt-4">
              <div className="text-sm text-gray-600 dark:text-gray-400">
                Showing {((currentPage - 1) * ITEMS_PER_PAGE) + 1} to{' '}
                {Math.min(currentPage * ITEMS_PER_PAGE, filteredCategories.length)} of{' '}
                {filteredCategories.length} categories
              </div>
              <div className="flex items-center gap-2">
                <button
                  onClick={handlePreviousPage}
                  disabled={currentPage === 1}
                  className="p-2 rounded-md border border-gray-300 dark:border-gray-600 text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-800 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
                >
                  <ChevronLeft className="w-5 h-5" />
                </button>
                <span className="text-sm text-gray-700 dark:text-gray-300">
                  Page {currentPage} of {totalPages}
                </span>
                <button
                  onClick={handleNextPage}
                  disabled={currentPage === totalPages}
                  className="p-2 rounded-md border border-gray-300 dark:border-gray-600 text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-gray-800 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
                >
                  <ChevronRight className="w-5 h-5" />
                </button>
              </div>
            </div>
          )}
        </CardContent>
      </Card>

      {/* Modal */}
      {modalOpen && (
        <div
          className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4"
          onClick={handleCloseModal}
        >
          <div
            className="bg-white dark:bg-gray-800 rounded-lg shadow-xl max-w-lg w-full"
            onClick={(e) => e.stopPropagation()}
          >
            {/* Modal Header */}
            <div className="bg-white dark:bg-gray-800 border-b border-gray-200 dark:border-gray-700 px-6 py-4 flex items-center justify-between rounded-t-lg">
              <h2 className="text-xl font-bold text-gray-900 dark:text-gray-100">
                {modalMode === 'create' ? 'Create New Category' : 'Edit Category'}
              </h2>
              <button
                onClick={handleCloseModal}
                className="text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-200 transition-colors"
              >
                <X className="w-5 h-5" />
              </button>
            </div>

            {/* Modal Content */}
            <div className="p-6">
              <CategoryModal
                category={selectedCategory}
                categories={categories}
                isOpen={modalOpen}
                onClose={handleCloseModal}
                onSave={handleSave}
                loading={loading}
              />
            </div>
          </div>
        </div>
      )}

      {/* Delete Confirmation Modal */}
      {deleteConfirmOpen && categoryToDelete && (
        <div
          className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4"
          onClick={cancelDelete}
        >
          <div
            className="bg-white dark:bg-gray-800 rounded-lg shadow-xl max-w-md w-full"
            onClick={(e) => e.stopPropagation()}
          >
            {/* Modal Header */}
            <div className="bg-white dark:bg-gray-800 border-b border-gray-200 dark:border-gray-700 px-6 py-4 flex items-center justify-between rounded-t-lg">
              <div className="flex items-center gap-2">
                <AlertTriangle className="w-5 h-5 text-red-600" />
                <h2 className="text-xl font-bold text-gray-900 dark:text-gray-100">
                  Confirm Delete
                </h2>
              </div>
              <button
                onClick={cancelDelete}
                className="text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-200 transition-colors"
              >
                <X className="w-5 h-5" />
              </button>
            </div>

            {/* Modal Content */}
            <div className="p-6">
              <p className="text-gray-700 dark:text-gray-300 mb-6">
                Are you sure you want to delete the category{' '}
                <strong className="font-semibold text-gray-900 dark:text-gray-100">
                  "{categoryToDelete.title}"
                </strong>
                ? This action cannot be undone.
              </p>

              {/* Action Buttons */}
              <div className="flex justify-end gap-3">
                <button
                  onClick={cancelDelete}
                  className="px-4 py-2 text-gray-700 dark:text-gray-300 bg-gray-100 dark:bg-gray-700 hover:bg-gray-200 dark:hover:bg-gray-600 rounded-md transition-colors"
                >
                  Cancel
                </button>
                <button
                  onClick={confirmDelete}
                  disabled={loading}
                  className="px-4 py-2 text-white bg-red-600 hover:bg-red-700 disabled:bg-red-400 disabled:cursor-not-allowed rounded-md transition-colors"
                >
                  {loading ? 'Deleting...' : 'Delete'}
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

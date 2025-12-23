import { useState, useEffect } from 'react';
import { TagList, TagModal, TagMerge, useTags } from 'nnews-react';
import type { Tag, TagInput, TagUpdate } from 'nnews-react';
import { X, AlertTriangle } from 'lucide-react';
import { toast } from 'sonner';
import { Card, CardContent } from '../components/ui/Card';

type ModalMode = 'create' | 'edit' | null;

export default function TagsPage() {
  const [modalMode, setModalMode] = useState<ModalMode>(null);
  const [selectedTag, setSelectedTag] = useState<Tag | null>(null);
  const [modalOpen, setModalOpen] = useState(false);
  const [deleteConfirmOpen, setDeleteConfirmOpen] = useState(false);
  const [tagToDelete, setTagToDelete] = useState<Tag | null>(null);
  const [mergeModalOpen, setMergeModalOpen] = useState(false);
  const [tagToMerge, setTagToMerge] = useState<Tag | null>(null);
  
  const {
    tags,
    loading,
    error,
    fetchTags,
    createTag,
    updateTag,
    deleteTag,
    mergeTags,
  } = useTags();

  // Fetch tags on mount
  useEffect(() => {
    fetchTags();
  }, []);

  const handleEdit = (tag: Tag) => {
    setSelectedTag(tag);
    setModalMode('edit');
    setModalOpen(true);
  };

  const handleCreate = () => {
    setSelectedTag(null);
    setModalMode('create');
    setModalOpen(true);
  };

  const handleSave = async (tagData: TagInput | TagUpdate) => {
    try {
      if (modalMode === 'create') {
        await createTag(tagData as TagInput);
        toast.success('Tag created successfully!');
      } else if (modalMode === 'edit' && selectedTag) {
        await updateTag({ ...tagData, tagId: selectedTag.tagId } as TagUpdate);
        toast.success('Tag updated successfully!');
      }
      handleCloseModal();
      await fetchTags();
    } catch (err) {
      toast.error(err instanceof Error ? err.message : 'Failed to save tag');
      throw err;
    }
  };

  const handleDelete = (tag: Tag) => {
    setTagToDelete(tag);
    setDeleteConfirmOpen(true);
  };

  const confirmDelete = async () => {
    if (!tagToDelete) return;

    try {
      await deleteTag(tagToDelete.tagId || 0);
      toast.success('Tag deleted successfully!');
      await fetchTags();
    } catch (err) {
      toast.error(err instanceof Error ? err.message : 'Failed to delete tag');
    } finally {
      setDeleteConfirmOpen(false);
      setTagToDelete(null);
    }
  };

  const cancelDelete = () => {
    setDeleteConfirmOpen(false);
    setTagToDelete(null);
  };

  const handleCloseModal = () => {
    setModalOpen(false);
    setModalMode(null);
    setSelectedTag(null);
  };

  const handleMerge = (tag: Tag) => {
    setTagToMerge(tag);
    setMergeModalOpen(true);
  };

  const confirmMerge = async (sourceId: number, targetId: number) => {
    try {
      await mergeTags(sourceId, targetId);
      toast.success('Tags merged successfully!');
      await fetchTags();
    } catch (err) {
      toast.error(err instanceof Error ? err.message : 'Failed to merge tags');
      throw err;
    } finally {
      setMergeModalOpen(false);
      setTagToMerge(null);
    }
  };

  const cancelMerge = () => {
    setMergeModalOpen(false);
    setTagToMerge(null);
  };

  return (
    <div className="container mx-auto px-4">
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-gray-900 dark:text-gray-100">
          Manage Tags
        </h1>
        <p className="mt-2 text-gray-600 dark:text-gray-400">
          Create and manage tags for organizing articles
        </p>
      </div>

      <Card className="pt-5">
        <CardContent className="space-y-4">
          {/* Create Button */}
          <div className="flex justify-end">
            <button
              onClick={handleCreate}
              className="rounded-md bg-blue-600 px-4 py-2 text-white hover:bg-blue-700 transition-colors"
            >
              New Tag
            </button>
          </div>

          <TagList
            tags={tags}
            loading={loading}
            error={error}
            onEditClick={handleEdit}
            onDeleteClick={handleDelete}
            onMergeClick={handleMerge}
            showActions={true}
            emptyMessage="No tags found. Create your first tag!"
          />
        </CardContent>
      </Card>

      {/* Modal */}
      {modalOpen && (
        <div
          className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4"
          onClick={handleCloseModal}
        >
          <div
            className="bg-white dark:bg-gray-800 rounded-lg shadow-xl max-w-md w-full"
            onClick={(e) => e.stopPropagation()}
          >
            {/* Modal Header */}
            <div className="bg-white dark:bg-gray-800 border-b border-gray-200 dark:border-gray-700 px-6 py-4 flex items-center justify-between rounded-t-lg">
              <h2 className="text-xl font-bold text-gray-900 dark:text-gray-100">
                {modalMode === 'create' ? 'Create New Tag' : 'Edit Tag'}
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
              <TagModal
                tag={selectedTag}
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
      {deleteConfirmOpen && tagToDelete && (
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
                Are you sure you want to delete the tag{' '}
                <strong className="font-semibold text-gray-900 dark:text-gray-100">
                  "{tagToDelete.title}"
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

      {/* Merge Modal */}
      {mergeModalOpen && tagToMerge && (
        <div
          className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4"
          onClick={cancelMerge}
        >
          <div
            className="bg-white dark:bg-gray-800 rounded-lg shadow-xl max-w-md w-full"
            onClick={(e) => e.stopPropagation()}
          >
            {/* Modal Header */}
            <div className="bg-white dark:bg-gray-800 border-b border-gray-200 dark:border-gray-700 px-6 py-4 flex items-center justify-between rounded-t-lg">
              <h2 className="text-xl font-bold text-gray-900 dark:text-gray-100">
                Merge Tags
              </h2>
              <button
                onClick={cancelMerge}
                className="text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-200 transition-colors"
              >
                <X className="w-5 h-5" />
              </button>
            </div>

            {/* Modal Content */}
            <div className="p-6">
              <TagMerge
                sourceTag={tagToMerge}
                availableTags={tags}
                isOpen={mergeModalOpen}
                onClose={cancelMerge}
                onMerge={confirmMerge}
                loading={loading}
              />
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

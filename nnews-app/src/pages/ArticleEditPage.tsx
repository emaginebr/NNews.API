import { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useArticles, useCategories, useTags, ArticleEditor, type ArticleInput, type ArticleUpdate } from 'nnews-react';
import { ArrowLeft } from 'lucide-react';
import { toast } from 'sonner';

export function ArticleEditPage() {
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();
  const isEditing = id !== 'new' && id !== undefined;

  const {
    articles,
    loading: articleLoading,
    createArticle,
    updateArticle,
    fetchArticles,
  } = useArticles();

  const { categories, fetchCategories } = useCategories();
  const { tags, fetchTags } = useTags();

  const [currentArticle, setCurrentArticle] = useState<any>(null);
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    fetchCategories();
    fetchTags();

    if (isEditing) {
      fetchArticles({ page: 1, pageSize: 100 });
    }
  }, []);

  useEffect(() => {
    if (isEditing && articles) {
      const article = articles.items.find(
        (a) => a.articleId === Number(id)
      );
      if (article) {
        setCurrentArticle(article);
      }
    }
  }, [isEditing, id, articles]);

  const handleSave = async (articleData: ArticleInput | ArticleUpdate) => {
    setSaving(true);
    try {
      if (isEditing && currentArticle) {
        await updateArticle({ 
          ...articleData, 
          articleId: currentArticle.articleId 
        } as ArticleUpdate);
        toast.success('Article updated successfully');
      } else {
        await createArticle(articleData as ArticleInput);
        toast.success('Article created successfully');
      }
      navigate('/articles');
    } catch (error) {
      toast.error(isEditing ? 'Failed to update article' : 'Failed to create article');
      console.error('Save error:', error);
    } finally {
      setSaving(false);
    }
  };

  const handleCancel = () => {
    navigate('/articles');
  };

  const handleBack = () => {
    navigate('/articles');
  };

  if (isEditing && articleLoading) {
    return (
      <div className="container mx-auto px-4 py-8">
        <div className="flex items-center justify-center p-8">
          <div className="text-gray-500 dark:text-gray-400">Loading article...</div>
        </div>
      </div>
    );
  }

  if (isEditing && !currentArticle && !articleLoading) {
    return (
      <div className="container mx-auto px-4 py-8">
        <div className="rounded-lg bg-red-50 dark:bg-red-900/20 p-4">
          <p className="text-sm text-red-800 dark:text-red-200">
            Article not found
          </p>
        </div>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <div className="mb-6">
        <button
          onClick={handleBack}
          className="flex items-center gap-2 text-gray-600 dark:text-gray-400 hover:text-gray-900 dark:hover:text-gray-100 transition-colors"
        >
          <ArrowLeft className="h-5 w-5" />
          Back to Articles
        </button>
      </div>

      <div className="mb-6">
        <h1 className="text-3xl font-bold text-gray-900 dark:text-gray-100">
          {isEditing ? 'Edit Article' : 'New Article'}
        </h1>
        <p className="mt-2 text-gray-600 dark:text-gray-400">
          {isEditing
            ? 'Update the article information below'
            : 'Fill in the information to create a new article'}
        </p>
      </div>

      <div className="rounded-lg bg-white dark:bg-gray-800 shadow p-6">
        <ArticleEditor
          article={currentArticle}
          categories={categories || []}
          tags={tags || []}
          onSave={handleSave}
          onCancel={handleCancel}
          loading={saving}
        />
      </div>
    </div>
  );
}

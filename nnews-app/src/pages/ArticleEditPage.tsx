import { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useArticles, useCategories, ArticleEditor, AIArticleGenerator, type ArticleInput, type ArticleUpdate, type Article } from 'nnews-react';
import { ArrowLeft, Sparkles } from 'lucide-react';
import { toast } from 'sonner';
import 'react-quill-new/dist/quill.snow.css';

export function ArticleEditPage() {
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();
  const isEditing = id !== 'new' && id !== undefined;
  const [showAIModal, setShowAIModal] = useState(false);

  const {
    articles,
    loading: articleLoading,
    createArticle,
    updateArticle,
    fetchArticles,
  } = useArticles();

  const { categories, fetchCategories } = useCategories();

  const [currentArticle, setCurrentArticle] = useState<any>(null);
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    fetchCategories();

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

  const handleAISuccess = (article: Article) => {
    toast.success(`Article ${isEditing ? 'updated' : 'created'} successfully with AI!`);
    setShowAIModal(false);
    // Refresh the current article data
    if (isEditing) {
      fetchArticles({ page: 1, pageSize: 100 });
    } else {
      // Navigate to edit the new article
      navigate(`/articles/edit/${article.articleId}`);
    }
  };

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
      <div className="mb-6 flex items-center justify-between">
        <button
          onClick={handleBack}
          className="flex items-center gap-2 text-gray-600 dark:text-gray-400 hover:text-gray-900 dark:hover:text-gray-100 transition-colors"
        >
          <ArrowLeft className="h-5 w-5" />
          Back to Articles
        </button>
        
        {isEditing && (
          <button
            onClick={() => setShowAIModal(true)}
            className="flex items-center gap-2 rounded-lg bg-gradient-to-r from-blue-600 to-purple-600 px-4 py-2 text-white hover:from-blue-700 hover:to-purple-700 transition-all shadow-md hover:shadow-lg"
          >
            <Sparkles className="h-5 w-5" />
            Edit with AI
          </button>
        )}
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
          onSave={handleSave}
          onCancel={handleCancel}
          loading={saving}
        />
      </div>

      <AIArticleGenerator
        mode={isEditing ? 'update' : 'create'}
        articleId={currentArticle?.articleId}
        isOpen={showAIModal}
        onSuccess={handleAISuccess}
        onClose={() => setShowAIModal(false)}
      />
    </div>
  );
}

import { LoginForm } from 'nauth-react';
import { useNavigate, Navigate } from 'react-router-dom';
import { useAuth } from 'nauth-react';
import { toast } from 'sonner';
import { ROUTES } from '../lib/constants';
import { Card, CardHeader, CardTitle, CardDescription, CardContent } from '../components/ui/Card';

export function LoginPage() {
  const navigate = useNavigate();
  const { isAuthenticated, isLoading } = useAuth();

  if (isLoading) {
    return (
      <div className="min-h-[80vh] flex items-center justify-center">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  // Redirect authenticated users to dashboard
  if (isAuthenticated) {
    return <Navigate to={ROUTES.DASHBOARD} replace />;
  }

  const handleSuccess = () => {
    toast.success('Login successful! Welcome back.');
    
    // Check if there's a stored redirect destination
    const redirectTo = sessionStorage.getItem('redirectAfterLogin');
    if (redirectTo) {
      sessionStorage.removeItem('redirectAfterLogin');
      navigate(redirectTo);
    } else {
      navigate(ROUTES.DASHBOARD);
    }
  };

  const handleError = (error: Error) => {
    toast.error(error.message || 'Login failed. Please try again.');
  };

  return (
    <div className="min-h-[80vh] flex items-center justify-center">
      <div className="w-full max-w-md">
        <Card className="p-8 bg-white dark:bg-gray-800">
          <CardHeader className="mb-8">
            <CardTitle>Welcome Back</CardTitle>
            <CardDescription>
              Sign in to your account to continue
            </CardDescription>
          </CardHeader>

          <CardContent>
            <LoginForm
              onSuccess={handleSuccess}
              onError={handleError}
              showRememberMe={true}
              className="space-y-4"
            />
          </CardContent>
        </Card>
      </div>
    </div>
  );
}

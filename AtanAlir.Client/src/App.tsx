import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { AuthProvider } from './context/AuthContext';
import { Login } from './pages/Login';
import { Register } from './pages/Register';
import { Home } from './pages/Home';
import { Header } from './components/layout/Header';
import { PrivateRoute } from './components/PrivateRoute';
import { MatchChat } from './pages/MatchChat';
import { EmailVerification } from './pages/EmailVerification';
import { SetNickname } from './pages/SetNickname';
import { ErrorBoundary } from './components/ErrorBoundary';

const queryClient = new QueryClient();

function App() {
  return (
    <ErrorBoundary>
    <QueryClientProvider client={queryClient}>
      <AuthProvider>
        <Router>
          <div className="min-h-screen bg-gray-50">
            <Header />
            {/* max-w-7xl ve container sınıflarını kaldırıyoruz */}
            <main className="w-screen">
              <Routes>
                <Route path="/login" element={<Login />} />
                <Route path="/register" element={<Register />} />
                <Route path="/verify-email" element={<EmailVerification />} />
                <Route path="/set-nickname" element={<SetNickname />} />
                <Route 
                  path="/" 
                  element={
                    <PrivateRoute>
                      <Home />
                    </PrivateRoute>
                  } 
                />
                <Route 
                  path="/match/:matchId" 
                  element={
                    <PrivateRoute>
                      <MatchChat />
                    </PrivateRoute>
                  } 
                />
              </Routes>
            </main>
          </div>
        </Router>
      </AuthProvider>
    </QueryClientProvider>
    </ErrorBoundary>
  );
}

export default App;
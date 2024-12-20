// components/PrivateRoute.tsx
import { Navigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export function PrivateRoute({ children }: { children: React.ReactNode }) {
    const { user } = useAuth();
    const token = localStorage.getItem('token');

    // Kullanıcı giriş yapmamışsa veya token yoksa login sayfasına yönlendir
    if (!user || !token) {
        return <Navigate to="/login" />;
    }

    // Kullanıcı giriş yapmışsa children'ı render et
    return <>{children}</>;
}
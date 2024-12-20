import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Input } from '../components/ui/Input';
import { Button } from '../components/ui/Button';
import api from '../services/api';
import { useAuth } from '../context/AuthContext';

export function Login() {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const [isLoading, setIsLoading] = useState(false);
    const navigate = useNavigate();
    const { setUser } = useAuth();

    
    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError('');
        setIsLoading(true);
    
        // Debug için log ekleyelim
        console.log('Login attempt with:', {
            email,
            password,
            url: import.meta.env.PROD ? '' : ''
        });
    
        try {
            const { data } = await api.post('/auth/login', { email, password });
            console.log('Login response:', data);
            localStorage.setItem('token', data.token);
    
            setUser({
                id: data.id,
                email: email,
                firstName: data.firstName,
                lastName: data.lastName,
                nickname: data.nickname,
                isEmailVerified: data.isEmailVerified
            });
    
            navigate('/');
        } catch (err) {
            console.error('Login error details:', err);
            setError('Giriş başarısız oldu. Lütfen bilgilerinizi kontrol edin.');
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <div className="min-h-screen w-screen bg-gray-100 flex items-center justify-center py-12 px-4">
        <div className="max-w-md w-full bg-white rounded-lg shadow-lg p-8">
                <div>
                    <h2 className="mt-6 text-center text-3xl font-extrabold text-gray-900">
                        Giriş Yap
                    </h2>
                </div>
                <form className="mt-8 space-y-6" onSubmit={handleSubmit}>
                    <div className="space-y-4">
                        <Input
                            label="Email"
                            type="email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            required
                        />
                        <Input
                            label="Şifre"
                            type="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            required
                        />
                    </div>

                    {error && (
                        <p className="text-red-500 text-sm">{error}</p>
                    )}

                    <Button
                        type="submit"
                        className="w-full"
                        isLoading={isLoading}
                    >
                        Giriş Yap
                    </Button>

                    <div className="text-center">
                        <button
                            type="button"
                            onClick={() => navigate('/register')}
                            className="text-sm text-blue-600 hover:text-blue-500"
                        >
                            Hesabınız yok mu? Kayıt olun
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}
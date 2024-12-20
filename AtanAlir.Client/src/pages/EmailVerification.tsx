import { useState } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { Input } from '../components/ui/Input';
import { Button } from '../components/ui/Button';
import api from '../services/api';

interface LocationState {
    email: string;
}

export function EmailVerification() {
    const [code, setCode] = useState('');
    const [error, setError] = useState('');
    const [isLoading, setIsLoading] = useState(false);
    const navigate = useNavigate();
    const location = useLocation();
    const email = (location.state as LocationState)?.email;

    // EmailVerification.tsx içindeki handleSubmit fonksiyonu
const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!email) {
        setError('Email bilgisi eksik');
        navigate('/register');
        return;
    }
    setError('');
    setIsLoading(true);

    try {
        // Email doğrulama işlemini yap
        await api.post('/auth/verify-email', { 
            email,
            code 
        });

        // Nickname belirleme sayfasına yönlendir
        navigate('/set-nickname', { 
            state: { email },
            replace: true
        });
    } catch  {
        setError('Doğrulama kodu geçersiz. Lütfen tekrar deneyin.');
    } finally {
        setIsLoading(false);
    }
};

    return (
        <div className="min-h-screen bg-gray-100 flex items-center justify-center py-12 px-4 sm:px-6 lg:px-8">
            <div className="max-w-md w-full bg-white rounded-lg shadow-lg p-8">
                <h2 className="text-center text-3xl font-extrabold text-gray-900 mb-4">
                    Email Doğrulama
                </h2>
                <p className="text-center text-gray-600 mb-6">
                    {email} adresine gönderilen doğrulama kodunu girin
                </p>

                <form onSubmit={handleSubmit} className="space-y-6">
                    <Input
                        label="Doğrulama Kodu"
                        value={code}
                        onChange={(e) => setCode(e.target.value)}
                        required
                    />

                    {error && (
                        <p className="text-red-500 text-sm">{error}</p>
                    )}

                    <Button
                        type="submit"
                        className="w-full"
                        isLoading={isLoading}
                    >
                        Doğrula
                    </Button>
                </form>
            </div>
        </div>
    );
}
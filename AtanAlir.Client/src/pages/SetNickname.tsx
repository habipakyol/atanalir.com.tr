import { useState } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { Input } from '../components/ui/Input';
import { Button } from '../components/ui/Button';
import api from '../services/api';

interface LocationState {
    email: string;
}

export function SetNickname() {
    const [nickname, setNickname] = useState('');
    const [error, setError] = useState('');
    const [isLoading, setIsLoading] = useState(false);
    const navigate = useNavigate();
    const location = useLocation();
    const email = (location.state as LocationState)?.email;

    // SetNickname.tsx içindeki handleSubmit fonksiyonu
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
        // Nickname belirleme işlemini yap
        await api.post('/auth/set-nickname', { 
            email,
            nickname 
        });

        // Login sayfasına yönlendir
        navigate('/login', { replace: true });
    } catch  {
        setError('Bu nickname zaten kullanımda. Lütfen başka bir nickname seçin.');
    } finally {
        setIsLoading(false);
    }
};

    return (
        <div className="min-h-screen bg-gray-100 flex items-center justify-center py-12 px-4 sm:px-6 lg:px-8">
            <div className="max-w-md w-full bg-white rounded-lg shadow-lg p-8">
                <h2 className="text-center text-3xl font-extrabold text-gray-900 mb-4">
                    Nickname Belirle
                </h2>
                <p className="text-center text-gray-600 mb-6">
                    Chat sayfalarında kullanacağınız bir nickname belirleyin
                </p>

                <form onSubmit={handleSubmit} className="space-y-6">
                    <Input
                        label="Nickname"
                        value={nickname}
                        onChange={(e) => setNickname(e.target.value)}
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
                        Devam Et
                    </Button>
                </form>
            </div>
        </div>
    );
}
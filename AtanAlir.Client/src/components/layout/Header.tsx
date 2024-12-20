import { useAuth } from '../../context/AuthContext';
import { useNavigate } from 'react-router-dom';

export function Header() {
    const { user, setUser } = useAuth();
    const navigate = useNavigate();

    const handleLogout = () => {
        localStorage.removeItem('token');
        setUser(null);
        navigate('/login');
    };

    return (
        <header className="bg-blue-600 text-white shadow-lg w-screen">
            <div className="w-screen px-4 h-16 flex items-center justify-between">
                <div className="flex items-center gap-4">
                    <h1 className="text-2xl font-bold cursor-pointer" onClick={() => navigate('/')}>AtanAlır</h1>
                    {/* {user && (
                        <div className="flex items-center gap-2">
                            <span className="text-sm">Hoş geldin,</span>
                            <span className="font-medium">{user.nickname || user.firstName}</span>
                        </div>
                    )} */}
                </div>
                {user && (
                    <div>
                        <button 
                            onClick={handleLogout}
                            className="px-4 py-2 bg-blue-700 hover:bg-blue-800 rounded-lg text-sm transition-colors"
                        >
                            Çıkış Yap
                        </button>
                    </div>
                )}
            </div>
        </header>
    );
    
}
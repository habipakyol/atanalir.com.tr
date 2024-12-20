import { useState, useEffect, useRef } from 'react';
import { useParams } from 'react-router-dom';
import { HubConnectionBuilder, HubConnection } from '@microsoft/signalr';
import { useAuth } from '../context/AuthContext';
import EmojiPicker, { EmojiClickData } from 'emoji-picker-react';
import { useQuery } from '@tanstack/react-query';
import { matchService } from '../services/matchService';

interface ChatMessage {
    userId: string;
    nickname: string;
    message: string;
    quotedMessage?: string;
    timestamp: Date;
}

export function MatchChat() {
    const [messages, setMessages] = useState<ChatMessage[]>([]);
    const [newMessage, setNewMessage] = useState('');
    const [connection, setConnection] = useState<HubConnection | null>(null);
    const [quotedMessage, setQuotedMessage] = useState<ChatMessage | null>(null);
    const [showEmojiPicker, setShowEmojiPicker] = useState(false);
    const { matchId } = useParams();
    const { user } = useAuth();
    const messagesEndRef = useRef<HTMLDivElement>(null);
    const [lastMessageTime, setLastMessageTime] = useState<Date | null>(null);
    const [error, setError] = useState<string>('');
    

    // Maç bilgisi sorgusu
    const { data: match, isLoading, error: matchError } = useQuery({
        queryKey: ['match', matchId],
        queryFn: () => matchService.getMatch(Number(matchId)),
        enabled: !!matchId,
    });

    useEffect(() => {
        let mounted = true;
        const hubConnection = new HubConnectionBuilder()
            .withUrl('https://localhost:7276/chatHub', {
                accessTokenFactory: () => localStorage.getItem('token') || ''
            })
            .withAutomaticReconnect()
            .build();

        hubConnection.on('ReceiveMessage', (message: ChatMessage) => {
            if (mounted) {
                setMessages(prev => {
                    const isDuplicate = prev.some(m => 
                        m.userId === message.userId && 
                        m.message === message.message && 
                        m.timestamp === message.timestamp
                    );
                    return isDuplicate ? prev : [...prev, message];
                });
            }
        });

        hubConnection.start()
            .then(() => {
                if (mounted) {
                    setConnection(hubConnection);
                    return hubConnection.invoke('JoinMatch', parseInt(matchId!, 10));
                }
            })
            .catch(err => console.error('SignalR Connection Error: ', err));

        return () => {
            mounted = false;
            if (hubConnection) {
                hubConnection.invoke('LeaveMatch', parseInt(matchId!, 10))
                    .then(() => hubConnection.stop())
                    .catch(console.error);
            }
        };
    }, [matchId]);

    useEffect(() => {
        messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
    }, [messages]);

    const handleSendMessage = async () => {
        if (!connection || !newMessage.trim()) return;

        if (lastMessageTime) {
            const timeSinceLastMessage = new Date().getTime() - lastMessageTime.getTime();
            const remainingTime = 5000 - timeSinceLastMessage;

            if (timeSinceLastMessage < 5000) {
                setError(`Lütfen ${Math.ceil(remainingTime / 1000)} saniye bekleyin`);
                return;
            }
        }

        try {
            await connection.invoke('SendMessage', 
                parseInt(matchId!, 10),
                newMessage, 
                quotedMessage?.message
            );
            setNewMessage('');
            setQuotedMessage(null);
            setShowEmojiPicker(false);
            setLastMessageTime(new Date());
            setError('');
        } catch (err) {
            console.error('Error sending message: ', err);
        }
    };

    const handleEmojiClick = (emojiData: EmojiClickData) => {
        setNewMessage(prev => prev + emojiData.emoji);
    };

    

    return (
        <div className="w-screen h-screen flex flex-col bg-gray-100">
            {/* Maç Bilgisi Header */}
            <div className="bg-white shadow-md p-4">
                <div className="max-w-7xl mx-auto flex justify-between items-center">
                    {isLoading ? (
                        <span>Yükleniyor...</span>
                    ) : matchError ? (
                        <span>Bir hata oluştu: {matchError.message}</span>
                    ) : (
                        <>
                            <div className="text-lg font-bold text-gray-800">
                                {match?.homeTeam?.name} vs {match?.awayTeam?.name}
                            </div>
                            <div className="text-xl font-bold text-gray-900">
                                {match?.score?.fullTime?.home} - {match?.score?.fullTime?.away}
                            </div>
                        </>
                    )}
                </div>
            </div>

            {/* Mesajlar Alanı */}
            <div className="flex-1 overflow-y-auto p-4 space-y-4">
                {messages.map((msg, index) => (
                    <div 
                        key={index} 
                        className={`flex flex-col ${msg.userId === user?.id ? 'items-end' : 'items-start'} mb-4`}
                    >
                        <div 
                            className={`max-w-[80%] md:max-w-[70%] rounded-lg shadow-sm
                                ${msg.userId === user?.id 
                                    ? 'bg-blue-600 text-white rounded-br-none' 
                                    : 'bg-white text-gray-800 rounded-bl-none'}`}
                        >
                            <div className={`px-4 py-2 border-b ${msg.userId === user?.id ? 'border-blue-500' : 'border-gray-100'}`}>
                                <span className="text-sm font-medium">
                                    {msg.nickname}
                                </span>
                            </div>

                            {msg.quotedMessage && (
                                <div className={`mx-4 mt-2 p-2 rounded-lg text-sm
                                    ${msg.userId === user?.id 
                                        ? 'bg-blue-700 border-l-4 border-blue-400' 
                                        : 'bg-gray-50 border-l-4 border-gray-300'}`}>
                                    {msg.quotedMessage}
                                </div>
                            )}

                            <div className="p-4 pt-2">
                                <p className="text-base">{msg.message}</p>
                            </div>
                        </div>

                        <button 
                            onClick={() => setQuotedMessage(msg)}
                            className="text-xs text-gray-500 mt-1 hover:text-gray-700 px-2 py-1 rounded hover:bg-gray-100"
                        >
                            Alıntıla
                        </button>
                    </div>
                ))}
                <div ref={messagesEndRef} />
            </div>

            {quotedMessage && (
                <div className="bg-gray-100 border-t border-gray-200 p-3 flex justify-between items-center">
                    <div className="flex items-center space-x-2">
                        <div className="w-1 h-full bg-blue-500"></div>
                        <div>
                            <span className="text-sm font-medium text-gray-700">{quotedMessage.nickname}</span>
                            <p className="text-sm text-gray-600">{quotedMessage.message}</p>
                        </div>
                    </div>
                    <button 
                        onClick={() => setQuotedMessage(null)}
                        className="text-gray-500 hover:text-gray-700"
                    >
                        <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                        </svg>
                    </button>
                </div>
            )}

            <div className="border-t border-gray-200 bg-white p-4">
                <div className="max-w-7xl mx-auto">
                    <div className="flex items-center space-x-2">
                        <input 
                            type="text" 
                            value={newMessage} 
                            onChange={(e) => setNewMessage(e.target.value)} 
                            onKeyDown={(e) => e.key === 'Enter' && handleSendMessage()}
                            placeholder="Mesajınızı yazın..." 
                            className="flex-1 p-3 border border-gray-300 rounded-lg focus:outline-none focus:ring focus:ring-blue-300"
                        />
                        <button 
                            onClick={() => setShowEmojiPicker(!showEmojiPicker)} 
                            className="p-3 bg-gray-100 hover:bg-gray-200 rounded-lg">
                            😊
                        </button>
                        <button 
                            onClick={handleSendMessage} 
                            className="p-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700">
                            Gönder
                        </button>
                    </div>
                    {showEmojiPicker && (
                        <div className="absolute bottom-16 left-0">
                            <EmojiPicker onEmojiClick={handleEmojiClick} />
                        </div>
                    )}
                    {error && <p className="text-sm text-red-600 mt-2">{error}</p>}
                </div>
            </div>
        </div>
    );
}




import { useQuery } from '@tanstack/react-query';
import { matchService } from '../services/matchService';
import { Match } from '../types/match';
import { useNavigate } from 'react-router-dom';

function MatchCard({ match }: { match: Match }) {
    const navigate = useNavigate();

    const getBorderClass = () => {
        if (match.status === 'LIVE' || match.status === 'IN_PLAY' || match.status === 'PAUSED') 
            return 'border-l-4 border-red-500';
        return 'border-l-4 border-gray-200';
    };

    const getScore = () => {
        const homeScore = match.score?.fullTime?.home;
        const awayScore = match.score?.fullTime?.away;
        
        if (homeScore === null || awayScore === null) return 'vs';
        return `${homeScore} - ${awayScore}`;
    };

    const getMatchTime = () => {
        if (match.status === 'SCHEDULED' || match.status === 'TIMED') {
            return new Date(match.utcDate).toLocaleTimeString('tr-TR', {
                hour: '2-digit',
                minute: '2-digit'
            });
        }
        return match.status === 'LIVE' ? 'CANLI' : match.status;
    };

    const handleClick = () => {
        if (match.status === 'LIVE' || match.status === 'IN_PLAY' || match.status === 'PAUSED') {
            navigate(`/match/${match.id}`);
        }
    };

    return (
        <div 
            className={`bg-white shadow rounded-lg p-4 ${getBorderClass()} 
                       ${(match.status === 'LIVE' || match.status === 'IN_PLAY' || match.status === 'PAUSED') 
                         ? 'cursor-pointer hover:shadow-lg transition-shadow' : ''}`}
            onClick={handleClick}
        >
            <div className="flex justify-between items-center">
                <span className="font-medium text-gray-800">{match.homeTeam.name}</span>
                <span className="font-bold text-lg text-gray-900">{getScore()}</span>
                <span className="font-medium text-gray-800">{match.awayTeam.name}</span>
            </div>
            <div className="mt-2 text-sm text-center">
                <span className={`${match.status === 'LIVE' ? 'text-red-600' : 'text-gray-600'}`}>
                    {getMatchTime()}
                </span>
                {(match.status === 'LIVE' || match.status === 'IN_PLAY') && (
                    <div className="text-xs text-blue-600 mt-1">
                        Sohbete katılmak için tıklayın
                    </div>
                )}
            </div>
        </div>
    );
}

export function Home() {
    const { data: matches, isLoading } = useQuery({
        queryKey: ['matches'],
        queryFn: matchService.getMatches,
        refetchInterval: 30000 // Her 30 saniyede bir güncelle
    });

    return (
        <div className="w-screen p-4">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
                <div className="bg-white rounded-lg shadow-lg p-6">
                    <h2 className="text-xl font-semibold mb-6 text-gray-800 border-b pb-2">
                        Canlı Maçlar
                    </h2>
                    <div className="space-y-4">
                        {isLoading ? (
                            <div className="text-center py-4">Yükleniyor...</div>
                        ) : matches?.liveMatches.length ? (
                            matches.liveMatches.map(match => (
                                <MatchCard key={match.id} match={match} />
                            ))
                        ) : (
                            <div className="text-center py-4 text-gray-500">
                                Şu anda canlı maç bulunmuyor
                            </div>
                        )}
                    </div>
                </div>
                
                <div className="bg-white rounded-lg shadow-lg p-6">
                    <h2 className="text-xl font-semibold mb-6 text-gray-800 border-b pb-2">
                        Yaklaşan Maçlar
                    </h2>
                    <div className="space-y-4">
                        {isLoading ? (
                            <div className="text-center py-4">Yükleniyor...</div>
                        ) : matches?.upcomingMatches.length ? (
                            matches.upcomingMatches.map(match => (
                                <MatchCard key={match.id} match={match} />
                            ))
                        ) : (
                            <div className="text-center py-4 text-gray-500">
                                Yaklaşan maç bulunmuyor
                            </div>
                        )}
                    </div>
                </div>
            </div>
        </div>
    );
}
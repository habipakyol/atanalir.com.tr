import api from './api';
import { Match, MatchesResponse } from '../types/match';

interface FootballDataResponse {
    matches: Match[];
    competition: {
        id: number;
        name: string;
    };
    filters: {
        season: number;
    };
}



export const matchService = {
    getMatches: async () => {
        try {
            const { data } = await api.get<FootballDataResponse>('/footballdata/matches/today');
            
            // API'den gelen tüm maçları filtrele
            const allMatches = data.matches || [];
            
            // Canlı ve yaklaşan maçları ayır
            const liveMatches = allMatches.filter((match: Match) => 
                match.status === 'LIVE' || 
                match.status === 'IN_PLAY' || 
                match.status === 'PAUSED'
            );
            
            const upcomingMatches = allMatches.filter((match: Match) => 
                match.status === 'SCHEDULED' || 
                match.status === 'TIMED'
            );

            return {
                liveMatches,
                upcomingMatches
            } as MatchesResponse;
            
        } catch (error) {
            console.error('Error fetching matches:', error);
            throw error;
        }
    },

    getMatch: async (matchId: number) => {
        const { data } = await api.get<Match>(`/footballdata/matches/${matchId}`);
        return data;
    }

};
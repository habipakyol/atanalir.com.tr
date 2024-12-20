// types/match.ts
export interface Match {
    id: number;
    homeTeam: {
        id: number;
        name: string;
    };
    awayTeam: {
        id: number;
        name: string;
    };
    score: {
        fullTime: {
            home: number | null;
            away: number | null;
        };
        halfTime?: {
            home: number | null;
            away: number | null;
        };
    };
    status: string;  // 'SCHEDULED' | 'TIMED' | 'LIVE' | 'IN_PLAY' | 'PAUSED' | 'FINISHED'
    utcDate: string;
    competition: {
        id: number;
        name: string;
    };
}

export interface MatchesResponse {
    liveMatches: Match[];
    upcomingMatches: Match[];
}
import axios from 'axios';

const footballDataApi = axios.create({
  baseURL: 'https://api.football-data.org/v4',
  headers: {
    'X-Auth-Token': 'your-api-key'
  }
});

export const footballDataService = {
  getTurkishLeagueMatches: async () => {
    const { data } = await footballDataApi.get('/competitions/TR1/matches');
    return data;
  },

  getMatch: async (matchId: number) => {
    const { data } = await footballDataApi.get(`/matches/${matchId}`);
    return data;
  }
};
import axios from 'axios';
import { CryptoData, PricePoint } from '../types/crypto';

const API_URL = 'https://crypto-dashboard-api-m6lg.onrender.com/api';

const api = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

export const cryptoAPI = {
  getTopCryptos: async (): Promise<CryptoData[]> => {
    const response = await api.get<CryptoData[]>('/crypto/top');
    return response.data;
  },

  getCryptoData: async (symbol: string): Promise<CryptoData> => {
    const response = await api.get<CryptoData>(`/crypto/${symbol}`);
    return response.data;
  },

  getPriceHistory: async (symbol: string, days: number = 7): Promise<PricePoint[]> => {
    const response = await api.get<PricePoint[]>(`/crypto/${symbol}/history?days=${days}`);
    return response.data;
  },
};

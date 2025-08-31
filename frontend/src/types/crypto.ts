export interface CryptoData {
  id: string;
  symbol: string;
  name: string;
  currentPrice: number;
  priceChange24h: number;
  priceChangePercentage24h: number;
  marketCap: number;
  volume24h: number;
  lastUpdated: string;
  sparklineData: number[];
}

export interface PricePoint {
  timestamp: string;
  price: number;
}

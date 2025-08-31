# create-frontend-files.ps1
# Execute na pasta crypto-dashboard/frontend

Write-Host "Criando arquivos do Frontend React..." -ForegroundColor Green

# 1. Types
$cryptoTypes = @'
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
'@

Write-Host "Criando types/crypto.ts..." -ForegroundColor Yellow
Set-Content -Path "src\types\crypto.ts" -Value $cryptoTypes

# 2. SignalR Service
$signalRService = @'
import * as signalR from '@microsoft/signalr';
import { CryptoData } from '../types/crypto';

class SignalRService {
  private connection: signalR.HubConnection | null = null;
  private connectionPromise: Promise<void> | null = null;

  async connect(): Promise<void> {
    if (this.connection?.state === signalR.HubConnectionState.Connected) {
      return;
    }

    if (this.connectionPromise) {
      return this.connectionPromise;
    }

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5123/hubs/crypto', {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
      .build();

    this.connectionPromise = this.connection.start();
    
    try {
      await this.connectionPromise;
      console.log('SignalR Connected!');
    } catch (err) {
      console.error('SignalR Connection Error:', err);
      this.connectionPromise = null;
      throw err;
    }
  }

  async subscribe(symbol: string): Promise<void> {
    if (!this.connection) await this.connect();
    await this.connection!.invoke('Subscribe', symbol);
  }

  async unsubscribe(symbol: string): Promise<void> {
    if (!this.connection) return;
    await this.connection.invoke('Unsubscribe', symbol);
  }

  onPriceUpdate(callback: (data: CryptoData) => void): void {
    if (!this.connection) return;
    this.connection.on('PriceUpdate', callback);
  }

  onMarketUpdate(callback: (data: CryptoData[]) => void): void {
    if (!this.connection) return;
    this.connection.on('MarketUpdate', callback);
  }

  async disconnect(): Promise<void> {
    if (this.connection) {
      await this.connection.stop();
      this.connection = null;
      this.connectionPromise = null;
    }
  }
}

export default new SignalRService();
'@

Write-Host "Criando services/signalr.ts..." -ForegroundColor Yellow
Set-Content -Path "src\services\signalr.ts" -Value $signalRService

# 3. API Service
$apiService = @'
import axios from 'axios';
import { CryptoData, PricePoint } from '../types/crypto';

const API_URL = 'http://localhost:5123/api';

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
'@

Write-Host "Criando services/api.ts..." -ForegroundColor Yellow
Set-Content -Path "src\services\api.ts" -Value $apiService

# 4. CryptoCard Component
$cryptoCard = @'
import React from 'react';
import { CryptoData } from '../../types/crypto';

interface CryptoCardProps {
  crypto: CryptoData;
  onClick?: () => void;
}

const CryptoCard: React.FC<CryptoCardProps> = ({ crypto, onClick }) => {
  const isPositive = crypto.priceChangePercentage24h > 0;
  
  return (
    <div 
      onClick={onClick}
      className="bg-slate-800 rounded-xl p-6 hover:bg-slate-700 transition-all cursor-pointer border border-slate-700"
    >
      <div className="flex justify-between items-start mb-4">
        <div>
          <h3 className="text-xl font-bold text-white">{crypto.name}</h3>
          <p className="text-gray-400 uppercase">{crypto.symbol}</p>
        </div>
        <div className={`px-3 py-1 rounded-full text-sm font-medium ${
          isPositive ? 'bg-green-900 text-green-200' : 'bg-red-900 text-red-200'
        }`}>
          {isPositive ? '▲' : '▼'} {Math.abs(crypto.priceChangePercentage24h).toFixed(2)}%
        </div>
      </div>
      
      <div className="space-y-2">
        <div className="flex justify-between">
          <span className="text-gray-400">Price</span>
          <span className="text-white font-semibold">
            ${crypto.currentPrice.toLocaleString()}
          </span>
        </div>
        <div className="flex justify-between">
          <span className="text-gray-400">Volume 24h</span>
          <span className="text-gray-300">
            ${(crypto.volume24h / 1000000000).toFixed(2)}B
          </span>
        </div>
        <div className="flex justify-between">
          <span className="text-gray-400">Market Cap</span>
          <span className="text-gray-300">
            ${(crypto.marketCap / 1000000000).toFixed(2)}B
          </span>
        </div>
      </div>
    </div>
  );
};

export default CryptoCard;
'@

Write-Host "Criando components/Dashboard/CryptoCard.tsx..." -ForegroundColor Yellow
Set-Content -Path "src\components\Dashboard\CryptoCard.tsx" -Value $cryptoCard

# 5. PriceChart Component
$priceChart = @'
import React from 'react';
import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
  Area,
  AreaChart,
} from 'recharts';

interface PriceChartProps {
  data: { time: string; price: number }[];
  title: string;
}

const PriceChart: React.FC<PriceChartProps> = ({ data, title }) => {
  return (
    <div className="bg-slate-800 rounded-xl p-6 border border-slate-700">
      <h3 className="text-xl font-bold text-white mb-4">{title}</h3>
      <ResponsiveContainer width="100%" height={400}>
        <AreaChart data={data}>
          <defs>
            <linearGradient id="colorPrice" x1="0" y1="0" x2="0" y2="1">
              <stop offset="5%" stopColor="#8b5cf6" stopOpacity={0.8}/>
              <stop offset="95%" stopColor="#8b5cf6" stopOpacity={0}/>
            </linearGradient>
          </defs>
          <CartesianGrid strokeDasharray="3 3" stroke="#334155" />
          <XAxis 
            dataKey="time" 
            stroke="#94a3b8"
            style={{ fontSize: '12px' }}
          />
          <YAxis 
            stroke="#94a3b8"
            style={{ fontSize: '12px' }}
            domain={['dataMin - 100', 'dataMax + 100']}
          />
          <Tooltip 
            contentStyle={{ 
              backgroundColor: '#1e293b', 
              border: '1px solid #334155',
              borderRadius: '8px'
            }}
            labelStyle={{ color: '#e2e8f0' }}
          />
          <Area
            type="monotone"
            dataKey="price"
            stroke="#8b5cf6"
            fillOpacity={1}
            fill="url(#colorPrice)"
            strokeWidth={2}
          />
        </AreaChart>
      </ResponsiveContainer>
    </div>
  );
};

export default PriceChart;
'@

Write-Host "Criando components/Charts/PriceChart.tsx..." -ForegroundColor Yellow
Set-Content -Path "src\components\Charts\PriceChart.tsx" -Value $priceChart

# 6. Dashboard Component
$dashboard = @'
import React, { useEffect, useState } from 'react';
import { CryptoData } from '../../types/crypto';
import { cryptoAPI } from '../../services/api';
import signalRService from '../../services/signalr';
import CryptoCard from './CryptoCard';
import PriceChart from '../Charts/PriceChart';

const Dashboard: React.FC = () => {
  const [cryptos, setCryptos] = useState<CryptoData[]>([]);
  const [selectedCrypto, setSelectedCrypto] = useState<string>('BTC');
  const [loading, setLoading] = useState(true);
  const [connected, setConnected] = useState(false);

  useEffect(() => {
    const initializeDashboard = async () => {
      try {
        // Fetch initial data
        const topCryptos = await cryptoAPI.getTopCryptos();
        setCryptos(topCryptos);
        
        // Connect to SignalR
        await signalRService.connect();
        setConnected(true);
        
        // Subscribe to updates
        signalRService.onMarketUpdate((data) => {
          setCryptos(data);
        });
        
        signalRService.onPriceUpdate((data) => {
          setCryptos(prev => 
            prev.map(crypto => 
              crypto.symbol === data.symbol ? data : crypto
            )
          );
        });
        
        // Subscribe to specific cryptos
        ['BTC', 'ETH', 'ADA'].forEach(symbol => {
          signalRService.subscribe(symbol);
        });
        
      } catch (error) {
        console.error('Error initializing dashboard:', error);
      } finally {
        setLoading(false);
      }
    };

    initializeDashboard();

    return () => {
      signalRService.disconnect();
    };
  }, []);

  const selectedCryptoData = cryptos.find(c => c.symbol === selectedCrypto);
  const chartData = selectedCryptoData?.sparklineData.map((price, index) => ({
    time: `${index}h`,
    price: price
  })) || [];

  if (loading) {
    return (
      <div className="min-h-screen bg-slate-900 flex items-center justify-center">
        <div className="text-white text-2xl">Loading...</div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-slate-900 p-6">
      <div className="max-w-7xl mx-auto">
        {/* Header */}
        <div className="mb-8">
          <h1 className="text-4xl font-bold text-white mb-2">
            Crypto Dashboard
          </h1>
          <div className="flex items-center gap-4">
            <p className="text-gray-400">Real-time monitoring</p>
            <div className={`flex items-center gap-2 px-3 py-1 rounded-full ${
              connected ? 'bg-green-900 text-green-200' : 'bg-red-900 text-red-200'
            }`}>
              <div className={`w-2 h-2 rounded-full ${
                connected ? 'bg-green-400' : 'bg-red-400'
              }`} />
              {connected ? 'Connected' : 'Disconnected'}
            </div>
          </div>
        </div>

        {/* Crypto Cards */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
          {cryptos.map((crypto) => (
            <CryptoCard
              key={crypto.symbol}
              crypto={crypto}
              onClick={() => setSelectedCrypto(crypto.symbol)}
            />
          ))}
        </div>

        {/* Price Chart */}
        {selectedCryptoData && (
          <PriceChart
            data={chartData}
            title={`${selectedCryptoData.name} (${selectedCryptoData.symbol}) - Last 24h`}
          />
        )}
      </div>
    </div>
  );
};

export default Dashboard;
'@

Write-Host "Criando components/Dashboard/Dashboard.tsx..." -ForegroundColor Yellow
Set-Content -Path "src\components\Dashboard\Dashboard.tsx" -Value $dashboard

# 7. App.tsx
$appTsx = @'
import React from 'react';
import Dashboard from './components/Dashboard/Dashboard';
import './App.css';

function App() {
  return (
    <div className="App">
      <Dashboard />
    </div>
  );
}

export default App;
'@

Write-Host "Atualizando App.tsx..." -ForegroundColor Yellow
Set-Content -Path "src\App.tsx" -Value $appTsx

# 8. App.css
$appCss = @'
.App {
  text-align: center;
}
'@

Write-Host "Atualizando App.css..." -ForegroundColor Yellow
Set-Content -Path "src\App.css" -Value $appCss

Write-Host "" -ForegroundColor White
Write-Host "Todos os arquivos do Frontend foram criados!" -ForegroundColor Green
Write-Host "" -ForegroundColor White
Write-Host "Proximos passos:" -ForegroundColor Cyan
Write-Host "1. Certifique-se que o backend esta rodando (dotnet run)" -ForegroundColor White
Write-Host "2. Na pasta frontend, execute: npm start" -ForegroundColor White
Write-Host "3. Acesse: http://localhost:3000" -ForegroundColor White
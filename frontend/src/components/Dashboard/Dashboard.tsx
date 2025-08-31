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

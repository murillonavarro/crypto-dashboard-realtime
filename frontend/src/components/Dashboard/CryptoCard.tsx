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

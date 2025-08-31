# 📊 Crypto Dashboard Real-Time

Dashboard de monitoramento de criptomoedas em tempo real com atualização via WebSocket.

![.NET](https://img.shields.io/badge/.NET-8.0-purple)
![React](https://img.shields.io/badge/React-18.2-blue)
![TypeScript](https://img.shields.io/badge/TypeScript-5.0-blue)
![SignalR](https://img.shields.io/badge/SignalR-Real--time-orange)

## 🚀 Demo

[Ver Demo Online](#)

## ✨ Funcionalidades

- 📈 **Atualização em Tempo Real** - Preços atualizados via WebSocket (SignalR)
- 📊 **Gráficos Interativos** - Visualização com Recharts
- 💹 **Top Criptomoedas** - Bitcoin, Ethereum, Cardano
- 🔄 **Auto-refresh** - Atualização automática a cada 10 segundos
- 📱 **Design Responsivo** - Funciona em desktop e mobile
- 🎨 **Interface Moderna** - Dark theme com TailwindCSS

## 🛠️ Tecnologias

### Backend (.NET 8)
- **ASP.NET Core** - Web API
- **SignalR** - WebSocket para real-time
- **Redis** - Cache (opcional)
- **Clean Architecture** - Separação em camadas
- **Swagger** - Documentação da API

### Frontend (React)
- **React 18** com **TypeScript**
- **SignalR Client** - Conexão WebSocket
- **Recharts** - Gráficos
- **TailwindCSS** - Estilização
- **Axios** - Requisições HTTP

## 💻 Como Executar Localmente

### Pré-requisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/)
- [Docker](https://docker.com) (opcional para Redis)

### Instalação

1. **Clone o repositório**
```bash
git clone https://github.com/murillonavarro/crypto-dashboard-realtime.git
cd crypto-dashboard-realtime
```

2. **Redis (opcional)**
```bash
docker-compose up -d
```

3. **Backend**
```bash
cd backend/CryptoDashboard.API
dotnet run
# API rodando em http://localhost:5178
# Swagger em http://localhost:5178/swagger
```

4. **Frontend**
```bash
cd frontend
npm install
npm start
# Aplicação em http://localhost:3000
```

## 📡 API Endpoints

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/crypto/top` | Retorna top criptomoedas |
| GET | `/api/crypto/{symbol}` | Dados de uma crypto específica |
| GET | `/api/crypto/{symbol}/history` | Histórico de preços |
| WS | `/hubs/crypto` | Hub SignalR WebSocket |

## 🏗️ Arquitetura

```
crypto-dashboard/
├── backend/
│   ├── CryptoDashboard.API/           # API e SignalR Hub
│   ├── CryptoDashboard.Core/          # Modelos e Interfaces
│   └── CryptoDashboard.Infrastructure/# Serviços e Cache
├── frontend/
│   ├── src/
│   │   ├── components/                # Componentes React
│   │   ├── services/                  # API e SignalR
│   │   └── types/                     # TypeScript types
│   └── public/
└── docker-compose.yml                 # Redis config
```

## 🚀 Deploy

### Vercel (Frontend)
```bash
cd frontend
vercel
```

### Azure App Service (Backend)
```bash
cd backend/CryptoDashboard.API
dotnet publish -c Release
# Deploy via Azure Portal ou CLI
```

## 📸 Screenshots



## 👨‍💻 Autor

**Murillo Navarro**
- Portfolio: [murillonavarro.vercel.app](https://murillonavarro.vercel.app)
- GitHub: [@murillonavarro](https://github.com/murillonavarro)
- LinkedIn: [murillonavarro](https://linkedin.com/in/murillonavarro)

## 📄 Licença

Este projeto está sob licença MIT.
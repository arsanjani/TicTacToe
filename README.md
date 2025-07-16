# 🎮 Multiplayer Tic Tac Toe Game

A real-time multiplayer tic-tac-toe game built with ASP.NET Core 9.0, SignalR, and React 19. Players can create games, share links with friends, and play in real-time with beautiful, modern UI.

## 🌐 Live Demo

**Try the game now**: [https://apponline.ir](https://apponline.ir)

## ✨ Features

- **Real-time Multiplayer**: Play with friends in real-time using SignalR
- **Game Sharing**: Share game links to invite friends directly
- **Beautiful UI**: Modern, responsive design with smooth animations
- **Auto-matching**: Join available games or create new ones
- **Game Results**: See detailed game statistics and play again
- **Mobile Friendly**: Responsive design works on all devices

## 🏗️ Architecture

- **Backend**: ASP.NET Core 9.0 with SignalR for real-time communication
- **Frontend**: React 19 with TypeScript and modern CSS
- **State Management**: Custom React hooks for game state
- **Real-time**: SignalR hubs for instant game updates
- **Styling**: Beautiful gradient backgrounds with glassmorphism effects

## 🚀 Getting Started

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js](https://nodejs.org/) (version 18 or higher)
- [Yarn](https://yarnpkg.com/) package manager

### Installation

1. **Clone the repository** (if using git):
   ```bash
   git clone <repository-url>
   cd TicTacToe
   ```

2. **Install frontend dependencies**:
   ```bash
   cd tictactoe.client
   yarn install
   cd ..
   ```

3. **Build the application**:
   ```bash
   dotnet build TicTacToe.Server
   ```

### Running the Application

1. **Start the server** (from the project root):
   ```bash
   dotnet run --project TicTacToe.Server
   ```

2. **Open your browser** and navigate to:
   - https://localhost:7146 (or the URL shown in the console)

The application will automatically serve both the backend API and the React frontend.

## 🎯 How to Play

1. **Enter your name** and click "Create Game"
2. **Share the game link** with a friend or wait for someone to join
3. **Take turns** clicking on the grid to place X's and O's
4. **Win** by getting three symbols in a row (horizontal, vertical, or diagonal)
5. **Play again** when the game ends

## 📁 Project Structure

```
TicTacToe/
├── TicTacToe.Server/          # ASP.NET Core backend
│   ├── Models/                # Game, Player, and state models
│   ├── Services/              # Game logic and state management
│   ├── Hubs/                  # SignalR real-time communication
│   └── app_Start/             # Application startup configuration
├── tictactoe.client/          # React frontend
│   ├── src/
│   │   ├── components/Game/   # Game UI components
│   │   ├── hooks/             # Custom React hooks
│   │   ├── services/          # SignalR client service
│   │   └── types/             # TypeScript type definitions
│   └── public/                # Static assets
└── README.md                  # This file
```

## 🔧 Development

### Backend Development

The backend uses:
- **ASP.NET Core 9.0** for the web framework
- **SignalR** for real-time communication
- **In-memory state management** for game data
- **Clean architecture** with separate models, services, and hubs

Key files:
- `GameHub.cs` - SignalR hub for real-time game events
- `GameService.cs` - Game logic and state management
- `Game.cs`, `Player.cs` - Domain models

### Frontend Development

The frontend uses:
- **React 19** with TypeScript
- **Custom hooks** for state management
- **SignalR client** for real-time updates
- **Modern CSS** with animations and responsive design

Key files:
- `useGameState.ts` - Main game state hook
- `gameService.ts` - SignalR client wrapper
- `GameBoard.tsx` - Interactive game grid
- `WaitingRoom.tsx` - Player lobby and game joining

### Building for Production

```bash
# Build backend
dotnet build TicTacToe.Server --configuration Release

# Build frontend
cd tictactoe.client
yarn build
cd ..

# Publish (optional)
dotnet publish TicTacToe.Server --configuration Release
```

## 🎨 Design Features

- **Gradient Backgrounds**: Beautiful purple-to-blue gradients
- **Glassmorphism**: Translucent panels with backdrop blur
- **Smooth Animations**: Hover effects, transitions, and loading spinners
- **Responsive Design**: Works perfectly on desktop and mobile
- **Visual Feedback**: Clear indicators for turns, wins, and game states

## 🚀 Deployment

The application can be deployed to:
- **Azure App Service**
- **IIS**
- **Docker containers**
- **Any hosting platform supporting ASP.NET Core**

For production deployment, ensure:
- CORS settings are configured for your domain
- SignalR is properly configured for your hosting environment
- Static files are served correctly

## 🔮 Future Enhancements

- User accounts and player statistics
- Game history and replay system
- Tournament mode with brackets
- AI opponent for single-player mode
- Custom game room names
- Spectator mode
- Game recordings and sharing

## 📝 License

This project is open source and available under the [MIT License](LICENSE).

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📧 Support

If you encounter any issues or have questions, please create an issue in the repository.

---

**Built with ❤️ using ASP.NET Core and React** 
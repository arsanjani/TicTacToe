# 🎮 Multiplayer Tic Tac Toe Game

A real-time multiplayer tic-tac-toe game built with ASP.NET Core 9.0, SignalR, and React 19. Players can create games, share links with friends, and play in real-time with beautiful, modern UI.

## 🌐 Live Demo

**Try the game now**: [https://apponline.ir](https://apponline.ir)

## ✨ Features

- **Real-time Multiplayer**: Play with friends in real-time using SignalR
- **Multiple Board Sizes**: Choose between 3x3 classic or 4x4 extended boards
- **AI Opponent**: Single-player mode with intelligent AI for practice
- **Character Customization**: 11 unique character icons including X/O and cute characters
- **Private Games**: Create private games accessible only via share link
- **Game Sharing**: Share game links to invite friends directly
- **Database Persistence**: Complete game history and statistics tracking
- **Beautiful UI**: Modern, responsive design with smooth animations
- **Auto-matching**: Join available games or create new ones
- **Game Results**: See detailed game statistics and play again
- **Mobile Friendly**: Responsive design works perfectly on all devices

## 🎯 How to Play

1. **Enter your name** and **choose your character** from 11 available options
2. **Select board size**: Classic 3x3 or Extended 4x4
3. **Choose game type**:
   - **Multiplayer**: Share the game link with a friend or wait for someone to join
   - **AI Mode**: Start playing immediately against an intelligent AI opponent
   - **Private Game**: Create a private game only accessible via share link
4. **Take turns** clicking on the grid to place your character symbols
5. **Win** by getting symbols in a row (3 for 3x3 board, 4 for 4x4 board)
6. **Play again** with the same opponent or leave to start a new game

## 🎨 Character Options

Choose from 11 unique character icons:
- **Classic**: Cross (X), Circle (O)
- **Sanrio Characters**: Hello Kitty, My Melody, Kuromi, Cinnamoroll, Badtz-Maru, Keroppi, Pochacco
- **Special**: Spider-Man, AI Robot

## 🏗️ Architecture

- **Backend**: ASP.NET Core 9.0 with SignalR for real-time communication
- **Database**: Entity Framework Core with SQL Server for persistence
- **Frontend**: React 19 with TypeScript and modern CSS
- **State Management**: Custom React hooks for game state
- **Real-time**: SignalR hubs for instant game updates
- **Data Layer**: AutoMapper for DTO mapping and generic repository pattern
- **Styling**: Beautiful gradient backgrounds with glassmorphism effects

## 🚀 Getting Started

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js](https://nodejs.org/) (version 18 or higher)
- [Yarn](https://yarnpkg.com/) package manager
- [SQL Server](https://www.microsoft.com/en-us/sql-server) (LocalDB included with Visual Studio)

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

3. **Set up the database**:
   ```bash
   cd TicTacToe.Server
   dotnet ef database update
   cd ..
   ```

4. **Build the application**:
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

### Database Setup

The application uses SQL Server LocalDB by default. The database will be created automatically when you run the application for the first time. To reset the database:

```bash
cd TicTacToe.Server
# Windows
.\reset-migrations.bat
# Or manually
dotnet ef database drop --force
dotnet ef database update
```

## 📁 Project Structure

```
TicTacToe/
├── TicTacToe.Server/          # ASP.NET Core backend
│   ├── Models/                # Game, Player, and state models
│   ├── Entities/              # Entity Framework entities
│   ├── Data/                  # Database context and configurations
│   ├── Services/              # Game logic and AI implementation
│   ├── Hubs/                  # SignalR real-time communication
│   ├── Controllers/           # REST API controllers
│   ├── DTOs/                  # Data transfer objects
│   ├── Mappings/              # AutoMapper profiles
│   └── Migrations/            # Entity Framework migrations
├── tictactoe.client/          # React frontend
│   ├── src/
│   │   ├── components/Game/   # Game UI components
│   │   ├── hooks/             # Custom React hooks
│   │   ├── services/          # SignalR client service
│   │   ├── types/             # TypeScript type definitions
│   │   ├── theme/             # Comprehensive CSS styling
│   │   └── utils/             # Utility functions
│   └── public/                # Static assets and character icons
├── sql/                       # Database migration scripts
└── README.md                  # This file
```

## 🔧 Development

### Backend Development

The backend uses:
- **ASP.NET Core 9.0** for the web framework
- **Entity Framework Core** with SQL Server for data persistence
- **SignalR** for real-time communication
- **AutoMapper** for object mapping
- **Generic repository pattern** for data access
- **AI implementation** for single-player mode

Key files:
- `GameHub.cs` - SignalR hub for real-time game events
- `GameService.cs` - Game logic, AI implementation, and state management
- `Game.cs`, `Player.cs` - Domain models with dynamic board size support
- `TicTacToeDbContext.cs` - Entity Framework database context

### Frontend Development

The frontend uses:
- **React 19** with TypeScript
- **Custom hooks** for comprehensive state management
- **SignalR client** for real-time updates
- **Modern CSS** with extensive responsive design
- **Character icon system** with 11 unique options

Key files:
- `useGameState.ts` - Main game state hook with AI support
- `gameService.ts` - SignalR client wrapper
- `GameBoard.tsx` - Interactive game grid with character icons
- `GameLobby.tsx` - Game creation with board size and AI options
- `WaitingRoom.tsx` - Player lobby and game joining

### AI Implementation

The AI opponent features:
- **Strategic gameplay**: Win, block, center, corners, edges priority
- **Adaptive difficulty**: Works with both 3x3 and 4x4 boards
- **Natural timing**: Realistic move delays for better UX
- **Fair play**: Random first player selection

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
- **Character Icons**: 11 unique, colorful character options
- **Responsive Design**: Optimized for all screen sizes (320px to 1440px+)
- **Smooth Animations**: Hover effects, transitions, and loading spinners
- **Visual Feedback**: Clear indicators for turns, wins, and game states
- **Board Size Adaptation**: Dynamic layouts for 3x3 and 4x4 boards

## 🗄️ Database Features

- **Game History**: Complete tracking of all games played
- **Player Statistics**: Win/loss records and game duration
- **Session Management**: Proper cleanup and state management
- **Performance Optimized**: Indexed queries and efficient data structure
- **Migration Support**: Easy database updates and schema changes

## 🚀 Deployment

The application can be deployed to:
- **Azure App Service** with SQL Database
- **IIS** with SQL Server
- **Docker containers** with multi-container support
- **Any hosting platform supporting ASP.NET Core**

For production deployment, ensure:
- Database connection strings are configured
- CORS settings are configured for your domain
- SignalR is properly configured for your hosting environment
- Static files are served correctly
- SSL certificates are properly configured

## 🔮 Recent Updates (v2.0)

### Major New Features
- ✅ **Multiple Board Sizes**: 3x3 classic and 4x4 extended boards
- ✅ **AI Opponent**: Intelligent single-player mode
- ✅ **Character Customization**: 11 unique character icons
- ✅ **Database Persistence**: Complete game history tracking
- ✅ **Private Games**: Create private, link-only games
- ✅ **Enhanced UI**: Improved responsive design for all devices

### Technical Improvements
- ✅ **Entity Framework Integration**: Full database persistence
- ✅ **Advanced AI Logic**: Strategic gameplay for all board sizes
- ✅ **Performance Optimization**: Indexed database queries
- ✅ **Code Architecture**: Clean separation with DTOs and AutoMapper
- ✅ **Comprehensive Testing**: Improved error handling and validation

## 🔮 Future Enhancements

- User accounts and authentication
- Player rankings and leaderboards
- Tournament mode with brackets
- Spectator mode for ongoing games
- Game replay system
- Custom character upload
- Mobile app versions
- Advanced AI difficulty levels

## 📝 License

This project is open source and available under the [MIT License](LICENSE).

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📧 Support

If you encounter any issues or have questions, please create an issue in the repository.

---

**Built with ❤️ using ASP.NET Core, Entity Framework, and React** 
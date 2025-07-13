# Theme Folder Structure

This folder contains all CSS styles for the TicTacToe application, organized for better maintainability and readability.

## Folder Structure

```
theme/
├── index.css                    # Main theme file that imports all styles
├── global/                      # Global application styles
│   ├── index.css               # Base styles, resets, and global variables
│   └── app.css                 # App-level component styles
└── components/                  # Component-specific styles
    └── game/                   # Game-related component styles
        ├── GameBoard.css       # Game board styling with responsive design
        ├── GameLobby.css       # Game lobby and setup styling
        ├── WaitingRoom.css     # Waiting room and game joining styling
        └── GameResult.css      # Game result modal styling
```

## Features

- **Responsive Design**: All components include comprehensive responsive breakpoints:
  - Large Desktop (1440px+)
  - Desktop (1024px - 1439px)
  - Tablet (768px - 1023px)
  - Mobile Large (480px - 767px)
  - Mobile Small (320px - 479px)
  - Extra small screens (< 320px)

- **Touch-Friendly**: Optimized for touch devices with appropriate touch targets and interactions

- **Accessibility**: Includes accessibility improvements for reduced motion preferences

- **Dark Mode**: Support for dark mode preferences

- **Modern Features**: Utilizes modern CSS features like:
  - CSS Grid and Flexbox
  - CSS Custom Properties (CSS Variables)
  - Backdrop filters
  - CSS animations and transitions
  - Modern color gradients

## Usage

### Individual Imports (Current Implementation)
Components import their specific CSS files:
```tsx
import '../../theme/components/game/GameBoard.css';
```

### Centralized Import (Alternative)
You can also import all styles at once using the main theme file:
```tsx
import './theme/index.css';
```

## Responsive Breakpoints

The theme uses a mobile-first approach with the following breakpoints:

- **Mobile Small**: Up to 479px
- **Mobile Large**: 480px to 767px  
- **Tablet**: 768px to 1023px
- **Desktop**: 1024px to 1439px
- **Large Desktop**: 1440px and up

## Design System

The theme follows a consistent design system with:

- **Color Palette**: Blue-purple gradient theme with semantic colors
- **Typography**: System font stack with appropriate font scales
- **Spacing**: Consistent spacing scale using rem units
- **Shadows**: Layered shadow system for depth
- **Border Radius**: Consistent border radius scale
- **Transitions**: Smooth animations for better UX

## Maintenance

When adding new components:

1. Create CSS file in appropriate folder under `components/`
2. Add responsive breakpoints following existing patterns
3. Include accessibility and touch-friendly considerations
4. Add import to `index.css` if using centralized imports
5. Update this README if adding new categories 
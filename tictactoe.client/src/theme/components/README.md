# Reusable Components

This directory contains CSS and utility files for reusable components that can be used across the TicTacToe application.

## Confirmation Dialog Component

### Overview
The confirmation component provides a consistent, styled SweetAlert2 dialog system for user confirmations throughout the application. It includes predefined themes and utility functions for common confirmation scenarios.

### Files
- `confirmation.css` - CSS styles for SweetAlert2 dialogs
- `../utils/confirmationDialog.ts` - TypeScript utility functions

### CSS Features

#### Base Styling
- Custom gradient backgrounds matching the game theme
- Glassmorphism effects with backdrop blur
- Responsive design for mobile devices
- Accessibility support (reduced motion, high contrast)

#### Theme Variants
- **Default**: Purple gradient (matches game board)
- **Success**: Green gradient for positive actions
- **Info**: Blue gradient for informational dialogs
- **Dark**: Dark gradient for professional contexts

#### Responsive Design
- Mobile-first approach
- Stacked buttons on small screens
- Appropriate font sizes and spacing
- Touch-friendly button sizes

### JavaScript Utilities

#### Basic Functions

```typescript
import { 
  showDangerConfirmation, 
  showSuccessConfirmation, 
  showInfoConfirmation, 
  showAlert 
} from '../utils/confirmationDialog';

// For dangerous actions (red confirm button)
const confirmed = await showDangerConfirmation(
  'Delete Account?',
  'This action cannot be undone.'
);

// For success confirmations (green theme)
const continue = await showSuccessConfirmation(
  'Save Successful!',
  'Would you like to continue editing?'
);

// For informational confirmations (blue theme)
const understood = await showInfoConfirmation(
  'New Feature',
  'We\'ve added multiplayer support!'
);

// For simple alerts (no cancel option)
await showAlert('Welcome!', 'Thanks for joining!', 'success');
```

#### Advanced Usage

```typescript
import { showConfirmationDialog } from '../utils/confirmationDialog';

// Custom configuration
const result = await showConfirmationDialog({
  title: 'Custom Dialog',
  text: 'This is a custom confirmation dialog.',
  icon: 'question',
  confirmButtonText: 'Proceed',
  cancelButtonText: 'Go Back',
  theme: 'info',
  dangerousAction: false
});

if (result.isConfirmed) {
  // User clicked "Proceed"
} else {
  // User clicked "Go Back" or dismissed
}
```

### Usage Examples

#### Game Actions
```typescript
// Leaving a game
const leaveConfirmed = await showDangerConfirmation(
  'Leave Game?',
  'Are you sure you want to leave the game? This action cannot be undone.'
);

// Resetting a game
const resetConfirmed = await showDangerConfirmation(
  'Reset Game?',
  'This will restart the current game.'
);
```

#### User Management
```typescript
// Account deletion
const deleteConfirmed = await showDangerConfirmation(
  'Delete Account?',
  'Your account and all data will be permanently deleted.'
);

// Profile updates
const saveConfirmed = await showSuccessConfirmation(
  'Profile Updated!',
  'Would you like to continue editing your profile?'
);
```

#### Feature Announcements
```typescript
// New features
const tryFeature = await showInfoConfirmation(
  'New Characters Available!',
  'We\'ve added 5 new character icons. Want to check them out?'
);

// Updates
await showAlert(
  'Game Updated!',
  'Version 2.0 includes new themes and sounds.',
  'success'
);
```

### Customization

#### Adding New Themes
To add a new theme, update both the CSS and TypeScript files:

1. **CSS** (`confirmation.css`):
```css
/* New theme variant */
.swal-custom-theme .swal-popup {
  background: linear-gradient(135deg, #your-color 0%, #your-color-2 100%) !important;
}

.swal-custom-theme .swal-confirm-button {
  background: linear-gradient(135deg, #button-color, #button-color-dark) !important;
  box-shadow: 0 4px 15px rgba(your-color-rgb, 0.3) !important;
}
```

2. **TypeScript** (`confirmationDialog.ts`):
```typescript
// Update the theme type
theme?: 'default' | 'success' | 'info' | 'dark' | 'custom';

// Add case in switch statement
case 'custom':
  customClass = 'swal-custom-theme';
  break;
```

#### Styling Guidelines
- Use `!important` for SweetAlert2 overrides
- Follow existing naming conventions (`swal-*`)
- Maintain consistent spacing and typography
- Ensure accessibility compliance
- Test on mobile devices

### Best Practices

1. **Use appropriate confirmation types**:
   - `showDangerConfirmation()` for destructive actions
   - `showSuccessConfirmation()` for positive confirmations
   - `showInfoConfirmation()` for neutral information
   - `showAlert()` for simple notifications

2. **Write clear, concise text**:
   - Keep titles short and descriptive
   - Explain consequences in the description
   - Use action-oriented button text

3. **Consider user experience**:
   - Don't overuse confirmations
   - Make destructive actions clearly distinguishable
   - Provide clear cancel options

4. **Accessibility**:
   - All dialogs support keyboard navigation
   - Screen reader compatible
   - Respects user's motion preferences

### Dependencies
- SweetAlert2 (^11.22.2)
- Modern browser with CSS Grid support
- TypeScript support for type safety

### Browser Support
- Chrome 70+
- Firefox 63+
- Safari 12+
- Edge 79+
- Mobile browsers (iOS Safari 12+, Chrome Mobile 70+) 
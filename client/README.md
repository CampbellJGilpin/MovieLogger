# MovieLogger Client

The frontend application for MovieLogger, built with React, TypeScript, and Vite.

## Tech Stack

- **Framework**: React 18
- **Language**: TypeScript
- **Build Tool**: Vite
- **Styling**: Tailwind CSS
- **Icons**: Heroicons
- **Components**: Headless UI

## Project Structure

```
client/
├── src/
│   ├── components/     # React components
│   │   ├── common/     # Shared components
│   │   └── movies/     # Movie-related components
│   ├── services/       # API communication
│   ├── types/         # TypeScript interfaces
│   ├── App.tsx        # Main application component
│   └── main.tsx       # Application entry point
└── package.json
```

## Key Features

- Movie details view with three-column layout
- Star rating system for reviews
- Toggle switches for movie status (Owned, Watch Later, Favorite)
- View history with recent reviews
- Responsive grid layouts

## Getting Started

1. Install dependencies:
```bash
npm install
```

2. Start development server:
```bash
npm run dev
```

3. Build for production:
```bash
npm run build
```

## Development

### Available Scripts

- `npm run dev` - Start development server
- `npm run build` - Build for production
- `npm run lint` - Run ESLint
- `npm run preview` - Preview production build

### ESLint Configuration

The project uses ESLint with TypeScript support. To enable stricter type checking, update `eslint.config.js`:

```js
export default tseslint.config({
  extends: [
    ...tseslint.configs.recommendedTypeChecked,
    ...tseslint.configs.strictTypeChecked,
  ],
  languageOptions: {
    parserOptions: {
      project: ['./tsconfig.node.json', './tsconfig.app.json'],
      tsconfigRootDir: import.meta.dirname,
    },
  },
})
```

## Component Guidelines

- Use TypeScript interfaces for props
- Follow Tailwind CSS conventions for styling
- Implement responsive designs
- Use Headless UI for interactive components
- Include proper error handling
- Add loading states where appropriate

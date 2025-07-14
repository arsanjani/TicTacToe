import { fileURLToPath, URL } from 'node:url';

import { defineConfig } from 'vite';
import plugin from '@vitejs/plugin-react';
import { env } from 'process';

// Use HTTP target instead of HTTPS
const target = env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'http://localhost:5272';

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [plugin()],
    resolve: {
        alias: {
            '@': fileURLToPath(new URL('./src', import.meta.url))
        }
    },
    server: {
        proxy: {
            '^/weatherforecast': {
                target,
                secure: false
            },
            '^/gameHub': {
                target,
                secure: false,
                ws: true, // Enable WebSocket proxying for SignalR
                changeOrigin: true
            },
            '^/game_icons': {
                target,
                secure: false,
                changeOrigin: true
            }
        },
        port: 55577,
        // Remove HTTPS configuration - use HTTP instead
    }
})

import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'

// https://vite.dev/config/
export default defineConfig({
    plugins: [react(), tailwindcss()],


    // Configures Vitest for testing React components.
    test: {
        environment: 'jsdom',
        setupFiles: './src/test/setup.js',
        css: 'true'
    }
})
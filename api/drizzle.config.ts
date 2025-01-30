import { defineConfig } from 'drizzle-kit';

export default defineConfig({
    schema: './src/data/schema',
    out: './src/data/drizzle',
    dialect: 'postgresql',
    dbCredentials: {
        url: process.env.DEV_DB_CONNECTION ?? '',
    },
});
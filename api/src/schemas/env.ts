import { z } from 'zod';

export const envSchema = z.object({
    DEV_DB_CONNECTION: z.string(),
    PROD_DB_CONNECTION: z.string(),
    JWT_SECRET: z.string(),
    ANON_KEY: z.string(),
});

export type AppEnvVariables = z.infer<typeof envSchema>;
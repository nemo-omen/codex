import { createFactory } from "hono/factory";
import { type AppEnvVariables, envSchema } from "./schemas/env";
import { db } from "./data/db";
import { NodePgDatabase } from "drizzle-orm/node-postgres";

type AppVariables = {
    db: NodePgDatabase;
};

export type Variables = AppVariables & AppEnvVariables;
export const envVariables = envSchema.parse(process.env);

export const factory = createFactory<{ Variables: Variables; }>({
    initApp: (app) => {
        app.use(async (c, next) => {
            c.set('db', db);
            for (const [key, value] of Object.entries(envVariables)) {
                c.set(key as keyof AppEnvVariables, value);
            }
            await next();
        });
    },
});
import { drizzle } from "drizzle-orm/node-postgres";

const connectionString = process.env.DEV_DB_CONNECTION;

if (!connectionString) throw new Error("DEV_DB_CONNECTION is not set");

export const db = drizzle({ connection: connectionString, casing: 'snake_case' });
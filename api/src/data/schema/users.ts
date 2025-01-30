import { pgTable, text, uuid } from "drizzle-orm/pg-core";
import { timestamps } from "./timestamps";
import { pgEnum } from "drizzle-orm/pg-core";
import * as t from "drizzle-orm/pg-core";
import { uniqueIndex } from "drizzle-orm/pg-core";
import { relations } from "drizzle-orm";
import { bookmarks } from "./bookmarks";

export const rolesEnum = pgEnum("roles", ["admin", "user", "guest"]);

export const users = pgTable("users", {
    id: t.uuid().primaryKey().defaultRandom(),
    email: t.varchar({ length: 256 }).notNull(),
    password: t.varchar({ length: 256 }).notNull(),
    username: t.varchar({ length: 256 }),
    role: rolesEnum().default("guest"),
    ...timestamps
},
    (table) => {
        return [
            uniqueIndex("email_index").on(table.email),
            uniqueIndex("username_index").on(table.username)
        ];
    }
);

export const usersRelations = relations(users, ({ many }) => ({
    bookmarks: many(bookmarks)
}));

import { pgTable, text, uuid } from "drizzle-orm/pg-core";
import { timestamps } from "./timestamps";
import { users } from "./users";
import * as t from "drizzle-orm/pg-core";
import { relations } from "drizzle-orm";

export const bookmarks = pgTable("bookmarks", {
    id: t.uuid().primaryKey().defaultRandom(),
    userId: t.uuid().notNull(),
    url: t.varchar({ length: 256 }).notNull(),
    title: t.varchar({ length: 256 }),
    description: t.varchar({ length: 512 }),
    ...timestamps
},
    (table) => {
        return [
            t.index("userId_index").on(table.userId),
            t.index("url_index").on(table.url)
        ];
    }
);

export const bookmarksRelations = relations(bookmarks, ({ one }) => ({
    user: one(users, {
        fields: [bookmarks.userId],
        references: [users.id]
    })
}));
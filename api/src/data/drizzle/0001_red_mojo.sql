CREATE TABLE "bookmarks" (
	"id" uuid PRIMARY KEY DEFAULT gen_random_uuid() NOT NULL,
	"userId" uuid NOT NULL,
	"url" varchar(256) NOT NULL,
	"title" varchar(256),
	"description" varchar(512),
	"created_at" timestamp DEFAULT now() NOT NULL,
	"updated_at" timestamp,
	"deleted_at" timestamp
);
--> statement-breakpoint
CREATE INDEX "userId_index" ON "bookmarks" USING btree ("userId");--> statement-breakpoint
CREATE INDEX "url_index" ON "bookmarks" USING btree ("url");
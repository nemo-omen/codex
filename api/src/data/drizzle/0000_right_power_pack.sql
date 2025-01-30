CREATE TYPE "public"."roles" AS ENUM('admin', 'user', 'guest');--> statement-breakpoint
CREATE TABLE "users" (
	"id" uuid PRIMARY KEY DEFAULT gen_random_uuid() NOT NULL,
	"email" varchar(256) NOT NULL,
	"password" varchar(256) NOT NULL,
	"username" varchar(256),
	"role" "roles" DEFAULT 'guest',
	"created_at" timestamp DEFAULT now() NOT NULL,
	"updated_at" timestamp,
	"deleted_at" timestamp
);
--> statement-breakpoint
CREATE UNIQUE INDEX "email_index" ON "users" USING btree ("email");--> statement-breakpoint
CREATE UNIQUE INDEX "username_index" ON "users" USING btree ("username");
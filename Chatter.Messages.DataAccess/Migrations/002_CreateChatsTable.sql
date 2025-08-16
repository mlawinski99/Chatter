CREATE SCHEMA IF NOT EXISTS chat;

CREATE TABLE chat."Chats" (
                              "Id" UUID NOT NULL PRIMARY KEY,
                              "Type" VARCHAR(20) NOT NULL,
                              "Name" VARCHAR(1024) NULL,
                              "DateCreatedUtc" TIMESTAMP NOT NULL,
                              "DateModifiedUtc" TIMESTAMP NULL
);

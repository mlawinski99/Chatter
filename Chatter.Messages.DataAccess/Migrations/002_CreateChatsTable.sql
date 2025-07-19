CREATE SCHEMA IF NOT EXISTS chat;

CREATE TABLE chat."Chats" (
                              "Id" UUID NOT NULL PRIMARY KEY,
                              "Type" VARCHAR(20) NOT NULL,
                              "CreatedAt" TIMESTAMP NOT NULL,
                              "UpdatedAt" TIMESTAMP NULL
);

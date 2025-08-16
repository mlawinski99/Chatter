CREATE TABLE chat."Messages"
(
    "Id" UUID NOT NULL PRIMARY KEY,
    "DateCreatedUtc" TIMESTAMP NOT NULL,
    "DateModifiedUtc" TIMESTAMP NULL,
    "CreatedBy" UUID NULL,
    "ModifiedBy" UUID NULL,
    "Status" VARCHAR(30) NOT NULL,
    "Content" TEXT NOT NULL,
    "DateDeletedUtc" TIMESTAMP NULL,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "VersionId" INT NOT NULL,
    "VersionGroupId" UUID NULL,
    "SenderId" UUID NOT NULL,
    "ChatId" UUID NOT NULL,
    
    CONSTRAINT "FK_Messages_Chats_ChatId" FOREIGN KEY ("ChatId") REFERENCES chat."Chats"("Id"),
    CONSTRAINT "FK_Messages_Users_SenderId" FOREIGN KEY ("SenderId") REFERENCES public."Users"("Id")
);

CREATE INDEX "IX_Messages_ChatId"
    ON chat."Messages" ("ChatId");

CREATE INDEX "IX_Messages_SenderId"
    ON chat."Messages" ("SenderId");
CREATE TABLE chat."ChatMembers" (
                                    "Id" UUID NOT NULL PRIMARY KEY,
                                    "DateCreatedUtc" TIMESTAMP NOT NULL,
                                    "DateModifiedUtc" TIMESTAMP NULL,
                                    "CreatedBy" UUID,
                                    "UpdatedBy" UUID,
                                    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
                                    "DeletedAt" TIMESTAMP NULL,
                                    "ChatId" UUID NOT NULL,
                                    "UserId" UUID NOT NULL,

                                    CONSTRAINT "FK_ChatMembers_ChatId" FOREIGN KEY ("ChatId") REFERENCES chat."Chats"("Id"),
                                    CONSTRAINT "FK_ChatMembers_UserId" FOREIGN KEY ("UserId") REFERENCES public."Users"("Id")
);

CREATE INDEX "IX_ChatMembers_ChatId"
    ON chat."ChatMembers" ("ChatId");

CREATE INDEX "IX_ChatMembers_UserId"
    ON chat."ChatMembers" ("UserId");

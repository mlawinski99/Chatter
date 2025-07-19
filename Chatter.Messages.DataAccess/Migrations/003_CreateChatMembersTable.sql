CREATE TABLE chat."ChatMembers" (
                                    "Id" UUID NOT NULL PRIMARY KEY,
                                    "CreatedAt" TIMESTAMP NOT NULL,
                                    "UpdatedAt" TIMESTAMP NULL,
                                    "CreatedByUserId" UUID,
                                    "UpdatedByUserId" UUID,
                                    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
                                    "DeletedAt" TIMESTAMP NULL,
                                    "ChatId" UUID NOT NULL,
                                    "UserId" UUID NOT NULL,

                                    CONSTRAINT "FK_ChatMembers_ChatId" FOREIGN KEY ("ChatId") REFERENCES chat."Chats"("Id"),
                                    CONSTRAINT "FK_ChatMembers_UserId" FOREIGN KEY ("UserId") REFERENCES public."Users"("Id")
);

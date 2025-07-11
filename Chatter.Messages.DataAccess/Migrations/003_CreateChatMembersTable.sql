CREATE TABLE chat.ChatMembers (
                                  Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
                                  CreatedAt DATETIME2 NOT NULL,
                                  UpdatedAt DATETIME2 NULL,
                                  CreatedByUserId UNIQUEIDENTIFIER,
                                  UpdatedByUserId UNIQUEIDENTIFIER,
                                  IsDeleted BIT NOT NULL DEFAULT 0,
                                  DeletedAt DATETIME2 NULL,
                                  ChatId UNIQUEIDENTIFIER NOT NULL,
                                  UserId UNIQUEIDENTIFIER NOT NULL,

                                  CONSTRAINT FK_ChatMembers_ChatId FOREIGN KEY (ChatId) REFERENCES chat.Chats(Id),
                                  CONSTRAINT FK_ChatMembers_UserId FOREIGN KEY (UserId) REFERENCES dbo.Users(Id)
);
CREATE TABLE chat.Chats (
                            Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
                            Type VARCHAR(20) NOT NULL,
                            CreatedAt DATETIME2 NOT NULL,
                            UpdatedAt DATETIME2 NULL
);

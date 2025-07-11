CREATE TABLE dbo.Users (
                           Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
                           UserName NVARCHAR(1024) NOT NULL,
                           Email NVARCHAR(1024) NOT NULL,
                           DateCreatedUtc DATETIME2 NOT NULL,
                           DateModifiedUtc DATETIME2 NULL,
                           DateDeletedUtc DATETIME2 NULL,
                           IsDeleted BIT NOT NULL DEFAULT 0
);

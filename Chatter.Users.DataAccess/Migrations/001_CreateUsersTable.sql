CREATE TABLE public."Users" (
                             "Id" UUID NOT NULL PRIMARY KEY,
                             "UserName" VARCHAR(1024) NOT NULL,
                             "Email" VARCHAR(1024) NOT NULL,
                             "DateCreatedUtc" TIMESTAMP NOT NULL,
                             "DateModifiedUtc" TIMESTAMP NULL,
                             "DateDeletedUtc" TIMESTAMP NULL,
                             "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE
);
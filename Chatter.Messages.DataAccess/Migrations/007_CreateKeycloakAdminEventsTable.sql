CREATE TABLE public."KeycloakAdminEvents" (
                                              "Id"             BIGSERIAL      PRIMARY KEY,
                                              "OperationType"  VARCHAR(20)    NOT NULL,
                                              "ResourceType"   VARCHAR(50)    NOT NULL,
                                              "ResourcePath"   VARCHAR(255)   NOT NULL,
                                              "Time"           TIMESTAMP      NOT NULL,
                                              "IsProcessed"    BOOLEAN        NOT NULL   DEFAULT FALSE
);

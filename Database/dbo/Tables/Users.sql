CREATE TABLE [dbo].[Users] (
    [UserId]             INT            IDENTITY (1, 1) NOT NULL,
    [UserName]           NVARCHAR (256) NOT NULL,
    [NormalizedUserName] NVARCHAR (256) NOT NULL,
    [PasswordHash]       NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([UserId] ASC)
);


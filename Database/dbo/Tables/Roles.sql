CREATE TABLE [dbo].[Roles] (
    [RoleId]         INT            IDENTITY (1, 1) NOT NULL,
    [Name]           NVARCHAR (256) NOT NULL,
    [NormalizedName] NVARCHAR (256) NOT NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED ([RoleId] ASC)
);


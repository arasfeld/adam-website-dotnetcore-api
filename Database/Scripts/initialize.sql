CREATE DATABASE [AdamWebsite];

GO

USE [AdamWebsite];

GO

CREATE TABLE [dbo].[Images] (
	[ImageId]     INT            IDENTITY (1, 1) NOT NULL,
	[FileName]    NVARCHAR (100) NOT NULL,
	[ContentType] NVARCHAR (50)  NOT NULL,
	[Timestamp]   DATETIME       CONSTRAINT [DF_Images_Timestamp] DEFAULT (getutcdate()) NOT NULL,
	CONSTRAINT [PK_Images] PRIMARY KEY CLUSTERED ([ImageId] ASC)
)

GO

CREATE TABLE [dbo].[Photos] (
	[PhotoId]   INT             IDENTITY (1, 1) NOT NULL,
	[ImageId]   INT             NOT NULL,
	[Title]     NVARCHAR (500)  NULL,
	[Caption]   NVARCHAR (1000) NULL,
	[Timestamp] DATETIME        CONSTRAINT [DF_Photos_Timestamp] DEFAULT (getutcdate()) NOT NULL,
	CONSTRAINT [PK_Photos] PRIMARY KEY CLUSTERED ([PhotoId] ASC),
	CONSTRAINT [FK_Photos_Images] FOREIGN KEY ([ImageId]) REFERENCES [dbo].[Images] ([ImageId])
)

GO

CREATE TABLE [dbo].[Posts] (
	[PostId]    INT            IDENTITY (1, 1) NOT NULL,
	[ImageId]   INT            NULL,
	[Title]     NVARCHAR (500) NOT NULL,
	[Body]      NVARCHAR (MAX) NOT NULL,
	[Timestamp] DATETIME       CONSTRAINT [DF_Posts_Timestamp] DEFAULT (getutcdate()) NOT NULL,
	CONSTRAINT [PK_Posts] PRIMARY KEY CLUSTERED ([PostId] ASC),
	CONSTRAINT [FK_Posts_Images] FOREIGN KEY ([ImageId]) REFERENCES [dbo].[Images] ([ImageId])
)

GO

CREATE TABLE [dbo].[Users] (
	[UserId]             INT            IDENTITY (1, 1) NOT NULL,
	[UserName]           NVARCHAR (256) NOT NULL,
	[NormalizedUserName] NVARCHAR (256) NOT NULL,
	[PasswordHash]       NVARCHAR (MAX) NULL,
	CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([UserId] ASC)
)

GO

CREATE TABLE [dbo].[Roles] (
	[RoleId]         INT            IDENTITY (1, 1) NOT NULL,
	[Name]           NVARCHAR (256) NOT NULL,
	[NormalizedName] NVARCHAR (256) NOT NULL,
	CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED ([RoleId] ASC)
)

GO

CREATE TABLE [dbo].[UserRoles] (
	[UserId] INT NOT NULL,
	[RoleId] INT NOT NULL,
	CONSTRAINT [PK_UserRoles] PRIMARY KEY CLUSTERED ([UserId] ASC, [RoleId] ASC),
	CONSTRAINT [FK_UserRoles_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([RoleId]),
	CONSTRAINT [FK_UserRoles_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId])
)

GO

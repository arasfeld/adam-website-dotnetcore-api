CREATE TABLE [dbo].[Images] (
    [ImageId]     INT            IDENTITY (1, 1) NOT NULL,
    [FileName]    NVARCHAR (100) NOT NULL,
    [ContentType] NVARCHAR (50)  NOT NULL,
    [Timestamp]   DATETIME       CONSTRAINT [DF_Images_Timestamp] DEFAULT (getutcdate()) NOT NULL,
    CONSTRAINT [PK_Images] PRIMARY KEY CLUSTERED ([ImageId] ASC)
);


CREATE TABLE [dbo].[Posts] (
    [PostId]    INT            IDENTITY (1, 1) NOT NULL,
    [ImageId]   INT            NULL,
    [Title]     NVARCHAR (500) NOT NULL,
    [Body]      NVARCHAR (MAX) NOT NULL,
    [Timestamp] DATETIME       CONSTRAINT [DF_Posts_Timestamp] DEFAULT (getutcdate()) NOT NULL,
    CONSTRAINT [PK_Posts] PRIMARY KEY CLUSTERED ([PostId] ASC),
    CONSTRAINT [FK_Posts_Images] FOREIGN KEY ([ImageId]) REFERENCES [dbo].[Images] ([ImageId])
);


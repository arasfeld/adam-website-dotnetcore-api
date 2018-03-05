CREATE TABLE [dbo].[Photos] (
    [PhotoId]   INT             IDENTITY (1, 1) NOT NULL,
    [ImageId]   INT             NOT NULL,
    [Title]     NVARCHAR (500)  NULL,
    [Caption]   NVARCHAR (1000) NULL,
    [Timestamp] DATETIME        CONSTRAINT [DF_Photos_Timestamp] DEFAULT (getutcdate()) NOT NULL,
    CONSTRAINT [PK_Photos] PRIMARY KEY CLUSTERED ([PhotoId] ASC),
    CONSTRAINT [FK_Photos_Images] FOREIGN KEY ([ImageId]) REFERENCES [dbo].[Images] ([ImageId])
);


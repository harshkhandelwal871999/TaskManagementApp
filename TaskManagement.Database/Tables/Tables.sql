-- Tables for TaskManagementApp
CREATE TABLE [dbo].[Columns](
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(100) NOT NULL
);

CREATE TABLE [dbo].[Tasks](
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Name] NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [Deadline] DATETIME2 NULL,
    [IsFavorite] BIT NOT NULL DEFAULT 0,
    [ColumnId] INT NOT NULL,
    [ImagePath] NVARCHAR(500) NULL,
    [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_Tasks_Columns FOREIGN KEY (ColumnId) REFERENCES [dbo].[Columns](Id)
);

-- Seed columns
INSERT INTO [dbo].[Columns] ([Name]) VALUES ('ToDo'),('In Progress'),('Done');

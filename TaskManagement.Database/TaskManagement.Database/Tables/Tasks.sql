CREATE TABLE [dbo].[Tasks] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [Name] NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [Deadline] DATETIME2 NULL,
    [IsFavorite] BIT NOT NULL CONSTRAINT DF_Tasks_IsFavorite DEFAULT (0),
    [Status] TINYINT NOT NULL CONSTRAINT DF_Tasks_Status DEFAULT (0),
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT DF_Tasks_CreatedAt DEFAULT (SYSUTCDATETIME()),
    [UpdatedAt] DATETIME2 NULL,
    [IsDeleted] BIT NOT NULL,

    -- Constraints
		CONSTRAINT PK_Tasks PRIMARY KEY CLUSTERED ([Id]),
		CONSTRAINT CK_Tasks_Status CHECK ([Status] BETWEEN 0 AND 3) -- only valid enum values
);
GO;
-- Indexes
CREATE NONCLUSTERED INDEX IX_Tasks_Favorite_Name ON [dbo].[Tasks] ([IsFavorite] DESC, [Name] ASC);
Go;
CREATE NONCLUSTERED INDEX IX_Tasks_Status ON [dbo].[Tasks] ([Status]);
Go;
CREATE NONCLUSTERED INDEX IX_Tasks_CreatedAt ON dbo.Tasks(CreatedAt);
Go;
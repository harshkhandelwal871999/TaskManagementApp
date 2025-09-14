CREATE TABLE [dbo].[TaskImages] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [TaskId] BIGINT NOT NULL,
    [ImagePath] NVARCHAR(500) NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL CONSTRAINT DF_TaskImages_CreatedAt DEFAULT (SYSUTCDATETIME()),

    -- Constraints
    CONSTRAINT PK_TaskImages PRIMARY KEY CLUSTERED ([Id]),
    CONSTRAINT FK_TaskImages_Tasks FOREIGN KEY ([TaskId]) REFERENCES [dbo].[Tasks]([Id]) ON DELETE CASCADE
);
Go;
-- Index for faster queries per task
CREATE NONCLUSTERED INDEX IX_TaskImages_TaskId ON [dbo].[TaskImages] ([TaskId]);
Go;
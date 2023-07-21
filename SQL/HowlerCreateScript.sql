USE [master]

IF db_id('Howler') IS NULl
  CREATE DATABASE [Howler]
GO

USE [Howler]
GO

CREATE TABLE [User] (
  [Id] int PRIMARY KEY IDENTITY(1, 1),
  [DisplayName] nvarchar(30) UNIQUE NOT NULL,
  [Email] nvarchar(128) UNIQUE NOT NULL,
  [DateCreated] datetime NOT NULL,
  [FirebaseId] nvarchar(28) NOT NULL,
  [ProfilePictureUrl] nvarchar(255),
  [PackId] int,
  [IsBanned] bit NOT NULL DEFAULT (0)
)
GO

CREATE TABLE [Pack] (
  [Id] int PRIMARY KEY IDENTITY(1, 1),
  [Name] nvarchar(50) NOT NULL,
  [PackLeaderId] int NOT NULL,
  [Description] nvarchar(255) NOT NULL,
  [PrimaryBoardId] int
)
GO

CREATE TABLE [Board] (
  [Id] int PRIMARY KEY IDENTITY(1, 1),
  [Name] nvarchar(75) NOT NULL,
  [Topic] nvarchar(255) NOT NULL,
  [Description] nvarchar(255),
  [BoardOwnerId] int NOT NULL,
  [IsPackBoard] bit NOT NULL DEFAULT (0)
)
GO

CREATE TABLE [Post] (
  [Id] int PRIMARY KEY IDENTITY(1, 1),
  [Title] nvarchar(75) NOT NULL,
  [Content] nvarchar(1000) NOT NULL,
  [UserId] int NOT NULL,
  [BoardId] int NOT NULL,
  [CreatedOn] datetime NOT NULL
)
GO

CREATE TABLE [Comment] (
  [Id] int PRIMARY KEY IDENTITY(1, 1),
  [UserId] int NOT NULL,
  [PostId] int NOT NULL,
  [Content] nvarchar(255) NOT NULL,
  [CreatedOn] datetime NOT NULL
)
GO

ALTER TABLE [Post] ADD FOREIGN KEY ([UserId]) REFERENCES [User] ([Id])
GO

ALTER TABLE [Post] ADD FOREIGN KEY ([BoardId]) REFERENCES [Board] ([Id]) ON DELETE CASCADE
GO

ALTER TABLE [Pack] ADD FOREIGN KEY ([PrimaryBoardId]) REFERENCES [Board] ([Id]) ON DELETE SET NULL
GO

ALTER TABLE [User] ADD FOREIGN KEY ([PackId]) REFERENCES [Pack] ([Id]) ON DELETE SET NULL
GO

ALTER TABLE [Pack] ADD FOREIGN KEY ([PackLeaderId]) REFERENCES [User] ([Id])
GO

ALTER TABLE [Board] ADD FOREIGN KEY ([BoardOwnerId]) REFERENCES [User] ([Id])
GO

ALTER TABLE [Comment] ADD FOREIGN KEY ([UserId]) REFERENCES [User] ([Id])
GO

ALTER TABLE [Comment] ADD FOREIGN KEY ([PostId]) REFERENCES [Post] ([Id]) ON DELETE CASCADE
GO

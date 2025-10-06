IF SCHEMA_ID(N'supusr') IS NULL EXEC(N'CREATE SCHEMA [supusr];');
GO


CREATE TABLE [supusr].[AttractionAddressesDb] (
    [AddressId] uniqueidentifier NOT NULL,
    [StreetAddress] nvarchar(450) NOT NULL,
    [ZipCode] nvarchar(450) NOT NULL,
    [CityPlace] nvarchar(450) NOT NULL,
    [Country] nvarchar(450) NOT NULL,
    [Seeded] bit NOT NULL,
    CONSTRAINT [PK_AttractionAddressesDb] PRIMARY KEY ([AddressId])
);
GO


CREATE TABLE [supusr].[CategoriesDb] (
    [CategoryId] uniqueidentifier NOT NULL,
    [CategoryName] nvarchar(450) NOT NULL,
    [Seeded] bit NOT NULL,
    CONSTRAINT [PK_CategoriesDb] PRIMARY KEY ([CategoryId])
);
GO


CREATE TABLE [supusr].[UsersDb] (
    [UserId] uniqueidentifier NOT NULL,
    [FirstName] nvarchar(450) NOT NULL,
    [LastName] nvarchar(450) NOT NULL,
    [Email] nvarchar(450) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [Seeded] bit NOT NULL,
    CONSTRAINT [PK_UsersDb] PRIMARY KEY ([UserId])
);
GO


CREATE TABLE [supusr].[AttractionsDb] (
    [AttractionId] uniqueidentifier NOT NULL,
    [AttractionName] nvarchar(450) NOT NULL,
    [AttractionDescription] nvarchar(450) NULL,
    [AddressId] uniqueidentifier NULL,
    [Seeded] bit NOT NULL,
    CONSTRAINT [PK_AttractionsDb] PRIMARY KEY ([AttractionId]),
    CONSTRAINT [FK_AttractionsDb_AttractionAddressesDb_AddressId] FOREIGN KEY ([AddressId]) REFERENCES [supusr].[AttractionAddressesDb] ([AddressId]) ON DELETE SET NULL
);
GO


CREATE TABLE [supusr].[AttractionCategories] (
    [AttractionId] uniqueidentifier NOT NULL,
    [CategoryId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_AttractionCategories] PRIMARY KEY ([AttractionId], [CategoryId]),
    CONSTRAINT [FK_AttractionCategories_AttractionsDb_AttractionId] FOREIGN KEY ([AttractionId]) REFERENCES [supusr].[AttractionsDb] ([AttractionId]) ON DELETE CASCADE,
    CONSTRAINT [FK_AttractionCategories_CategoriesDb_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [supusr].[CategoriesDb] ([CategoryId]) ON DELETE CASCADE
);
GO


CREATE TABLE [supusr].[ReviewsDb] (
    [ReviewId] uniqueidentifier NOT NULL,
    [UserId] uniqueidentifier NOT NULL,
    [AttractionId] uniqueidentifier NOT NULL,
    [ReviewScore] int NOT NULL,
    [ReviewText] VARCHAR(250) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [Seeded] bit NOT NULL,
    CONSTRAINT [PK_ReviewsDb] PRIMARY KEY ([ReviewId]),
    CONSTRAINT [FK_ReviewsDb_AttractionsDb_AttractionId] FOREIGN KEY ([AttractionId]) REFERENCES [supusr].[AttractionsDb] ([AttractionId]) ON DELETE CASCADE,
    CONSTRAINT [FK_ReviewsDb_UsersDb_UserId] FOREIGN KEY ([UserId]) REFERENCES [supusr].[UsersDb] ([UserId]) ON DELETE CASCADE
);
GO


CREATE UNIQUE INDEX [IX_AttractionAddressesDb_StreetAddress_ZipCode_CityPlace_Country] ON [supusr].[AttractionAddressesDb] ([StreetAddress], [ZipCode], [CityPlace], [Country]);
GO


CREATE INDEX [IX_AttractionCategories_CategoryId] ON [supusr].[AttractionCategories] ([CategoryId]);
GO


CREATE INDEX [IX_AttractionsDb_AddressId] ON [supusr].[AttractionsDb] ([AddressId]);
GO


CREATE UNIQUE INDEX [IX_AttractionsDb_AttractionName_AttractionDescription_AddressId] ON [supusr].[AttractionsDb] ([AttractionName], [AttractionDescription], [AddressId]) WHERE [AttractionDescription] IS NOT NULL AND [AddressId] IS NOT NULL;
GO


CREATE UNIQUE INDEX [IX_CategoriesDb_CategoryName] ON [supusr].[CategoriesDb] ([CategoryName]);
GO


CREATE UNIQUE INDEX [IX_ReviewsDb_AttractionId_UserId] ON [supusr].[ReviewsDb] ([AttractionId], [UserId]);
GO


CREATE INDEX [IX_ReviewsDb_ReviewScore_ReviewText] ON [supusr].[ReviewsDb] ([ReviewScore], [ReviewText]);
GO


CREATE INDEX [IX_ReviewsDb_UserId] ON [supusr].[ReviewsDb] ([UserId]);
GO


CREATE UNIQUE INDEX [IX_UsersDb_Email] ON [supusr].[UsersDb] ([Email]);
GO


CREATE INDEX [IX_UsersDb_FirstName_LastName] ON [supusr].[UsersDb] ([FirstName], [LastName]);
GO


CREATE INDEX [IX_UsersDb_LastName_FirstName] ON [supusr].[UsersDb] ([LastName], [FirstName]);
GO



CREATE TABLE [Code_Category] (
[Code] varchar(3) NOT NULL,
[Desc] nvarchar(255) NOT NULL,
[Pooq_Code] nvarchar(255) NOT NULL,
PRIMARY KEY ([Code]) 
)
GO
CREATE TABLE [Code_Status] (
[Code] varchar(3) NOT NULL,
[Desc] nvarchar(255) NULL,
PRIMARY KEY ([Code]) 
)
GO
CREATE TABLE [Code_Country] (
[Code] varchar(3) NOT NULL,
[Desc] nvarchar(255) NULL,
PRIMARY KEY ([Code]) 
)
GO

CREATE TABLE [List_Content] (
[ID] varchar(30) NOT NULL,
[Name] nvarchar(255) NULL,
[InitialDate] datetime NULL,
[UpdateDate] datetime NULL,
[StatusCode] varchar(3) NULL,
[Ranking] int NULL,
[CategoryCode] varchar(50) NULL,
PRIMARY KEY ([ID]) 
)
GO

CREATE TABLE [Users] (
[ID] varchar(200) NOT NULL,
[Password] varchar(255) NOT NULL,
[Name] nvarchar(255) NULL,
[Contact] nvarchar(500) NULL,
[StatusCode] varchar(3) NULL,
PRIMARY KEY ([ID]) 
)
GO

CREATE TABLE [Subtitle] (
[ID] int NOT NULL IDENTITY(1,1),
[ContentID] varchar(30) NULL,
[Name] nvarchar(255) NULL,
[EpisodeNumber] varchar(4) NULL,
[CategoryCode] varchar(3) NULL,
[CountryCode] varchar(3) NULL,
[StatusCode] varchar(3) NULL,
[UpdateDate] datetime2 NULL,
[URL] varchar(255) NULL,
[subtitle] nvarchar(MAX) NULL,
PRIMARY KEY ([ID]) ,
CONSTRAINT [Constrain1] UNIQUE ([ContentID], [EpisodeNumber], [CategoryCode], [CountryCode])
)
GO

CREATE INDEX [search_idx] ON [Subtitle] ([ContentID] , [EpisodeNumber] , [CountryCode] )
GO

CREATE TABLE [Access] (
[ID] int NOT NULL IDENTITY(1,1),
[Type] varchar(2) NOT NULL,
[Param1] varchar(100) NULL,
[Param2] varchar(100) NULL,
PRIMARY KEY ([ID]) 
)
GO


CREATE TABLE [dbo].[ProgramTable](
	[Key] [int] IDENTITY(1,1) NOT NULL,
	[Id] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](200) NULL,
	[Image] [nvarchar](500) NULL,
	[Tag] [nvarchar](50) NULL,
	[PriceType] [nvarchar](50) NULL,
	[PriceInWon] [int] NULL,
	[ProgramGroupId] [nvarchar](50) NULL,
	[ProgramGroupName] [nvarchar](100) NULL,
	[ChannelId] [nvarchar](50) NULL,
	[ChannelName] [nvarchar](50) NULL,
	[LastEpisodeDate] [datetime] NULL,
	[ViewCountForLastHour] [int] NULL,
	[Ranking] [int] NULL,
	[AllEpisodes] [int] NULL,
	[FilteredEpisodes] [int] NULL,
	[Finished] [int] NOT NULL,
	[DayOfTheWeek] [nvarchar](50) NULL,
	[RatingAge] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Key] ASC,
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


CREATE TABLE [dbo].[EpisodeTable](
	[key] [int] IDENTITY(1,1) NOT NULL,
	[Id] [nvarchar](50) NOT NULL,
	[Name] [nvarchar](200) NULL,
	[Image] [nvarchar](500) NULL,
	[Tag] [nvarchar](50) NULL,
	[PriceType] [nvarchar](50) NULL,
	[PriceInWon] [int] NULL,
	[ProgramId] [nvarchar](50) NOT NULL,
	[ProgramName] [nvarchar](500) NULL,
	[MajorEpisodeNo] [int] NOT NULL,
	[MinorEpisodeNo] [int] NULL,
	[ProgramGroupId] [nvarchar](50) NULL,
	[ProgramGroupName] [nvarchar](100) NULL,
	[ChannelId] [nvarchar](50) NULL,
	[ChannelName] [nvarchar](50) NULL,
	[Date] [datetime] NULL,
	[ViewCountForLastHour] [int] NULL,
	[Ranking] [int] NULL,
	[Streammable] [int] NULL,
	[MessageForNotStreammable] [nvarchar](200) NULL,
	[Downloadable] [int] NULL,
	[MessageForNotDownloadable] [nvarchar](200) NULL,
	[RatingAge] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[key] ASC,
	[Id] ASC,
	[ProgramId] ASC,
	[MajorEpisodeNo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO





ALTER TABLE [Subtitle] ADD CONSTRAINT [fk_Subtitle_List_Content_1] FOREIGN KEY ([ContentID]) REFERENCES [List_Content] ([ID])
GO

ALTER TABLE [List_Content] ADD CONSTRAINT [fk_List_Content_Code_Status_1] FOREIGN KEY ([StatusCode]) REFERENCES [Code_Status] ([Code])
GO

ALTER TABLE [Subtitle] ADD CONSTRAINT [fk_Subtitle_Code_Status_1] FOREIGN KEY ([StatusCode]) REFERENCES [Code_Status] ([Code])
GO

ALTER TABLE [Subtitle] ADD CONSTRAINT [fk_Subtitle_Code_Category_1] FOREIGN KEY ([CategoryCode]) REFERENCES [Code_Category] ([Code])
GO

ALTER TABLE [Subtitle] ADD CONSTRAINT [fk_Subtitle_Code_Country_1] FOREIGN KEY ([CountryCode]) REFERENCES [Code_Country] ([Code])
GO

ALTER TABLE [Users] ADD CONSTRAINT [fk_Users_Code_Status_1] FOREIGN KEY ([StatusCode]) REFERENCES [Code_Status] ([Code])

GO

CREATE VIEW [dbo].[V2GetProgramList]
AS
SELECT DISTINCT 
               dbo.List_Content.ID, dbo.List_Content.Name, dbo.Code_Category.Pooq_Code, dbo.Code_Category.[Desc], 
               dbo.List_Content.Ranking, dbo.ProgramTable.Tag, dbo.ProgramTable.ChannelName, 
               dbo.ProgramTable.LastEpisodeDate, dbo.ProgramTable.Image, dbo.ProgramTable.PriceType
FROM     dbo.Code_Category INNER JOIN
               dbo.Subtitle ON dbo.Code_Category.Code = dbo.Subtitle.CategoryCode INNER JOIN
               dbo.List_Content ON dbo.Subtitle.ContentID = dbo.List_Content.ID INNER JOIN
               dbo.ProgramTable ON dbo.List_Content.ID = dbo.ProgramTable.Id

GO
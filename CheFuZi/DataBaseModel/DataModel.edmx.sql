
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 06/07/2017 00:46:55
-- Generated from EDMX file: E:\车夫子网站\chefuzi\CheFuZi\DataBaseModel\DataModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [chefuzi_data];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Article]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Article];
GO
IF OBJECT_ID(N'[dbo].[Article_Category]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Article_Category];
GO
IF OBJECT_ID(N'[dbo].[Article_Comment]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Article_Comment];
GO
IF OBJECT_ID(N'[dbo].[Sys_Area_City]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Sys_Area_City];
GO
IF OBJECT_ID(N'[dbo].[Sys_Area_District]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Sys_Area_District];
GO
IF OBJECT_ID(N'[dbo].[Sys_Area_Province]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Sys_Area_Province];
GO
IF OBJECT_ID(N'[dbo].[Sys_IdKey]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Sys_IdKey];
GO
IF OBJECT_ID(N'[dbo].[Sys_IdTable]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Sys_IdTable];
GO
IF OBJECT_ID(N'[dbo].[Sys_Status]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Sys_Status];
GO
IF OBJECT_ID(N'[dbo].[Sys_Suggestion]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Sys_Suggestion];
GO
IF OBJECT_ID(N'[dbo].[User_Collect]', 'U') IS NOT NULL
    DROP TABLE [dbo].[User_Collect];
GO
IF OBJECT_ID(N'[dbo].[User_LoginCategory]', 'U') IS NOT NULL
    DROP TABLE [dbo].[User_LoginCategory];
GO
IF OBJECT_ID(N'[dbo].[User_PhoneCheckCode]', 'U') IS NOT NULL
    DROP TABLE [dbo].[User_PhoneCheckCode];
GO
IF OBJECT_ID(N'[dbo].[User_Roles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[User_Roles];
GO
IF OBJECT_ID(N'[dbo].[User_UserInfo]', 'U') IS NOT NULL
    DROP TABLE [dbo].[User_UserInfo];
GO
IF OBJECT_ID(N'[dbo].[User_UserLogin]', 'U') IS NOT NULL
    DROP TABLE [dbo].[User_UserLogin];
GO
IF OBJECT_ID(N'[dbo].[User_UserName]', 'U') IS NOT NULL
    DROP TABLE [dbo].[User_UserName];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Article'
CREATE TABLE [dbo].[Article] (
    [ArticlId] bigint IDENTITY(1,1) NOT NULL,
    [CateId] int  NOT NULL,
    [UserId] bigint  NULL,
    [ArticleTitle] nvarchar(500)  NULL,
    [ArticleAuthor] nvarchar(500)  NULL,
    [ArticleDate] datetime  NULL,
    [Reference] varchar(150)  NULL,
    [ArticleSummary] nvarchar(1000)  NULL,
    [ArticleContent] nvarchar(max)  NULL,
    [ArticleImages] varchar(1000)  NULL,
    [ArticleVideo] varchar(500)  NULL,
    [AddDate] datetime  NOT NULL,
    [ReadTimes] int  NOT NULL,
    [CommentCount] int  NULL,
    [PraiseCount] int  NOT NULL,
    [OrderBy] int  NOT NULL,
    [Status] int  NOT NULL
);
GO

-- Creating table 'Article_Category'
CREATE TABLE [dbo].[Article_Category] (
    [CateId] int  NOT NULL,
    [ParentId] int  NOT NULL,
    [CateTitle] nvarchar(50)  NOT NULL,
    [CateImage] varchar(500)  NULL,
    [Description] nvarchar(100)  NULL,
    [AddDate] datetime  NOT NULL,
    [OrderBy] int  NOT NULL,
    [Status] int  NOT NULL
);
GO

-- Creating table 'Article_Comment'
CREATE TABLE [dbo].[Article_Comment] (
    [CommentId] bigint IDENTITY(1,1) NOT NULL,
    [AboutId] bigint  NULL,
    [MobilePhone] char(11)  NULL,
    [NickName] varchar(100)  NULL,
    [Detail] nvarchar(max)  NULL,
    [AddDate] datetime  NULL,
    [Status] int  NOT NULL
);
GO

-- Creating table 'Sys_Area_City'
CREATE TABLE [dbo].[Sys_Area_City] (
    [CityId] int  NOT NULL,
    [CityName] varchar(50)  NULL,
    [ZipCode] varchar(50)  NULL,
    [Phone] varchar(4)  NULL,
    [ProvinceID] int  NOT NULL,
    [OrderBy] bigint  NULL
);
GO

-- Creating table 'Sys_Area_District'
CREATE TABLE [dbo].[Sys_Area_District] (
    [DistrictId] int  NOT NULL,
    [DistrictName] varchar(50)  NULL,
    [CityID] int  NOT NULL,
    [OrderBy] int  NULL
);
GO

-- Creating table 'Sys_Area_Province'
CREATE TABLE [dbo].[Sys_Area_Province] (
    [ProvinceId] int  NOT NULL,
    [ProvinceName] varchar(50)  NULL,
    [CountryId] int  NOT NULL,
    [OrderBy] int  NULL
);
GO

-- Creating table 'Sys_IdKey'
CREATE TABLE [dbo].[Sys_IdKey] (
    [IdKey] bigint IDENTITY(1,1) NOT NULL,
    [TableId] int  NOT NULL
);
GO

-- Creating table 'Sys_IdTable'
CREATE TABLE [dbo].[Sys_IdTable] (
    [TableId] int  NOT NULL,
    [TableName] varchar(50)  NOT NULL
);
GO

-- Creating table 'Sys_Status'
CREATE TABLE [dbo].[Sys_Status] (
    [StatusId] int  NOT NULL,
    [StatusName] nvarchar(50)  NULL,
    [Description] nvarchar(100)  NULL
);
GO

-- Creating table 'Sys_Suggestion'
CREATE TABLE [dbo].[Sys_Suggestion] (
    [SuggestionId] bigint IDENTITY(1,1) NOT NULL,
    [SuggestionTitle] nvarchar(50)  NOT NULL,
    [Description] nvarchar(max)  NULL,
    [UserId] bigint  NOT NULL,
    [AddDate] datetime  NULL
);
GO

-- Creating table 'User_Collect'
CREATE TABLE [dbo].[User_Collect] (
    [CollectId] bigint IDENTITY(1,1) NOT NULL,
    [UserId] bigint  NOT NULL,
    [AboutId] bigint  NOT NULL,
    [AboutTitle] nvarchar(100)  NULL,
    [CollectDate] datetime  NOT NULL,
    [OrderBy] bigint  NULL
);
GO

-- Creating table 'User_LoginCategory'
CREATE TABLE [dbo].[User_LoginCategory] (
    [LoginCategoryId] int  NOT NULL,
    [Description] varchar(50)  NOT NULL
);
GO

-- Creating table 'User_PhoneCheckCode'
CREATE TABLE [dbo].[User_PhoneCheckCode] (
    [MobilePhone] char(11)  NOT NULL,
    [CheckCode] char(5)  NULL,
    [AddDate] datetime  NULL,
    [ExpiredDate] datetime  NULL,
    [AlreadCheck] bit  NULL
);
GO

-- Creating table 'User_Roles'
CREATE TABLE [dbo].[User_Roles] (
    [RoleId] int  NOT NULL,
    [RoleName] varchar(10)  NULL
);
GO

-- Creating table 'User_UserInfo'
CREATE TABLE [dbo].[User_UserInfo] (
    [UserId] bigint  NOT NULL,
    [Email] varchar(50)  NULL,
    [PassWord] varchar(50)  NOT NULL,
    [RealName] varchar(50)  NULL,
    [Sex] bit  NOT NULL,
    [BirthDate] datetime  NULL,
    [HeadImage] varchar(200)  NULL,
    [ProvinceId] int  NULL,
    [ProvinceName] varchar(50)  NULL,
    [CityId] int  NULL,
    [CityName] varchar(50)  NULL,
    [DistrictId] int  NULL,
    [DistrictName] varchar(50)  NULL,
    [Address] nvarchar(100)  NULL,
    [AddDate] datetime  NULL,
    [CheckDate] datetime  NULL,
    [Status] int  NOT NULL
);
GO

-- Creating table 'User_UserLogin'
CREATE TABLE [dbo].[User_UserLogin] (
    [UserId] bigint  NOT NULL,
    [PassWord] varchar(50)  NOT NULL,
    [NickName] varchar(50)  NULL,
    [RolesIdList] varchar(100)  NULL
);
GO

-- Creating table 'User_UserName'
CREATE TABLE [dbo].[User_UserName] (
    [UserName] varchar(50)  NOT NULL,
    [UserId] bigint  NOT NULL,
    [LoginCategoryId] int  NOT NULL,
    [AddDate] datetime  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [ArticlId] in table 'Article'
ALTER TABLE [dbo].[Article]
ADD CONSTRAINT [PK_Article]
    PRIMARY KEY CLUSTERED ([ArticlId] ASC);
GO

-- Creating primary key on [CateId] in table 'Article_Category'
ALTER TABLE [dbo].[Article_Category]
ADD CONSTRAINT [PK_Article_Category]
    PRIMARY KEY CLUSTERED ([CateId] ASC);
GO

-- Creating primary key on [CommentId] in table 'Article_Comment'
ALTER TABLE [dbo].[Article_Comment]
ADD CONSTRAINT [PK_Article_Comment]
    PRIMARY KEY CLUSTERED ([CommentId] ASC);
GO

-- Creating primary key on [CityId] in table 'Sys_Area_City'
ALTER TABLE [dbo].[Sys_Area_City]
ADD CONSTRAINT [PK_Sys_Area_City]
    PRIMARY KEY CLUSTERED ([CityId] ASC);
GO

-- Creating primary key on [DistrictId] in table 'Sys_Area_District'
ALTER TABLE [dbo].[Sys_Area_District]
ADD CONSTRAINT [PK_Sys_Area_District]
    PRIMARY KEY CLUSTERED ([DistrictId] ASC);
GO

-- Creating primary key on [ProvinceId] in table 'Sys_Area_Province'
ALTER TABLE [dbo].[Sys_Area_Province]
ADD CONSTRAINT [PK_Sys_Area_Province]
    PRIMARY KEY CLUSTERED ([ProvinceId] ASC);
GO

-- Creating primary key on [IdKey] in table 'Sys_IdKey'
ALTER TABLE [dbo].[Sys_IdKey]
ADD CONSTRAINT [PK_Sys_IdKey]
    PRIMARY KEY CLUSTERED ([IdKey] ASC);
GO

-- Creating primary key on [TableId] in table 'Sys_IdTable'
ALTER TABLE [dbo].[Sys_IdTable]
ADD CONSTRAINT [PK_Sys_IdTable]
    PRIMARY KEY CLUSTERED ([TableId] ASC);
GO

-- Creating primary key on [StatusId] in table 'Sys_Status'
ALTER TABLE [dbo].[Sys_Status]
ADD CONSTRAINT [PK_Sys_Status]
    PRIMARY KEY CLUSTERED ([StatusId] ASC);
GO

-- Creating primary key on [SuggestionId] in table 'Sys_Suggestion'
ALTER TABLE [dbo].[Sys_Suggestion]
ADD CONSTRAINT [PK_Sys_Suggestion]
    PRIMARY KEY CLUSTERED ([SuggestionId] ASC);
GO

-- Creating primary key on [CollectId] in table 'User_Collect'
ALTER TABLE [dbo].[User_Collect]
ADD CONSTRAINT [PK_User_Collect]
    PRIMARY KEY CLUSTERED ([CollectId] ASC);
GO

-- Creating primary key on [LoginCategoryId] in table 'User_LoginCategory'
ALTER TABLE [dbo].[User_LoginCategory]
ADD CONSTRAINT [PK_User_LoginCategory]
    PRIMARY KEY CLUSTERED ([LoginCategoryId] ASC);
GO

-- Creating primary key on [MobilePhone] in table 'User_PhoneCheckCode'
ALTER TABLE [dbo].[User_PhoneCheckCode]
ADD CONSTRAINT [PK_User_PhoneCheckCode]
    PRIMARY KEY CLUSTERED ([MobilePhone] ASC);
GO

-- Creating primary key on [RoleId] in table 'User_Roles'
ALTER TABLE [dbo].[User_Roles]
ADD CONSTRAINT [PK_User_Roles]
    PRIMARY KEY CLUSTERED ([RoleId] ASC);
GO

-- Creating primary key on [UserId] in table 'User_UserInfo'
ALTER TABLE [dbo].[User_UserInfo]
ADD CONSTRAINT [PK_User_UserInfo]
    PRIMARY KEY CLUSTERED ([UserId] ASC);
GO

-- Creating primary key on [UserId] in table 'User_UserLogin'
ALTER TABLE [dbo].[User_UserLogin]
ADD CONSTRAINT [PK_User_UserLogin]
    PRIMARY KEY CLUSTERED ([UserId] ASC);
GO

-- Creating primary key on [UserName] in table 'User_UserName'
ALTER TABLE [dbo].[User_UserName]
ADD CONSTRAINT [PK_User_UserName]
    PRIMARY KEY CLUSTERED ([UserName] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------
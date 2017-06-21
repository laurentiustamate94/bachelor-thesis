USE [bachelor-thesis]
GO

/****** Object: Table [dbo].[Logging] ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Logging] (
    [Id]            BIGINT        IDENTITY (1, 1) NOT NULL,
    [MessageId]     VARCHAR (64)  NOT NULL,
    [RawText]       VARCHAR (MAX) NOT NULL,
    [TranslateJson] VARCHAR (MAX) NULL,
    [QnAMakerJson]  VARCHAR (MAX) NULL,
    [LuisJson]      VARCHAR (MAX) NULL,
    [AnalysisJson]  VARCHAR (MAX) NULL,
    [CustomJson]    VARCHAR (MAX) NULL
);



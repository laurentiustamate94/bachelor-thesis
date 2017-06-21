USE [bachelor-thesis]
GO

/****** Object: Table [dbo].[Feedback] ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Feedback] (
    [Id]        BIGINT        IDENTITY (1, 1) NOT NULL,
    [LoggingId] BIGINT        NULL,
    [UsersId]   BIGINT        NULL,
    [RawText]   VARCHAR (MAX) NOT NULL
);



USE [bachelor-thesis]
GO

/****** Object: Table [dbo].[Users] ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Users] (
    [Id]             BIGINT        IDENTITY (1, 1) NOT NULL,
    [ConversationId] VARCHAR (64)  NOT NULL,
    [Email]          VARCHAR (MAX) NOT NULL
);



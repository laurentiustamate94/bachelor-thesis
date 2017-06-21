USE [bachelor-thesis]
GO

/****** Object: Table [dbo].[KnowledgeBase] ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[KnowledgeBase] (
    [Id]           BIGINT        IDENTITY (1, 1) NOT NULL,
    [Question]     VARCHAR (MAX) NOT NULL,
    [Answer]       VARCHAR (MAX) NOT NULL,
    [Analysis]     VARCHAR (MAX) NOT NULL,
    [PairChecksum] VARCHAR (64)  NOT NULL,
    [Intent]       VARCHAR (256) NULL,
    [Hits]         INT           NOT NULL
);



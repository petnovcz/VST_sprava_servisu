
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 11/09/2017 14:36:34
-- Generated from EDMX file: C:\Users\novakp\documents\visual studio 2017\Projects\VST_sprava_servisu\VST_sprava_servisu\Model1.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [Servis];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_RegionZakaznik]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ZakaznikSet] DROP CONSTRAINT [FK_RegionZakaznik];
GO
IF OBJECT_ID(N'[dbo].[FK_JazykZakaznik]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ZakaznikSet] DROP CONSTRAINT [FK_JazykZakaznik];
GO
IF OBJECT_ID(N'[dbo].[FK_ProvozZakaznik]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ProvozSet] DROP CONSTRAINT [FK_ProvozZakaznik];
GO
IF OBJECT_ID(N'[dbo].[FK_UmisteniProvoz]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UmisteniSet] DROP CONSTRAINT [FK_UmisteniProvoz];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[RegionSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RegionSet];
GO
IF OBJECT_ID(N'[dbo].[ZakaznikSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ZakaznikSet];
GO
IF OBJECT_ID(N'[dbo].[JazykSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[JazykSet];
GO
IF OBJECT_ID(N'[dbo].[ProvozSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ProvozSet];
GO
IF OBJECT_ID(N'[dbo].[UmisteniSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UmisteniSet];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Region'
CREATE TABLE [dbo].[Region] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [NazevRegionu] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'Zakaznik'
CREATE TABLE [dbo].[Zakaznik] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [NazevZakaznika] nvarchar(max)  NOT NULL,
    [KodSAP] nvarchar(255)  NOT NULL,
    [RegionId] int  NOT NULL,
    [IC] nvarchar(20)  NOT NULL,
    [DIC] nvarchar(max)  NOT NULL,
    [Telefon] nvarchar(25)  NOT NULL,
    [Kontakt] nvarchar(50)  NOT NULL,
    [Email] nvarchar(255)  NOT NULL,
    [JazykId] int  NOT NULL,
    [Adresa] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Jazyk'
CREATE TABLE [dbo].[Jazyk] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [NazevJazyku] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'Provoz'
CREATE TABLE [dbo].[Provoz] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ZakaznikId] int  NOT NULL,
    [NazevProvozu] nvarchar(max)  NOT NULL,
    [OddeleniVybuchu] bit  NOT NULL,
    [PotlaceniVybuchu] bit  NOT NULL
);
GO

-- Creating table 'Umisteni'
CREATE TABLE [dbo].[Umisteni] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ProvozId] int  NOT NULL,
    [NazevUmisteni] nvarchar(max)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Region'
ALTER TABLE [dbo].[Region]
ADD CONSTRAINT [PK_Region]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Zakaznik'
ALTER TABLE [dbo].[Zakaznik]
ADD CONSTRAINT [PK_Zakaznik]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Jazyk'
ALTER TABLE [dbo].[Jazyk]
ADD CONSTRAINT [PK_Jazyk]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Provoz'
ALTER TABLE [dbo].[Provoz]
ADD CONSTRAINT [PK_Provoz]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Umisteni'
ALTER TABLE [dbo].[Umisteni]
ADD CONSTRAINT [PK_Umisteni]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [RegionId] in table 'Zakaznik'
ALTER TABLE [dbo].[Zakaznik]
ADD CONSTRAINT [FK_RegionZakaznik]
    FOREIGN KEY ([RegionId])
    REFERENCES [dbo].[Region]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RegionZakaznik'
CREATE INDEX [IX_FK_RegionZakaznik]
ON [dbo].[Zakaznik]
    ([RegionId]);
GO

-- Creating foreign key on [JazykId] in table 'Zakaznik'
ALTER TABLE [dbo].[Zakaznik]
ADD CONSTRAINT [FK_JazykZakaznik]
    FOREIGN KEY ([JazykId])
    REFERENCES [dbo].[Jazyk]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_JazykZakaznik'
CREATE INDEX [IX_FK_JazykZakaznik]
ON [dbo].[Zakaznik]
    ([JazykId]);
GO

-- Creating foreign key on [ZakaznikId] in table 'Provoz'
ALTER TABLE [dbo].[Provoz]
ADD CONSTRAINT [FK_ProvozZakaznik]
    FOREIGN KEY ([ZakaznikId])
    REFERENCES [dbo].[Zakaznik]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ProvozZakaznik'
CREATE INDEX [IX_FK_ProvozZakaznik]
ON [dbo].[Provoz]
    ([ZakaznikId]);
GO

-- Creating foreign key on [ProvozId] in table 'Umisteni'
ALTER TABLE [dbo].[Umisteni]
ADD CONSTRAINT [FK_UmisteniProvoz]
    FOREIGN KEY ([ProvozId])
    REFERENCES [dbo].[Provoz]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UmisteniProvoz'
CREATE INDEX [IX_FK_UmisteniProvoz]
ON [dbo].[Umisteni]
    ([ProvozId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------
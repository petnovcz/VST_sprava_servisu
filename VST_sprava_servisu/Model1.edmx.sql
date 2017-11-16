
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 11/16/2017 09:36:09
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

IF OBJECT_ID(N'[dbo].[FK_JazykZakaznik]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Zakaznik] DROP CONSTRAINT [FK_JazykZakaznik];
GO
IF OBJECT_ID(N'[dbo].[FK_ProvozRevize]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Revize] DROP CONSTRAINT [FK_ProvozRevize];
GO
IF OBJECT_ID(N'[dbo].[FK_ProvozSCProvozu]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SCProvozu] DROP CONSTRAINT [FK_ProvozSCProvozu];
GO
IF OBJECT_ID(N'[dbo].[FK_ProvozZakaznik]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Provoz] DROP CONSTRAINT [FK_ProvozZakaznik];
GO
IF OBJECT_ID(N'[dbo].[FK_RegionZakaznik]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Zakaznik] DROP CONSTRAINT [FK_RegionZakaznik];
GO
IF OBJECT_ID(N'[dbo].[FK_RevizeSCRevize]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RevizeSC] DROP CONSTRAINT [FK_RevizeSCRevize];
GO
IF OBJECT_ID(N'[dbo].[FK_RevizeSCSCProvozu]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[RevizeSC] DROP CONSTRAINT [FK_RevizeSCSCProvozu];
GO
IF OBJECT_ID(N'[dbo].[FK_SerioveCisloArtikl]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SerioveCislo] DROP CONSTRAINT [FK_SerioveCisloArtikl];
GO
IF OBJECT_ID(N'[dbo].[FK_SerioveCisloSCProvozu]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SCProvozu] DROP CONSTRAINT [FK_SerioveCisloSCProvozu];
GO
IF OBJECT_ID(N'[dbo].[FK_StatusRevizeRevize]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Revize] DROP CONSTRAINT [FK_StatusRevizeRevize];
GO
IF OBJECT_ID(N'[dbo].[FK_StatusSCProvozu]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SCProvozu] DROP CONSTRAINT [FK_StatusSCProvozu];
GO
IF OBJECT_ID(N'[dbo].[FK_UmisteniProvoz]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Umisteni] DROP CONSTRAINT [FK_UmisteniProvoz];
GO
IF OBJECT_ID(N'[dbo].[FK_ZakaznikKontakniOsoba]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[KontakniOsoba] DROP CONSTRAINT [FK_ZakaznikKontakniOsoba];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Artikl]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Artikl];
GO
IF OBJECT_ID(N'[dbo].[Jazyk]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Jazyk];
GO
IF OBJECT_ID(N'[dbo].[KontakniOsoba]', 'U') IS NOT NULL
    DROP TABLE [dbo].[KontakniOsoba];
GO
IF OBJECT_ID(N'[dbo].[Provoz]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Provoz];
GO
IF OBJECT_ID(N'[dbo].[Region]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Region];
GO
IF OBJECT_ID(N'[dbo].[Revize]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Revize];
GO
IF OBJECT_ID(N'[dbo].[RevizeSC]', 'U') IS NOT NULL
    DROP TABLE [dbo].[RevizeSC];
GO
IF OBJECT_ID(N'[dbo].[SCProvozu]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SCProvozu];
GO
IF OBJECT_ID(N'[dbo].[SerioveCislo]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SerioveCislo];
GO
IF OBJECT_ID(N'[dbo].[Status]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Status];
GO
IF OBJECT_ID(N'[dbo].[StatusRevize]', 'U') IS NOT NULL
    DROP TABLE [dbo].[StatusRevize];
GO
IF OBJECT_ID(N'[dbo].[Umisteni]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Umisteni];
GO
IF OBJECT_ID(N'[dbo].[Zakaznik]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Zakaznik];
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

-- Creating table 'Artikl'
CREATE TABLE [dbo].[Artikl] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Nazev] nvarchar(max)  NOT NULL,
    [Oznaceni] nvarchar(max)  NOT NULL,
    [Typ] nvarchar(max)  NOT NULL,
    [RozsahProvoznichTeplot] nvarchar(max)  NOT NULL,
    [KodSAP] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Revize'
CREATE TABLE [dbo].[Revize] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ProvozId] int  NOT NULL,
    [DatumRevize] datetime  NOT NULL,
    [StatusRevizeId] int  NOT NULL,
    [DatumVystaveni] datetime  NOT NULL,
    [ZjistenyStav] nvarchar(max)  NOT NULL,
    [ProvedeneZasahy] nvarchar(max)  NOT NULL,
    [OpatreniKOdstraneni] nvarchar(max)  NOT NULL,
    [KontrolaProvedenaDne] datetime  NOT NULL,
    [PristiKontrola] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'RevizeSC'
CREATE TABLE [dbo].[RevizeSC] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [RevizeId] int  NOT NULL,
    [SCProvozuId] int  NOT NULL,
    [StavKoroze] nvarchar(max)  NOT NULL,
    [StavZnecisteni] nvarchar(max)  NOT NULL,
    [JineZavady] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'SCProvozu'
CREATE TABLE [dbo].[SCProvozu] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ProvozId] int  NOT NULL,
    [SerioveCisloId] int  NOT NULL,
    [StatusId] int  NOT NULL,
    [DatumPrirazeni] datetime  NOT NULL,
    [DatumPosledniZmeny] datetime  NOT NULL,
    [DatumVymeny] datetime  NOT NULL
);
GO

-- Creating table 'SerioveCislo'
CREATE TABLE [dbo].[SerioveCislo] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ArtiklId] int  NOT NULL,
    [DatumVyroby] datetime  NOT NULL,
    [DatumPosledniTlakoveZkousky] datetime  NOT NULL
);
GO

-- Creating table 'Status'
CREATE TABLE [dbo].[Status] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [NazevStatusu] nvarchar(max)  NOT NULL,
    [Aktivni] bit  NOT NULL,
    [Neaktivni] bit  NOT NULL
);
GO

-- Creating table 'StatusRevize'
CREATE TABLE [dbo].[StatusRevize] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [NazevStatusuRevize] nvarchar(max)  NOT NULL,
    [Planovana] bit  NOT NULL,
    [Potvrzena] bit  NOT NULL,
    [Realizovana] bit  NOT NULL
);
GO

-- Creating table 'KontakniOsoba'
CREATE TABLE [dbo].[KontakniOsoba] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ZakaznikId] int  NOT NULL,
    [JmenoPrijmeni] nvarchar(max)  NOT NULL,
    [Pozice] nvarchar(max)  NOT NULL,
    [Telefon] nvarchar(max)  NOT NULL,
    [Email] nvarchar(max)  NOT NULL
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

-- Creating primary key on [Id] in table 'Artikl'
ALTER TABLE [dbo].[Artikl]
ADD CONSTRAINT [PK_Artikl]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Revize'
ALTER TABLE [dbo].[Revize]
ADD CONSTRAINT [PK_Revize]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'RevizeSC'
ALTER TABLE [dbo].[RevizeSC]
ADD CONSTRAINT [PK_RevizeSC]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SCProvozu'
ALTER TABLE [dbo].[SCProvozu]
ADD CONSTRAINT [PK_SCProvozu]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SerioveCislo'
ALTER TABLE [dbo].[SerioveCislo]
ADD CONSTRAINT [PK_SerioveCislo]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Status'
ALTER TABLE [dbo].[Status]
ADD CONSTRAINT [PK_Status]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'StatusRevize'
ALTER TABLE [dbo].[StatusRevize]
ADD CONSTRAINT [PK_StatusRevize]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'KontakniOsoba'
ALTER TABLE [dbo].[KontakniOsoba]
ADD CONSTRAINT [PK_KontakniOsoba]
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

-- Creating foreign key on [ArtiklId] in table 'SerioveCislo'
ALTER TABLE [dbo].[SerioveCislo]
ADD CONSTRAINT [FK_SerioveCisloArtikl]
    FOREIGN KEY ([ArtiklId])
    REFERENCES [dbo].[Artikl]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SerioveCisloArtikl'
CREATE INDEX [IX_FK_SerioveCisloArtikl]
ON [dbo].[SerioveCislo]
    ([ArtiklId]);
GO

-- Creating foreign key on [ProvozId] in table 'Revize'
ALTER TABLE [dbo].[Revize]
ADD CONSTRAINT [FK_ProvozRevize]
    FOREIGN KEY ([ProvozId])
    REFERENCES [dbo].[Provoz]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ProvozRevize'
CREATE INDEX [IX_FK_ProvozRevize]
ON [dbo].[Revize]
    ([ProvozId]);
GO

-- Creating foreign key on [ProvozId] in table 'SCProvozu'
ALTER TABLE [dbo].[SCProvozu]
ADD CONSTRAINT [FK_ProvozSCProvozu]
    FOREIGN KEY ([ProvozId])
    REFERENCES [dbo].[Provoz]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ProvozSCProvozu'
CREATE INDEX [IX_FK_ProvozSCProvozu]
ON [dbo].[SCProvozu]
    ([ProvozId]);
GO

-- Creating foreign key on [RevizeId] in table 'RevizeSC'
ALTER TABLE [dbo].[RevizeSC]
ADD CONSTRAINT [FK_RevizeSCRevize]
    FOREIGN KEY ([RevizeId])
    REFERENCES [dbo].[Revize]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RevizeSCRevize'
CREATE INDEX [IX_FK_RevizeSCRevize]
ON [dbo].[RevizeSC]
    ([RevizeId]);
GO

-- Creating foreign key on [StatusRevizeId] in table 'Revize'
ALTER TABLE [dbo].[Revize]
ADD CONSTRAINT [FK_StatusRevizeRevize]
    FOREIGN KEY ([StatusRevizeId])
    REFERENCES [dbo].[StatusRevize]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_StatusRevizeRevize'
CREATE INDEX [IX_FK_StatusRevizeRevize]
ON [dbo].[Revize]
    ([StatusRevizeId]);
GO

-- Creating foreign key on [SCProvozuId] in table 'RevizeSC'
ALTER TABLE [dbo].[RevizeSC]
ADD CONSTRAINT [FK_RevizeSCSCProvozu]
    FOREIGN KEY ([SCProvozuId])
    REFERENCES [dbo].[SCProvozu]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_RevizeSCSCProvozu'
CREATE INDEX [IX_FK_RevizeSCSCProvozu]
ON [dbo].[RevizeSC]
    ([SCProvozuId]);
GO

-- Creating foreign key on [SerioveCisloId] in table 'SCProvozu'
ALTER TABLE [dbo].[SCProvozu]
ADD CONSTRAINT [FK_SerioveCisloSCProvozu]
    FOREIGN KEY ([SerioveCisloId])
    REFERENCES [dbo].[SerioveCislo]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SerioveCisloSCProvozu'
CREATE INDEX [IX_FK_SerioveCisloSCProvozu]
ON [dbo].[SCProvozu]
    ([SerioveCisloId]);
GO

-- Creating foreign key on [StatusId] in table 'SCProvozu'
ALTER TABLE [dbo].[SCProvozu]
ADD CONSTRAINT [FK_StatusSCProvozu]
    FOREIGN KEY ([StatusId])
    REFERENCES [dbo].[Status]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_StatusSCProvozu'
CREATE INDEX [IX_FK_StatusSCProvozu]
ON [dbo].[SCProvozu]
    ([StatusId]);
GO

-- Creating foreign key on [ZakaznikId] in table 'KontakniOsoba'
ALTER TABLE [dbo].[KontakniOsoba]
ADD CONSTRAINT [FK_ZakaznikKontakniOsoba]
    FOREIGN KEY ([ZakaznikId])
    REFERENCES [dbo].[Zakaznik]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ZakaznikKontakniOsoba'
CREATE INDEX [IX_FK_ZakaznikKontakniOsoba]
ON [dbo].[KontakniOsoba]
    ([ZakaznikId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------
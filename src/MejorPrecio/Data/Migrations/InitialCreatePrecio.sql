IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260715180855_InitialCreatePrecio'
)
BEGIN
    CREATE TABLE [Precio] (
        [Id] int NOT NULL IDENTITY,
        [Proveedor] varchar(10) NOT NULL,
        [ProductoProveed] varchar(20) NOT NULL,
        [Categoria] varchar(3) NOT NULL,
        [PrecioUnitario] decimal(18,2) NOT NULL,
        [VigenciaDesde] date NOT NULL,
        [VigenciaHasta] date NOT NULL,
        CONSTRAINT [PK_Precio] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Precio_Categoria_Categoria] FOREIGN KEY ([Categoria]) REFERENCES [Categoria] ([Categoria]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Precio_ProductoProveedor_Proveedor_ProductoProveed] FOREIGN KEY ([Proveedor], [ProductoProveed]) REFERENCES [ProductoProveedor] ([Proveedor], [ProductoProveed]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Precio_Proveedor_Proveedor] FOREIGN KEY ([Proveedor]) REFERENCES [Proveedor] ([Proveedor]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260715180855_InitialCreatePrecio'
)
BEGIN
    CREATE INDEX [IX_Precio_Categoria] ON [Precio] ([Categoria]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260715180855_InitialCreatePrecio'
)
BEGIN
    CREATE INDEX [IX_Precio_ProductoProveed] ON [Precio] ([ProductoProveed]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260715180855_InitialCreatePrecio'
)
BEGIN
    CREATE INDEX [IX_Precio_Proveedor_ProductoProveed] ON [Precio] ([Proveedor], [ProductoProveed]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260715180855_InitialCreatePrecio'
)
BEGIN
    CREATE INDEX [IX_Precio_Vigencia_Categoria] ON [Precio] ([VigenciaDesde], [VigenciaHasta], [Categoria]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260715180855_InitialCreatePrecio'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260715180855_InitialCreatePrecio', N'10.0.10');
END;

COMMIT;
GO


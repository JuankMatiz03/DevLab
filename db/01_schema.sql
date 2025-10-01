-- mapeo de base de datos y login, crea LabDev y el usuario developer con permisos
IF DB_ID('LabDev') IS NULL
  CREATE DATABASE LabDev;
GO

USE master;
IF NOT EXISTS (SELECT 1 FROM sys.sql_logins WHERE name = 'developer')
  CREATE LOGIN developer WITH PASSWORD='abc123ABC', CHECK_POLICY=OFF, CHECK_EXPIRATION=OFF;
GO

USE LabDev;
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = 'developer')
  CREATE USER developer FOR LOGIN developer;
EXEC sp_addrolemember 'db_owner', 'developer';
GO

-- esquema de datos
IF OBJECT_ID('dbo.Customers') IS NULL
CREATE TABLE dbo.Customers(
  Id  INT IDENTITY(1,1) PRIMARY KEY,
  Name  NVARCHAR(120) NOT NULL,
  Document NVARCHAR(30)  NOT NULL UNIQUE,  -- documento/id del cliente
  Email NVARCHAR(120) NULL,
  IsActive  BIT NOT NULL DEFAULT (1)
);

IF OBJECT_ID('dbo.Products') IS NULL
CREATE TABLE dbo.Products(
  Id INT IDENTITY(1,1) PRIMARY KEY,
  Name NVARCHAR(120) NOT NULL,
  Price DECIMAL(18,2) NOT NULL,
  ImageUrl NVARCHAR(300) NULL,
  IsActive BIT NOT NULL DEFAULT (1)
);

IF OBJECT_ID('dbo.Invoices') IS NULL
CREATE TABLE dbo.Invoices(
  Id INT IDENTITY(1,1) PRIMARY KEY,
  Number INT NOT NULL UNIQUE,                 
  CustomerId INT NOT NULL REFERENCES dbo.Customers(Id),
  Subtotal DECIMAL(18,2) NOT NULL,
  Tax DECIMAL(18,2) NOT NULL, -- 19% del subtotal
  Total DECIMAL(18,2) NOT NULL, -- Subtotal + Tax
  CreatedAt DATETIME2 NOT NULL DEFAULT (SYSDATETIME())
);

IF OBJECT_ID('dbo.InvoiceDetails') IS NULL
CREATE TABLE dbo.InvoiceDetails(
  Id INT IDENTITY(1,1) PRIMARY KEY,
  InvoiceId INT NOT NULL REFERENCES dbo.Invoices(Id),
  ProductId INT NOT NULL REFERENCES dbo.Products(Id),
  Quantity  INT NOT NULL CHECK (Quantity > 0),
  UnitPrice DECIMAL(18,2) NOT NULL,
  LineTotal AS (Quantity * UnitPrice) PERSISTED
);

-- índices de apoyo, mejoran lasbusquedas por cliente y por numero)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_Invoices_CustomerId')
  CREATE INDEX IX_Invoices_CustomerId ON dbo.Invoices(CustomerId);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_Invoices_Number')
  CREATE INDEX IX_Invoices_Number ON dbo.Invoices(Number);

-- tipo de tabla TVP para insertar detalles en lotes, recrea si existia
IF TYPE_ID('dbo.InvoiceItemType') IS NOT NULL DROP TYPE dbo.InvoiceItemType;
GO
CREATE TYPE dbo.InvoiceItemType AS TABLE(
  ProductId  INT NOT NULL,
  Quantity   INT NOT NULL,
  UnitPrice  DECIMAL(18,2) NOT NULL
);
GO

-- datos semilla para pruebas
IF NOT EXISTS (SELECT 1 FROM dbo.Customers)
INSERT dbo.Customers (Name, Document, Email) VALUES
('Acme S.A.S', '900111222', 'purchasing@acme.com'),
('Beta Ltda',  '800333444', 'info@beta.com');

IF NOT EXISTS (SELECT 1 FROM dbo.Products)
INSERT dbo.Products (Name, Price, ImageUrl) VALUES
('Keyboard', 85000,  'https://picsum.photos/seed/keyboard/200'),
('Mouse',    45000,  'https://picsum.photos/seed/mouse/200'),
('Monitor',  850000, 'https://picsum.photos/seed/monitor/200');
GO

-- lookups, clientes y productos activos
IF OBJECT_ID('dbo.usp_Customers_List') IS NOT NULL DROP PROC dbo.usp_Customers_List;
GO
CREATE PROC dbo.usp_Customers_List
AS
BEGIN
  SET NOCOUNT ON;
  SELECT Id, Name FROM dbo.Customers WHERE IsActive = 1 ORDER BY Name;
END
GO

IF OBJECT_ID('dbo.usp_Products_List') IS NOT NULL DROP PROC dbo.usp_Products_List;
GO
CREATE PROC dbo.usp_Products_List
AS
BEGIN
  SET NOCOUNT ON;
  SELECT Id, Name, Price, ImageUrl FROM dbo.Products WHERE IsActive = 1 ORDER BY Name;
END
GO

-- crear factura, transaccion, valida numero unico, calcula subtotal-IVA-total
IF OBJECT_ID('dbo.usp_Invoices_Create') IS NOT NULL DROP PROC dbo.usp_Invoices_Create;
GO
CREATE PROC dbo.usp_Invoices_Create
  @Number     INT,
  @CustomerId INT,
  @Items      dbo.InvoiceItemType READONLY
AS
BEGIN
  SET NOCOUNT ON;
  BEGIN TRY
    BEGIN TRAN;

    -- validacion para evitar facturas duplicads
    IF EXISTS (SELECT 1 FROM dbo.Invoices WHERE Number = @Number)
      THROW 51000, 'Invoice number already exists.', 1;

    -- calculo de totales
    DECLARE @Subtotal DECIMAL(18,2) = (
      SELECT SUM(Quantity * UnitPrice) FROM @Items
    );
    IF @Subtotal IS NULL SET @Subtotal = 0;

    DECLARE @Tax   DECIMAL(18,2) = ROUND(@Subtotal * 0.19, 2);
    DECLARE @Total DECIMAL(18,2) = @Subtotal + @Tax;

    -- inserta encabezado
    INSERT dbo.Invoices (Number, CustomerId, Subtotal, Tax, Total)
    VALUES (@Number, @CustomerId, @Subtotal, @Tax, @Total);

    DECLARE @InvoiceId INT = SCOPE_IDENTITY();

    -- inserta detalle 
    INSERT dbo.InvoiceDetails (InvoiceId, ProductId, Quantity, UnitPrice)
    SELECT @InvoiceId, ProductId, Quantity, UnitPrice
    FROM @Items;

    COMMIT;
    SELECT @InvoiceId AS InvoiceId;  -- retorna id generado
  END TRY
  BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK;
    THROW;  -- propaga el error original
  END CATCH
END
GO

-- busqueda de facturas por cliente o por numero, limite por defecto de 50
IF OBJECT_ID('dbo.usp_Invoices_Search') IS NOT NULL DROP PROC dbo.usp_Invoices_Search;
GO
CREATE PROC dbo.usp_Invoices_Search
  @CustomerId INT = NULL,
  @Number     INT = NULL
AS
BEGIN
  SET NOCOUNT ON;

  -- por cliente devuelve lista de encabezados mas recientes primero
  IF @CustomerId IS NOT NULL
  BEGIN
    SELECT i.Id, i.Number, c.Name AS Customer, i.Subtotal, i.Tax, i.Total, i.CreatedAt
    FROM dbo.Invoices i
    JOIN dbo.Customers c ON c.Id = i.CustomerId
    WHERE i.CustomerId = @CustomerId
    ORDER BY i.CreatedAt DESC;
    RETURN;
  END

  -- por numero devuelve encabezado + detalle
  IF @Number IS NOT NULL
  BEGIN
    -- encabezado
    SELECT TOP 1 i.Id, i.Number, c.Name AS Customer, i.Subtotal, i.Tax, i.Total, i.CreatedAt
    FROM dbo.Invoices i
    JOIN dbo.Customers c ON c.Id = i.CustomerId
    WHERE i.Number = @Number;

    -- detalle
    SELECT d.ProductId, p.Name, d.Quantity, d.UnitPrice, d.LineTotal, p.ImageUrl
    FROM dbo.InvoiceDetails d
    JOIN dbo.Products p ON p.Id = d.ProductId
    JOIN dbo.Invoices i ON i.Id = d.InvoiceId
    WHERE i.Number = @Number
    ORDER BY d.Id;
    RETURN;
  END

  -- por defecto devuelve ultimas 50 facturas 
  SELECT TOP 50 i.Id, i.Number, c.Name AS Customer, i.Subtotal, i.Tax, i.Total, i.CreatedAt
  FROM dbo.Invoices i
  JOIN dbo.Customers c ON c.Id = i.CustomerId
  ORDER BY i.CreatedAt DESC;
END
GO

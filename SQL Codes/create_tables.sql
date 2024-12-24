/*==========================================================
||                  CUSTOMER MANAGEMENT                   ||
==========================================================*/

/*-----------------------------------------------
-- Creating Customer table
-----------------------------------------------*/

CREATE TABLE Customer (
    CustomerID INT PRIMARY KEY IDENTITY(1,1),
    FirstName NVARCHAR(100) NOT NULL CHECK (LEN(FirstName) > 1),
    LastName NVARCHAR(100) NOT NULL CHECK (LEN(LastName) > 1),
    Email NVARCHAR(255) UNIQUE CHECK (Email LIKE '%@%.%'),
    PhoneNumber NVARCHAR(15) CHECK (PhoneNumber LIKE '[0-9]%'),
    AddressLine1 NVARCHAR(255),
    AddressLine2 NVARCHAR(255),
    City NVARCHAR(100),
    PostalCode NVARCHAR(20) CHECK (LEN(PostalCode) <= 20),
    RegistrationDate DATE NOT NULL DEFAULT GETDATE()
);

/*==========================================================
||                  EMPLOYEE MANAGEMENT                   ||
==========================================================*/

/*-----------------------------------------------
-- Creating Role table
-----------------------------------------------*/

CREATE TABLE Role (
    RoleID INT PRIMARY KEY IDENTITY(1,1),
    RoleName NVARCHAR(100) NOT NULL CHECK (LEN(RoleName) > 0)
);

/*-----------------------------------------------
-- Creating Employee table
-----------------------------------------------*/

CREATE TABLE Employee (
    EmployeeID INT PRIMARY KEY IDENTITY(1,1),
    FirstName NVARCHAR(100) NOT NULL CHECK (LEN(FirstName) > 1),
    LastName NVARCHAR(100) NOT NULL CHECK (LEN(LastName) > 1),
    RoleID INT NOT NULL,
    Email NVARCHAR(255) UNIQUE NOT NULL CHECK (Email LIKE '%@%.%'),
    PhoneNumber NVARCHAR(15) CHECK (PhoneNumber LIKE '[0-9]%'),
    HireDate DATE NOT NULL CHECK (HireDate <= GETDATE()),
    FOREIGN KEY (RoleID) REFERENCES Role(RoleID) ON DELETE CASCADE
);

/*-----------------------------------------------
-- Creating AuditLog table
-----------------------------------------------*/

CREATE TABLE AuditLog (
    LogID INT PRIMARY KEY IDENTITY(1,1),
    Action NVARCHAR(255) NOT NULL,
    PerformedBy INT NULL, -- Allow NULL values for SET NULL action
    ActionTimestamp DATETIME NOT NULL,
    TableName NVARCHAR(100),
    FOREIGN KEY (PerformedBy) REFERENCES Employee(EmployeeID) ON DELETE SET NULL
);

/*==========================================================
||                   REPORT GENERATION                    ||
==========================================================*/

/*-----------------------------------------------
-- Creating Report table
-----------------------------------------------*/

CREATE TABLE Report (
    ReportID INT PRIMARY KEY IDENTITY(1,1),
    ReportName NVARCHAR(255) NOT NULL CHECK (LEN(ReportName) > 0),
    GeneratedBy INT NOT NULL,
    GeneratedDate DATETIME NOT NULL DEFAULT GETDATE(),
    ReportType NVARCHAR(100) NOT NULL,
    ReportData NVARCHAR(MAX),
    FOREIGN KEY (GeneratedBy) REFERENCES Employee(EmployeeID) ON DELETE CASCADE
);

/*==========================================================
||              PRODUCT AND STOCK MANAGEMENT              ||
==========================================================*/

/*-----------------------------------------------
-- Creating Category table
-----------------------------------------------*/

CREATE TABLE Category (
    CategoryID INT PRIMARY KEY IDENTITY(1,1),
    CategoryName NVARCHAR(100) NOT NULL UNIQUE CHECK (LEN(CategoryName) > 0)
);

/*-----------------------------------------------
-- Creating Brand table
-----------------------------------------------*/

CREATE TABLE Brand (
    BrandID INT PRIMARY KEY IDENTITY(1,1),
    BrandName NVARCHAR(100) NOT NULL UNIQUE CHECK (LEN(BrandName) > 0)
);

/*-----------------------------------------------
-- Creating TaxRate table
-----------------------------------------------*/

CREATE TABLE TaxRate (
    TaxRateID INT PRIMARY KEY IDENTITY(1,1),
    TaxRate DECIMAL(5, 2) NOT NULL CHECK (TaxRate >= 0 AND TaxRate <= 100)
);

/*-----------------------------------------------
-- Creating Product table
-----------------------------------------------*/

CREATE TABLE Product (
    ProductID INT PRIMARY KEY IDENTITY(1,1),
    ProductName NVARCHAR(150) NOT NULL CHECK (LEN(ProductName) > 0),
    CategoryID INT NOT NULL,
    BrandID INT NOT NULL,
    UnitPrice DECIMAL(10, 2) NOT NULL CHECK (UnitPrice >= 0),
    TaxRateID INT NOT NULL,
    ReorderLevel INT NOT NULL CHECK (ReorderLevel >= 0),
    IsActive BIT NOT NULL DEFAULT 1,
    FOREIGN KEY (CategoryID) REFERENCES Category(CategoryID) ON DELETE CASCADE,
    FOREIGN KEY (BrandID) REFERENCES Brand(BrandID) ON DELETE CASCADE,
    FOREIGN KEY (TaxRateID) REFERENCES TaxRate(TaxRateID) ON DELETE CASCADE
);

/*-----------------------------------------------
-- Creating Warehouse table
-----------------------------------------------*/

CREATE TABLE Warehouse (
    WarehouseID INT PRIMARY KEY IDENTITY(1,1),
    WarehouseName NVARCHAR(150) NOT NULL CHECK (LEN(WarehouseName) > 0),
    Location NVARCHAR(255),
    ManagerID INT,
    FOREIGN KEY (ManagerID) REFERENCES Employee(EmployeeID) ON DELETE SET NULL
);

/*-----------------------------------------------
-- Creating Stock table
-----------------------------------------------*/

CREATE TABLE Stock (
    StockID INT PRIMARY KEY IDENTITY(1,1),
    ProductID INT NOT NULL,
    WarehouseID INT NOT NULL,
    QuantityAvailable INT NOT NULL CHECK (QuantityAvailable >= 0),
    LastRestockedDate DATE CHECK (LastRestockedDate <= GETDATE()),
    FOREIGN KEY (ProductID) REFERENCES Product(ProductID) ON DELETE CASCADE,
    FOREIGN KEY (WarehouseID) REFERENCES Warehouse(WarehouseID) ON DELETE CASCADE
);

/*==========================================================
||                   DISCOUNT MANAGEMENT                  ||
==========================================================*/

/*-----------------------------------------------
-- Creating Discount table
-----------------------------------------------*/

CREATE TABLE Discount (
    DiscountID INT PRIMARY KEY IDENTITY(1,1),
    DiscountName NVARCHAR(150) NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    DiscountPercentage DECIMAL(5, 2),
    DiscountType NVARCHAR(50) NOT NULL,
    CONSTRAINT CK_Discount_DateRange CHECK (EndDate >= StartDate) -- Table-level CHECK constraint
);

/*-----------------------------------------------
-- Creating ProductDiscount table
-----------------------------------------------*/

CREATE TABLE ProductDiscount (
    ProductDiscountID INT PRIMARY KEY IDENTITY(1,1),
    ProductID INT NOT NULL,
    DiscountID INT NOT NULL,
    FOREIGN KEY (ProductID) REFERENCES Product(ProductID) ON DELETE CASCADE,
    FOREIGN KEY (DiscountID) REFERENCES Discount(DiscountID) ON DELETE CASCADE
);

/*==========================================================
||                   SALES MANAGEMENT                     ||
==========================================================*/

/*-----------------------------------------------
-- Creating PaymentMethod table
-----------------------------------------------*/

CREATE TABLE PaymentMethod (
    PaymentMethodID INT PRIMARY KEY IDENTITY(1,1),
    PaymentType NVARCHAR(100) NOT NULL CHECK (LEN(PaymentType) > 0)
);

/*-----------------------------------------------
-- Creating SalesTransaction table
-----------------------------------------------*/

CREATE TABLE SalesTransaction (
    TransactionID INT PRIMARY KEY IDENTITY(1,1),
    CustomerID INT,
    TransactionDate DATETIME NOT NULL DEFAULT GETDATE(),
    PaymentMethodID INT NOT NULL,
    TotalAmount DECIMAL(10, 2) NOT NULL CHECK (TotalAmount >= 0),
    DiscountApplied DECIMAL(10, 2) CHECK (DiscountApplied >= 0),
    FinalAmount DECIMAL(10, 2) NOT NULL CHECK (FinalAmount >= 0),
    FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID) ON DELETE SET NULL,
    FOREIGN KEY (PaymentMethodID) REFERENCES PaymentMethod(PaymentMethodID) ON DELETE CASCADE
);

/*-----------------------------------------------
-- Creating SalesTransactionDetail table
-----------------------------------------------*/

CREATE TABLE SalesTransactionDetail (
    TransactionDetailID INT PRIMARY KEY IDENTITY(1,1),
    TransactionID INT NOT NULL,
    ProductID INT NOT NULL,
    Quantity INT NOT NULL CHECK (Quantity > 0),
    UnitPrice DECIMAL(10, 2) NOT NULL CHECK (UnitPrice >= 0),
    LineTotal DECIMAL(10, 2) NOT NULL CHECK (LineTotal >= 0),
    FOREIGN KEY (TransactionID) REFERENCES SalesTransaction(TransactionID) ON DELETE CASCADE,
    FOREIGN KEY (ProductID) REFERENCES Product(ProductID) ON DELETE CASCADE
);

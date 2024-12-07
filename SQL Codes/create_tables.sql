/*==========================================================
||                  CUSTOMER MANAGEMENT                   ||
==========================================================*/

/*-----------------------------------------------
-- Creating Customer table
-----------------------------------------------*/

-- Customer Management
CREATE TABLE Customer (
    CustomerID INT PRIMARY KEY IDENTITY(1,1),
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) UNIQUE,
    PhoneNumber NVARCHAR(15),
    AddressLine1 NVARCHAR(255),
    AddressLine2 NVARCHAR(255),
    City NVARCHAR(100),
    PostalCode NVARCHAR(20),
    RegistrationDate DATE NOT NULL
);


/*==========================================================
||          REPORTING AND ADMINISTRATIVE DATA             ||
==========================================================*/

/*-----------------------------------------------
-- Creating Role table
-----------------------------------------------*/

CREATE TABLE Role (
    RoleID INT PRIMARY KEY IDENTITY(1,1),
    RoleName NVARCHAR(100) NOT NULL
);

/*-----------------------------------------------
-- Creating Employee table
-----------------------------------------------*/

CREATE TABLE Employee (
    EmployeeID INT PRIMARY KEY IDENTITY(1,1),
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    RoleID INT NOT NULL,
    Email NVARCHAR(255) UNIQUE NOT NULL,
    PhoneNumber NVARCHAR(15),
    HireDate DATE NOT NULL,
    FOREIGN KEY (RoleID) REFERENCES Role(RoleID)
);

/*-----------------------------------------------
-- Creating AuditLog table
-----------------------------------------------*/

CREATE TABLE AuditLog (
    LogID INT PRIMARY KEY IDENTITY(1,1),
    Action NVARCHAR(255) NOT NULL,
    PerformedBy INT NOT NULL,
    ActionTimestamp DATETIME NOT NULL,
    TableName NVARCHAR(100),
    FOREIGN KEY (PerformedBy) REFERENCES Employee(EmployeeID)
);

/*-----------------------------------------------
-- Creating Report table
-----------------------------------------------*/

CREATE TABLE Report (
    ReportID INT PRIMARY KEY IDENTITY(1,1),
    ReportName NVARCHAR(255) NOT NULL,
    GeneratedBy INT NOT NULL,
    GeneratedDate DATETIME NOT NULL,
    ReportType NVARCHAR(100) NOT NULL,
    ReportData NVARCHAR(MAX),
    FOREIGN KEY (GeneratedBy) REFERENCES Employee(EmployeeID)
);


/*==========================================================
||          PRODUCT AND STOCK MANAGEMENT MODULE           ||
==========================================================*/

/*-----------------------------------------------
-- Creating Category table
-----------------------------------------------*/

CREATE TABLE Category (
    CategoryID INT PRIMARY KEY IDENTITY(1,1),
    CategoryName NVARCHAR(100) NOT NULL
);

/*-----------------------------------------------
-- Creating Brand table
-----------------------------------------------*/

CREATE TABLE Brand (
    BrandID INT PRIMARY KEY IDENTITY(1,1),
    BrandName NVARCHAR(100) NOT NULL
);

/*-----------------------------------------------
-- Creating TaxRate table
-----------------------------------------------*/

CREATE TABLE TaxRate (
    TaxRateID INT PRIMARY KEY IDENTITY(1,1),
    TaxRate DECIMAL(5, 2) NOT NULL
);

/*-----------------------------------------------
-- Creating Product table
-----------------------------------------------*/

CREATE TABLE Product (
    ProductID INT PRIMARY KEY IDENTITY(1,1),
    ProductName NVARCHAR(150) NOT NULL,
    CategoryID INT NOT NULL,
    BrandID INT NOT NULL,
    UnitPrice DECIMAL(10, 2) NOT NULL,
    TaxRateID INT NOT NULL,
    ReorderLevel INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    FOREIGN KEY (CategoryID) REFERENCES Category(CategoryID),
    FOREIGN KEY (BrandID) REFERENCES Brand(BrandID),
    FOREIGN KEY (TaxRateID) REFERENCES TaxRate(TaxRateID)
);

/*-----------------------------------------------
-- Creating Warehouse table
-----------------------------------------------*/

CREATE TABLE Warehouse (
    WarehouseID INT PRIMARY KEY IDENTITY(1,1),
    WarehouseName NVARCHAR(150) NOT NULL,
    Location NVARCHAR(255),
    ManagerID INT,
    FOREIGN KEY (ManagerID) REFERENCES Employee(EmployeeID)
);

/*-----------------------------------------------
-- Creating Stock table
-----------------------------------------------*/

CREATE TABLE Stock (
    StockID INT PRIMARY KEY IDENTITY(1,1),
    ProductID INT NOT NULL,
    WarehouseID INT NOT NULL,
    QuantityAvailable INT NOT NULL,
    LastRestockedDate DATE,
    FOREIGN KEY (ProductID) REFERENCES Product(ProductID),
    FOREIGN KEY (WarehouseID) REFERENCES Warehouse(WarehouseID)
);


/*==========================================================
||              DISCOUNT MANAGEMENT MODULE                ||
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
    DiscountType NVARCHAR(50) NOT NULL
);

/*-----------------------------------------------
-- Creating ProductDiscount table
-----------------------------------------------*/

CREATE TABLE ProductDiscount (
    ProductDiscountID INT PRIMARY KEY IDENTITY(1,1),
    ProductID INT NOT NULL,
    DiscountID INT NOT NULL,
    FOREIGN KEY (ProductID) REFERENCES Product(ProductID),
    FOREIGN KEY (DiscountID) REFERENCES Discount(DiscountID)
);


/*==========================================================
||                SALES MANAGEMENT MODULE                 ||
==========================================================*/

/*-----------------------------------------------
-- Creating PaymentMethod table
-----------------------------------------------*/

CREATE TABLE PaymentMethod (
    PaymentMethodID INT PRIMARY KEY IDENTITY(1,1),
    PaymentType NVARCHAR(100) NOT NULL
);

/*-----------------------------------------------
-- Creating SalesTransaction table
-----------------------------------------------*/

CREATE TABLE SalesTransaction (
    TransactionID INT PRIMARY KEY IDENTITY(1,1),
    CustomerID INT,
    TransactionDate DATETIME NOT NULL,
    PaymentMethodID INT NOT NULL,
    TotalAmount DECIMAL(10, 2) NOT NULL,
    DiscountApplied DECIMAL(10, 2),
    FinalAmount DECIMAL(10, 2) NOT NULL,
    FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID),
    FOREIGN KEY (PaymentMethodID) REFERENCES PaymentMethod(PaymentMethodID)
);

/*-----------------------------------------------
-- Creating SalesTransactionDetail table
-----------------------------------------------*/

CREATE TABLE SalesTransactionDetail (
    TransactionDetailID INT PRIMARY KEY IDENTITY(1,1),
    TransactionID INT NOT NULL,
    ProductID INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(10, 2) NOT NULL,
    LineTotal DECIMAL(10, 2) NOT NULL,
    FOREIGN KEY (TransactionID) REFERENCES SalesTransaction(TransactionID),
    FOREIGN KEY (ProductID) REFERENCES Product(ProductID)
);

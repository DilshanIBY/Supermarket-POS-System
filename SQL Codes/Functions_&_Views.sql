-- ==========================================
-- Function Name: dbo.fn_CalculateTotalSales
-- Description: Calculates the total amount spent by a customer on all transactions
-- Inputs:
--  @CustomerID (INT) - The ID of the customer
-- Returns:
--  DECIMAL - The total amount spent by the customer
-- ==========================================

CREATE FUNCTION dbo.fn_CalculateTotalSales (@CustomerID INT)
RETURNS DECIMAL(10, 2)
AS
BEGIN
    DECLARE @TotalSales DECIMAL(10, 2);

    -- Calculate the total sales for the customer by summing the FinalAmount from SalesTransaction
    SELECT @TotalSales = SUM(T.FinalAmount)
    FROM SalesTransaction T
    WHERE T.CustomerID = @CustomerID;

    -- Return the result
    RETURN ISNULL(@TotalSales, 0);  -- Return 0 if no sales are found
END



-- ==========================================
-- Function Name: dbo.fn_CalculateProductSales
-- Description: Calculates the total sales for a given product
-- Inputs:
--  @ProductID (INT) - The ID of the product
-- Returns:
--  DECIMAL - The total sales for the product
-- ==========================================

CREATE FUNCTION dbo.fn_CalculateProductSales (@ProductID INT)
RETURNS DECIMAL(10, 2)
AS
BEGIN
    DECLARE @TotalSales DECIMAL(10, 2);

    -- Calculate the total sales for the product by summing the LineTotal from SalesTransactionDetail
    SELECT @TotalSales = SUM(D.LineTotal)
    FROM SalesTransactionDetail D
    WHERE D.ProductID = @ProductID;

    -- Return the result
    RETURN ISNULL(@TotalSales, 0);  -- Return 0 if no sales are found
END


-- ==========================================
-- Function Name: dbo.fn_GetProductAvailability
-- Description: Returns the available quantity of a product in a specific warehouse
-- Inputs:
--  @ProductID (INT) - The ID of the product
--  @WarehouseID (INT) - The ID of the warehouse
-- Returns:
--  INT - The available quantity of the product in the warehouse
-- ==========================================

CREATE FUNCTION dbo.fn_GetProductAvailability (@ProductID INT, @WarehouseID INT)
RETURNS INT
AS
BEGIN
    DECLARE @QuantityAvailable INT;

    -- Get the quantity of the product in the specified warehouse
    SELECT @QuantityAvailable = QuantityAvailable
    FROM Stock
    WHERE ProductID = @ProductID AND WarehouseID = @WarehouseID;

    -- Return the result
    RETURN ISNULL(@QuantityAvailable, 0);  -- Return 0 if the product is not found
END


-- ==========================================
-- Function Name: dbo.fn_GetDiscountedPrice
-- Description: Calculates the price of a product after applying a discount
-- Inputs:
--  @ProductID (INT) - The ID of the product
--  @DiscountID (INT) - The ID of the discount to be applied
-- Returns:
--  DECIMAL - The discounted price of the product
-- ==========================================

CREATE FUNCTION dbo.fn_GetDiscountedPrice (@ProductID INT, @DiscountID INT)
RETURNS DECIMAL(10, 2)
AS
BEGIN
    DECLARE @DiscountPercentage DECIMAL(5, 2);
    DECLARE @UnitPrice DECIMAL(10, 2);
    DECLARE @DiscountedPrice DECIMAL(10, 2);

    -- Get the unit price of the product
    SELECT @UnitPrice = UnitPrice
    FROM Product
    WHERE ProductID = @ProductID;

    -- Get the discount percentage for the given discount ID
    SELECT @DiscountPercentage = DiscountPercentage
    FROM Discount
    WHERE DiscountID = @DiscountID;

    -- Calculate the discounted price
    SET @DiscountedPrice = @UnitPrice - (@UnitPrice * @DiscountPercentage / 100);

    -- Return the discounted price
    RETURN @DiscountedPrice;
END


-- VIEWS --

-- ==========================================
-- View Name: vw_CustomerSalesSummary
-- Description: Summary view of total sales and transaction count for each customer
-- ==========================================

CREATE VIEW vw_CustomerSalesSummary AS
SELECT 
    C.CustomerID,
    C.FirstName + ' ' + C.LastName AS CustomerName,
    COUNT(T.TransactionID) AS TotalTransactions,
    SUM(T.FinalAmount) AS TotalSpent,
    MAX(T.TransactionDate) AS LastTransactionDate
FROM 
    Customer C
    LEFT JOIN SalesTransaction T ON C.CustomerID = T.CustomerID
GROUP BY 
    C.CustomerID, C.FirstName, C.LastName;


-- ==========================================
-- View Name: vw_ProductSalesOverview
-- Description: Provides an overview of products with total sales and current stock
-- ==========================================

CREATE VIEW vw_ProductSalesOverview AS
SELECT 
    P.ProductID,
    P.ProductName,
    ISNULL(SUM(D.LineTotal), 0) AS TotalSales,
    ISNULL(S.QuantityAvailable, 0) AS StockAvailable
FROM 
    Product P
    LEFT JOIN SalesTransactionDetail D ON P.ProductID = D.ProductID
    LEFT JOIN Stock S ON P.ProductID = S.ProductID
GROUP BY 
    P.ProductID, P.ProductName, S.QuantityAvailable;



-- ==========================================
-- View Name: vw_EmployeeRoleSummary
-- Description: Provides employee details along with their roles and contact info
-- ==========================================

CREATE VIEW vw_EmployeeRoleSummary AS
SELECT 
    E.EmployeeID,
    E.FirstName + ' ' + E.LastName AS EmployeeName,
    R.RoleName,
    E.Email,
    E.PhoneNumber,
    E.HireDate
FROM 
    Employee E
    JOIN Role R ON E.RoleID = R.RoleID;



-- ==========================================
-- View Name: vw_SalesTransactionDetails
-- Description: Provides details of each sales transaction, including product info
-- ==========================================

CREATE VIEW vw_SalesTransactionDetails AS
SELECT 
    T.TransactionID,
    C.FirstName + ' ' + C.LastName AS CustomerName,
    P.ProductName,
    D.Quantity,
    D.UnitPrice,
    D.LineTotal,
    T.TransactionDate
FROM 
    SalesTransaction T
    JOIN Customer C ON T.CustomerID = C.CustomerID
    JOIN SalesTransactionDetail D ON T.TransactionID = D.TransactionID
    JOIN Product P ON D.ProductID = P.ProductID;

-- ==========================================
-- Procedure Name: sp_InsertCustomer
-- Description: Inserts a new customer into the Customer table and logs the action in AuditLog
-- Inputs:
--  @FirstName (NVARCHAR) - First name of the customer
--  @LastName (NVARCHAR) - Last name of the customer
--  @Email (NVARCHAR) - Email of the customer (must be unique)
--  @PhoneNumber (NVARCHAR) - Phone number of the customer (optional)
--  @AddressLine1, @AddressLine2, @City, @PostalCode - Customer address details (optional)
-- ==========================================

CREATE PROCEDURE sp_InsertCustomer
    @FirstName NVARCHAR(100),
    @LastName NVARCHAR(100),
    @Email NVARCHAR(255),
    @PhoneNumber NVARCHAR(15) = NULL,
    @AddressLine1 NVARCHAR(255) = NULL,
    @AddressLine2 NVARCHAR(255) = NULL,
    @City NVARCHAR(100) = NULL,
    @PostalCode NVARCHAR(20) = NULL
AS
BEGIN
    -- Begin error handling block
    BEGIN TRY
        -- Insert new customer into the Customer table
        INSERT INTO Customer 
            (FirstName, LastName, Email, PhoneNumber, AddressLine1, AddressLine2, City, PostalCode, RegistrationDate)
        VALUES 
            (@FirstName, @LastName, @Email, @PhoneNumber, @AddressLine1, @AddressLine2, @City, @PostalCode, GETDATE());

        -- Get the CustomerID of the newly inserted record
        DECLARE @CustomerID INT = SCOPE_IDENTITY();

        -- Log the action in the AuditLog
        INSERT INTO AuditLog (Action, PerformedBy, ActionTimestamp, TableName)
        VALUES ('Insert New Customer', 1, GETDATE(), 'Customer'); -- Assuming PerformedBy = 1 for now

        -- Print success message
        PRINT 'Customer inserted successfully with CustomerID = ' + CAST(@CustomerID AS NVARCHAR(20));
    END TRY
    BEGIN CATCH
        -- Print error message
        PRINT 'Error: ' + ERROR_MESSAGE();
    END CATCH
END




-- ==========================================
-- Procedure Name: sp_GenerateSalesReport
-- Description: Generate a sales report for a given date range
-- Inputs:
--  @StartDate (DATE) - The start date of the report
--  @EndDate (DATE) - The end date of the report
-- ==========================================

CREATE PROCEDURE sp_GenerateSalesReport
    @StartDate DATE,
    @EndDate DATE
AS
BEGIN
    -- Begin error handling block
    BEGIN TRY
        -- Select all transactions that happened in the date range
        SELECT 
            T.TransactionID,
            C.FirstName + ' ' + C.LastName AS CustomerName,
            T.TransactionDate,
            P.ProductName,
            D.Quantity,
            D.LineTotal
        FROM 
            SalesTransaction T
            JOIN Customer C ON T.CustomerID = C.CustomerID
            JOIN SalesTransactionDetail D ON T.TransactionID = D.TransactionID
            JOIN Product P ON D.ProductID = P.ProductID
        WHERE 
            T.TransactionDate BETWEEN @StartDate AND @EndDate;

        -- Log the report generation
        INSERT INTO AuditLog (Action, PerformedBy, ActionTimestamp, TableName)
        VALUES ('Generate Sales Report', 1, GETDATE(), 'SalesTransaction');
    END TRY
    BEGIN CATCH
        -- Print error message
        PRINT 'Error: ' + ERROR_MESSAGE();
    END CATCH
END


-- ==========================================
-- Procedure Name: sp_AssignProductDiscount
-- Description: Assigns a discount to a product and ensures no duplicates
-- Inputs:
--  @ProductID (INT) - The ID of the product to which the discount will be assigned
--  @DiscountID (INT) - The ID of the discount to be applied
-- ==========================================

CREATE PROCEDURE sp_AssignProductDiscount
    @ProductID INT,
    @DiscountID INT
AS
BEGIN
    -- Begin error handling block
    BEGIN TRY
        -- Check if this product-discount combination already exists
        IF NOT EXISTS (
            SELECT 1 
            FROM ProductDiscount 
            WHERE ProductID = @ProductID AND DiscountID = @DiscountID
        )
        BEGIN
            -- Insert a new product-discount association
            INSERT INTO ProductDiscount (ProductID, DiscountID)
            VALUES (@ProductID, @DiscountID);

            -- Print success message
            PRINT 'Product assigned to discount successfully.';
        END
        ELSE
        BEGIN
            -- Notify if the product-discount combo already exists
            PRINT 'Product is already assigned to this discount.';
        END
    END TRY
    BEGIN CATCH
        -- Print error message
        PRINT 'Error: ' + ERROR_MESSAGE();
    END CATCH
END


-- ==========================================
-- Trigger Name: trg_UpdateStockAfterSale
-- Description: After a new sale, reduce the stock of the sold product
-- Trigger Fires: AFTER INSERT on SalesTransactionDetail
-- ==========================================

CREATE TRIGGER trg_UpdateStockAfterSale 
ON SalesTransactionDetail 
AFTER INSERT
AS
BEGIN
    -- Turn off additional result sets for the trigger
    SET NOCOUNT ON;

    -- Reduce the stock quantity based on the product and quantity sold
    UPDATE S
    SET S.QuantityAvailable = S.QuantityAvailable - I.Quantity
    FROM Stock S
    INNER JOIN INSERTED I ON S.ProductID = I.ProductID
    WHERE S.WarehouseID = 1; -- Adjust WarehouseID as required

    -- Log the stock update action
    INSERT INTO AuditLog (Action, PerformedBy, ActionTimestamp, TableName)
    VALUES ('Stock Updated After Sale', 1, GETDATE(), 'Stock');
END


-- ==========================================
-- Trigger Name: trg_ReorderNotification
-- Description: Triggers when stock is updated, logs an alert if quantity falls below reorder level
-- Trigger Fires: AFTER UPDATE on Stock
-- ==========================================

CREATE TRIGGER trg_ReorderNotification 
ON Stock 
AFTER UPDATE
AS
BEGIN
    -- Turn off additional result sets for the trigger
    SET NOCOUNT ON;

    -- Insert an alert into the AuditLog if stock is below reorder level
    INSERT INTO AuditLog (Action, PerformedBy, ActionTimestamp, TableName)
    SELECT 
        'Reorder Alert: Stock Low for ProductID ' + CAST(I.ProductID AS NVARCHAR(10)),
        1, 
        GETDATE(), 
        'Stock'
    FROM INSERTED I
    JOIN Product P ON I.ProductID = P.ProductID
    WHERE I.QuantityAvailable < P.ReorderLevel;
END


-- ==========================================
-- Trigger Name: trg_LogCustomerDeletion
-- Description: Logs the deletion of a customer in the AuditLog
-- Trigger Fires: AFTER DELETE on Customer
-- ==========================================

CREATE TRIGGER trg_LogCustomerDeletion 
ON Customer 
AFTER DELETE
AS
BEGIN
    -- Turn off additional result sets for the trigger
    SET NOCOUNT ON;

    -- Insert log into AuditLog table
    INSERT INTO AuditLog (Action, PerformedBy, ActionTimestamp, TableName)
    SELECT 
        'Deleted Customer: ' + D.FirstName + ' ' + D.LastName + ' (Email: ' + D.Email + ')',
        1, -- Assuming PerformedBy = 1 (can be adjusted based on user context)
        GETDATE(), 
        'Customer'
    FROM DELETED D;
END

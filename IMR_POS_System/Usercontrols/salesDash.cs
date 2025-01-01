using POS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IMR_POS_System.Usercontrols
{
    public partial class salesDash : UserControl
    {

        // INITIALIZE & LOAD
        public salesDash()
        {
            InitializeComponent();
            InitializeGrid();
            InitializeTransactionDetailGrid();
            InitializePaymentMethodGrid();
        }
        private void sales_Load(object sender, EventArgs e)
        {
            btnSales.PerformClick(); // Default state
            loadIDs();
            LoadData();
            viewdetails();
            PID.SelectedIndex = 0;  // Select the first item by default (index 0)
            DID.SelectedIndex = 0;  // Select the first item by default (index 0)
        }
        private void loadIDs()
        {
            try
            {
                db db = new db();
                // Load TransactionIDs into TID combo box
                using (SqlDataReader reader = db.Select("SELECT TransactionID FROM SalesTransaction"))
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("TransactionID", typeof(int)); // Ensure consistent data type
                    dt.Columns.Add("DisplayText", typeof(string)); // For user-friendly display

                    // Add "New" placeholder row
                    DataRow newRow = dt.NewRow();
                    newRow["TransactionID"] = -1; // Reserved value for "new item"
                    newRow["DisplayText"] = "New"; // What the user sees
                    dt.Rows.Add(newRow);

                    // Add actual TransactionIDs
                    while (reader.Read())
                    {
                        DataRow row = dt.NewRow();
                        row["TransactionID"] = reader.GetInt32(0); // Assuming TransactionID is an int in your database
                        row["DisplayText"] = reader.GetInt32(0).ToString(); // Display as string
                        dt.Rows.Add(row);
                    }

                    // Bind to the ComboBox
                    TID.DisplayMember = "DisplayText"; // Text shown to the user
                    TID.ValueMember = "TransactionID"; // Actual value (int) used internally
                    TID.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load Transaction data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadData()
        {
            try
            {
                db db = new db();

                // Load PaymentMethod IDs into payment method ComboBox
                DataTable paymentMethodTable = new DataTable();
                paymentMethodTable.Columns.Add("PaymentMethodID", typeof(int));
                paymentMethodTable.Columns.Add("PaymentType", typeof(string));

                // Add a default empty row
                DataRow emptyPaymentMethodRow = paymentMethodTable.NewRow();
                emptyPaymentMethodRow["PaymentMethodID"] = DBNull.Value; // No value selected
                emptyPaymentMethodRow["PaymentType"] = ""; // Display as empty
                paymentMethodTable.Rows.Add(emptyPaymentMethodRow);

                using (SqlDataReader reader = db.Select("SELECT PaymentMethodID, PaymentType FROM PaymentMethod"))
                {
                    while (reader.Read())
                    {
                        DataRow row = paymentMethodTable.NewRow();
                        row["PaymentMethodID"] = reader.GetInt32(0); // Assuming PaymentMethodID is int
                        row["PaymentType"] = reader.GetString(1); // PaymentType as string
                        paymentMethodTable.Rows.Add(row);
                    }
                }
                cmbPaymentType.DisplayMember = "PaymentType";
                cmbPaymentType.ValueMember = "PaymentMethodID";
                cmbPaymentType.DataSource = paymentMethodTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private bool CheckCustomerID(string customerName)
        {
            try
            {
                // Query to check if the customer name exists
                string checkQuery = "SELECT COUNT(*) FROM Customer WHERE Lastname = @Lastname";
                db db = new db();
                SqlParameter[] checkParams = {
                    new SqlParameter("@Lastname", customerName)
                };

                // Check if the customer exists
                int count = Convert.ToInt32(db.ExecuteScalar(checkQuery, checkParams));
                if (count == 0)
                {
                    return false; // Return "false" if customer does not exist
                }
                else
                {
                    return true; // Return "true" if customer does exist
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error checking CustomerID: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false; // Return false if an error occurs
            }
        }
        private string GetCustomerID(string customerName)
        {
            try
            {
                db db = new db();
                // Query to get the CustomerID
                string query = "SELECT CustomerID FROM Customer WHERE Lastname = @Lastname";
                SqlParameter[] parameters = {
                    new SqlParameter("@Lastname", customerName)
                };
                return db.ExecuteScalar(query, parameters).ToString(); // Returns the CustomerID
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching CustomerID: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "0"; // Return "0" if an error occurs
            }
        }
        private string GetCustomerName(string customerID)
        {
            try
            {
                db db = new db();
                string query = $"SELECT Lastname FROM Customer WHERE CustomerID = '{customerID}'";
                using (SqlDataReader reader = db.Select(query))
                {
                    if (reader.Read())
                    {
                        return reader["Lastname"].ToString(); // Returns the CustomerName
                    }
                }
                return string.Empty; // Return empty string if no record is found
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching Customer Lastname: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return string.Empty; // Return empty string if an error occurs
            }
        }
        private string GetPaymentMethodID(string paymentType)
        {
            try
            {
                string query = "SELECT PaymentMethodID FROM PaymentMethod WHERE PaymentType = @PaymentType";
                db db = new db();
                SqlParameter[] parameters = {
                    new SqlParameter("@PaymentType", paymentType)
                };
                return db.ExecuteScalar(query, parameters).ToString(); // Returns the PaymentMethodID
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching PaymentMethodID: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "0"; // Return "0" if an error occurs
            }
        }
        private string GetPaymentType(string paymentMethodID)
        {
            try
            {
                db db = new db();
                string query = $"SELECT PaymentType FROM PaymentMethod WHERE PaymentMethodID = '{paymentMethodID}'";
                using (SqlDataReader reader = db.Select(query))
                {
                    if (reader.Read())
                    {
                        return reader["PaymentType"].ToString(); // Returns the PaymentType
                    }
                }
                return string.Empty; // Return empty string if no record is found
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching PaymentType: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return string.Empty; // Return empty string if an error occurs
            }
        }


        // GRID
        private DataTable GetPaymentMethodList()
        {
            string query = "SELECT PaymentMethodID, PaymentType FROM PaymentMethod";
            return new db().SelectDataTable(query);
        }
        private DataTable GetProductList()
        {
            string query = "SELECT ProductID, ProductName FROM Product";
            return new db().SelectDataTable(query);
        }
        private void InitializeGrid()
        {
            dataGridView1.Columns.Clear();

            // TransactionID Column
            var transactionIDColumn = new DataGridViewTextBoxColumn
            {
                Name = "TransactionID",
                HeaderText = "Transaction ID"
            };
            dataGridView1.Columns.Add(transactionIDColumn);

            // LastName (Customer) Column
            var lastNameColumn = new DataGridViewTextBoxColumn
            {
                Name = "LastName",
                HeaderText = "Customer Last Name"
            };
            dataGridView1.Columns.Add(lastNameColumn);

            // TransactionDate Column
            var transactionDateColumn = new DataGridViewTextBoxColumn
            {
                Name = "TransactionDate",
                HeaderText = "Transaction Date"
            };
            dataGridView1.Columns.Add(transactionDateColumn);

            // PaymentType ComboBox Column
            var paymentTypeColumn = new DataGridViewComboBoxColumn
            {
                Name = "PaymentType",
                HeaderText = "Payment Type",
                DataSource = GetPaymentMethodList(), // Fetch payment types from DB
                DisplayMember = "PaymentType",
                ValueMember = "PaymentMethodID"
            };
            dataGridView1.Columns.Add(paymentTypeColumn);

            // TotalAmount Column
            var totalAmountColumn = new DataGridViewTextBoxColumn
            {
                Name = "TotalAmount",
                HeaderText = "Total Amount"
            };
            dataGridView1.Columns.Add(totalAmountColumn);

            // DiscountApplied Column
            var discountAppliedColumn = new DataGridViewTextBoxColumn
            {
                Name = "DiscountApplied",
                HeaderText = "Discount Applied"
            };
            dataGridView1.Columns.Add(discountAppliedColumn);

            // FinalAmount Column
            var finalAmountColumn = new DataGridViewTextBoxColumn
            {
                Name = "FinalAmount",
                HeaderText = "Final Amount"
            };
            dataGridView1.Columns.Add(finalAmountColumn);
        }
        private void viewdetails()
        {
            string query = @"
            SELECT 
                st.TransactionID,
                c.LastName,
                st.TransactionDate,
                pm.PaymentType,
                st.TotalAmount,
                st.DiscountApplied,
                st.FinalAmount,
                st.PaymentMethodID
            FROM 
                SalesTransaction st
            LEFT JOIN 
                Customer c ON st.CustomerID = c.CustomerID
            LEFT JOIN 
                PaymentMethod pm ON st.PaymentMethodID = pm.PaymentMethodID";

            var reader = new db().Select(query);
            dataGridView1.Rows.Clear();

            while (reader.Read())
            {
                dataGridView1.Rows.Add(
                    reader["TransactionID"],
                    reader["LastName"],
                    reader["TransactionDate"],
                    reader["PaymentType"],
                    reader["TotalAmount"],
                    reader["DiscountApplied"],
                    reader["FinalAmount"]
                );
            }
        }
        private void InitializeTransactionDetailGrid()
        {
            dataGridView2.Columns.Clear();

            // TransactionDetailID Column (hidden)
            var transactionDetailIDColumn = new DataGridViewTextBoxColumn
            {
                Name = "TransactionDetailID",
                HeaderText = "Transaction Detail ID"
            };
            dataGridView2.Columns.Add(transactionDetailIDColumn);

            // TransactionID Column
            var transactionIDColumn = new DataGridViewTextBoxColumn
            {
                Name = "TransactionID",
                HeaderText = "Transaction ID",
                Visible = false
            };
            dataGridView2.Columns.Add(transactionIDColumn);

            // Product ComboBox Column
            var productColumn = new DataGridViewComboBoxColumn
            {
                Name = "Product",
                HeaderText = "Product",
                DataSource = GetProductList(), // Fetch products from DB
                DisplayMember = "ProductName",
                ValueMember = "ProductID"
            };
            dataGridView2.Columns.Add(productColumn);

            // Quantity Column
            var quantityColumn = new DataGridViewTextBoxColumn
            {
                Name = "Quantity",
                HeaderText = "Quantity"
            };
            dataGridView2.Columns.Add(quantityColumn);

            // UnitPrice Column
            var unitPriceColumn = new DataGridViewTextBoxColumn
            {
                Name = "UnitPrice",
                HeaderText = "Unit Price"
            };
            dataGridView2.Columns.Add(unitPriceColumn);

            // LineTotal Column
            var lineTotalColumn = new DataGridViewTextBoxColumn
            {
                Name = "LineTotal",
                HeaderText = "Line Total"
            };
            dataGridView2.Columns.Add(lineTotalColumn);
        }
        private void ViewTransactionDetails()
        {
            string query = @"
                    SELECT 
                        td.TransactionDetailID,
                        td.TransactionID,
                        p.ProductName,
                        td.Quantity,
                        td.UnitPrice,
                        (td.Quantity * td.UnitPrice) AS LineTotal,
                        td.ProductID
                    FROM 
                        SalesTransactionDetail td
                    LEFT JOIN 
                        Product p ON td.ProductID = p.ProductID";

            var reader = new db().Select(query);
            dataGridView2.Rows.Clear();

            while (reader.Read())
            {
                dataGridView2.Rows.Add(
                    reader["TransactionDetailID"],
                    reader["TransactionID"],
                    reader["ProductName"],
                    reader["Quantity"],
                    reader["UnitPrice"],
                    reader["LineTotal"]
                );
            }
        }
        private void InitializePaymentMethodGrid()
        {
            dataGridView3.Columns.Clear();

            // PaymentMethodID Column
            var paymentMethodIDColumn = new DataGridViewTextBoxColumn
            {
                Name = "PaymentMethodID",
                HeaderText = "Payment Method ID"
            };
            dataGridView3.Columns.Add(paymentMethodIDColumn);

            // PaymentType Column
            var paymentTypeColumn = new DataGridViewTextBoxColumn
            {
                Name = "PaymentType",
                HeaderText = "Payment Type"
            };
            dataGridView3.Columns.Add(paymentTypeColumn);
        }
        private void ViewPaymentMethods()
        {
            string query = @"
                    SELECT 
                        pm.PaymentMethodID,
                        pm.PaymentType
                    FROM 
                        PaymentMethod pm";

            var reader = new db().Select(query);
            dataGridView3.Rows.Clear();

            while (reader.Read())
            {
                dataGridView3.Rows.Add(
                    reader["PaymentMethodID"],
                    reader["PaymentType"]
                );
            }
        }
        private void ClearFields()
        {
            txtCustomerLastName.Clear();
            cmbPaymentType.SelectedIndex = -1;
            txtTotalAmount.Clear();
            txtDiscountApplied.Clear();
            txtFinalAmount.Clear();
            dtpTransactionDate.Value = DateTime.Today;
        }
        private void dataGridView1_DataError_1(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Context == DataGridViewDataErrorContexts.Commit)
            {
                MessageBox.Show("Invalid value. Please select from the dropdown list.");
                e.Cancel = true;
            }
        }
        private void dataGridView2_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Context == DataGridViewDataErrorContexts.Commit)
            {
                MessageBox.Show("Invalid value. Please select from the dropdown list.");
                e.Cancel = true;
            }
        }



        // BUTTONS
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            // Validate input fields
            string CustomerLastName = txtCustomerLastName.Text.Trim();
            string PaymentType = cmbPaymentType.Text.Trim();
            string TransactionDate = dtpTransactionDate.Value.ToString("yyyy-MM-dd");
            string TotalAmount = txtTotalAmount.Text.Trim();
            string DiscountApplied = txtDiscountApplied.Text.Trim();
            string FinalAmount = txtFinalAmount.Text.Trim();

            // Check if the customer is registered
            bool isCustomerRegistered = CheckCustomerID(CustomerLastName);
            if (!isCustomerRegistered)
            {
                MessageBox.Show("This Customer is not registered!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Stop execution
            }

            // Get related IDs
            string CustomerID = GetCustomerID(CustomerLastName);
            string PaymentMethodID = GetPaymentMethodID(PaymentType);

            if (string.IsNullOrEmpty(CustomerID) || string.IsNullOrEmpty(PaymentMethodID))
            {
                MessageBox.Show("Invalid Customer Name or Payment Type.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Initialize database connection
            db DB = new db();

            string query = $@"
            INSERT INTO SalesTransaction (CustomerID, TransactionDate, PaymentMethodID, TotalAmount, DiscountApplied, FinalAmount)
            VALUES ('{CustomerID}', '{TransactionDate}', '{PaymentMethodID}', '{TotalAmount}', '{DiscountApplied}', '{FinalAmount}')";

            try
            {
                // Execute SQL query
                DB.Execute(query);
                MessageBox.Show("Transaction inserted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                viewdetails(); // Refresh the grid
                loadIDs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            // Validate input fields
            string TransactionID = TID.Text.Trim();
            string CustomerLastName = txtCustomerLastName.Text.Trim();
            string PaymentType = cmbPaymentType.Text.Trim();
            string TransactionDate = dtpTransactionDate.Value.ToString("yyyy-MM-dd");
            string TotalAmount = txtTotalAmount.Text.Trim();
            string DiscountApplied = txtDiscountApplied.Text.Trim();
            string FinalAmount = txtFinalAmount.Text.Trim();

            // Check if the customer is registered
            bool isCustomerRegistered = CheckCustomerID(CustomerLastName);
            if (!isCustomerRegistered)
            {
                MessageBox.Show("This Customer is not registered!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Stop execution
            }

            // Get related IDs
            string CustomerID = GetCustomerID(CustomerLastName);
            string PaymentMethodID = GetPaymentMethodID(PaymentType);

            // Check if TransactionID is a valid integer
            if (!int.TryParse(TransactionID, out _))
            {
                MessageBox.Show("Please select a valid TransactionID to make Updates.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(CustomerID) || string.IsNullOrEmpty(PaymentMethodID))
            {
                MessageBox.Show("Invalid Customer Name or Payment Type.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Initialize database connection
            db DB = new db();

            string query = $@"
            UPDATE SalesTransaction 
            SET CustomerID='{CustomerID}', 
                TransactionDate='{TransactionDate}', 
                PaymentMethodID='{PaymentMethodID}', 
                TotalAmount='{TotalAmount}', 
                DiscountApplied='{DiscountApplied}', 
                FinalAmount='{FinalAmount}'
            WHERE TransactionID='{TransactionID}'";

            try
            {
                // Execute SQL query
                DB.Execute(query);
                MessageBox.Show("Transaction updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                viewdetails(); // Refresh the grid
                loadIDs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            // Validate input fields
            string TransactionID = TID.Text.Trim();

            // Check if TransactionID is a valid integer
            if (!int.TryParse(TransactionID, out int parsedTransactionID))
            {
                MessageBox.Show("Please select a valid TransactionID to delete.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Show confirmation message
            DialogResult result = MessageBox.Show(
                $"Are you sure you want to delete the TransactionID Row {parsedTransactionID}?",
                "Confirm Deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            // If the user confirms, proceed with deletion
            if (result == DialogResult.Yes)
            {
                // Initialize database connection
                db DB = new db();
                string query = $@"DELETE FROM SalesTransaction WHERE TransactionID='{parsedTransactionID}'";

                try
                {
                    // Execute SQL query
                    DB.Execute(query);
                    MessageBox.Show("Transaction deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearFields();
                    viewdetails(); // Refresh the grid
                    loadIDs();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void TID_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (sender is ComboBox comboBox) // Ensure sender is a ComboBox
            {
                string newTransactionID = comboBox.SelectedValue?.ToString(); // Get the actual TransactionID value
                try
                {
                    db db = new db();

                    string query = $@"SELECT TransactionDate, CustomerID, PaymentMethodID, TotalAmount, DiscountApplied, FinalAmount
                              FROM SalesTransaction WHERE TransactionID='{newTransactionID}'";

                    using (SqlDataReader reader = db.Select(query))
                    {
                        if (reader.Read()) // Check if data exists
                        {
                            string CustomerID = reader["CustomerID"].ToString();
                            string PaymentMethodID = reader["PaymentMethodID"].ToString();

                            dtpTransactionDate.Text = reader["TransactionDate"].ToString();
                            txtCustomerLastName.Text = GetCustomerName(CustomerID); // Assuming GetCustomerLastName is defined to fetch last name from CustomerID
                            cmbPaymentType.Text = GetPaymentType(PaymentMethodID); // Assuming GetPaymentType is defined to fetch payment type from PaymentType
                            txtTotalAmount.Text = reader["TotalAmount"].ToString();
                            txtDiscountApplied.Text = reader["DiscountApplied"].ToString();
                            txtFinalAmount.Text = reader["FinalAmount"].ToString();
                        }
                        else
                        {
                            // Handle case where no data is found
                            MessageBox.Show("No transaction found with the specified TransactionID.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to load Transaction data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSales_Click(object sender, EventArgs e)
        {
            grpSales.Visible = true;
            grpTransactions.Visible = false;
            grpPayments.Visible = false;
            
            SetButtonStyle(btnSales, true);
            SetButtonStyle(btnTransactions, false);
            SetButtonStyle(btnPayments, false);
        }
        private void btnTransactions_Click(object sender, EventArgs e)
        {
            grpTransactions.Visible = true;
            grpSales.Visible = false;
            grpPayments.Visible = false;
            ViewTransactionDetails();

            SetButtonStyle(btnTransactions, true);
            SetButtonStyle(btnSales, false);
            SetButtonStyle(btnPayments, false);
        }
        private void btnPayments_Click(object sender, EventArgs e)
        {
            grpPayments.Visible = true;
            grpTransactions.Visible = true;
            grpSales.Visible = false;
            ViewPaymentMethods();

            SetButtonStyle(btnPayments, true);
            SetButtonStyle(btnTransactions, false);
            SetButtonStyle(btnSales, false);
        }
        private void SetButtonStyle(Button button, bool isActive)
        {
            if (isActive)
            {
                // Apply clicked style
                button.BackColor = Color.LightBlue;
                button.Font = new Font(button.Font, FontStyle.Bold);
            }
            else
            {
                // Reset to default style
                button.BackColor = SystemColors.Control;
                button.Font = new Font(button.Font, FontStyle.Regular);
            }
        }
    }
}

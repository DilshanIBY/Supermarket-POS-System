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
    public partial class stocksDash : UserControl
    {

        // INITIALIZE & LOAD
        public stocksDash()
        {
            InitializeComponent();
            InitializeStockGrid();
            InitializeWarehouseGrid();
        }
        private void stocks_Load(object sender, EventArgs e)
        {
            btnStocks.PerformClick(); // Default state
            loadIDs();
            LoadData();
            viewdetails();
            WID.SelectedIndex = 0;  // Select the first item by default (index 0)
        }
        private void loadIDs()
        {
            try
            {

                db db = new db();
                // Load StockIDs into PID combo box
                using (SqlDataReader reader = db.Select("SELECT StockID FROM Stock"))
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("StockID", typeof(int)); // Ensure consistent data type
                    dt.Columns.Add("DisplayText", typeof(string)); // For user-friendly display

                    // Add "New" placeholder row
                    DataRow newRow = dt.NewRow();
                    newRow["StockID"] = -1; // Reserved value for "new item"
                    newRow["DisplayText"] = "New"; // What the user sees
                    dt.Rows.Add(newRow);

                    // Add actual StockIDs
                    while (reader.Read())
                    {
                        DataRow row = dt.NewRow();
                        row["StockID"] = reader.GetInt32(0); // Assuming StockID is an int in your database
                        row["DisplayText"] = reader.GetInt32(0).ToString(); // Display as string
                        dt.Rows.Add(row);
                    }

                    // Bind to the ComboBox
                    SID.DisplayMember = "DisplayText"; // Text shown to the user
                    SID.ValueMember = "StockID";    // Actual value (int) used internally
                    SID.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load Stock data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadData()
        {
            try
            {
                db db = new db();

                // Load Product IDs into pname ComboBox
                DataTable productTable = new DataTable();
                productTable.Columns.Add("ProductID", typeof(int));
                productTable.Columns.Add("ProductName", typeof(string));

                // Add a default empty row
                DataRow emptyProductRow = productTable.NewRow();
                emptyProductRow["ProductID"] = DBNull.Value; // No value selected
                emptyProductRow["ProductName"] = ""; // Display as empty
                productTable.Rows.Add(emptyProductRow);

                using (SqlDataReader reader = db.Select("SELECT ProductID, ProductName FROM Product"))
                {
                    while (reader.Read())
                    {
                        DataRow row = productTable.NewRow();
                        row["ProductID"] = reader.GetInt32(0); // Assuming ProductID is int
                        row["ProductName"] = reader.GetString(1); // ProductName as string
                        productTable.Rows.Add(row);
                    }
                }
                pname.DisplayMember = "ProductName";
                pname.ValueMember = "ProductID";
                pname.DataSource = productTable;

                // Load Warehouse IDs into wname ComboBox
                DataTable warehouseTable = new DataTable();
                warehouseTable.Columns.Add("WarehouseID", typeof(int));
                warehouseTable.Columns.Add("WarehouseName", typeof(string));

                // Add a default empty row
                DataRow emptyWarehouseRow = warehouseTable.NewRow();
                emptyWarehouseRow["WarehouseID"] = DBNull.Value; // No value selected
                emptyWarehouseRow["WarehouseName"] = ""; // Display as empty
                warehouseTable.Rows.Add(emptyWarehouseRow);

                using (SqlDataReader reader = db.Select("SELECT WarehouseID, WarehouseName FROM Warehouse"))
                {
                    while (reader.Read())
                    {
                        DataRow row = warehouseTable.NewRow();
                        row["WarehouseID"] = reader.GetInt32(0); // Assuming WarehouseID is int
                        row["WarehouseName"] = reader.GetString(1); // WarehouseName as string
                        warehouseTable.Rows.Add(row);
                    }
                }
                wname.DisplayMember = "WarehouseName";
                wname.ValueMember = "WarehouseID";
                wname.DataSource = warehouseTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private string GetProductID(string productName)
        {
            try
            {
                string query = "SELECT ProductID FROM Product WHERE ProductName = @ProductName";
                db db = new db();
                SqlParameter[] parameters = {
            new SqlParameter("@ProductName", productName)
        };
                return db.ExecuteScalar(query, parameters).ToString(); // Returns the ProductID
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching ProductID: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "0"; // Return "0" if an error occurs
            }
        }
        private string GetWarehouseID(string warehouseName)
        {
            try
            {
                string query = "SELECT WarehouseID FROM Warehouse WHERE WarehouseName = @WarehouseName";
                db db = new db();
                SqlParameter[] parameters = {
            new SqlParameter("@WarehouseName", warehouseName)
        };
                return db.ExecuteScalar(query, parameters).ToString(); // Returns the WarehouseID
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching WarehouseID: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "0"; // Return "0" if an error occurs
            }
        }
        private string GetProductName(string productID)
        {
            try
            {
                db db = new db();
                string query = $"SELECT ProductName FROM Product WHERE ProductID = '{productID}'";
                using (SqlDataReader reader = db.Select(query))
                {
                    if (reader.Read())
                    {
                        return reader["ProductName"].ToString(); // Returns the ProductName
                    }
                }
                return string.Empty; // Return empty string if no record is found
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching ProductName: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return string.Empty; // Return empty string if an error occurs
            }
        }
        private string GetWarehouseName(string warehouseID)
        {
            try
            {
                db db = new db();
                string query = $"SELECT WarehouseName FROM Warehouse WHERE WarehouseID = '{warehouseID}'";
                using (SqlDataReader reader = db.Select(query))
                {
                    if (reader.Read())
                    {
                        return reader["WarehouseName"].ToString(); // Returns the WarehouseName
                    }
                }
                return string.Empty; // Return empty string if no record is found
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching WarehouseName: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return string.Empty; // Return empty string if an error occurs
            }
        }


        // GRID
        private DataTable GetProductList()
        {
            string query = "SELECT ProductID, ProductName FROM Product";
            return new db().SelectDataTable(query);
        }
        private DataTable GetWarehouseList()
        {
            string query = "SELECT WarehouseID, WarehouseName FROM Warehouse";
            return new db().SelectDataTable(query);
        }
        private DataTable GetEmployeeList()
        {
            string query = "SELECT EmployeeID, LastName FROM Employee";
            return new db().SelectDataTable(query);
        }
        private void InitializeStockGrid()
        {
            dataGridView1.Columns.Clear();

            // StockID Column (hidden)
            var stockIDColumn = new DataGridViewTextBoxColumn
            {
                Name = "StockID",
                HeaderText = "Stock ID"
            };
            dataGridView1.Columns.Add(stockIDColumn);

            // ProductName ComboBox Column
            var productColumn = new DataGridViewComboBoxColumn
            {
                Name = "ProductName",
                HeaderText = "Product",
                DataSource = GetProductList(), // Fetch products from DB
                DisplayMember = "ProductName", // Display value
                ValueMember = "ProductID"      // Internal value
            };
            dataGridView1.Columns.Add(productColumn);

            // WarehouseName ComboBox Column
            var warehouseColumn = new DataGridViewComboBoxColumn
            {
                Name = "WarehouseName",
                HeaderText = "Warehouse",
                DataSource = GetWarehouseList(), // Fetch warehouses from DB
                DisplayMember = "WarehouseName",
                ValueMember = "WarehouseID"
            };
            dataGridView1.Columns.Add(warehouseColumn);

            // QuantityAvailable Column
            var quantityColumn = new DataGridViewTextBoxColumn
            {
                Name = "QuantityAvailable",
                HeaderText = "Available Quantity"
            };
            dataGridView1.Columns.Add(quantityColumn);

            // LastRestockedDate Column
            var restockDateColumn = new DataGridViewTextBoxColumn
            {
                Name = "LastRestockedDate",
                HeaderText = "Last Restock Date"
            };
            dataGridView1.Columns.Add(restockDateColumn);
        }
        private void viewdetails()
        {
            string query = @"
            SELECT 
                s.StockID, 
                s.ProductID, 
                p.ProductName, 
                s.WarehouseID, 
                w.WarehouseName, 
                s.QuantityAvailable, 
                s.LastRestockedDate
            FROM 
                Stock s
            LEFT JOIN 
                Product p ON s.ProductID = p.ProductID
            LEFT JOIN 
                Warehouse w ON s.WarehouseID = w.WarehouseID";

            var reader = new db().Select(query);
            dataGridView1.Rows.Clear();

            while (reader.Read())
            {
                dataGridView1.Rows.Add(
                    reader["StockID"],
                    reader["ProductName"],
                    reader["WarehouseName"],
                    reader["QuantityAvailable"],
                    reader["LastRestockedDate"]
                );
            }
        }
        private void InitializeWarehouseGrid()
        {
            dataGridView2.Columns.Clear();

            // WarehouseID Column
            var warehouseIDColumn = new DataGridViewTextBoxColumn
            {
                Name = "WarehouseID",
                HeaderText = "Warehouse ID"
            };
            dataGridView2.Columns.Add(warehouseIDColumn);

            // WarehouseName Column
            var warehouseNameColumn = new DataGridViewTextBoxColumn
            {
                Name = "WarehouseName",
                HeaderText = "Warehouse Name"
            };
            dataGridView2.Columns.Add(warehouseNameColumn);

            // Location Column
            var locationColumn = new DataGridViewTextBoxColumn
            {
                Name = "Location",
                HeaderText = "Location"
            };
            dataGridView2.Columns.Add(locationColumn);

            // Manager ComboBox Column
            var managerColumn = new DataGridViewComboBoxColumn
            {
                Name = "Manager",
                HeaderText = "Manager",
                DataSource = GetEmployeeList(), // Fetch employees from DB
                DisplayMember = "LastName",
                ValueMember = "EmployeeID"
            };
            dataGridView2.Columns.Add(managerColumn);
        }
        private void ViewWarehouseDetails()
        {
            string query = @"
                    SELECT 
                        w.WarehouseID,
                        w.WarehouseName,
                        w.Location,
                        e.LastName AS Manager
                    FROM 
                        Warehouse w
                    LEFT JOIN 
                        Employee e ON w.ManagerID = e.EmployeeID";

            var reader = new db().Select(query);
            dataGridView2.Rows.Clear();

            while (reader.Read())
            {
                dataGridView2.Rows.Add(
                    reader["WarehouseID"],
                    reader["WarehouseName"],
                    reader["Location"],
                    reader["Manager"]
                );
            }
        }
        private void ClearFields()
        {
            pname.Text = "";
            wname.Text = "";
            qty.Text = "";
            ldate.Value = DateTime.Now;
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
            string ProductName = pname.Text.Trim();
            string WarehouseName = wname.Text.Trim();
            string QuantityAvailable = qty.Text.Trim();
            string LastRestockedDate = ldate.Value.ToString("yyyy-MM-dd");

            string ProductID = GetProductID(ProductName);
            string WarehouseID = GetWarehouseID(WarehouseName);

            // Initialize database connection
            db DB = new db();

            string query = $@"INSERT INTO Stock (ProductID, WarehouseID, QuantityAvailable, LastRestockedDate) 
            VALUES ('{ProductID}','{WarehouseID}','{QuantityAvailable}','{LastRestockedDate}')";

            try
            {
                // Execute SQL query
                DB.Execute(query);
                MessageBox.Show("Data inserted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                viewdetails();
                loadIDs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            ClearFields();
        }
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            // Validate input fields
            string StockID = SID.Text.Trim();
            string ProductName = pname.Text.Trim();
            string WarehouseName = wname.Text.Trim();
            string QuantityAvailable = qty.Text.Trim();
            string LastRestockedDate = ldate.Value.ToString("yyyy-MM-dd");

            string ProductID = GetProductID(ProductName);
            string WarehouseID = GetWarehouseID(WarehouseName);

            // Check if StockID is a valid integer
            if (!int.TryParse(StockID, out _))
            {
                MessageBox.Show("Please select a valid StockID to make Updates.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Initialize database connection
            db DB = new db();

            string query = $@"UPDATE Stock 
                      SET ProductID='{ProductID}', 
                          WarehouseID='{WarehouseID}', 
                          QuantityAvailable='{QuantityAvailable}',
                          LastRestockedDate='{LastRestockedDate}' 
                      WHERE StockID='{StockID}'";

            try
            {
                // Execute SQL query
                DB.Execute(query);
                MessageBox.Show("Data updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
                viewdetails();
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
            string StockID = SID.Text.Trim();

            // Check if StockID is a valid integer
            if (!int.TryParse(StockID, out int parsedStockID))
            {
                MessageBox.Show("Please select a valid StockID to delete.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Show confirmation message
            DialogResult result = MessageBox.Show(
                $"Are you sure you want to delete the StockID Row {parsedStockID}?",
                "Confirm Deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            // If the user confirms, proceed with deletion
            if (result == DialogResult.Yes)
            {
                // Initialize database connection
                db DB = new db();
                string query = $@"DELETE FROM Stock WHERE StockID='{parsedStockID}'";

                try
                {
                    // Execute SQL query
                    DB.Execute(query);
                    MessageBox.Show("Data deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearFields();
                    viewdetails();
                    loadIDs();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void SID_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (sender is ComboBox comboBox) // Ensure sender is a ComboBox
            {
                string newStockID = comboBox.SelectedValue?.ToString(); // Get the actual StockID value
                try
                {
                    db db = new db();

                    string query = $@"SELECT ProductID, WarehouseID, QuantityAvailable, LastRestockedDate 
                              FROM Stock WHERE StockID='{newStockID}'";

                    using (SqlDataReader reader = db.Select(query))
                    {
                        if (reader.Read()) // Check if data exists
                        {
                            string ProductID = reader["ProductID"].ToString();
                            string WarehouseID = reader["WarehouseID"].ToString();

                            pname.Text = GetProductName(ProductID);
                            wname.Text = GetWarehouseName(WarehouseID);
                            qty.Text = reader["QuantityAvailable"].ToString();
                            ldate.Text = reader["LastRestockedDate"].ToString();
                        }
                        else
                        {
                            // Handle case where no data is found
                            MessageBox.Show("No stock found with the specified StockID.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to load Stock data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void btnStocks_Click(object sender, EventArgs e)
        {
            // Show grpEmployees and hide grpRoles
            grpStocks.Visible = true;
            grpWarehouses.Visible = false;

            // Apply "clicked" style to btnEmployees
            SetButtonStyle(btnStocks, true);
            SetButtonStyle(btnWarehouses, false);
        }
        private void btnWarehouses_Click(object sender, EventArgs e)
        {
            grpWarehouses.Visible = true;
            grpStocks.Visible = false;
            ViewWarehouseDetails();

            SetButtonStyle(btnWarehouses, true);
            SetButtonStyle(btnStocks, false);
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

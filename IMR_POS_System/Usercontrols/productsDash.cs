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
    public partial class productsDash : UserControl
    {

        // INITIALIZE & LOAD
        public productsDash()
        {
            InitializeComponent();
            InitializeGrid();
            InitializeRoleGrid();
            InitializeCategoryGrid();
        }
        private void products_Load(object sender, EventArgs e)
        {
            btnProducts.PerformClick(); // Default state
            loadIDs();
            LoadData();
            viewdetails();
            CID.SelectedIndex = 0;  // Select the first item by default (index 0)
            BID.SelectedIndex = 0;  // Select the first item by default (index 0)
        }
        private void loadIDs()
        {
            try
            {
                db db = new db();
                // Load ProductIDs into PID combo box
                using (SqlDataReader reader = db.Select("SELECT ProductID FROM Product"))
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("ProductID", typeof(int)); // Ensure consistent data type
                    dt.Columns.Add("DisplayText", typeof(string)); // For user-friendly display

                    // Add "New" placeholder row
                    DataRow newRow = dt.NewRow();
                    newRow["ProductID"] = -1; // Reserved value for "new item"
                    newRow["DisplayText"] = "New"; // What the user sees
                    dt.Rows.Add(newRow);

                    // Add actual ProductIDs
                    while (reader.Read())
                    {
                        DataRow row = dt.NewRow();
                        row["ProductID"] = reader.GetInt32(0); // Assuming ProductID is an int in your database
                        row["DisplayText"] = reader.GetInt32(0).ToString(); // Display as string
                        dt.Rows.Add(row);
                    }

                    // Bind to the ComboBox
                    PID.DisplayMember = "DisplayText"; // Text shown to the user
                    PID.ValueMember = "ProductID";    // Actual value (int) used internally
                    PID.DataSource = dt;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load Employee data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadData()
        {
            try
            {
                db db = new db();

                // Load Category IDs into cname ComboBox
                DataTable categoryTable = new DataTable();
                categoryTable.Columns.Add("CategoryID", typeof(int));
                categoryTable.Columns.Add("CategoryName", typeof(string));

                // Add a default empty row
                DataRow emptyCategoryRow = categoryTable.NewRow();
                emptyCategoryRow["CategoryID"] = DBNull.Value; // No value selected
                emptyCategoryRow["CategoryName"] = ""; // Display as empty
                categoryTable.Rows.Add(emptyCategoryRow);

                using (SqlDataReader reader = db.Select("SELECT CategoryID, CategoryName FROM Category"))
                {
                    while (reader.Read())
                    {
                        DataRow row = categoryTable.NewRow();
                        row["CategoryID"] = reader.GetInt32(0); // Assuming CategoryID is int
                        row["CategoryName"] = reader.GetString(1); // CategoryName as string
                        categoryTable.Rows.Add(row);
                    }
                }
                cname.DisplayMember = "CategoryName";
                cname.ValueMember = "CategoryID";
                cname.DataSource = categoryTable;

                // Load Brand IDs into bname ComboBox
                DataTable brandTable = new DataTable();
                brandTable.Columns.Add("BrandID", typeof(int));
                brandTable.Columns.Add("BrandName", typeof(string));

                // Add a default empty row
                DataRow emptyBrandRow = brandTable.NewRow();
                emptyBrandRow["BrandID"] = DBNull.Value; // No value selected
                emptyBrandRow["BrandName"] = ""; // Display as empty
                brandTable.Rows.Add(emptyBrandRow);

                using (SqlDataReader reader = db.Select("SELECT BrandID, BrandName FROM Brand"))
                {
                    while (reader.Read())
                    {
                        DataRow row = brandTable.NewRow();
                        row["BrandID"] = reader.GetInt32(0);
                        row["BrandName"] = reader.GetString(1);
                        brandTable.Rows.Add(row);
                    }
                }
                bname.DisplayMember = "BrandName";
                bname.ValueMember = "BrandID";
                bname.DataSource = brandTable;

                // Load Tax Rates into TID ComboBox
                DataTable taxRateTable = new DataTable();
                taxRateTable.Columns.Add("TaxRateID", typeof(int));
                taxRateTable.Columns.Add("TaxRate", typeof(string));

                // Add a default empty row
                DataRow emptyTaxRow = taxRateTable.NewRow();
                emptyTaxRow["TaxRateID"] = DBNull.Value; // No value selected
                emptyTaxRow["TaxRate"] = ""; // Display as empty
                taxRateTable.Rows.Add(emptyTaxRow);

                using (SqlDataReader reader = db.Select("SELECT TaxRateID, TaxRate FROM TaxRate"))
                {
                    while (reader.Read())
                    {
                        DataRow row = taxRateTable.NewRow();
                        row["TaxRateID"] = reader.GetInt32(0); // Assuming TaxRateID is int
                        row["TaxRate"] = reader.GetDecimal(1).ToString("F2"); // Format as 8.00
                        taxRateTable.Rows.Add(row);
                    }
                }
                txrate.DisplayMember = "TaxRate";
                txrate.ValueMember = "TaxRateID";
                txrate.DataSource = taxRateTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private string GetCategoryID(string categoryName)
        {
            try
            {
                string query = "SELECT CategoryID FROM Category WHERE CategoryName = @CategoryName";
                db db = new db();
                SqlParameter[] parameters = {
            new SqlParameter("@CategoryName", categoryName)
        };
                return db.ExecuteScalar(query, parameters).ToString(); // Returns the CategoryID
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching CategoryID: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "0"; // Return -1 if an error occurs
            }
        }
        private string GetBrandID(string brandName)
        {
            try
            {
                string query = "SELECT BrandID FROM Brand WHERE BrandName = @BrandName";
                db db = new db();
                SqlParameter[] parameters = {
            new SqlParameter("@BrandName", brandName)
        };
                return db.ExecuteScalar(query, parameters).ToString(); // Returns the BrandID
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching BrandID: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "0"; // Return -1 if an error occurs
            }
        }
        private string GetTaxRateID(string taxRate)
        {
            try
            {
                string query = "SELECT TaxRateID FROM TaxRate WHERE TaxRate = @TaxRate";
                db db = new db();
                SqlParameter[] parameters = {
            new SqlParameter("@TaxRate", taxRate)
        };
                return db.ExecuteScalar(query, parameters).ToString(); // Returns the TaxRateID
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching TaxRateID: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "0"; // Return -1 if an error occurs
            }
        }
        private string GetCategoryName(string categoryID)
        {
            try
            {
                db db = new db();
                string query = $"SELECT CategoryName FROM Category WHERE CategoryID = '{categoryID}'";
                using (SqlDataReader reader = db.Select(query))
                {
                    if (reader.Read())
                    {
                        return reader["CategoryName"].ToString();
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching CategoryName: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return string.Empty;
            }
        }
        private string GetBrandName(string brandID)
        {
            try
            {
                db db = new db();
                string query = $"SELECT BrandName FROM Brand WHERE BrandID = '{brandID}'";
                using (SqlDataReader reader = db.Select(query))
                {
                    if (reader.Read())
                    {
                        return reader["BrandName"].ToString();
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching BrandName: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return string.Empty;
            }
        }
        private string GetTaxRateText(string taxRateID)
        {
            try
            {
                db db = new db();
                string query = $"SELECT TaxRate FROM TaxRate WHERE TaxRateID = '{taxRateID}'";
                using (SqlDataReader reader = db.Select(query))
                {
                    if (reader.Read())
                    {
                        return reader["TaxRate"].ToString();
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching TaxRate: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return string.Empty;
            }
        }


        // GRID
        private DataTable GetCategoryList()
        {
            string query = "SELECT CategoryID, CategoryName FROM Category";
            return new db().SelectDataTable(query);
        }
        private DataTable GetBrandList()
        {
            string query = "SELECT BrandID, BrandName FROM Brand";
            return new db().SelectDataTable(query);
        }
        private DataTable GetTaxRateList()
        {
            string query = "SELECT TaxRateID, TaxRate FROM TaxRate";
            return new db().SelectDataTable(query);
        }
        private void InitializeGrid()
        {
            dataGridView1.Columns.Clear();

            // ProductID Column (hidden)
            var productIDColumn = new DataGridViewTextBoxColumn
            {
                Name = "ProductID",
                HeaderText = "Product ID"
            };
            dataGridView1.Columns.Add(productIDColumn);

            // ProductName Column
            var productNameColumn = new DataGridViewTextBoxColumn
            {
                Name = "ProductName",
                HeaderText = "Product Name"
            };
            dataGridView1.Columns.Add(productNameColumn);

            // CategoryName ComboBox Column
            var categoryColumn = new DataGridViewComboBoxColumn
            {
                Name = "CategoryName",
                HeaderText = "Category",
                DataSource = GetCategoryList(), // Fetch categories from DB
                DisplayMember = "CategoryName", // Display value
                ValueMember = "CategoryID",    // Internal value
            };
            dataGridView1.Columns.Add(categoryColumn);

            // BrandName ComboBox Column
            var brandColumn = new DataGridViewComboBoxColumn
            {
                Name = "BrandName",
                HeaderText = "Brand",
                DataSource = GetBrandList(), // Fetch brands from DB
                DisplayMember = "BrandName",
                ValueMember = "BrandID",
            };
            dataGridView1.Columns.Add(brandColumn);

            // UnitPrice Column
            dataGridView1.Columns.Add("UnitPrice", "Unit Price");

            // TaxRate ComboBox Column
            var taxRateColumn = new DataGridViewComboBoxColumn
            {
                Name = "TaxRate",
                HeaderText = "Tax Rate",
                DataSource = GetTaxRateList(), // Fetch tax rates from DB
                DisplayMember = "TaxRate",
                ValueMember = "TaxRateID",
            };
            dataGridView1.Columns.Add(taxRateColumn);

            // ReorderLevel Column
            dataGridView1.Columns.Add("ReorderLevel", "Reorder Level");

            // IsActive Checkbox Column
            var isActiveColumn = new DataGridViewCheckBoxColumn
            {
                Name = "IsActive",
                HeaderText = "Active Status"
            };
            dataGridView1.Columns.Add(isActiveColumn);
        }
        private void viewdetails()
        {
            // SQL query to join related tables for meaningful data display
            string query = @"
            SELECT 
                p.ProductID, 
                p.ProductName, 
                c.CategoryName, 
                b.BrandName, 
                p.UnitPrice, 
                t.TaxRate, 
                p.ReorderLevel, 
                p.IsActive 
            FROM 
                Product p
            LEFT JOIN 
                Category c ON p.CategoryID = c.CategoryID
            LEFT JOIN 
                Brand b ON p.BrandID = b.BrandID
            LEFT JOIN 
                TaxRate t ON p.TaxRateID = t.TaxRateID";

            var reader = new db().Select(query);
            dataGridView1.Rows.Clear();

            while (reader.Read())
            {
                // Add rows to DataGridView with meaningful data
                dataGridView1.Rows.Add(
                    reader["ProductID"],
                    reader["ProductName"],
                    reader["CategoryName"],
                    reader["BrandName"],
                    reader["UnitPrice"],
                    reader["TaxRate"],
                    reader["ReorderLevel"],
                    Convert.ToBoolean(reader["IsActive"]) // Pass boolean directly
                );
            }
        }
        private void InitializeRoleGrid()
        {
            dataGridView2.Columns.Clear();

            // BrandID Column
            var brandIDColumn = new DataGridViewTextBoxColumn
            {
                Name = "BrandID",
                HeaderText = "Brand ID"
            };
            dataGridView2.Columns.Add(brandIDColumn);

            // BrandName Column
            var brandNameColumn = new DataGridViewTextBoxColumn
            {
                Name = "BrandName",
                HeaderText = "Brand Name"
            };
            dataGridView2.Columns.Add(brandNameColumn);
        }
        private void ViewBrandDetails()
        {
            string query = @"
                    SELECT 
                        BrandID,
                        BrandName
                    FROM 
                        Brand";

            var reader = new db().Select(query);
            dataGridView2.Rows.Clear();

            while (reader.Read())
            {
                dataGridView2.Rows.Add(
                    reader["BrandID"],
                    reader["BrandName"]
                );
            }
        }
        private void InitializeCategoryGrid()
        {
            dataGridView3.Columns.Clear();

            // CategoryID Column
            var categoryIDColumn = new DataGridViewTextBoxColumn
            {
                Name = "CategoryID",
                HeaderText = "Category ID"
            };
            dataGridView3.Columns.Add(categoryIDColumn);

            // CategoryName Column
            var categoryNameColumn = new DataGridViewTextBoxColumn
            {
                Name = "CategoryName",
                HeaderText = "Category Name"
            };
            dataGridView3.Columns.Add(categoryNameColumn);
        }
        private void ViewCategoryDetails()
        {
            string query = @"
                    SELECT 
                        CategoryID,
                        CategoryName
                    FROM 
                        Category";

            var reader = new db().Select(query);
            dataGridView3.Rows.Clear();

            while (reader.Read())
            {
                dataGridView3.Rows.Add(
                    reader["CategoryID"],
                    reader["CategoryName"]
                );
            }
        }
        private void ClearFields()
        {
            pname.Text = "";
            cname.Text = "";
            bname.Text = "";
            uprice.Text = "";
            txrate.Text = "";
            rlevel.Text = "";
            status.Text = "";
        }
        private void dataGridView1_DataError_1(object sender, DataGridViewDataErrorEventArgs e)
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
            string ProductName = pname.Text.Trim();
            string CategoryName = cname.Text.Trim();
            string BrandName = bname.Text.Trim();
            string UnitPrice = uprice.Text.Trim();
            string TaxRate = txrate.Text.Trim();
            string ReorderLevel = rlevel.Text.Trim();
            string IsActive = status.Text.Trim();

            string CategoryID = GetCategoryID(CategoryName);
            string BrandID = GetBrandID(BrandName);
            string TaxRateID = GetTaxRateID(TaxRate);

            if (IsActive == "Active")
            {
                IsActive = "1";
            }
            else
            {
                IsActive = "0";
            }

            // Initialize database connection
            db DB = new db();

            string query = $@"INSERT INTO Product (ProductName, CategoryID , BrandID ,UnitPrice ,TaxRateID , ReorderLevel ,IsActive) 
            VALUES ('{ProductName}' , '{CategoryID}' ,'{BrandID}' , '{UnitPrice}' , '{TaxRateID}' , '{ReorderLevel}' ,'{IsActive}')";


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
        }
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            // Validate input fields
            string ProductID = PID.Text.Trim();
            string ProductName = pname.Text.Trim();
            string CategoryName = cname.Text.Trim();
            string BrandName = bname.Text.Trim();
            string UnitPrice = uprice.Text.Trim();
            string TaxRate = txrate.Text.Trim();
            string ReorderLevel = rlevel.Text.Trim();
            string IsActive = status.Text.Trim();

            string CategoryID = GetCategoryID(CategoryName);
            string BrandID = GetBrandID(BrandName);
            string TaxRateID = GetTaxRateID(TaxRate);

            // Check if ProductID is a valid integer
            if (!int.TryParse(ProductID, out _))
            {
                MessageBox.Show("Please select a valid ProductID to make Updates.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Convert IsActive to 1 or 0
            IsActive = (IsActive == "Active") ? "1" : "0";

            // Initialize database connection
            db DB = new db();

            string query = $@"UPDATE Product 
                      SET ProductName='{ProductName}', 
                          CategoryID='{CategoryID}', 
                          BrandID='{BrandID}', 
                          UnitPrice='{UnitPrice}', 
                          TaxRateID='{TaxRateID}', 
                          ReorderLevel='{ReorderLevel}', 
                          IsActive='{IsActive}' 
                      WHERE ProductID='{ProductID}'";

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
            string ProductID = PID.Text.Trim();

            // Check if ProductID is a valid integer
            if (!int.TryParse(ProductID, out int parsedProductID))
            {
                MessageBox.Show("Please select a valid ProductID to delete.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Show confirmation message
            DialogResult result = MessageBox.Show(
                $"Are you sure you want to delete the ProductID Row {parsedProductID}?",
                "Confirm Deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            // If the user confirms, proceed with deletion
            if (result == DialogResult.Yes)
            {
                // Initialize database connection
                db DB = new db();
                string query = $@"DELETE FROM Product WHERE ProductID='{parsedProductID}'";

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
        private void PID_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (sender is ComboBox comboBox) // Ensure sender is a ComboBox
            {
                string newProductID = comboBox.SelectedValue?.ToString(); // Get the actual ProductID value
                try
                {
                    db db = new db();

                    string query = $@"SELECT ProductName, UnitPrice, ReorderLevel, IsActive, CategoryID, BrandID, TaxRateID 
                              FROM Product WHERE ProductID='{newProductID}'";

                    using (SqlDataReader reader = db.Select(query))
                    {
                        if (reader.Read()) // Check if data exists
                        {

                            string CategoryID = reader["CategoryID"].ToString();
                            string BrandID = reader["BrandID"].ToString();
                            string TaxRateID = reader["TaxRateID"].ToString();

                            pname.Text = reader["ProductName"].ToString();
                            uprice.Text = reader["UnitPrice"].ToString();
                            rlevel.Text = reader["ReorderLevel"].ToString();
                            status.Text = (Convert.ToBoolean(reader["IsActive"])) ? "Active" : "Inactive";
                            cname.Text = GetCategoryName(CategoryID);
                            bname.Text = GetBrandName(BrandID);
                            txrate.Text = GetTaxRateText(TaxRateID);
                        }
                        else
                        {
                            // Handle case where no data is found
                            MessageBox.Show("No product found with the specified ProductID.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to load Product data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnProducts_Click(object sender, EventArgs e)
        {
            grpProducts.Visible = true;
            grpBrands.Visible = false;
            grpCategories.Visible = false;

            SetButtonStyle(btnProducts, true);
            SetButtonStyle(btnBrands, false);
            SetButtonStyle(btnCategories, false);
        }
        private void btnBrands_Click(object sender, EventArgs e)
        {
            grpBrands.Visible = true;
            grpProducts.Visible = false;
            grpCategories.Visible = false;
            ViewBrandDetails();

            SetButtonStyle(btnBrands, true);
            SetButtonStyle(btnProducts, false);
            SetButtonStyle(btnCategories, false);
        }
        private void btnCategories_Click(object sender, EventArgs e)
        {
            grpCategories.Visible = true;
            grpProducts.Visible = false;
            grpBrands.Visible = false;
            ViewCategoryDetails();

            SetButtonStyle(btnCategories, true);
            SetButtonStyle(btnProducts, false);
            SetButtonStyle(btnBrands, false);
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

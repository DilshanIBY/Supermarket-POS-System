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
    public partial class discountsDash : UserControl
    {

        // INITIALIZE & LOAD
        public discountsDash()
        {
            InitializeComponent();
            InitializeDiscountGrid();
            InitializeProductDiscountGrid();
            InitializeTaxRateGrid();
        }
        private void discount_Load(object sender, EventArgs e)
        {
            btnDiscounts.PerformClick(); // Default state
            loadDiscountIDs();
            ViewDiscountDetails();
            DisID.SelectedIndex = 0;  // Select the first item by default (index 0)
            TxID.SelectedIndex = 0;  // Select the first item by default (index 0)
        }
        private void loadDiscountIDs()
        {
            try
            {
                db db = new db();

                // Load DiscountIDs into a combo box
                using (SqlDataReader reader = db.Select("SELECT DiscountID FROM Discount"))
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("DiscountID", typeof(int)); // Ensure consistent data type
                    dt.Columns.Add("DisplayText", typeof(string)); // For user-friendly display

                    // Add "New" placeholder row
                    DataRow newRow = dt.NewRow();
                    newRow["DiscountID"] = -1; // Reserved value for "new item"
                    newRow["DisplayText"] = "New"; // What the user sees
                    dt.Rows.Add(newRow);

                    // Add actual DiscountIDs and DiscountNames
                    while (reader.Read())
                    {
                        DataRow row = dt.NewRow();
                        row["DiscountID"] = reader.GetInt32(0); // Assuming DiscountID is int in your database
                        row["DisplayText"] = reader.GetInt32(0).ToString(); // Display as string
                        dt.Rows.Add(row);
                    }

                    // Bind to the ComboBox
                    DID.DisplayMember = "DisplayText"; // Text shown to the user
                    DID.ValueMember = "DiscountID";    // Actual value (int) used internally
                    DID.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load Discount data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        // GRID
        private DataTable GetProductList()
        {
            string query = "SELECT ProductID, ProductName FROM Product";
            return new db().SelectDataTable(query);
        }
        private DataTable GetDiscountList()
        {
            string query = "SELECT DiscountID, DiscountName FROM Discount";
            return new db().SelectDataTable(query);
        }
        private void InitializeDiscountGrid()
        {
            dataGridView1.Columns.Clear();

            // DiscountID Column (hidden)
            var discountIDColumn = new DataGridViewTextBoxColumn
            {
                Name = "DiscountID",
                HeaderText = "Discount ID",
            };
            dataGridView1.Columns.Add(discountIDColumn);

            // DiscountName Column
            var discountNameColumn = new DataGridViewTextBoxColumn
            {
                Name = "DiscountName",
                HeaderText = "Discount Name"
            };
            dataGridView1.Columns.Add(discountNameColumn);

            // StartDate Column
            var startDateColumn = new DataGridViewTextBoxColumn
            {
                Name = "StartDate",
                HeaderText = "Start Date"
            };
            dataGridView1.Columns.Add(startDateColumn);

            // EndDate Column
            var endDateColumn = new DataGridViewTextBoxColumn
            {
                Name = "EndDate",
                HeaderText = "End Date"
            };
            dataGridView1.Columns.Add(endDateColumn);

            // DiscountPercentage Column
            var discountPercentageColumn = new DataGridViewTextBoxColumn
            {
                Name = "DiscountPercentage",
                HeaderText = "Discount Percentage"
            };
            dataGridView1.Columns.Add(discountPercentageColumn);

            // DiscountType Column
            var discountTypeColumn = new DataGridViewTextBoxColumn
            {
                Name = "DiscountType",
                HeaderText = "Discount Type"
            };
            dataGridView1.Columns.Add(discountTypeColumn);
        }
        private void ViewDiscountDetails()
        {
            string query = @"
                    SELECT 
                        DiscountID,
                        DiscountName,
                        StartDate,
                        EndDate,
                        DiscountPercentage,
                        DiscountType
                    FROM 
                        Discount";

            var reader = new db().Select(query);
            dataGridView1.Rows.Clear();

            while (reader.Read())
            {
                dataGridView1.Rows.Add(
                    reader["DiscountID"],
                    reader["DiscountName"],
                    Convert.ToDateTime(reader["StartDate"]).ToString("yyyy-MM-dd"), // Format date
                    Convert.ToDateTime(reader["EndDate"]).ToString("yyyy-MM-dd"),   // Format date
                    reader["DiscountPercentage"],
                    reader["DiscountType"]
                );
            }
        }
        private void InitializeProductDiscountGrid()
        {
            dataGridView3.Columns.Clear();

            // ProductDiscountID Column (hidden)
            var productDiscountIDColumn = new DataGridViewTextBoxColumn
            {
                Name = "ProductDiscountID",
                HeaderText = "Product Discount ID",
            };
            dataGridView3.Columns.Add(productDiscountIDColumn);

            // Product ComboBox Column
            var productColumn = new DataGridViewComboBoxColumn
            {
                Name = "Product",
                HeaderText = "Product",
                DataSource = GetProductList(), // Fetch products from DB
                DisplayMember = "ProductName",
                ValueMember = "ProductID"
            };
            dataGridView3.Columns.Add(productColumn);

            // Discount ComboBox Column
            var discountColumn = new DataGridViewComboBoxColumn
            {
                Name = "Discount",
                HeaderText = "Discount",
                DataSource = GetDiscountList(), // Fetch discounts from DB
                DisplayMember = "DiscountName",
                ValueMember = "DiscountID"
            };
            dataGridView3.Columns.Add(discountColumn);
        }
        private void ViewProductDiscountDetails()
        {
            string query = @"
                    SELECT 
                        pd.ProductDiscountID,
                        p.ProductName,
                        d.DiscountName,
                        pd.ProductID,
                        pd.DiscountID
                    FROM 
                        ProductDiscount pd
                    LEFT JOIN 
                        Product p ON pd.ProductID = p.ProductID
                    LEFT JOIN 
                        Discount d ON pd.DiscountID = d.DiscountID";

            var reader = new db().Select(query);
            dataGridView3.Rows.Clear();

            while (reader.Read())
            {
                dataGridView3.Rows.Add(
                    reader["ProductDiscountID"],
                    reader["ProductName"],
                    reader["DiscountName"]
                );
            }
        }
        private void InitializeTaxRateGrid()
        {
            dataGridView2.Columns.Clear();

            // TaxRateID Column (hidden)
            var taxRateIDColumn = new DataGridViewTextBoxColumn
            {
                Name = "TaxRateID",
                HeaderText = "Tax Rate ID",
            };
            dataGridView2.Columns.Add(taxRateIDColumn);

            // TaxRate Column
            var taxRateColumn = new DataGridViewTextBoxColumn
            {
                Name = "TaxRate",
                HeaderText = "Tax Rate"
            };
            dataGridView2.Columns.Add(taxRateColumn);
        }
        private void ViewTaxRateDetails()
        {
            string query = @"
                    SELECT 
                        TaxRateID,
                        TaxRate
                    FROM 
                        TaxRate";

            var reader = new db().Select(query);
            dataGridView2.Rows.Clear();

            while (reader.Read())
            {
                dataGridView2.Rows.Add(
                    reader["TaxRateID"],
                    reader["TaxRate"]
                );
            }
        }
        private void ClearDiscountFields()
        {
            txtdiscountName.Text = "";
            txtstartDate.Text = "";
            txtendDate.Text = "";
            txtdiscountPercentage.Text = "";
            cmbdiscountType.Text = ""; 
        }
        private void dataGridView1_DataError_1(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Context == DataGridViewDataErrorContexts.Commit)
            {
                MessageBox.Show("Invalid value. Please select from the dropdown list.");
                e.Cancel = true;
            }
        }
        private void dataGridView3_DataError(object sender, DataGridViewDataErrorEventArgs e)
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
            string DiscountName = txtdiscountName.Text.Trim();
            string StartDate = txtstartDate.Value.ToString("yyyy-MM-dd");
            string EndDate = txtendDate.Value.ToString("yyyy-MM-dd");
            string DiscountPercentage = txtdiscountPercentage.Text.Trim();
            string DiscountType = cmbdiscountType.Text.Trim();

            // Initialize database connection
            db DB = new db();

            string query = $@"INSERT INTO Discount (DiscountName, StartDate, EndDate, DiscountPercentage, DiscountType) 
                      VALUES ('{DiscountName}', '{StartDate}', '{EndDate}', '{DiscountPercentage}', '{DiscountType}')";

            try
            {
                // Execute SQL query
                DB.Execute(query);
                MessageBox.Show("Discount inserted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearDiscountFields();
                ViewDiscountDetails();
                loadDiscountIDs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            // Validate input fields
            string DiscountID = DID.Text.Trim();
            string DiscountName = txtdiscountName.Text.Trim();
            string StartDate = txtstartDate.Value.ToString("yyyy-MM-dd");
            string EndDate = txtendDate.Value.ToString("yyyy-MM-dd");
            string DiscountPercentage = txtdiscountPercentage.Text.Trim();
            string DiscountType = cmbdiscountType.Text.Trim();

            // Check if DiscountID is valid
            if (!int.TryParse(DiscountID, out _))
            {
                MessageBox.Show("Please select a valid DiscountID to update.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Initialize database connection
            db DB = new db();

            string query = $@"UPDATE Discount 
                      SET DiscountName='{DiscountName}', 
                          StartDate='{StartDate}', 
                          EndDate='{EndDate}', 
                          DiscountPercentage='{DiscountPercentage}', 
                          DiscountType='{DiscountType}' 
                      WHERE DiscountID='{DiscountID}'";

            try
            {
                // Execute SQL query
                DB.Execute(query);
                MessageBox.Show("Discount updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearDiscountFields();
                ViewDiscountDetails();
                loadDiscountIDs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            // Validate input fields
            string DiscountID = DID.Text.Trim();

            // Check if DiscountID is valid
            if (!int.TryParse(DiscountID, out int parsedDiscountID))
            {
                MessageBox.Show("Please select a valid DiscountID to delete.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Show confirmation message
            DialogResult result = MessageBox.Show(
                $"Are you sure you want to delete the DiscountID Row {parsedDiscountID}?",
                "Confirm Deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            // If the user confirms, proceed with deletion
            if (result == DialogResult.Yes)
            {
                // Initialize database connection
                db DB = new db();
                string query = $@"DELETE FROM Discount WHERE DiscountID='{parsedDiscountID}'";

                try
                {
                    // Execute SQL query
                    DB.Execute(query);
                    MessageBox.Show("Discount deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearDiscountFields();
                    ViewDiscountDetails();
                    loadDiscountIDs();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void DID_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (sender is ComboBox comboBox) // Ensure sender is a ComboBox
            {
                string newDiscountID = comboBox.SelectedValue?.ToString(); // Get the actual DiscountID value
                try
                {
                    db db = new db();

                    string query = $@"SELECT DiscountName, StartDate, EndDate, DiscountPercentage, DiscountType 
                              FROM Discount WHERE DiscountID='{newDiscountID}'";

                    using (SqlDataReader reader = db.Select(query))
                    {
                        if (reader.Read()) // Check if data exists
                        {
                            txtdiscountName.Text = reader["DiscountName"].ToString();
                            txtstartDate.Text = reader["StartDate"].ToString();
                            txtendDate.Text = reader["EndDate"].ToString();
                            txtdiscountPercentage.Text = reader["DiscountPercentage"].ToString();
                            cmbdiscountType.Text = reader["DiscountType"].ToString();
                        }
                        else
                        {
                            // Handle case where no data is found
                            MessageBox.Show("No discount found with the specified DiscountID.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to load Discount data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void btnDiscounts_Click(object sender, EventArgs e)
        {
            grpDiscounts.Visible = true;
            grpTaxes.Visible = false;
            grpDisProducts.Visible = false;

            SetButtonStyle(btnDiscounts, true);
            SetButtonStyle(btnTaxes, false);
            SetButtonStyle(btnDisProducts, false);
        }
        private void btnTaxes_Click(object sender, EventArgs e)
        {
            grpTaxes.Visible = true;
            grpDiscounts.Visible = false;
            grpDisProducts.Visible = false;
            ViewTaxRateDetails();

            SetButtonStyle(btnTaxes, true);
            SetButtonStyle(btnDiscounts, false);
            SetButtonStyle(btnDisProducts, false);
        }
        private void btnDisProducts_Click(object sender, EventArgs e)
        {
            grpDisProducts.Visible = true;
            grpTaxes.Visible = false;
            grpDiscounts.Visible = false;
            ViewProductDiscountDetails();

            SetButtonStyle(btnDisProducts, true);
            SetButtonStyle(btnTaxes, false);
            SetButtonStyle(btnDiscounts, false);
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

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
    public partial class customersDash : UserControl
    {

        // INITIALIZE & LOAD
        public customersDash()
        {
            InitializeComponent();
            InitializeGrid();
        }
        private void customer_Load(object sender, EventArgs e)
        {
            btnCustomers.PerformClick(); // Default state
            loadIDs();
            viewdetails();
        }
        private void loadIDs()
        {
            try
            {

                db db = new db();
                // Load CustomerIDs into PID combo box
                using (SqlDataReader reader = db.Select("SELECT CustomerID FROM Customer"))
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("CustomerID", typeof(int)); // Ensure consistent data type
                    dt.Columns.Add("DisplayText", typeof(string)); // For user-friendly display

                    // Add "New" placeholder row
                    DataRow newRow = dt.NewRow();
                    newRow["CustomerID"] = -1; // Reserved value for "new item"
                    newRow["DisplayText"] = "New"; // What the user sees
                    dt.Rows.Add(newRow);

                    // Add actual CustomerIDs
                    while (reader.Read())
                    {
                        DataRow row = dt.NewRow();
                        row["CustomerID"] = reader.GetInt32(0); // Assuming CustomerID is an int in your database
                        row["DisplayText"] = reader.GetInt32(0).ToString(); // Display as string
                        dt.Rows.Add(row);
                    }

                    // Bind to the ComboBox
                    CID.DisplayMember = "DisplayText"; // Text shown to the user
                    CID.ValueMember = "CustomerID";    // Actual value (int) used internally
                    CID.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load Customer data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // GRID
        private void InitializeGrid()
        {
            dataGridView1.Columns.Clear();

            // CustomerID Column
            var customerIDColumn = new DataGridViewTextBoxColumn
            {
                Name = "CustomerID",
                HeaderText = "Customer ID"
            };
            dataGridView1.Columns.Add(customerIDColumn);

            // FirstName Column
            var firstNameColumn = new DataGridViewTextBoxColumn
            {
                Name = "FirstName",
                HeaderText = "First Name"
            };
            dataGridView1.Columns.Add(firstNameColumn);

            // LastName Column
            var lastNameColumn = new DataGridViewTextBoxColumn
            {
                Name = "LastName",
                HeaderText = "Last Name"
            };
            dataGridView1.Columns.Add(lastNameColumn);

            // Email Column
            var emailColumn = new DataGridViewTextBoxColumn
            {
                Name = "Email",
                HeaderText = "Email"
            };
            dataGridView1.Columns.Add(emailColumn);

            // PhoneNumber Column
            var phoneNumberColumn = new DataGridViewTextBoxColumn
            {
                Name = "PhoneNumber",
                HeaderText = "Phone Number"
            };
            dataGridView1.Columns.Add(phoneNumberColumn);

            // AddressLine1 Column
            var addressLine1Column = new DataGridViewTextBoxColumn
            {
                Name = "AddressLine1",
                HeaderText = "Address Line 1"
            };
            dataGridView1.Columns.Add(addressLine1Column);

            // AddressLine2 Column
            var addressLine2Column = new DataGridViewTextBoxColumn
            {
                Name = "AddressLine2",
                HeaderText = "Address Line 2"
            };
            dataGridView1.Columns.Add(addressLine2Column);

            // City Column
            var cityColumn = new DataGridViewTextBoxColumn
            {
                Name = "City",
                HeaderText = "City"
            };
            dataGridView1.Columns.Add(cityColumn);

            // PostalCode Column
            var postalCodeColumn = new DataGridViewTextBoxColumn
            {
                Name = "PostalCode",
                HeaderText = "Postal Code"
            };
            dataGridView1.Columns.Add(postalCodeColumn);

            // RegistrationDate Column
            var registrationDateColumn = new DataGridViewTextBoxColumn
            {
                Name = "RegistrationDate",
                HeaderText = "Registration Date"
            };
            dataGridView1.Columns.Add(registrationDateColumn);
        }
        private void viewdetails()
        {
            string query = @"
                    SELECT 
                        c.CustomerID,
                        c.FirstName,
                        c.LastName,
                        c.Email,
                        c.PhoneNumber,
                        c.AddressLine1,
                        c.AddressLine2,
                        c.City,
                        c.PostalCode,
                        c.RegistrationDate
                    FROM 
                        Customer c";

            var reader = new db().Select(query);
            dataGridView1.Rows.Clear();

            while (reader.Read())
            {
                dataGridView1.Rows.Add(
                    reader["CustomerID"],
                    reader["FirstName"],
                    reader["LastName"],
                    reader["Email"],
                    reader["PhoneNumber"],
                    reader["AddressLine1"],
                    reader["AddressLine2"],
                    reader["City"],
                    reader["PostalCode"],
                    reader["RegistrationDate"]
                );
            }
        }
        private void ClearFields()
        {
            fname.Text = "";
            lname.Text = "";
            emails.Text = "";
            tp.Text = "";
            adl1.Text = "";
            adl2.Text = "";
            cty.Text = "";
            pcode.Text = "";
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
            string FirstName = fname.Text.Trim();
            string LastName = lname.Text.Trim();
            string Email = emails.Text.Trim();
            string contact = tp.Text.Trim();
            string addressline1 = adl1.Text.Trim();
            string addressline2 = adl2.Text.Trim();
            string city = cty.Text.Trim();
            string postalcode = pcode.Text.Trim();

            // Check for empty fields
            if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName) || string.IsNullOrEmpty(Email) ||
                string.IsNullOrEmpty(contact) || string.IsNullOrEmpty(addressline1) || string.IsNullOrEmpty(city) ||
                string.IsNullOrEmpty(postalcode))
            {
                MessageBox.Show("All fields marked with * are required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validate email format
            if (!IsValidEmail(Email))
            {
                MessageBox.Show("Invalid email address.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validate phone number format (assuming it's numeric and has a specific length, e.g., 10 digits)
            if (!IsValidPhoneNumber(contact))
            {
                MessageBox.Show("Invalid phone number. It should be 10 digits long.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Initialize database connection
            db DB = new db();

            string query = $@"INSERT INTO Customer (FirstName, LastName , Email ,PhoneNumber ,AddressLine1 , AddressLine2 ,City , PostalCode) 
            VALUES ('{FirstName}' , '{LastName}' ,'{Email}' , '{contact}' , '{addressline1}' , '{addressline2}' ,'{city}' , '{postalcode}')";

            var reader = new db().Select($"SELECT * FROM Customer WHERE  PhoneNumber = '{contact}'");
            var result = reader.Read();
            if (result)
            {
                MessageBox.Show("Customer already exists.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

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
            string CustomerID = CID.Text.Trim();
            string FirstName = fname.Text.Trim();
            string LastName = lname.Text.Trim();
            string Email = emails.Text.Trim();
            string contact = tp.Text.Trim();
            string addressline1 = adl1.Text.Trim();
            string addressline2 = adl2.Text.Trim();
            string city = cty.Text.Trim();
            string postalcode = pcode.Text.Trim();

            // Check if CustomerID is a valid integer
            if (!int.TryParse(CustomerID, out _))
            {
                MessageBox.Show("Please select a valid CustomerID to make Updates.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Initialize database connection
            db DB = new db();

            string query = $@"UPDATE Customer 
                      SET FirstName='{FirstName}', 
                          LastName='{LastName}', 
                          Email='{Email}', 
                          PhoneNumber='{contact}', 
                          AddressLine1='{addressline1}', 
                          AddressLine2='{addressline2}', 
                          City='{city}', 
                          PostalCode='{postalcode}' 
                      WHERE CustomerID='{CustomerID}'";

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
            string CustomerID = CID.Text.Trim();

            // Check if CustomerID is a valid integer
            if (!int.TryParse(CustomerID, out int parsedCustomerID))
            {
                MessageBox.Show("Please select a valid CustomerID to delete.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Show confirmation message
            DialogResult result = MessageBox.Show(
                $"Are you sure you want to delete the CustomerID Row {parsedCustomerID}?",
                "Confirm Deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            // If the user confirms, proceed with deletion
            if (result == DialogResult.Yes)
            {
                // Initialize database connection
                db DB = new db();
                string query = $@"DELETE FROM Customer WHERE CustomerID='{parsedCustomerID}'";

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
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        private bool IsValidPhoneNumber(string phoneNumber)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^\d{10}$");
        }
        private void CID_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (sender is System.Windows.Forms.ComboBox comboBox) // Ensure sender is a ComboBox
            {
                string newCustomerID = comboBox.SelectedValue?.ToString(); // Get the actual CustomerID value
                try
                {
                    db db = new db();

                    string query = $@"SELECT FirstName, LastName, Email, PhoneNumber, AddressLine1, AddressLine2, City, PostalCode 
                              FROM Customer WHERE CustomerID='{newCustomerID}'";

                    using (SqlDataReader reader = db.Select(query))
                    {
                        if (reader.Read()) // Check if data exists
                        {
                            fname.Text = reader["FirstName"].ToString();
                            lname.Text = reader["LastName"].ToString();
                            emails.Text = reader["Email"].ToString();
                            tp.Text = reader["PhoneNumber"].ToString();
                            adl1.Text = reader["AddressLine1"].ToString();
                            adl2.Text = reader["AddressLine2"].ToString();
                            cty.Text = reader["City"].ToString();
                            pcode.Text = reader["PostalCode"].ToString();
                        }
                        else
                        {
                            // Handle case where no data is found
                            MessageBox.Show("No product found with the specified CustomerID.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to load Product data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void btnCustomers_Click(object sender, EventArgs e)
        {
            grpCustomers.Visible = true;
            SetButtonStyle(btnCustomers, true);
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

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
    public partial class employeesDash : UserControl
    {

        // INITIALIZE & LOAD
        public employeesDash()
        {
            InitializeComponent();
            InitializeGrid();
            InitializeRoleGrid();
        }
        private void employee_Load(object sender, EventArgs e)
        {
            btnEmployees.PerformClick(); // Default state
            loadIDs();
            LoadData();
            viewdetails();
            RID.SelectedIndex = 0;  // Select the first item by default (index 0)
        }
        private void loadIDs()
        {
            try
            {

                db db = new db();
                // Load EmployeeIDs into PID combo box
                using (SqlDataReader reader = db.Select("SELECT EmployeeID FROM Employee"))
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("EmployeeID", typeof(int)); // Ensure consistent data type
                    dt.Columns.Add("DisplayText", typeof(string)); // For user-friendly display

                    // Add "New" placeholder row
                    DataRow newRow = dt.NewRow();
                    newRow["EmployeeID"] = -1; // Reserved value for "new item"
                    newRow["DisplayText"] = "New"; // What the user sees
                    dt.Rows.Add(newRow);

                    // Add actual EmployeeIDs
                    while (reader.Read())
                    {
                        DataRow row = dt.NewRow();
                        row["EmployeeID"] = reader.GetInt32(0); // Assuming EmployeeID is an int in your database
                        row["DisplayText"] = reader.GetInt32(0).ToString(); // Display as string
                        dt.Rows.Add(row);
                    }

                    // Bind to the ComboBox
                    EID.DisplayMember = "DisplayText"; // Text shown to the user
                    EID.ValueMember = "EmployeeID";    // Actual value (int) used internally
                    EID.DataSource = dt;
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

                // Load Role IDs into role ComboBox
                DataTable roleTable = new DataTable();
                roleTable.Columns.Add("RoleID", typeof(int));
                roleTable.Columns.Add("RoleName", typeof(string));

                // Add a default empty row
                DataRow emptyRoleRow = roleTable.NewRow();
                emptyRoleRow["RoleID"] = DBNull.Value; // No value selected
                emptyRoleRow["RoleName"] = ""; // Display as empty
                roleTable.Rows.Add(emptyRoleRow);

                using (SqlDataReader reader = db.Select("SELECT RoleID, RoleName FROM Role"))
                {
                    while (reader.Read())
                    {
                        DataRow row = roleTable.NewRow();
                        row["RoleID"] = reader.GetInt32(0); // Assuming RoleID is int
                        row["RoleName"] = reader.GetString(1); // RoleName as string
                        roleTable.Rows.Add(row);
                    }
                }
                rname.DisplayMember = "RoleName";
                rname.ValueMember = "RoleID";
                rname.DataSource = roleTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load roles: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private string GetRoleID(string roleName)
        {
            try
            {
                string query = "SELECT RoleID FROM Role WHERE RoleName = @RoleName";
                db db = new db();
                SqlParameter[] parameters = {
            new SqlParameter("@RoleName", roleName)
        };
                return db.ExecuteScalar(query, parameters).ToString(); // Returns the RoleID
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching RoleID: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "0"; // Return "0" if an error occurs
            }
        }
        private string GetRoleName(string roleID)
        {
            try
            {
                db db = new db();
                string query = $"SELECT RoleName FROM Role WHERE RoleID = '{roleID}'";
                using (SqlDataReader reader = db.Select(query))
                {
                    if (reader.Read())
                    {
                        return reader["RoleName"].ToString(); // Returns the RoleName
                    }
                }
                return string.Empty; // Return empty string if no record is found
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching RoleName: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return string.Empty; // Return empty string if an error occurs
            }
        }


        // GRID
        private DataTable GetRoleList()
        {
            string query = "SELECT RoleID, RoleName FROM Role";
            return new db().SelectDataTable(query);
        }
        private void InitializeGrid()
        {
            dataGridView1.Columns.Clear();

            // EmployeeID Column (hidden)
            var employeeIDColumn = new DataGridViewTextBoxColumn
            {
                Name = "EmployeeID",
                HeaderText = "Employee ID",
            };
            dataGridView1.Columns.Add(employeeIDColumn);

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

            // Role ComboBox Column
            var roleColumn = new DataGridViewComboBoxColumn
            {
                Name = "Role",
                HeaderText = "Role",
                DataSource = GetRoleList(), // Fetch roles from DB
                DisplayMember = "RoleName",
                ValueMember = "RoleID"
            };
            dataGridView1.Columns.Add(roleColumn);

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

            // HireDate Column
            var hireDateColumn = new DataGridViewTextBoxColumn
            {
                Name = "HireDate",
                HeaderText = "Hire Date"
            };
            dataGridView1.Columns.Add(hireDateColumn);
        }
        private void viewdetails()
        {
            string query = @"
            SELECT 
                e.EmployeeID,
                e.FirstName,
                e.LastName,
                r.RoleName,
                e.Email,
                e.PhoneNumber,
                e.HireDate,
                e.RoleID
            FROM 
                Employee e
            LEFT JOIN 
                Role r ON e.RoleID = r.RoleID";

            var reader = new db().Select(query);
            dataGridView1.Rows.Clear();

            while (reader.Read())
            {
                dataGridView1.Rows.Add(
                    reader["EmployeeID"],
                    reader["FirstName"],
                    reader["LastName"],
                    reader["RoleName"],
                    reader["Email"],
                    reader["PhoneNumber"],
                    reader["HireDate"]
                );
            }
        }
        private void InitializeRoleGrid()
        {
            dataGridView2.Columns.Clear();

            // RoleID Column
            var roleIDColumn = new DataGridViewTextBoxColumn
            {
                Name = "RoleID",
                HeaderText = "Role ID"
            };
            dataGridView2.Columns.Add(roleIDColumn);

            // RoleName Column
            var roleNameColumn = new DataGridViewTextBoxColumn
            {
                Name = "RoleName",
                HeaderText = "Role Name"
            };
            dataGridView2.Columns.Add(roleNameColumn);
        }
        private void ViewRoleDetails()
        {
            string query = @"
            SELECT 
                RoleID,
                RoleName
            FROM 
                Role";

            var reader = new db().Select(query);
            dataGridView2.Rows.Clear();

            while (reader.Read())
            {
                dataGridView2.Rows.Add(
                    reader["RoleID"],
                    reader["RoleName"]
                );
            }
        }
        private void ClearFields()
        {
            fname.Text = "";
            lname.Text = "";
            emails.Text = "";
            tp.Text = "";
            rname.Text = "";
            hiredate.Text = "";
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
            string RoleName = rname.Text.Trim();
            string Hiredate = hiredate.Value.ToString("yyyy-MM-dd");

            string RoleID = GetRoleID(RoleName);


            // Initialize database connection
            db DB = new db();

            string query = $@"INSERT INTO Employee (FirstName, LastName , RoleID ,Email , PhoneNumber , HireDate) 
            VALUES ('{FirstName}' , '{LastName}' , '{RoleID}' ,'{Email}', '{contact}' , '{Hiredate}')";

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
            string EmployeeID = EID.Text.Trim();
            string FirstName = fname.Text.Trim();
            string LastName = lname.Text.Trim();
            string Email = emails.Text.Trim();
            string contact = tp.Text.Trim();
            string RoleName = rname.Text.Trim();
            string Hiredate = hiredate.Value.ToString("yyyy-MM-dd");

            string RoleID = GetRoleID(RoleName);

            // Check if EmployeeID is a valid integer
            if (!int.TryParse(EmployeeID, out _))
            {
                MessageBox.Show("Please select a valid EmployeeID to make Updates.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Initialize database connection
            db DB = new db();

            string query = $@"UPDATE Employee 
                      SET FirstName='{FirstName}', 
                          LastName='{LastName}', 
                          RoleID='{RoleID}',
                          Email='{Email}', 
                          PhoneNumber='{contact}', 
                          Hiredate='{Hiredate}'
                      WHERE EmployeeID='{EmployeeID}'";

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
            string EmployeeID = EID.Text.Trim();

            // Check if EmployeeID is a valid integer
            if (!int.TryParse(EmployeeID, out int parsedEmployeeID))
            {
                MessageBox.Show("Please select a valid EmployeeID to delete.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Show confirmation message
            DialogResult result = MessageBox.Show(
                $"Are you sure you want to delete the EmployeeID Row {parsedEmployeeID}?",
                "Confirm Deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            // If the user confirms, proceed with deletion
            if (result == DialogResult.Yes)
            {
                // Initialize database connection
                db DB = new db();
                string query = $@"DELETE FROM Employee WHERE EmployeeID='{parsedEmployeeID}'";

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
        private void EID_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (sender is ComboBox comboBox) // Ensure sender is a ComboBox
            {
                string newEmployeeID = comboBox.SelectedValue?.ToString(); // Get the actual EmployeeID value
                try
                {
                    db db = new db();

                    string query = $@"SELECT FirstName, LastName, RoleID, Email, PhoneNumber, HireDate 
                              FROM Employee WHERE EmployeeID='{newEmployeeID}'";

                    using (SqlDataReader reader = db.Select(query))
                    {
                        if (reader.Read()) // Check if data exists
                        {
                            string RoleID = reader["RoleID"].ToString();

                            fname.Text = reader["FirstName"].ToString();
                            lname.Text = reader["LastName"].ToString();
                            rname.Text = GetRoleName(RoleID);
                            emails.Text = reader["Email"].ToString();
                            tp.Text = reader["PhoneNumber"].ToString();
                            hiredate.Text = reader["HireDate"].ToString();
                        }
                        else
                        {
                            // Handle case where no data is found
                            MessageBox.Show("No product found with the specified EmployeeID.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to load Product data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnEmployees_Click(object sender, EventArgs e)
        {
            // Show grpEmployees and hide grpRoles
            grpEmployees.Visible = true;
            grpRoles.Visible = false;

            // Apply "clicked" style to btnEmployees
            SetButtonStyle(btnEmployees, true);
            SetButtonStyle(btnRoles, false);
        }
        private void btnRoles_Click(object sender, EventArgs e)
        {
            // Show grpRoles and hide grpEmployees
            grpRoles.Visible = true;
            grpEmployees.Visible = false;
            ViewRoleDetails();

            // Apply "clicked" style to btnRoles
            SetButtonStyle(btnRoles, true);
            SetButtonStyle(btnEmployees, false);
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

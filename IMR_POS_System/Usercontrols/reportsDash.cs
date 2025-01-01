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
    public partial class reportsDash : UserControl
    {

        // INITIALIZE & LOAD
        public reportsDash()
        {
            InitializeComponent();
            InitializeGrid();
            InitializeAuditLogGrid();
        }
        private void report_Load(object sender, EventArgs e)
        {
            btnReports.PerformClick(); // Default state
            loadIDs();
            viewdetails();
            LID.SelectedIndex = 0;  // Select the first item by default (index 0)
        }
        private void loadIDs()
        {
            try
            {
                db db = new db();
                // Load ReportIDs into a ComboBox
                using (SqlDataReader reader = db.Select("SELECT ReportID FROM Report"))
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("ReportID", typeof(int)); // Ensure consistent data type
                    dt.Columns.Add("DisplayText", typeof(string)); // For user-friendly display

                    // Add "New" placeholder row
                    DataRow newRow = dt.NewRow();
                    newRow["ReportID"] = -1; // Reserved value for "new item"
                    newRow["DisplayText"] = "New"; // What the user sees
                    dt.Rows.Add(newRow);

                    // Add actual ReportIDs
                    while (reader.Read())
                    {
                        DataRow row = dt.NewRow();
                        row["ReportID"] = reader.GetInt32(0); // Assuming ReportID is an int in your database
                        row["DisplayText"] = reader.GetInt32(0).ToString(); // Display as string
                        dt.Rows.Add(row);
                    }

                    // Bind to the ComboBox
                    RID.DisplayMember = "DisplayText"; // Text shown to the user
                    RID.ValueMember = "ReportID";    // Actual value (int) used internally
                    RID.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load Report data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private bool CheckEmployeeID(string employeeName)
        {
            try
            {
                // Query to check if the employee name exists
                string checkQuery = "SELECT COUNT(*) FROM Employee WHERE Lastname = @Lastname";
                db db = new db();
                SqlParameter[] checkParams = {
            new SqlParameter("@Lastname", employeeName)
        };

                // Check if the employee exists
                int count = Convert.ToInt32(db.ExecuteScalar(checkQuery, checkParams));
                if (count == 0)
                {
                    return false; // Return "false" if employee does not exist
                }
                else
                {
                    return true; // Return "true" if employee does exist
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error checking EmployeeID: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false; // Return false if an error occurs
            }
        }
        private string GetEmployeeID(string employeeName)
        {
            try
            {
                db db = new db();
                // Query to get the EmployeeID
                string query = "SELECT EmployeeID FROM Employee WHERE Lastname = @Lastname";
                SqlParameter[] parameters = {
            new SqlParameter("@Lastname", employeeName)
        };
                return db.ExecuteScalar(query, parameters).ToString(); // Returns the EmployeeID
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching EmployeeID: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "0"; // Return "0" if an error occurs
            }
        }
        private string GetEmployeeName(string employeeID)
        {
            try
            {
                db db = new db();
                string query = $"SELECT Lastname FROM Employee WHERE EmployeeID = '{employeeID}'";
                using (SqlDataReader reader = db.Select(query))
                {
                    if (reader.Read())
                    {
                        return reader["Lastname"].ToString(); // Returns the EmployeeName
                    }
                }
                return string.Empty; // Return empty string if no record is found
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching Employee Lastname: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return string.Empty; // Return empty string if an error occurs
            }
        }


        // GRID
        private DataTable GetEmployeeList()
        {
            string query = "SELECT EmployeeID, LastName FROM Employee";
            return new db().SelectDataTable(query);
        }
        private DataTable GetTableList()
        {
            string query = "SELECT TABLE_NAME AS TableName FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";
            return new db().SelectDataTable(query);
        }
        private void InitializeGrid()
        {
            dataGridView1.Columns.Clear();

            // ReportID Column (hidden)
            var reportIDColumn = new DataGridViewTextBoxColumn
            {
                Name = "ReportID",
                HeaderText = "Report ID",
            };
            dataGridView1.Columns.Add(reportIDColumn);

            // ReportName Column
            var reportNameColumn = new DataGridViewTextBoxColumn
            {
                Name = "ReportName",
                HeaderText = "Report Name"
            };
            dataGridView1.Columns.Add(reportNameColumn);

            // GeneratedBy Column
            var generatedByColumn = new DataGridViewTextBoxColumn
            {
                Name = "GeneratedBy",
                HeaderText = "Generated By"
            };
            dataGridView1.Columns.Add(generatedByColumn);

            // GeneratedDate Column
            var generatedDateColumn = new DataGridViewTextBoxColumn
            {
                Name = "GeneratedDate",
                HeaderText = "Generated Date"
            };
            dataGridView1.Columns.Add(generatedDateColumn);

            // ReportType Column
            var reportTypeColumn = new DataGridViewTextBoxColumn
            {
                Name = "ReportType",
                HeaderText = "Report Type"
            };
            dataGridView1.Columns.Add(reportTypeColumn);

            // ReportData Column
            var reportDataColumn = new DataGridViewTextBoxColumn
            {
                Name = "ReportData",
                HeaderText = "Report Data"
            };
            dataGridView1.Columns.Add(reportDataColumn);
        }
        private void viewdetails()
        {
            string query = @"
            SELECT 
                r.ReportID,
                r.ReportName,
                e.LastName AS GeneratedBy,
                r.GeneratedDate,
                r.ReportType,
                r.ReportData
            FROM 
                Report r
            LEFT JOIN 
                Employee e ON r.GeneratedBy = e.EmployeeID";

            var reader = new db().Select(query);
            dataGridView1.Rows.Clear();

            while (reader.Read())
            {
                dataGridView1.Rows.Add(
                    reader["ReportID"],
                    reader["ReportName"],
                    reader["GeneratedBy"],
                    reader["GeneratedDate"],
                    reader["ReportType"],
                    reader["ReportData"]
                );
            }
        }
        private void InitializeAuditLogGrid()
        {
            dataGridView2.Columns.Clear();

            // LogID Column
            var logIDColumn = new DataGridViewTextBoxColumn
            {
                Name = "LogID",
                HeaderText = "Log ID"
            };
            dataGridView2.Columns.Add(logIDColumn);

            // Action Column
            var actionColumn = new DataGridViewTextBoxColumn
            {
                Name = "Action",
                HeaderText = "Action"
            };
            dataGridView2.Columns.Add(actionColumn);

            // PerformedBy ComboBox Column
            var performedByColumn = new DataGridViewComboBoxColumn
            {
                Name = "PerformedBy",
                HeaderText = "Performed By",
                DataSource = GetEmployeeList(), // Fetch employees from DB
                DisplayMember = "LastName",
                ValueMember = "EmployeeID"
            };
            dataGridView2.Columns.Add(performedByColumn);

            // ActionTimeStamp Column
            var actionTimeStampColumn = new DataGridViewTextBoxColumn
            {
                Name = "ActionTimeStamp",
                HeaderText = "Action TimeStamp"
            };
            dataGridView2.Columns.Add(actionTimeStampColumn);

            // TableName ComboBox Column
            var tableNameColumn = new DataGridViewComboBoxColumn
            {
                Name = "TableName",
                HeaderText = "Table Name",
                DataSource = GetTableList(), // Fetch table names from DB
                DisplayMember = "TableName",
                ValueMember = "TableName"
            };
            dataGridView2.Columns.Add(tableNameColumn);
        }
        private void ViewAuditLogDetails()
        {
            string query = @"
                    SELECT 
                        a.LogID,
                        a.Action,
                        e.LastName AS PerformedBy,
                        a.ActionTimeStamp,
                        a.TableName
                    FROM 
                        AuditLog a
                    LEFT JOIN 
                        Employee e ON a.PerformedBy = e.EmployeeID";

            var reader = new db().Select(query);
            dataGridView2.Rows.Clear();

            while (reader.Read())
            {
                dataGridView2.Rows.Add(
                    reader["LogID"],
                    reader["Action"],
                    reader["PerformedBy"],
                    reader["ActionTimeStamp"],
                    reader["TableName"]
                );
            }
        }
        private void ClearFields()
        {
            txtReportName.Text = "";
            cmbGeneratedBy.Text = "";
            dtpGeneratedDate.Text = "";
            txtReportType.Text = "";
            txtReportData.Text = "";
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
            string ReportName = txtReportName.Text.Trim();
            string GeneratedBy = cmbGeneratedBy.Text.Trim();
            string GeneratedDate = dtpGeneratedDate.Value.ToString("yyyy-MM-dd");
            string ReportType = txtReportType.Text.Trim();
            string ReportData = txtReportData.Text.Trim();

            // Check if the employee is registered
            bool isEmployeeRegistered = CheckEmployeeID(GeneratedBy);
            if (!isEmployeeRegistered)
            {
                MessageBox.Show("This Employee is not registered!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Stop execution
            }

            // Get related IDs
            string EmployeeID = GetEmployeeID(GeneratedBy);

            if (string.IsNullOrEmpty(EmployeeID))
            {
                MessageBox.Show("Invalid Employee Name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Initialize database connection
            db DB = new db();

            string query = $@"
                    INSERT INTO Report (ReportName, GeneratedBy, GeneratedDate, ReportType, ReportData)
                    VALUES ('{ReportName}', '{EmployeeID}', '{GeneratedDate}', '{ReportType}', '{ReportData}')";

            try
            {
                // Execute SQL query
                DB.Execute(query);
                MessageBox.Show("Report inserted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            string ReportID = RID.Text.Trim();
            string ReportName = txtReportName.Text.Trim();
            string GeneratedBy = cmbGeneratedBy.Text.Trim();
            string GeneratedDate = dtpGeneratedDate.Value.ToString("yyyy-MM-dd");
            string ReportType = txtReportType.Text.Trim();
            string ReportData = txtReportData.Text.Trim();

            // Check if the employee is registered
            bool isEmployeeRegistered = CheckEmployeeID(GeneratedBy);
            if (!isEmployeeRegistered)
            {
                MessageBox.Show("This Employee is not registered!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Stop execution
            }

            // Get related IDs
            string EmployeeID = GetEmployeeID(GeneratedBy);

            // Check if ReportID is a valid integer
            if (!int.TryParse(ReportID, out _))
            {
                MessageBox.Show("Please select a valid ReportID to make Updates.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(EmployeeID))
            {
                MessageBox.Show("Invalid Employee Name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Initialize database connection
            db DB = new db();

            string query = $@"
                    UPDATE Report 
                    SET ReportName='{ReportName}', 
                        GeneratedBy='{EmployeeID}', 
                        GeneratedDate='{GeneratedDate}', 
                        ReportType='{ReportType}', 
                        ReportData='{ReportData}'
                    WHERE ReportID='{ReportID}'";

            try
            {
                // Execute SQL query
                DB.Execute(query);
                MessageBox.Show("Report updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            string ReportID = RID.Text.Trim();

            // Check if ReportID is a valid integer
            if (!int.TryParse(ReportID, out int parsedReportID))
            {
                MessageBox.Show("Please select a valid ReportID to delete.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Show confirmation message
            DialogResult result = MessageBox.Show(
                $"Are you sure you want to delete the ReportID Row {parsedReportID}?",
                "Confirm Deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            // If the user confirms, proceed with deletion
            if (result == DialogResult.Yes)
            {
                // Initialize database connection
                db DB = new db();
                string query = $@"DELETE FROM Report WHERE ReportID='{parsedReportID}'";

                try
                {
                    // Execute SQL query
                    DB.Execute(query);
                    MessageBox.Show("Report deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        private void RID_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (sender is ComboBox comboBox) // Ensure sender is a ComboBox
            {
                string newReportID = comboBox.SelectedValue?.ToString(); // Get the actual ReportID value
                try
                {
                    db db = new db();

                    string query = $@"SELECT ReportName, GeneratedBy, GeneratedDate, ReportType, ReportData
                              FROM Report WHERE ReportID='{newReportID}'";

                    using (SqlDataReader reader = db.Select(query))
                    {
                        if (reader.Read()) // Check if data exists
                        {
                            string EmployeeID = reader["GeneratedBy"].ToString();

                            txtReportName.Text = reader["ReportName"].ToString();
                            cmbGeneratedBy.Text = GetEmployeeName(EmployeeID); // Assuming GetEmployeeName is defined to fetch name from EmployeeID
                            dtpGeneratedDate.Text = reader["GeneratedDate"].ToString();
                            txtReportType.Text = reader["ReportType"].ToString();
                            txtReportData.Text = reader["ReportData"].ToString();
                        }
                        else
                        {
                            // Handle case where no data is found
                            MessageBox.Show("No report found with the specified ReportID.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to load Report data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            grpReports.Visible = true;
            grpLogs.Visible = false;

            SetButtonStyle(btnReports, true);
            SetButtonStyle(btnLogs, false);
        }
        private void btnLogs_Click(object sender, EventArgs e)
        {
            grpLogs.Visible = true;
            grpReports.Visible = false;
            ViewAuditLogDetails();

            SetButtonStyle(btnLogs, true);
            SetButtonStyle(btnReports, false);
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

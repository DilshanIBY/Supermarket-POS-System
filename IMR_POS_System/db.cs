using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS
{
    internal class db
    {
        //1. Address of SQL server and database ( Connection string )
        string ConnectionString = @"Data Source=DILSHAN\SQLEXPRESS02;Initial Catalog=SuperStorePOS;Integrated Security=True";

        //2. Establish connection ( C# sqlconnection class)
        SqlConnection conn = null;


        public db()
        {
            conn = new SqlConnection(ConnectionString);
        }


        public void Execute(string Query)
        {
            try
            {
                //3. Open Connection
                conn.Open();

                //4. Prepare sql Query
                SqlCommand cmd = new SqlCommand(Query, conn);

                //5. Execute query ( C# sqlcommand class)
                cmd.ExecuteNonQuery();
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            finally
            {
                //6. Close Connection 
                conn.Close();
            }

        }
        public SqlDataReader Select(string Query)
        {
            try
            {
                //3. Open Connection
                conn.Open();

                //4. Prepare sql Query
                SqlCommand cmd = new SqlCommand(Query, conn);

                //5. Execute query ( C# sqlcommand class) ExecuteReader
                return cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return null;
        }


        public int ExecuteScalar(string query, SqlParameter[] parameters)
        {
            using (var conn = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                cmd.Parameters.AddRange(parameters);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public DataTable SelectDataTable(string query)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            return dataTable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle the error
                throw new Exception("An error occurred while fetching data: " + ex.Message);
            }
        }
    }
}

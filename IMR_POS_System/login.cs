using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IMR_POS_System
{
    public partial class login : Form
    {
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;

        private string _usernamePlaceholder = "Enter Username";
        private string _passwordPlaceholder = "Enter Password";
        private Color _placeholderColor = Color.Gray;
        public login()
        {
            InitializeComponent();
            // Add mouse event handlers for panel1
            panel1.MouseDown += new MouseEventHandler(panel1_MouseDown);
            panel1.MouseMove += new MouseEventHandler(panel1_MouseMove);
            panel1.MouseUp += new MouseEventHandler(panel1_MouseUp);
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            dragging = true;
            dragCursorPoint = Cursor.Position;
            dragFormPoint = this.Location;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point diff = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(diff));
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        private void login_Load(object sender, EventArgs e)
        {
            textBox1.Text = _usernamePlaceholder;
            textBox1.ForeColor = _placeholderColor;

            if (textBox2 != null)
            {
                textBox2.Text = _passwordPlaceholder;
                textBox2.ForeColor = _placeholderColor;
                textBox2.PasswordChar = '*';
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit?", "Exit", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "Admin" && textBox2.Text == "Admin")
            {
                this.Hide();
                home home = new home();
                home.Show();
            }
            else
            {
                MessageBox.Show("Invalid Username or Password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

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
    public partial class home : Form
    {
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;

        public home()
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

        private void home_Load(object sender, EventArgs e)
        {
            // Call the click event for button8 programmatically
            button8_Click_1(null, EventArgs.Empty);
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

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to log out?", "Log Out", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this.Hide();
                login login = new login();
                login.Show();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mainpanel.Controls.Clear();
            Usercontrols.employeesDash emp = new Usercontrols.employeesDash();
            emp.Dock = DockStyle.Fill;
            mainpanel.Controls.Add(emp);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            mainpanel.Controls.Clear();
            Usercontrols.customersDash customer = new Usercontrols.customersDash();
            customer.Dock = DockStyle.Fill;
            mainpanel.Controls.Add(customer);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            mainpanel.Controls.Clear();
            Usercontrols.stocksDash stocks = new Usercontrols.stocksDash();
            stocks.Dock = DockStyle.Fill;
            mainpanel.Controls.Add(stocks);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            mainpanel.Controls.Clear();
            Usercontrols.salesDash Sales = new Usercontrols.salesDash();
            Sales.Dock = DockStyle.Fill;
            mainpanel.Controls.Add(Sales);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            mainpanel.Controls.Clear();
            Usercontrols.discountsDash dis = new Usercontrols.discountsDash();
            dis.Dock = DockStyle.Fill;
            mainpanel.Controls.Add(dis);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            mainpanel.Controls.Clear();
            Usercontrols.reportsDash report = new Usercontrols.reportsDash();
            report.Dock = DockStyle.Fill;
            mainpanel.Controls.Add(report);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            mainpanel.Controls.Clear();
            Usercontrols.productsDash product = new Usercontrols.productsDash();
            product.Dock = DockStyle.Fill;
            mainpanel.Controls.Add(product);
        }

        private void button8_Click_1(object sender, EventArgs e)
        {
            mainpanel.Controls.Clear();
            Usercontrols.overviewDash overview = new Usercontrols.overviewDash();
            overview.Dock = DockStyle.Fill;
            mainpanel.Controls.Add(overview);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _1812756_NguyenTrongHieu
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btn_Click(object sender, EventArgs e)
        {
            try
            {
                ServerProgram server = new ServerProgram();
                server.Start();
                btn.Enabled = false;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
           

        }
    }
}

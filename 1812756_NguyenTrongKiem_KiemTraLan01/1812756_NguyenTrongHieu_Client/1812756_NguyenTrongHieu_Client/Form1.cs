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
    public partial class frmClient : Form
    {
        ClientProgram client;
        public frmClient()
        {
            InitializeComponent();
        }

        private void btnKetNoi_Click(object sender, EventArgs e)
        {
            client = new ClientProgram();
            string str = client.Connect();
            txtHoTen.Text = str;
            btnKetNoi.Enabled = false;
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            string data = "";
            data += txtHoTen.Text +";"+ txtMSSV.Text + ";"+ txtQueQuan.Text;
           string str = client.SendData(data);
            
            MessageBox.Show(str);

            txtHoTen.Text = "";
            txtMSSV.Text = "";
            txtQueQuan.Text = "";
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
          string data =  client.SendData(txtTimKiem.Text);
            if (data.Contains(";"))
            {
                txtHoTen.Text = data.Split(';')[0];
                txtMSSV.Text = data.Split(';')[1];
                txtQueQuan.Text = data.Split(';')[2];
            }
            else
            {
                MessageBox.Show(data);
            }
           
        }
    }
}

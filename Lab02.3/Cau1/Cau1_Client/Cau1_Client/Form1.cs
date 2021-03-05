using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cau1_Client
{
    public partial class Form1 : Form
    {
        StreamReader sr;
        StreamWriter sw;

        public Form1()
        {
            InitializeComponent();
        }

        public void AddTextFunc(string text)
        {
            txtMessageDisplay.AppendText(text);
            txtMessageDisplay.AppendText(Environment.NewLine);
        }

        public void connect()
        {
            TcpClient client = new TcpClient("127.0.0.1", 5000);
            NetworkStream ns = client.GetStream();
            sr = new StreamReader(ns);
            sw = new StreamWriter(ns);
            String str = sr.ReadLine();
            AddTextFunc(str);
        }

        public void sendData(String text)
        {
            sw.WriteLine(text);
            sw.Flush();
            AddTextFunc(text);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            connect();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            sendData(txtMessageInput.Text);
            txtMessageInput.Text = "";
        }
    }
}

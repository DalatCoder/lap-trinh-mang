using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    public partial class frmServer : Form
    {
        UdpClient serverSocket;
        IPEndPoint clientEndpoint;

        List<User> users = new List<User>();
        List<Form> remoteScreens = new List<Form>();

        public frmServer()
        {
            CheckForIllegalCrossThreadCalls = false; 
            InitializeComponent();

            lbClients.DoubleClick += LbClients_DoubleClick;
        }

        private void LbClients_DoubleClick(object sender, EventArgs e)
        {
            if (lbClients.SelectedItem == null)
                return;

            string userNickname = lbClients.SelectedItem.ToString();
            User user = users.Find(u => u.Nickname.Equals(userNickname));

            clientForm frm = new clientForm(user, SendData);

            frm.Show();

            remoteScreens.Add(frm);
        }

        void SendData(string str, IPEndPoint remoteEndpoint)
        {
            byte[] data = Encoding.ASCII.GetBytes(str);
            serverSocket.Send(data, data.Length, remoteEndpoint);
        }

        void StartServer()
        {
            IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Any, 3000);
            serverSocket = new UdpClient(serverEndpoint);

            clientEndpoint = new IPEndPoint(IPAddress.Any, 0);

            lbClientMessages.Items.Add("Dang cho client ket noi den...");

            byte[] data = new byte[2048];

            string dataStr;

            while (true)
            {
                data = serverSocket.Receive(ref clientEndpoint);

                foreach (clientForm form in remoteScreens)
                {
                    if (clientEndpoint.ToString().Equals(form.RemoteEndpoint.ToString()))
                    {
                        form.AppendMessage(Encoding.ASCII.GetString(data, 0, data.Length));
                    }
                }

                if (IsNewClientConnection(clientEndpoint))
                {
                    lbClientMessages.Items.Add("Thong tin client ket noi: " + clientEndpoint);

                    dataStr = Encoding.ASCII.GetString(data, 0, data.Length);

                    // lbClients.Items.Add(clientEndpoint.ToString());
                    lbClients.Items.Add(dataStr);

                    users.Add(new User(dataStr, clientEndpoint));

                    lbClientMessages.Items.Add(clientEndpoint + ": " + dataStr);

                    dataStr = "Xin chao client";
                    data = Encoding.ASCII.GetBytes(dataStr);

                    serverSocket.Send(data, data.Length, clientEndpoint);
                }
                else
                {

                    dataStr = Encoding.ASCII.GetString(data, 0, data.Length);
                    lbClientMessages.Items.Add(clientEndpoint + ": " + dataStr);
                }
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Thread startServerThread = new Thread(new ThreadStart(StartServer));
            startServerThread.IsBackground = true;
            startServerThread.Start();

            btnStart.Enabled = false;
        }

        bool IsNewClientConnection(IPEndPoint clientEndpoint)
        {
            if (lbClients.Items.Count == 0) return true;

            foreach (User user in users)
            {
                if (user.IpEndpoint.ToString().Equals(clientEndpoint.ToString()))
                    return false;
            }

            return true;
        }

        private void frmServer_Load(object sender, EventArgs e)
        {
            btnStart.PerformClick();
        }
    }
}

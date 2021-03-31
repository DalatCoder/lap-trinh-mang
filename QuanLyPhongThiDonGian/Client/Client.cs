using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Common;

namespace Multichat_KTeam
{
    public partial class Client : Form
    {
        IPEndPoint IP;
        Socket client;

        public Client()
        {
            InitializeComponent();

            CheckForIllegalCrossThreadCalls = false;
            btnFinishExam.Enabled = true;
        }

        /// <summary>
        /// Kết nối đến server
        /// </summary>
        void Connect(string hostname, int port)
        {
            IP = new IPEndPoint(IPAddress.Parse(hostname), port);
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                client.Connect(IP);
            }
            catch
            {
                MessageBox.Show("Không thể kết nối đến server", "Lỗi");
                return;
            }

            Thread listen = new Thread(Receive);
            listen.IsBackground = true;
            listen.Start();
        }

        /// <summary>
        /// Đóng kết nối socket
        /// </summary>
        
        void CloseConnection()
        {
            client.Close();
        }

        /// <summary>
        /// Lắng nghe phản hồi từ phía server
        /// </summary>
        void Receive()
        {
            try
            {
                while (true)
                {
                    byte[] buffer = new byte[1024 * 1024 * 20];
                    client.Receive(buffer);

                    ServerResponse response = (ServerResponse)Deserialize(buffer);

                    switch (response.Type)
                    {

                        case ServerResponseType.SendStudentList:
                            List<Student> students = response.Data as List<Student>;
                            cbDSThi.DataSource = students;
                            cbDSThi.DisplayMember = "FullNameAndId";
                            cbDSThi.ValueMember = "ID";
                            break;

                        case ServerResponseType.SendFile:

                            ServerResponseFile file = response.Data as ServerResponseFile;

                            lblDeThi.Text = file.Info.Name;

                            buffer = file.FileContent;

                            string filename = file.Info.Name;

                            using (var fileStream = File.Create(filename))
                            {
                                fileStream.Write(buffer, 0, buffer.Length);
                            }

                            break;


                        case ServerResponseType.BeginExam:
                            lblThoiGian.Text = (string)response.Data;
                            break;

                        default:
                            break;
                    }
                }
            } 
            catch {
                MessageBox.Show("Có lỗi xảy ra trong quá trình nhận phản hồi từ server. Đóng kết nối");
                CloseConnection();
            }
        }

        /// <summary>
        /// Phân mảnh dữ liệu, tạo thành mảng byte để gửi đi
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        byte[] Serialize(object data)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();

            formatter.Serialize(stream, data);

            return stream.ToArray();
        }

        /// <summary>
        /// Gom mảnh dữ liệu, tạo thành đối tượng ban đầu
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        object Deserialize(byte[] data)
        {
            MemoryStream stream = new MemoryStream(data);
            BinaryFormatter formatter = new BinaryFormatter();

            return formatter.Deserialize(stream);
        }

        /// <summary>
        /// Đóng kết nối khi đóng form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Client_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseConnection();
        }

        private void btnFinishExam_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "All files (*.*)|*.*";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                ServerResponseFile file = new ServerResponseFile();
                file.SetFile(dialog.FileName);

                ServerResponse response = new ServerResponse();
                response.Type = ServerResponseType.SendFile;
                response.Data = file;

                client.Send(Serialize(response));
            }
        }

        private void btnConnectToServer_Click(object sender, EventArgs e)
        {
            string[] parts = txtServerIP.Text.Split(':');
            Connect(parts[0], Convert.ToInt32(parts[1]));
        }

        private void cmdChapNhan_Click(object sender, EventArgs e)
        {

        }
    }
}

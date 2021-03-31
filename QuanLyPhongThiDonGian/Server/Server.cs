using Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;

namespace Server
{
    public partial class Server : Form
    {
        IPEndPoint IP;
        Socket server;
        List<Socket> clientList;

        public Server()
        {
            InitializeComponent();

            CheckForIllegalCrossThreadCalls = false;

            Start();
        }
        

        /// <summary>
        /// Kết nối đến server
        /// </summary>
        void Start()
        {
            IP = new IPEndPoint(IPAddress.Any, 2010);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientList = new List<Socket>();

            server.Bind(IP);

            Thread listen = new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        server.Listen(100);
                        Socket client = server.Accept();
                        clientList.Add(client);

                        Thread receive = new Thread(Receive);
                        receive.IsBackground = true;
                        receive.Start(client);

                        AddMessage(client.RemoteEndPoint.ToString() + ": " + "Đã kết nối");
                    }
                } 
                catch
                {
                    IP = new IPEndPoint(IPAddress.Any, 2010);
                    server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                }
            });

            listen.IsBackground = true;
            listen.Start();
        }

        /// <summary>
        /// Đóng kết nối socket
        /// </summary>

        void CloseConnection()
        {
            server.Close();
        }

        /// <summary>
        /// Gửi tin nhắn cho client
        /// </summary>
        void Send(Socket client)
        {
            if (!string.IsNullOrWhiteSpace(txtInput.Text))
            {
                ServerResponse response = new ServerResponse()
                {
                    Type = ServerResponseType.SendString,
                    Data = txtInput.Text
                };

                client.Send(Serialize(response));
            }       
        }

        /// <summary>
        /// Lắng nghe phản hồi từ phía server
        /// </summary>
        void Receive(object obj)
        {
            Socket client = obj as Socket;

            try
            {
                while (true)
                {
                    byte[] buffer = new byte[1024 * 1024 * 20];
                    client.Receive(buffer);

                    ServerResponse response = (ServerResponse)Deserialize(buffer);

                    switch (response.Type)
                    {
                        case ServerResponseType.SendFile:
                            ServerResponseFile file = response.Data as ServerResponseFile;

                            buffer = file.FileContent;

                            string filename = file.Info.Name;
                            AddMessage(client.RemoteEndPoint.ToString() + ": Receive file, save as: " + filename);
                            AddMessage(client.RemoteEndPoint.ToString() + ": File size: " + file.Info.Length + " B");

                            using (var fileStream = File.Create(filename))
                            {
                                fileStream.Write(buffer, 0, buffer.Length);
                            }


                            break;
                        
                        default:
                            break;
                    }
                }
            }
            catch
            {
                AddMessage(client.RemoteEndPoint.ToString() + ": " + "Đã đóng kết nối");
                clientList.Remove(client);
                client.Close();
            }
        }


        /// <summary>
        /// Thêm tin nhắn vào màn hình
        /// </summary>
        /// <param name="message"></param>
        void AddMessage(string message)
        {
            lsvMain.Items.Add(new ListViewItem() { Text = message });
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
        /// Gửi tin cho tất cả client
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSend_Click(object sender, EventArgs e)
        {
            foreach (Socket socket in clientList)
            {
                Send(socket);
            }

            AddMessage(txtInput.Text);
            txtInput.Clear();
        }

        /// <summary>
        /// Đóng kết nối khi đóng form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Server_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseConnection();
        }

        private void btnSendFile_Click(object sender, EventArgs e)
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

                foreach (Socket socket in clientList)
                {
                    socket.Send(Serialize(response));
                }

                AddMessage("Server: Send file to all clients");
            }
        }

        private void btnSendStudents_Click(object sender, EventArgs e)
        {
            List<Student> students = new List<Student>()
            {
                new Student() { Id = "1812756", FirstName = "Hieu", LastName = "Nguyen Trong" },
                new Student() { Id = "1812751", FirstName = "Ha", LastName = "Nguyen Thi" }
            };

            ServerResponse response = new ServerResponse();
            response.Type = ServerResponseType.SendStudentList;
            response.Data = students;

            foreach (Socket socket in clientList)
            {
                socket.Send(Serialize(response));
            }

            AddMessage("Server: Send student list to all clients");
        }

        private void btnBegin_Click(object sender, EventArgs e)
        {
            string time = txtSetTime.Text;

            ServerResponse response = new ServerResponse();
            response.Type = ServerResponseType.BeginExam;
            response.Data = time;

            foreach (Socket socket in clientList)
            {
                socket.Send(Serialize(response));
            }

            AddMessage("Server: Begin exam");
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            ServerResponse response = new ServerResponse();
            response.Type = ServerResponseType.FinishExam;
            response.Data = null;

            foreach (Socket socket in clientList)
            {
                socket.Send(Serialize(response));
            }

            AddMessage("Server: Finish exam");
        }
    }
}

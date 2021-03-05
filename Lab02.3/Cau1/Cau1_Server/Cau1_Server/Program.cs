using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cau1_Server
{
    public class AddTextEvenArgs : EventArgs
    {
        string text;

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public AddTextEvenArgs(String text)
        {
            this.text = text;
        }
    }

    public delegate void AddTextEvenHandle(object sender, AddTextEvenArgs AddTextEA);

    public class ServerTcpConnect
    {
        event AddTextEvenHandle OnAddText;

        TcpListener server;
        public void Start()
        {
            Thread mainThread = new Thread(new ThreadStart(StartConnection));
            mainThread.Start();
        }

        public void StartConnection()
        {
            server = new TcpListener(IPAddress.Any, 5000);
            server.Start();
            while (true)
            {
                Thread clientThread = new Thread(new ThreadStart(ClientConnected));
                clientThread.Start();
            }
        }

        public void ClientConnected()
        {
            TcpClient client = server.AcceptTcpClient();
            NetworkStream ns = client.GetStream();
            StreamReader sr = new StreamReader(ns);
            StreamWriter wr = new StreamWriter(ns);

            String str = "Hello client";
            wr.WriteLine(str);
            wr.Flush();

            while (true)
            {
                str = sr.ReadLine();

                if (OnAddText != null)
                {
                    this.OnAddText(this, new AddTextEvenArgs(client.Client.RemoteEndPoint.ToString() + ": " + str));
                }

                wr.WriteLine(str);
                wr.Flush();
            }
        }
    }

    public class Program
    {        
        static void Main(string[] args)
        {
            ServerTcpConnect server = new ServerTcpConnect();
            server.Start();
        }
    }
}

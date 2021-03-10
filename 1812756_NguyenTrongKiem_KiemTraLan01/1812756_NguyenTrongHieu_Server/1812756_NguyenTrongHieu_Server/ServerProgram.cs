using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace _1812756_NguyenTrongHieu
{
    public class ServerProgram
    {
        TcpListener server;
        
        public void Start()
        {
            Thread mainThread = new Thread(new ThreadStart(StartConnection));
            mainThread.Start();
        }

        void StartConnection()
        {
            server = new TcpListener(IPAddress.Any, 5000);
            server.Start();

            while(true)
            {
                Thread thread = new Thread(new ThreadStart(ClientConnected));
                thread.Start();
            }
        }

        void ClientConnected()
        {
            TcpClient client = server.AcceptTcpClient();
            NetworkStream network = client.GetStream();
            StreamReader reader = new StreamReader(network);
            StreamWriter writer = new StreamWriter(network);

            string str = "Nguyen Thi Ha xin chao";
            writer.WriteLine(str);
            writer.Flush();

            try
            {
                while(true)
                {
                    str = reader.ReadLine();
                    if (str.Contains(";"))
                    {
                        str += "\n";
                        File.AppendAllText("../../data.txt", str);

                        writer.WriteLine("Luu thong tin thanh cong");
                        writer.Flush();
                    }
                    else
                    {
                        string fileData = File.ReadAllText("../../data.txt");
                        string[] lines = fileData.Split('\n');

                        bool timra = false;

                        foreach (string line in lines)
                        {
                            if (line.Contains(str))
                            {
                                writer.WriteLine(line);
                                writer.Flush();
                                timra = true;
                                break;
                            }
                        }

                        if (timra==false)
                        {
                            writer.WriteLine("Khong tim thay sinh vien co ma so: " + str);
                            writer.Flush();
                        }
                    }
                  
                }
            }
            catch 
            {
                //throw new Exception("Client da ngat ket noi");            
            }
        }
    }
}

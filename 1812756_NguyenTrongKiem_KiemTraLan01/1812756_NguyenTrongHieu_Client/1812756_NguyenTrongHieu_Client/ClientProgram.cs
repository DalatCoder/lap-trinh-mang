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
    
    public class ClientProgram
    {
        StreamReader reader;
        StreamWriter writer;
        public string Connect()
        {
            TcpClient client = new TcpClient("127.0.0.1", 5000);
            NetworkStream network = client.GetStream();
            reader = new StreamReader(network);
            writer = new StreamWriter(network);

            string str = reader.ReadLine();

            return str;
        }

        public string SendData(string data)
        {
            writer.WriteLine(data);
            writer.Flush();

            string str = reader.ReadLine();

            return str;
        }
    }
}

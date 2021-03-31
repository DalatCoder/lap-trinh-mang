using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [Serializable]
    public enum ServerResponseType
    {
        SendString,
        SendStudentList,
        SendPersonalStudent,
        SendFile,
        BeginExam,
        FinishExam
    }

    [Serializable]
    public class ServerResponseFile
    {
        public FileInfo Info { get; private set; }
        public byte[] FileContent { get; private set; }

        public void SetFile(string filename)
        {
          
            Info = new FileInfo(filename);
            using (FileStream filestream = new FileStream(filename, FileMode.Open))
            {
                MemoryStream memoryStream = new MemoryStream();
                filestream.CopyTo(memoryStream);
                FileContent = memoryStream.ToArray();
            }
        }
    }

    [Serializable]
    public class ServerResponse
    {
        public ServerResponseType Type { get; set; }
        public object Data { get; set; }

        public ServerResponse()
        {

        }

        public ServerResponse(ServerResponseType type, object data)
        {
            Type = type;
            Data = data;
        }
    }
}

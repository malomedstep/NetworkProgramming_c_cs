using System;
using System.IO;
using System.Net;

namespace cs_FtpClient {
    class Program {
        static void Main(string[] args) {
            var request = WebRequest.Create("ftp://localhost/tsetup.1.8.15.exe") as FtpWebRequest;
            request.Method = WebRequestMethods.Ftp.DownloadFile;
           
            var response = request.GetResponse() as FtpWebResponse;
            var stream = response.GetResponseStream();
            
            var fs = new FileStream("telegram_installer.exe", FileMode.Create);
            stream.CopyTo(fs);
            stream.Close();
            fs.Close();

            // FileZilla

            // IIS
        }
    }
}
